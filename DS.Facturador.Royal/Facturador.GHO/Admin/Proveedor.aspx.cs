using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLToolkit.Data;
using BLToolkit.Data.Linq;

namespace Facturador.GHO.Admin
{
    public partial class Proveedor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LlenarProveedores();
        }

        protected void LlenarProveedores()
        {
            using (var db = new DataModel.OstarDB())
            {
                var query = db.proveedor;

                viewProveedores.DataSource = query;
                viewProveedores.DataBind();
            }
        }

        protected void Registrar(object sender, EventArgs e)
        {
            using (var db = new DataModel.OstarDB())
            {
                var value = db.proveedor.Insert(() => new DataModel.proveedor
                    {
                        razon_social = this.RazonSocial.Text,
                        rfc = this.RFC.Text,
                        correo_electronico = this.Correo.Text,
                        contacto = this.Contacto.Text,
                        telefono = this.Telefono.Text
                    });
            }
            LlenarProveedores();
        }

        protected void Editar(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(this.idEditar.Value);
            Editarproveedor(id);
        }

        protected void Editarproveedor(int id)
        {
            using (var db = new DataModel.OstarDB())
            {
                var value = db.proveedor
                    .Where(e => e.idproveedor == id)
                    .Update(e => new DataModel.proveedor
                {
                    razon_social = this.EditarRazonSocial.Text,
                    rfc = this.EditarRFC.Text,
                    correo_electronico = this.EditarCorreo.Text,
                    contacto = this.EditarContacto.Text,
                    telefono = this.EditarTelefono.Text
                });
            }
            LlenarProveedores();
        }

        protected void viewProveedores_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Editar")
            {
                int id = Convert.ToInt32(viewProveedores.DataKeys[Convert.ToInt32(e.CommandArgument)].Value);
                LlenarEditar(id);
            }
            if (e.CommandName == "Eliminar")
            {
                int id = Convert.ToInt32(viewProveedores.DataKeys[Convert.ToInt32(e.CommandArgument)].Value);
                LlenarEliminar(id);
            }
        }

        protected void LlenarEditar(int id)
        {
            using (var db = new DataModel.OstarDB())
            {
                var query =
                    (from e in db.proveedor
                     where e.idproveedor == id
                     select e).FirstOrDefault();
                if (query != null)
                {
                    this.idEditar.Value = id.ToString();
                    this.EditarRazonSocial.Text = query.razon_social;
                    this.EditarRFC.Text = query.rfc;
                    this.EditarCorreo.Text = query.correo_electronico;
                    this.EditarContacto.Text = query.contacto;
                    this.EditarTelefono.Text = query.telefono;

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append(@"<script type='text/javascript'>");
                    sb.Append("$('#editar').modal('show');");
                    sb.Append(@"</script>");
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EditarModalScript", sb.ToString(), false);
                }
            }
        }

        protected void LlenarEliminar(int id)
        {
            using (var db = new DataModel.OstarDB())
            {
                var query =
                    (from e in db.proveedor
                     where e.idproveedor == id
                     select e).FirstOrDefault();
                if (query != null)
                {
                    this.idEliminar.Value = id.ToString();
                    this.EliminarRazonSocial.Text = query.razon_social;
                    this.EliminarRFC.Text = query.rfc;
                    this.EliminarCorreo.Text = query.correo_electronico;
                    this.EliminarContacto.Text = query.contacto;
                    this.EliminarTelefono.Text = query.telefono;

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append(@"<script type='text/javascript'>");
                    sb.Append("$('#eliminar').modal('show');");
                    sb.Append(@"</script>");
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EliminarModalScript", sb.ToString(), false);
                }
            }
        }

        protected void Eliminar(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(this.idEliminar.Value);
            Eliminarproveedor(id);
        }

        protected void Eliminarproveedor(int id)
        {
            using (var db = new DataModel.OstarDB())
            {
                var value = db.proveedor

                    .Delete(e => e.idproveedor == id);
            }
            LlenarProveedores();
        
        }

        
    }
}