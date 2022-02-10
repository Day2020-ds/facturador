using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using Facturador.GHO.Controllers;

namespace Facturador.GHO.Cliente
{
    public partial class Factura : System.Web.UI.Page
    {
        private Seguridad seg = null;
        private string rutaComprobantes = @"/Facturacion/Empresas/Comprobantes/";
        private string rutaCertificado = @"/Facturacion/Empresas/Certificados/";
        private string fecha;
        private string rfc;
        private string empresa;
        private string identificador;
        private int idemisor;
        private int tipo_pdf;
        private string tipo_documento;
        private string logotipo;
        private string cedula;
        private string origen;
        private string contrato;
        private DateTime? fecha_pago;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                seg = new Seguridad();
                rutaComprobantes = Server.MapPath("~" + System.Configuration.ConfigurationManager.AppSettings["rutaComprobantes"]);
                rutaCertificado = Server.MapPath("~" + System.Configuration.ConfigurationManager.AppSettings["rutaCertificado"]);
                this.UUID.Text = Session["uuid"].ToString();
                this.LlenarInformacion();
            }
            catch(Exception ex)
            {
                ErrorMessage.Text = "No se encontro la factura. " + this.UUID.Text + "  " + ex.Message;
            }
        }

        private void LlenarInformacion()
        {
            try
            {
                using (var db = new DataModel.OstarDB())
                {
                    var factura = (from f in db.factura
                                   join t in db.ticket on f.ticket equals t.idticket
                                   where f.uuid == this.UUID.Text
                                   select new
                                   {
                                       f.uuid,
                                       f.fecha_timbrado,
                                       no_ticket = t.serie + t.folio,
                                       t.subtotal,
                                       t.total,
                                       t.identificador,
                                       nombreEmisor = seg.Desencriptar(t.fkemisor.razon_social),
                                       idemisor = t.fkemisor.idemisor,
                                       logotipo = t.fkemisor.logotipo,
                                       cedula = t.fkemisor.cedula,
                                       rfcEmisor = t.rfc_emisor,
                                       rfcReceptor = t.rfc_receptor,
                                       origen = t.origen,
                                       contrato = t.contrato,
                                       fecha_pago = t.fecha_pago,
                                       tipo_pdf = t.fkemisor.tipo_pdf,
                                       tipo_documento = t.tipo_documento
                                   }).FirstOrDefault();

                    if (factura != null)
                    {
                        this.UUID.Text = factura.uuid;
                        this.fecha = factura.fecha_timbrado.ToString("yyyy-MM");
                        this.Ticket.Text = factura.no_ticket;
                        this.rfc = factura.rfcEmisor;
                        this.identificador = factura.identificador;
                        this.EmisorRFC.Text = rfc;
                        this.empresa = factura.nombreEmisor;
                        this.ReceptorRFC.Text = factura.rfcReceptor;
                        this.Subtotal.Text = factura.subtotal.ToString();
                        this.Total.Text = factura.total.ToString();
                        this.idemisor = factura.idemisor;
                        this.logotipo = factura.logotipo;
                        this.cedula = factura.cedula;
                        this.origen = factura.origen;
                        this.contrato = factura.contrato;
                        this.fecha_pago = factura.fecha_pago;
                        this.tipo_pdf = factura.tipo_pdf == null ? 1 : factura.tipo_pdf.Value;

                        if (factura.tipo_documento != null)
                        {
                            var tipoDoc = db.tipo.Where(x => x.id == factura.tipo_documento.Value).FirstOrDefault();
                            this.tipo_documento = tipoDoc == null ? "FACTURA" : tipoDoc.tipo_comprobante;
                        }
                        else
                            this.tipo_documento = "FACTURA";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = ex.Message;
            }
        }

        protected void DescargarXML(object sender, EventArgs e)
        {
            try
            {
                Byte[] archivo = File.ReadAllBytes(rutaComprobantes + identificador + "/" + fecha + "/" + this.UUID.Text + ".xml");
                Response.Clear();
                Response.AppendHeader("Content-Disposition", "filename=" + this.UUID.Text + ".xml");
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
                if (!File.Exists(rutaComprobantes + identificador + "/" + fecha + "/" + this.UUID.Text + ".pdf"))
                {
                    Reporte.Imprimir imp = new Reporte.Imprimir();
                    string rutaImagen = string.Empty;
                    string rutaCedula = string.Empty;
                    if (!string.IsNullOrEmpty(logotipo))
                        rutaImagen = rutaCertificado + idemisor + "/" + logotipo;
                    if (!string.IsNullOrEmpty(cedula))
                        rutaCedula = rutaCertificado + idemisor + "/" + cedula;
                    imp.Origen = this.origen;
                    imp.Contrato = this.contrato;
                    imp.FechaPago = this.fecha_pago == null ? "" : this.fecha_pago.Value.ToString("yyyy-MM-dd");
                    imp.CrearPDF(rutaComprobantes + identificador + "/" + fecha + "/" + this.UUID.Text + ".xml", rutaComprobantes + identificador + "/" + fecha + "/" + this.UUID.Text + ".pdf", Server.MapPath("~"), rutaImagen, rutaCedula, tipo_pdf, tipo_documento, 0);
                }
                Byte[] archivo = File.ReadAllBytes(rutaComprobantes + identificador + "/" + fecha + "/" + this.UUID.Text + ".pdf");
                Response.Clear();
                Response.AppendHeader("Content-Disposition", "filename=" + this.UUID.Text + ".pdf");
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
                if (!File.Exists(rutaComprobantes + identificador + "/" + fecha + "/" + this.UUID.Text + ".pdf"))
                {
                    Reporte.Imprimir imp = new Reporte.Imprimir();
                    string rutaImagen = string.Empty;
                    string rutaCedula = string.Empty;
                    if (!string.IsNullOrEmpty(logotipo))
                        rutaImagen = rutaCertificado + idemisor + "/" + logotipo;
                    if (!string.IsNullOrEmpty(cedula))
                        rutaCedula = rutaCertificado + idemisor + "/" + cedula;
                    imp.CrearPDF(rutaComprobantes + identificador + "/" + fecha + "/" + this.UUID.Text + ".xml", rutaComprobantes + identificador + "/" + fecha + "/" + this.UUID.Text + ".pdf", Server.MapPath("~"));
                }
                mail.AgregarAdjunto(rutaComprobantes + identificador + "/" + fecha + "/" + this.UUID.Text + ".xml", this.UUID.Text + ".xml");
                
                mail.AgregarAdjunto(rutaComprobantes + identificador + "/" + fecha + "/" + this.UUID.Text + ".pdf", this.UUID.Text + ".pdf");

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

                string asunto = "Factura: " + this.Ticket.Text;

                mail.EnviarCorreo("Factura electrónica " + empresa, asunto, mensaje, true);
                ErrorMessage.Text = "Se ha enviado el correo electrónico";
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = ex.Message;
            }
        }
    }
}