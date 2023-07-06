using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetsuiteLibrary.Clases
{

    public class MapNegadas
    {
        public string ClaveCliente { get; set; }
        public string NombreCliente { get; set; }
        public string NS_ExternalID { get; set; }
        public string NS_InternalID { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Fecha { get; set; }
        public string Mostrador { get; set; }
        public string Factura { get; set; }
        public string Estatus { get; set; }
        public double Total_Factura { get; set; }
        public string NS_InternalID_Factura { get; set; }

    }
}
