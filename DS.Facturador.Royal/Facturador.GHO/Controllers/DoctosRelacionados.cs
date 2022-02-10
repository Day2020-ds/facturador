using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Facturador.GHO.Controllers
{
    public class DoctosRelacionados
    {
        public string parcialidad { get; set; }
        public string uuid { get; set; }
        public string serie { get; set; }
        public string folio { get; set; }
        public string moneda { get; set; }
        public decimal total { get; set; }
        public decimal saldoAnterior { get; set; }
        public decimal importePagado { get; set; }
        public decimal tipo_cambio { get; set; }
        public DateTime fechaTimbrado { get; set; }
    }
}