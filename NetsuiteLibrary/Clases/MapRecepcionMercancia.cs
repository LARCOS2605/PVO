using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetsuiteLibrary.Clases
{

    public class MapRecepcionMercancia
    {
        public string Internal_ID { get; set; }
        public string Nota { get; set; }
        public List<MapDetalleRecepcionMercancia> l_Detalle { get; set; }
    }
}
