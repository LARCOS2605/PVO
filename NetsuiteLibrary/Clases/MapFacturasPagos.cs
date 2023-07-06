using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetsuiteLibrary.Clases
{

    public class MapFacturasPagos
    {
        public string FolioPago { get; set; }
        public double MontoPago { get; set; }
        public string MetodoPago { get; set; }
        public string CuentaBancaria { get; set; }
        public string InternalID { get; set; }
        public DateTime FechaEmision { get; set; }
        public string fecha { get; set; }
    }
}
