using Microsoft.AspNet.Identity;
using AspNet.Identity.MySQL;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLToolkit.Data.Linq;
using System.Collections.Generic;
using Facturador.GHO.Models;
using Facturador.GHO.Controllers;

namespace Facturador.GHO.Admin
{
    public partial class Usuario : System.Web.UI.Page
    {
        private Seguridad seg;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                seg = new Seguridad();
                if (!IsPostBack)
                {
                    LlenarUsuarios();
                    LlenarEmisores();
                }
                ErrorMessage.Text = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = ex.Message;
            }
        }

        protected void LlenarEmisores()
        {
            using (var db = new DataModel.OstarDB())
            {
                var emisores = from h in db.emisor
                               select new
                               {
                                   h.idemisor,
                                   h.identificador,
                                   razon_social = seg.Desencriptar(h.razon_social),
                                   rfc = seg.Desencriptar(h.rfc)
                               };

                viewEmpresa.DataSource = emisores;
                viewEmpresa.DataBind();
            }
        }
        protected void LLenarEmisoresUsuario(string idUsuario)
        {
            using (var db = new DataModel.OstarDB())
            {
                List<int> emisoresActuales = (from e in db.emisor
                                                 join ue in db.useremisor on e.idemisor equals ue.EmisorId
                                                 where ue.UserId == idUsuario
                                                 select e.idemisor).ToList();

                var emisores = from h in db.emisor
                               select new
                               {
                                   h.idemisor,
                                   h.identificador,
                                   razon_social = seg.Desencriptar(h.razon_social),
                                   rfc = seg.Desencriptar(h.rfc),
                                   permisos = emisoresActuales.Contains(h.idemisor) ? true : false
                               };

                viewEditarEmpresa.DataSource = emisores;
                viewEditarEmpresa.DataBind();
            }
        }
        protected void LlenarUsuarios()
        {
            using (var db = new DataModel.OstarDB())
            {
                var query = (from u in db.users
                             join ur in db.userroles on u.Id equals ur.UserId
                             join r in db.roles on ur.RoleId equals r.Id
                             where (r.Name == "Admin" || r.Name == "Usuario")
                             select new
                             {
                                 u.Id,
                                 u.UserName,
                                 Email = string.IsNullOrEmpty(u.Email) ? null : seg.Desencriptar(u.Email),
                                 Phone = string.IsNullOrEmpty(u.Phone) ? null : seg.Desencriptar(u.Phone),
                                 IsAdmin = r.Name == "Admin" ? true : false
                             }
                     );

                viewUsuarios.DataSource = query;
                viewUsuarios.DataBind();
            }
        }
        protected void CreateUsuarios_Click(object sender, EventArgs e)
        {
            try
            {
                var manager = new UserManager();
                var roleManager = new RoleManager();
                if (!roleManager.RoleExists("Usuario"))
                {
                    var roleResult = roleManager.Create(new IdentityRole("Usuario"));
                }
                var user = new IdentityUser()
                {
                    UserName = UserName.Text,
                    Email = string.IsNullOrEmpty(Correo.Text) ? "" : seg.Encriptar(Correo.Text),
                    Phone = string.IsNullOrEmpty(Telefono.Text) ? "" : seg.Encriptar(Telefono.Text)
                };
                IdentityResult result = manager.Create(user, Correo.Text);
                if (result.Succeeded)
                {
                    var role = manager.AddToRole(user.Id, chkAdministrador.Checked ? "Admin" : "Usuario");

                    using (var db = new DataModel.OstarDB())
                    {
                        foreach (GridViewRow row in viewEmpresa.Rows)
                        {
                            int emisorId = Convert.ToInt32(viewEmpresa.DataKeys[row.RowIndex].Value.ToString());
                            CheckBox chk = (CheckBox)row.FindControl("chkPermiso");
                            if (chk != null)
                            {
                                if (chk.Checked)
                                {
                                    var insert = db.useremisor.Insert(() => new DataModel.useremisor
                                    {
                                        UserId = user.Id,
                                        EmisorId = emisorId
                                    });
                                }
                            }
                        }
                    }
                    EnviarCorreo("Credenciales de acceso", Correo.Text, UserName.Text, UserName.Text);
                }
                else
                {
                    ErrorMessage.Text = result.Errors.FirstOrDefault();
                }
                LlenarUsuarios();
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = ex.Message;
            }
        }
        protected void EditUsuarios_Click(object sender, EventArgs e)
        {
            string id = this.idEditar.Value;
            EditarUsuarios(id);
        }
        protected void EditarUsuarios(string id)
        {
            UserManager manager = new UserManager();
            var user = manager.FindById(id);
            if (user != null)
            {
                user.Email = string.IsNullOrEmpty(EditarCorreo.Text) ? "" : seg.Encriptar(EditarCorreo.Text);
                user.Phone = string.IsNullOrEmpty(EditarTelefono.Text) ? "" : seg.Encriptar(EditarTelefono.Text);
                IdentityResult result = manager.Update(user);
                if (result.Succeeded)
                {
                    var role = manager.AddToRole(user.Id, chkEditAdministrador.Checked ? "Admin" : "Usuario");
                    using (var db = new DataModel.OstarDB())
                    {
                        List<string> empresas = (from e in db.emisor
                                                 join ue in db.useremisor on e.idemisor equals ue.EmisorId
                                                 where ue.UserId == id
                                                 select e.idemisor.ToString()).ToList();

                        foreach (GridViewRow row in viewEditarEmpresa.Rows)
                        {
                            int emisorId = Convert.ToInt32(viewEditarEmpresa.DataKeys[row.RowIndex].Value.ToString());
                            CheckBox chk = (CheckBox)row.FindControl("chkPermiso");
                            if (chk != null)
                            {
                                if (empresas.Contains(emisorId.ToString()))
                                {
                                    if (!chk.Checked)
                                    {
                                        var delete = db.useremisor
                                            .Where(h => h.UserId == id && h.EmisorId == emisorId)
                                            .Delete();
                                    }
                                }
                                else
                                {
                                    if (chk.Checked)
                                    {
                                        var insert = db.useremisor.Insert(() => new DataModel.useremisor
                                        {
                                            UserId = id,
                                            EmisorId = emisorId
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            LlenarUsuarios();
        }
        protected void DeleteUsuarios_Click(object sender, EventArgs e)
        {
            string id = this.idEliminar.Value;
            EliminarUsuarios(id);
        }
        protected void EliminarUsuarios(string id)
        {
            UserManager manager = new UserManager();
            string role = manager.IsInRole(id, "Admin") ? "Admin" : "Usuario";
            IdentityResult result = manager.RemoveFromRole(id, role);
            if (result.Succeeded)
            {
            }
            LlenarUsuarios();
        }
        protected void viewUsuarios_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Editar")
            {
                string id = viewUsuarios.DataKeys[Convert.ToInt32(e.CommandArgument)].Value.ToString();
                LlenarEditar(id);
            }
            if (e.CommandName == "Eliminar")
            {
                string id = viewUsuarios.DataKeys[Convert.ToInt32(e.CommandArgument)].Value.ToString();
                LlenarEliminar(id);
            }
        }
        protected void LlenarEditar(string id)
        {
            UserManager manager = new UserManager();
            var user = manager.FindById(id);
            if (user != null)
            {
                EditarCorreo.Text = string.IsNullOrEmpty(user.Email) ? "" : seg.Desencriptar(user.Email);
                EditarTelefono.Text = string.IsNullOrEmpty(user.Phone) ? "" : seg.Desencriptar(user.Phone);
                chkEditAdministrador.Checked = manager.IsInRole(user.Id, "Admin");
                this.idEditar.Value = id;

                this.LLenarEmisoresUsuario(id);

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#editar').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EditarModalScript", sb.ToString(), false);
            }
        }
        protected void LlenarEliminar(string id)
        {
            UserManager manager = new UserManager();
            var user = manager.FindById(id);
            if (user != null)
            {
                this.idEliminar.Value = id.ToString();
                this.EliminarCorreo.Text = string.IsNullOrEmpty(user.Email) ? "" : seg.Desencriptar(user.Email);
                this.EliminarTelefono.Text = string.IsNullOrEmpty(user.Phone) ? "" : seg.Desencriptar(user.Phone);

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#eliminar').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EliminarModalScript", sb.ToString(), false);
            }
        }
        protected void EnviarCorreo(string asunto, string receptor, string usuario, string contacto)
        {
            try
            {
                CorreoElectronico mail = new CorreoElectronico();
                mail.AgregarDestinatario(receptor);

                string mensaje = "<tr>" +
                    "<td style=\"width:157px;height:157px;\"></td>" +
                    "<td style=\"height:157px;\">" +
                    "<p style=\"text-align:left;font-size:13px;color:#0b6dc5;direction:ltr;font-family:Arial;font-variant:normal;font-weight:normal;\">" + "" +
                    "<font size=\"3\"><strong>Estimado " + "Usuario" + ": </strong></font><br>" +
                    "<br>" +
                    "Les hacemos llegar sus credenciales de acceso." +
                    "<br>" +
                    "<br>" +
                    "Usuario: " + usuario +
                    "<br>" +
                    "Contraseña: " + receptor +
                    "<br>" +
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
                ErrorMessage.Text = ex.Message;
            }
        }
    }
}