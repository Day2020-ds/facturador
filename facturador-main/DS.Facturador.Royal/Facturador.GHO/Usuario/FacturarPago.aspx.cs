using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class FacturarPago : System.Web.UI.Page
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
        private DataModel.ticket cabecera;
        private List<DataModel.ticket_detalle> detalles;
        private List<DataModel.ticket_retencion> retenciones;
        private List<DataModel.ticket_traslado> traslados;
        private List<DataModel.ticket_retencion_local> retenciones_locales;
        private List<DataModel.ticket_traslado_local> traslados_locales;
        private DataModel.receptor receptor;
        private DataModel.domicilio domicilio;
        private List<DoctosRelacionados> relacionados;

        private decimal totalIVA = 0;
        private decimal totalISH = 0;
        private decimal totalISN = 0;
        private decimal tasa = 0;
        private string tipofactor;
        private decimal totalRet;
        private bool IVACero = false;

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

                    relacionados = new List<DoctosRelacionados>();
                    Session["objRelacionados"] = this.relacionados;

                    LlenarEmpresas();
                    LlenarMoneda();
                    this.LLenarFolios(Convert.ToInt32(Empresa.SelectedValue));
                    LlenarFormaPago();
                    LlenarPais();
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

                pag.Add("", "0");
                ddlPais.DataTextField = "Key";
                ddlPais.DataValueField = "Value";
                ddlPais.DataSource = pag.Reverse();
                ddlPais.DataBind();
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

                    var query = from f in db.folio
                                join t in db.tipo on f.tipo equals t.id
                                where f.emisor == idEmisor && t.tipo_documento == "P"
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

                }
            }
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
        }

        private void LlenarProducto(DataModel.producto producto)
        {
        }

        protected void btnAgregarRelacionado_Click(object sender, EventArgs e)
        {
            try
            {
                this.detalles = Session["objDetalles"] as List<DataModel.ticket_detalle>;

                DataModel.ticket_detalle detalle = new DataModel.ticket_detalle();
                detalle.importe = detalle.cantidad * detalle.valor_unitario;
                this.detalles.Add(detalle);

                Session["objDetalles"] = this.detalles;
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
                if (e.CommandName == "Agregar")
                {
                    this.relacionados = Session["objRelacionados"] as List<DoctosRelacionados>;
                    int index = Convert.ToInt32(e.CommandArgument);
                    DoctosRelacionados relacionado = new DoctosRelacionados();
                    GridViewRow row = viewFacturas.Rows[index];

                    
                    relacionado.uuid = viewFacturas.Rows[index].Cells[0].Text;
                    relacionado.serie = viewFacturas.Rows[index].Cells[1].Text;
                    relacionado.folio = viewFacturas.Rows[index].Cells[2].Text;
                    relacionado.moneda = viewFacturas.Rows[index].Cells[4].Text;
                    relacionado.tipo_cambio = Convert.ToDecimal(viewFacturas.Rows[index].Cells[5].Text);
                    relacionado.total = Convert.ToDecimal(viewFacturas.Rows[index].Cells[7].Text);

                    relacionado.saldoAnterior = Convert.ToDecimal(SaldoAnterior.Text);
                    relacionado.importePagado = Convert.ToDecimal(ImportePagado.Text);
                    relacionado.parcialidad = parcialidad.Text;

                    this.relacionados.Add(relacionado);

                    Session["objRelacionados"] = this.relacionados;
                    LlenarRelacionados();
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
            this.cabecera.forma_pago = this.ddlFormaPago.SelectedItem.Value;
            this.cabecera.motivo_decuento = string.Empty;
            this.cabecera.residencia_fiscal = string.IsNullOrWhiteSpace(this.ddlPais.Text) ? null : this.ddlPais.SelectedValue;

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

            decimal subtotal = 0;
            decimal descuento = 0;
            decimal total = 0;

            this.cabecera.subtotal = subtotal;
            this.cabecera.descuento = descuento;
            this.cabecera.total = total;

            this.traslados = new List<DataModel.ticket_traslado>();
            this.retenciones = new List<DataModel.ticket_retencion>();

            
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

        private void LlenarRelacionados()
        {
            this.relacionados = Session["objRelacionados"] as List<DoctosRelacionados>;
            viewRelacionados.DataSource = this.relacionados;
            viewRelacionados.DataBind();

            this.ImportePagado.Text = string.Empty;
            this.SaldoAnterior.Text = string.Empty;
        }

        protected void viewFacturas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ObtenerComprobantes(); 
            viewFacturas.PageIndex = e.NewPageIndex;
            viewFacturas.DataBind();

        }

        protected void viewFacturas_RowCreated(object sender, GridViewRowEventArgs e)
        {
            LinkButton button = (LinkButton)e.Row.FindControl("LinkCancelar");
            if (button != null)
            {
                ScriptManager.GetCurrent(this.Page).RegisterAsyncPostBackControl(button);
            }
        }

        protected void Consultar(object sender, EventArgs e)
        {
            viewFacturas.PageIndex = 0;
            ObtenerComprobantes();
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
                    this.relacionados = Session["objRelacionados"] as List<DoctosRelacionados>; 
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
                    cfdi.Comprobante.SubTotal = Convert.ToDecimal(string.Format("{0:0}", cabecera.subtotal));
                    
                    cfdi.Comprobante.Moneda = "XXX";

                    cfdi.Comprobante.Emisor = new ComprobanteEmisor();
                    cfdi.Comprobante.Emisor.Nombre = seg.Desencriptar(queryEmisor.razon_social);
                    cfdi.Comprobante.Emisor.Rfc = seg.Desencriptar(queryEmisor.rfc);
                    cfdi.Comprobante.Emisor.RegimenFiscal = queryRegimen.codigo;
                    cfdi.Comprobante.LugarExpedicion = queryEmisor.lugar_expedicion;

                    cfdi.Comprobante.Receptor = new ComprobanteReceptor();
                    cfdi.Comprobante.Receptor.Nombre = this.RazonSocial.Text;
                    cfdi.Comprobante.Receptor.Rfc = this.RFC.Text;
                    cfdi.Comprobante.Receptor.UsoCFDI = "P01";

                    //Concepto
                    cfdi.Comprobante.Conceptos = new ComprobanteConcepto[1];
                    ComprobanteConcepto compr_concepto = new ComprobanteConcepto();
                    compr_concepto.Cantidad = Convert.ToDecimal(string.Format("{0:0}", 1));
                    compr_concepto.ClaveProdServ = "84111506";
                    compr_concepto.ClaveUnidad = "ACT";
                    compr_concepto.Descripcion = "Pago";
                    compr_concepto.ValorUnitario = Convert.ToDecimal(string.Format("{0:0}", 0));
                    compr_concepto.ValorUnitario = Math.Abs(compr_concepto.ValorUnitario);

                    compr_concepto.Importe = Convert.ToDecimal(string.Format("{0:0}", 0));
                    compr_concepto.Importe = Math.Abs(compr_concepto.Importe);

                    cfdi.Comprobante.Conceptos[0] = compr_concepto;

                    cfdi.Comprobante.Total = 0;

                    var query = db.moneda.Where(x => x.id == Convert.ToInt32(Moneda.SelectedItem.Value)).FirstOrDefault();

                    Pagos pagos = cfdi.AgregarComplementoPagos();
                    pagos.Pago = new PagosPago[1];
                    PagosPago pago = new PagosPago();
                    pago.FechaPago =  Convert.ToDateTime(FechaPago.Text);
                    pago.FormaDePagoP = this.ddlFormaPago.SelectedItem.Value;
                    pago.MonedaP = query.abreviatura;
                    if (pago.MonedaP != "MXN")
                    {
                        pago.TipoCambioP = Convert.ToDecimal(string.Format("{0:0.00}", TipoCambio.Text));
                        pago.TipoCambioPSpecified = true;
                    }
                    double mont = Convert.ToDouble(Monto.Text);
                    pago.Monto = string.Format("{0:0.00}", mont);
                    pagos.Pago[0] = pago;

                    pago.DoctoRelacionado = new PagosPagoDoctoRelacionado[relacionados.Count()];
                    int count = 0;
                    int contador = 1;
                    foreach (var doctoRelacionado in relacionados)
                    {
                        PagosPagoDoctoRelacionado relacionado = new PagosPagoDoctoRelacionado();
                        relacionado.IdDocumento = Convert.ToString(doctoRelacionado.uuid);
                        relacionado.Serie = doctoRelacionado.serie;
                        relacionado.Folio = doctoRelacionado.folio;
                        relacionado.MonedaDR = doctoRelacionado.moneda;
                        if (relacionado.MonedaDR != pago.MonedaP && relacionado.MonedaDR == "USD")
                        {
                            relacionado.TipoCambioDR = Convert.ToDecimal(doctoRelacionado.tipo_cambio);
                            relacionado.TipoCambioDRSpecified = true;
                        }
                        relacionado.MetodoDePagoDR = "PPD";
                        relacionado.NumParcialidad = doctoRelacionado.parcialidad;
                        relacionado.ImpSaldoAnt = string.Format("{0:0.00}", doctoRelacionado.saldoAnterior);
                        relacionado.ImpPagado = string.Format("{0:0.00}", doctoRelacionado.importePagado);
                        decimal impSaldoInsoluto = Convert.ToDecimal(doctoRelacionado.saldoAnterior - doctoRelacionado.importePagado);
                        relacionado.ImpSaldoInsoluto = string.Format("{0:0.00}", impSaldoInsoluto);

                        pago.DoctoRelacionado[count] = relacionado;
                        count++;
                        contador++;
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
                        if (seg.Desencriptar(queryEmisor.rfc) != "AAA010101AAA")
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
                        imp.CrearPDF(rutaXML, rutaXML.Replace(".xml", ".pdf"), Server.MapPath("~"), rutaImagen, rutaCedula, queryEmisor.tipo_pdf == null ? 1 : queryEmisor.tipo_pdf.Value, tipoDoc == null ? "FACTURA" : tipoDoc.tipo_comprobante, 0);
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

        protected void ddlFormaPago_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void ddlPais_SelectedIndexChanged(object sender, EventArgs e)
        {

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
                    int idEmisor = Convert.ToInt32(Empresa.Text);
                    var query2 = (from f in db.factura
                                  join t in db.ticket on f.ticket equals t.idticket
                                  where  (idEmisor == 0 ? true : t.emisor == idEmisor) && (RFC.Text == string.Empty ? true : t.rfc_receptor.Contains(RFC.Text))
                                  && t.metodo_pago == "PPD"
                                  orderby f.fecha_timbrado ascending
                                  select new
                                  {
                                      idEmisor = t.emisor,
                                      uuid = f.uuid,
                                      serie = t.serie,
                                      folio = t.folio,
                                      fecha_timbrado = f.fecha_timbrado,
                                      moneda = t.moneda,
                                      tipo_cambio = t.tipo_cambio,
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
                    viewFacturas.DataSource = query2.ToList();
                    viewFacturas.DataBind();
                }
            }

            catch (Exception ex)
            {

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
    }
}