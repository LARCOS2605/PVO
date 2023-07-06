using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetsuiteLibrary.Clases
{
    //Datos Alta del Cliente
    public class AltaCustomer
    {
        public string Nombre_Empresa { get; set; } // CompanyName
        public string Rfc { get; set; } //custentity_mx_rfc
        public string Nombre { get; set; } // FirstName
        public string Apellido { get; set; }
        public string Nombre_Sat { get; set; } // custentity_mx_sat_registered_name
        public string InternalID_RegimenFiscal { get; set; } // InternalID_RegimenFiscal
        public string Correo { get; set; } //Email
        public string PaisDom { get; set; }
        public string EstadoDom { get; set; }
        public string MunicipioDom { get; set; }
        public string ColoniaDom { get; set; }
        public string CalleDom { get; set; }
        public string NumExtDom { get; set; }
        public string CpDom { get; set; }
        public string InternalId_Mostrador { get; set; } //custentity_maf_most_asig
        public string Comentarios { get; set; } //comments
        public string PaisFis { get; set; }
        public string Atencion { get; set; }
        public string Destinatario { get; set; }
        public string CalleFis { get; set; }
        public string NumExtFis { get; set; }
        public string Piso { get; set; }
        public string NumIntFis { get; set; }
        public string ColoniaFis { get; set; }
        public string MunicipioFis { get; set; }
        public string CpFis { get; set; }
        public string CiudadFis { get; set; }
        public string Internalid_estado { get; set; }
        public string EstadoFis { get; set; }
        public string Direc { get; set; }
        public string PersonaFisica { get; set; }
        public string TipoPago { get; set; }
        public string InternalidPago { get; set; }
        public string Telefono { get; set; }
        public string Celular { get; set; }

    }
}
