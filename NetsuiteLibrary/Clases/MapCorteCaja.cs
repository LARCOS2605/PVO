using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetsuiteLibrary.Clases
{

    public class MapCorteCaja
    {
        public string ClaveCliente { get; set; }
        public string NombreCliente { get; set; }
        public string FolioPago { get; set; }
        public string DiasCredito { get; set; }
        public string FolioFactura { get; set; }
        public double MontoPago { get; set; }
        public string MetodoPago { get; set; }
        public string CuentaBancaria { get; set; }
        public double MontoFactura { get; set; }
        public string metodopagoFactura { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string fecha { get; set; }
        public string fechavenc { get; set; }
        public string fechacre { get; set; }
    }
}
