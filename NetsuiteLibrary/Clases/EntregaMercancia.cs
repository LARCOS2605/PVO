using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetsuiteLibrary.Clases
{
    //Datos Cabecero FullFillment
    public class EntregaMercancia
    {
        public string NS_InternalID_Doc { get; set; }
        public string Memo { get; set; }
        public List<DetalleEntregaMercancia> l_Detalle {get; set;}
    }

    public class DetalleEntregaMercancia
    {
        public double NS_InternalID_Mercancia { get; set; }
        public string Cantidad { get; set; }
        public string IssuesInventoryID { get; set; }
        public string TipoMaterial { get; set; }
    }
}
