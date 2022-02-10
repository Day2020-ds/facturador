using Microsoft.AspNet.Identity;
using AspNet.Identity.MySQL;
using System;
using System.Web.UI;
using Facturador.GHO.Models;

namespace Facturador.GHO.Account
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void LogIn(object sender, EventArgs e)
        {
            if (IsValid)
            {
                // Validar la contraseña del usuario
                var manager = new UserManager();
                IdentityUser user = manager.Find(UserName.Text, Password.Text);
                if (user != null)
                {
                    IdentityHelper.SignIn(manager, user, RememberMe.Checked);
                    IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
                }
                else
                {
                    FailureText.Text = "El usuario o contraseña no es valido.";
                    ErrorMessage.Visible = true;
                }
            }
        }
    }
}