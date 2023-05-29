using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLToolkit.Data.Linq;
using Facturador.GHO.Controllers;
using System.IO;
using Microsoft.AspNet.Identity;

namespace Facturador.GHO.Usuario
{
    public partial class Producto : System.Web.UI.Page
    {
        Seguridad seg;
        private License lic;
        string rutaLicencia = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                seg = new Seguridad();
                LeerLicencia();
                if (!IsPostBack)
                {
                    LlenarEmisores();
                    LlenarProductos(Convert.ToInt32(Empresa.SelectedValue));
                }
                ErrorMessage.Text = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = ex.Message;
            }
        }

        protected void LlenarProductos(int idEmpresa)
        {
            using (var db = new DataModel.OstarDB())
            {
                var empresas = db.producto.Where(x => x.emisor == idEmpresa)
                    .Select(x => new 
                    {
                        x.id,
                        x.codigo,
                        x.descripcion,
                        valor_unitario = x.valor_unitario.ToString("0,0.00####"),
                        iva = x.iva != null ? x.iva.Value.ToString("0,0.00####") : "",
                        ieps = x.iva != null ? x.ieps.Value.ToString("0,0.00####") : "",
                        ish = x.iva != null ? x.ish.Value.ToString("0,0.00####") : "",
                        isn = x.iva != null ? x.isn.Value.ToString("0,0.00####") : ""
                    });

                viewProductos.DataSource = empresas;
                viewProductos.DataBind();
            }
        }

        protected void LeerLicencia()
        {
            rutaLicencia = Server.MapPath("~/Facturacion/Licencia/");
            if (File.Exists(rutaLicencia + "licencia.vali"))
                lic = new License(rutaLicencia + "licencia.vali");
            else
                lic = new License();
        }

        protected void LlenarEmisores()
        {
            using (var db = new DataModel.OstarDB())
            {
                Dictionary<string, string> emp = new Dictionary<string, string>();

                if (User.IsInRole("Admin"))
                {
                    emp = (from h in db.emisor
                           select new
                           {
                               razon_social = h.identificador + " - " + seg.Desencriptar(h.razon_social),
                               rfc = h.idemisor.ToString()
                           }).Take(lic.ObtenerEmpresas()).ToDictionary(t => t.rfc, t => t.razon_social);
                }
                else
                {
                    emp = (from ue in db.useremisor
                           where ue.UserId == User.Identity.GetUserId()
                           select new
                           {
                               razon_social = ue.ibfk2.identificador + " - " + seg.Desencriptar(ue.ibfk2.razon_social),
                               rfc = ue.ibfk2.idemisor.ToString()
                           }).Take(lic.ObtenerEmpresas()).ToDictionary(t => t.rfc, t => t.razon_social);
                }

                Empresa.DataValueField = "Key";
                Empresa.DataTextField = "Value";
                Empresa.DataSource = emp;
                Empresa.DataBind();
            }
        }

        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (this.idProducto.Value.Split('|')[0] != "0")
                this.LimpiarCampos();
            this.idProducto.Value = 0 + "|" + 1;

          //  titleProducto.Text = "Registrar nuevo producto";
            btnProducto.CssClass = "btn btn-primary";
            btnProducto.Text = "Registrar";

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#registro').modal('show');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "RegistroModalScript", sb.ToString(), false);
        }

        protected void LimpiarCampos()
        {
            this.idProducto.Value = string.Empty;
            this.Codigo.Text = string.Empty;
          //  this.CuentaPredial.Text = string.Empty;
            this.Descripcion.Text = string.Empty;
            this.Descuento.Text = string.Empty;
            this.IEPS.Text = string.Empty;
           // this.ISH.Text = string.Empty;
          //  this.ISN.Text = string.Empty;
            this.IVA.Text = string.Empty;
            this.UnidadMedida.Text = string.Empty;
            this.ValorUnitario.Text = string.Empty;
        }

        protected void viewProductos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Editar")
                {
                    int id = Convert.ToInt32(viewProductos.DataKeys[Convert.ToInt32(e.CommandArgument)].Value);
                    LlenarProducto(id, 2);
                }
                else if (e.CommandName == "Eliminar")
                {
                    int id = Convert.ToInt32(viewProductos.DataKeys[Convert.ToInt32(e.CommandArgument)].Value);
                    LlenarProducto(id, 3);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = ex.Message;
            }
        }
        protected void LlenarProducto(int id, int idInstruccion)
        {
            string instruccion = string.Empty;
            switch (idInstruccion)
            {
                case 2://Editar producto
                    instruccion = "Editar producto: ";
                    btnProducto.CssClass = "btn btn-primary";
                    btnProducto.Text = "Editar";
                    break;
                case 3://Eliminar cliente
                    instruccion = "Eliminar producto: ";
                    btnProducto.CssClass = "btn btn-danger";
                    btnProducto.Text = "Eliminar";
                    break;
            }
            using (var db = new DataModel.OstarDB())
            {
                var query = db.producto.Where(x => x.id == id).FirstOrDefault();
                if (query != null)
                {
                   // titleProducto.Text = instruccion + query.codigo;
                    this.idProducto.Value = id + "|" + idInstruccion;
                    this.Codigo.Text = query.codigo;
                 //   this.CuentaPredial.Text = query.cuenta_predial;
                    this.Descripcion.Text = query.descripcion;
                    this.Descuento.Text = query.descuento != null ? query.descuento.Value.ToString("0.00####") : "";
                    this.IEPS.Text = query.ieps != null ? query.ieps.Value.ToString("0.00####") : "";
                  //  this.ISH.Text = query.ish != null ? query.ish.Value.ToString("0.00####") : "";
                  //  this.ISN.Text = query.isn != null ? query.isn.Value.ToString("0.00####") : "";
                    this.IVA.Text = query.iva != null ? query.iva.Value.ToString("0.00####") : "";
                    this.UnidadMedida.Text = query.unidad_medida;
                    this.ValorUnitario.Text = query.valor_unitario.ToString("0.00####");
                }

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#registro').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "RegistroModalScript", sb.ToString(), false);
            }
        }

        protected void btnProducto_Click(object sender, EventArgs e)
        {
            try
            {
                string[] instrucciones = this.idProducto.Value.Split('|');
                int id = Convert.ToInt32(instrucciones[0]);
                int idInstruccion = Convert.ToInt32(instrucciones[1]);
                switch (idInstruccion)
                {
                    case 1://Registrar usuario
                        this.RegistrarProducto();
                        break;
                    case 2://Editar usuario
                        this.EditarProducto(id);
                        break;
                    case 3://Eliminar usuario
                        this.EliminarEmpresa(id);
                        break;
                }
                this.LimpiarCampos();
                this.LlenarProductos(Convert.ToInt32(Empresa.SelectedValue));
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
        private void RegistrarProducto()
        {
            using (var db = new DataModel.OstarDB())
            {
                db.BeginTransaction();

                var value = db.producto.Insert(() => new DataModel.producto
                {
                    emisor = Convert.ToInt32(Empresa.SelectedValue),
                    codigo = this.Codigo.Text,
                    cuenta_predial = "",//this.CuentaPredial.Text,
                    descripcion = this.Descripcion.Text,
                    descuento = string.IsNullOrWhiteSpace(this.Descuento.Text) ? null : (Decimal?)Convert.ToDecimal(this.Descuento.Text),
                    ieps = string.IsNullOrWhiteSpace(this.IEPS.Text) ? null : (Decimal?)Convert.ToDecimal(this.IEPS.Text),
                    ish = 0,//string.IsNullOrWhiteSpace(this.ISH.Text) ? null : (Decimal?)Convert.ToDecimal(this.ISH.Text),
                    isn = 0,//string.IsNullOrWhiteSpace(this.ISN.Text) ? null : (Decimal?)Convert.ToDecimal(this.ISN.Text),
                    iva = 0,//string.IsNullOrWhiteSpace(this.IVA.Text) ? null : (Decimal?)Convert.ToDecimal(this.IVA.Text),
                    unidad_medida = this.UnidadMedida.Text,
                    valor_unitario = Convert.ToDecimal(this.ValorUnitario.Text)
                });

                db.CommitTransaction();
            }
        }

        private void EditarProducto(int id)
        {
            using (var db = new DataModel.OstarDB())
            {
                db.BeginTransaction();

                var value = db.producto.
                    Where(p => p.id == id)
                    .Update(p => new DataModel.producto
                {
                    codigo = this.Codigo.Text,
                    cuenta_predial = "",//this.CuentaPredial.Text,
                    descripcion = this.Descripcion.Text,
                    descuento = string.IsNullOrWhiteSpace(this.Descuento.Text) ? null : (Decimal?)Convert.ToDecimal(this.Descuento.Text),
                    ieps = string.IsNullOrWhiteSpace(this.IEPS.Text) ? null : (Decimal?)Convert.ToDecimal(this.IEPS.Text),
                    ish = 0,//string.IsNullOrWhiteSpace(this.ISH.Text) ? null : (Decimal?)Convert.ToDecimal(this.ISH.Text),
                    isn = 0,//string.IsNullOrWhiteSpace(this.ISN.Text) ? null : (Decimal?)Convert.ToDecimal(this.ISN.Text),
                    iva = string.IsNullOrWhiteSpace(this.IVA.Text) ? null : (Decimal?)Convert.ToDecimal(this.IVA.Text),
                    unidad_medida = this.UnidadMedida.Text,
                    valor_unitario = Convert.ToDecimal(this.ValorUnitario.Text)
                });

                db.CommitTransaction();
            }
        }

        protected void EliminarEmpresa(int id)
        {
            using (var db = new DataModel.OstarDB())
            {
                db.BeginTransaction();

                var query = db.producto
                    .Delete(e => e.id == id);

                db.CommitTransaction();
            }
        }

        protected void Empresa_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.LlenarProductos(Convert.ToInt32(Empresa.SelectedValue));
        }

        protected void Importar_Click(object sender, EventArgs e)
        {
            try
            {
                if (file1 == null)
                    throw new Exception("No has seleccionado un archivo valido");
                else if (file1.PostedFile == null)
                    throw new Exception("No has seleccionado un archivo valido");
                string rutaIncumplido = Server.MapPath("~/Facturacion/Excel/");
                if (!Directory.Exists(rutaIncumplido))
                {
                    Directory.CreateDirectory(rutaIncumplido);
                }
                file1.PostedFile.SaveAs(rutaIncumplido + file1.PostedFile.FileName);
                var data = (new ExcelReader()).ReadExcel(rutaIncumplido + file1.PostedFile.FileName, "Productos");


                int codigo = -1, unidad = -1, descripcion = -1, valor = -1,
                    descuento = -1, ieps = -1, iva = -1, ish = -1, isn = -1, predial = -1;

                for (int x = 0; x < data.Headers.Count; x++)
                {
                    if (data.Headers[x].Contains("codigo")) codigo = x;
                    else if (data.Headers[x].Contains("unidad")) unidad = x;
                    else if (data.Headers[x].Contains("descripcion")) descripcion = x;
                    else if (data.Headers[x].Contains("valor")) valor = x;
                    else if (data.Headers[x].Contains("descuento")) descuento = x;
                    else if (data.Headers[x].Contains("ieps")) ieps = x;
                    else if (data.Headers[x].Contains("iva")) iva = x;
                    else if (data.Headers[x].Contains("ish")) ish = x;
                    else if (data.Headers[x].Contains("isn")) isn = x;
                    else if (data.Headers[x].Contains("predial")) predial = x;
                }

                using (var db = new DataModel.OstarDB())
                {
                    foreach (List<string> row in data.DataRows)
                    {
                        try
                        {
                            DataModel.producto prod = new DataModel.producto();
                            prod.emisor = Convert.ToInt32(Empresa.SelectedValue);
                            prod.codigo = codigo != -1 ? row[codigo] : DateTime.Now.ToString("yyyyMMddddhhmmss");

                            var exist = db.producto.Where(x => x.codigo == prod.codigo && x.emisor == prod.emisor).FirstOrDefault();
                            if (exist == null)
                            {
                                prod.cuenta_predial = predial != -1 ? row[predial] : null;
                                prod.descripcion = descripcion != -1 ? row[descripcion] : null;
                                prod.descuento = descuento != -1 ? (string.IsNullOrWhiteSpace(row[descuento]) ? null : (Decimal?)Convert.ToDecimal(row[descuento])) : null;
                                prod.ieps = ieps != -1 ? (string.IsNullOrWhiteSpace(row[ieps]) ? null : (Decimal?)Convert.ToDecimal(row[ieps])) : null;
                                prod.ish = ish != -1 ? (string.IsNullOrWhiteSpace(row[ish]) ? null : (Decimal?)Convert.ToDecimal(row[ish])) : null;
                                prod.isn = isn != -1 ? (string.IsNullOrWhiteSpace(row[isn]) ? null : (Decimal?)Convert.ToDecimal(row[isn])) : null;
                                prod.iva = iva != -1 ? (string.IsNullOrWhiteSpace(row[iva]) ? null : (Decimal?)Convert.ToDecimal(row[iva])) : null;
                                prod.unidad_medida = unidad != -1 ? (string.IsNullOrWhiteSpace(row[unidad]) ? "No aplica" : row[unidad]) : "No aplica";
                                prod.valor_unitario = valor != -1 ? Convert.ToDecimal(row[valor]) : 0;
                                if (!string.IsNullOrWhiteSpace(prod.descripcion))
                                    db.Insert(prod);
                            }
                        }
                        catch { }
                    }
                }
                this.LlenarProductos(Convert.ToInt32(Empresa.SelectedValue));
                ErrorMessage.Text = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = ex.Message;
            }
        }
    }
}