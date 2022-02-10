using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using BLToolkit.Data;
using BLToolkit.Data.Linq;
using Facturador.GHO.Controllers;

namespace Facturador.GHO.Admin
{
    public partial class Configuracion : System.Web.UI.Page
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
                if (!IsPostBack)
                {
                    ObtenerInfo();
                }
            }
            catch(Exception ex)
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
            NumeroSerie.Text = lic.GetHardwareID();
            if (lic.IsValidLicenseAvailable())
                Estatus.Text = "Con licencia para " + lic.ObtenerCompania();
            else
                Estatus.Text = "No se cuenta con licencia";
        }

        protected void ObtenerInfo()
        {
            try
            {
                using (var db = new DataModel.OstarDB())
                {
                    var info = db.config.FirstOrDefault();

                    if (info != null)
                    {
                        AplicacionNombre.Text = info.aplicacion;
                        //Correo.Text = string.IsNullOrEmpty(info.mensaje_correo) ? mensajeAlternativo : info.mensaje_correo;
                        Host.Text = info.servidor;
                        Puerto.Text = info.puerto.ToString();
                        SSL.Checked = info.ssl == 1 ? true : false;
                        Nombre.Text = info.nombre;
                        Email.Text = info.correo;
                        Contrasenia.Text = string.IsNullOrEmpty(info.contrasenia) ? "" : seg.Desencriptar(info.contrasenia);
                        Contrasenia.Attributes["value"] = string.IsNullOrEmpty(info.contrasenia) ? "" : seg.Desencriptar(info.contrasenia);
                    }

                    var serv = db.servisim.FirstOrDefault();
                    if (serv != null)
                    {                        
                        UsuarioServisim.Text = serv.usuario;
                        PasswordServisim.Text = serv.contrasenia;
                        TokenServisim.Text = serv.token;
                        PasswordServisim.Attributes["value"] = string.IsNullOrEmpty(serv.contrasenia) ? "" : seg.Desencriptar(serv.contrasenia);
                        TokenServisim.Attributes["value"] = string.IsNullOrEmpty(serv.token) ? "" : seg.Desencriptar(serv.token);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = "Error al obtener la configuración. " + ex.Message;
            }
        }

        protected void btnActivar_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(rutaLicencia))
            {
                Directory.CreateDirectory(rutaLicencia);
            }
            fileLicense.PostedFile.SaveAs(rutaLicencia + "licencia.vali");
            lic.LoadLicense(rutaLicencia + "licencia.vali");
            if (lic.IsValidLicenseAvailable())
            {
                lic.ReadLicenseInformation();
                ErrorMessage.Text = "<strong>Exito!</strong> Tu licencia a sido activada correctamente";
                Estatus.Text = "Con licencia para " + lic.ObtenerCompania();
            }
            else
            {
                ErrorMessage.Text = "<strong>Error!</strong> La licencia seleccionada no es valida";
                Estatus.Text = "La licencia no es valida";
            }
        }

        protected void btnNombre_Click(object sender, EventArgs e)
        {
            try
            {
                using (var db = new DataModel.OstarDB())
                {
                    var info = db.config.FirstOrDefault();
                    if (info != null)
                    {
                        var upd = db.config.Where(x => x.aplicacion == info.aplicacion)
                            .Update(x => new DataModel.config
                            {
                                aplicacion = AplicacionNombre.Text
                            });
                    }
                    else
                    {
                        var ins = db.config.Insert(() => new DataModel.config
                        {
                            aplicacion = AplicacionNombre.Text
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = "Error al activar. " + ex.Message;
            }
        }

        protected void btnLogo_Click(object sender, EventArgs e)
        {
            string rutaLogo = Server.MapPath("~/Facturacion/Logo/");
            if (!Directory.Exists(rutaLogo))
            {
                Directory.CreateDirectory(rutaLogo);
            }
            file1.PostedFile.SaveAs(rutaLogo + "logo.png");
        }

        private void EnviarCorreo(string asunto, string receptor)
        {
            try
            {
                CorreoElectronico mail = new CorreoElectronico();
                mail.AgregarDestinatario(receptor);

                string mensaje = "<tr>" +
                    "<td style=\"width:157px;height:157px;\"></td>" +
                    "<td style=\"height:157px;\">" +
                    "<div style=\"text-align:left;font-size:13px;direction:ltr;font-family:Arial;font-variant:normal;font-weight:normal;\">" + "" +
                    "<font size=\"3\"><strong>Configuración de correo exitosa. </strong></font><br>" +
                    "<br>" +
                    "<p>Por su atenci&oacute;n gracias.</p>" +
                    "<p>Favor de no responder este correo. Los correos son enviados por un programa autom&aacute;tico.</p>" +
                    "</div>" +
                    "</td>" +
                    "</tr>";

                mail.EnviarCorreo(asunto, mensaje, true);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                bool exep = ex.InnerException != null;

                while (exep)
                {
                    if (ex.Message == ex.InnerException.Message)
                    {
                        exep = false;
                    }
                    else
                    {
                        ex = ex.InnerException;
                        error += " " + ex.Message;
                        exep = ex.InnerException != null;
                    }
                }
                //ErrorMessage.Text = error;
                throw new Exception(error);
            }
        }

        protected void btnCorreo_Click(object sender, EventArgs e)
        {
            try
            {
                using (var db = new DataModel.OstarDB())
                {
                    var info = db.config.FirstOrDefault();
                    if (info != null)
                    {
                        var upd = db.config.Where(x => x.aplicacion == info.aplicacion)
                            .Update(x => new DataModel.config
                            {
                                servidor = Host.Text,
                                puerto = Convert.ToInt32(Puerto.Text),
                                ssl = SSL.Checked ? 1 : 0,
                                nombre = Nombre.Text,
                                correo = Email.Text,
                                contrasenia = seg.Encriptar(Contrasenia.Text)
                            });
                    }
                    else
                    {
                        var ins = db.config.Insert(() => new DataModel.config
                        {
                            aplicacion = "",
                            servidor = Host.Text,
                            puerto = Convert.ToInt32(Puerto.Text),
                            ssl = SSL.Checked ? 1 : 0,
                            nombre = Nombre.Text,
                            correo = Email.Text,
                            contrasenia = seg.Encriptar(Contrasenia.Text)
                        });
                    }
                    Contrasenia.Attributes["value"] = Contrasenia.Text;
                    EnviarCorreo("Prueba de correo", Email.Text);
                    ErrorMessage.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = "Error al guardar configuración del correo. " + ex.Message;
            }
        }
        protected void btnMensaje_Click(object sender, EventArgs e)
        {
            try
            {
                using (var db = new DataModel.OstarDB())
                {
                    var info = db.config.FirstOrDefault();
                    if (info != null)
                    {
                        var upd = db.config.Where(x => x.nombre == info.nombre)
                            .Update(x => new DataModel.config
                            {
                                //mensaje = Correo.Text
                            });
                    }
                    else
                    {
                        var ins = db.config.Insert(() => new DataModel.config
                        {
                            aplicacion = "",
                            //mensaje = Correo.Text
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = "Error al guardar mensaje. " + ex.Message;
            }
        }

        protected void btnServisim_Click(object sender, EventArgs e)
        {
            try
            {
                using (var db = new DataModel.OstarDB())
                {
                    var info = db.servisim.FirstOrDefault();
                    if (info != null)
                    {
                        var upd = db.servisim.Where(x => x.id == info.id)
                            .Update(x => new DataModel.servisim
                            {
                                usuario = UsuarioServisim.Text,
                                contrasenia = string.IsNullOrWhiteSpace(PasswordServisim.Text) ? "" : seg.Encriptar(PasswordServisim.Text),
                                token = string.IsNullOrWhiteSpace(TokenServisim.Text) ? "" : seg.Encriptar(TokenServisim.Text)
                            });
                    }
                    else
                    {
                        var ins = db.servisim.Insert(() => new DataModel.servisim
                        {
                            usuario = UsuarioServisim.Text,
                            contrasenia = string.IsNullOrWhiteSpace(PasswordServisim.Text) ? "" : seg.Encriptar(PasswordServisim.Text),
                            token = string.IsNullOrWhiteSpace(TokenServisim.Text) ? "" : seg.Encriptar(TokenServisim.Text)
                        });
                    }
                    PasswordServisim.Attributes["value"] = PasswordServisim.Text;
                    TokenServisim.Attributes["value"] = TokenServisim.Text;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = "Error al guardar la información. " + ex.Message;
            }
        }
    }
}