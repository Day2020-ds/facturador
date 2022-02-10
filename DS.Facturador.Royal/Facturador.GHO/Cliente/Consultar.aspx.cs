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
    public partial class Consultar : System.Web.UI.Page
    {
        private Seguridad seg;
        private License lic;
        string rutaLicencia = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                seg = new Seguridad();
                LeerLicencia();
                LlenarEmpresas();
            }
            catch (Exception ex)
            { ErrorMessage.Text = ex.Message; }
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
                var query = (from h in db.emisor
                             select new
                             {
                                 h.idemisor,
                                 razon_social = seg.Desencriptar(h.razon_social),
                                 rfc = seg.Desencriptar(h.rfc)
                             }).Take(lic.ObtenerEmpresas());

                Empresa.DataValueField = "idemisor";
                Empresa.DataTextField = "razon_social";
                Empresa.DataSource = query;
                Empresa.DataBind();
            }
        }

        protected void ConsultarUUID(object sender, EventArgs e)
        {
            try
            {
                bool existe = false;
                string uuid = string.Empty;
                using (var db = new DataModel.OstarDB())
                {
                    var factura = db.factura
                        .Where(f => f.uuid == this.UUID.Text).FirstOrDefault();

                    if (factura != null)
                    {
                        existe = true;
                        uuid = factura.uuid;
                    }
                }
                if (existe)
                {
                    Session.Add("uuid", uuid);
                    Response.Redirect("Factura");
                    Response.End();
                }
                else
                {
                    ErrorMessage.Text = "No existen la factura con el folio fiscal indicado";
                }
            }
            catch(Exception ex)
            {
                ErrorMessage.Text = ex.Message;
            }
        }

        protected void ConsultarTicket(object sender, EventArgs e)
        {
            bool exite = false;
            string uuid = string.Empty;
            try
            {
                using (var db = new DataModel.OstarDB())
                {
                    var ticket = db.ticket
                        .Where(t =>
                            t.serie == this.Ticket.Text &&
                            //t.folio == Convert.ToInt32(this.Folio.Text) &&
                        t.total == Convert.ToDecimal(this.Total.Text) &&
                        t.emisor == Convert.ToInt32(this.Empresa.SelectedValue)).FirstOrDefault();

                    if (ticket != null)
                    {
                        var factura = db.factura
                            .Where(f => f.ticket == ticket.idticket).FirstOrDefault();

                        if (factura != null)
                        {
                            exite = true;
                            uuid = factura.uuid;
                        }
                        else
                        {
                            ErrorMessage.Text = "La factura no se ha generado.";
                        }
                    }
                    else
                    {
                        ErrorMessage.Text = "No se encuentra la factura solicitada.";
                    }
                }
                if (exite)
                {
                    Session.Add("uuid", uuid);
                    Response.Redirect("Factura");
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = ex.Message;
            }
        }
    }
}