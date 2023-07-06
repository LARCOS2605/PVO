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

    public class ItemHelper
    {
        public InventoryItem GetItemByID(string id)
        {
            NetSuiteService service;
            InventoryItem itemVenta;

            RecordRef recordRef = new RecordRef();
            recordRef.type = RecordType.inventoryItem;
            recordRef.typeSpecified = true;
            recordRef.internalId = id;

            service = ConnectionManager.GetNetSuiteService();
            itemVenta = (InventoryItem)service.get(recordRef).record;

            return itemVenta;
        }

        public Customer GetItemByName(string customerName)
        {
            // NetSuite Service class
            NetSuiteService service = new NetSuiteService();

            // Search results
            SearchResult searchResults;

            Customer customer = new Customer();

            // Authenticate and get the NetSuite service
            service = ConnectionManager.GetNetSuiteService();

            // Subsidary search object
            CustomerSearch customerSearch = new CustomerSearch();
            CustomerSearchBasic customerSearchBasic = new CustomerSearchBasic();
            SearchStringField customerNameField = new SearchStringField();

            // Filter customer search by name
            customerNameField.@operator = SearchStringFieldOperator.contains;
            customerNameField.operatorSpecified = true;
            customerNameField.searchValue = customerName;
            customerSearchBasic.entityId = customerNameField;

            customerSearch.basic = customerSearchBasic;

            // Perform the search
            searchResults = service.search(customerSearch);

            customer = (Customer)searchResults.recordList.FirstOrDefault();

            return customer;
        }

        //public InventoryItem GetItemByID(string id)
        //{
        //    NetSuiteService service;
        //    InventoryItem itemVenta;

        //    RecordRef recordRef = new RecordRef();
        //    recordRef.type = RecordType.inventoryItem;
        //    recordRef.typeSpecified = true;
        //    recordRef.internalId = id;

        //    service = ConnectionManager.GetNetSuiteService();
        //    itemVenta = (InventoryItem)service.get(recordRef).record;

        //    return itemVenta;
        //}

        public AssemblyItem GetItemAssemblyitemsByID(string id)
        {
            NetSuiteService service;
            AssemblyItem itemVenta;

            RecordRef recordRef = new RecordRef();
            recordRef.type = RecordType.assemblyItem;
            recordRef.typeSpecified = true;
            recordRef.internalId = id;

            service = ConnectionManager.GetNetSuiteService();
            itemVenta = (AssemblyItem) service.get(recordRef).record;

            return itemVenta;
        }

        public ServiceResaleItem GetItemServiceByID(string id)
        {
            NetSuiteService service;
            ServiceResaleItem itemVenta;
            
            RecordRef recordRef = new RecordRef();
            recordRef.type = RecordType.serviceResaleItem;
            recordRef.typeSpecified = true;
            recordRef.internalId = id;

            service = ConnectionManager.GetNetSuiteService();
            itemVenta = (ServiceResaleItem) service.get(recordRef).record;

            return itemVenta;
        }

        public LotNumberedAssemblyItem GetLotNumberedAssemblyItemByID(string id)
        {
            NetSuiteService service;
            LotNumberedAssemblyItem itemVenta;

            RecordRef recordRef = new RecordRef();
            recordRef.type = RecordType.lotNumberedAssemblyItem;
            recordRef.typeSpecified = true;
            recordRef.internalId = id;

            service = ConnectionManager.GetNetSuiteService();
            itemVenta = (LotNumberedAssemblyItem)service.get(recordRef).record;

            //itemVenta.pricingMatrix.pricing

            return itemVenta;
        }

        public List<LotNumberedAssemblyItem> GetAllItemsLotNumberedAssemblyItem()
        {
            NetSuiteService service;
            List<LotNumberedAssemblyItem> l_AssemblyList = new List<LotNumberedAssemblyItem>();

            ItemSearchBasic myItemSearchBasic = new ItemSearchBasic();
            SearchBooleanField basics = new SearchBooleanField();

            basics.searchValue = true;
            basics.searchValueSpecified = true;

            SearchEnumMultiSelectField myEnum = new SearchEnumMultiSelectField();
            myEnum.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            myEnum.operatorSpecified = true;
            String[] searchStringArray = new String[1];
            searchStringArray[0] = "_assembly";
            myEnum.searchValue = searchStringArray;
            myItemSearchBasic.type = myEnum;
            myItemSearchBasic.isLotItem = basics;

            ItemSearch myItemSearch = new ItemSearch();
            myItemSearch.basic = myItemSearchBasic;

            service = ConnectionManager.GetNetSuiteService();
            SearchResult searchResult = service.search(myItemSearch);

            var tamaño = searchResult.totalPages;

            for (int n = 1; n < tamaño; n++) {

                service = ConnectionManager.GetNetSuiteService();
                searchResult = service.searchMoreWithId(searchResult.searchId,n);

                if (searchResult.recordList != null && searchResult.recordList.Length > 0)
            {
                for (int i = 0; i < searchResult.recordList.Length; i++)
                {
                    if (searchResult.recordList[i] is LotNumberedAssemblyItem)
                    {

                        LotNumberedAssemblyItem objItem = (LotNumberedAssemblyItem)searchResult.recordList[i];
                        l_AssemblyList.Add(objItem);
                    }
                }
            }

            }

            return l_AssemblyList;
        }

        public List<AssemblyItem> GetAllItemsAssemblyItem()
        {
            NetSuiteService service;
            List<AssemblyItem> l_AssemblyList = new List<AssemblyItem>();

            ItemSearchBasic myItemSearchBasic = new ItemSearchBasic();
            SearchBooleanField basics = new SearchBooleanField();

            basics.searchValue = false;
            basics.searchValueSpecified = true;

            SearchEnumMultiSelectField myEnum = new SearchEnumMultiSelectField();
            myEnum.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            myEnum.operatorSpecified = true;
            String[] searchStringArray = new String[1];
            searchStringArray[0] = "_assembly";
            myEnum.searchValue = searchStringArray;
            myItemSearchBasic.type = myEnum;
            myItemSearchBasic.isLotItem = basics;
            //myItemSearchBasic.buildEntireAssembly = basics;
            //myItemSearchBasic.isSerialItem = basics;
            

            ItemSearch myItemSearch = new ItemSearch();
            myItemSearch.basic = myItemSearchBasic;

            service = ConnectionManager.GetNetSuiteService();
            SearchResult searchResult = service.search(myItemSearch);

            var tamaño = searchResult.totalPages;

            if (tamaño == 1)
            {
                if (searchResult.recordList != null && searchResult.recordList.Length > 0)
                {
                    for (int i = 0; i < searchResult.recordList.Length; i++)
                    {

                        if (searchResult.recordList[i] is AssemblyItem)
                        {

                            AssemblyItem objItem = (AssemblyItem)searchResult.recordList[i];

                            l_AssemblyList.Add(objItem);
                        }
                    }
                }
            }
            else
            {
                for (int n = 1; n <= tamaño; n++)
                {

                    service = ConnectionManager.GetNetSuiteService();
                    searchResult = service.searchMoreWithId(searchResult.searchId, n);

                    if (searchResult.recordList != null && searchResult.recordList.Length > 0)
                    {
                        for (int i = 0; i < searchResult.recordList.Length; i++)
                        {
                            try { 
                                if (searchResult.recordList[i] is AssemblyItem){

                                AssemblyItem objItem = (AssemblyItem)searchResult.recordList[i];

                                l_AssemblyList.Add(objItem);
                                 }
                            } catch(Exception ex)
                            {
                                continue;
                            }

                        }
                    }

                }
            }


            return l_AssemblyList;
        }

        public LotNumberedInventoryItem GetLotNumberedInventoryItemByID(string id)
        {
            NetSuiteService service;
            LotNumberedInventoryItem itemVenta;

            RecordRef recordRef = new RecordRef();
            recordRef.type = RecordType.lotNumberedInventoryItem;
            recordRef.typeSpecified = true;
            recordRef.internalId = id;

            service = ConnectionManager.GetNetSuiteService();
            itemVenta = (LotNumberedInventoryItem) service.get(recordRef).record;

            return itemVenta;
        }

        public InventoryItem GetInventoryItemByID(string id)
        {
            NetSuiteService service;
            InventoryItem itemVenta;

            RecordRef recordRef = new RecordRef();
            recordRef.type = RecordType.inventoryItem;
            recordRef.typeSpecified = true;
            recordRef.internalId = id;

            service = ConnectionManager.GetNetSuiteService();
            itemVenta = (InventoryItem) service.get(recordRef).record;

            return itemVenta;
        }

        public List<InventoryItem> GetAllInventoryItems()
        {
            NetSuiteService service;
            List<InventoryItem> l_InventoryList = new List<InventoryItem>();

            ItemSearchBasic myItemSearchBasic = new ItemSearchBasic();
            SearchBooleanField basics = new SearchBooleanField();

            basics.searchValue = true;
            basics.searchValueSpecified = true;

            SearchEnumMultiSelectField myEnum = new SearchEnumMultiSelectField();
            myEnum.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            myEnum.operatorSpecified = true;
            String[] searchStringArray = new String[1];
            searchStringArray[0] = "inventoryItem";
            myEnum.searchValue = searchStringArray;
            myItemSearchBasic.type = myEnum;
            //myItemSearchBasic.isLotItem = basics;

            ItemSearch myItemSearch = new ItemSearch();
            myItemSearch.basic = myItemSearchBasic;

            service = ConnectionManager.GetNetSuiteService();
            SearchResult searchResult = service.search(myItemSearch);

            var tamaño = searchResult.totalPages;

            for (int n = 1; n <= tamaño; n++)
            {

                service = ConnectionManager.GetNetSuiteService();
                searchResult = service.searchMoreWithId(searchResult.searchId, n);

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

            }

            return l_InventoryList;
        }

        public List<ServiceResaleItem> GetAllServicesItems()
        {
            NetSuiteService service;
            List<ServiceResaleItem> l_InventoryList = new List<ServiceResaleItem>();

            ItemSearchBasic myItemSearchBasic = new ItemSearchBasic();
            SearchBooleanField basics = new SearchBooleanField();

            basics.searchValue = true;
            basics.searchValueSpecified = true;

            SearchEnumMultiSelectField myEnum = new SearchEnumMultiSelectField();
            myEnum.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            myEnum.operatorSpecified = true;
            String[] searchStringArray = new String[1];
            searchStringArray[0] = "Service";
            myEnum.searchValue = searchStringArray;
            myItemSearchBasic.type = myEnum;
            //myItemSearchBasic.isLotItem = basics;

            ItemSearch myItemSearch = new ItemSearch();
            myItemSearch.basic = myItemSearchBasic;

            service = ConnectionManager.GetNetSuiteService();
            SearchResult searchResult = service.search(myItemSearch);

            var tamaño = searchResult.totalPages;

            for (int n = 1; n <= tamaño; n++)
            {

                service = ConnectionManager.GetNetSuiteService();
                searchResult = service.searchMoreWithId(searchResult.searchId, n);

                if (searchResult.recordList != null && searchResult.recordList.Length > 0)
                {
                    for (int i = 0; i < searchResult.recordList.Length; i++)
                    {
                        if (searchResult.recordList[i] is ServiceResaleItem)
                        {
                            ServiceResaleItem objItem = (ServiceResaleItem)searchResult.recordList[i];

                            //if (objItem. == "")
                            //{

                            //}

                            l_InventoryList.Add(objItem);
                        }
                    }
                }

            }

            return l_InventoryList;
        }


    }
}
