using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Facturador.GHO.Controllers;
using System.IO;

namespace Facturador.GHO.Cliente
{
    public partial class Comprobante : System.Web.UI.Page
    {
        private string rutaComprobantes = @"/Facturacion/Empresas/Comprobantes/";
        private string folioFiscal = string.Empty;
        private string fecha = string.Empty;
        private string rfc = string.Empty;
        private string empresa = string.Empty;
        private Seguridad seg;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                seg = new Seguridad();
             //   rutaComprobantes = System.Configuration.ConfigurationManager.AppSettings["rutaComprobantes"];
                rutaComprobantes = Server.MapPath("~" + System.Configuration.ConfigurationManager.AppSettings["rutaComprobantes"]);
                folioFiscal = Session["uuid"].ToString().ToUpper();
                LlenarInformacion2(folioFiscal);
            }
            catch(Exception ex) {
                ErrorMessage.Text = "No se encontro el comprobante fiscal " + (folioFiscal ?? "") + ". " + ex.Message;
            }            
        }

        protected void DescargarXML(object sender, EventArgs e)
        {
            try
            {
                Byte[] archivo = File.ReadAllBytes(rutaComprobantes + rfc + "/" + fecha + "/" + folioFiscal + ".xml");
                Response.Clear();
                Response.AppendHeader("Content-Disposition", "filename=" + folioFiscal + ".xml");
                Response.ContentType = "application/octet-stream";
                Response.BinaryWrite(archivo);
                Response.End();
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = ex.Message;
            }
        }

        protected void DescargarPDF(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists(rutaComprobantes + rfc + "/" + fecha + "/" + folioFiscal + ".pdf"))
                {
                    Reporte.Imprimir imp = new Reporte.Imprimir();
                    imp.CrearPDF(rutaComprobantes + rfc + "/" + fecha + "/" + folioFiscal + ".xml", rutaComprobantes + rfc + "/" + fecha + "/" + folioFiscal + ".pdf", Server.MapPath("~"));
                }
                Byte[] archivo = File.ReadAllBytes(rutaComprobantes + rfc + "/" + fecha + "/" + folioFiscal + ".pdf");
                Response.Clear();
                Response.AppendHeader("Content-Disposition", "filename=" + folioFiscal + ".pdf");
                Response.AppendHeader("Content-Length", archivo.Length.ToString());
                Response.ContentType = "application/octet-stream";
                Response.BinaryWrite(archivo);
                Response.End();
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = ex.Message;
            }
        }

        protected void EnviarCorreo(object sender, EventArgs e)
        {
            try
            {
                CorreoElectronico mail = new CorreoElectronico();
                mail.AgregarDestinatario(this.Correo.Text);
                mail.AgregarAdjunto(rutaComprobantes + rfc + "/" + fecha + "/" + folioFiscal + ".xml", folioFiscal + ".xml");
                if (!File.Exists(rutaComprobantes + rfc + "/" + fecha + "/" + folioFiscal + ".pdf"))
                {
                    Reporte.Imprimir imp = new Reporte.Imprimir();
                    imp.CrearPDF(rutaComprobantes + rfc + "/" + fecha + "/" + folioFiscal + ".xml", rutaComprobantes + rfc + "/" + fecha + "/" + folioFiscal + ".pdf", Server.MapPath("~"));
                }
                mail.AgregarAdjunto(rutaComprobantes + rfc + "/" + fecha + "/" + folioFiscal + ".pdf", folioFiscal + ".pdf");

                string mensaje = "<tr>" +
                    "<td style=\"width:157px;height:157px;\"></td>" +
                    "<td style=\"height:157px;\">" +
                    "<p style=\"text-align:left;font-size:13px;color:#0b6dc5;direction:ltr;font-family:Arial;font-variant:normal;font-weight:normal;\">" + "" +
                    "<font size=\"3\"><strong>Estimado Cliente: </strong></font><br>" +
                    "<br>" +
                    "Les hacemos llegar su comprobante fiscal." +
                    "<br>" +
                    "Favor de no responder a este correo. Los correos son enviados por un programa automático." +
                    "<br>" +
                    "</p>" +
                    "</td>" +
                    "</tr>";

                string asunto = "Factura: " + empresa;

                mail.EnviarCorreo(asunto, mensaje, true);
                ErrorMessage.Text = "Se ha enviado el correo electrónico";
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = ex.Message;
            }
        }

        private class InfoComprobante
        {
            public string uuid { get; set; }
            public DateTime fecha_timbrado { get; set; }
            public string no_ticket { get; set; }
            public decimal subtotal { get; set; }
            public decimal total { get; set; }
            public string nombreEmisor { get; set; }
            public string nombreReceptor { get; set; }
            public string rfcEmisor { get; set; }
            public string rfcReceptor { get; set; }
        }

        private void LlenarInformacion2(string uuid)
        {
            try
            {
                bool existe = false;
                string uuid2 = string.Empty;
                using (var db = new DataModel.OstarDB())
                {
                    var factura =  db.factura
                        .Where(f => f.uuid == this.UUID.Text).FirstOrDefault();

                    if (factura != null)
                    {
                        existe = true;
                        uuid2 = factura.uuid;
                    }
                }
                if (existe)
                {
                    UUID.Text = uuid2;
                }
                else
                {
                    ErrorMessage.Text = "No existen la factura con el folio fiscal indicado";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = ex.Message;
            }

        }

        private void LlenarInformacion(string uuid)
        {
            try
            {
                using (var db = new DataModel.OstarDB())
                {
                    //var query = (from f in db.factura
                    //     join t in db.ticket on f.ticket equals t.idticket
                    //     where f.uuid == folioFiscal
                    //     select new
                    //     {
                    //         f.uuid,
                    //         f.fecha_timbrado,
                    //         t.no_ticket,
                    //         t.subtotal,
                    //         t.total,
                    //         nombreEmisor = seg.Desencriptar(t.fkemisor.razon_social),
                    //         rfcEmisor = seg.Desencriptar(t.fkemisor.rfc),
                    //         nombreReceptor = seg.Desencriptar(f.fkreceptor.razon_social),
                    //         rfcReceptor = seg.Desencriptar(f.fkreceptor.rfc)
                    //     }
                    //     ).FirstOrDefault();

                    string sel = "SELECT f.uuid " +
                                    ",f.fecha_timbrado " +
                                    ",t.no_ticket " +
                                    ",e.rfc as rfcEmisor " +
                                    ",e.razon_social as nombreEmisor " +
                                    ",r.rfc as rfcReceptor " +
                                    ",r.razon_social as nombreReceptor " +
                                    ",t.subtotal " +
                                    ",t.total " +
                                    "from factura f " +
                                    "join ticket t on f.ticket = t.idticket " +
                                    "join emisor e on t.emisor = e.idemisor " +
                                    "join receptor r on f.receptor = r.idreceptor " +
                                    "where UPPER(f.uuid) = '" + uuid + "'";

                    List<InfoComprobante> info = db.SetCommand(sel).ExecuteList<InfoComprobante>();
                    InfoComprobante query = info.FirstOrDefault();

                    if (string.IsNullOrWhiteSpace(query.uuid))
                    {
                        var factura = db.factura
                            .Where(f => f.uuid == uuid).FirstOrDefault();
                        if(factura!=null)
                        {
                            sel += "; " + factura.uuid;
                        }
                        throw new Exception(sel);
                    }
                    this.UUID.Text = query.uuid;
                    this.fecha = query.fecha_timbrado.ToString("yyyy-MM");
                    this.Ticket.Text = query.no_ticket;
                    this.rfc = string.IsNullOrWhiteSpace(query.rfcEmisor) ? "" : seg.Desencriptar(query.rfcEmisor);
                    this.EmisorRFC.Text = rfc;
                    this.empresa = string.IsNullOrWhiteSpace(query.nombreEmisor) ? "" : seg.Desencriptar(query.nombreEmisor);
                    this.ReceptorRFC.Text = string.IsNullOrWhiteSpace(query.rfcReceptor) ? "" : seg.Desencriptar(query.rfcReceptor);
                    this.Subtotal.Text = query.subtotal.ToString();
                    this.Total.Text = query.total.ToString();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}