using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetsuiteLibrary.Clases
{

    public class MapFacturasPendientesPago
    {
        public string NS_InternalID { get; set; }
        public string NS_ExternalID { get; set; }
        public double Importe_Adeudado { get; set; }
        public string Fecha { get; set; }
        public string Fecha_Venci { get; set; }
        public DateTime Fecha_Vencimiento { get; set; }
        public DateTime FechaCreated { get; set; }
        public string MetodoPagoSAT { get; set; }
        public double Total { get; set; }
    }
}
