using System;
using System.Linq;
using System.Web.UI;
using BLToolkit.Data.Linq;
using System.Web.UI.WebControls;

namespace Facturador.GHO.Admin
{
    public partial class ClaveProd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    LlenarClavesProd();
                }
                ErrorMessage.Text = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = ex.Message;
            }
        }
        protected void LlenarClavesProd()
        {
            using (var db = new DataModel.OstarDB())
            {
                var clavesProd = db.cClaveProdServ;

                viewClavesProd.DataSource = clavesProd;
                viewClavesProd.DataBind();
            }
        }

        protected void viewClavesProd_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Editar")
                {
                    string id = viewClavesProd.DataKeys[Convert.ToInt32(e.CommandArgument)].Value.ToString();
                    LlenarClavesProd(id, 2);
                }
                else if (e.CommandName == "Eliminar")
                {
                    string id = viewClavesProd.DataKeys[Convert.ToInt32(e.CommandArgument)].Value.ToString();
                    LlenarClavesProd(id, 3);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = ex.Message;
            }
        }
        protected void LlenarClavesProd(string id, int idInstruccion)
        {
            string instruccion = string.Empty;
            switch (idInstruccion)
            {
                case 2://Editar clave producto
                    instruccion = "Editar Clave Producto: ";
                    btnClavesProd.CssClass = "btn btn-primary";
                    btnClavesProd.Text = "Editar";
                    break;
                case 3://Eliminar clave producto
                    instruccion = "Eliminar Clave Producto: ";
                    btnClavesProd.CssClass = "btn btn-danger";
                    btnClavesProd.Text = "Eliminar";
                    break;
            }
            using (var db = new DataModel.OstarDB())
            {
                var query = db.cClaveProdServ.Where(x => x.cClaveProdServ_id == id).FirstOrDefault();
                if (query != null)
                {
                    titleClaveProd.Text = instruccion + query.codigo;
                    this.cClaveProdServ_id.Value = id + "|" + idInstruccion;

                    this.Codigo.Text = query.codigo;
                    this.Descripcion.Text = query.descripcion;
                }

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#registro').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "RegistroModalScript", sb.ToString(), false);
            }
        }

        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (this.cClaveProdServ_id.Value.Split('|')[0] != "0")
                this.LimpiarCampos();
            this.cClaveProdServ_id.Value = 0 + "|" + 1;

            titleClaveProd.Text = "Agregar nueva clave producto";
            btnClavesProd.CssClass = "btn btn-primary";
            btnClavesProd.Text = "Registrar";

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#registro').modal('show');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "RegistroModalScript", sb.ToString(), false);
        }

        private void LimpiarCampos()
        {
            this.Codigo.Text = string.Empty;
            this.Descripcion.Text = string.Empty;
        }

        protected void btnClavesProd_Click(object sender, EventArgs e)
        {
            try
            {
                string[] instrucciones = this.cClaveProdServ_id.Value.Split('|');
                string id = instrucciones[0];
                int idInstruccion = Convert.ToInt32(instrucciones[1]);
                switch (idInstruccion)
                {
                    case 1://Registrar empresa
                        this.RegistrarClaveProd();
                        break;
                    case 2://Editar empresa
                        this.EditarClaveProd(id);
                        break;
                    case 3://Eliminar empresa
                        this.EliminarClaveProd(id);
                        break;
                }
                this.LlenarClavesProd();
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#registro').modal('hide');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "RegistroModalScript", sb.ToString(), false);
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = ex.Message;
            }
        }
        private void RegistrarClaveProd()
        {
            using (var db = new DataModel.OstarDB())
            {
                db.BeginTransaction();

                var query = db.cClaveProdServ.Insert(() => new DataModel.cClaveProdServ
                {
                    cClaveProdServ_id = Guid.NewGuid().ToString(),
                    codigo = Codigo.Text,
                    descripcion = Descripcion.Text
                });

                db.CommitTransaction();
            }
        }

        private void EditarClaveProd(string id)
        {
            using (var db = new DataModel.OstarDB())
            {
                db.BeginTransaction();

                var query = db.cClaveProdServ
                    .Where(e => e.cClaveProdServ_id == id)
                    .Update(e => new DataModel.cClaveProdServ
                    {
                        codigo = Codigo.Text,
                        descripcion = Descripcion.Text
                    });
                db.CommitTransaction();
            }
        }

        protected void EliminarClaveProd(string id)
        {
            using (var db = new DataModel.OstarDB())
            {
                db.BeginTransaction();

                var query = db.cClaveProdServ
                    .Delete(e => e.cClaveProdServ_id == id);

                db.CommitTransaction();
            }
        }
    }
}