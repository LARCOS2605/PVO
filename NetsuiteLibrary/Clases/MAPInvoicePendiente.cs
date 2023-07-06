using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetsuiteLibrary.Clases
{

    public class MAPInvoicePendiente
    {        
        public string InternalID { get; set; }
        public string NS_ExternalID { get; set; }
        public DateTime Fecha { get; set; }

        public string ClaveCliente { get; set; }
        public string NombreCliente { get; set; }
        public string Credito { get; set; }
        public string FormaPago { get; set; }
        public string FolioFactura { get; set; }
        public string NoFactura { get; set; }
        public double SubtotalFactura { get; set; }
        public double IvaFactura { get; set; }
        public double TotalFactura { get; set; }
        public double SaldoPendiente { get; set; }
        public double SaldoPagado { get; set; }
        public string Descripcion_MetodoPago { get; set; }   
        public string ExistePVO { get; set; }


    }
}
