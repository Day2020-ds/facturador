using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using BLToolkit.Data;
using BLToolkit.Data.Linq;
using Facturador.GHO.Controllers;

namespace Facturador.GHO.Admin
{
    /// <summary>
    /// Descripción breve de UploadTicket
    /// </summary>
    public class UploadTicket : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string filename = string.Empty;
            try
            {
                HttpFileCollection files = context.Request.Files;

                string savepath = "";
                string tempPath = "";
                tempPath = System.Configuration.ConfigurationManager.AppSettings["rutaComprobantes"];
                savepath = context.Server.MapPath(tempPath);
                foreach (string key in files)
                {
                    HttpPostedFile postedFile = files[key];
                    filename = postedFile.FileName;
                    //if (!Directory.Exists(savepath))
                    //    Directory.CreateDirectory(savepath);
                    //postedFile.SaveAs(savepath + @"\" + filename);
                    if (filename.ToLower().EndsWith(".txt"))
                    {
                        StreamReader sr = new StreamReader(postedFile.InputStream);
                        GuardarDB(sr);
                    }

                    context.Response.ContentType = "text/plain";
                    var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    var result = new { name = postedFile.FileName };
                    context.Response.Write(serializer.Serialize(result));
                }
            }
            catch (Exception ex)
            {
                string path = HttpContext.Current.Server.MapPath("~/Error/");
                RegistrarError(filename, ex, path);
                context.Response.Write("Error: " + ex.Message);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
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
                    else
                        throw new Exception("Ya existe el ticket");
                }

                db.CommitTransaction();
            }
        }

        protected static void RegistrarError(string id, Exception ex, string rutaAplicacion)
        {
            {
                StreamWriter wr = null;
                string error = null;
                try
                {
                    if (!Directory.Exists(rutaAplicacion))
                        Directory.CreateDirectory(rutaAplicacion);
                    string nombreLog = String.Format("ErrorLog_{0}.txt", DateTime.Now.ToString("yyyyMM"));
                    error = String.Format("{0}|ID:{1}; {2}", DateTime.Now.ToString(), id, ex.Message);
                    wr = new StreamWriter(Path.Combine(rutaAplicacion, nombreLog), true);
                    wr.WriteLine(error);
                }
                catch (Exception) { }
                finally
                {
                    if (wr != null)
                    {
                        wr.Close();
                        wr.Dispose();
                    }
                }
            }
        }
    }
}