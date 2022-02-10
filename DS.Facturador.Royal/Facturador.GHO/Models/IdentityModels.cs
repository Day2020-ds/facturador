using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Web;
using System;
using Facturador.GHO.Models;
using AspNet.Identity.MySQL;

namespace Facturador.GHO.Models
{
    // You can add User data for the user by adding more properties to your User class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    //public class IdentityUser : IdentityUser
    //{
    //}

    //public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    //{
    //    public ApplicationDbContext()
    //        : base("DefaultConnection")
    //    {
    //    }
    //}

    #region Helpers
    public class UserManager : UserManager<IdentityUser>
    {
        public UserManager()
            : base(new UserStore(new MySQLDatabase()))
        {
            this.UserValidator = new UserValidator<IdentityUser>(this) { AllowOnlyAlphanumericUserNames = false };
        }
    }

    public class RoleManager : RoleManager<IdentityRole>
    {
        public RoleManager()
            : base(new RoleStore(new MySQLDatabase()))
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

        public static void SignIn(UserManager manager, IdentityUser user, bool isPersistent)
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