using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Facturador.GHO.Startup))]
namespace Facturador.GHO
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
