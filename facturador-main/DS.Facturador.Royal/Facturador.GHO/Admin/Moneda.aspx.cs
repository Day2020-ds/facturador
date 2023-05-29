using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLToolkit.Data.Linq;

namespace Facturador.GHO.Admin
{
    public partial class Moneda : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    LlenarMonedas();
                }
                ErrorMessage.Text = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = ex.Message;
            }
        }
        protected void LlenarMonedas()
        {
            using (var db = new DataModel.OstarDB())
            {
                var monedas = db.moneda;

                viewMonedas.DataSource = monedas;
                viewMonedas.DataBind();
            }
        }

        protected void viewMonedas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Editar")
                {
                    int id = Convert.ToInt32(viewMonedas.DataKeys[Convert.ToInt32(e.CommandArgument)].Value);
                    LlenarMoneda(id, 2);
                }
                else if (e.CommandName == "Eliminar")
                {
                    int id = Convert.ToInt32(viewMonedas.DataKeys[Convert.ToInt32(e.CommandArgument)].Value);
                    LlenarMoneda(id, 3);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = ex.Message;
            }
        }
        protected void LlenarMoneda(int id, int idInstruccion)
        {
            string instruccion = string.Empty;
            switch (idInstruccion)
            {
                case 2://Editar moneda
                    instruccion = "Editar moneda: ";
                    btnMoneda.CssClass = "btn btn-primary";
                    btnMoneda.Text = "Editar";
                    break;
                case 3://Eliminar moneda
                    instruccion = "Eliminar moneda: ";
                    btnMoneda.CssClass = "btn btn-danger";
                    btnMoneda.Text = "Eliminar";
                    break;
            }
            using (var db = new DataModel.OstarDB())
            {
                var query = db.moneda.Where(x => x.id == id).FirstOrDefault();
                if (query != null)
                {
                    titleMoneda.Text = instruccion + query.nombre;
                    this.idMoneda.Value = id + "|" + idInstruccion;

                    this.Nombre.Text = query.nombre;
                    this.TipoCambio.Text = query.tipo_cambio.Value.ToString("0.00####");
                    this.Abreviatura.Text = query.abreviatura;
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
            if (this.idMoneda.Value.Split('|')[0] != "0")
                this.LimpiarCampos();
            this.idMoneda.Value = 0 + "|" + 1;

            titleMoneda.Text = "Agregar nueva moneda";
            btnMoneda.CssClass = "btn btn-primary";
            btnMoneda.Text = "Registrar";

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#registro').modal('show');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "RegistroModalScript", sb.ToString(), false);
        }

        private void LimpiarCampos()
        {
            this.Nombre.Text = string.Empty;
            this.TipoCambio.Text = string.Empty;
            this.Abreviatura.Text = string.Empty;
        }

        protected void btnMoneda_Click(object sender, EventArgs e)
        {
            try
            {
                string[] instrucciones = this.idMoneda.Value.Split('|');
                int id = Convert.ToInt32(instrucciones[0]);
                int idInstruccion = Convert.ToInt32(instrucciones[1]);
                switch (idInstruccion)
                {
                    case 1://Registrar empresa
                        this.RegistrarMoneda();
                        break;
                    case 2://Editar empresa
                        this.EditarMoneda(id);
                        break;
                    case 3://Eliminar empresa
                        this.EliminarMoneda(id);
                        break;
                }
                this.LlenarMonedas();
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
        private void RegistrarMoneda()
        {
            using (var db = new DataModel.OstarDB())
            {
                db.BeginTransaction();

                var query = db.moneda.Insert(() => new DataModel.moneda
                {
                    nombre = Nombre.Text,
                    tipo_cambio = Convert.ToDecimal(TipoCambio.Text),
                    abreviatura = Abreviatura.Text
                });

                db.CommitTransaction();
            }
        }

        private void EditarMoneda(int id)
        {
            using (var db = new DataModel.OstarDB())
            {
                db.BeginTransaction();

                var query = db.moneda
                    .Where(e => e.id == id)
                    .Update(e => new DataModel.moneda
                    {
                        nombre = Nombre.Text,
                        tipo_cambio = Convert.ToDecimal(TipoCambio.Text),
                        abreviatura = Abreviatura.Text
                    });
                db.CommitTransaction();
            }
        }

        protected void EliminarMoneda(int id)
        {
            using (var db = new DataModel.OstarDB())
            {
                db.BeginTransaction();

                var query = db.moneda
                    .Delete(e => e.id == id);

                db.CommitTransaction();
            }
        }
    }
}