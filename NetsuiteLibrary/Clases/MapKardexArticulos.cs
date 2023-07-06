using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetsuiteLibrary.Clases
{

    public class MapKardexArticulos
    {
        public DateTime Fecha { get; set; }
        public string Periodo { get; set; }
        public string TipoTransaccion { get; set; }
        public string Articulo { get; set; }
        public string Descripcion { get; set; }
        public string FolioDocumento { get; set; }
        public string Nombre { get; set; }
        public string Nota { get; set; }
        public double Importe { get; set; }
        public double Cantidad { get; set; }
        public string linea_principal { get; set; }
        public string ubicacion { get; set; }
    }

}
