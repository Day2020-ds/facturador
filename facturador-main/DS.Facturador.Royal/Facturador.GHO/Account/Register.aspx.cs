using Microsoft.AspNet.Identity;
using AspNet.Identity.MySQL;
using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using Facturador.GHO.Models;

namespace Facturador.GHO.Account
{
    public partial class Register : Page
    {
        protected void CreateUser_Click(object sender, EventArgs e)
        {
            var manager = new UserManager();
            var roleManager = new RoleManager();
            if (!roleManager.RoleExists("Admin"))
            {
                var roleResult = roleManager.Create(new IdentityRole("Admin"));
            }
            var user = new IdentityUser() { UserName = UserName.Text };
            IdentityResult result = manager.Create(user, Password.Text);
            if (result.Succeeded)
            {
                var role = manager.AddToRole(user.Id, "Admin");
                IdentityHelper.SignIn(manager, user, isPersistent: false);
                IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
            }
            else 
            {
                ErrorMessage.Text = result.Errors.FirstOrDefault();
            }
        }
    }
}