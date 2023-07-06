using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetsuiteLibrary.Clases
{

    public class Consulta_Pagos
    {
        public string ClaveCliente { get; set; }
        public string NombreCliente { get; set; }
        public string FolioPago { get; set; }
        public string NS_InternalID { get; set; }
        public string Nombre_Cliente { get; set; }
        public double MontoPago { get; set; }
        public string MetodoPago { get; set; }
        public string UUID { get; set; }
        public DateTime FechaEmision { get; set; }
        public string fecha { get; set; }
        public string Timbrado { get; set; }
    }
}
