using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetsuiteLibrary.Clases
{
    public class AltaTransferOrder
    {
        public string InternalID_Mostrador_Origen { get; set; }
        public string InternalID_Mostrador_Destino { get; set; }
        public string InternalID_Incoterm { get; set; }
        public string InternalID_Clase { get; set; }
        public string InternalID_Estatus { get; set; }
        public List<DetalleTransferOrder> DetalleTransfer { get; set; }
    }

    public class DetalleTransferOrder
    {
        public double Cantidad { get; set; }
        public string IssuesInventoryID { get; set; }
        public string idProducto { get; set; }
        public string InternalID { get; set; }
        public string NumLote { get; set; }
        public string TipoMaterial { get; set; }
        public DateTime FechaPrevistaEnvio { get; set; }
        public DateTime FechaPrevistaRecepcion { get; set; }
    }
}
