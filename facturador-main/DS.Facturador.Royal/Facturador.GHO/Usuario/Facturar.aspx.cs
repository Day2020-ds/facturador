using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Facturador.GHO.Controllers;
using Microsoft.AspNet.Identity;
using System.IO;
using BLToolkit.Data.Linq;
using Daysoft.DICI.Facturador.DFacture;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using CFDi;

namespace Facturador.GHO.Usuario
{
    public partial class Facturar : System.Web.UI.Page
    {
        private Seguridad seg;
        private License lic;
        protected ImpuestosLocales impuestosLocales;
        protected Donatarias donatarias1;
        string rutaLicencia = string.Empty;
        private string rutaCertificado = @"/Facturacion/Empresas/Certificados/";
        private string rutaComprobantes = @"/Facturacion/Empresas/Comprobantes/";
        private string PAC = string.Empty;
        private string comprobante = string.Empty;
        private string Version = string.Empty;
        private string NoAutorizacion = string.Empty;
        private string FechaAutorizacion = string.Empty;
        private string NoAutorizacion2 = string.Empty;
        private string FechaAutorizacion2 = string.Empty;
        private string Leyenda = string.Empty;
        private static string rutaCadenaOriginal = string.Empty;
        private string mostrarRetenciones = string.Empty;
        private DataModel.ticket cabecera;
        private List<DataModel.ticket_detalle> detalles;
        private List<DataModel.ticket_retencion> retenciones;
        private List<DataModel.ticket_traslado> traslados;
        private List<DataModel.ticket_retencion_local> retenciones_locales;
        private List<DataModel.ticket_traslado_local> traslados_locales;
        private DataModel.receptor receptor;
        private DataModel.domicilio domicilio;

        private decimal totalIVA = 0;
        private decimal totalISH = 0;
        private decimal totalISN = 0;
        private decimal tasa = 0;
        private string tipofactor;
        private decimal totalRet;
        private bool IVACero = false;
        private bool IVA16 = false;
        private bool IVAExento = false;
        private bool RetIVA = false;
        private bool RetISR = false;
        private decimal totalRetIva = 0;
        private decimal totalRetIsr = 0;

        public ImpuestosLocales ImpuestosLocales { get { return this.impuestosLocales; } }
        public Donatarias Dontarias { get { return this.donatarias1; } }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                seg = new Seguridad();
                rutaComprobantes = Server.MapPath("~" + System.Configuration.ConfigurationManager.AppSettings["rutaComprobantes"]);
                rutaCertificado = Server.MapPath("~" + System.Configuration.ConfigurationManager.AppSettings["rutaCertificado"]);
                PAC = System.Configuration.ConfigurationManager.AppSettings["PAC"];
                Version = System.Configuration.ConfigurationManager.AppSettings["VersionDonat"];
                NoAutorizacion = System.Configuration.ConfigurationManager.AppSettings["NoAutorizacionDonat"];
                FechaAutorizacion = System.Configuration.ConfigurationManager.AppSettings["FechaAutorizacionDonat"];
                NoAutorizacion2 = System.Configuration.ConfigurationManager.AppSettings["NoAutorizacionDonat2"];
                FechaAutorizacion2 = System.Configuration.ConfigurationManager.AppSettings["FechaAutorizacionDonat2"];
                Leyenda = System.Configuration.ConfigurationManager.AppSettings["LeyendaDonat"];
                rutaCadenaOriginal = Server.MapPath("~" + System.Configuration.ConfigurationManager.AppSettings["rutaCadenaOriginal"]);
                mostrarRetenciones = Server.MapPath("~" + System.Configuration.ConfigurationManager.AppSettings["mostrarRetenciones"]);
                LeerLicencia();
                if (!IsPostBack)
                {
                    cabecera = new DataModel.ticket();
                    Session["objCabecera"] = this.cabecera;
                    detalles = new List<DataModel.ticket_detalle>();
                    Session["objDetalles"] = this.detalles;
                    retenciones = new List<DataModel.ticket_retencion>();
                    Session["objRetenciones"] = this.retenciones;
                    traslados = new List<DataModel.ticket_traslado>();
                    Session["objTraslados"] = this.traslados;
                    retenciones_locales = new List<DataModel.ticket_retencion_local>();
                    Session["objRetencionesLocales"] = this.retenciones_locales;
                    traslados_locales = new List<DataModel.ticket_traslado_local>();
                    Session["objTrasladoslocales"] = this.traslados_locales;
                    receptor = new DataModel.receptor();
                    Session["objReceptor"] = this.receptor;
                    domicilio = new DataModel.domicilio();
                    Session["objDomicilio"] = this.domicilio;

                    LlenarTotales();
                    LlenarEmpresas();
                    LlenarMoneda();
                    this.LLenarFolios(Convert.ToInt32(Empresa.SelectedValue));
                    this.MostrarRetenciones(Convert.ToInt32(Serie.SelectedValue));
                    this.MostrarRetencionesLocales(Convert.ToInt32(Serie.SelectedValue));
                    LlenarMetodoPago();
                    LlenarFormaPago();
                    LlenarUsoCFDI();
                    LlenarPais();
                    LlenarClaveUnidad();
                    LlenarcProdServ(Convert.ToInt32(Empresa.SelectedValue));
                    LlenarTipoFactor();
                    LlenarTasaOCuota();
                    LlenarTipoRelacion();

                    this.LlenarTipoCambio(Convert.ToInt32(Moneda.SelectedValue));
                }
            }
            catch (Exception ex)
            {
                this.EnviarError(ex.Message);
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

                if (User.IsInRole("Admin"))
                {
                    emp = (from h in db.emisor
                           select new
                           {
                               razon_social = h.identificador + " - " + seg.Desencriptar(h.razon_social),
                               rfc = h.idemisor.ToString()
                           }).Take(lic.ObtenerEmpresas()).ToDictionary(t => t.rfc, t => t.razon_social);
                }
                else
                {
                    emp = (from ue in db.useremisor
                           where ue.UserId == User.Identity.GetUserId()
                           select new
                           {
                               razon_social = ue.ibfk2.identificador + " - " + seg.Desencriptar(ue.ibfk2.razon_social),
                               rfc = ue.ibfk2.idemisor.ToString()
                           }).Take(lic.ObtenerEmpresas()).ToDictionary(t => t.rfc, t => t.razon_social);
                }

                Empresa.DataValueField = "Key";
                Empresa.DataTextField = "Value";
                Empresa.DataSource = emp;
                Empresa.DataBind();
            }
        }

        private void LlenarMoneda()
        {
            using (var db = new DataModel.OstarDB())
            {
                var query = db.moneda;
                Moneda.DataTextField = "nombre";
                Moneda.DataValueField = "id";
                Moneda.DataSource = query;
                Moneda.DataBind();
            }
        }

        private void LlenarMetodoPago()
        {
            using (var db = new DataModel.OstarDB())
            {
                Dictionary<string, string> pag = new Dictionary<string, string>();
                pag = (from mp in db.cMetodoPago
                       select new
                       {
                           titulo = mp.codigo + " - " + mp.descripcion,
                           valor = mp.codigo
                       }).ToDictionary(t => t.titulo, t => t.valor);

                ddlMetodoPago.DataTextField = "Key";
                ddlMetodoPago.DataValueField = "Value";
                ddlMetodoPago.DataSource = pag;
                ddlMetodoPago.DataBind();
            }
        }

        private void LlenarFormaPago()
        {
            using (var db = new DataModel.OstarDB())
            {
                Dictionary<string, string> pag = new Dictionary<string, string>();
                pag = (from fp in db.cFormaPago
                       orderby fp.codigo descending
                       select new
                       {
                           titulo = fp.codigo + " - " + fp.descripcion,
                           valor = fp.codigo
                       }).ToDictionary(t => t.titulo, t => t.valor);

                pag.Add("", "0");
                ddlFormaPago.DataTextField = "Key";
                ddlFormaPago.DataValueField = "Value";
                ddlFormaPago.DataSource = pag.Reverse();
                ddlFormaPago.DataBind();
            }
        }

        private void LlenarUsoCFDI()
        {
            using (var db = new DataModel.OstarDB())
            {
                Dictionary<string, string> pag = new Dictionary<string, string>();
                pag = (from uCFDi in db.cUsoCFDI
                        orderby uCFDi.codigo descending
                       select new
                        {
                            titulo = uCFDi.codigo + " - " + uCFDi.descripcion,
                            valor = uCFDi.codigo
                        }).ToDictionary(t => t.titulo, t => t.valor);

                pag.Add("", "0");
                ddlUsoCFDI.DataTextField = "Key";
                ddlUsoCFDI.DataValueField = "Value";
                ddlUsoCFDI.DataSource = pag.Reverse();
                ddlUsoCFDI.DataBind();
            }
        }

        private void LlenarTipoRelacion()
        {
            using (var db = new DataModel.OstarDB())
            {
                Dictionary<string, string> pag = new Dictionary<string, string>();
                pag = (from uTipoRel in db.cTipoRelacion
                       orderby uTipoRel.codigo descending
                       select new
                       {
                           titulo = uTipoRel.codigo + " - " + uTipoRel.descripcion,
                           valor = uTipoRel.codigo
                       }).ToDictionary(t => t.titulo, t => t.valor);

                pag.Add("", "0");
                ddlTipoRelacion.DataTextField = "Key";
                ddlTipoRelacion.DataValueField = "Value";
                ddlTipoRelacion.DataSource = pag.Reverse();
                ddlTipoRelacion.DataBind();
            }
        }

        private void LlenarPais()
        {
            using (var db = new DataModel.OstarDB())
            {
                Dictionary<string, string> pag = new Dictionary<string, string>();
                pag = (from p in db.cPais
                       orderby p.codigo descending
                       select new
                       {
                           titulo = p.codigo + " - " + p.descripcion,
                           valor = p.codigo
                       }).ToDictionary(t => t.titulo, t => t.valor);

                pag.Add("MEX", "MXN");
                ddlPais.DataTextField = "Key";
                ddlPais.DataValueField = "Value";
                ddlPais.DataSource = pag.Reverse();
                ddlPais.DataBind();
            }
        }

        private void LlenarClaveUnidad()
        {
            using (var db = new DataModel.OstarDB())
            {
                Dictionary<string, string> pag = new Dictionary<string, string>();
                pag = (from cu in db.cClaveUnidad
                       orderby cu.codigo descending
                       select new
                       {
                           titulo = cu.codigo + " - " + cu.descripcion,
                           valor = cu.codigo
                       }).ToDictionary(t => t.titulo, t => t.valor);

                pag.Add("", "0");
                DdlUnidad.DataTextField = "Key";
                DdlUnidad.DataValueField = "Value";
                DdlUnidad.DataSource = pag.Reverse(); 
                DdlUnidad.DataBind();
            }
        }

        private void LlenarcProdServ(int idEmisor)
        {
            using (var db = new DataModel.OstarDB())
            {
                Dictionary<string, string> pag = new Dictionary<string, string>();
                pag = (from cps in db.emisor_cProdServ
                       where cps.emisor_id == idEmisor
                       orderby cps.fkcProdServ_emisor.codigo descending
                       select new
                       {
                           titulo = cps.fkcProdServ_emisor.codigo + " - " + cps.fkcProdServ_emisor.descripcion,
                           valor = cps.fkcProdServ_emisor.codigo
                       }).ToDictionary(t => t.titulo, t => t.valor);

                pag.Add("", "0");
                DdlcProdServ.DataTextField = "Key";
                DdlcProdServ.DataValueField = "Value";
                DdlcProdServ.DataSource = pag.Reverse();
                DdlcProdServ.DataBind();
            }
        }

        private void LlenarTipoFactor()
        {
            using (var db = new DataModel.OstarDB())
            {
                Dictionary<string, string> pag = new Dictionary<string, string>();
                pag = (from tf in db.cTipoFactor
                       orderby tf.descripcion descending
                       select new
                       {
                           titulo = tf.descripcion,
                           valor = tf.descripcion
                       }).ToDictionary(t => t.titulo, t => t.valor);

                pag.Add("", string.Empty);
                
                DdlTipoFactorIVA.DataTextField = "Key";
                DdlTipoFactorIVA.DataValueField = "Value";
                DdlTipoFactorIVA.DataSource = pag.Reverse();
                DdlTipoFactorIVA.DataBind();

                DdlTipoFactorIVAret.DataTextField = "Key";
                DdlTipoFactorIVAret.DataValueField = "Value";
                DdlTipoFactorIVAret.DataSource = pag.Reverse();
                DdlTipoFactorIVAret.DataBind();

                DdlTipoFactorISRret.DataTextField = "Key";
                DdlTipoFactorISRret.DataValueField = "Value";
                DdlTipoFactorISRret.DataSource = pag.Reverse();
                DdlTipoFactorISRret.DataBind();

            }
        }

        private void LlenarTasaOCuota()
        {
            using (var db = new DataModel.OstarDB())
            {
                Dictionary<string, string> pag = new Dictionary<string, string>();
                pag = (from toc in db.cTasaOCuota
                       orderby toc.tasacuota descending
                       select new
                       {
                           titulo = toc.tasacuota,
                           valor = toc.tasacuota
                       }).ToDictionary(t => t.titulo, t => t.valor);

                pag.Add("", string.Empty);
                DdlTasaoCuotaIVA.DataTextField = "Key";
                DdlTasaoCuotaIVA.DataValueField = "Value";
                DdlTasaoCuotaIVA.DataSource = pag.Reverse();
                DdlTasaoCuotaIVA.DataBind();

                DdlTasaoCuotaIVAret.DataTextField = "Key";
                DdlTasaoCuotaIVAret.DataValueField = "Value";
                DdlTasaoCuotaIVAret.DataSource = pag.Reverse();
                DdlTasaoCuotaIVAret.DataBind();

                DdlTasaoCuotaISRret.DataTextField = "Key";
                DdlTasaoCuotaISRret.DataValueField = "Value";
                DdlTasaoCuotaISRret.DataSource = pag.Reverse();
                DdlTasaoCuotaISRret.DataBind();
            }
        }

        private void LLenarFolios(int idEmisor)
        {
            using (var db = new DataModel.OstarDB())
            {
                int idAnterior = Convert.ToInt32(string.IsNullOrWhiteSpace(idEmpresa.Value) ? "0" : idEmpresa.Value);
                if (idAnterior != idEmisor)
                {
                    var tipoPdf = db.emisor.Where(x => x.idemisor == idEmisor).Select(x => x.tipo_pdf).FirstOrDefault();
                    if (tipoPdf == null)
                        tipoPdf = 1;

                    switch (tipoPdf)
                    {
                        case 1:
                            MostrarOrigenContrato(false);
                            MostrarFechaPago(false);
                            break;
                        case 2:
                            MostrarOrigenContrato(true);
                            MostrarFechaPago(false);
                            break;
                        case 3:
                            MostrarOrigenContrato(false);
                            MostrarFechaPago(true);
                            break;
                        case 4:
                            MostrarOrigenContrato(true);
                            MostrarFechaPago(true);
                            break;
                        case 5:
                        case 6:
                            MostrarOrigenContrato(false);
                            MostrarFechaPago(false);
                            break;
                        default:
                            MostrarOrigenContrato(false);
                            MostrarFechaPago(false);
                            break;
                    }

                    var query = from f in db.folio
                                join t in db.tipo on f.tipo equals t.id
                                where f.emisor == idEmisor
                                select new
                                {
                                    id = f.id,
                                    serie = f.serie + " - " + t.tipo_comprobante
                                };
                    Serie.DataTextField = "serie";
                    Serie.DataValueField = "id";
                    Serie.DataSource = query;
                    Serie.DataBind();

                    idEmpresa.Value = idEmisor.ToString();
                    this.MostrarRetenciones(Convert.ToInt32(Serie.SelectedValue));
                    this.MostrarRetencionesLocales(Convert.ToInt32(Serie.SelectedValue));

                }
            }
        }

        private void MostrarOrigenContrato(bool mostrar)
        {
            divOrigen.Style.Remove(HtmlTextWriterStyle.Display);
            divOrigen.Style.Add(HtmlTextWriterStyle.Display, mostrar ? "block" : "none");
            rfvOrigen.Enabled = mostrar;
            divContrato.Style.Remove(HtmlTextWriterStyle.Display);
            divContrato.Style.Add(HtmlTextWriterStyle.Display, mostrar ? "block" : "none");
            rfvContrato.Enabled = mostrar;
        }

        private void MostrarFechaPago(bool mostrar)
        {
            divFechaPago.Style.Remove(HtmlTextWriterStyle.Display);
            divFechaPago.Style.Add(HtmlTextWriterStyle.Display, mostrar ? "block" : "none");
            rfvFechaPago.Enabled = mostrar;
        }

        protected void Identificador_TextChanged(object sender, EventArgs e)
        {
            int idCli = 0;
            using (var db = new DataModel.OstarDB())
            {
                var rec = db.receptor.Where(x => (x.identificador == this.Identificador.Text && x.emisor == Convert.ToInt32(Empresa.SelectedValue))).FirstOrDefault();

                if (rec != null)
                {
                    idCli = rec.idreceptor;
                    this.idReceptor.Value = rec.idreceptor.ToString();
                }
                else
                {
                    this.idReceptor.Value = string.Empty;
                }
            }
            if (idCli != 0)
                this.LlenarCliente(idCli);
        }

        protected void btnSearchClient_Click(object sender, EventArgs e)
        {
            this.BuscarIdentificador.Text = this.Identificador.Text.Trim();
            this.BuscarRazonSocial.Text = string.Empty;
            this.BuscarRFC.Text = string.Empty;
            LlenarEmpresas(Convert.ToInt32(Empresa.SelectedValue));

            titleCliente.Text = "Busqueda de clientes";

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#modalCliente').modal('show');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ClienteModalScript", sb.ToString(), false);
        }

        protected void Empresa_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.LLenarFolios(Convert.ToInt32(Empresa.SelectedValue));
            this.LlenarcProdServ(Convert.ToInt32(Empresa.SelectedValue));
        }

        protected void ClaveCargo_TextChanged(object sender, EventArgs e)
        {
            using (var db = new DataModel.OstarDB())
            {
                DataModel.producto concepto = db.producto
                    .Where(x => (x.codigo == this.ClaveCargo.Text && x.emisor == Convert.ToInt32(Empresa.SelectedValue)))
                    .FirstOrDefault();
                if (concepto != null)
                {
                    this.LlenarProducto(concepto);
                }
            }
        }

        private void LlenarProducto(DataModel.producto producto)
        {
            this.ClaveCargo.Text = producto.codigo;
          //  this.CuentaPredial.Text = producto.cuenta_predial;
            this.Descripcion.Text = producto.descripcion;
            this.Descuento.Text = producto.descuento != null ? producto.descuento.Value.ToString("0.00") : "";
           // this.ISH.Text = producto.ish != null ? producto.ish.Value.ToString("0.00") : "";
            this.ISN.Text = producto.isn != null ? producto.isn.Value.ToString("0.00") : "";
            this.DdlUnidad.Text = producto.unidad_medida;
            this.Precio.Text = producto.valor_unitario.ToString("0.00");
        }

        protected void btnAgregarConcepto_Click(object sender, EventArgs e)
        {
            try
            {
                this.detalles = Session["objDetalles"] as List<DataModel.ticket_detalle>;

                DataModel.ticket_detalle detalle = new DataModel.ticket_detalle();
                detalle.no_identificacion = this.ClaveCargo.Text;
                detalle.clave_prod_serv = DdlcProdServ.SelectedValue;
                detalle.cantidad = Convert.ToDecimal(this.Cantidad.Text);
                detalle.clave_unidad = DdlUnidad.SelectedValue;
                detalle.valor_unitario = Convert.ToDecimal(this.Precio.Text);
                detalle.cuenta_predial = "";// this.CuentaPredial.Text;
                detalle.descuento = string.IsNullOrWhiteSpace(this.Descuento.Text) ? null : (decimal?)Convert.ToDecimal(this.Descuento.Text);
                detalle.iva = string.IsNullOrWhiteSpace(this.DdlTasaoCuotaIVA.SelectedValue) ? null : (decimal?)Convert.ToDecimal(this.DdlTasaoCuotaIVA.SelectedValue);
                detalle.tipo_factor_iva = this.DdlTipoFactorIVA.SelectedValue;
                detalle.iva_ret = string.IsNullOrWhiteSpace(this.DdlTasaoCuotaIVAret.SelectedValue) ? null : (decimal?)Convert.ToDecimal(this.DdlTasaoCuotaIVAret.SelectedValue);
                detalle.tipo_factor_iva_ret = this.DdlTipoFactorIVAret.SelectedValue;
                detalle.isr_ret = string.IsNullOrWhiteSpace(this.DdlTasaoCuotaISRret.SelectedValue) ? null : (decimal?)Convert.ToDecimal(this.DdlTasaoCuotaISRret.SelectedValue);
                detalle.tipo_factor_isr_ret = this.DdlTipoFactorISRret.SelectedValue;
                detalle.ish = 0; //string.IsNullOrWhiteSpace(this.ISH.Text) ? null : (decimal?)Convert.ToDecimal(this.ISH.Text);
                detalle.isn = string.IsNullOrWhiteSpace(this.ISN.Text) ? null : (decimal?)Convert.ToDecimal(this.ISN.Text);
                detalle.fecha_pago = string.IsNullOrWhiteSpace(this.FechaPago.Text) ? null : (DateTime?)DateTime.ParseExact(this.FechaPago.Text, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                detalle.tipo_cambio = string.IsNullOrWhiteSpace(this.TipoCambio.Text) ? null : (decimal?)Convert.ToDecimal(this.TipoCambio.Text);
                detalle.descripcion = this.Descripcion.Text;
                detalle.importe = detalle.cantidad * detalle.valor_unitario;
                this.detalles.Add(detalle);

                Session["objDetalles"] = this.detalles;

                LlenarTotales();
                this.LimpiarConcepto();
            }
            catch (Exception ex)
            {
                this.EnviarError(ex.Message);
            }
        }

        protected void viewFacturas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Editar")
                {
                    this.detalles = Session["objDetalles"] as List<DataModel.ticket_detalle>;
                    DataModel.ticket_detalle detalle = this.detalles[Convert.ToInt32(e.CommandArgument)];

                    this.ClaveCargo.Text = detalle.no_identificacion;
                    this.Cantidad.Text = detalle.cantidad.ToString("0.00####");
                    this.Precio.Text = detalle.valor_unitario.ToString("0.00####");
                 //   this.CuentaPredial.Text = detalle.cuenta_predial;
                    this.Descuento.Text = detalle.descuento != null ? detalle.descuento.Value.ToString("0.00####") : "";
                  //  this.ISH.Text = detalle.ish != null ? detalle.ish.Value.ToString("0.00####") : "";
                    this.ISN.Text = detalle.isn != null ? detalle.isn.Value.ToString("0.00####") : "";
                    this.Descripcion.Text = detalle.descripcion;

                    this.detalles.Remove(detalle);

                    Session["objDetalles"] = this.detalles;

                    LlenarTotales();
                }
                else if (e.CommandName == "Eliminar")
                {
                    this.detalles = Session["objDetalles"] as List<DataModel.ticket_detalle>;
                    detalles.RemoveAt(Convert.ToInt32(e.CommandArgument));

                    Session["objDetalles"] = this.detalles;

                    LlenarTotales();
                }
            }
            catch (Exception ex)
            {
                this.EnviarError(ex.Message);
            }
        }

        private void LlenarObjetos()
        {
            this.cabecera = Session["objCabecera"] as DataModel.ticket;
            this.detalles = Session["objDetalles"] as List<DataModel.ticket_detalle>;
            this.retenciones = Session["objRetenciones"] as List<DataModel.ticket_retencion>;
            this.traslados = Session["objTraslados"] as List<DataModel.ticket_traslado>;
            this.retenciones_locales = Session["objRetencionesLocales"] as List<DataModel.ticket_retencion_local>;
            this.traslados_locales = Session["objTrasladoslocales"] as List<DataModel.ticket_traslado_local>;
            this.receptor = Session["objCabecera"] as DataModel.receptor;
            this.domicilio = Session["objCabecera"] as DataModel.domicilio;
            this.cabecera.metodo_pago = this.ddlMetodoPago.SelectedItem.Value; 
            this.cabecera.forma_pago = this.ddlFormaPago.SelectedItem.Value; 
            this.cabecera.motivo_decuento = string.Empty;
            this.cabecera.origen = this.Origen.Text;
            this.cabecera.contrato = this.Contrato.Text;
            this.cabecera.residencia_fiscal = string.IsNullOrWhiteSpace(this.ddlPais.Text) ? null : this.ddlPais.SelectedValue;
            this.cabecera.uso_cfdi = this.ddlUsoCFDI.SelectedValue;
            this.cabecera.tipo_relacion = this.ddlTipoRelacion.SelectedValue;
            this.cabecera.cfdi_relacionado = this.CfdiRelacionado.Text;

            using (var db = new DataModel.OstarDB())
            {
                int idFolio = Convert.ToInt32(Serie.SelectedValue);
                var folioSig = db.folio.Where(x => x.id == idFolio).FirstOrDefault();
                if (folioSig == null)
                    throw new Exception("No se encontro la serie seleccionada");
                else
                {
                    var tipo = db.tipo.Where(x => x.id == folioSig.tipo).FirstOrDefault();
                    this.cabecera.serie = folioSig.serie;
                    this.cabecera.folio = folioSig.folio_actual;
                    this.cabecera.tipo_comprobante = tipo.tipo_documento;
                    this.cabecera.tipo_documento = tipo.id;
                    comprobante = tipo.tipo_comprobante;
                }

                int idTipoCambio = Convert.ToInt32(Moneda.SelectedValue);
                var moneda = db.moneda.Where(x => x.id == idTipoCambio).FirstOrDefault();
                if (moneda == null)
                    throw new Exception("No se encontro la moneda seleccionada");
                else
                {
                    this.cabecera.moneda = moneda.abreviatura;
                    this.cabecera.tipo_cambio = string.IsNullOrWhiteSpace(this.TipoCambio.Text) ? null : (decimal?)Convert.ToDecimal(this.TipoCambio.Text);
                }

                var emi = db.emisor.Where(x => x.idemisor == Convert.ToInt32(Empresa.SelectedValue)).FirstOrDefault();
                if (emi != null)
                {
                    cabecera.emisor = emi.idemisor;
                    cabecera.rfc_emisor = seg.Desencriptar(emi.rfc);
                    cabecera.identificador = emi.identificador;
                }
            }
            this.cabecera.estatus = "Vigente";
            this.cabecera.fecha = DateTime.Now;
            this.cabecera.condiciones_pago = ""; // this.CondicionesPago.Text;
            this.cabecera.origen = this.Origen.Text;
            this.cabecera.contrato = this.Contrato.Text;
            this.cabecera.fecha_pago = string.IsNullOrWhiteSpace(this.FechaPago.Text) ? null : (DateTime?)DateTime.ParseExact(this.FechaPago.Text, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);

            decimal subtotal = detalles.Sum(x => x.importe);
            decimal descuento = detalles.Sum(x => (x.importe * (x.descuento ?? 0) / 100));
            decimal iva = detalles.Sum(x => (x.importe * (x.iva ?? 0)));
            decimal ieps = detalles.Sum(x => (x.importe * (x.ieps ?? 0)));
            decimal retencionIVA = detalles.Sum(x => (x.importe * (x.iva_ret ?? 0)));
            decimal retencionISR = detalles.Sum(x => (x.importe * (x.isr_ret ?? 0)));
            decimal ish = Math.Round(detalles.Sum(x => (x.importe * (x.ish ?? 0) / 100)), 2);
            decimal isn = Math.Round(detalles.Sum(x => (x.importe * (x.isn ?? 0) / 100)), 2);
            decimal total = subtotal - descuento + iva + ieps - retencionIVA - retencionISR + ish - isn;

            this.cabecera.subtotal = subtotal;
            this.cabecera.descuento = descuento;
            this.cabecera.total = total;

            this.traslados = new List<DataModel.ticket_traslado>();
            this.retenciones = new List<DataModel.ticket_retencion>();

            foreach(var det in detalles.Where(x => x.iva != null))
            {
                DataModel.ticket_traslado tras = new DataModel.ticket_traslado();
                tras.base_impuesto = det.importe;
                tras.impuesto = "002";
                tras.tasa = Convert.ToDecimal(det.iva);
                tras.importe = det.importe * Convert.ToDecimal(det.iva);
                tras.tipo_factor = det.tipo_factor_iva;
                traslados.Add(tras);
            }

            foreach (var det in detalles.Where(x => x.iva_ret != null))
            {
                DataModel.ticket_retencion ret = new DataModel.ticket_retencion();
                ret.base_impuesto = det.importe;
                ret.impuesto = "002";
                ret.tasa = Convert.ToDecimal(det.iva_ret);
                ret.importe = det.importe * Convert.ToDecimal(det.iva_ret);
                ret.tipo_factor = det.tipo_factor_ieps;
                retenciones.Add(ret);
            }

            foreach (var det in detalles.Where(x => x.isr_ret != null))
            {
                DataModel.ticket_retencion ret = new DataModel.ticket_retencion();
                ret.base_impuesto = det.importe;
                ret.impuesto = "001";
                ret.tasa = Convert.ToDecimal(det.isr_ret);
                ret.importe = det.importe * Convert.ToDecimal(det.isr_ret);
                ret.tipo_factor = det.tipo_factor_isr_ret;
                retenciones.Add(ret);
            }

            this.traslados_locales = new List<DataModel.ticket_traslado_local>();

            this.traslados_locales.AddRange(detalles.Where(x => x.ish != null).GroupBy(x => x.ish)
                .Select(g => new DataModel.ticket_traslado_local
                {
                    tasa = g.Key.Value,
                    impuesto = "ISH",
                    importe = g.Sum(x => (x.importe * (x.ish ?? 0) / 100))
                }));

            this.retenciones_locales = new List<DataModel.ticket_retencion_local>();
            this.retenciones_locales.AddRange(detalles.Where(x => x.isn != null).GroupBy(x => x.isn)
                .Select(g => new DataModel.ticket_retencion_local
                {
                    tasa = g.Key.Value,
                    impuesto = "ISN",
                    importe = g.Sum(x => (x.importe * (x.isn ?? 0) / 100))
                }));

            this.receptor = new DataModel.receptor();
            if (!string.IsNullOrWhiteSpace(this.idReceptor.Value))
                this.receptor.idreceptor = Convert.ToInt32(this.idReceptor.Value);
            this.receptor.rfc = string.IsNullOrEmpty(this.RFC.Text) ? "" : seg.Encriptar(this.RFC.Text);
            this.receptor.razon_social = string.IsNullOrEmpty(this.RazonSocial.Text) ? "" : seg.Encriptar(this.RazonSocial.Text);
            this.receptor.identificador = string.IsNullOrWhiteSpace(this.Identificador.Text) ? DateTime.Now.ToString("yyyyMMddhhmmss") : this.Identificador.Text;
            this.receptor.emisor = cabecera.emisor;
            this.cabecera.rfc_receptor = this.RFC.Text;

            Session["objCabecera"] = this.cabecera;
            Session["objDetalles"] = this.detalles;
            Session["objRetenciones"] = this.retenciones;
            Session["objTraslados"] = this.traslados;
            Session["objRetencionesLocales"] = this.retenciones_locales;
            Session["objTrasladoslocales"] = this.traslados_locales;
            Session["objReceptor"] = this.receptor;
            Session["objDomicilio"] = this.domicilio;
        }

        private void LlenarTotales()
        {
            this.detalles = Session["objDetalles"] as List<DataModel.ticket_detalle>;

            viewFacturas.DataSource = this.detalles;
            viewFacturas.DataBind();

            decimal subtotal = detalles.Sum(x => x.importe);
            decimal descuento = detalles.Sum(x => (x.importe * (x.descuento ?? 0) / 100));
            decimal iva = detalles.Sum(x => (x.importe * (x.iva ?? 0)));
            decimal ieps = detalles.Sum(x => (x.importe * (x.ieps ?? 0)));
            decimal retencionIVA = detalles.Sum(x => (x.importe * (x.iva_ret ?? 0)));
            decimal retencionISR = detalles.Sum(x => (x.importe * (x.isr_ret ?? 0)));
            decimal ish = Math.Round(detalles.Sum(x => (x.importe * (x.ish ?? 0) / 100)), 2);
            decimal isn = Math.Round(detalles.Sum(x => (x.importe * (x.isn ?? 0) / 100)), 2);
            decimal total = subtotal - descuento + iva + ieps - retencionIVA - retencionISR + ish - isn;

            this.Subtotal.Text = subtotal.ToString("0.00");
            this.DescuentoTotal.Text = descuento.ToString("0.00");
            this.divDescuento.Style.Add(HtmlTextWriterStyle.Display, descuento == 0 ? "none" : "block");
            this.IvaTotal.Text = iva.ToString("0.00");
            this.divIVA.Style.Add(HtmlTextWriterStyle.Display, iva == 0 ? "none" : "block");

            this.RetencionIvaTotal.Text = retencionIVA.ToString("0.00");
            this.divRetencionIVA.Style.Add(HtmlTextWriterStyle.Display, retencionIVA == 0 ? "none" : "block");
            this.RetencionIsrTotal.Text = retencionISR.ToString("0.00");
            this.divRetencionISR.Style.Add(HtmlTextWriterStyle.Display, retencionISR == 0 ? "none" : "block");
            this.IshTotal.Text = ish.ToString("0.00");
            this.divISH.Style.Add(HtmlTextWriterStyle.Display, ish == 0 ? "none" : "block");
            this.IsnTotal.Text = isn.ToString("0.00");
            this.divISN.Style.Add(HtmlTextWriterStyle.Display, isn == 0 ? "none" : "block");
            this.Total.Text = total.ToString("0.00");
        }

        protected void btnGenerar_Click(object sender, EventArgs e)
        {
            this.LlenarObjetos();
            using (var db = new DataModel.OstarDB())
            {
                try
                {
                    db.BeginTransaction();
                    int idReceptor = 0;
                    if (receptor.idreceptor == 0)
                    {
                        var idRec = db.InsertWithIdentity(receptor);
                        idReceptor = Convert.ToInt32(idRec);
                    }
                    else
                        idReceptor = receptor.idreceptor;
                    cabecera.receptor = idReceptor;
                    var idCabecera = db.InsertWithIdentity(cabecera);
                    cabecera.idticket = Convert.ToInt32(idCabecera);
                    foreach (DataModel.ticket_detalle det in detalles)
                    {
                        det.ticket = Convert.ToInt32(idCabecera);
                        det.fecha_pago = cabecera.fecha_pago;
                        var idDetalle = db.InsertWithIdentity(det);
                    }
                    foreach (DataModel.ticket_retencion det in retenciones)
                    {
                        det.ticket = Convert.ToInt32(idCabecera);
                        var idDetalle = db.InsertWithIdentity(det);
                    }
                    foreach (DataModel.ticket_traslado det in traslados)
                    {
                        det.ticket = Convert.ToInt32(idCabecera);
                        var idDetalle = db.InsertWithIdentity(det);
                    }
                    foreach (DataModel.ticket_retencion_local det in retenciones_locales)
                    {
                        det.ticket = Convert.ToInt32(idCabecera);
                        var idDetalle = db.InsertWithIdentity(det);
                    }
                    foreach (DataModel.ticket_traslado_local det in traslados_locales)
                    {
                        det.ticket = Convert.ToInt32(idCabecera);
                        var idDetalle = db.InsertWithIdentity(det);
                    }

                    GenerarFactura(db);
                    
                    db.CommitTransaction();
                    
                    Response.Redirect("~/Cliente/Factura");
                    Response.End();
                }
                catch (Exception ex)
                {
                    try { db.RollbackTransaction(); }
                    catch { }
                    btnGenerar.Enabled = true;
                    btnGenerar.Text = "Generar factura";

                    this.EnviarError(ex.Message);
                }
            }
        }

        private void GenerarFactura(DataModel.OstarDB db)
        {
            try
            {
                {
                    int idFolio = Convert.ToInt32(Serie.SelectedValue);
                    db.folio.Where(x => x.id == idFolio).Set(x => x.folio_actual, this.cabecera.folio + 1).Update();

                    var queryEmisor = db.emisor
                        .Where(t => t.idemisor == cabecera.emisor).FirstOrDefault();

                    var queryRegimen = db.cRegimenFiscal
                        .Where(r => r.cRegimenFiscal_id == queryEmisor.cRegimenFiscal_id).FirstOrDefault();

                    var certificado = db.certificado
                        .Where(t => t.idcertificado == queryEmisor.cetificado).FirstOrDefault();

                    var tipoDoc = db.tipo
                        .Where(t => t.id == cabecera.tipo_documento).FirstOrDefault();

                    Cfdi cfdi = new Cfdi();
                    cfdi.Comprobante.Fecha = cabecera.fecha.Value;
                    cfdi.Comprobante.TipoDeComprobante = (ComprobanteTipoDeComprobante)Enum.Parse(typeof(ComprobanteTipoDeComprobante), cabecera.tipo_comprobante);
                    if (!string.IsNullOrWhiteSpace(cabecera.serie))
                        cfdi.Comprobante.Serie = cabecera.serie;
                    if (cabecera.folio != 0)
                        cfdi.Comprobante.Folio = cabecera.folio.ToString();
                    cfdi.Comprobante.Certificado = Convert.ToBase64String(File.ReadAllBytes(rutaCertificado + queryEmisor.idemisor + "/" + certificado.csd));
                    cfdi.Comprobante.NoCertificado = cfdi.ObtenerNumeroCertificado(rutaCertificado + queryEmisor.idemisor + "/" + certificado.csd);
                    cfdi.Comprobante.MetodoPago = cabecera.metodo_pago;
                    cfdi.Comprobante.FormaPago = cabecera.forma_pago;
                    cfdi.Comprobante.SubTotal = Convert.ToDecimal(string.Format("{0:0.00}", cabecera.subtotal));

                    if(cabecera.tipo_relacion != "0")
                    {
                        cfdi.Comprobante.CfdiRelacionados = new ComprobanteCfdiRelacionados();
                        cfdi.Comprobante.CfdiRelacionados.TipoRelacion = cabecera.tipo_relacion;
                        cfdi.Comprobante.CfdiRelacionados.CfdiRelacionado = new ComprobanteCfdiRelacionadosCfdiRelacionado[1];
                        ComprobanteCfdiRelacionadosCfdiRelacionado rela = new ComprobanteCfdiRelacionadosCfdiRelacionado();
                        rela.UUID = Convert.ToString(cabecera.cfdi_relacionado);
                        cfdi.Comprobante.CfdiRelacionados.CfdiRelacionado[0] = rela;
                    }

                    if (!string.IsNullOrWhiteSpace(cabecera.moneda))
                        cfdi.Comprobante.Moneda = cabecera.moneda;
                    if (cabecera.descuento != 0)
                    {
                        cfdi.Comprobante.descuentoSpecified = true;
                        cfdi.Comprobante.Descuento = Convert.ToDecimal(string.Format("{0:0.00}", cabecera.descuento.Value));
                    }
                    else
                    {
                        cfdi.Comprobante.descuentoSpecified = true;
                        cfdi.Comprobante.Descuento = Convert.ToDecimal(string.Format("{0:0.00}", cabecera.descuento.Value));
                    }
                    if(cfdi.Comprobante.Moneda == "MXN")
                    {
                        cfdi.Comprobante.TipoCambio = Convert.ToDecimal(string.Format("{0:0}", cabecera.tipo_cambio));
                        cfdi.Comprobante.TipoCambioSpecified = true;
                    }
                    else
                    {
                        cfdi.Comprobante.TipoCambio = Convert.ToDecimal(string.Format("{0:0.00}", cabecera.tipo_cambio));
                        cfdi.Comprobante.TipoCambioSpecified = true;
                    }

                    cfdi.Comprobante.Emisor = new ComprobanteEmisor();
                    cfdi.Comprobante.Emisor.Nombre = seg.Desencriptar(queryEmisor.razon_social);
                    cfdi.Comprobante.Emisor.Rfc = seg.Desencriptar(queryEmisor.rfc);
                    cfdi.Comprobante.Emisor.RegimenFiscal = queryRegimen.codigo;
                    cfdi.Comprobante.LugarExpedicion = queryEmisor.lugar_expedicion;

                    cfdi.Comprobante.Receptor = new ComprobanteReceptor();
                    cfdi.Comprobante.Receptor.Nombre = this.RazonSocial.Text;
                    cfdi.Comprobante.Receptor.Rfc = this.RFC.Text;
                    cfdi.Comprobante.Receptor.UsoCFDI = cabecera.uso_cfdi;

                    cfdi.Comprobante.Conceptos = new ComprobanteConcepto[detalles.Count()];
                    int cont = 0;
                    foreach (DataModel.ticket_detalle det in detalles)
                    {
                        ComprobanteConcepto concepto = new ComprobanteConcepto();
                        concepto.ClaveProdServ = det.clave_prod_serv;
                        concepto.ClaveUnidad = det.clave_unidad;
                        if (!string.IsNullOrWhiteSpace(det.no_identificacion))
                            concepto.NoIdentificacion = det.no_identificacion;
                        concepto.Cantidad = det.cantidad;
                        concepto.Descripcion = det.descripcion;
                        concepto.ValorUnitario = Convert.ToDecimal(string.Format("{0:0.00}", det.valor_unitario));
                        concepto.Importe = Convert.ToDecimal(string.Format("{0:0.00}", det.importe));

                        
                        concepto.Impuestos = new ComprobanteConceptoImpuestos();
                        if (det.tipo_factor_iva != null)
                        {
                            string iva = string.Empty;
                            concepto.Impuestos.Traslados =  new ComprobanteConceptoImpuestosTraslado[detalles.Count];
                            ComprobanteConceptoImpuestosTraslado conceptoImpuestoTraslado = new ComprobanteConceptoImpuestosTraslado();
                            conceptoImpuestoTraslado.Base = Convert.ToDecimal(string.Format("{0:0.00}", det.importe));
                            conceptoImpuestoTraslado.Impuesto = "002";
                            conceptoImpuestoTraslado.TipoFactor = det.tipo_factor_iva;
                            if(det.tipo_factor_iva != "Exento")
                            {
                                IVAExento = true;
                                conceptoImpuestoTraslado.TasaOCuota = Convert.ToDecimal(det.iva);
                                tasa = Convert.ToDecimal(det.iva);
                                conceptoImpuestoTraslado.TasaOCuotaSpecified = true;
                                conceptoImpuestoTraslado.Importe = det.importe * Convert.ToDecimal(det.iva);
                                iva = conceptoImpuestoTraslado.Importe.ToString("N4").Remove(conceptoImpuestoTraslado.Importe.ToString("N4").Length - 2, 2);
                                conceptoImpuestoTraslado.Importe = Convert.ToDecimal(string.Format("{0:0.00}", iva));
                                conceptoImpuestoTraslado.ImporteSpecified = true;

                                
                                totalIVA = totalIVA + conceptoImpuestoTraslado.Importe;
                                if (tasa < Convert.ToDecimal(0.160000))
                                {
                                    IVACero = true;
                                }
                                if (tasa >= Convert.ToDecimal(0.160000))
                                {
                                    IVA16 = true;
                                }
                            }
                            concepto.Impuestos.Traslados[cont] = conceptoImpuestoTraslado;

                        }
                        if(det.iva_ret != null || det.isr_ret != null)
                        {
                            concepto.Impuestos.Retenciones = new ComprobanteConceptoImpuestosRetencion[2];
                            int cont2 = 0;
                            if (det.iva_ret != null)
                            {
                                RetIVA = true;
                                string ret002 = string.Empty;
                                ComprobanteConceptoImpuestosRetencion conceptoImpuestoRetencionIVA = new ComprobanteConceptoImpuestosRetencion();
                                conceptoImpuestoRetencionIVA.Base = Convert.ToDecimal(string.Format("{0:0.00}", det.importe));
                                conceptoImpuestoRetencionIVA.Impuesto = "002";
                                conceptoImpuestoRetencionIVA.TipoFactor = det.tipo_factor_iva_ret;
                                conceptoImpuestoRetencionIVA.TasaOCuota = Convert.ToDecimal(det.iva_ret);
                                tasa = Convert.ToDecimal(det.iva_ret);
                                conceptoImpuestoRetencionIVA.Importe = det.importe * Convert.ToDecimal(det.iva_ret);
                                ret002 = conceptoImpuestoRetencionIVA.Importe.ToString("N4").Remove(conceptoImpuestoRetencionIVA.Importe.ToString("N4").Length - 2, 2);
                                conceptoImpuestoRetencionIVA.Importe = Convert.ToDecimal(string.Format("{0:0.00}", ret002));
                                concepto.Impuestos.Retenciones[cont2] = conceptoImpuestoRetencionIVA;
                                cont2++;

                                totalRetIva += conceptoImpuestoRetencionIVA.Importe;
                            }

                            if (det.isr_ret != null)
                            {
                                RetISR = true;
                                ComprobanteConceptoImpuestosRetencion conceptoImpuestoRetencionISR = new ComprobanteConceptoImpuestosRetencion();
                                conceptoImpuestoRetencionISR.Base = Convert.ToDecimal(string.Format("{0:0.00}", det.importe));
                                conceptoImpuestoRetencionISR.Impuesto = "001";
                                conceptoImpuestoRetencionISR.TipoFactor = det.tipo_factor_isr_ret;
                                conceptoImpuestoRetencionISR.TasaOCuota = Convert.ToDecimal(det.isr_ret);
                                conceptoImpuestoRetencionISR.Importe = det.importe * Convert.ToDecimal(string.Format("{0:0.00}", det.isr_ret));
                                conceptoImpuestoRetencionISR.Importe = Convert.ToDecimal(string.Format("{0:0.00}", conceptoImpuestoRetencionISR.Importe));
                                concepto.Impuestos.Retenciones[cont2] = conceptoImpuestoRetencionISR;

                                totalRetIsr += conceptoImpuestoRetencionISR.Importe;
                            }
                        }
                        
                        cfdi.Comprobante.Conceptos[cont] = concepto;
                        cont++;

                        if (!string.IsNullOrWhiteSpace(det.cuenta_predial))
                        {
                            ComprobanteConceptoCuentaPredial predial = new ComprobanteConceptoCuentaPredial();
                            predial.Numero = det.cuenta_predial;
                            concepto.Items = new object[2];
                            concepto.Items[0] = predial;
                            
                        }
                    }

                    cont = IVACero == true && IVA16 == true ? 2 : 1;
                    int cont3 = 0;

                    if (IVA16 || IVACero || RetISR || RetIVA)
                    {
                        cfdi.Comprobante.Impuestos = new ComprobanteImpuestos();
                    }


                    cont = 0;

                    //if (traslados.Count > 0)
                    if (IVA16 || IVACero)
                    {
                        cfdi.Comprobante.Impuestos.Traslados = new ComprobanteImpuestosTraslado[traslados.Count];
                        if (IVA16)
                        {
                            ComprobanteImpuestosTraslado traslado = new ComprobanteImpuestosTraslado();
                            traslado.Impuesto = "002";
                            if (Math.Abs(totalIVA) > 0)
                            {
                                traslado.TasaOCuota = Convert.ToDecimal("0.160000");
                            }
                            traslado.Importe = Convert.ToDecimal(string.Format("{0:0.00}", totalIVA));
                            traslado.Importe = Math.Abs(traslado.Importe);
                            traslado.TipoFactor = "Tasa";
                            cfdi.Comprobante.Impuestos.Traslados[cont3] = traslado;
                            cont3 = cont3 + 1;
                        }
                        if (IVACero)
                        {
                            ComprobanteImpuestosTraslado traslado2 = new ComprobanteImpuestosTraslado();
                            traslado2.Impuesto = "002";
                            traslado2.TipoFactor = "Tasa";
                            traslado2.TasaOCuota = Convert.ToDecimal("0.000000");
                            traslado2.Importe = Convert.ToDecimal(string.Format("{0:0.00}", 0));
                            cfdi.Comprobante.Impuestos.Traslados[cont3] = traslado2;
                        }
                        

                        cfdi.Comprobante.Impuestos.totalImpuestosTrasladadosSpecified = true;
                        cfdi.Comprobante.Impuestos.TotalImpuestosTrasladados = Convert.ToDecimal(string.Format("{0:0.00}", totalIVA));
                        totalIVA = cfdi.Comprobante.Impuestos.TotalImpuestosTrasladados;
                    }

                    cont = 0;
                    //if (retenciones.Count > 0)
                    //{
                    //    cfdi.Comprobante.Impuestos.Retenciones = new ComprobanteImpuestosRetencion[retenciones.Count];
                    //    string ret002 = string.Empty;
                    //    foreach (DataModel.ticket_retencion ret in retenciones)
                    //    {
                    //        if(ret.impuesto == "002")
                    //        {
                    //            ComprobanteImpuestosRetencion retencion = new ComprobanteImpuestosRetencion();
                    //            retencion.Impuesto = "002"; 
                    //            ret002 = ret.importe.ToString("N4").Remove(ret.importe.ToString("N4").Length - 2, 2);
                    //            retencion.Importe = Convert.ToDecimal(string.Format("{0:0.00}", ret002));
                    //            cfdi.Comprobante.Impuestos.Retenciones[cont] = retencion;
                    //            totalRet = totalRet + retencion.Importe;
                    //            cont++;
                    //        }
                    //        else if(ret.impuesto == "001")
                    //        {
                    //            ComprobanteImpuestosRetencion retencion = new ComprobanteImpuestosRetencion();
                    //            retencion.Impuesto = "001"; 
                    //            retencion.Importe = Convert.ToDecimal(string.Format("{0:0.00}", ret.importe));
                    //            cfdi.Comprobante.Impuestos.Retenciones[cont] = retencion;
                    //            totalRet = totalRet + retencion.Importe;
                    //            cont++;
                    //        }
                    //    }
                    //    cfdi.Comprobante.Impuestos.TotalImpuestosRetenidosSpecified = true;
                    //    cfdi.Comprobante.Impuestos.TotalImpuestosRetenidos = Convert.ToDecimal(string.Format("{0:0.00}", totalRet)); //cfdi.Comprobante.Impuestos.Traslados.Sum(i => i.Importe);
                    //}
                    if(RetIVA || RetISR)
                    {
                        int countRet = 0;
                        countRet = RetIVA && RetISR ? countRet = 2 : 1;
                        cfdi.Comprobante.Impuestos.Retenciones = new ComprobanteImpuestosRetencion[countRet];

                        if (RetIVA)
                        {
                            ComprobanteImpuestosRetencion retencion = new ComprobanteImpuestosRetencion();
                            retencion.Impuesto = "002";
                            //ret002 = ret.importe.ToString("N4").Remove(ret.importe.ToString("N4").Length - 2, 2);
                            retencion.Importe = Convert.ToDecimal(string.Format("{0:0.00}", totalRetIva));
                            cfdi.Comprobante.Impuestos.Retenciones[cont] = retencion;
                            totalRet = totalRet + retencion.Importe;
                            cont++;
                        }
                        if (RetISR)
                        {
                            ComprobanteImpuestosRetencion retencion = new ComprobanteImpuestosRetencion();
                            retencion.Impuesto = "001";
                            retencion.Importe = Convert.ToDecimal(string.Format("{0:0.00}", totalRetIsr));
                            cfdi.Comprobante.Impuestos.Retenciones[cont] = retencion;
                            totalRet = totalRet + retencion.Importe;
                            cont++;
                        }
                        cfdi.Comprobante.Impuestos.TotalImpuestosRetenidosSpecified = true;
                        cfdi.Comprobante.Impuestos.TotalImpuestosRetenidos = Convert.ToDecimal(string.Format("{0:0.00}", totalRet)); //cfdi.Comprobante.Impuestos.Traslados.Sum(i => i.Importe);
                    }

                    #region impuestos locales
                    cont = 0;
                    if (traslados_locales.Count > 0 || retenciones_locales.Count > 0)
                    {
                        ImpuestosLocales impLocales = cfdi.AgregarImpuestosLocales();
                        if (traslados_locales.Count > 0)
                        {
                            impLocales.TrasladosLocales = new ImpuestosLocalesTrasladosLocales[traslados_locales.Count];
                            foreach (DataModel.ticket_traslado_local trasLoc in traslados_locales)
                            {
                                ImpuestosLocalesTrasladosLocales traslado = new ImpuestosLocalesTrasladosLocales();
                                traslado.ImpLocTrasladado = trasLoc.impuesto;
                                traslado.TasadeTraslado = trasLoc.tasa;
                                traslado.Importe = Math.Round(trasLoc.importe,2);
                                impLocales.TrasladosLocales[cont] = traslado;
                                cont++;
                            }
                            impLocales.TotaldeTraslados = impLocales.TrasladosLocales.Sum(i => i.Importe);
                        }
                        cont = 0;
                        if (retenciones_locales.Count > 0)
                        {
                            impLocales.RetencionesLocales = new ImpuestosLocalesRetencionesLocales[retenciones_locales.Count];
                            foreach (DataModel.ticket_retencion_local retLoc in retenciones_locales)
                            {
                                ImpuestosLocalesRetencionesLocales retencion = new ImpuestosLocalesRetencionesLocales();
                                retencion.ImpLocRetenido = retLoc.impuesto;
                                retencion.TasadeRetencion = retLoc.tasa;
                                retencion.Importe = Convert.ToDecimal(string.Format("{0:0.00}", retLoc.importe));//retLoc.importe;
                                impLocales.RetencionesLocales[cont] = retencion;
                                cont++;
                            }
                            impLocales.TotaldeRetenciones = impLocales.RetencionesLocales.Sum(i => i.Importe);
                        }
                        totalISH = impLocales.TotaldeTraslados;
                        totalISN = impLocales.TotaldeRetenciones;
                    }
                    #endregion
                    
                    #region donatarias
                    if(comprobante == "RECIBO DE DONATIVOS")
                    {
                        Donatarias donatarias = cfdi.AgregarDonatarias();
                        donatarias.version = Version;
                        if(cfdi.Comprobante.Emisor.Rfc == "BVA161128DZA")
                        {
                            donatarias.noAutorizacion = NoAutorizacion2;
                            donatarias.fechaAutorizacion = Convert.ToDateTime(FechaAutorizacion2);
                        }
                        else
                        {
                            donatarias.noAutorizacion = NoAutorizacion;
                            donatarias.fechaAutorizacion = Convert.ToDateTime(FechaAutorizacion);
                        }
                        
                        donatarias.leyenda = Leyenda;
                        
                    }
                    #endregion
                    
                    try
                    {
                        cfdi.Comprobante.Impuestos.TotalImpuestosRetenidos = Convert.ToDecimal(string.Format("{0:0.00}", cfdi.Comprobante.Impuestos.TotalImpuestosRetenidos));
                        cfdi.Comprobante.Total = cfdi.Comprobante.SubTotal + totalIVA - cfdi.Comprobante.Impuestos.TotalImpuestosRetenidos + totalISH - totalISN;
                        
                    }
                    catch
                    {
                        cfdi.Comprobante.Total = cfdi.Comprobante.SubTotal + totalIVA + totalISH - totalISN;
                    }
                    
                    cfdi.Comprobante.Sello = cfdi.ObtenerSelloDigital(rutaCertificado + queryEmisor.idemisor + "/" + certificado.llave_privada, seg.Desencriptar(certificado.contrasenia), rutaCadenaOriginal,"");

                    string rutaXML = string.Empty;
                    string uuid = string.Empty;
                    rutaXML = rutaComprobantes + cabecera.identificador + "/" + DateTime.Now.ToString("yyyy-MM") + "/";
                    if (!Directory.Exists(rutaXML))
                    {
                        Directory.CreateDirectory(rutaXML);
                    }
                    rutaXML = rutaXML + cfdi.Comprobante.Serie + ".xml";
                    cfdi.GuardarXml(rutaXML);
                    if (PAC == "1")
                    {
                        DSTimbrado timbreDFacture = new DSTimbrado();
                        if (seg.Desencriptar(queryEmisor.rfc) != "GAGJ7712024S3")
                            timbreDFacture.TipoTimbrado = Daysoft.DICI.Facturador.DFacture.wsDaySoft.TypeCFD.Production;
                        else
                            timbreDFacture.TipoTimbrado = Daysoft.DICI.Facturador.DFacture.wsDaySoft.TypeCFD.Test;

                        timbreDFacture.TimbrarCFDI(cfdi.ObtenerXml());

                        rutaXML = rutaComprobantes + cabecera.identificador + "/" + timbreDFacture.ObtenerFechaTimbrado().ToString("yyyy-MM") + "/";

                        if (!Directory.Exists(rutaXML))
                        {
                            Directory.CreateDirectory(rutaXML);
                        }
                        rutaXML = rutaXML + timbreDFacture.ObtenerUUID() + ".xml";

                        timbreDFacture.GuardarCFDTimbrado(rutaXML);
                        uuid = timbreDFacture.ObtenerUUID();

                        var factura = db.factura.Insert(() => new DataModel.factura
                        {
                            uuid = timbreDFacture.ObtenerUUID(),
                            fecha_timbrado = timbreDFacture.ObtenerFechaTimbrado(),
                            ticket = cabecera.idticket
                        });

                    }

                    try
                    {
                        Reporte.Imprimir imp = new Reporte.Imprimir();
                        string rutaImagen = string.Empty;
                        string rutaCedula = string.Empty;
                        if (!string.IsNullOrEmpty(queryEmisor.logotipo))
                            rutaImagen = rutaCertificado + queryEmisor.idemisor + "/" + queryEmisor.logotipo;
                        if (!string.IsNullOrEmpty(queryEmisor.cedula))
                            rutaCedula = rutaCertificado + queryEmisor.idemisor + "/" + queryEmisor.cedula;
                        imp.Origen = cabecera.origen;
                        imp.Contrato = cabecera.contrato;
                        imp.FechaPago = cabecera.fecha_pago == null ? "" : cabecera.fecha_pago.Value.ToString("yyyy-MM-dd");
                        imp.CrearPDF(rutaXML, rutaXML.Replace(".xml", ".pdf"), Server.MapPath("~"), rutaImagen, rutaCedula, queryEmisor.tipo_pdf == null ? 1 : queryEmisor.tipo_pdf.Value, tipoDoc == null ? "FACTURA" : tipoDoc.tipo_comprobante, totalIVA);
                    }

                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    Session.Add("uuid", uuid);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void ddlMetodoPago_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        
        protected void ddlFormaPago_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void ddlUsoCFDI_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void ddlddlTipoRelacion_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void ddlPais_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void DdlUnidad_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void DdlcProdServ_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        protected void DdlTipoFactorIVA_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void DdlTasaoCuotaIVA_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //
        protected void DdlTasaoCuotaIVAret_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void DdlTipoFactorIVAret_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void DdlTasaoCuotaISRret_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void DdlTipoFactorISRret_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private bool ValidarMetodoPago(string id)
        {
            using (var db = new DataModel.OstarDB())
            {
                var query = db.pago.Where(x => x.id == id).FirstOrDefault();                 
                return query == null ? false : true;
            }
        }

        protected void LlenarEmpresas(int id)
        {
            using (var db = new DataModel.OstarDB())
            {
                var empresas = (from h in db.receptor
                                where h.emisor == id && h.identificador.Contains(this.BuscarIdentificador.Text)
                                select new
                                {
                                    h.idreceptor,
                                    razon_social = string.IsNullOrEmpty(h.razon_social) ? "" : seg.Desencriptar(h.razon_social),
                                    rfc = string.IsNullOrEmpty(h.rfc) ? "" : seg.Desencriptar(h.rfc),
                                    identificador = string.IsNullOrEmpty(h.identificador) ? "" : h.identificador
                                }).ToList();

                viewEmpresas.DataSource = empresas.Where(x => x.rfc.Contains(this.BuscarRFC.Text.ToUpper()) && x.razon_social.ToUpper().Contains(this.BuscarRazonSocial.Text.ToUpper()));
                viewEmpresas.DataBind();
            }
        }

        protected void viewEmpresas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Seleccionar")
                {
                    int id = Convert.ToInt32(viewEmpresas.DataKeys[Convert.ToInt32(e.CommandArgument)].Value);
                    this.idReceptor.Value = id.ToString();
                    LlenarCliente(id);

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append(@"<script type='text/javascript'>");
                    sb.Append("$('#modalCliente').modal('hide');");
                    sb.Append(@"</script>");
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ProductoModalScript", sb.ToString(), false);
                }
            }
            catch (Exception ex)
            {
                this.EnviarError(ex.Message);
            }
        }

        private void LlenarCliente(int id)
        {
            using (var db = new DataModel.OstarDB())
            {
                var query =
                    (from r in db.receptor
                     join d in db.domicilio on r.domicilio equals d.iddomicilio
                     where r.idreceptor == id
                     select new
                     {
                         r.idreceptor,
                         r.identificador,
                         r.razon_social,
                         r.rfc,
                         d.calle,
                         d.no_exterior,
                         d.no_interior,
                         d.colonia,
                         d.localidad,
                         d.referencia,
                         d.municipio,
                         d.estado,
                         d.pais,
                         d.codigo_postal,
                     }).FirstOrDefault();
                if (query != null)
                {
                    this.idReceptor.Value = query.idreceptor.ToString();
                    this.Identificador.Text = string.IsNullOrEmpty(query.identificador) ? "" : query.identificador;
                    this.RazonSocial.Text = string.IsNullOrEmpty(query.razon_social) ? "" : seg.Desencriptar(query.razon_social);
                    this.RFC.Text = string.IsNullOrEmpty(query.rfc) ? "" : seg.Desencriptar(query.rfc);
                }
                else
                {
                    this.idReceptor.Value = string.Empty;
                }
            }
        }

        protected void viewProductos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Seleccionar")
                {
                    int id = Convert.ToInt32(viewProductos.DataKeys[Convert.ToInt32(e.CommandArgument)].Value);
                    using (var db = new DataModel.OstarDB())
                    {
                        DataModel.producto concepto = db.producto
                            .Where(x => x.id == id)
                            .FirstOrDefault();
                        if (concepto != null)
                        {
                            this.LlenarProducto(concepto);

                            System.Text.StringBuilder sb = new System.Text.StringBuilder();
                            sb.Append(@"<script type='text/javascript'>");
                            sb.Append("$('#modalProducto').modal('hide');");
                            sb.Append(@"</script>");
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ProductoModalScript", sb.ToString(), false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.EnviarError(ex.Message);
            }
        }

        protected void btnSearchProducto_Click(object sender, EventArgs e)
        {
            try
            {
                this.BuscarClave.Text = this.ClaveCargo.Text.Trim();
                this.BuscarDescripcion.Text = string.Empty;
                
                this.LlenarProductos(Convert.ToInt32(Empresa.SelectedValue));

                this.titleProducto.Text = "Busqueda de productos";

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#modalProducto').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ProductoModalScript", sb.ToString(), false);
            }
            catch(Exception ex)
            {
                this.EnviarError(ex.Message);
            }
        }

        protected void LlenarProductos(int id)
        {
            using (var db = new DataModel.OstarDB())
            {
                var prodcutos = db.producto.Where(x => x.emisor == id 
                    && x.codigo.Contains(this.BuscarClave.Text) 
                    && x.descripcion.Contains(this.BuscarDescripcion.Text)).ToList();

                viewProductos.DataSource = prodcutos;
                viewProductos.DataBind();
            }
        }

        protected void IvaRetenido_TextChanged(object sender, EventArgs e)
        {
            LlenarTotales();
        }

        protected void IsrRetenido_TextChanged(object sender, EventArgs e)
        {
            LlenarTotales();
        }

        protected void IsnRetenido_TextChanged(object sender, EventArgs e)
        {
            LlenarTotales();
        }
        private void LimpiarConcepto()
        {
            this.ClaveCargo.Text = string.Empty;
          //  this.CuentaPredial.Text = string.Empty;
            this.Descripcion.Text = string.Empty;
            this.Descuento.Text = string.Empty;
          //  this.ISH.Text = string.Empty;
            this.ISN.Text = string.Empty;
            this.Precio.Text = string.Empty;
            this.Cantidad.Text = string.Empty;
            LlenarClaveUnidad();
            LlenarcProdServ(Convert.ToInt32(Empresa.SelectedValue));
            LlenarTasaOCuota();
            LlenarTipoFactor();
        }

        private void EnviarError(string mensaje)
        {
            lblError.Text = mensaje;
            lblErrorPie.Text = mensaje;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#msg_error').show();");
            sb.Append("$('#msg_error_pie').show();");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ErrorModalScript", sb.ToString(), false);
        }

        protected void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            LlenarEmpresas(Convert.ToInt32(Empresa.SelectedValue));
        }

        protected void btnBuscarProducto_Click(object sender, EventArgs e)
        {
            this.LlenarProductos(Convert.ToInt32(Empresa.SelectedValue));
        }

        protected void RFC_TextChanged(object sender, EventArgs e)
        {
            if (RFC.Text != "XAXX010101000" && RFC.Text != "XEXX010101000")
            {
                int idCli = 0;
                using (var db = new DataModel.OstarDB())
                {
                    var rec = db.receptor.Where(x => (x.rfc == seg.Encriptar(this.RFC.Text) && x.emisor == Convert.ToInt32(Empresa.SelectedValue))).FirstOrDefault();

                    if (rec != null)
                    {
                        idCli = rec.idreceptor;
                        this.idReceptor.Value = rec.idreceptor.ToString();
                    }
                    else
                    {
                        this.idReceptor.Value = string.Empty;
                    }
                }
                if (idCli != 0)
                    this.LlenarCliente(idCli);
            }
        }

        protected void Moneda_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.LlenarTipoCambio(Convert.ToInt32(Moneda.SelectedValue));
        }

        private void LlenarTipoCambio(int id)
        {
            using (var db = new DataModel.OstarDB())
            {
                var query = db.moneda.Where(x => x.id == id).FirstOrDefault();
                if (query != null)
                    TipoCambio.Text = query.abreviatura == "MXN" ? "1" : string.Empty;
                else
                    TipoCambio.Text = string.Empty;
            }
        }

        protected void Serie_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.MostrarRetenciones(Convert.ToInt32(Serie.SelectedValue));
            this.MostrarRetencionesLocales(Convert.ToInt32(Serie.SelectedValue));
        }

        public Donatarias AgregarDonatarias()
        {
            try
            {
                donatarias1 = new Donatarias();
                return this.Dontarias;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private XmlElement ProcesarDonatarias()
        {
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("donat", "http://www.sat.gob.mx/donat");
            XmlSerializer serializer = new XmlSerializer(typeof(Donatarias));

            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, donatarias1, namespaces);

            XmlDocument xmlDonatarias = new XmlDocument();
            xmlDonatarias.LoadXml(writer.ToString());

            return xmlDonatarias.DocumentElement;
        }

        public ImpuestosLocales AgregarImpuestosLocales()
        {
            try
            {
                impuestosLocales = new ImpuestosLocales();
                return this.ImpuestosLocales;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private XmlElement ProcesarImpuestosLocales()
        {
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("implocal", "http://www.sat.gob.mx/implocal");
            XmlSerializer serializer = new XmlSerializer(typeof(ImpuestosLocales));

            StringWriter writer = new Utf8StringWriter();
            serializer.Serialize(writer, impuestosLocales, namespaces);

            XmlDocument xmlImpuesoslocales = new XmlDocument();
            xmlImpuesoslocales.LoadXml(writer.ToString());

            return xmlImpuesoslocales.DocumentElement;
        }

        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }

        private void MostrarRetencionesLocales(int id)
        {
            using (var db = new DataModel.OstarDB())
            {
                var query = db.folio.Where(x => x.id == id).FirstOrDefault();
                if (query != null)
                {
                    if ((query.emisor == 87 || query.emisor == 88) || query.emisor == 1 || query.emisor == 96 || query.emisor == 97)
                    {
                        divIsnRetenido.Style.Remove(HtmlTextWriterStyle.Display);
                        divIsnRetenido.Style.Add(HtmlTextWriterStyle.Display, "block");
                    }
                    else
                    {
                        divIsnRetenido.Style.Remove(HtmlTextWriterStyle.Display);
                        divIsnRetenido.Style.Add(HtmlTextWriterStyle.Display, "none");
                        ISN.Text = string.Empty;
                    }
                }
                else
                {
                    divIsnRetenido.Style.Remove(HtmlTextWriterStyle.Display);
                    divIsnRetenido.Style.Add(HtmlTextWriterStyle.Display, "none");
                    ISN.Text = string.Empty;
                }
            }
            this.LlenarTotales();
        }

        private void MostrarRetenciones(int id)
        {
            using(var db = new DataModel.OstarDB())
            {
                var query = db.folio.Where(x => x.id == id).FirstOrDefault();
                if(query!=null)
                {
                    //if(query.tipo == 5 || query.tipo == 4 || query.emisor == 25 || query.emisor == 81 || query.emisor == 80 || query.emisor == 1)
                    if (query.tipo == 5 || query.tipo == 4 || mostrarRetenciones.Contains(query.emisor.ToString()))
                    {
                        //IVA
                        divIvaRetenido.Style.Remove(HtmlTextWriterStyle.Display);
                        divIvaRetenido.Style.Add(HtmlTextWriterStyle.Display, "block");

                        divFacIvaRetenido.Style.Remove(HtmlTextWriterStyle.Display);
                        divFacIvaRetenido.Style.Add(HtmlTextWriterStyle.Display, "block");
                        //ISR
                        divIsrRetenido.Style.Remove(HtmlTextWriterStyle.Display);
                        divIsrRetenido.Style.Add(HtmlTextWriterStyle.Display, "block");

                        divFacIsrRetenido.Style.Remove(HtmlTextWriterStyle.Display);
                        divFacIsrRetenido.Style.Add(HtmlTextWriterStyle.Display, "block");
                    }
                    else
                    {
                        //IVA
                        divIvaRetenido.Style.Remove(HtmlTextWriterStyle.Display);
                        divIvaRetenido.Style.Add(HtmlTextWriterStyle.Display, "none");

                        divFacIvaRetenido.Style.Remove(HtmlTextWriterStyle.Display);
                        divFacIvaRetenido.Style.Add(HtmlTextWriterStyle.Display, "none");
                        //ISR
                        divIsrRetenido.Style.Remove(HtmlTextWriterStyle.Display);
                        divIsrRetenido.Style.Add(HtmlTextWriterStyle.Display, "none");

                        divFacIsrRetenido.Style.Remove(HtmlTextWriterStyle.Display);
                        divFacIsrRetenido.Style.Add(HtmlTextWriterStyle.Display, "none");
                    }
                }
                else
                {
                    //IVA
                    divIvaRetenido.Style.Remove(HtmlTextWriterStyle.Display);
                    divIvaRetenido.Style.Add(HtmlTextWriterStyle.Display, "none");

                    divFacIvaRetenido.Style.Remove(HtmlTextWriterStyle.Display);
                    divFacIvaRetenido.Style.Add(HtmlTextWriterStyle.Display, "none");
                    //ISR
                    divIsrRetenido.Style.Remove(HtmlTextWriterStyle.Display);
                    divIsrRetenido.Style.Add(HtmlTextWriterStyle.Display, "none");

                    divFacIsrRetenido.Style.Remove(HtmlTextWriterStyle.Display);
                    divFacIsrRetenido.Style.Add(HtmlTextWriterStyle.Display, "none");
                }
            }
            this.LlenarTotales();
        }
    }
}