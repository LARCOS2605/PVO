using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetsuiteLibrary.Clases
{
    //Datos Cabecero FullFillment
    class ItemList_Records
    {
        public string referenceRecId { get; set; }
        public string memo { get; set; }
        public string shipstatus { get; set; }
        public List<ItemList_Model> l_Items { get; set; }
    }

    //Datos Items
    class ItemList_Model
    {
        public string Item { get; set; }
        public string Quantity { get; set; }
        public List<ItemInventoryDetail> l_Detail { get; set; }
    }

    //Datos Detalle Items (List of Lists)
    class ItemInventoryDetail
    {
        public string issueinventorynumber { get; set; }
        public string quantity { get; set; }
    }
}
