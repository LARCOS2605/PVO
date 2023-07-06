using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetsuiteLibrary.Clases
{

    public class MapClientes
    {
        public string InternalID { get; set; }
        public string ExternalID { get; set; }
        public string Nombre { get; set; }
        public string RFC { get; set; }
        public string Nivel_Precios { get; set; }
        public string Terminos { get; set; }
        public string DiasVencimiento { get; set; }
        public string Estado { get; set; }
        public double limite_Credito { get; set; }
    }

    public class MapLocationVB
    {
        public string InternalID { get; set; }
        public string Nombre { get; set; }
    }
}
