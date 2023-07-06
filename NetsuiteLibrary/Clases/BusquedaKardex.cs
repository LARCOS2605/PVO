using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetsuiteLibrary.Clases
{

    public class BusquedaKardex
    {
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public string Location_ID { get; set; }
        public string Articulo { get; set; }
        public string Clasificacion_Articulo { get; set; }
    }
}
