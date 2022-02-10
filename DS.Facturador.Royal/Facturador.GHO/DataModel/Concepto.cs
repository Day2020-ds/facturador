using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Facturador.GHO.Model
{
    public class Concepto
    {
        public string claveCargo { get; set; }
        public decimal cantidad { get; set; }
        public string unidadMedida { get; set; }
        public decimal precioUnitario { get; set; }
        public decimal importe { get; set; }
        public string cuentaPredial { get; set; }
        public string descripcion { get; set; }
        public decimal? descuento { get; set; }
        public decimal? iva { get; set; }
        public decimal? ieps { get; set; }
        public decimal? ish { get; set; }
        public DateTime? fechaPago { get; set; }
        public decimal? tipoCambio { get; set; }
    }
}