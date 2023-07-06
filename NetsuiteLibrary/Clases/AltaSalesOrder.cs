using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetsuiteLibrary.Clases
{
    //Datos Cabecero FullFillment
    public class AltaSalesOrder
    {
        public string InternalID_Customer { get; set; }
        public string InternalID_MetodoPagoSAT { get; set; }
        public string InternalID_FormaPagoSAT { get; set; }
        public string InternalID_UsoCFDI { get; set; }
        public string Ubicacion_Venta { get; set; }
        public DateTime Fecha_Creacion { get; set; }
        public string location { get; set; }
        public string memo { get; set; }
        public double EstatusDeEjecucion { get; set; }
        public string CantidadProductos { get; set; }
        public string Estatus { get; set; }
        public List<DetalleAltaSalesOrder> DetalleInventario { get; set; }
        public string Exitoso { get; set; }
    }

    public class DetalleAltaSalesOrder
    {
        public double Cantidad { get; set; }
        public string IssuesInventoryID { get; set; }
        public string idProducto { get; set; }
        public string InternalID { get; set; }
        public string UbicacionInventario { get; set; }
        public string NumLote { get; set; }
        public string TipoMaterial { get; set; }
        public string PrecioU { get; set; }
        public string Total { get; set; }
    }
}
