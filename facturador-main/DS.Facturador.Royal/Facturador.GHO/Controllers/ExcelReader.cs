using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using System.Xml.Linq;
using System.IO.Compression;
using System.Xml;

namespace Facturador.GHO.Controllers
{
    public class ExcelReader
    {
        private string GetColumnName(string cellReference)
        {
            var regex = new Regex("[A-Za-z]+");
            var match = regex.Match(cellReference);

            return match.Value;
        }

        private int ConvertColumnNameToNumber(string columnName)
        {
            var alpha = new Regex("^[A-Z]+$");
            if (!alpha.IsMatch(columnName)) throw new ArgumentException();

            char[] colLetters = columnName.ToCharArray();
            Array.Reverse(colLetters);

            var convertedValue = 0;
            for (int i = 0; i < colLetters.Length; i++)
            {
                char letter = colLetters[i];
                // ASCII 'A' = 65
                int current = i == 0 ? letter - 65 : letter - 64;
                convertedValue += current * (int)Math.Pow(26, i);
            }

            return convertedValue;
        }

        private IEnumerator<Cell> GetExcelCellEnumerator(Row row)
        {
            int currentCount = 0;
            foreach (Cell cell in row.Descendants<Cell>())
            {
                string columnName = GetColumnName(cell.CellReference);

                int currentColumnIndex = ConvertColumnNameToNumber(columnName);

                for (; currentCount < currentColumnIndex; currentCount++)
                {
                    var emptycell = new Cell()
                    {
                        DataType = null,
                        CellValue = new CellValue(string.Empty)
                    };
                    yield return emptycell;
                }

                yield return cell;
                currentCount++;
            }
        }

        private string ReadExcelCell(Cell cell, WorkbookPart workbookPart)
        {
            var cellValue = cell.CellValue;
            var text = (cellValue == null) ? cell.InnerText : cellValue.Text;
            if ((cell.DataType != null) && (cell.DataType == CellValues.SharedString))
            {
                text = workbookPart.SharedStringTablePart.SharedStringTable
                    .Elements<SharedStringItem>().ElementAt(
                        Convert.ToInt32(cell.CellValue.Text)).InnerText;
            }

            return (text ?? string.Empty).Trim();
        }

        public ExcelData ReadExcel(string file, string sheetName)
        {
            var data = new ExcelData ();

            // Check if the file is excel

            // Open the excel document
            WorkbookPart workbookPart = null;
            List<Row> rows = null;
            try {
                var document = SpreadsheetDocument.Open (file, false);
                workbookPart = document.WorkbookPart;

                var sheets = workbookPart.Workbook.Descendants<Sheet> ();
                var sheet = sheets.Where (s => s.Name == sheetName).FirstOrDefault ();
                data.SheetName = sheet.Name;

                var workSheet = ((WorksheetPart) workbookPart
                    .GetPartById (sheet.Id)).Worksheet;
                var columns = workSheet.Descendants<Columns> ().FirstOrDefault ();
                data.ColumnConfigurations = columns;

                var sheetData = workSheet.Elements<SheetData> ().First ();
                rows = sheetData.Elements<Row> ().ToList ();
            } catch (Exception e) {
                if (e.ToString ().Contains ("Invalid Hyperlink")) {
                    using (FileStream fs = new FileStream (file, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
                        UriFixer.FixInvalidUri (fs, brokenUri => FixUri (brokenUri));
                    }
                    var document = SpreadsheetDocument.Open (file, false);
                    workbookPart = document.WorkbookPart;

                    var sheets = workbookPart.Workbook.Descendants<Sheet> ();
                    var sheet = sheets.Where (s => s.Name == sheetName).FirstOrDefault ();
                    data.SheetName = sheet.Name;

                    var workSheet = ((WorksheetPart) workbookPart
                        .GetPartById (sheet.Id)).Worksheet;
                    var columns = workSheet.Descendants<Columns> ().FirstOrDefault ();
                    data.ColumnConfigurations = columns;

                    var sheetData = workSheet.Elements<SheetData> ().First ();
                    rows = sheetData.Elements<Row> ().ToList ();
                } else {
                    data.Status.Message = "No se puede abrir este archivo";
                    return data;
                }
            }

            // Read the header
            if (rows.Count > 0)
            {
                var row = rows[0];
                var cellEnumerator = GetExcelCellEnumerator(row);
                while (cellEnumerator.MoveNext())
                {
                    var cell = cellEnumerator.Current;
                    var text = ReadExcelCell(cell, workbookPart).Trim();
                    data.Headers.Add(this.RemoveDiacritics(text));
                }
            }

            // Read the sheet data
            if (rows.Count > 1)
            {
                for (var i = 1; i < rows.Count; i++)
                {
                    var dataRow = new List<string>();
                    data.DataRows.Add(dataRow);
                    var row = rows[i];
                    var cellEnumerator = GetExcelCellEnumerator(row);
                    while (cellEnumerator.MoveNext())
                    {
                        var cell = cellEnumerator.Current;
                        var text = ReadExcelCell(cell, workbookPart).Trim();
                        dataRow.Add(text);
                    }
                }
            }

            return data;
        }

        public string RemoveDiacritics(string input)
        {
            string stFormD = input.Normalize(System.Text.NormalizationForm.FormD);
            int len = stFormD.Length;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < len; i++)
            {
                System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stFormD[i]);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[i]);
                }
            }
            
            //return (sb.ToString().Normalize(System.Text.NormalizationForm.FormC));
            return Regex.Replace(sb.ToString().Normalize(System.Text.NormalizationForm.FormC),
                "[^a-zA-Z0-9 ]+", string.Empty).ToLower().Trim();
        }

        private Uri FixUri (string brokenUri)
        {
            return new Uri ("http://broken-link/");
        }

        public static class UriFixer {
            public static void FixInvalidUri (Stream fs, Func<string, Uri> invalidUriHandler)
            {
                XNamespace relNs = "http://schemas.openxmlformats.org/package/2006/relationships";
                using (ZipArchive za = new ZipArchive (fs, ZipArchiveMode.Update)) {
                    foreach (var entry in za.Entries.ToList ()) {
                        if (!entry.Name.EndsWith (".rels"))
                            continue;
                        bool replaceEntry = false;
                        XDocument entryXDoc = null;
                        using (var entryStream = entry.Open ()) {
                            try {
                                entryXDoc = XDocument.Load (entryStream);
                                if (entryXDoc.Root != null && entryXDoc.Root.Name.Namespace == relNs) {
                                    var urisToCheck = entryXDoc
                                        .Descendants (relNs + "Relationship")
                                        .Where (r => r.Attribute ("TargetMode") != null && (string) r.Attribute ("TargetMode") == "External");
                                    foreach (var rel in urisToCheck) {
                                        var target = (string) rel.Attribute ("Target");
                                        if (target != null) {
                                            try {
                                                Uri uri = new Uri (target);
                                            } catch (UriFormatException) {
                                                Uri newUri = invalidUriHandler (target);
                                                rel.Attribute ("Target").Value = newUri.ToString ();
                                                replaceEntry = true;
                                            }
                                        }
                                    }
                                }
                            } catch (XmlException) {
                                continue;
                            }
                        }
                        if (replaceEntry) {
                            var fullName = entry.FullName;
                            entry.Delete ();
                            var newEntry = za.CreateEntry (fullName);
                            using (StreamWriter writer = new StreamWriter (newEntry.Open ()))
                            using (XmlWriter xmlWriter = XmlWriter.Create (writer)) {
                                entryXDoc.WriteTo (xmlWriter);
                            }
                        }
                    }
                }
            }
        }
    }
}