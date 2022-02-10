using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Facturador.GHO.Controllers;
using System.IO;
using Microsoft.AspNet.Identity;
using Daysoft.DICI.Facturador.DFacture;
using BLToolkit.Data.Linq;
using Ionic.Zip;

namespace Facturador.GHO.Admin
{
    public partial class Consulta : System.Web.UI.Page
    {
        private Seguridad seg;
        private License lic;
        string rutaLicencia = string.Empty;
        private string rutaComprobantes = @"/Facturacion/Empresas/Comprobantes/";
        private string rutaComprobantesRIP = @"/Facturacion/Empresas/ComprobantesRIP/";
        private string rutaArchivosTemporales = @"/ArchivosTemporales/";
        private string rutaCertificado = @"/Facturacion/Empresas/Certificados/";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                seg = new Seguridad();
                rutaComprobantes = Server.MapPath("~" + System.Configuration.ConfigurationManager.AppSettings["rutaComprobantes"]);
                rutaComprobantesRIP = Server.MapPath("~" + System.Configuration.ConfigurationManager.AppSettings["rutaComprobantesRIP"]);
                rutaCertificado = Server.MapPath("~" + System.Configuration.ConfigurationManager.AppSettings["rutaCertificado"]);
                LeerLicencia();
                if (!IsPostBack)
                {
                    LlenarEmpresas();
                    LlenarTipoComprobante();
                    LlenarMetodoPago();
                    Llenartipocancelacion();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = ex.Message;
            }
        }

        protected void LeerLicencia()
        {
            rutaLicencia = Server.MapPath("~/Facturacion/Licencia/");
            if (File.Exists(rutaLicencia + "licencia.vali"))
                lic = new License(rutaLicencia + "licencia.vali");
            else
                lic = new License();
        }

        protected void LlenarEmpresas()
        {
            using (var db = new DataModel.OstarDB())
            {
                Dictionary<string, string> emp = new Dictionary<string, string>();
                if (User.IsInRole("Admin") || User.IsInRole("Super"))
                {
                    emp = (from h in db.emisor
                           select new
                           {
                               razon_social = seg.Desencriptar(h.razon_social),
                               rfc = h.idemisor.ToString()
                           }).Take(lic.ObtenerEmpresas()).ToDictionary(t => t.razon_social, t => t.rfc);
                }
                else
                {
                    List<int> emisoresActuales = (from e in db.emisor
                                                  join ue in db.useremisor on e.idemisor equals ue.EmisorId
                                                  where ue.UserId == User.Identity.GetUserId()
                                                  select e.idemisor).ToList();
                    emp = (from h in db.emisor
                           where emisoresActuales.Contains(h.idemisor)
                           select new
                           {
                               razon_social = seg.Desencriptar(h.razon_social),
                               rfc = h.idemisor.ToString()
                           }).Take(lic.ObtenerEmpresas()).ToDictionary(t => t.razon_social, t => t.rfc);
                }
               // emp.Add("---------------", "0");


                DDEmisor.DataValueField = "Value";
                DDEmisor.DataTextField = "Key";
                DDEmisor.DataSource = emp.Reverse();
                DDEmisor.DataBind();
            }
        }

        protected void LlenarTipoComprobante()
        {
            DDTipoComprobante.Items.Add(new ListItem("Factura", "1"));
            DDTipoComprobante.Items.Add(new ListItem("Pago", "2"));
            DDTipoComprobante.Items.Add(new ListItem("Nota de Credito", "3"));
            //DDTipoComprovante.Items.Add(new ListItem("CFDI RIP", "2"));

        }
        protected void Llenartipocancelacion()
        {
            DDTipocancelacion.Items.Add(new ListItem("01 - Comprobante emitido con errores y con relación", "01"));
            DDTipocancelacion.Items.Add(new ListItem("02 - Comprobante emitido con errores y sin relación", "02"));
            DDTipocancelacion.Items.Add(new ListItem("03 - No se llevó acabo la operación", "03"));
            DDTipocancelacion.Items.Add(new ListItem("04 - Operación nominativa relacionada en una factura global", "04"));
        }

        protected void LlenarMetodoPago()
        {
            DDMetodoPago.Items.Add(new ListItem("-----------------------", ""));
            DDMetodoPago.Items.Add(new ListItem("Pago en Parcialidades o Diferido", "PPD"));
            DDMetodoPago.Items.Add(new ListItem("Pago en una Sola Exhibición", "PUE"));
        }

        protected void Consultar(object sender, EventArgs e)
        {
            viewcfdirip.PageIndex = 0;
            viewFacturas.PageIndex = 0;
            ObtenerComprobantes();
        }

        protected void Limpiar(object sender, EventArgs e)
        {
           // this.DDEmisor.Text = "0";
            this.DDTipoComprobante.Text = "1";
            this.DDMetodoPago.Text = "PPD";
          //  this.RFCReceptor.Text = string.Empty;
            this.FechaInicial.Text = string.Empty;
            this.FechaFinal.Text = string.Empty;
            this.UUID.Text = string.Empty;
        }

        private void ObtenerComprobantes()
        {
            try
            {
                using (var db = new DataModel.OstarDB())
                {
                    List<int> emisoresActuales = new List<int>();
                    if (User.IsInRole("Admin") || User.IsInRole("Super"))
                    {
                        emisoresActuales = (from e in db.emisor
                                            select e.idemisor).ToList();
                    }
                    else
                    {
                        emisoresActuales = (from e in db.emisor
                                            join ue in db.useremisor on e.idemisor equals ue.EmisorId
                                            where ue.UserId == User.Identity.GetUserId()
                                            select e.idemisor).ToList();
                    }
                    string meto = string.Empty;
                    if (DDMetodoPago.SelectedValue != "")
                    {
                        meto = DDMetodoPago.SelectedValue;
                    }
                     //= DDMetodoPago.SelectedValue;
                    int idEmisor = Convert.ToInt32(DDEmisor.Text);
                    if (DDTipoComprobante.SelectedValue.Equals("1"))
                    {
                        var query2 = (from f in db.factura
                                      join t in db.ticket on f.ticket equals t.idticket
                                      where f.fecha_timbrado >= (string.IsNullOrWhiteSpace(FechaInicial.Text) ? DateTime.Now.AddMonths(-1) : DateTime.ParseExact(FechaInicial.Text.Trim(), "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture))
                                         && f.fecha_timbrado < (string.IsNullOrWhiteSpace(FechaFinal.Text) ? DateTime.Now.AddDays(1) : DateTime.ParseExact(FechaFinal.Text.Trim(), "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture).AddDays(1))
                                         && f.uuid.Contains(UUID.Text) && emisoresActuales.Contains(t.emisor) && (idEmisor == 0 ? true : t.emisor == idEmisor) && t.tipo_comprobante.ToUpper().Contains("I")
                                         && (DDMetodoPago.SelectedValue == "" ? true : t.metodo_pago == DDMetodoPago.SelectedValue)
                                      orderby f.fecha_timbrado ascending
                                      select new
                                      {
                                          idEmisor = t.emisor,
                                          uuid = f.uuid,
                                          serie = t.serie + t.folio,
                                          fecha_timbrado = f.fecha_timbrado,
                                          emisor = t.rfc_emisor,
                                          receptor = t.rfc_receptor,
                                          subtotal = t.subtotal,
                                          iva = t.fktraslados.Where(x => x.impuesto == "IVA").Sum(x => x.importe),
                                          ieps = t.fktraslados.Where(x => x.impuesto == "IEPS").Sum(x => x.importe),
                                          ish = t.fktrasladoslocales.Sum(x => x.importe),
                                          isn = t.fkretencioneslocales.Sum(x => x.importe),
                                          ret_iva = t.fkretenciones.Where(x => x.impuesto == "IVA").Sum(x => x.importe),
                                          ret_isr = t.fkretenciones.Where(x => x.impuesto == "ISR").Sum(x => x.importe),
                                          total = t.total,
                                          tipo = t.tipo_comprobante,
                                          estatus = t.estatus
                                      });
                        viewcfdirip.DataSource = null;
                        viewcfdirip.DataBind();
                        viewFacturas.DataSource = query2.ToList();
                        viewFacturas.DataBind();
                        Session["DDTipoComprovante"] = DDTipoComprobante.SelectedValue;
                        Session["DDMetodoPago"] = DDMetodoPago.SelectedValue;
                    }
                    else if (DDTipoComprobante.SelectedValue.Equals("3"))
                    {
                        var query4 = (from f in db.factura
                                      join t in db.ticket on f.ticket equals t.idticket
                                      where f.fecha_timbrado >= (string.IsNullOrWhiteSpace(FechaInicial.Text) ? DateTime.Now.AddMonths(-1) : DateTime.ParseExact(FechaInicial.Text.Trim(), "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture))
                                         && f.fecha_timbrado < (string.IsNullOrWhiteSpace(FechaFinal.Text) ? DateTime.Now.AddDays(1) : DateTime.ParseExact(FechaFinal.Text.Trim(), "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture).AddDays(1))
                                         && f.uuid.Contains(UUID.Text) && emisoresActuales.Contains(t.emisor) && (idEmisor == 0 ? true : t.emisor == idEmisor) && t.tipo_comprobante.ToUpper().Contains("E")
                                         && (DDMetodoPago.SelectedValue == "" ? true : t.metodo_pago == DDMetodoPago.SelectedValue)
                                      orderby f.fecha_timbrado ascending
                                      select new
                                      {
                                          idEmisor = t.emisor,
                                          uuid = f.uuid,
                                          serie = t.serie + t.folio,
                                          fecha_timbrado = f.fecha_timbrado,
                                          emisor = t.rfc_emisor,
                                          receptor = t.rfc_receptor,
                                          subtotal = t.subtotal,
                                          iva = t.fktraslados.Where(x => x.impuesto == "IVA").Sum(x => x.importe),
                                          ieps = t.fktraslados.Where(x => x.impuesto == "IEPS").Sum(x => x.importe),
                                          ish = t.fktrasladoslocales.Sum(x => x.importe),
                                          isn = t.fkretencioneslocales.Sum(x => x.importe),
                                          ret_iva = t.fkretenciones.Where(x => x.impuesto == "IVA").Sum(x => x.importe),
                                          ret_isr = t.fkretenciones.Where(x => x.impuesto == "ISR").Sum(x => x.importe),
                                          total = t.total,
                                          tipo = t.tipo_comprobante,
                                          estatus = t.estatus
                                      });
                        viewcfdirip.DataSource = null;
                        viewcfdirip.DataBind();
                        viewFacturas.DataSource = query4.ToList();
                        viewFacturas.DataBind();
                        Session["DDTipoComprovante"] = DDTipoComprobante.SelectedValue;
                        Session["DDMetodoPago"] = DDMetodoPago.SelectedValue;
                    }
                    else
                    {
                        var query3 = (from f in db.factura
                                      join t in db.ticket on f.ticket equals t.idticket
                                      where f.fecha_timbrado >= (string.IsNullOrWhiteSpace(FechaInicial.Text) ? DateTime.Now.AddMonths(-1) : DateTime.ParseExact(FechaInicial.Text.Trim(), "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture))
                                         && f.fecha_timbrado < (string.IsNullOrWhiteSpace(FechaFinal.Text) ? DateTime.Now.AddDays(1) : DateTime.ParseExact(FechaFinal.Text.Trim(), "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture).AddDays(1))
                                         && f.uuid.Contains(UUID.Text) && emisoresActuales.Contains(t.emisor) && (idEmisor == 0 ? true : t.emisor == idEmisor) && t.tipo_comprobante.ToUpper().Contains("P")
                                      orderby f.fecha_timbrado ascending
                                      select new
                                      {
                                          idEmisor = t.emisor,
                                          uuid = f.uuid,
                                          serie = t.serie + t.folio,
                                          fecha_timbrado = f.fecha_timbrado,
                                          emisor = t.rfc_emisor,
                                          receptor = t.rfc_receptor,
                                          subtotal = t.subtotal,
                                          iva = t.fktraslados.Where(x => x.impuesto == "IVA").Sum(x => x.importe),
                                          ieps = t.fktraslados.Where(x => x.impuesto == "IEPS").Sum(x => x.importe),
                                          ish = t.fktrasladoslocales.Sum(x => x.importe),
                                          isn = t.fkretencioneslocales.Sum(x => x.importe),
                                          ret_iva = t.fkretenciones.Where(x => x.impuesto == "IVA").Sum(x => x.importe),
                                          ret_isr = t.fkretenciones.Where(x => x.impuesto == "ISR").Sum(x => x.importe),
                                          total = t.total,
                                          tipo = t.tipo_comprobante,
                                          estatus = t.estatus
                                      });
                        viewFacturas.DataSource = null;
                        viewFacturas.DataBind();
                        viewFacturas.DataSource = query3.ToList();
                        viewFacturas.DataBind();
                        Session["DDTipoComprovante"] = DDTipoComprobante.SelectedValue;
                        Session["DDMetodoPago"] = DDMetodoPago.SelectedValue;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        protected void viewFacturas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ObtenerComprobantes();
            viewFacturas.PageIndex = e.NewPageIndex;
            viewFacturas.DataBind();
            //if (DDTipoComprovante.SelectedValue.Equals("1") || DDMetodoPago.SelectedValue.Equals("PPD"))
            //{
            //    viewFacturas.PageIndex = e.NewPageIndex;
            //    viewFacturas.DataBind();
            //}
            //else
            //{
            //    viewcfdirip.PageIndex = e.NewPageIndex;
            //    viewcfdirip.DataBind();
            //}

        }
        protected void viewFacturas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DescargarXML")
            {
                string id = viewFacturas.DataKeys[Convert.ToInt32(e.CommandArgument)].Value.ToString();
                DescargarXML(id);
            }
            else if (e.CommandName == "DescargarPDF")
            {
                string id = viewFacturas.DataKeys[Convert.ToInt32(e.CommandArgument)].Value.ToString();
                DescargarPDF(id);
            }
            else if (e.CommandName == "Ver")
            {
                string id = viewFacturas.DataKeys[Convert.ToInt32(e.CommandArgument)].Value.ToString();

                Session.Add("uuid", id);

                Response.Redirect("~/Cliente/Factura");
                Response.End();
            }
            else if (e.CommandName == "Cancelar")
            {
                string id = viewFacturas.DataKeys[Convert.ToInt32(e.CommandArgument)].Value.ToString();
                CancelarXML(id);
            }
        }

        protected void viewCFDIRIP_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DescargarXML")
            {
                string id = viewcfdirip.DataKeys[Convert.ToInt32(e.CommandArgument)].Value.ToString();
                DescargarXML(id);
            }
            else if (e.CommandName == "DescargarPDF")
            {
                string id = viewcfdirip.DataKeys[Convert.ToInt32(e.CommandArgument)].Value.ToString();
                DescargarPDF(id);
            }
            else if (e.CommandName == "Ver")
            {
                string id = viewcfdirip.DataKeys[Convert.ToInt32(e.CommandArgument)].Value.ToString();

                Session.Add("uuid", id);

                Response.Redirect("~/Cliente/Factura");
                Response.End();
            }
            else if (e.CommandName == "Cancelar")
            {
                string id = viewcfdirip.DataKeys[Convert.ToInt32(e.CommandArgument)].Value.ToString();
                CancelarXML(id);
            }
        }

        protected void DescargarXML(string id)
        {
            try
            {
                if (Session["DDTipoComprovante"].Equals("1") || DDMetodoPago.SelectedValue.Equals("PPD") || Session["DDTipoComprovante"].Equals("2") || Session["DDTipoComprovante"].Equals("3"))
                {
                    using (var db = new DataModel.OstarDB())
                    {
                        var query =
                            (from f in db.factura
                             join t in db.ticket on f.ticket equals t.idticket
                             join e in db.emisor on t.emisor equals e.idemisor
                             where f.uuid == id
                             select new
                             {
                                 f.uuid,
                                 f.fecha_timbrado,
                                 emisor = seg.Desencriptar(e.rfc),
                                 identificador = t.identificador // seg.Desencriptar(e.identificador)
                             }).FirstOrDefault();
                        string rutaArchivo = rutaComprobantes + query.identificador + "/" + query.fecha_timbrado.ToString("yyyy-MM") + "/" + query.uuid;

                        if (!Directory.Exists(rutaArchivo.Replace("/" + query.uuid, "")))
                            Directory.CreateDirectory(rutaArchivo.Replace("/" + query.uuid, ""));

                        if (!File.Exists(rutaArchivo + ".xml"))
                        {
                            DSTimbrado timbre = new DSTimbrado();
                            timbre.RecuperarXML(query.uuid.ToUpper(), rutaArchivo.Replace("/" + query.uuid, ""));
                        }

                        if (File.Exists(rutaArchivo + ".xml"))
                        {
                            Byte[] archivo = File.ReadAllBytes(rutaArchivo + ".xml");
                            Response.Clear();
                            Response.AppendHeader("Content-Disposition", "filename=" + query.uuid + ".xml");
                            Response.AppendHeader("Content-Length", archivo.Length.ToString());
                            Response.ContentType = "application/octet-stream";
                            Response.BinaryWrite(archivo);
                            Response.End();
                        }
                        else
                            throw new Exception("No se pudo encontrar el archivo");

                        ErrorMessage.Text = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = ex.Message;
            }
        }

        protected void DescargarPDF(string id)
        {
            try
            {
                if (Session["DDTipoComprovante"].Equals("1") || DDMetodoPago.SelectedValue.Equals("PPD"))
                {
                    using (var db = new DataModel.OstarDB())
                    {
                        string tipo_documento = string.Empty;
                        var query =
                            (from f in db.factura
                             join t in db.ticket on f.ticket equals t.idticket
                             join e in db.emisor on t.emisor equals e.idemisor
                             where f.uuid == id
                             select new
                             {
                                 f.uuid,
                                 f.fecha_timbrado,
                                 emisor = seg.Desencriptar(e.rfc),
                                 idemisor = e.idemisor,
                                 logotipo = e.logotipo,
                                 cedula = e.cedula,
                                 origen = t.origen,
                                 contrato = t.contrato,
                                 fecha_pago = t.fecha_pago,
                                 tipo_pdf = e.tipo_pdf,
                                 tipo_documento = t.tipo_documento,
                                 identificador = t.identificador // seg.Desencriptar(e.identificador)
                             }).FirstOrDefault();
                        string rutaArchivo = rutaComprobantes + query.identificador + "/" + query.fecha_timbrado.ToString("yyyy-MM") + "/" + query.uuid;

                        if (File.Exists(rutaArchivo + ".xml") && !File.Exists(rutaArchivo + ".pdf"))
                        {
                            if (query.tipo_documento != null)
                            {
                                var tipoDoc = db.tipo.Where(x => x.id == query.tipo_documento.Value).FirstOrDefault();
                                tipo_documento = tipoDoc == null ? "FACTURA" : tipoDoc.tipo_comprobante;
                            }
                            else
                                tipo_documento = "FACTURA";

                            Facturador.GHO.Reporte.Imprimir imp = new Facturador.GHO.Reporte.Imprimir();
                            string rutaImagen = string.Empty;
                            string rutaCedula = string.Empty;
                            if (!string.IsNullOrEmpty(query.logotipo))
                                rutaImagen = rutaCertificado + query.idemisor + "/" + query.logotipo;
                            if (!string.IsNullOrEmpty(query.cedula))
                                rutaCedula = rutaCertificado + query.idemisor + "/" + query.cedula;
                            imp.Origen = query.origen;
                            imp.Contrato = query.contrato;
                            imp.FechaPago = query.fecha_pago == null ? "" : query.fecha_pago.Value.ToString("yyyy-MM-dd");
                            imp.CrearPDF(rutaArchivo + ".xml", rutaArchivo + ".pdf", Server.MapPath("~"), rutaImagen, rutaCedula, query.tipo_pdf == null ? 1 : query.tipo_pdf.Value, tipo_documento, 0);
                        }
                        if (File.Exists(rutaArchivo + ".pdf"))
                        {
                            Byte[] archivo = File.ReadAllBytes(rutaArchivo + ".pdf");
                            Response.Clear();
                            Response.AppendHeader("Content-Disposition", "filename=" + query.uuid + ".pdf");
                            Response.AppendHeader("Content-Length", archivo.Length.ToString());
                            Response.ContentType = "application/octet-stream";
                            Response.BinaryWrite(archivo);
                            Response.End();
                        }
                        else
                            throw new Exception("No se pudo encontrar el archivo");
                        ErrorMessage.Text = string.Empty;
                    }
                }
                if (Session["DDTipoComprovante"].Equals("2"))
                {
                    using (var db = new DataModel.OstarDB())
                    {
                        string tipocomplemento = string.Empty;
                        var query =
                            (from f in db.facturacfdirip
                             join c in db.cfdirip on f.cfdirip equals c.idcfdirip
                             join e in db.emisor on c.emisor equals e.idemisor
                             where f.uuid == id
                             select new
                             {
                                 f.uuid,
                                 f.fecha_timbrado,
                                 emisor = seg.Desencriptar(e.rfc),
                                 idemisor = e.idemisor,
                                 logotipo = e.logotipo,
                                 cedula = e.cedula,
                                 tipocomplemento =
                                 (string.IsNullOrEmpty(c.arrendamientoenfideicomiso.ToString()) ? null : "Arrendamiento en fideicomiso") +
                                 (string.IsNullOrEmpty(c.dividendos.ToString()) ? null : "Dividendos") +
                                 (string.IsNullOrEmpty(c.enajenaciondeacciones.ToString()) ? null : "Enajenación de acciones") +
                                 (string.IsNullOrEmpty(c.fideicnoempresarial.ToString()) ? null : "Fideicomiso no empresarial") +
                                 (string.IsNullOrEmpty(c.intereses.ToString()) ? null : "Intereses") +
                                 (string.IsNullOrEmpty(c.intereseshipotecarios.ToString()) ? null : "Intereses hipotecarios") +
                                 (string.IsNullOrEmpty(c.operacionesconderivados.ToString()) ? null : "Operaciones con derivados") +
                                 (string.IsNullOrEmpty(c.pagosaextranjeros.ToString()) ? null : "Pagos a extranjeros") +
                                 (string.IsNullOrEmpty(c.planesderetiro.ToString()) ? null : "Planes de retiro") +
                                 (string.IsNullOrEmpty(c.premios.ToString()) ? null : "Premios") +
                                 (string.IsNullOrEmpty(c.sectorfinanciero.ToString()) ? null : "Sector financiero"),
                                 identificador = e.identificador
                             }).FirstOrDefault();
                        string rutaArchivo = rutaComprobantesRIP + query.identificador + "/" + query.fecha_timbrado.ToString("yyyy-MM") + "/" + query.tipocomplemento + query.uuid;

                        if (File.Exists(rutaArchivo + ".xml") && !File.Exists(rutaArchivo + ".pdf"))
                        {

                            Facturador.GHO.Reporte.Imprimir imp = new Facturador.GHO.Reporte.Imprimir();
                            string rutaImagen = string.Empty;
                            string rutaCedula = string.Empty;
                            if (!string.IsNullOrEmpty(query.logotipo))
                                rutaImagen = rutaCertificado + query.idemisor + "/" + query.logotipo;
                            if (!string.IsNullOrEmpty(query.cedula))
                                rutaCedula = rutaCertificado + query.idemisor + "/" + query.cedula;
                            //imp.CrearPDF(rutaArchivo + ".xml", rutaArchivo + ".pdf", Server.MapPath("~"), rutaImagen, rutaCedula, query.tipo_pdf == null ? 1 : query.tipo_pdf.Value, tipo_documento);
                        }
                        if (File.Exists(rutaArchivo + ".pdf"))
                        {
                            Byte[] archivo = File.ReadAllBytes(rutaArchivo + ".pdf");
                            Response.Clear();
                            Response.AppendHeader("Content-Disposition", "filename=" + query.tipocomplemento + query.uuid + ".pdf");
                            Response.AppendHeader("Content-Length", archivo.Length.ToString());
                            Response.ContentType = "application/octet-stream";
                            Response.BinaryWrite(archivo);
                            Response.End();
                        }
                        else
                            throw new Exception("No se pudo encontrar el archivo");
                        ErrorMessage.Text = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = ex.Message;
            }
        }

        private void CancelarXML(string id)
        {
            try
            {
                this.idCancelar.Value = id;
                DateTime fechaTim = DateTime.Now;

                try
                {
                    using (var db = new DataModel.OstarDB())
                    {
                        string uuid = this.idCancelar.Value;

                        var folio = (from f in db.factura
                                     join t in db.ticket on f.ticket equals t.idticket
                                     where f.uuid == uuid
                                     select new
                                     {
                                         t.idticket,
                                         f.uuid,
                                         f.fecha_timbrado,
                                         t.rfc_emisor,
                                         t.identificador,
                                         t.emisor
                                     }).FirstOrDefault();

                        fechaTim = folio.fecha_timbrado;
                    }
                }
                catch (Exception ex)
                {
                    ErrorCancelar.Text = ex.Message;
                }

                this.ErrorCancelar.Text = string.Empty;

                this.textoCancelar.Text = "¿Desea cancelar el comprobante: " + id + "?";

                this.texto1.Text = "Motivo de cancelación:";
                this.texto2.Text = "Si el motivo de cancelación es 01, por favor ingrese el UUID de la nueva factura que sustituye a esta.";
                //if (fechaTim.AddDays(3) >= DateTime.Now)
                //{
                //    this.textoCancelar.Text = "¿Desea cancelar el comprobante: " + id + "?";
                //}
                //else
                //{
                //    this.textoCancelar.Text = "Ya se cumplio el plazo de 72 horas para cancelar el comprobante:" + id + " Debe de solicitar autorizacion a través del Buzón Triburario y regresar una vez que la tenga";
                //}
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#Cancelacion').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CancelacionModalScript", sb.ToString(), false);
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = ex.Message;
            }
        }

        //protected void CambioDeEstatusCancelados() {
        //    try
        //    {
        //        List<string> archivosCambiados = new List<string>();
        //        List<string> archivosNoCambiados = new List<string>();
        //        string line;
        //        string ruta = ruta = @"C:\Proyectos\Poryectos 2016\DS.Facturador.Royal.New\Facturador.GHO\ArchivosTemporales\EstatusCancelaciones.txt";

        //        System.IO.StreamReader file = new System.IO.StreamReader(ruta);

        //        while ((line = file.ReadLine()) != null)
        //        {
        //            using (var db = new DataModel.OstarDB())
        //            {
        //                var queryEstatus = (from f in db.factura
        //                                    join t in db.ticket on f.ticket equals t.idticket
        //                                    where f.uuid == line
        //                                    select t.idticket).ToList();

        //                int ticketValorId = Convert.ToInt32(queryEstatus[0]);

        //                if (ActualizarEstatus(ticketValorId))
        //                    archivosCambiados.Add(line);
        //                else
        //                    archivosNoCambiados.Add(line);

        //            }

        //        }
        //        ErrorMessage.Text = archivosCambiados.ToArray().ToString();
        //        ErrorMessage.Text += archivosNoCambiados.ToArray().ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorMessage.Text += "Cambio de estatus de facturas canceladas" + ex.Message;

        //    }
        //}
        //protected bool ActualizarEstatus(int valor)
        //{
        //    bool resultadoActualizacion = false;
        //    try
        //    {
        //        using (var db = new DataModel.OstarDB())
        //        { 
        //                var ticket = db.ticket
        //                    .Where(x => x.idticket == valor)
        //                    .Update(t => new DataModel.ticket
        //                    {
        //                        estatus = "Cancelado"
        //                    });
        //                    resultadoActualizacion = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        resultadoActualizacion = false;
        //        throw new Exception("Error actualizacion: "+ex.Message);
        //    }
        //    return resultadoActualizacion;
        //}
        public override void VerifyRenderingInServerForm(Control control)
        {

        }

        protected void btnExportar_Click(object sender, EventArgs e)
        {
            try
            {
                using (StringWriter sw = new StringWriter())
                {
                    HtmlTextWriter hw = new HtmlTextWriter(sw);
                    GridView DataGrd = new GridView();

                    DataGrd.AllowPaging = false;

                    using (var db = new DataModel.OstarDB())
                    {
                        List<int> emisoresActuales = new List<int>();
                        if (User.IsInRole("Admin") || User.IsInRole("Super"))
                        {
                            emisoresActuales = (from em in db.emisor
                                                select em.idemisor).ToList();
                        }
                        else
                        {
                            emisoresActuales = (from em in db.emisor
                                                join ue in db.useremisor on em.idemisor equals ue.EmisorId
                                                where ue.UserId == User.Identity.GetUserId()
                                                select em.idemisor).ToList();
                        }
                        //var query =
                        //    (from f in db.factura
                        //     join t in db.ticket on f.ticket equals t.idticket
                        //     join em in db.emisor on t.emisor equals em.idemisor
                        //     join r in db.receptor on f.receptor equals r.idreceptor
                        //     where f.fecha_timbrado >= (string.IsNullOrWhiteSpace(FechaInicial.Text) ? DateTime.Now.AddMonths(-1) : Convert.ToDateTime(FechaInicial.Text))
                        //        && f.fecha_timbrado < (string.IsNullOrWhiteSpace(FechaFinal.Text) ? DateTime.Now.AddDays(1) : Convert.ToDateTime(FechaFinal.Text).AddDays(1))
                        //        && f.uuid.Contains(UUID.Text) && emisoresActuales.Contains(em.idemisor) //&& r.rfc.Contains(RFCReceptor.Text) && em.rfc.Contains(RFCEmisor.Text)
                        //     select new
                        //     {
                        //         f.uuid,
                        //         f.fecha_timbrado,
                        //         no_ticket = t.serie + t.folio,
                        //         nombreEmisor = seg.Desencriptar(em.razon_social),
                        //         emisor = seg.Desencriptar(em.rfc),
                        //         nombreReceptor = seg.Desencriptar(r.razon_social),
                        //         receptor = seg.Desencriptar(r.rfc),
                        //         subtotal = t.subtotal,
                        //         impuestos = t.iva + t.ieps,
                        //         t.total
                        //     }).OrderBy(x => x.fecha_timbrado).ToList();
                        //DataGrd.DataSource = query.Where(x => x.emisor.Contains(DDEmisor.Text) && x.receptor.Contains(RFCReceptor.Text)).ToList();
                        //DataGrd.DataBind();
                        int idEmisor = Convert.ToInt32(DDEmisor.Text);
                        var query2 = (from f in db.factura
                                      join t in db.ticket on f.ticket equals t.idticket
                                      //where f.fecha_timbrado >= (string.IsNullOrWhiteSpace(FechaInicial.Text) ? DateTime.Now.AddMonths(-1) : Convert.ToDateTime(FechaInicial.Text))
                                      //   && f.fecha_timbrado < (string.IsNullOrWhiteSpace(FechaFinal.Text) ? DateTime.Now.AddDays(1) : Convert.ToDateTime(FechaFinal.Text).AddDays(1))
                                      where f.fecha_timbrado >= (string.IsNullOrWhiteSpace(FechaInicial.Text) ? DateTime.Now.AddMonths(-1) : DateTime.ParseExact(FechaInicial.Text.Trim(), "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture))
                                         && f.fecha_timbrado < (string.IsNullOrWhiteSpace(FechaFinal.Text) ? DateTime.Now.AddDays(1) : DateTime.ParseExact(FechaFinal.Text.Trim(), "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture).AddDays(1))
                                         && f.uuid.Contains(UUID.Text) && emisoresActuales.Contains(t.emisor) && (idEmisor == 0 ? true : t.emisor == idEmisor)
                                      orderby f.fecha_timbrado ascending
                                      select new
                                      {
                                          idEmisor = t.emisor,
                                          uuid = f.uuid,
                                          serieyfolio = t.serie + t.folio,
                                          fecha_timbrado = f.fecha_timbrado,
                                          emisor = t.rfc_emisor,
                                          receptor = t.rfc_receptor,
                                          subtotal = t.subtotal,
                                          iva = t.fktraslados.Where(x => x.impuesto == "IVA").Sum(x => x.importe),
                                          ieps = t.fktraslados.Where(x => x.impuesto == "IEPS").Sum(x => x.importe),
                                          ish = t.fktrasladoslocales.Sum(x => x.importe),
                                          isn = t.fkretencioneslocales.Sum(x => x.importe),
                                          ret_iva = t.fkretenciones.Where(x => x.impuesto == "IVA").Sum(x => x.importe),
                                          ret_isr = t.fkretenciones.Where(x => x.impuesto == "ISR").Sum(x => x.importe),
                                          total = t.total,
                                          moneda = t.moneda,
                                          tipo_cambio = t.tipo_cambio,
                                          tipo = t.tipo_comprobante,
                                          estatus = t.estatus,
                                          origen = t.origen,
                                          contrato = t.contrato,
                                          serie = t.serie
                                      });
                        DataGrd.DataSource = query2.ToList();
                        DataGrd.DataBind();
                    }

                    DataGrd.HeaderRow.BackColor = System.Drawing.Color.White;
                    foreach (TableCell cell in DataGrd.HeaderRow.Cells)
                    {
                        cell.BackColor = DataGrd.HeaderStyle.BackColor;
                    }
                    foreach (GridViewRow row in DataGrd.Rows)
                    {
                        row.BackColor = System.Drawing.Color.White;
                        foreach (TableCell cell in row.Cells)
                        {
                            if (row.RowIndex % 2 == 0)
                            {
                                cell.BackColor = System.Drawing.Color.LightBlue;
                            }
                            else
                            {
                                cell.BackColor = System.Drawing.Color.White;
                            }
                            cell.CssClass = "textmode";
                        }
                    }

                    DataGrd.RenderControl(hw);

                    Response.Clear();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment;filename=Reporte.xls");
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.ms-excel";

                    string style = @"<style> .textmode { mso-number-format:\@; } </style>";
                    Response.Write(style);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }
            catch
            {
            }
        }
        protected void btnDescargar_Click(object sender, EventArgs em)
        {
            //CambioDeEstatusCancelados();
            DSTimbrado timbre = new DSTimbrado();
            ErrorMessage.Text = "";
            List<string> archivosInExistentes = new List<string>();
            List<string> archivosDescargables = new List<string>();
            if (string.IsNullOrEmpty(FechaInicial.Text) || string.IsNullOrEmpty(FechaFinal.Text) || DDEmisor.Text.Equals("0"))
            {
                ErrorMessage.Text = "Los campos Emisor, Fecha inicial y Fecha final son requeridos para este proceso";
            }
            else
            {
                try
                {
                    #region Obtencion de archivos
                    //Se hacen las consultas para obtener los uuid 
                    using (var db = new DataModel.OstarDB())
                    {
                        List<int> emisoresActuales = new List<int>();
                        if (User.IsInRole("Admin") || User.IsInRole("Super"))
                        {
                            emisoresActuales = (from e in db.emisor
                                                select e.idemisor).ToList();
                        }
                        else
                        {
                            emisoresActuales = (from e in db.emisor
                                                join ue in db.useremisor on e.idemisor equals ue.EmisorId
                                                where ue.UserId == User.Identity.GetUserId()
                                                select e.idemisor).ToList();
                        }
                        int idEmisor = Convert.ToInt32(DDEmisor.Text);
                        string tipo_documento = string.Empty;
                        var query = (from f in db.factura
                                     join t in db.ticket on f.ticket equals t.idticket
                                     where f.fecha_timbrado >= (string.IsNullOrWhiteSpace(FechaInicial.Text) ? DateTime.Now.AddMonths(-1) : DateTime.ParseExact(FechaInicial.Text.Trim(), "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture))
                                         && f.fecha_timbrado < (string.IsNullOrWhiteSpace(FechaFinal.Text) ? DateTime.Now.AddDays(1) : DateTime.ParseExact(FechaFinal.Text.Trim(), "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture).AddDays(1))
                                         && f.uuid.Contains(UUID.Text) && emisoresActuales.Contains(t.emisor) && (idEmisor == 0 ? true : t.emisor == idEmisor)
                                     orderby f.fecha_timbrado ascending
                                     select f.uuid).ToList();
                        db.Close();
                        //if (query.Count > 500)
                        //{
                        //    ErrorMessage.Text="Facturas: "+query.Count+", la cantidad maxima de facturas descargables es de 500";
                        //    return;
                        //}

                        #endregion
                        #region Validacion de existencia y regeneracion de PDF
                        //Se hacen las consultas para ir validando existencias de los archivos fisicamente
                        using (var db2 = new DataModel.OstarDB())
                        {
                            foreach (string id in query)
                            {
                                var query2 =
                                 (from f in db2.factura
                                  join t in db2.ticket on f.ticket equals t.idticket
                                  join e in db2.emisor on t.emisor equals e.idemisor
                                  where f.uuid == id
                                  select new
                                  {
                                      f.uuid,
                                      f.fecha_timbrado,
                                      emisor = seg.Desencriptar(e.rfc),
                                      idemisor = e.idemisor,
                                      logotipo = e.logotipo,
                                      cedula = e.cedula,
                                      origen = t.origen,
                                      contrato = t.contrato,
                                      fecha_pago = t.fecha_pago,
                                      tipo_pdf = e.tipo_pdf,
                                      tipo_documento = t.tipo_documento,
                                      identificador = t.identificador // seg.Desencriptar(e.identificador)
                                  }).FirstOrDefault();
                                if (query2 == null)
                                {
                                    throw new Exception("La consulta de regeneracion no encuentra ninguna coincidencia");
                                }
                                string rutaArchivo = rutaComprobantes + query2.identificador + "/" + query2.fecha_timbrado.ToString("yyyy-MM") + "/" + query2.uuid;
                                //Regenera el xml sino lo encuentra
                                if (!File.Exists(rutaArchivo + ".xml"))
                                    timbre.RecuperarXML(query2.uuid.ToUpper(), rutaArchivo.Replace("/" + query2.uuid, ""));

                                if (!File.Exists(rutaArchivo + ".xml"))
                                    archivosInExistentes.Add(query2.uuid + ".xml");
                                else
                                {
                                    try
                                    {
                                        archivosDescargables.Add(rutaArchivo + ".xml");

                                        if (File.Exists(rutaArchivo + ".xml") && !File.Exists(rutaArchivo + ".pdf"))
                                        {
                                            if (query2.tipo_documento != null)
                                            {
                                                var tipoDoc = db2.tipo.Where(x => x.id == query2.tipo_documento.Value).FirstOrDefault();
                                                tipo_documento = tipoDoc == null ? "FACTURA" : tipoDoc.tipo_comprobante;
                                            }
                                            else
                                                tipo_documento = "FACTURA";

                                            Facturador.GHO.Reporte.Imprimir imp = new Facturador.GHO.Reporte.Imprimir();
                                            string rutaImagen = string.Empty;
                                            string rutaCedula = string.Empty;
                                            if (!string.IsNullOrEmpty(query2.logotipo))
                                                rutaImagen = rutaCertificado + query2.idemisor + "/" + query2.logotipo;
                                            if (!string.IsNullOrEmpty(query2.cedula))
                                                rutaCedula = rutaCertificado + query2.idemisor + "/" + query2.cedula;
                                            imp.Origen = query2.origen;
                                            imp.Contrato = query2.contrato;
                                            imp.FechaPago = query2.fecha_pago == null ? "" : query2.fecha_pago.Value.ToString("yyyy-MM-dd");
                                            imp.CrearPDF(rutaArchivo + ".xml", rutaArchivo + ".pdf", Server.MapPath("~"), rutaImagen, rutaCedula, query2.tipo_pdf == null ? 1 : query2.tipo_pdf.Value, tipo_documento, 0);
                                        }
                                        archivosDescargables.Add(rutaArchivo + ".pdf");
                                    }
                                    catch (Exception ex)
                                    {
                                        archivosInExistentes.Add(query2.uuid + ".pdf");
                                    }
                                }
                            }

                        }
                        #endregion
                        try
                        {
                            if (!Directory.Exists(Server.MapPath("~" + rutaArchivosTemporales)))
                                Directory.CreateDirectory(Server.MapPath("~" + rutaArchivosTemporales));

                            ZipFile archivoZipMasivo = new ZipFile();
                            archivoZipMasivo.AddFiles(archivosDescargables, @"/");
                            archivoZipMasivo.Save(Server.MapPath("~" + rutaArchivosTemporales + "FACTURAS.zip"));

                            Response.Clear();
                            Response.AddHeader("Content-Disposition", "attachment; filename=Facturacion.zip");
                            Response.ContentType = "application/octet-stream";
                            Response.TransmitFile(Server.MapPath("~" + rutaArchivosTemporales + "FACTURAS.zip"));
                            Response.Flush();
                            File.Delete(Server.MapPath("~" + rutaArchivosTemporales + "FACTURAS.zip"));
                            //Response.End();
                            ErrorMessage.Text = "Los siguientes archivos no se pudieron descargar: ";
                            foreach (string archivo in archivosInExistentes)
                                ErrorMessage.Text += archivo + ", ";
                        }
                        catch (Exception ex)
                        {
                            ErrorMessage.Text = ex.Message;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage.Text = "Error en las consultas: " + ex.Message;
                }
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            ////string rutaXML = rutaComprobantes + "MTY" + "/";

            ////if (!Directory.Exists(rutaXML))
            ////{
            ////    Directory.CreateDirectory(rutaXML);
            ////}
            ////DSTimbrado timbre = new DSTimbrado();
            ////timbre.RecuperarAcuse("ea972201-cc26-43c3-971c-9276109a872c", rutaXML);

            try
            {
                using (var db = new DataModel.OstarDB())
                {
                    string uuid = this.idCancelar.Value;
                    string motivo_c = DDTipocancelacion.SelectedValue.ToString();
                    string uuid_relacionado = this.uuid_relacionado.Text;

                    if (motivo_c != "01") {
                        uuid_relacionado = "";
                    }

                    var folio = (from f in db.factura
                                 join t in db.ticket on f.ticket equals t.idticket
                                 where f.uuid == uuid
                                 select new
                                 {
                                     t.idticket,
                                     f.uuid,
                                     f.fecha_timbrado,
                                     t.rfc_emisor,
                                     t.rfc_receptor,
                                     t.total,
                                     t.identificador,
                                     t.emisor
                                 }).FirstOrDefault();

                    var queryEmisor = db.emisor
                            .Where(t => t.idemisor == folio.emisor).FirstOrDefault();

                    var certificado = db.certificado
                        .Where(t => t.idcertificado == queryEmisor.cetificado).FirstOrDefault();


                    if (folio == null)
                        throw new Exception("No se encontro el comprobante");
                    else
                    {
                        DSTimbrado timbre = new DSTimbrado();
                        byte[] cerArray = System.IO.File.ReadAllBytes(rutaCertificado + queryEmisor.idemisor + "/" + certificado.csd);
                        string cer64 = Convert.ToBase64String(cerArray);
                        byte[] keyArray = System.IO.File.ReadAllBytes(rutaCertificado + queryEmisor.idemisor + "/" + certificado.llave_privada);
                        string key64 = Convert.ToBase64String(keyArray);

                        if (folio.rfc_emisor != "AAA010101AAA")
                            timbre.TipoTimbrado = Daysoft.DICI.Facturador.DFacture.wsDaySoft.TypeCFD.Production;
                        else
                            timbre.TipoTimbrado = Daysoft.DICI.Facturador.DFacture.wsDaySoft.TypeCFD.Test;

                        try
                        {
                            string[] param = folio.total.ToString().Split('.');
                            string status = timbre.CancelarCFDI(folio.rfc_emisor, folio.rfc_receptor, param[0] + "." + param[1].Substring(0, 2), folio.uuid, cer64, key64, seg.Desencriptar(certificado.contrasenia), motivo_c, uuid_relacionado);

                            if (status.Contains("Comprobante cancelado exitosamente") || status.Contains("Comprobante cancelado previamente"))
                            {
                                string rutaXML = rutaComprobantes + folio.identificador + "/" + folio.fecha_timbrado.ToString("yyyy-MM") + "/";

                                if (!Directory.Exists(rutaXML))
                                {
                                    Directory.CreateDirectory(rutaXML);
                                }
                                rutaXML = rutaXML + folio.uuid + ".can";
                                timbre.GuardarAcuse(rutaXML);
                            }

                            var ticket = db.ticket
                                .Where(x => x.idticket == folio.idticket)
                                .Update(t => new DataModel.ticket
                                {
                                    estatus = status
                                });
                        }
                        catch (Exception ex)
                        {
                            var ticket = db.ticket
                                .Where(x => x.idticket == folio.idticket)
                                .Update(t => new DataModel.ticket
                                {
                                    estatus = ex.Message
                                });
                        }
                    }
                }
                this.ObtenerComprobantes();

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#Cancelacion').modal('hide');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CancelacionModalScript", sb.ToString(), false);
            }
            catch (Exception ex)
            {
                ErrorCancelar.Text = ex.Message;
            }
        }

        protected void viewFacturas_RowCreated(object sender, GridViewRowEventArgs e)
        {
            LinkButton button = (LinkButton)e.Row.FindControl("LinkCancelar");
            if (button != null)
            {
                ScriptManager.GetCurrent(this.Page).RegisterAsyncPostBackControl(button);
            }
        }
    }
}