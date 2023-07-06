using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetsuiteLibrary.SuiteTalk_Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.VisualBasic;
    using NetsuiteLibrary.SuiteTalk;

    public class InventaryHelper
    {
        public List<InventoryItem> GetAllInventary()
        {
            NetSuiteService service;
            ItemSearch ItemsSearch = new ItemSearch();
            ItemSearchBasic ItemsSearchBasic = new ItemSearchBasic();
            SearchEnumMultiSelectField Inventorystage = new SearchEnumMultiSelectField();
            List<InventoryItem> l_InventoryList = new List<InventoryItem>();

            Inventorystage.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            Inventorystage.operatorSpecified = true;
            Inventorystage.searchValue = new string[] { "_inventoryItem" };

            //ItemsSearchBasic.stage = Inventorystage;
            ItemsSearch.basic = ItemsSearchBasic;

            service = ConnectionManager.GetNetSuiteService();
            SearchResult searchResult = service.search(ItemsSearch);

            if (searchResult.recordList != null && searchResult.recordList.Length > 0)
            {
                for (int i = 0; i < searchResult.recordList.Length; i++)
                {
                    if (searchResult.recordList[i] is InventoryItem)
                    {
                        InventoryItem objItem = (InventoryItem) searchResult.recordList[i];
                        l_InventoryList.Add(objItem);
                    }
                }
            }

            return l_InventoryList;
        }

    }

}
