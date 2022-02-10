using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Facturador.GHO.Controllers
{
    public class SLExcelStatus
    {
        public string Message { get; set; }
        public bool Success
        {
            get { return string.IsNullOrWhiteSpace(Message); }
        }
    }

    public class ExcelData
    {
        public SLExcelStatus Status { get; set; }
        public Columns ColumnConfigurations { get; set; }
        public List<string> Headers { get; set; }
        public List<List<string>> DataRows { get; set; }
        public string SheetName { get; set; }

        public ExcelData()
        {
            Status = new SLExcelStatus();
            Headers = new List<string>();
            DataRows = new List<List<string>>();
        }
    }
}