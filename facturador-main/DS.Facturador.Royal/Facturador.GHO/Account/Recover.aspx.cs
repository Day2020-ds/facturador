using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Facturador.GHO.Models;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using AspNet.Identity.MySQL;
using Microsoft.AspNet.Identity.Owin;
using Facturador.GHO.Controllers;
namespace Facturador.GHO.Account
{
    public partial class Recover : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Recover_Click(object sender, EventArgs e)
        {
            try
            {
                Controllers.Seguridad seguridad = new Controllers.Seguridad();
                var manager = new UserManager();
                IdentityUser user = manager.FindByName(UserName.Text);
                if (user != null)
                {
                    if (seguridad.Desencriptar(user.Email) == Correo.Text)
                    {
                        string password = Membership.GeneratePassword(10, 2);
                        IdentityResult rs1 = manager.RemovePassword(user.Id);
                        if (rs1.Succeeded)
                        {
                            rs1 = manager.AddPassword(user.Id, password);
                            if (rs1.Succeeded)
                            {
                                EnviarCorreo("Recuperación de contraseña", user.UserName, password, Correo.Text);
                                ErrorMessage.Text = "La contraseña a sido restaurada y enviada al correo proporcionado";
                            }
                            else
                                throw new Exception("Error al generar contraseña");
                        }
                        else
                            throw new Exception("Error al generar contraseña");
                    }
                    else
                    {
                        throw new Exception("El usuario o correo no son validos");
                    }
                }
                else
                {
                    throw new Exception("El usuario o correo no son validos");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = ex.Message;
            }
        }

        protected void EnviarCorreo(string asunto, string usuario, string password, string correo)
        {
            try
            {
                CorreoElectronico mail = new CorreoElectronico();
                mail.AgregarDestinatario(correo);

                string mensaje = "<tr>" +
                    "<td style=\"width:157px;height:157px;\"></td>" +
                    "<td style=\"height:157px;\">" +
                    "<p style=\"text-align:left;font-size:13px;color:#0b6dc5;direction:ltr;font-family:Arial;font-variant:normal;font-weight:normal;\">" + "" +
                    "<font size=\"3\"><strong>Estimado usuario, le hacemos llegar sus nuevas credenciales de acceso. </strong></font><br>" +
                    "<br>" +
                    "Usuario: " + usuario +
                    "<br>" +
                    "Contraseña: " + password +
                    "<br>" +
                    "Favor de no responder a este correo. Los correos son enviados por un programa automático." +
                    "<br>" +
                    "</p>" +
                    "</td>" +
                    "</tr>";

                mail.EnviarCorreo(asunto, mensaje, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}