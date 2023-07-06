using NetsuiteLibrary.SuiteTalk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetsuiteLibrary.Clases
{

    public class DatosSat
    {
        public string UUID { get; set; }
        public string NS_XML { get; set; }
        public string NS_PDF { get; set; }
        public string Serie { get; set; }
        public string Folio { get; set; }
        public string FechaTimbrado { get; set; }
        public string Estatus { get; set; }


        public string NS_InternalID { get; set; }
        public string NS_ExternalID { get; set; }
    }
}
