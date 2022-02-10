using System;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using AspNet.Identity.MySQL;
using Facturador.GHO.Models;

namespace Facturador.GHO
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_OnStart(object sender, EventArgs e)
        {
            // Código que se ejecuta al iniciar la aplicación
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // Comprueba que hay un rol de administrador y un usuario asociado
            var manager = new UserManager();
            var roleManager = new RoleManager();
            if (!roleManager.RoleExists("Super"))
            {
                roleManager.Create(new IdentityRole("Super"));
                var user = new IdentityUser() { UserName = "Administrador" };
                IdentityResult result = manager.Create(user, "Admin2014");
                if (result.Succeeded)
                {
                    var role = manager.AddToRole(user.Id, "Super");
                }
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}