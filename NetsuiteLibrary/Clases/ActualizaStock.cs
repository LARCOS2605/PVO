using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetsuiteLibrary.Clases
{
    //Datos Cabecero FullFillment
    public class ActualizaStock
    {
        public string InventoryID { get; set; }
        public string IssuesID { get; set; }
        public string item { get; set; }
        public string location { get; set; }
        public double quantityavailable { get; set; }
        public string Estatus { get; set; }
        public double Precio { get; set; }
        public double PrecioLista { get; set; }
        public double StockPlanta { get; set; }
        public double StockMuelles { get; set; }
        public string DescripcionPrecio { get; set; }
    }
}
