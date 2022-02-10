using System;
using System.Web;
using System.IO;
using System.Data;
using System.Collections.Generic;

namespace Facturador.GHO.Controllers
{
    public class Ticket
    {
        private DataModel.ticket ticket;
        private List<DataModel.ticket_detalle> detalles;
        private string rfcEmisor;

        public Ticket(StreamReader archivo)
        {
            try
            {
                this.ticket = new DataModel.ticket();
                this.detalles = new List<DataModel.ticket_detalle>();
                this.ObtenerDatosCFD(archivo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void ObtenerDatosCFD(StreamReader sr)
        {
            string seccion = string.Empty;
            string textoLinea = sr.ReadLine();
            try
            {
                string[] lineaSeparada = textoLinea.Split('|');
                seccion = "Cabecera";

                ticket.fecha = DateTime.ParseExact(lineaSeparada[1].Trim(), "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                rfcEmisor = lineaSeparada[2].Trim();
                ticket.tipo_cambio = Convert.ToDecimal(string.IsNullOrWhiteSpace(lineaSeparada[3].Trim()) ? "1" : lineaSeparada[3].Trim());
                ticket.moneda = lineaSeparada[4].Trim();
                ticket.forma_pago = lineaSeparada[5].Trim();
                ticket.metodo_pago = lineaSeparada[6].Trim();
                ticket.cuenta_pago = lineaSeparada[7].Trim();
                ticket.subtotal = Convert.ToDecimal(string.IsNullOrWhiteSpace(lineaSeparada[8].Trim()) ? "0" : lineaSeparada[8].Trim()  );
                ticket.descuento = Convert.ToDecimal(string.IsNullOrWhiteSpace(lineaSeparada[9].Trim()) ? "0" : lineaSeparada[9].Trim()  );
                ticket.motivo_decuento = lineaSeparada[10].Trim();
                ticket.total = Convert.ToDecimal(string.IsNullOrWhiteSpace(lineaSeparada[11].Trim()) ? "0" : lineaSeparada[11].Trim()  );
                ticket.tasa_iva = Convert.ToDecimal(string.IsNullOrWhiteSpace(lineaSeparada[12].Trim()) ? "0" : lineaSeparada[12].Trim()  );
                ticket.iva = Convert.ToDecimal(string.IsNullOrWhiteSpace(lineaSeparada[13].Trim()) ? "0" : lineaSeparada[13].Trim()  );
                ticket.tasa_ieps = Convert.ToDecimal(string.IsNullOrWhiteSpace(lineaSeparada[14].Trim()) ? "0" : lineaSeparada[14].Trim()  );
                ticket.ieps = Convert.ToDecimal(string.IsNullOrWhiteSpace(lineaSeparada[15].Trim()) ? "0" : lineaSeparada[15].Trim()  );
                //
                seccion = "Conceptos";

                while ((textoLinea = sr.ReadLine()) != null)
                {
                    lineaSeparada = textoLinea.Split('|');
                    DataModel.ticket_detalle detalle = new DataModel.ticket_detalle();
                    detalle.cantidad = Convert.ToDecimal(string.IsNullOrWhiteSpace(lineaSeparada[0].Trim()) ? "0" : lineaSeparada[0].Trim());
                    detalle.unidad = lineaSeparada[1].Trim();
                    detalle.no_identificacion = lineaSeparada[2].Trim();
                    detalle.descripcion = lineaSeparada[3].Trim();
                    detalle.valor_unitario = Convert.ToDecimal(lineaSeparada[4]);
                    detalle.importe = Convert.ToDecimal(lineaSeparada[5]);
                    this.detalles.Add(detalle);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Sección " + seccion + ": " + ex.Message);
            }
            finally
            {
                if (sr != null)
                    sr.Close();
            }
        }

        public DataModel.ticket ObtenerCabecera()
        {
            return this.ticket;
        }

        public List<DataModel.ticket_detalle> ObtenerDetalles()
        {
            return this.detalles;
        }

        public string ObtenerRFC()
        {
            return this.rfcEmisor;
        }
    }
}
