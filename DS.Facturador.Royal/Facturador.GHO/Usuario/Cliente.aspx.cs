using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Facturador.GHO.Controllers;
using BLToolkit.Data.Linq;
using System.IO;
using Microsoft.AspNet.Identity;

namespace Facturador.GHO.Usuario
{
    public partial class Cliente : System.Web.UI.Page
    {
        Seguridad seg;
        private License lic;
        string rutaLicencia = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                seg = new Seguridad();
                LeerLicencia();
                if (!IsPostBack)
                {
                    //LlenarNacionalidad();
                    LlenarEmisores();
                    LlenarEmpresas(Convert.ToInt32(Empresa.SelectedValue));
                }
                ErrorMessage.Text = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = ex.Message;
            }
        }
        protected void LlenarEmpresas(int id)
        {
            using (var db = new DataModel.OstarDB())
            {
                var empresas = (from h in db.receptor
                                where h.emisor == id
                                select new
                                {
                                    h.idreceptor,
                                    razon_social = string.IsNullOrEmpty(h.razon_social) ? "" : seg.Desencriptar(h.razon_social),
                                    rfc = string.IsNullOrEmpty(h.rfc) ? "" : seg.Desencriptar(h.rfc),
                                    identificador = string.IsNullOrEmpty(h.identificador) ? "" : h.identificador,
                                    curp=string.IsNullOrEmpty(h.curp) ? "":h.curp
                                });

                viewEmpresas.DataSource = empresas;
                viewEmpresas.DataBind();
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

        protected void LlenarEmisores()
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

        //protected void LlenarNacionalidad(){
        //    ddlNacionalidad.Items.Add(new ListItem("Nacional","Nacional"));
        //    ddlNacionalidad.Items.Add(new ListItem("Extranjero", "Extranjero"));
        //}

        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (this.idEmisor.Value.Split('|')[0] != "0")
                this.LimpiarCampos();
            this.idEmisor.Value = 0 + "|" + 1;

           // titleEmisor.Text = "Registrar nuevo emisor";
            btnEmisor.CssClass = "btn btn-primary";
            btnEmisor.Text = "Registrar";

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#registro').modal('show');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "RegistroModalScript", sb.ToString(), false);
        }

        //protected void ddlNacionalidad_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (ddlNacionalidad.SelectedValue.Equals("Nacional")) {
        //        divClienteNacional.Visible = true;
        //        divClienteExtranjero.Visible = false;
        //        revCURP.Enabled = true;
        //    }else{
        //        divClienteNacional.Visible = false;
        //        divClienteExtranjero.Visible = true;
        //        revCURP.Enabled = false;
        //    }
        //}

        protected void LimpiarCampos()
        {
            this.idEmisor.Value = string.Empty;
            this.Identificador.Text = string.Empty;
            this.RFC.Text = string.Empty;
            this.RazonSocial.Text = string.Empty;
            this.CURP.Text = string.Empty;
            this.txbNumRegIdentFiscalCliente.Text = string.Empty;
            this.Calle.Text = string.Empty;
            this.NoExterior.Text = string.Empty;
            this.NoInterior.Text = string.Empty;
            this.Colonia.Text = string.Empty;
            this.Referencia.Text = string.Empty;
            this.Localidad.Text = string.Empty;
            this.Municipio.Text = string.Empty;
            this.Estado.Text = string.Empty;
            this.Pais.Text = string.Empty;
            this.CodigoPostal.Text = string.Empty;
        }

        protected void viewEmpresas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Editar")
                {
                    int id = Convert.ToInt32(viewEmpresas.DataKeys[Convert.ToInt32(e.CommandArgument)].Value);
                    LlenarEmisor(id, 2);
                }
                else if (e.CommandName == "Eliminar")
                {
                    int id = Convert.ToInt32(viewEmpresas.DataKeys[Convert.ToInt32(e.CommandArgument)].Value);
                    LlenarEmisor(id, 3);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = ex.Message;
            }
        }
        protected void LlenarEmisor(int id, int idInstruccion)
        {
            string instruccion = string.Empty;
            switch (idInstruccion)
            {
                case 2://Editar cliente
                    instruccion = "Editar cliente: ";
                    btnEmisor.CssClass = "btn btn-primary";
                    btnEmisor.Text = "Editar";
                    break;
                case 3://Eliminar cliente
                    instruccion = "Eliminar cliente: ";
                    btnEmisor.CssClass = "btn btn-danger";
                    btnEmisor.Text = "Eliminar";
                    break;
            }
            using (var db = new DataModel.OstarDB())
            {
                var query =
                    (from r in db.receptor
                     join d in db.domicilio on r.domicilio equals d.iddomicilio
                     where r.idreceptor == id
                     select new
                     {
                         r.identificador,
                         r.razon_social,
                         r.rfc,
                         r.nacionalidad,
                         r.curp,
                         r.numregidtrib,
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
                   // titleEmisor.Text = instruccion + (string.IsNullOrEmpty(query.identificador) ? "" : query.identificador);
                    this.idEmisor.Value = id + "|" + idInstruccion;
                    this.Identificador.Text = string.IsNullOrEmpty(query.identificador) ? "" : query.identificador;
                    this.RazonSocial.Text = string.IsNullOrEmpty(query.razon_social) ? "" : seg.Desencriptar(query.razon_social);
                    this.RFC.Text = string.IsNullOrEmpty(query.rfc) ? "" : seg.Desencriptar(query.rfc);
                    //this.ddlNacionalidad.SelectedValue = query.nacionalidad;
                    this.CURP.Text = string.IsNullOrEmpty(query.curp) ? "" : query.curp;
                    this.txbNumRegIdentFiscalCliente.Text = string.IsNullOrEmpty(query.numregidtrib) ? "" : query.numregidtrib;
                    this.Calle.Text = string.IsNullOrEmpty(query.calle) ? "" : seg.Desencriptar(query.calle);
                    this.NoExterior.Text = string.IsNullOrEmpty(query.no_exterior) ? "" : seg.Desencriptar(query.no_exterior);
                    this.NoInterior.Text = string.IsNullOrEmpty(query.no_interior) ? "" : seg.Desencriptar(query.no_interior);
                    this.Colonia.Text = string.IsNullOrEmpty(query.colonia) ? "" : seg.Desencriptar(query.colonia);
                    this.Referencia.Text = string.IsNullOrEmpty(query.referencia) ? "" : seg.Desencriptar(query.referencia);
                    this.Localidad.Text = string.IsNullOrEmpty(query.localidad) ? "" : seg.Desencriptar(query.localidad);
                    this.Municipio.Text = string.IsNullOrEmpty(query.municipio) ? "" : seg.Desencriptar(query.municipio);
                    this.Estado.Text = string.IsNullOrEmpty(query.estado) ? "" : seg.Desencriptar(query.estado);
                    this.Pais.Text = string.IsNullOrEmpty(query.pais) ? "" : seg.Desencriptar(query.pais);
                    this.CodigoPostal.Text = string.IsNullOrEmpty(query.codigo_postal) ? "" : seg.Desencriptar(query.codigo_postal);
                }

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#registro').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "RegistroModalScript", sb.ToString(), false);
            }
        }

        protected void btnEmisor_Click(object sender, EventArgs e)
        {
            try
            {
                string[] instrucciones = this.idEmisor.Value.Split('|');
                int id = Convert.ToInt32(instrucciones[0]);
                int idInstruccion = Convert.ToInt32(instrucciones[1]);
                switch (idInstruccion)
                {
                    case 1://Registrar usuario
                        this.RegistrarEmpresa();
                        break;
                    case 2://Editar usuario
                        this.EditarEmpresa(id);
                        break;
                    case 3://Eliminar usuario
                        this.EliminarEmpresa(id);
                        break;
                }
                this.LimpiarCampos();
                this.LlenarEmpresas(Convert.ToInt32(Empresa.SelectedValue));
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#registro').modal('hide');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "RegistroModalScript", sb.ToString(), false);
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = ex.Message;
            }
        }
        private void RegistrarEmpresa()
        {
            using (var db = new DataModel.OstarDB())
            {
                db.BeginTransaction();

                var idDomicilio = db.domicilio.InsertWithIdentity(() => new DataModel.domicilio
                {
                    calle = string.IsNullOrEmpty(this.Calle.Text) ? "" : seg.Encriptar(this.Calle.Text),
                    colonia = string.IsNullOrEmpty(this.Colonia.Text) ? "" : seg.Encriptar(this.Colonia.Text),
                    codigo_postal = string.IsNullOrEmpty(this.CodigoPostal.Text) ? "" : seg.Encriptar(this.CodigoPostal.Text),
                    estado = string.IsNullOrEmpty(this.Estado.Text) ? "" : seg.Encriptar(this.Estado.Text),
                    localidad = string.IsNullOrEmpty(this.Localidad.Text) ? "" : seg.Encriptar(this.Localidad.Text),
                    municipio = string.IsNullOrEmpty(this.Municipio.Text) ? "" : seg.Encriptar(this.Municipio.Text),
                    no_exterior = string.IsNullOrEmpty(this.NoExterior.Text) ? "" : seg.Encriptar(this.NoExterior.Text),
                    no_interior = string.IsNullOrEmpty(this.NoInterior.Text) ? "" : seg.Encriptar(this.NoInterior.Text),
                    pais = string.IsNullOrEmpty(this.Pais.Text) ? "" : seg.Encriptar(this.Pais.Text),
                    referencia = string.IsNullOrEmpty(this.Referencia.Text) ? "" : seg.Encriptar(this.Referencia.Text)
                });

                var value = db.receptor.InsertWithIdentity(() => new DataModel.receptor
                {
                    identificador = this.Identificador.Text,
                    rfc = seg.Encriptar(this.RFC.Text),
                    //nacionalidad = this.ddlNacionalidad.SelectedValue,
                    //curp = this.ddlNacionalidad.SelectedValue.Equals("Nacional") ? this.CURP.Text : "",
                    //numregidtrib = this.ddlNacionalidad.SelectedValue.Equals("Extranjero") ? this.txbNumRegIdentFiscalCliente.Text : "", 
                    razon_social = seg.Encriptar(this.RazonSocial.Text),
                    domicilio = Convert.ToInt32(idDomicilio),
                    emisor = Convert.ToInt32(Empresa.SelectedValue)
                });

                db.CommitTransaction();
            }
        }

        private void EditarEmpresa(int id)
        {
            using (var db = new DataModel.OstarDB())
            {
                db.BeginTransaction();

                var query = (from r in db.receptor
                             where r.idreceptor == id
                             select r).FirstOrDefault();

                var idDomicilio = db.domicilio
                    .Where(e => e.iddomicilio == query.domicilio)
                    .Update(e => new DataModel.domicilio
                    {
                        calle = string.IsNullOrEmpty(this.Calle.Text) ? "" : seg.Encriptar(this.Calle.Text),
                        no_exterior = string.IsNullOrEmpty(this.NoExterior.Text) ? "" : seg.Encriptar(this.NoExterior.Text),
                        no_interior = string.IsNullOrEmpty(this.NoInterior.Text) ? "" : seg.Encriptar(this.NoInterior.Text),
                        colonia = string.IsNullOrEmpty(this.Colonia.Text) ? "" : seg.Encriptar(this.Colonia.Text),
                        referencia = string.IsNullOrEmpty(this.Referencia.Text) ? "" : seg.Encriptar(this.Referencia.Text),
                        localidad = string.IsNullOrEmpty(this.Localidad.Text) ? "" : seg.Encriptar(this.Localidad.Text),
                        municipio = string.IsNullOrEmpty(this.Municipio.Text) ? "" : seg.Encriptar(this.Municipio.Text),
                        estado = string.IsNullOrEmpty(this.Estado.Text) ? "" : seg.Encriptar(this.Estado.Text),
                        pais = string.IsNullOrEmpty(this.Pais.Text) ? "" : seg.Encriptar(this.Pais.Text),
                        codigo_postal = string.IsNullOrEmpty(this.CodigoPostal.Text) ? "" : seg.Encriptar(this.CodigoPostal.Text)
                    });

                var idReceptor = db.receptor
                    .Where(e => e.idreceptor == id)
                    .Update(e => new DataModel.receptor
                    {
                        identificador = this.Identificador.Text,
                        razon_social = seg.Encriptar(this.RazonSocial.Text),
                        rfc = seg.Encriptar(this.RFC.Text),
                        //nacionalidad = this.ddlNacionalidad.SelectedValue,
                        //curp = this.ddlNacionalidad.SelectedValue.Equals("Nacional") ? this.CURP.Text : "",
                        //numregidtrib = this.ddlNacionalidad.SelectedValue.Equals("Extranjero") ? this.txbNumRegIdentFiscalCliente.Text : "",
                        ////emisor = Convert.ToInt32(Empresa.SelectedValue)
                    });

                db.CommitTransaction();
            }
        }

        protected void EliminarEmpresa(int id)
        {
            using (var db = new DataModel.OstarDB())
            {
                db.BeginTransaction();

                var query = (from e in db.receptor
                             where e.idreceptor == id
                             select e).FirstOrDefault();

                var idReceptor = db.receptor
                    .Delete(e => e.idreceptor == id);

                var idDomicilio = db.domicilio
                    .Delete(e => e.iddomicilio == query.domicilio);

                db.CommitTransaction();
            }
        }

        protected void Empresa_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.LlenarEmpresas(Convert.ToInt32(Empresa.SelectedValue));
        }

        protected void Importar_Click(object sender, EventArgs e)
        {
            try
            {
                if (file1 == null)
                    throw new Exception("No has seleccionado un archivo valido");
                else if (file1.PostedFile == null)
                    throw new Exception("No has seleccionado un archivo valido");
                string rutaIncumplido = Server.MapPath("~/Facturacion/Excel/");
                if (!Directory.Exists(rutaIncumplido))
                {
                    Directory.CreateDirectory(rutaIncumplido);
                }
                file1.PostedFile.SaveAs(rutaIncumplido + file1.PostedFile.FileName);
                var data = (new ExcelReader()).ReadExcel(rutaIncumplido + file1.PostedFile.FileName, "Clientes");


                int rfc = -1, razon = -1, calle = -1, exterior = -1, interior = -1, colonia = -1,
                    localidad = -1, referencia = -1, municipio = -1, estado = -1, pais = -1, cp = -1;

                for (int x = 0; x < data.Headers.Count; x++)
                {
                    if (data.Headers[x].Contains("rfc")) rfc = x;
                    else if (data.Headers[x].Contains("razon")) razon = x;
                    else if (data.Headers[x].Contains("calle")) calle = x;
                    else if (data.Headers[x].Contains("exterior")) exterior = x;
                    else if (data.Headers[x].Contains("interior")) interior = x;
                    else if (data.Headers[x].Contains("colonia")) colonia = x;
                    else if (data.Headers[x].Contains("referencia")) referencia = x;
                    else if (data.Headers[x].Contains("localidad")) localidad = x;
                    else if (data.Headers[x].Contains("municipio")) municipio = x;
                    else if (data.Headers[x].Contains("estado")) estado = x;
                    else if (data.Headers[x].Contains("pais")) pais = x;
                    else if (data.Headers[x].Contains("cp")) cp = x;
                }

                if(rfc!=-1)
                {
                using (var db = new DataModel.OstarDB())
                {
                    foreach (List<string> row in data.DataRows)
                    {
                        try
                        {
                            if (!string.IsNullOrWhiteSpace(row[rfc]))
                            {
                                db.BeginTransaction();

                                DataModel.receptor rec = new DataModel.receptor();
                                rec.emisor = Convert.ToInt32(Empresa.SelectedValue);
                                rec.identificador = row[rfc].Trim();
                                rec.rfc = string.IsNullOrWhiteSpace(row[rfc]) ? "" : seg.Encriptar(row[rfc].Trim());
                                rec.razon_social = razon != -1 ? (string.IsNullOrWhiteSpace(row[razon]) ? "" : seg.Encriptar(row[razon].Trim())) : "";                                

                                var exist = db.receptor.Where(x => x.identificador == rec.identificador && x.emisor == rec.emisor).FirstOrDefault();
                                if (exist == null)
                                {
                                    DataModel.domicilio dom = new DataModel.domicilio();
                                    dom.calle = calle != -1 ? (string.IsNullOrWhiteSpace(row[calle]) ? null : seg.Encriptar(row[calle].Trim())) : null;
                                    dom.codigo_postal = cp != -1 ? (string.IsNullOrWhiteSpace(row[cp]) ? null : seg.Encriptar(row[cp].Trim())) : null;
                                    dom.colonia = colonia != -1 ? (string.IsNullOrWhiteSpace(row[colonia]) ? null : seg.Encriptar(row[colonia].Trim())) : null;
                                    dom.estado = estado != -1 ? (string.IsNullOrWhiteSpace(row[estado]) ? null : seg.Encriptar(row[estado].Trim())) : null;
                                    dom.localidad = localidad != -1 ? (string.IsNullOrWhiteSpace(row[localidad]) ? null : seg.Encriptar(row[localidad].Trim())) : null;
                                    dom.municipio = municipio != -1 ? (string.IsNullOrWhiteSpace(row[municipio]) ? null : seg.Encriptar(row[municipio].Trim())) : null;
                                    dom.no_exterior = exterior != -1 ? (string.IsNullOrWhiteSpace(row[exterior]) ? null : seg.Encriptar(row[exterior].Trim())) : null;
                                    dom.no_interior = interior != -1 ? (string.IsNullOrWhiteSpace(row[interior]) ? null : seg.Encriptar(row[interior].Trim())) : null;
                                    dom.pais = pais != -1 ? (string.IsNullOrWhiteSpace(row[pais]) ? "México" : seg.Encriptar(row[pais])) : "México";
                                    dom.referencia = referencia != -1 ? (string.IsNullOrWhiteSpace(row[referencia]) ? null : seg.Encriptar(row[referencia].Trim())) : null;

                                    var idDom = db.InsertWithIdentity(dom);
                                    rec.domicilio = Convert.ToInt32(idDom);
                                    db.Insert(rec);
                                }
                                db.CommitTransaction();
                            }
                        }
                        catch { db.RollbackTransaction(); }
                    }
                }
                this.LlenarEmpresas(Convert.ToInt32(Empresa.SelectedValue));
                ErrorMessage.Text = string.Empty;
            }
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = ex.Message;
            }
        }
    }
}