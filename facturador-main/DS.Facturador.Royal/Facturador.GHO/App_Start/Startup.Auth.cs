using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace Facturador.GHO
{
    public partial class Startup {

        // Para obtener más información sobre la configuración de la autenticación, visite http://go.microsoft.com/fwlink/?LinkId=301883
        public void ConfigureAuth(IAppBuilder app)
        {
            // Habilitar la aplicación para que use una cookie para almacenar la información del usuario que inició sesión
            // y almacenar también información acerca de un usuario que inicie sesión con un proveedor de inicio de sesión de un tercero.
            // Es obligatorio si la aplicación permite a los usuarios iniciar sesión
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
        }
    }
}
