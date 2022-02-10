using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLToolkit.Data;
using BLToolkit.Data.Linq;
using System.IO;
//using Facturador.GHO.Servicio;
using Daysoft.DICI.Facturador.DFacture;
using Facturador.GHO.Controllers;
using Microsoft.AspNet.Identity;

namespace Facturador.GHO.Admin
{
    public partial class Empresa : System.Web.UI.Page
    {
        Seguridad seg;
        private License lic;
        string rutaLicencia = string.Empty;
        private string rutaCertificado = @"/Facturacion/Empresas/Certificados/";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                seg = new Seguridad();
                rutaCertificado = Server.MapPath("~" + System.Configuration.ConfigurationManager.AppSettings["rutaCertificado"]);
                LeerLicencia();
                if (!IsPostBack)
                {
                    LlenarEmpresas();
                    LlenarTiposDocumento();
                    LlenarRegimenes();
                    LlenarClaveProdServ();
                }
                ErrorMessage.Text = string.Empty;
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
                List<int> emisoresActuales = new List<int>();
                if (User.IsInRole("Admin") || User.IsInRole("Super"))
                {
                    emisoresActuales = (from em in db.emisor
                                        select em.idemisor).ToList();
                }
                else
                {
                    btnRegistrar.Visible = false;
                    //btnRegistro.Visible = false;
                    emisoresActuales = (from em in db.emisor
                                        join ue in db.useremisor on em.idemisor equals ue.EmisorId
                                        where ue.UserId == User.Identity.GetUserId()
                                        select em.idemisor).ToList();
                }
                var empresas = (from h in db.emisor
                                where emisoresActuales.Contains(h.idemisor)
                                select new
                                {
                                    h.idemisor,
                                    razon_social = string.IsNullOrEmpty(h.razon_social) ? "" : seg.Desencriptar(h.razon_social),
                                    rfc = string.IsNullOrEmpty(h.rfc) ? "" : seg.Desencriptar(h.rfc),
                                    curp = string.IsNullOrEmpty(h.curp)? "" : h.curp,
                                    identificador = h.identificador// string.IsNullOrEmpty(h.identificador) ? "" : seg.Desencriptar(h.identificador)
                                }).Take(lic.ObtenerEmpresas());

                viewEmpresas.DataSource = empresas;
                viewEmpresas.DataBind();
            }
        }

        private void LlenarTiposDocumento()
        {
            using (var db = new DataModel.OstarDB())
            {
                var tiposDocumento = db.tipo;

                TipoDocumento.DataValueField = "id";
                TipoDocumento.DataTextField = "tipo_comprobante";
                TipoDocumento.DataSource = tiposDocumento;
                TipoDocumento.DataBind();
            }
        }

        private void LlenarClaveProdServ()
        {
            using (var db = new DataModel.OstarDB())
            {
                var claveProdServ = db.cClaveProdServ;

                CclaveProdServ.DataValueField = "cClaveProdServ_id";
                CclaveProdServ.DataTextField = "descripcion";
                CclaveProdServ.DataSource = claveProdServ;
                CclaveProdServ.DataBind();
            }
        }

        private void LlenarRegimenes()
        {
            using (var db = new DataModel.OstarDB())
            {
                var regimenes = db.cRegimenFiscal;

                cRegimen.DataValueField = "cRegimenFiscal_id";
                cRegimen.DataTextField = "descripcion";
                cRegimen.DataSource = regimenes;
                cRegimen.DataBind();
            }
        }

        protected void Registrar(object sender, EventArgs e)
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
                this.LlenarEmpresas();
                this.LlenarRegimenes();
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#registro').modal('hide');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "RegistroModalScript", sb.ToString(), false);
            }
            catch(Exception ex)
            {
                ErrorMessage.Text = ex.Message;
            }
        }

        private void RegistrarEmpresa()
        {
            //if (lic.IsValidLicenseAvailable())
            //{
                using (var db = new DataModel.OstarDB())
                {
                    var numEmp = db.emisor.Count();
                    if (numEmp < lic.ObtenerEmpresas())
                    {
                        db.BeginTransaction();

                        string llave = LlavePrivada.FileName;
                        string certificado = Certificado.FileName;
                        string logo = string.Empty;
                        string cedula = string.Empty;

                        if (FileLogo.HasFile)
                        {
                            logo = FileLogo.FileName;
                        }

                        if (FileCedula.HasFile)
                        {
                            cedula = FileCedula.FileName;
                        }

                        Byte[] cert = new byte[Certificado.PostedFile.ContentLength];
                        Certificado.PostedFile.InputStream.Read(cert, 0, cert.Length);
                        string cert64 = Convert.ToBase64String(cert);
                        Byte[] llavePriv = new byte[LlavePrivada.PostedFile.ContentLength];
                        LlavePrivada.PostedFile.InputStream.Read(cert, 0, cert.Length);
                        string llavePriv64 = Convert.ToBase64String(cert);

                        var idCertificado = db.certificado.InsertWithIdentity(() => new DataModel.certificado
                        {
                            contrasenia = seg.Encriptar(this.Contrasenia.Text),
                            llave_privada = llave,
                            csd = certificado
                        });

                        var value = db.emisor.InsertWithIdentity(() => new DataModel.emisor
                        {
                            identificador = this.Identificador.Text, 
                            rfc = seg.Encriptar(this.RFC.Text),
                            razon_social = seg.Encriptar(this.RazonSocial.Text),
                            curp = this.CURP.Text,
                            cRegimenFiscal_id = Guid.Parse(cRegimen.SelectedValue),
                            logotipo = logo,
                            cedula = cedula,
                            
                            cetificado = Convert.ToInt32(idCertificado),
                        });

                        if (!Directory.Exists(rutaCertificado + value))
                        {
                            Directory.CreateDirectory(rutaCertificado + value);
                        }
                        LlavePrivada.SaveAs(rutaCertificado + value + "/" + llave);
                        Certificado.SaveAs(rutaCertificado + value + "/" + certificado);

                        if (FileLogo.HasFile)
                        {
                            FileLogo.SaveAs(rutaCertificado + value + "/" + logo);
                        }
                        if (FileCedula.HasFile)
                        {
                            FileCedula.SaveAs(rutaCertificado + value + "/" + cedula);
                        }

                        db.CommitTransaction();
                    }
                    else
                    {
                        throw new Exception("Tu licencia solo te permite registrar " + lic.ObtenerEmpresas() + " empresas");
                    }
                }
            //}
            //else
            //    throw new Exception("Necesitas registar la licencia para poder registrar empresas");
        }

        private void EditarEmpresa(int id)
        {
            using (var db = new DataModel.OstarDB())
            {
                db.BeginTransaction();

                var query =
                    (from e in db.emisor
                     where e.idemisor == id
                     select e).FirstOrDefault();

                //var idDomicilio = db.domicilio
                //    .Where(e => e.iddomicilio == query.domicilio)
                //    .Update(e => new DataModel.domicilio
                //    {
                //        calle = string.IsNullOrEmpty(this.Calle.Text) ? "" : seg.Encriptar(this.Calle.Text),
                //        no_exterior = string.IsNullOrEmpty(this.NoExterior.Text) ? "" : seg.Encriptar(this.NoExterior.Text),
                //        no_interior = string.IsNullOrEmpty(this.NoInterior.Text) ? "" : seg.Encriptar(this.NoInterior.Text),
                //        colonia = string.IsNullOrEmpty(this.Colonia.Text) ? "" : seg.Encriptar(this.Colonia.Text),
                //        referencia = string.IsNullOrEmpty(this.Referencia.Text) ? "" : seg.Encriptar(this.Referencia.Text),
                //        localidad = string.IsNullOrEmpty(this.Localidad.Text) ? "" : seg.Encriptar(this.Localidad.Text),
                //        municipio = string.IsNullOrEmpty(this.Municipio.Text) ? "" : seg.Encriptar(this.Municipio.Text),
                //        estado = string.IsNullOrEmpty(this.Estado.Text) ? "" : seg.Encriptar(this.Estado.Text),
                //        pais = string.IsNullOrEmpty(this.Pais.Text) ? "" : seg.Encriptar(this.Pais.Text),
                //        codigo_postal = string.IsNullOrEmpty(this.CodigoPostal.Text) ? "" : seg.Encriptar(this.CodigoPostal.Text)
                //    });

                //int? idSucursal = null;

                //if (chkSucursal.Checked)
                //{
                //    if (query.sucursal != null)
                //    {
                //        var idSucursal2 = db.domicilio
                //            .Where(e => e.iddomicilio == query.sucursal)
                //            .Update(e => new DataModel.domicilio
                //        {
                //            calle = string.IsNullOrEmpty(this.SucursalCalle.Text) ? "" : seg.Encriptar(this.SucursalCalle.Text),
                //            colonia = string.IsNullOrEmpty(this.SucursalColonia.Text) ? "" : seg.Encriptar(this.SucursalColonia.Text),
                //            codigo_postal = string.IsNullOrEmpty(this.SucursalCodigoPostal.Text) ? "" : seg.Encriptar(this.SucursalCodigoPostal.Text),
                //            estado = string.IsNullOrEmpty(this.SucursalEstado.Text) ? "" : seg.Encriptar(this.SucursalEstado.Text),
                //            localidad = string.IsNullOrEmpty(this.SucursalLocalidad.Text) ? "" : seg.Encriptar(this.SucursalLocalidad.Text),
                //            municipio = string.IsNullOrEmpty(this.SucursalMunicipio.Text) ? "" : seg.Encriptar(this.SucursalMunicipio.Text),
                //            no_exterior = string.IsNullOrEmpty(this.SucursalNoExterior.Text) ? "" : seg.Encriptar(this.SucursalNoExterior.Text),
                //            no_interior = string.IsNullOrEmpty(this.SucursalNoInterior.Text) ? "" : seg.Encriptar(this.SucursalNoInterior.Text),
                //            pais = string.IsNullOrEmpty(this.SucursalPais.Text) ? "" : seg.Encriptar(this.SucursalPais.Text),
                //            referencia = string.IsNullOrEmpty(this.SucursalReferencia.Text) ? "" : seg.Encriptar(this.SucursalReferencia.Text)
                //        });
                //        idSucursal = query.sucursal;
                //    }
                //    else
                //    {
                //        var idSucursal2 = db.domicilio.InsertWithIdentity(() => new DataModel.domicilio
                //        {
                //            calle = string.IsNullOrEmpty(this.SucursalCalle.Text) ? "" : seg.Encriptar(this.SucursalCalle.Text),
                //            colonia = string.IsNullOrEmpty(this.SucursalColonia.Text) ? "" : seg.Encriptar(this.SucursalColonia.Text),
                //            codigo_postal = string.IsNullOrEmpty(this.SucursalCodigoPostal.Text) ? "" : seg.Encriptar(this.SucursalCodigoPostal.Text),
                //            estado = string.IsNullOrEmpty(this.SucursalEstado.Text) ? "" : seg.Encriptar(this.SucursalEstado.Text),
                //            localidad = string.IsNullOrEmpty(this.SucursalLocalidad.Text) ? "" : seg.Encriptar(this.SucursalLocalidad.Text),
                //            municipio = string.IsNullOrEmpty(this.SucursalMunicipio.Text) ? "" : seg.Encriptar(this.SucursalMunicipio.Text),
                //            no_exterior = string.IsNullOrEmpty(this.SucursalNoExterior.Text) ? "" : seg.Encriptar(this.SucursalNoExterior.Text),
                //            no_interior = string.IsNullOrEmpty(this.SucursalNoInterior.Text) ? "" : seg.Encriptar(this.SucursalNoInterior.Text),
                //            pais = string.IsNullOrEmpty(this.SucursalPais.Text) ? "" : seg.Encriptar(this.SucursalPais.Text),
                //            referencia = string.IsNullOrEmpty(this.SucursalReferencia.Text) ? "" : seg.Encriptar(this.SucursalReferencia.Text)
                //        });
                //        idSucursal = Convert.ToInt32(idSucursal2);
                //    }
                //}
                var asdf = cRegimen.SelectedValue;
                var idEmisor = db.emisor
                    .Where(e => e.idemisor == id)
                    .Update(e => new DataModel.emisor
                    {
                        identificador = this.Identificador.Text, // seg.Encriptar(this.Identificador.Text),
                        razon_social = seg.Encriptar(this.RazonSocial.Text),
                        curp = this.CURP.Text,
                        rfc = seg.Encriptar(this.RFC.Text),
                        cRegimenFiscal_id = Guid.Parse(cRegimen.SelectedValue),
                        lugar_expedicion = CodigoPostal.Text,
                        //regimen_fiscal = seg.Encriptar(this.RegimenFiscal.Text),
                        //sucursal = idSucursal
                    });

                if (!string.IsNullOrEmpty(this.Contrasenia.Text))
                {
                    var idCertificado = db.certificado
                        .Where(e => e.idcertificado == query.cetificado)
                        .Update(e => new DataModel.certificado
                        {
                            contrasenia = seg.Encriptar(this.Contrasenia.Text)
                        });
                }

                if (!Directory.Exists(rutaCertificado + id))
                {
                    Directory.CreateDirectory(rutaCertificado + id);
                }

                if (LlavePrivada.HasFile)
                {
                    var llave = db.certificado
                        .Where(e => e.idcertificado == query.cetificado)
                        .Update(e => new DataModel.certificado
                        {
                            llave_privada = this.LlavePrivada.FileName
                        });
                    LlavePrivada.SaveAs(rutaCertificado + id + "/" + this.LlavePrivada.FileName);
                }
                if (Certificado.HasFile)
                {
                    var cert = db.certificado
                        .Where(e => e.idcertificado == query.cetificado)
                        .Update(e => new DataModel.certificado
                        {
                            csd = this.Certificado.FileName
                        });
                    Certificado.SaveAs(rutaCertificado + id + "/" + this.Certificado.FileName);
                }
                if (FileLogo.HasFile)
                {
                    var logo = db.emisor
                       .Where(e => e.idemisor == id)
                       .Update(e => new DataModel.emisor
                       {
                           logotipo = FileLogo.FileName
                       });
                    FileLogo.SaveAs(rutaCertificado + id + "/" + this.FileLogo.FileName);
                }
                if (FileCedula.HasFile)
                {
                    var cedula = db.emisor
                       .Where(e => e.idemisor == id)
                       .Update(e => new DataModel.emisor
                       {
                           cedula = FileCedula.FileName
                       });
                    FileCedula.SaveAs(rutaCertificado + id + "/" + this.FileCedula.FileName);
                }
                db.CommitTransaction();
            }
        }

        protected void EliminarEmpresa(int id)
        {
            using (var db = new DataModel.OstarDB())
            {
                db.BeginTransaction();

                var query =
                    (from e in db.emisor
                     where e.idemisor == id
                     select e).FirstOrDefault();

                var idEmisor = db.emisor
                    .Delete(e => e.idemisor == id);

                //var idDomicilio = db.domicilio
                //    .Delete(e => e.iddomicilio == query.domicilio);

                var idCertificado = db.certificado
                    .Delete(e => e.idcertificado == query.cetificado);

                db.CommitTransaction();
            }
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
                else if (e.CommandName == "Folio")
                {
                    int id = Convert.ToInt32(viewEmpresas.DataKeys[Convert.ToInt32(e.CommandArgument)].Value);
                    LlenarFolios(id);
                }
                else if (e.CommandName == "Pdf")
                {
                    int id = Convert.ToInt32(viewEmpresas.DataKeys[Convert.ToInt32(e.CommandArgument)].Value);
                    LlenarPdf(id);
                }
                else if (e.CommandName == "cProdServ")
                {
                    int id = Convert.ToInt32(viewEmpresas.DataKeys[Convert.ToInt32(e.CommandArgument)].Value);
                    LlenarClaveProdServ(id);
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
                case 2://Editar usuario
                    instruccion = "Editar emisor: ";
                    btnEmisor.CssClass = "btn btn-primary";
                    btnEmisor.Text = "Editar";
                    FileLogo.Enabled = true;
                    FileCedula.Enabled = true;
                    LlavePrivada.Enabled = true;
                    Certificado.Enabled = true;
                    break;
                case 3://Eliminar usuario
                    instruccion = "Eliminar emisor: ";
                    btnEmisor.CssClass = "btn btn-danger";
                    btnEmisor.Text = "Eliminar";
                    FileLogo.Enabled = false;
                    FileCedula.Enabled = false;
                    LlavePrivada.Enabled = false;
                    Certificado.Enabled = false;
                    break;
            }
            using (var db = new DataModel.OstarDB())
            {
                int? idSuc = null;
                var query =
                    (from e in db.emisor
                     //join d in db.domicilio on e.domicilio equals d.iddomicilio
                     join c in db.certificado on e.cetificado equals c.idcertificado
                     where e.idemisor == id
                     select new
                     {
                         e.identificador,
                         e.razon_social,
                         e.curp,
                         e.rfc,
                         //e.regimen_fiscal,
                         //d.calle,
                         //d.no_exterior,
                         //d.no_interior,
                         //d.colonia,
                         //d.localidad,
                         //d.referencia,
                         //d.municipio,
                         //d.estado,
                         //d.pais,
                         //d.codigo_postal,
                         c.llave_privada,
                         c.contrasenia,
                         c.csd,
                         e.sucursal,
                         e.lugar_expedicion
                     }
                     ).FirstOrDefault();
                if (query != null)
                {
                    this.rfvLLavePrivada.Enabled = false;
                    this.rfvCertificado.Enabled = false;

                    titleEmisor.Text = instruccion + (string.IsNullOrEmpty(query.identificador) ? "" : query.identificador); //seg.Desencriptar(query.identificador));
                    this.idEmisor.Value = id + "|" + idInstruccion;
                    this.Identificador.Text = query.identificador; // string.IsNullOrEmpty(query.identificador) ? "" : seg.Desencriptar(query.identificador);
                    this.RazonSocial.Text = string.IsNullOrEmpty(query.razon_social) ? "" : seg.Desencriptar(query.razon_social);
                    this.CURP.Text = string.IsNullOrEmpty(query.curp) ? "" : query.curp;
                    //this.RegimenFiscal.Text = string.IsNullOrEmpty(query.regimen_fiscal) ? "" : seg.Desencriptar(query.regimen_fiscal);
                    this.RFC.Text = string.IsNullOrEmpty(query.rfc) ? "" : seg.Desencriptar(query.rfc);
                    //this.Calle.Text = string.IsNullOrEmpty(query.calle) ? "" : seg.Desencriptar(query.calle);
                    //this.NoExterior.Text = string.IsNullOrEmpty(query.no_exterior) ? "" : seg.Desencriptar(query.no_exterior);
                    //this.NoInterior.Text = string.IsNullOrEmpty(query.no_interior) ? "" : seg.Desencriptar(query.no_interior);
                    //this.Colonia.Text = string.IsNullOrEmpty(query.colonia) ? "" : seg.Desencriptar(query.colonia);
                    //this.Referencia.Text = string.IsNullOrEmpty(query.referencia) ? "" : seg.Desencriptar(query.referencia);
                    //this.Localidad.Text = string.IsNullOrEmpty(query.localidad) ? "" : seg.Desencriptar(query.localidad);
                    //this.Municipio.Text = string.IsNullOrEmpty(query.municipio) ? "" : seg.Desencriptar(query.municipio);
                    //this.Estado.Text = string.IsNullOrEmpty(query.estado) ? "" : seg.Desencriptar(query.estado);
                    //this.Pais.Text = string.IsNullOrEmpty(query.pais) ? "" : seg.Desencriptar(query.pais);
                    //this.CodigoPostal.Text = string.IsNullOrEmpty(query.codigo_postal) ? "" : seg.Desencriptar(query.codigo_postal);
                    this.CodigoPostal.Text = query.lugar_expedicion;
                    this.Contrasenia.Text = string.IsNullOrEmpty(query.contrasenia) ? "" : seg.Desencriptar(query.contrasenia);
                    this.Contrasenia.Attributes["value"] = string.IsNullOrEmpty(query.contrasenia) ? "" : seg.Desencriptar(query.contrasenia);

                    idSuc = query.sucursal;
                }
                //if (idSuc != null)
                //{
                //    chkSucursal.Checked = true;
                //    this.MostrarSucursal(true);
                //    var suc = db.domicilio.Where(x => x.iddomicilio == idSuc.Value).FirstOrDefault();
                //    this.SucursalCalle.Text = string.IsNullOrEmpty(suc.calle) ? "" : seg.Desencriptar(suc.calle);
                //    this.SucursalNoExterior.Text = string.IsNullOrEmpty(suc.no_exterior) ? "" : seg.Desencriptar(suc.no_exterior);
                //    this.SucursalNoInterior.Text = string.IsNullOrEmpty(suc.no_interior) ? "" : seg.Desencriptar(suc.no_interior);
                //    this.SucursalColonia.Text = string.IsNullOrEmpty(suc.colonia) ? "" : seg.Desencriptar(suc.colonia);
                //    this.SucursalReferencia.Text = string.IsNullOrEmpty(suc.referencia) ? "" : seg.Desencriptar(suc.referencia);
                //    this.SucursalLocalidad.Text = string.IsNullOrEmpty(suc.localidad) ? "" : seg.Desencriptar(suc.localidad);
                //    this.SucursalMunicipio.Text = string.IsNullOrEmpty(suc.municipio) ? "" : seg.Desencriptar(suc.municipio);
                //    this.SucursalEstado.Text = string.IsNullOrEmpty(suc.estado) ? "" : seg.Desencriptar(suc.estado);
                //    this.SucursalPais.Text = string.IsNullOrEmpty(suc.pais) ? "" : seg.Desencriptar(suc.pais);
                //    this.SucursalCodigoPostal.Text = string.IsNullOrEmpty(suc.codigo_postal) ? "" : seg.Desencriptar(suc.codigo_postal);
                //}
                //else
                //{
                //    chkSucursal.Checked = false;
                //    this.MostrarSucursal(false);
                //}

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#registro').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "RegistroModalScript", sb.ToString(), false);
            }
        }

        protected void LlenarFolios(int id)
        {
            this.idFolioEmpresa.Value = id.ToString();
            this.LimpiarFolios();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#folios').modal('show');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "FoliosModalScript", sb.ToString(), false);
        }


        protected void LlenarClaveProdServ(int id)
        {
            this.idClaveProdServEmpresa.Value = id.ToString();
            this.LimpiarcClaveProdServ();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#cProdServ').modal('show');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "cProdServModalScript", sb.ToString(), false);
        }
        protected void LlenarPdf(int id)
        {
            this.idReporteEmpresa.Value = id.ToString();
            this.SeleccionarPdf();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#reporte').modal('show');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "FoliosModalScript", sb.ToString(), false);
        }

        private void LimpiarFolios()
        {
            using (var db = new DataModel.OstarDB())
            {
                this.idFolio.Value = "";
                this.idFolioInstruccion.Value = "1";
                int id = Convert.ToInt32(this.idFolioEmpresa.Value);
                //var folios = db.folio.Where(x => x.emisor == id);
                var folios = (from f in db.folio
                              join t in db.tipo on f.tipo equals t.id
                              where f.emisor == id
                              orderby f.serie ascending
                              select new
                              {
                                  id = f.id,
                                  f.serie,
                                  f.folio_inicio,
                                  f.folio_actual,
                                  t.tipo_comprobante
                              });
                viewFolios.DataSource = folios;
                viewFolios.DataBind();
                Serie.Text = string.Empty;
                FolioInicio.Text = string.Empty;
                FolioActual.Text = string.Empty;

                btnAgregarFolio.Text = "<span class='glyphicon glyphicon-plus'></span> Agregar Folio";
                btnAgregarFolio.CssClass = "btn btn-primary pull-right";
            }
        }

        private void LimpiarcClaveProdServ()
        {
            using (var db = new DataModel.OstarDB())
            {
                this.idFolio.Value = "";
                this.idcProdServInstruccion.Value = "1";
                int id = Convert.ToInt32(this.idClaveProdServEmpresa.Value);
                var claves = (from c in db.emisor_cProdServ
                              where c.emisor_id == id
                              orderby c.cClaveProdServ_id ascending
                              select new
                              {
                                  c.cClaveProdServ_id,
                                  c.fkcProdServ_emisor.codigo,
                                  c.fkcProdServ_emisor.descripcion
                              });
                viewcProdServ.DataSource = claves;
                viewcProdServ.DataBind();

                btnAgregarcProdServ.Text = "<span class='glyphicon glyphicon-plus'></span> Agregar Clave";
                btnAgregarcProdServ.CssClass = "btn btn-primary pull-right";
            }
        }

        private void SeleccionarPdf()
        {
            using (var db = new DataModel.OstarDB())
            {
                int id = Convert.ToInt32(this.idReporteEmpresa.Value);

                var verPdf = db.emisor.Where(x => x.idemisor == id).Select(x => x.tipo_pdf).FirstOrDefault();

                if (verPdf != null)
                    rblReporte.SelectedIndex = verPdf.Value - 1;
                else
                    rblReporte.SelectedIndex = 0;
            }
        }

        protected void btnAgregarcProdServ_Click(object sender, EventArgs e)
        {
            try
            {
                using (var db = new DataModel.OstarDB())
                {
                    int idInstruccion = Convert.ToInt32(idcProdServInstruccion.Value);
                    int emis = Convert.ToInt32(this.idClaveProdServEmpresa.Value);
                    switch (idInstruccion)
                    {
                        case 1://Agregar Folio
                            var existe = db.emisor_cProdServ.Where(x => (x.emisor_id == emis && x.cClaveProdServ_id == Guid.Parse(CclaveProdServ.Text))).FirstOrDefault();
                            if (existe != null)
                                throw new Exception("No puedes repetir la Clave");
                            else
                            {
                                var query = db.emisor_cProdServ.Insert(() => new DataModel.emisor_cProdServ
                                {
                                    emisor_id = emis,
                                    cClaveProdServ_id = Guid.Parse(CclaveProdServ.Text)
                                });
                            }
                            break;
                        case 3://Eliminar folio
                            var deleteClave = db.emisor_cProdServ
                                    .Where(x => x.emisor_id == emis && x.cClaveProdServ_id == Guid.Parse(idClave.Value))
                                    .Delete();
                            break;
                    }
                }
                this.LimpiarcClaveProdServ();
                this.ErrorClave.Text = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorClave.Text = ex.Message;
            }
        }


        protected void btnAgregarFolio_Click(object sender, EventArgs e)
        {
            try
            {
                using (var db = new DataModel.OstarDB())
                {
                    int idInstruccion = Convert.ToInt32(idFolioInstruccion.Value);
                    int emis = Convert.ToInt32(this.idFolioEmpresa.Value);
                    int idFol = 0;
                    switch (idInstruccion)
                    {
                        case 1://Agregar Folio
                            var existe = db.folio.Where(x => (x.emisor == emis && x.serie == Serie.Text)).FirstOrDefault();
                            if (existe != null)
                                throw new Exception("No puedes repetir la serie");
                            else
                            {
                                var query = db.folio.Insert(() => new DataModel.folio
                                {
                                    serie = Serie.Text,
                                    folio_inicio = string.IsNullOrEmpty(FolioInicio.Text) ? 1 : (Convert.ToInt32(FolioInicio.Text) <= 0 ? 1 : Convert.ToInt32(FolioInicio.Text)),
                                    folio_actual = string.IsNullOrEmpty(FolioActual.Text) ? 1 : (Convert.ToInt32(FolioActual.Text) <= 0 ? 1 : Convert.ToInt32(FolioActual.Text)),
                                    tipo = Convert.ToInt32(TipoDocumento.SelectedValue),
                                    emisor = emis
                                });
                            }
                            break;
                        case 2://Editar Folio
                            idFol = Convert.ToInt32(this.idFolio.Value);
                            var existe2 = db.folio.Where(x => (x.emisor == emis && x.serie == Serie.Text && x.id != idFol)).FirstOrDefault();
                            if (existe2 != null)
                                throw new Exception("No puedes repetir la serie");
                            else
                            {
                                var query = db.folio
                                    .Where(x => x.id == idFol)
                                    .Update(x => new DataModel.folio
                                {
                                    serie = Serie.Text,
                                    folio_inicio = string.IsNullOrEmpty(FolioInicio.Text) ? 1 : (Convert.ToInt32(FolioInicio.Text) <= 0 ? 1 : Convert.ToInt32(FolioInicio.Text)),
                                    folio_actual = string.IsNullOrEmpty(FolioActual.Text) ? 1 : (Convert.ToInt32(FolioActual.Text) <= 0 ? 1 : Convert.ToInt32(FolioActual.Text)),
                                    tipo = Convert.ToInt32(TipoDocumento.SelectedValue),
                                    emisor = emis
                                });
                            }
                            break;
                        case 3://Eliminar folio
                            idFol = Convert.ToInt32(this.idFolio.Value);
                            var deleteFolio = db.folio
                                    .Where(x => x.id == idFol)
                                    .Delete();
                            break;
                    }
                }
                this.LimpiarFolios();
                this.ErrorFolio.Text = string.Empty;
            }
            catch(Exception ex)
            {
                ErrorFolio.Text = ex.Message;
            }
        }

        protected void LimpiarCampos()
        {
            this.idEmisor.Value = string.Empty;
            this.Identificador.Text = string.Empty;
            this.RFC.Text = string.Empty;
            this.RazonSocial.Text = string.Empty;
            this.CURP.Text = string.Empty;
            //this.RegimenFiscal.Text = string.Empty;
            this.RFC.Text = string.Empty;
            //this.Calle.Text = string.Empty;
            //this.NoExterior.Text = string.Empty;
            //this.NoInterior.Text = string.Empty;
            //this.Colonia.Text = string.Empty;
            //this.Referencia.Text = string.Empty;
            //this.Localidad.Text = string.Empty;
            //this.Municipio.Text = string.Empty;
            //this.Estado.Text = string.Empty;
            //this.Pais.Text = string.Empty;
            this.CodigoPostal.Text = string.Empty;
            this.Contrasenia.Text = string.Empty;
            this.Contrasenia.Attributes["value"] = string.Empty;
            //this.SucursalCalle.Text = string.Empty;
            //this.SucursalNoExterior.Text = string.Empty;
            //this.SucursalNoInterior.Text = string.Empty;
            //this.SucursalColonia.Text = string.Empty;
            //this.SucursalReferencia.Text = string.Empty;
            //this.SucursalLocalidad.Text = string.Empty;
            //this.SucursalMunicipio.Text = string.Empty;
            //this.SucursalEstado.Text = string.Empty;
            //this.SucursalPais.Text = string.Empty;
            //this.SucursalCodigoPostal.Text = string.Empty;

            //this.chkSucursal.Checked = false;
            //this.MostrarSucursal(false);
            FileLogo.Enabled = true;
            FileCedula.Enabled = true;
            LlavePrivada.Enabled = true;
            Certificado.Enabled = true;
        }

        protected void btnRegistro_Click(object sender, EventArgs e)
        {
            if (this.idEmisor.Value.Split('|')[0] != "0")
                this.LimpiarCampos();
            this.idEmisor.Value = 0 + "|" + 1;
            
            titleEmisor.Text = "Registrar nuevo emisor";
            btnEmisor.CssClass = "btn btn-primary";
            btnEmisor.Text = "Registrar";
            this.rfvLLavePrivada.Enabled = true;
            this.rfvCertificado.Enabled = true;
            FileLogo.Enabled = true;
            FileCedula.Enabled = true;
            LlavePrivada.Enabled = true;
            Certificado.Enabled = true;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#registro').modal('show');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "RegistroModalScript", sb.ToString(), false);
        }

        //protected void chkSucursal_CheckedChanged(object sender, EventArgs e)
        //{
        //    CheckBox check = (CheckBox)sender;
        //    MostrarSucursal(check.Checked);
        //}

        //private void MostrarSucursal(bool mostrar)
        //{
        //    divSucursal.Style.Remove(HtmlTextWriterStyle.Display);
        //    divSucursal.Style.Add(HtmlTextWriterStyle.Display, mostrar ? "block" : "none");

        //    rfvSucursalPais.Enabled = mostrar;
        //}

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            this.LimpiarFolios();
            this.ErrorFolio.Text = string.Empty;
        }

        protected void viewFolios_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Editar")
                {
                    int id = Convert.ToInt32(viewFolios.DataKeys[Convert.ToInt32(e.CommandArgument)].Value);
                    EditarFolio(id, 2);
                }
                else if (e.CommandName == "Eliminar")
                {
                    int id = Convert.ToInt32(viewFolios.DataKeys[Convert.ToInt32(e.CommandArgument)].Value);
                    EditarFolio(id, 3);
                }
                ErrorFolio.Text = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorFolio.Text = ex.Message;
            }
        }

        protected void viewcProdServ_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Eliminar")
                {
                    //int id = Convert.ToInt32(viewcProdServ.DataKeys[Convert.ToInt32(e.CommandArgument)].Value);
                    string cadena = Convert.ToString(viewcProdServ.DataKeys[Convert.ToInt32(e.CommandArgument)].Value);
                    Guid id = Guid.Parse(cadena);
                    EditarClave(id, 3);
                }
                ErrorFolio.Text = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorFolio.Text = ex.Message;
            }
        }

        private void EditarFolio(int id, int idInstruccion)
        {
            using (var db = new DataModel.OstarDB())
            {
                int idEmpresa = Convert.ToInt32(this.idFolioEmpresa.Value);
                //var folios = db.folio.Where(x => (x.emisor == idEmpresa && x.id != id));

                var folios = (from f in db.folio
                              join t in db.tipo on f.tipo equals t.id
                              where f.emisor == idEmpresa && f.id != id
                              orderby f.serie ascending
                              select new
                              {
                                  id = f.id,
                                  f.serie,
                                  f.folio_inicio,
                                  f.folio_actual,
                                  t.tipo_comprobante
                              });

                viewFolios.DataSource = folios;
                viewFolios.DataBind();

                var folio = db.folio.Where(x => x.id == id).FirstOrDefault();
                if (folio == null)
                    throw new Exception("No se encontro la serie seleccionada");
                else
                {
                    this.idFolio.Value = folio.id.ToString();
                    this.idFolioInstruccion.Value = idInstruccion.ToString();
                    Serie.Text = folio.serie;
                    FolioInicio.Text = folio.folio_inicio.ToString();
                    FolioActual.Text = folio.folio_actual.ToString();
                    TipoDocumento.Text = folio.tipo.ToString();
                }

                if (idInstruccion == 2) //Editar
                {
                    btnAgregarFolio.Text = "<span class='glyphicon glyphicon-floppy-disk'></span> Editar Folio";
                    btnAgregarFolio.CssClass = "btn btn-success pull-right";
                }
                else if (idInstruccion == 3) //Eliminar
                {
                    btnAgregarFolio.Text = "<span class='glyphicon glyphicon-trash'></span> Eliminar Folio";
                    btnAgregarFolio.CssClass = "btn btn-danger pull-right";
                }
            }
        }

        private void EditarClave(Guid id, int idInstruccion)
        {
            using (var db = new DataModel.OstarDB())
            {
                this.idcProdServInstruccion.Value = idInstruccion.ToString();
                this.idClave.Value = id.ToString();
                if (idInstruccion == 3) //Eliminar
                {
                    btnAgregarcProdServ.Text = "<span class='glyphicon glyphicon-trash'></span> Eliminar Folio";
                    btnAgregarcProdServ.CssClass = "btn btn-danger pull-right";
                }
            }
        }

        protected void rblReporte_SelectedIndexChanged(object sender, EventArgs e)
        {
            EditarPDF();
        }

        private void EditarPDF()
        {
            using (var db = new DataModel.OstarDB())
            {
                int idEmpresa = Convert.ToInt32(this.idReporteEmpresa.Value);

                var updatePdf = db.emisor.Where(x => x.idemisor == idEmpresa)
                    .Update(e => new DataModel.emisor
                    {
                        tipo_pdf = Convert.ToInt32(rblReporte.SelectedValue)
                    });
            }
        }
    }
}