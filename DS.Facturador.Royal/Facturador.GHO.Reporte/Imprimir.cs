using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using ThoughtWorks.QRCode.Codec;
using System.Drawing;
using System.Globalization;
using NumeroLetras;
using Microsoft.Reporting.WebForms;

namespace Facturador.GHO.Reporte
{
    public class Imprimir
    {
        //private IFormatProvider cultura = CultureInfo.GetCultureInfo("es-MX").NumberFormat;
        //private string fmtCFD = "0.00####";
        //private string fmtPDF = "#,##0.00";
        //private string fmtTotalCBB = "0000000000.000000";
        private string origen;
        private string contrato;
        private string fechaPago;

        private byte[] GenerarCodigoBidimensional(XmlDocument xmlCFDi)
        {
            try
            {
                QRCodeEncoder qrCode = new QRCodeEncoder
                {
                    QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE,
                    QRCodeScale = 4,
                    QRCodeVersion = 0,
                    QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L
                };
                //
                StringBuilder cadenaCBB = new StringBuilder();
                //
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlCFDi.NameTable);
                nsmgr.AddNamespace("sat", "http://www.sat.gob.mx/cfd/3");
                //
                cadenaCBB.Append("https://verificacfdi.facturaelectronica.sat.gob.mx/default.aspx?");
                //
                nsmgr.AddNamespace("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");
                XmlNode elemento = xmlCFDi.SelectSingleNode("//sat:Comprobante/sat:Complemento/tfd:TimbreFiscalDigital", nsmgr);
                cadenaCBB.Append(String.Format("&id={0}", elemento.Attributes["UUID"].Value));
                //
                elemento = xmlCFDi.SelectSingleNode("//sat:Comprobante/sat:Emisor", nsmgr);
                cadenaCBB.Append(String.Format("&re={0}", elemento.Attributes["Rfc"].Value));
                //
                elemento = xmlCFDi.SelectSingleNode("//sat:Comprobante/sat:Receptor", nsmgr);
                cadenaCBB.Append(String.Format("&rr={0}", elemento.Attributes["Rfc"].Value));
                //
                elemento = xmlCFDi.SelectSingleNode("//sat:Comprobante", nsmgr);
                cadenaCBB.Append(String.Format("&tt={0}", Convert.ToDouble(elemento.Attributes["Total"].Value).ToString()));
                //
                elemento = xmlCFDi.SelectSingleNode("//sat:Comprobante", nsmgr);
                string sellos = elemento.Attributes["Sello"].Value;
                cadenaCBB.Append(String.Format("&fe={0}", sellos.Substring(sellos.Length - 8, 8)));
                //
                Image cbb = null;
                try
                {
                    cbb = qrCode.Encode(cadenaCBB.ToString());
                }
                catch
                {
                    try
                    {
                        cbb = qrCode.Encode(cadenaCBB.ToString(), Encoding.ASCII);
                    }
                    catch
                    {
                        try
                        {
                            cbb = qrCode.Encode(cadenaCBB.ToString(), Encoding.Unicode);
                        }
                        catch
                        {
                        }
                    }
                }

                MemoryStream scbb = new MemoryStream();
                cbb.Save(scbb, System.Drawing.Imaging.ImageFormat.Jpeg);

                return scbb.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string Origen
        {
            get { return this.origen; }
            set { this.origen = value; }
        }

        public string Contrato
        {
            get { return this.contrato; }
            set { this.contrato = value; }
        }

        public string FechaPago
        {
            get { return this.fechaPago; }
            set { this.fechaPago = value; }
        }

        public void CrearPDF(string rutaXML, string rutaPDF, string rutaReporte)
        {
            this.CrearPDF(rutaXML, rutaPDF, rutaReporte, null, null, 1, "FACTURA", 0);
        }

        public void CrearPDF(string rutaXML, string rutaPDF, string rutaReporte, string rutaLogo)
        {
            this.CrearPDF(rutaXML, rutaPDF, rutaReporte, rutaLogo, null, 1, "FACTURA", 0);
        }

        public void CrearPDF(string rutaXML, string rutaPDF, string rutaReporte, string rutaLogo, string rutaCedula, int tipoPdf, string tipoComprobante, decimal TotalIVA)
        {
            string LeyendaFormaDePago = string.Empty;
            string LeyendaMetodoPago = string.Empty;
            string leyendaRegimen = string.Empty;
            string leyendaUso = string.Empty;
            string totalIVA16 = "0";
            string totalRet001 = "0";
            string totalRet002 = "0";

            NewDataSet ds = null;
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string filenameExtension;
            try
            {
                ds = new NewDataSet
                {
                    EnforceConstraints = false
                };
                XmlDocument xmlDocumento = new XmlDocument();
                xmlDocumento.Load(rutaXML);
                //
                ds.TimbreFiscalDigital.Namespace = "http://www.sat.gob.mx/TimbreFiscalDigital";
                ds.TimbreFiscalDigital.Prefix = "TimbreFiscalDigital";

                ds.ImpuestosLocales.Namespace = "http://www.sat.gob.mx/implocal";
                ds.ImpuestosLocales.Prefix = "ImpuestosLocales";

                ds.Pagos.Namespace = "http://www.sat.gob.mx/Pagos";
                ds.Pagos.Prefix = "pago10";

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocumento.NameTable);
                nsmgr.AddNamespace("cfdi", "http://www.sat.gob.mx/cfd/3");
                nsmgr.AddNamespace("pago10", "http://www.sat.gob.mx/Pagos");
                XmlNode xmlNodo = xmlDocumento.SelectSingleNode("//" + "cfdi" + ":" + "Comprobante", nsmgr);
                 
                XmlNodeList Impuestos = xmlDocumento.SelectNodes("//" + "cfdi" + ":" + "Impuestos", nsmgr);
                foreach(XmlNode impuesto in Impuestos)
                {
                    try
                    {
                        totalIVA16 = impuesto.Attributes["TotalImpuestosTrasladados"].Value;
                        

                    }
                    catch { }
                }

                try
                {
                    XmlNodeList Retenciones = xmlDocumento.SelectNodes("//" + "cfdi" + ":" + "Retencion", nsmgr);
                    foreach(XmlNode retencion in Retenciones)
                    {
                        if(retencion.Attributes["Impuesto"].Value == "002")
                        {
                            totalRet002 = retencion.Attributes["Importe"].Value;
                        }
                        else if(retencion.Attributes["Impuesto"].Value == "001")
                        {
                            totalRet001 = retencion.Attributes["Importe"].Value;
                        }
                    }
                }
                catch { }
                ////UsoCFDi
                XmlNode xmlReceptor = xmlDocumento.SelectSingleNode("//" + "cfdi" + ":" + "Receptor", nsmgr);
                string UsoCfdi = xmlReceptor.Attributes["UsoCFDI"].Value;
                //string leyendaUso;
                switch (UsoCfdi)
                {
                    case "G01":
                        leyendaUso = "G01 - Adquisición de mercancias";
                        break;
                    case "G02":
                        leyendaUso = "G02 - Devoluciones, descuentos o bonificaciones";
                        break;
                    case "G03":
                        leyendaUso = "G03 - Gastos en general";
                        break;
                    case "I01":
                        leyendaUso = "I01 - Construcciones";
                        break;
                    case "I02":
                        leyendaUso = "I02 - Mobilario y equipo de oficina por inversiones";
                        break;
                    case "I03":
                        leyendaUso = "I03 - Equipo de transporte";
                        break;
                    case "I04":
                        leyendaUso = "I04 - Equipo de computo y accesorios";
                        break;
                    case "I05":
                        leyendaUso = "I05 - Dados, troqueles, moldes, matrices y herramental";
                        break;
                    case "I06":
                        leyendaUso = "I06 - Comunicaciones telefónicas";
                        break;
                    case "I07":
                        leyendaUso = "I07 - Comunicaciones satelitales";
                        break;
                    case "I08":
                        leyendaUso = "I08 - Otra maquinaria y equipo";
                        break;
                    case "D01":
                        leyendaUso = "D01 - Honorarios médicos, dentales y gastos hospitalarios.";
                        break;
                    case "D02":
                        leyendaUso = "D02 - Gastos médicos por incapacidad o discapacidad";
                        break;
                    case "D03":
                        leyendaUso = "D03 - Gastos funerales.";
                        break;
                    case "D04":
                        leyendaUso = "D04 - Donativos";
                        break;
                    case "D05":
                        leyendaUso = "D05 - Intereses reales efectivamente pagados por créditos hipotecarios (casa habitación).";
                        break;
                    case "D06":
                        leyendaUso = "D06 - Aportaciones voluntarias al SAR.";
                        break;
                    case "D07":
                        leyendaUso = "D07 - Primas por seguros de gastos médicos.";
                        break;
                    case "D08":
                        leyendaUso = "D08 - Gastos de transportación escolar obligatoria.";
                        break;
                    case "D09":
                        leyendaUso = "D09 - Depósitos en cuentas para el ahorro, primas que tengan como base planes de pensiones.";
                        break;
                    case "D10":
                        leyendaUso = "D10 - Pagos por servicios educativos (colegiaturas)";
                        break;
                    case "P01":
                        leyendaUso = "P01 - Por definir";
                        break;
                    default:
                        leyendaUso = UsoCfdi;
                        break;
                }
                //Regimen
                XmlNode xmlNodoEmisor = xmlDocumento.SelectSingleNode("//" + "cfdi" + ":" + "Emisor", nsmgr);
                string regimen = xmlNodoEmisor.Attributes["RegimenFiscal"].Value;
                //string leyendaRegimen;
                switch (regimen)
                {
                    case "601":
                        leyendaRegimen = "601 - General de Ley Personas Morales";
                        break;
                    case "603":
                        leyendaRegimen = "603 - Personas Morales con Fines no Lucrativos";
                        break;
                    case "605":
                        leyendaRegimen = "605 - Sueldos y Salarios e Ingresos Asimilados a Salarios";
                        break;
                    case "606":
                        leyendaRegimen = "606 - Arrendamiento";
                        break;
                    case "608":
                        leyendaRegimen = "608 - Demás ingresos";
                        break;
                    case "609":
                        leyendaRegimen = "609 - Consolidación";
                        break;
                    case "610":
                        leyendaRegimen = "610 - Residentes en el Extranjero sin Establecimiento Permanente en México";
                        break;
                    case "611":
                        leyendaRegimen = "611 - Ingresos por Dividendos (socios y accionistas)";
                        break;
                    case "612":
                        leyendaRegimen = "612 - Personas Físicas con Actividades Empresariales y Profesionales";
                        break;
                    case "614":
                        leyendaRegimen = "614 - Ingresos por intereses";
                        break;
                    case "616":
                        leyendaRegimen = "616 - Sin obligaciones fiscales";
                        break;
                    case "620":
                        leyendaRegimen = "620 - Sociedades Cooperativas de Producción que optan por diferir sus ingresos";
                        break;
                    case "621":
                        leyendaRegimen = "621 - Incorporación Fiscal";
                        break;
                    case "622":
                        leyendaRegimen = "622 - Actividades Agrícolas, Ganaderas, Silvícolas y Pesqueras";
                        break;
                    case "623":
                        leyendaRegimen = "623 - Opcional para Grupos de Sociedades";
                        break;
                    case "624":
                        leyendaRegimen = "624 - Coordinados";
                        break;
                    case "628":
                        leyendaRegimen = "268 - Hidrocarburos";
                        break;
                    case "607":
                        leyendaRegimen = "607 - Régimen de Enajenación o Adquisición de Bienes";
                        break;
                    case "629":
                        leyendaRegimen = "629 - De los Regímenes Fiscales Preferentes y de las Empresas Multinacionales";
                        break;
                    case "630":
                        leyendaRegimen = "630 - Enajenación de acciones en bolsa de valores";
                        break;
                    case "615":
                        leyendaRegimen = "615 - Régimen de los ingresos por obtención de premios";
                        break;
                    default:
                        leyendaRegimen = regimen;
                        break;
                }
                //Metodo Pago
                try
                {
                    string metodoPago = xmlNodo.Attributes["MetodoPago"].Value;

                    switch (metodoPago)
                    {
                        case "PUE":
                            LeyendaMetodoPago = "PUE - Pago en una sola exhibición";
                            break;
                        case "PPD":
                            LeyendaMetodoPago = "PPD - Pago en parcialidades o diferido";
                            break;
                        default:
                            LeyendaMetodoPago = metodoPago;
                            break;
                    }
                }
                catch { }

                try
                {
                    #region LEER Forma de Pago

                    string[] valorAtributo = xmlNodo.Attributes["FormaPago"].Value.Split(',');
                    for (int i = 0; i < valorAtributo.Count(); i++)
                    {
                        if (i > 0)
                            LeyendaFormaDePago += ", ";
                        switch (valorAtributo[i])
                        {
                            case "01":
                                LeyendaFormaDePago += "01 - Efectivo";
                                break;
                            case "02":
                                LeyendaFormaDePago += "02 - Cheque nominativo";
                                break;
                            case "03":
                                LeyendaFormaDePago += "03 - Transferencia electrónica de fondos";
                                break;
                            case "04":
                                LeyendaFormaDePago += "04 - Tarjeta de Crédito";
                                break;
                            case "05":
                                LeyendaFormaDePago += "05 - Monedero Electrónico";
                                break;
                            case "06":
                                LeyendaFormaDePago += "06 - Dinero electrónico";
                                break;
                            case "08":
                                LeyendaFormaDePago += "08 - Vales de despensa";
                                break;
                            case "12":
                                LeyendaFormaDePago += "12 - Dación en pago";
                                break;
                            case "13":
                                LeyendaFormaDePago += "13 - Pago por subrogación";
                                break;
                            case "14":
                                LeyendaFormaDePago += "14 - Pago por consignación";
                                break;
                            case "15":
                                LeyendaFormaDePago += "15 - Condonación";
                                break;
                            case "17":
                                LeyendaFormaDePago += "17 - Compensación";
                                break;
                            case "23":
                                LeyendaFormaDePago += "23 -Novación";
                                break;
                            case "24":
                                LeyendaFormaDePago += "24 - Confusión";
                                break;
                            case "25":
                                LeyendaFormaDePago += "25 - Remisión de deuda";
                                break;
                            case "26":
                                LeyendaFormaDePago += "26 - Prescripción o caducidad";
                                break;
                            case "27":
                                LeyendaFormaDePago += "27 - A satisfacción del acreedor";
                                break;
                            case "28":
                                LeyendaFormaDePago += "28 - Tarjeta de Débito";
                                break;
                            case "29":
                                LeyendaFormaDePago += "29 - Tarjeta de Servicio";
                                break;
                            case "30":
                                LeyendaFormaDePago += "30 - Aplicación de anticipos";
                                break;
                            case "99":
                                LeyendaFormaDePago += "99 - Por definir";
                                break;
                            default:
                                LeyendaFormaDePago += valorAtributo[i];
                                break;
                        }
                    }
                    #endregion
                }
                catch { }

                //Forma Pago Complemento
                try
                {
                    XmlNode xmlNodeComplemento = xmlDocumento.SelectSingleNode("//" + "pago10" + ":" + "Pago", nsmgr);
                    string formaPagoP = xmlNodeComplemento.Attributes["FormaDePagoP"].Value;
                    switch (formaPagoP)
                    {
                        case "01":
                            LeyendaFormaDePago = "01 - Efectivo";
                            break;
                        case "02":
                            LeyendaFormaDePago = "02 - Cheque nominativo";
                            break;
                        case "03":
                            LeyendaFormaDePago = "03 - Transferencia electrónica de fondos";
                            break;
                        case "04":
                            LeyendaFormaDePago = "04 - Tarjeta de Crédito";
                            break;
                        case "05":
                            LeyendaFormaDePago = "05 - Monedero Electrónico";
                            break;
                        case "06":
                            LeyendaFormaDePago = "06 - Dinero electrónico";
                            break;
                        case "08":
                            LeyendaFormaDePago = "08 - Vales de despensa";
                            break;
                        case "12":
                            LeyendaFormaDePago = "12 - Dación en pago";
                            break;
                        case "13":
                            LeyendaFormaDePago = "13 - Pago por subrogación";
                            break;
                        case "14":
                            LeyendaFormaDePago = "14 - Pago por consignación";
                            break;
                        case "15":
                            LeyendaFormaDePago = "15 - Condonación";
                            break;
                        case "17":
                            LeyendaFormaDePago = "17 - Compensación";
                            break;
                        case "23":
                            LeyendaFormaDePago = "23 -Novación";
                            break;
                        case "24":
                            LeyendaFormaDePago = "24 - Confusión";
                            break;
                        case "25":
                            LeyendaFormaDePago = "25 - Remisión de deuda";
                            break;
                        case "26":
                            LeyendaFormaDePago = "26 - Prescripción o caducidad";
                            break;
                        case "27":
                            LeyendaFormaDePago = "27 - A satisfacción del acreedor";
                            break;
                        case "28":
                            LeyendaFormaDePago = "28 - Tarjeta de Débito";
                            break;
                        case "29":
                            LeyendaFormaDePago = "29 - Tarjeta de Servicio";
                            break;
                        case "30":
                            LeyendaFormaDePago = "30 - Aplicación de anticipos";
                            break;
                        case "99":
                            LeyendaFormaDePago = "99 - Por definir";
                            break;
                        default:
                            LeyendaFormaDePago = formaPagoP;
                            break;
                    }
                }
                catch { }

                ds.ReadXml(new XmlTextReader(new StringReader(xmlDocumento.OuterXml)));

                ReportViewer reportViewer1 = new ReportViewer();
                string pago = string.Empty;
                if (tipoComprobante == "COMPLEMENTO PAGO")
                    pago = "_P";
                switch (tipoPdf)
                {
                    case 1:
                    case 2:
                        reportViewer1.LocalReport.ReportPath = Path.Combine(rutaReporte, @"bin\CFDi" + pago + ".rdlc");
                        break;
                    case 3:
                    case 4:
                        reportViewer1.LocalReport.ReportPath = Path.Combine(rutaReporte, @"bin\CFDi2" + pago + ".rdlc");
                        break;
                    case 5:
                        reportViewer1.LocalReport.ReportPath = Path.Combine(rutaReporte, @"bin\CFDi3" + pago + ".rdlc");
                        break;
                    case 6:
                        reportViewer1.LocalReport.ReportPath = Path.Combine(rutaReporte, @"bin\CFDi4" + pago + ".rdlc");
                        break;
                    case 7:
                        reportViewer1.LocalReport.ReportPath = Path.Combine(rutaReporte, @"bin\CFDi5" + pago + ".rdlc");
                        break;
                    case 8:
                        reportViewer1.LocalReport.ReportPath = Path.Combine(rutaReporte, @"bin\CFDi6" + pago + ".rdlc");
                        break;
                    case 9:
                        reportViewer1.LocalReport.ReportPath = Path.Combine(rutaReporte, @"bin\CFDi7" + pago + ".rdlc");
                        break;
                    case 10:
                        reportViewer1.LocalReport.ReportPath = Path.Combine(rutaReporte, @"bin\CFDi8" + pago + ".rdlc");
                        break;
                    default:
                        reportViewer1.LocalReport.ReportPath = Path.Combine(rutaReporte, @"bin\CFDi" + pago + ".rdlc");
                        break;
                }
                reportViewer1.LocalReport.DataSources.Clear();
                byte[] cbb = GenerarCodigoBidimensional(xmlDocumento);
                ds.Comprobante[0].cbb = cbb;

                if (!string.IsNullOrEmpty(rutaLogo))
                {
                    byte[] imagen = File.ReadAllBytes(rutaLogo);
                    ds.Emisor[0].logo = imagen;
                }

                if (!string.IsNullOrEmpty(rutaCedula))
                {
                    byte[] imagen = File.ReadAllBytes(rutaCedula);
                    ds.Emisor[0].cedula = imagen;
                }

                ds.Comprobante[0].FechaPago = this.fechaPago;
                ds.Comprobante[0].Origen = this.origen;
                ds.Comprobante[0].Contrato = this.contrato;
                ds.Comprobante[0].TipoDocumento = tipoComprobante;

                //string NumeroLetras = clsNumeroLetras.NumeroLetras(Convert.ToDouble(ds.Comprobante[0].total), clsNumeroLetras.Idioma.Español, "PESOS", "M.N.");
                
                string NumeroLetras = string.Empty;
                //if (ds.Comprobante[0].IsMonedaNull())
                //    NumeroLetras = clsNumeroLetras.NumeroLetras(Convert.ToDouble(ds.Comprobante[0].Total), clsNumeroLetras.Idioma.Español, "PESOS", "M.N.");
                if (ds.Comprobante[0].IsTipoCambioNull())
                    NumeroLetras = clsNumeroLetras.NumeroLetras(Convert.ToDouble(ds.Comprobante[0].Total), clsNumeroLetras.Idioma.Español, "PESOS", "M.N.");
                else if (ds.Comprobante[0].Moneda == "MXN")
                    NumeroLetras = clsNumeroLetras.NumeroLetras(Convert.ToDouble(ds.Comprobante[0].Total), clsNumeroLetras.Idioma.Español, "PESOS", "M.N.");
                else if (ds.Comprobante[0].Moneda == "USD")
                    NumeroLetras = clsNumeroLetras.NumeroLetras(Convert.ToDouble(ds.Comprobante[0].Total), clsNumeroLetras.Idioma.Ingles, "DOLLARS", "U.S.D.");
                else if (ds.Comprobante[0].Moneda == "EUR")
                    NumeroLetras = clsNumeroLetras.NumeroLetras(Convert.ToDouble(ds.Comprobante[0].Total), clsNumeroLetras.Idioma.Español, "EUROS", "EUR");
                else
                    NumeroLetras = clsNumeroLetras.NumeroLetras(Convert.ToDouble(ds.Comprobante[0].Total), clsNumeroLetras.Idioma.Español, "", ds.Comprobante[0].Moneda);


                //reportViewer1.LocalReport.SetParameters(new ReportParameter ("importeLetra", NumeroLetras));
                ds.Comprobante[0].ImporteLetra = NumeroLetras;



                ReportDataSource rpds1 = new ReportDataSource
                {
                    Name = "Emisor",
                    Value = ds.Emisor
                };
                reportViewer1.LocalReport.DataSources.Add(rpds1);

                rpds1 = new ReportDataSource
                {
                    Name = "Receptor",
                    Value = ds.Receptor
                };
                reportViewer1.LocalReport.DataSources.Add(rpds1);

                rpds1 = new ReportDataSource
                {
                    Name = "Comprobante",
                    Value = ds.Comprobante
                };
                reportViewer1.LocalReport.DataSources.Add(rpds1);

                rpds1 = new ReportDataSource
                {
                    Name = "Concepto",
                    Value = ds.Concepto
                };
                reportViewer1.LocalReport.DataSources.Add(rpds1);

                rpds1 = new ReportDataSource
                {
                    Name = "CuentaPredial",
                    Value = ds.CuentaPredial
                };
                reportViewer1.LocalReport.DataSources.Add(rpds1);

                //rpds1 = new ReportDataSource();
                //rpds1.Name = "InformacionAduanera";
                //rpds1.Value = ds.InformacionAduanera;
                //reportViewer1.LocalReport.DataSources.Add(rpds1);

                rpds1 = new ReportDataSource
                {
                    Name = "TimbreFiscalDigital",
                    Value = ds.TimbreFiscalDigital
                };
                reportViewer1.LocalReport.DataSources.Add(rpds1);

                rpds1 = new ReportDataSource
                {
                    Name = "Traslado",
                    Value = ds.Traslado
                };
                reportViewer1.LocalReport.DataSources.Add(rpds1);

                rpds1 = new ReportDataSource
                {
                    Name = "Retencion",
                    Value = ds.Retencion
                };
                reportViewer1.LocalReport.DataSources.Add(rpds1);

                rpds1 = new ReportDataSource
                {
                    Name = "RetencionesLocales",
                    Value = ds.RetencionesLocales
                };
                reportViewer1.LocalReport.DataSources.Add(rpds1);

                rpds1 = new ReportDataSource
                {
                    Name = "TrasladosLocales",
                    Value = ds.TrasladosLocales
                };
                reportViewer1.LocalReport.DataSources.Add(rpds1);

                //Pagos
                rpds1 = new ReportDataSource
                {
                    Name = "Pago",
                    Value = ds.Pago
                };
                reportViewer1.LocalReport.DataSources.Add(rpds1);

                rpds1 = new ReportDataSource
                {
                    Name = "DoctoRelacionado",
                    Value = ds.DoctoRelacionado
                };
                reportViewer1.LocalReport.DataSources.Add(rpds1);

                //// Paso de parametro
                
                ReportParameter reporteParametro2 = new ReportParameter("LeyendaRegimen", leyendaRegimen);
                ReportParameter reporteParametro4 = new ReportParameter("UsoCfdi", leyendaUso);
                ReportParameter reporteParametro = new ReportParameter("LeyendaFormaDePago", LeyendaFormaDePago);

                reportViewer1.LocalReport.SetParameters(reporteParametro);
                reportViewer1.LocalReport.SetParameters(reporteParametro2);
                reportViewer1.LocalReport.SetParameters(reporteParametro4);
                
                
                if (tipoComprobante != "COMPLEMENTO PAGO")
                {
                    if (tipoPdf == 5 || tipoPdf == 7 || tipoPdf == 6)
                    {
                        ReportParameter reporteParametro5 = new ReportParameter("TotalRet001", totalRet001);
                        ReportParameter reporteParametro6 = new ReportParameter("TotalRet002", totalRet002);
                        reportViewer1.LocalReport.SetParameters(reporteParametro5);
                        reportViewer1.LocalReport.SetParameters(reporteParametro6);
                    }

                    ReportParameter reporteParametro1 = new ReportParameter("LeyendaMetodoDePago", LeyendaMetodoPago);
                    ReportParameter reporteParametro3 = new ReportParameter("TotalTraslados", totalIVA16);
                    reportViewer1.LocalReport.SetParameters(reporteParametro1);
                    reportViewer1.LocalReport.SetParameters(reporteParametro3);
                }
                reportViewer1.LocalReport.Refresh();

                byte[] bytes = reportViewer1.LocalReport.Render(
                    "PDF", null, out mimeType, out encoding, out filenameExtension,
                    out streamids, out warnings);

                using (FileStream fs = new FileStream(rutaPDF, FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
            catch (Exception ex)
            {
                bool inExc = true;
                string error = string.Empty;
                while (inExc)
                {
                    error += ex.Message + ". ";
                    if (ex.InnerException != null)
                        ex = ex.InnerException;
                    else
                        inExc = false;
                }
                throw new Exception(error);
            }
            finally
            {
                if (ds != null)
                    ds.Dispose();
            }
        }
    }
}
