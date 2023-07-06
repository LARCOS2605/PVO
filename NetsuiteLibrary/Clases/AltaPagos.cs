using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetsuiteLibrary.Clases
{
    //Datos Cabecero FullFillment
    public class AltaPagos
    {
        public string InternalID_Customer { get; set; }
        public string InternalID_Pago { get; set; }
        public string InternalID_MetodoPago { get; set; }
        public string Localizacion { get; set; }
        public string memo { get; set; }
        public DateTime FechaPago { get; set; }
        public string Nombre_Banco { get; set; }
        public string Cuenta_Banco { get; set; }
        public string MetodoPagoSAT { get; set; }
        public string Terminal { get; set; }
        public string IndicadorPlantilla { get; set; }
        //Transferencia Electronica
        public string RFCBanco_Beneficiario { get; set; }
        public string NumCuenta_Beneficiario { get; set; }
        public string NS_ID_BancoBeneficiario { get; set; }
        public string RFCBanco_Ordenante { get; set; }
        public string NumCuenta_Ordenante { get; set; }
        public string NS_ID_BancoOrdenante { get; set; }
        public string NumTransaccion { get; set; }
        public string TipoMetodoPago { get; set; }
        public List<DetalleAltaPagos> DetallePagos { get; set; }
    }

    public class DetalleAltaPagos
    {
        public string InternalID_Factura { get; set; }
        public double Monto { get; set; }
        public string TipoPago { get; set; }
    }
}
