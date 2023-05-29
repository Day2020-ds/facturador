using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System.Web;
using System;
using Facturador.GHO.Models;

namespace Facturador.GHO.Models
{
    public class IdentityUser : IUser

{

        public IdentityUser(){...}

        public IdentityUser(string userName) (){...}

        public string Id { get; set; }

        public string UserName { get; set; }

        public string PasswordHash { get; set; }

        public string SecurityStamp { get; set; }

}

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }
    }

    #region Helpers
    public class UserManager : UserManager<ApplicationUser>
    {
        public UserManager()
            : base(new UserStore<ApplicationUser>(new ApplicationDbContext()))
        {
        }
    }
}

namespace Facturador.GHO
{
    public static class IdentityHelper
    {
        // Used for XSRF when linking external logins
        public const string XsrfKey = "XsrfId";

        public static void SignIn(UserManager manager, ApplicationUser user, bool isPersistent)
        {
            IAuthenticationManager authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
            authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = manager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
            authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        public const string ProviderNameKey = "providerName";
        public static string GetProviderNameFromRequest(HttpRequest request)
        {
            return request[ProviderNameKey];
        }

        public static string GetExternalLoginRedirectUrl(string accountProvider)
        {
            return "/Account/RegisterExternalLogin?" + ProviderNameKey + "=" + accountProvider;
        }

        private static bool IsLocalUrl(string url)
        {
            return !string.IsNullOrEmpty(url) && ((url[0] == '/' && (url.Length == 1 || (url[1] != '/' && url[1] != '\\'))) || (url.Length > 1 && url[0] == '~' && url[1] == '/'));
        }

        public static void RedirectToReturnUrl(string returnUrl, HttpResponse response)
        {
            if (!String.IsNullOrEmpty(returnUrl) && IsLocalUrl(returnUrl))
            {
                response.Redirect(returnUrl);
            }
            else
            {
                response.Redirect("~/");
            }
        }
    }
    #endregion
}