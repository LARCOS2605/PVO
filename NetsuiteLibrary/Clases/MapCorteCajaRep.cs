using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetsuiteLibrary.Clases
{

    public class MapCorteCajaRep
    {
        public string ClaveCliente { get; set; }
        public string NombreCliente { get; set; }
        public string Credito { get; set; }
        public string FormaPago { get; set; }
        public string FolioFactura { get; set; }
        public string NoFactura { get; set; }
        public double SubtotalFactura { get; set; }
        public double IvaFactura { get; set; }
        public double TotalFactura { get; set; }
        public string Descripcion_MetodoPago { get; set; }
        public DateTime Fecha { get; set; }
        public string ExistePVO { get; set; }
        public string Usuario { get; set; }
        public string NS_ID_inv { get; set; }
        public string NS_ID_OS { get; set; }
        public string Fechas { get; set; }

    }
}
