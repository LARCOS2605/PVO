using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetsuiteLibrary.Clases
{

    public class BusquedaCorteCaja
    {
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public string Location_ID { get; set; }
        public string ReporteDia { get; set; }
        public string Customer { get; set; }
        public string ClaveArticulo { get; set; }
        public string Estatus { get; set; }
    }
}
