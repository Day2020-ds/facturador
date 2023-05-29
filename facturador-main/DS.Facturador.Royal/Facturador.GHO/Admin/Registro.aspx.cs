using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using BLToolkit.Data;
using BLToolkit.Data.Linq;
using Facturador.GHO.Controllers;

namespace Facturador.GHO.Admin
{
    public partial class Registro : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Procesar(object sender, EventArgs e)
        {
            //if (FileRecibo.HasFiles)
            //{
            //    foreach (HttpPostedFile file in FileRecibo.PostedFiles)
            //    {
            //        if (file.FileName.ToLower().EndsWith(".txt"))
            //        {
            //            StreamReader sr = new StreamReader(file.InputStream);
            //            GuardarDB(sr);
            //        }
            //    }
            //}
        }

        protected void GuardarDB(StreamReader archivo)
        {
            Ticket ticket = new Ticket(archivo);
            DataModel.ticket cabecera = ticket.ObtenerCabecera();
            List<DataModel.ticket_detalle> detalles = ticket.ObtenerDetalles();

            using (var db = new DataModel.OstarDB())
            {
                db.BeginTransaction();

                var emisor = (from e in db.emisor
                              where e.rfc == ticket.ObtenerRFC()
                              select e).FirstOrDefault();

                if (emisor != null)
                {
                    var existeTicket = db.ticket
                        .Where(t => t.emisor == emisor.idemisor && t.serie == cabecera.serie && t.folio == cabecera.folio && t.total == cabecera.total).Count();

                    if (existeTicket <= 0)
                    {
                        cabecera.emisor = emisor.idemisor;
                        var idTicket = db.InsertWithIdentity(cabecera);

                        foreach (DataModel.ticket_detalle detalle in detalles)
                        {
                            detalle.ticket = Convert.ToInt32(idTicket);
                            var query = db.InsertWithIdentity(detalle);
                        }
                    }
                }

                db.CommitTransaction();
            }
        }
    }
}