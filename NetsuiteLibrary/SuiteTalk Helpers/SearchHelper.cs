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
    using NetsuiteLibrary.Clases;
    using NetsuiteLibrary.SuiteTalk;

    public class SearchHelper
    {

        public RespuestaServicios GetListLotAvaible(string id)
        {
            try
            {
                RespuestaServicios resp = new RespuestaServicios();

                NetSuiteService service;
                List<ActualizaStock> l_RegistrarStock = new List<ActualizaStock>();

                InventoryNumberSearchAdvanced searchInventory = new InventoryNumberSearchAdvanced();
                searchInventory.savedSearchId = id;

                service = ConnectionManager.GetNetSuiteService();
                SearchResult result = service.search(searchInventory);

                if (result.status.isSuccess)
                {
                    var tamaño = result.totalPages;

                    for (int n = 1; n <= tamaño; n++)
                    {

                        service = ConnectionManager.GetNetSuiteService();
                        result = service.searchMoreWithId(result.searchId, n);

                        if (result.searchRowList != null && result.searchRowList.Length > 0)
                        {
                            for (int i = 0; i <= result.searchRowList.Length; i++)
                            {
                                ActualizaStock registrarStock = new ActualizaStock();
                                InventoryNumberSearchRow resultados = (InventoryNumberSearchRow)result.searchRowList[i];

                                registrarStock.IssuesID = resultados.basic.internalId[0].searchValue.internalId;
                                registrarStock.InventoryID = resultados.basic.inventoryNumber[0].searchValue;
                                registrarStock.quantityavailable = resultados.basic.quantityavailable[0].searchValue;
                                registrarStock.item = resultados.basic.item[0].searchValue.internalId;
                                registrarStock.location = resultados.basic.location[0].searchValue.internalId;

                                l_RegistrarStock.Add(registrarStock);
                            }
                        }

                    }

                    resp.Estatus = "Exito";
                    resp.Valor = l_RegistrarStock;
                    return resp;
                }
                else
                {

                    resp.Estatus = "Fracaso";
                    return resp;
                }

            }
            catch (Exception ex)
            {
                RespuestaServicios resp = new RespuestaServicios();
                resp.Estatus = "Fracaso";
                return resp;
            }

        }

        public RespuestaServicios GetListPaymentForLocation(BusquedaCorteCaja model)
        {
            NetSuiteService service;
            RespuestaServicios resp = new RespuestaServicios();

            LocationSearchBasic location = new LocationSearchBasic();
            List<MapCorteCaja> l_RegistrarCorteCaja = new List<MapCorteCaja>();

            RecordRef LocationRef = new RecordRef();
            LocationRef.type = RecordType.location;
            LocationRef.internalId = model.Location_ID;
            LocationRef.typeSpecified = true;

            //TransactionSearchBasic Invoices = new TransactionSearchBasic();
            //SearchEnumMultiSelectField InvoicessearchMultiSelectField = new SearchEnumMultiSelectField();
            //InvoicessearchMultiSelectField.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            //InvoicessearchMultiSelectField.operatorSpecified = true;
            //InvoicessearchMultiSelectField.searchValue = new string[] { "_invoice" };
            //Invoices.type = InvoicessearchMultiSelectField;

            //RecordRef InvoiceRef = new RecordRef();
            //InvoiceRef.type = RecordType.invoice;
            //InvoiceRef.typeSpecified = true;
            //RecordRef[] custRecordINvRefArray = new RecordRef[1];
            //custRecordINvRefArray[0] = InvoiceRef;
            //SearchMultiSelectField searchInvoice = new SearchMultiSelectField();
            //searchInvoice.@operator = SearchMultiSelectFieldOperator.anyOf;
            //searchInvoice.operatorSpecified = true;
            //searchInvoice.searchValue = custRecordINvRefArray;

            RecordRef[] custRecordRefArray = new RecordRef[1];
            custRecordRefArray[0] = LocationRef;
            SearchMultiSelectField searchCustomer = new SearchMultiSelectField();
            searchCustomer.@operator = SearchMultiSelectFieldOperator.anyOf;
            searchCustomer.operatorSpecified = true;
            searchCustomer.searchValue = custRecordRefArray;
            location.internalId = searchCustomer;

            SearchDateField searchDate = new SearchDateField();
            searchDate.@operator = SearchDateFieldOperator.within;
            searchDate.searchValue = model.FechaDesde;
            searchDate.searchValue2 = model.FechaHasta;
            searchDate.operatorSpecified = true;
            searchDate.searchValueSpecified = true;
            searchDate.searchValue2Specified = true;

            TransactionSearchBasic Transaction = new TransactionSearchBasic();
            SearchEnumMultiSelectField searchMultiSelectField = new SearchEnumMultiSelectField();
            searchMultiSelectField.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            searchMultiSelectField.operatorSpecified = true;
            searchMultiSelectField.searchValue = new string[] { "customerPayment" };
            Transaction.type = searchMultiSelectField;
            Transaction.dateCreated = searchDate;
            //Transaction.
            //Transaction.amountPaid = 
            //Transaction.paymentApproved
            //Transaction.appliedToTransaction = searchInvoice;
            //Transaction.invo

            TransactionSearch BusquedaTransaccion = new TransactionSearch();
            BusquedaTransaccion.basic = Transaction;
            //BusquedaTransaccion.appliedToTransactionJoin = Invoices;
            BusquedaTransaccion.locationJoin = location;

            service = ConnectionManager.GetNetSuiteService();

            SearchResult searchResult = service.search(BusquedaTransaccion);

            if (searchResult.status.isSuccess)
            {
                Record[] searchRows = searchResult.recordList;
                if (searchRows != null && searchRows.Length >= 1)
                {

                    foreach (CustomerPayment rows_Inventory in searchRows)
                    {

                        if (rows_Inventory != null)
                        {
                            MapCorteCaja RegistrarCorteCaja = new MapCorteCaja();
                            RegistrarCorteCaja.NombreCliente = rows_Inventory.customer.name;
                            RegistrarCorteCaja.FolioPago = rows_Inventory.tranId;
                            RegistrarCorteCaja.MontoPago = rows_Inventory.applied;

                            if (rows_Inventory.paymentMethod is null)
                            {
                                RegistrarCorteCaja.MetodoPago = "Sin Metodo de Pago";
                            }
                            else
                            {
                                RegistrarCorteCaja.MetodoPago = rows_Inventory.paymentMethod.name;
                            }
                            

                            l_RegistrarCorteCaja.Add(RegistrarCorteCaja);
                        }
                    }

                    resp.Estatus = "Exito";
                    resp.Valor = l_RegistrarCorteCaja;
                    return resp;

                }
                else
                {
                    resp.Estatus = "Exito";
                    resp.Valor = l_RegistrarCorteCaja;
                    return resp;
                }
            }
            else
            {
                resp.Estatus = "Fracaso";
                return resp;
            }

        }

        public RespuestaServicios GetListPaymentForLocationAdvancedVer(BusquedaCorteCaja model)
        {
            NetSuiteService service;
            RespuestaServicios resp = new RespuestaServicios();
            List<MapCorteCaja> l_RegistrarCorteCaja = new List<MapCorteCaja>();

            RecordRef LocationRef = new RecordRef();
            LocationRef.type = RecordType.location;
            LocationRef.internalId = model.Location_ID;
            LocationRef.typeSpecified = true;

            TransactionSearchAdvanced BusquedaTransaccion = new TransactionSearchAdvanced();

            //search Criteria
            TransactionSearchBasic tranbasic = new TransactionSearchBasic();
            TransactionSearch transearch = new TransactionSearch();
            SearchMultiSelectField field = new SearchMultiSelectField();

            SearchDateField searchDate = new SearchDateField();
            searchDate.@operator = SearchDateFieldOperator.within;
            searchDate.searchValue = model.FechaDesde;
            searchDate.searchValue2 = model.FechaHasta;
            searchDate.operatorSpecified = true;
            searchDate.searchValueSpecified = true;
            searchDate.searchValue2Specified = true;

            SearchEnumMultiSelectField searchMultiSelectField = new SearchEnumMultiSelectField();
            searchMultiSelectField.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            searchMultiSelectField.operatorSpecified = true;
            searchMultiSelectField.searchValue = new string[] { "CustPymt" };

            RecordRef[] binsLocation = new RecordRef[1];
            binsLocation[0] = LocationRef;

            field.operatorSpecified = true;
            field.searchValue = binsLocation;

            tranbasic.location = field;
            tranbasic.dateCreated = searchDate;
            tranbasic.type = searchMultiSelectField;
            //tranbasic.
            //tranbasic.is
            transearch.basic = tranbasic;

            TransactionSearchRow Columns = new TransactionSearchRow();
            TransactionSearchRowBasic basicColumns = new TransactionSearchRowBasic();
            LocationSearchRowBasic locationJoins = new LocationSearchRowBasic();
            CustomerSearchRowBasic customerJoins = new CustomerSearchRowBasic();
            TransactionSearchRowBasic invJoins = new TransactionSearchRowBasic();
            CustomSearchRowBasic custjoin = new CustomSearchRowBasic();

            basicColumns.tranId = new SearchColumnStringField[] { new SearchColumnStringField() };
            basicColumns.appliedToTransaction = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.entity = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.appliedToLinkAmount = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.paymentMethod = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.paymentOption = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.account = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.dateCreated = new SearchColumnDateField[] { new SearchColumnDateField() };
            basicColumns.tranDate = new SearchColumnDateField[] { new SearchColumnDateField() };
            basicColumns.dueDate = new SearchColumnDateField[] { new SearchColumnDateField() };
            basicColumns.customFieldList = new SearchColumnSelectCustomField[] { new SearchColumnSelectCustomField() { scriptId = "custbody_mx_txn_sat_payment_method" } };

            locationJoins.name = new SearchColumnStringField[] { new SearchColumnStringField() };
            customerJoins.altName = new SearchColumnStringField[] { new SearchColumnStringField() };
            customerJoins.entityId = new SearchColumnStringField[] { new SearchColumnStringField() };
            
            invJoins.tranId = new SearchColumnStringField[] { new SearchColumnStringField() };
            invJoins.total = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            invJoins.customFieldList = new SearchColumnSelectCustomField[] { new SearchColumnSelectCustomField() { scriptId = "custbody_mx_txn_sat_payment_term" } };

            //itemJoin. = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            //itemJoin.entity = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            //itemJoin.appliedToLinkAmount = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            //itemJoin.paymentMethod = new SearchColumnSelectField[] { new SearchColumnSelectField() };

            Columns.basic = basicColumns;
            Columns.locationJoin = locationJoins;
            Columns.customerJoin = customerJoins;
            Columns.appliedToTransactionJoin = invJoins;

            BusquedaTransaccion.criteria = transearch;
            BusquedaTransaccion.columns = Columns;

            service = ConnectionManager.GetNetSuiteService();
            SearchResult res = service.search(BusquedaTransaccion);
            if (res.status.isSuccess)
            {

                var tamaño = res.totalPages;

                if (tamaño <= 1)
                {
                    SearchRow[] searchRows = res.searchRowList;
                    if (searchRows != null && searchRows.Length >= 1)
                    {

                        foreach (TransactionSearchRow rows_Inventory in searchRows)
                        {
                            try
                            {
                                //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando

                                if (rows_Inventory.basic.appliedToTransaction != null && rows_Inventory.basic.appliedToLinkAmount != null)
                                {
                                    MapCorteCaja RegistrarCorteCaja = new MapCorteCaja();

                                    RegistrarCorteCaja.ClaveCliente = rows_Inventory.customerJoin.entityId[0].searchValue;
                                    RegistrarCorteCaja.NombreCliente = rows_Inventory.customerJoin.altName[0].searchValue;
                                    RegistrarCorteCaja.FolioPago = rows_Inventory.basic.tranId[0].searchValue;
                                    RegistrarCorteCaja.MontoPago = rows_Inventory.basic.appliedToLinkAmount[0].searchValue;
                                    RegistrarCorteCaja.FolioFactura = rows_Inventory.appliedToTransactionJoin.tranId[0].searchValue;
                                    RegistrarCorteCaja.MontoFactura = rows_Inventory.appliedToTransactionJoin.total[0].searchValue;
                                    RegistrarCorteCaja.CuentaBancaria = rows_Inventory.basic.account[0].searchValue.internalId;
                                    //RegistrarCorteCaja.FechaEmision = rows_Inventory.appliedToTransactionJoin.tranDate[0].searchValue;
                                    RegistrarCorteCaja.FechaCreacion = rows_Inventory.basic.dateCreated[0].searchValue;
                                    RegistrarCorteCaja.FechaEmision = rows_Inventory.basic.tranDate[0].searchValue;
                                    //RegistrarCorteCaja.FechaVencimiento = rows_Inventory.basic.dueDate[0].searchValue;

                                    foreach (SearchColumnCustomField customField in rows_Inventory.basic.customFieldList)
                                    {
                                        SearchColumnSelectCustomField custField = (SearchColumnSelectCustomField)customField;
                                        RegistrarCorteCaja.MetodoPago = custField.searchValue.internalId;
                                    }

                                    if (rows_Inventory.appliedToTransactionJoin.customFieldList != null)
                                    {
                                        foreach (SearchColumnCustomField customField in rows_Inventory.appliedToTransactionJoin.customFieldList)
                                        {
                                            SearchColumnSelectCustomField custField = (SearchColumnSelectCustomField)customField;
                                            RegistrarCorteCaja.metodopagoFactura = custField.searchValue.internalId;
                                        }
                                    }
                                    else
                                    {
                                        RegistrarCorteCaja.metodopagoFactura = "SNM";
                                    }





                                    l_RegistrarCorteCaja.Add(RegistrarCorteCaja);
                                }
                            }
                            catch (Exception ex)
                            {
                                continue;
                            }


                        }

                        resp.Estatus = "Exito";
                        resp.Valor = l_RegistrarCorteCaja;
                        return resp;

                    }
                    else
                    {
                        resp.Estatus = "Exito";
                        resp.Valor = l_RegistrarCorteCaja;
                        return resp;
                    }
                } else {

                    for (int n = 1; n <= tamaño; n++)
                    {

                        service = ConnectionManager.GetNetSuiteService();
                        res = service.searchMoreWithId(res.searchId, n);

                        SearchRow[] searchRows = res.searchRowList;
                        if (searchRows != null && searchRows.Length >= 1)
                        {

                            foreach (TransactionSearchRow rows_Inventory in searchRows)
                            {
                                try
                                {
                                    //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando

                                    if (rows_Inventory.basic.appliedToTransaction != null && rows_Inventory.basic.appliedToLinkAmount != null)
                                    {
                                        MapCorteCaja RegistrarCorteCaja = new MapCorteCaja();

                                        if (rows_Inventory.basic.tranId[0].searchValue == "PYMT21494")
                                        {
                                            var s = "";
                                        }

                                        RegistrarCorteCaja.ClaveCliente = rows_Inventory.customerJoin.entityId[0].searchValue;
                                        RegistrarCorteCaja.NombreCliente = rows_Inventory.customerJoin.altName[0].searchValue;
                                        RegistrarCorteCaja.FolioPago = rows_Inventory.basic.tranId[0].searchValue;
                                        RegistrarCorteCaja.MontoPago = rows_Inventory.basic.appliedToLinkAmount[0].searchValue;
                                        RegistrarCorteCaja.FolioFactura = rows_Inventory.appliedToTransactionJoin.tranId[0].searchValue;
                                        RegistrarCorteCaja.MontoFactura = rows_Inventory.appliedToTransactionJoin.total[0].searchValue;
                                        RegistrarCorteCaja.CuentaBancaria = rows_Inventory.basic.account[0].searchValue.internalId;
                                        //RegistrarCorteCaja.FechaEmision = rows_Inventory.appliedToTransactionJoin.tranDate[0].searchValue;
                                        RegistrarCorteCaja.FechaCreacion = rows_Inventory.basic.dateCreated[0].searchValue;
                                        RegistrarCorteCaja.FechaEmision = rows_Inventory.basic.tranDate[0].searchValue;
                                        //RegistrarCorteCaja.FechaVencimiento = rows_Inventory.basic.dueDate[0].searchValue;

                                        foreach (SearchColumnCustomField customField in rows_Inventory.basic.customFieldList)
                                        {
                                            SearchColumnSelectCustomField custField = (SearchColumnSelectCustomField)customField;
                                            RegistrarCorteCaja.MetodoPago = custField.searchValue.internalId;
                                        }

                                        if (rows_Inventory.appliedToTransactionJoin.customFieldList != null)
                                        {
                                            foreach (SearchColumnCustomField customField in rows_Inventory.appliedToTransactionJoin.customFieldList)
                                            {
                                                SearchColumnSelectCustomField custField = (SearchColumnSelectCustomField)customField;
                                                RegistrarCorteCaja.metodopagoFactura = custField.searchValue.internalId;
                                            }
                                        }
                                        else
                                        {
                                            RegistrarCorteCaja.metodopagoFactura = "SNM";
                                        }





                                        l_RegistrarCorteCaja.Add(RegistrarCorteCaja);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }


                            }

                            resp.Estatus = "Exito";
                            resp.Valor = l_RegistrarCorteCaja;
                            return resp;

                        }
                        else
                        {
                            resp.Estatus = "Exito";
                            resp.Valor = l_RegistrarCorteCaja;
                            return resp;
                        }

                    }
                }

            }
            resp.Estatus = "Fracaso";
            resp.Mensaje = "Hubo un problema al procesar esta busqueda. Intentelo más tarde.";
            return resp;

        }

        public RespuestaServicios GetListPaymentsForCustomSearchAdvanced()
        {
            try
            {
                RespuestaServicios resp = new RespuestaServicios();

                NetSuiteService service;
                List<ActualizaStock> l_RegistrarStock = new List<ActualizaStock>();

                TransactionSearchAdvanced searchInventory = new TransactionSearchAdvanced();
                searchInventory.savedSearchId = "2063";

                service = ConnectionManager.GetNetSuiteService();
                SearchResult result = service.search(searchInventory);

                if (result.status.isSuccess)
                {
                    var tamaño = result.totalPages;

                    for (int n = 1; n <= tamaño; n++)
                    {

                        service = ConnectionManager.GetNetSuiteService();
                        result = service.searchMoreWithId(result.searchId, n);

                        if (result.searchRowList != null && result.searchRowList.Length > 0)
                        {
                            for (int i = 0; i <= result.searchRowList.Length; i++)
                            {
                                ActualizaStock registrarStock = new ActualizaStock();
                                InventoryNumberSearchRow resultados = (InventoryNumberSearchRow)result.searchRowList[i];

                                registrarStock.IssuesID = resultados.basic.internalId[0].searchValue.internalId;
                                registrarStock.InventoryID = resultados.basic.inventoryNumber[0].searchValue;
                                registrarStock.quantityavailable = resultados.basic.quantityavailable[0].searchValue;
                                registrarStock.item = resultados.basic.item[0].searchValue.internalId;
                                registrarStock.location = resultados.basic.location[0].searchValue.internalId;

                                l_RegistrarStock.Add(registrarStock);
                            }
                        }

                    }

                    resp.Estatus = "Exito";
                    resp.Valor = l_RegistrarStock;
                    return resp;
                }
                else
                {

                    resp.Estatus = "Fracaso";
                    return resp;
                }

            }
            catch (Exception ex)
            {
                RespuestaServicios resp = new RespuestaServicios();
                resp.Estatus = "Fracaso";
                return resp;
            }

        }

        public RespuestaServicios GetListCustomerAdvancedVer()
        {
            NetSuiteService service;
            RespuestaServicios resp = new RespuestaServicios();
            List<MapCorteCaja> l_RegistrarCorteCaja = new List<MapCorteCaja>();

            CustomerSearchAdvanced BusquedaClientes = new CustomerSearchAdvanced();

            //search Criteria
            CustomerSearchBasic custbasic = new CustomerSearchBasic();
            CustomerSearch custsearch = new CustomerSearch();
            SearchMultiSelectField field = new SearchMultiSelectField();

            SearchEnumMultiSelectField customerStage = new SearchEnumMultiSelectField();
            customerStage.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            customerStage.operatorSpecified = true;
            customerStage.searchValue = new string[] { "_customer" };

            field.operatorSpecified = true;
            custbasic.stage = customerStage;
            custsearch.basic = custbasic;

            //Columnas a consultar
            CustomerSearchRow Columns = new CustomerSearchRow();
            CustomerSearchRowBasic basicColumns = new CustomerSearchRowBasic();
            PricingSearchRowBasic pricingJoins = new PricingSearchRowBasic();

            basicColumns.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.entityId = new SearchColumnStringField[] { new SearchColumnStringField() };
            basicColumns.altName = new SearchColumnStringField[] { new SearchColumnStringField() };
            basicColumns.priceLevel = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.itemPricingLevel = new SearchColumnStringField[] { new SearchColumnStringField() };
            basicColumns.itemPricingUnitPrice = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.pricingItem = new SearchColumnStringField[] { new SearchColumnStringField() };
            //basicColumns.groupPricingLevel 
            basicColumns.customFieldList = new SearchColumnCustomField[] { new SearchColumnStringCustomField() { scriptId = "custentity_mx_rfc" } };

            pricingJoins.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            pricingJoins.item = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            pricingJoins.priceLevel = new SearchColumnSelectField[] { new SearchColumnSelectField() };

            Columns.basic = basicColumns;
            //Columns.pricingJoin = pricingJoins;
            //Columns.customSearchJoin = 

            BusquedaClientes.criteria = custsearch;
            BusquedaClientes.columns = Columns;

            service = ConnectionManager.GetNetSuiteService();
            SearchResult res = service.search(BusquedaClientes);

            if (res.status.isSuccess)
            {
                var tamaño = res.totalPages;

                for (int n = 1; n <= tamaño; n++)
                {

                    service = ConnectionManager.GetNetSuiteService();
                    res = service.searchMoreWithId(res.searchId, n);

                    SearchRow[] searchRows = res.searchRowList;
                    if (searchRows != null && searchRows.Length >= 1)
                    {

                        foreach (CustomerSearchRow rows_Customer in searchRows)
                        {
                            try
                            {
                                var iD = rows_Customer.basic.internalId[0].searchValue.internalId;
                                //if (iD == "4004")
                                //{
                                //    var l = "";
                                //}

                                //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando

                                //if (rows_Inventory.basic.appliedToTransaction != null && rows_Inventory.basic.appliedToLinkAmount != null)
                                //{
                                //    MapCorteCaja RegistrarCorteCaja = new MapCorteCaja();

                                //    RegistrarCorteCaja.NombreCliente = rows_Inventory.customerJoin.entityId[0].searchValue + " - " + rows_Inventory.customerJoin.altName[0].searchValue;
                                //    RegistrarCorteCaja.FolioPago = rows_Inventory.basic.tranId[0].searchValue;
                                //    RegistrarCorteCaja.MontoPago = rows_Inventory.basic.appliedToLinkAmount[0].searchValue;
                                //    RegistrarCorteCaja.FolioFactura = rows_Inventory.appliedToTransactionJoin.tranId[0].searchValue;

                                //    //if (rows_Inventory.basic.paymentMethod is null)
                                //    //{
                                //    //    RegistrarCorteCaja.MetodoPago = "Sin Metodo de Pago";
                                //    //}
                                //    //else
                                //    //{
                                //    //    //RegistrarCorteCaja.MetodoPago = rows_Inventory.basic.paymentMethod.name;
                                //    //}


                                //    l_RegistrarCorteCaja.Add(RegistrarCorteCaja);
                                //}
                            }
                            catch (Exception ex)
                            {
                                continue;
                            }


                        }

                        resp.Estatus = "Exito";
                        resp.Valor = l_RegistrarCorteCaja;


                    }
                    else
                    {
                        resp.Estatus = "Fracaso";
                    }

                }
            }

            return resp;

        }

        public RespuestaServicios GetListAssemblyItemsSearchAdvanced()
        {
            try
            {
                NetSuiteService service;
                RespuestaServicios resp = new RespuestaServicios();
                Sinc_JobItems sincJobs = new Sinc_JobItems();
                List<Sinc_AssemblyItems> l_Registraritems = new List<Sinc_AssemblyItems>();
                List<Sinc_PricingItems> l_RegistrarPrecios = new List<Sinc_PricingItems>();

                ItemSearchAdvanced BusquedaItems = new ItemSearchAdvanced();

                //Criterios de Busqueda
                ItemSearchBasic myItemSearchBasic = new ItemSearchBasic();
                ItemSearch transearch = new ItemSearch();
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
                transearch.basic = myItemSearchBasic;

                ItemSearchRow Columns = new ItemSearchRow();
                ItemSearchRowBasic basicColumns = new ItemSearchRowBasic();
                PricingSearchRowBasic pricingJ = new PricingSearchRowBasic();

                pricingJ.currency = new SearchColumnSelectField[] { new SearchColumnSelectField() };
                pricingJ.item = new SearchColumnSelectField[] { new SearchColumnSelectField() };
                pricingJ.unitPrice = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
                pricingJ.priceLevel = new SearchColumnSelectField[] { new SearchColumnSelectField() };

                basicColumns.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };
                basicColumns.itemId = new SearchColumnStringField[] { new SearchColumnStringField() };
                basicColumns.displayName = new SearchColumnStringField[] { new SearchColumnStringField() };

                Columns.basic = basicColumns;
                Columns.pricingJoin = pricingJ;

                BusquedaItems.criteria = transearch;
                BusquedaItems.columns = Columns;

                service = ConnectionManager.GetNetSuiteService();
                SearchResult res = service.search(BusquedaItems);
                if (res.status.isSuccess)
                {
                    var tamaño = res.totalPages;

                    for (int n = 1; n <= tamaño; n++)
                    {

                        service = ConnectionManager.GetNetSuiteService();
                        res = service.searchMoreWithId(res.searchId, n);

                        SearchRow[] searchRows = res.searchRowList;
                        if (searchRows != null && searchRows.Length >= 1)
                        {

                            foreach (ItemSearchRow rows_Inventory in searchRows)
                            {
                                try
                                {
                                    //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando

                                    if (rows_Inventory.basic != null && rows_Inventory.pricingJoin != null)
                                    {
                                        //Sinc_AssemblyItems RegistrarItem = new Sinc_AssemblyItems();
                                        Sinc_PricingItems RegistrarPrecio = new Sinc_PricingItems();
                                        //MapCorteCaja RegistrarCorteCaja = new MapCorteCaja();

                                        //RegistrarItem.s_displayName = rows_Inventory.basic.displayName[0].searchValue;
                                        //RegistrarItem.s_internalId = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                        //RegistrarItem.s_itemId = rows_Inventory.basic.itemId[0].searchValue;
                                        //l_Registraritems.Add(RegistrarItem);

                                        RegistrarPrecio.s_currency = rows_Inventory.pricingJoin.currency[0].searchValue.internalId;
                                        RegistrarPrecio.s_internalIdpricing = rows_Inventory.pricingJoin.priceLevel[0].searchValue.internalId;
                                        RegistrarPrecio.s_iteminternalId = rows_Inventory.pricingJoin.item[0].searchValue.internalId;
                                        RegistrarPrecio.s_value = rows_Inventory.pricingJoin.unitPrice[0].searchValue;
                                        l_RegistrarPrecios.Add(RegistrarPrecio);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }

                            }
                        }
                        else
                        {
                            continue;
                        }
                    }

                    sincJobs.l_Items = l_Registraritems;
                    sincJobs.l_Pricing = l_RegistrarPrecios;

                    resp.Estatus = "Exito";
                    resp.Valor = sincJobs;
                    return resp;
                }
                else
                {
                resp.Estatus = "Fracaso";
                resp.Mensaje = "Hubo un problema al procesar esta busqueda. Intentelo más tarde.";
                return resp;
                }

            }
            catch (Exception ex)
            {
                RespuestaServicios resp = new RespuestaServicios();
                resp.Estatus = "Fracaso";
                return resp;
            }

        }

        public RespuestaServicios GetListNotLotAssemblyItemsSearchAdvanced()
        {
            try
            {
                NetSuiteService service;
                RespuestaServicios resp = new RespuestaServicios();
                Sinc_JobItems sincJobs = new Sinc_JobItems();
                List<Sinc_AssemblyItems> l_Registraritems = new List<Sinc_AssemblyItems>();
                List<Sinc_PricingItems> l_RegistrarPrecios = new List<Sinc_PricingItems>();

                ItemSearchAdvanced BusquedaItems = new ItemSearchAdvanced();

                //Criterios de Busqueda
                ItemSearchBasic myItemSearchBasic = new ItemSearchBasic();
                ItemSearch transearch = new ItemSearch();
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
                transearch.basic = myItemSearchBasic;

                ItemSearchRow Columns = new ItemSearchRow();
                ItemSearchRowBasic basicColumns = new ItemSearchRowBasic();
                PricingSearchRowBasic pricingJ = new PricingSearchRowBasic();

                pricingJ.currency = new SearchColumnSelectField[] { new SearchColumnSelectField() };
                pricingJ.item = new SearchColumnSelectField[] { new SearchColumnSelectField() };
                pricingJ.unitPrice = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
                pricingJ.priceLevel = new SearchColumnSelectField[] { new SearchColumnSelectField() };

                basicColumns.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };
                basicColumns.itemId = new SearchColumnStringField[] { new SearchColumnStringField() };
                basicColumns.displayName = new SearchColumnStringField[] { new SearchColumnStringField() };

                Columns.basic = basicColumns;
                Columns.pricingJoin = pricingJ;

                BusquedaItems.criteria = transearch;
                BusquedaItems.columns = Columns;

                service = ConnectionManager.GetNetSuiteService();
                SearchResult res = service.search(BusquedaItems);
                if (res.status.isSuccess)
                {
                    var tamaño = res.totalPages;

                    for (int n = 1; n <= tamaño; n++)
                    {

                        service = ConnectionManager.GetNetSuiteService();
                        res = service.searchMoreWithId(res.searchId, n);

                        SearchRow[] searchRows = res.searchRowList;
                        if (searchRows != null && searchRows.Length >= 1)
                        {

                            foreach (ItemSearchRow rows_Inventory in searchRows)
                            {
                                try
                                {
                                    //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando

                                    if (rows_Inventory.basic != null && rows_Inventory.pricingJoin != null)
                                    {
                                        //Sinc_AssemblyItems RegistrarItem = new Sinc_AssemblyItems();
                                        Sinc_PricingItems RegistrarPrecio = new Sinc_PricingItems();
                                        //MapCorteCaja RegistrarCorteCaja = new MapCorteCaja();

                                        //RegistrarItem.s_displayName = rows_Inventory.basic.displayName[0].searchValue;
                                        //RegistrarItem.s_internalId = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                        //RegistrarItem.s_itemId = rows_Inventory.basic.itemId[0].searchValue;
                                        //l_Registraritems.Add(RegistrarItem);

                                        RegistrarPrecio.s_currency = rows_Inventory.pricingJoin.currency[0].searchValue.internalId;
                                        RegistrarPrecio.s_internalIdpricing = rows_Inventory.pricingJoin.priceLevel[0].searchValue.internalId;
                                        RegistrarPrecio.s_iteminternalId = rows_Inventory.pricingJoin.item[0].searchValue.internalId;
                                        RegistrarPrecio.s_value = rows_Inventory.pricingJoin.unitPrice[0].searchValue;
                                        l_RegistrarPrecios.Add(RegistrarPrecio);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }

                            }
                        }
                        else
                        {
                            continue;
                        }
                    }

                    sincJobs.l_Items = l_Registraritems;
                    sincJobs.l_Pricing = l_RegistrarPrecios;

                    resp.Estatus = "Exito";
                    resp.Valor = sincJobs;
                    return resp;
                }
                else
                {
                    resp.Estatus = "Fracaso";
                    resp.Mensaje = "Hubo un problema al procesar esta busqueda. Intentelo más tarde.";
                    return resp;
                }

            }
            catch (Exception ex)
            {
                RespuestaServicios resp = new RespuestaServicios();
                resp.Estatus = "Fracaso";
                return resp;
            }

        }

        public RespuestaServicios GetListInventoryItemsSearchAdvanced()
        {
            try
            {
                NetSuiteService service;
                RespuestaServicios resp = new RespuestaServicios();
                Sinc_JobItems sincJobs = new Sinc_JobItems();
                List<Sinc_AssemblyItems> l_Registraritems = new List<Sinc_AssemblyItems>();
                List<Sinc_PricingItems> l_RegistrarPrecios = new List<Sinc_PricingItems>();

                ItemSearchAdvanced BusquedaItems = new ItemSearchAdvanced();

                //Criterios de Busqueda
                ItemSearchBasic myItemSearchBasic = new ItemSearchBasic();
                ItemSearch transearch = new ItemSearch();
                SearchBooleanField basics = new SearchBooleanField();

                basics.searchValue = false;
                basics.searchValueSpecified = true;

                SearchEnumMultiSelectField myEnum = new SearchEnumMultiSelectField();
                myEnum.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
                myEnum.operatorSpecified = true;
                String[] searchStringArray = new String[1];
                searchStringArray[0] = "inventoryItem";
                myEnum.searchValue = searchStringArray;
                myItemSearchBasic.type = myEnum;
                //myItemSearchBasic.isLotItem = basics;
                transearch.basic = myItemSearchBasic;

                ItemSearchRow Columns = new ItemSearchRow();
                ItemSearchRowBasic basicColumns = new ItemSearchRowBasic();
                PricingSearchRowBasic pricingJ = new PricingSearchRowBasic();

                pricingJ.currency = new SearchColumnSelectField[] { new SearchColumnSelectField() };
                pricingJ.item = new SearchColumnSelectField[] { new SearchColumnSelectField() };
                pricingJ.unitPrice = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
                pricingJ.priceLevel = new SearchColumnSelectField[] { new SearchColumnSelectField() };

                basicColumns.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };
                basicColumns.itemId = new SearchColumnStringField[] { new SearchColumnStringField() };
                basicColumns.displayName = new SearchColumnStringField[] { new SearchColumnStringField() };

                Columns.basic = basicColumns;
                Columns.pricingJoin = pricingJ;

                BusquedaItems.criteria = transearch;
                BusquedaItems.columns = Columns;

                service = ConnectionManager.GetNetSuiteService();
                SearchResult res = service.search(BusquedaItems);
                if (res.status.isSuccess)
                {
                    var tamaño = res.totalPages;

                    for (int n = 1; n <= tamaño; n++)
                    {

                        service = ConnectionManager.GetNetSuiteService();
                        res = service.searchMoreWithId(res.searchId, n);

                        SearchRow[] searchRows = res.searchRowList;
                        if (searchRows != null && searchRows.Length >= 1)
                        {

                            foreach (ItemSearchRow rows_Inventory in searchRows)
                            {
                                try
                                {
                                    //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando

                                    if (rows_Inventory.basic != null && rows_Inventory.pricingJoin != null)
                                    {
                                        Sinc_AssemblyItems RegistrarItem = new Sinc_AssemblyItems();
                                        Sinc_PricingItems RegistrarPrecio = new Sinc_PricingItems();
                                        MapCorteCaja RegistrarCorteCaja = new MapCorteCaja();

                                        RegistrarItem.s_displayName = rows_Inventory.basic.displayName[0].searchValue;
                                        RegistrarItem.s_internalId = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                        RegistrarItem.s_itemId = rows_Inventory.basic.itemId[0].searchValue;
                                        l_Registraritems.Add(RegistrarItem);

                                        RegistrarPrecio.s_currency = rows_Inventory.pricingJoin.currency[0].searchValue.internalId;
                                        RegistrarPrecio.s_internalIdpricing = rows_Inventory.pricingJoin.priceLevel[0].searchValue.internalId;
                                        RegistrarPrecio.s_iteminternalId = rows_Inventory.pricingJoin.item[0].searchValue.internalId;
                                        RegistrarPrecio.s_value = rows_Inventory.pricingJoin.unitPrice[0].searchValue;
                                        l_RegistrarPrecios.Add(RegistrarPrecio);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }

                            }
                        }
                        else
                        {
                            continue;
                        }
                    }

                    sincJobs.l_Items = l_Registraritems;
                    sincJobs.l_Pricing = l_RegistrarPrecios;

                    resp.Estatus = "Exito";
                    resp.Valor = sincJobs;
                    return resp;
                }
                else
                {
                    resp.Estatus = "Fracaso";
                    resp.Mensaje = "Hubo un problema al procesar esta busqueda. Intentelo más tarde.";
                    return resp;
                }

            }
            catch (Exception ex)
            {
                RespuestaServicios resp = new RespuestaServicios();
                resp.Estatus = "Fracaso";
                return resp;
            }

        }

        public RespuestaServicios GetListServiceItemsSearchAdvanced()
        {
            try
            {
                NetSuiteService service;
                RespuestaServicios resp = new RespuestaServicios();
                Sinc_JobItems sincJobs = new Sinc_JobItems();
                List<Sinc_AssemblyItems> l_Registraritems = new List<Sinc_AssemblyItems>();
                List<Sinc_PricingItems> l_RegistrarPrecios = new List<Sinc_PricingItems>();

                ItemSearchAdvanced BusquedaItems = new ItemSearchAdvanced();

                //Criterios de Busqueda
                ItemSearchBasic myItemSearchBasic = new ItemSearchBasic();
                ItemSearch transearch = new ItemSearch();
                SearchBooleanField basics = new SearchBooleanField();

                basics.searchValue = false;
                basics.searchValueSpecified = true;

                SearchEnumMultiSelectField myEnum = new SearchEnumMultiSelectField();
                myEnum.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
                myEnum.operatorSpecified = true;
                String[] searchStringArray = new String[1];
                searchStringArray[0] = "Service";
                myEnum.searchValue = searchStringArray;
                myItemSearchBasic.type = myEnum;
                myItemSearchBasic.isLotItem = basics;
                transearch.basic = myItemSearchBasic;

                ItemSearchRow Columns = new ItemSearchRow();
                ItemSearchRowBasic basicColumns = new ItemSearchRowBasic();
                PricingSearchRowBasic pricingJ = new PricingSearchRowBasic();

                pricingJ.currency = new SearchColumnSelectField[] { new SearchColumnSelectField() };
                pricingJ.item = new SearchColumnSelectField[] { new SearchColumnSelectField() };
                pricingJ.unitPrice = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
                pricingJ.priceLevel = new SearchColumnSelectField[] { new SearchColumnSelectField() };

                basicColumns.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };
                basicColumns.itemId = new SearchColumnStringField[] { new SearchColumnStringField() };
                basicColumns.displayName = new SearchColumnStringField[] { new SearchColumnStringField() };

                Columns.basic = basicColumns;
                Columns.pricingJoin = pricingJ;

                BusquedaItems.criteria = transearch;
                BusquedaItems.columns = Columns;

                service = ConnectionManager.GetNetSuiteService();
                SearchResult res = service.search(BusquedaItems);
                if (res.status.isSuccess)
                {
                    var tamaño = res.totalPages;

                    for (int n = 1; n <= tamaño; n++)
                    {

                        service = ConnectionManager.GetNetSuiteService();
                        res = service.searchMoreWithId(res.searchId, n);

                        SearchRow[] searchRows = res.searchRowList;
                        if (searchRows != null && searchRows.Length >= 1)
                        {

                            foreach (ItemSearchRow rows_Inventory in searchRows)
                            {
                                try
                                {
                                    //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando

                                    if (rows_Inventory.basic != null && rows_Inventory.pricingJoin != null)
                                    {
                                        Sinc_AssemblyItems RegistrarItem = new Sinc_AssemblyItems();
                                        Sinc_PricingItems RegistrarPrecio = new Sinc_PricingItems();
                                        MapCorteCaja RegistrarCorteCaja = new MapCorteCaja();

                                        RegistrarItem.s_displayName = rows_Inventory.basic.displayName[0].searchValue;
                                        RegistrarItem.s_internalId = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                        RegistrarItem.s_itemId = rows_Inventory.basic.itemId[0].searchValue;
                                        l_Registraritems.Add(RegistrarItem);

                                        RegistrarPrecio.s_currency = rows_Inventory.pricingJoin.currency[0].searchValue.internalId;
                                        RegistrarPrecio.s_internalIdpricing = rows_Inventory.pricingJoin.priceLevel[0].searchValue.internalId;
                                        RegistrarPrecio.s_iteminternalId = rows_Inventory.pricingJoin.item[0].searchValue.internalId;
                                        RegistrarPrecio.s_value = rows_Inventory.pricingJoin.unitPrice[0].searchValue;
                                        l_RegistrarPrecios.Add(RegistrarPrecio);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }

                            }
                        }
                        else
                        {
                            continue;
                        }
                    }

                    sincJobs.l_Items = l_Registraritems;
                    sincJobs.l_Pricing = l_RegistrarPrecios;

                    resp.Estatus = "Exito";
                    resp.Valor = sincJobs;
                    return resp;
                }
                else
                {
                    resp.Estatus = "Fracaso";
                    resp.Mensaje = "Hubo un problema al procesar esta busqueda. Intentelo más tarde.";
                    return resp;
                }

            }
            catch (Exception ex)
            {
                RespuestaServicios resp = new RespuestaServicios();
                resp.Estatus = "Fracaso";
                return resp;
            }

        }

        public RespuestaServicios GetInventoryItemStock(string NS)
        {
            try
            {
                RespuestaServicios resp = new RespuestaServicios();

                NetSuiteService service;
                List<Sinc_StockInventarios> l_RegistrarStock = new List<Sinc_StockInventarios>();

                ItemSearchAdvanced searchInventory = new ItemSearchAdvanced();
                searchInventory.savedSearchId = NS;

                service = ConnectionManager.GetNetSuiteService();
                SearchResult result = service.search(searchInventory);

                if (result.status.isSuccess)
                {
                    var tamaño = result.totalPages;

                    for (int n = 1; n <= tamaño; n++)
                    {

                        service = ConnectionManager.GetNetSuiteService();
                        result = service.searchMoreWithId(result.searchId, n);

                        if (result.searchRowList != null && result.searchRowList.Length > 0)
                        {
                            for (int i = 0; i < result.searchRowList.Length; i++)
                            {
                                try
                                {
                                Sinc_StockInventarios registrarStock = new Sinc_StockInventarios();
                                ItemSearchRow resultados = (ItemSearchRow) result.searchRowList[i];

                                registrarStock.s_iteminternalId = resultados.basic.internalId[0].searchValue.internalId;
                                registrarStock.s_locationinternalId = resultados.basic.inventoryLocation[0].searchValue.internalId;
                                    if (resultados.basic.locationQuantityAvailable == null)
                                    {
                                        registrarStock.s_StockDisponible = 0;
                                    }
                                    else
                                    {
                                        registrarStock.s_StockDisponible = resultados.basic.locationQuantityAvailable[0].searchValue;
                                    }
                                
                                //registrarStock.s_locationinternalId = resultados.basic.inventoryNumber[0].searchValue;
                                //registrarStock.quantityavailable = resultados.basic.quantityavailable[0].searchValue;
                                //registrarStock.item = resultados.basic.item[0].searchValue.internalId;
                                //registrarStock.location = resultados.basic.location[0].searchValue.internalId;

                                l_RegistrarStock.Add(registrarStock);

                                } catch (Exception ex)
                                {
                                    var mesage = ex.Message;
                                    continue;
                                }

                            }
                        }

                    }

                    resp.Estatus = "Exito";
                    resp.Valor = l_RegistrarStock;
                    return resp;
                }
                else
                {

                    resp.Estatus = "Fracaso";
                    return resp;
                }

            }
            catch (Exception ex)
            {
                RespuestaServicios resp = new RespuestaServicios();
                resp.Estatus = "Fracaso";
                return resp;
            }

        }

        public RespuestaServicios GetnotLotAssemblyItemStock(string NS)
        {
            try
            {
                RespuestaServicios resp = new RespuestaServicios();

                NetSuiteService service;
                List<Sinc_StockInventarios> l_RegistrarStock = new List<Sinc_StockInventarios>();

                ItemSearchAdvanced searchInventory = new ItemSearchAdvanced();
                searchInventory.savedSearchId = NS;

                service = ConnectionManager.GetNetSuiteService();
                SearchResult result = service.search(searchInventory);

                if (result.status.isSuccess)
                {
                    var tamaño = result.totalPages;

                    for (int n = 1; n <= tamaño; n++)
                    {

                        service = ConnectionManager.GetNetSuiteService();
                        result = service.searchMoreWithId(result.searchId, n);

                        if (result.searchRowList != null && result.searchRowList.Length > 0)
                        {
                            for (int i = 0; i < result.searchRowList.Length; i++)
                            {
                                try
                                {
                                    Sinc_StockInventarios registrarStock = new Sinc_StockInventarios();
                                    ItemSearchRow resultados = (ItemSearchRow)result.searchRowList[i];

                                    registrarStock.s_iteminternalId = resultados.basic.internalId[0].searchValue.internalId;
                                    registrarStock.s_locationinternalId = resultados.basic.inventoryLocation[0].searchValue.internalId;
                                    registrarStock.s_StockDisponible = resultados.basic.locationQuantityAvailable[0].searchValue;

                                    l_RegistrarStock.Add(registrarStock);

                                }
                                catch (Exception ex)
                                {
                                    var mesage = ex.Message;
                                    continue;
                                }

                            }
                        }

                    }

                    resp.Estatus = "Exito";
                    resp.Valor = l_RegistrarStock;
                    return resp;
                }
                else
                {

                    resp.Estatus = "Fracaso";
                    return resp;
                }

            }
            catch (Exception ex)
            {
                RespuestaServicios resp = new RespuestaServicios();
                resp.Estatus = "Fracaso";
                return resp;
            }

        }

        public RespuestaServicios GetListAccountSearchAdvanced(string NS)
        {
            try
            {
                RespuestaServicios resp = new RespuestaServicios();

                NetSuiteService service;
                List<Sinc_Account> l_Account = new List<Sinc_Account>();

                AccountSearchAdvanced searchInventory = new AccountSearchAdvanced();
                searchInventory.savedSearchId = NS;

                service = ConnectionManager.GetNetSuiteService();
                SearchResult result = service.search(searchInventory);

                if (result.status.isSuccess)
                {
                    var tamaño = result.totalPages;

                    for (int n = 1; n <= tamaño; n++)
                    {

                        service = ConnectionManager.GetNetSuiteService();
                        result = service.searchMoreWithId(result.searchId, n);

                        if (result.searchRowList != null && result.searchRowList.Length > 0)
                        {
                            for (int i = 0; i <= result.searchRowList.Length; i++)
                            {
                                try
                                {
                                Sinc_Account registrarCuenta = new Sinc_Account();
                                AccountSearchRow resultados = (AccountSearchRow)result.searchRowList[i];

                                registrarCuenta.s_iteminternalId = resultados.basic.internalId[0].searchValue.internalId;
                                registrarCuenta.s_Descripcion = resultados.basic.displayName[0].searchValue;

                                l_Account.Add(registrarCuenta);
                                }
                                catch (Exception ex)
                                {
                                    var se = ex.Message;
                                }

                            }
                        }

                    }

                    resp.Estatus = "Exito";
                    resp.Valor = l_Account;
                    return resp;
                }
                else
                {

                    resp.Estatus = "Fracaso";
                    return resp;
                }

            }
            catch (Exception ex)
            {
                RespuestaServicios resp = new RespuestaServicios();
                resp.Estatus = "Fracaso";
                return resp;
            }

        }

        public RespuestaServicios GetListInvoiceForLocationAdvancedVer(BusquedaCorteCaja model)
        {
            NetSuiteService service;
            RespuestaServicios resp = new RespuestaServicios();
            List<MapCorteCajaRep> l_RegistrarCorteCaja = new List<MapCorteCajaRep>();

            RecordRef LocationRef = new RecordRef();
            LocationRef.type = RecordType.location;
            LocationRef.internalId = model.Location_ID;
            LocationRef.typeSpecified = true;

            TransactionSearchAdvanced BusquedaTransaccion = new TransactionSearchAdvanced();

            //search Criteria
            TransactionSearchBasic tranbasic = new TransactionSearchBasic();
            TransactionSearch transearch = new TransactionSearch();
            SearchMultiSelectField field = new SearchMultiSelectField();

            SearchDateField searchDate = new SearchDateField();
            searchDate.@operator = SearchDateFieldOperator.within;
            searchDate.searchValue = model.FechaDesde;
            searchDate.searchValue2 = model.FechaHasta;
            searchDate.operatorSpecified = true;
            searchDate.searchValueSpecified = true;
            searchDate.searchValue2Specified = true;

            SearchEnumMultiSelectField searchMultiSelectField = new SearchEnumMultiSelectField();
            searchMultiSelectField.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            searchMultiSelectField.operatorSpecified = true;
            searchMultiSelectField.searchValue = new string[] { "CustInvc" };

            RecordRef[] binsLocation = new RecordRef[1];
            binsLocation[0] = LocationRef;

            field.operatorSpecified = true;
            field.searchValue = binsLocation;

            tranbasic.location = field;
            tranbasic.dateCreated = searchDate;
            tranbasic.type = searchMultiSelectField;
            transearch.basic = tranbasic;

            TransactionSearchRow Columns = new TransactionSearchRow();
            TransactionSearchRowBasic basicColumns = new TransactionSearchRowBasic();
            LocationSearchRowBasic locationJoins = new LocationSearchRowBasic();
            CustomerSearchRowBasic customerJoins = new CustomerSearchRowBasic();

            basicColumns.tranId = new SearchColumnStringField[] { new SearchColumnStringField() };
            basicColumns.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.entity = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.paymentMethod = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.paymentOption = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.account = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.customFieldList = new SearchColumnSelectCustomField[] { new SearchColumnSelectCustomField() { scriptId = "custbody_mx_txn_sat_payment_method" } };
            basicColumns.taxAmount = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.taxTotal = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.netAmountNoTax = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.netAmount = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.type = new SearchColumnEnumSelectField[] { new SearchColumnEnumSelectField() };
            basicColumns.amount = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.discountAmount = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.fxAmount = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.total = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.dateCreated = new SearchColumnDateField[] { new SearchColumnDateField() };
            basicColumns.createdFrom = new SearchColumnSelectField[] { new SearchColumnSelectField() };

            locationJoins.name = new SearchColumnStringField[] { new SearchColumnStringField() };
            customerJoins.altName = new SearchColumnStringField[] { new SearchColumnStringField() };
            customerJoins.entityId = new SearchColumnStringField[] { new SearchColumnStringField() };

            Columns.basic = basicColumns;
            Columns.locationJoin = locationJoins;
            Columns.customerJoin = customerJoins;

            BusquedaTransaccion.criteria = transearch;
            BusquedaTransaccion.columns = Columns;

            service = ConnectionManager.GetNetSuiteService();
            SearchResult res = service.search(BusquedaTransaccion);
            if (res.status.isSuccess)
            {
                var tamaño = res.totalPages;

                if (tamaño <= 1)
                {
                    SearchRow[] searchRows = res.searchRowList;
                    if (searchRows != null && searchRows.Length >= 1)
                    {

                        foreach (TransactionSearchRow rows_Inventory in searchRows)
                        {
                            try
                            {
                                if (rows_Inventory.customerJoin != null)
                                {

                                    var iNS = rows_Inventory.basic.internalId[0].searchValue.internalId;

                                    if (l_RegistrarCorteCaja.Any(l => l.NoFactura == iNS))
                                    {

                                    }
                                    else
                                    {
                                        //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando
                                        MapCorteCajaRep RegistrarCorteCaja = new MapCorteCajaRep();

                                        RegistrarCorteCaja.ClaveCliente = rows_Inventory.customerJoin.entityId[0].searchValue;
                                        RegistrarCorteCaja.NombreCliente = rows_Inventory.customerJoin.altName[0].searchValue;
                                        RegistrarCorteCaja.FolioFactura = rows_Inventory.basic.tranId[0].searchValue;
                                        RegistrarCorteCaja.NoFactura = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                        RegistrarCorteCaja.SubtotalFactura = Math.Round(rows_Inventory.basic.netAmount[0].searchValue - rows_Inventory.basic.taxTotal[0].searchValue, 2);
                                        RegistrarCorteCaja.IvaFactura = rows_Inventory.basic.taxTotal[0].searchValue;
                                        RegistrarCorteCaja.TotalFactura = rows_Inventory.basic.netAmount[0].searchValue;
                                        RegistrarCorteCaja.Fecha = rows_Inventory.basic.dateCreated[0].searchValue;
                                        RegistrarCorteCaja.NS_ID_inv = rows_Inventory.basic.internalId[0].searchValue.internalId;

                                        if(rows_Inventory.basic.createdFrom != null)
                                        {
                                            RegistrarCorteCaja.NS_ID_OS = rows_Inventory.basic.createdFrom[0].searchValue.internalId;
                                        }
                                        else
                                        {
                                            RegistrarCorteCaja.NS_ID_OS = "Sin SO";
                                        }
                                        

                                        foreach (SearchColumnCustomField customField in rows_Inventory.basic.customFieldList)
                                        {
                                            SearchColumnSelectCustomField custField = (SearchColumnSelectCustomField)customField;
                                            RegistrarCorteCaja.FormaPago = custField.searchValue.internalId;
                                        }


                                        l_RegistrarCorteCaja.Add(RegistrarCorteCaja);
                                    }

                                }

                            }
                            catch (Exception ex)
                            {
                                continue;
                            }
                        }

                        resp.Estatus = "Exito";
                        resp.Valor = l_RegistrarCorteCaja;
                        return resp;
                    }
                    else
                    {
                        resp.Estatus = "Exito";
                        resp.Valor = l_RegistrarCorteCaja;
                        return resp;
                    }
                } else {

                    for (int n = 1; n <= tamaño; n++)
                    {

                        service = ConnectionManager.GetNetSuiteService();
                        res = service.searchMoreWithId(res.searchId, n);

                        SearchRow[] searchRows = res.searchRowList;
                        if (searchRows != null && searchRows.Length >= 1)
                        {

                            foreach (TransactionSearchRow rows_Inventory in searchRows)
                            {
                                try
                                {
                                    if (rows_Inventory.customerJoin != null)
                                    {

                                        var iNS = rows_Inventory.basic.internalId[0].searchValue.internalId;

                                        if (l_RegistrarCorteCaja.Any(l => l.NoFactura == iNS))
                                        {

                                        }
                                        else
                                        {
                                            //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando
                                            MapCorteCajaRep RegistrarCorteCaja = new MapCorteCajaRep();
                                            RegistrarCorteCaja.ClaveCliente = rows_Inventory.customerJoin.entityId[0].searchValue;
                                            RegistrarCorteCaja.NombreCliente = rows_Inventory.customerJoin.altName[0].searchValue;
                                            RegistrarCorteCaja.FolioFactura = rows_Inventory.basic.tranId[0].searchValue;
                                            RegistrarCorteCaja.NoFactura = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                            RegistrarCorteCaja.SubtotalFactura = Math.Round(rows_Inventory.basic.netAmount[0].searchValue - rows_Inventory.basic.taxTotal[0].searchValue, 2);
                                            RegistrarCorteCaja.IvaFactura = rows_Inventory.basic.taxTotal[0].searchValue;
                                            RegistrarCorteCaja.TotalFactura = rows_Inventory.basic.netAmount[0].searchValue;
                                            RegistrarCorteCaja.Fecha = rows_Inventory.basic.dateCreated[0].searchValue;
                                            RegistrarCorteCaja.NS_ID_inv = rows_Inventory.basic.internalId[0].searchValue.internalId;

                                            if (rows_Inventory.basic.createdFrom != null)
                                            {
                                                RegistrarCorteCaja.NS_ID_OS = rows_Inventory.basic.createdFrom[0].searchValue.internalId;
                                            }
                                            else
                                            {
                                                RegistrarCorteCaja.NS_ID_OS = "Sin SO";
                                            }

                                            foreach (SearchColumnCustomField customField in rows_Inventory.basic.customFieldList)
                                            {
                                                SearchColumnSelectCustomField custField = (SearchColumnSelectCustomField)customField;
                                                RegistrarCorteCaja.FormaPago = custField.searchValue.internalId;
                                            }


                                            l_RegistrarCorteCaja.Add(RegistrarCorteCaja);
                                        }

                                    }

                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }
                            }
                        }

                    }

                    resp.Estatus = "Exito";
                    resp.Valor = l_RegistrarCorteCaja;
                    return resp;
                }


            }
            else
            {
                resp.Estatus = "Fracaso";
                resp.Mensaje = "Hubo un problema al procesar esta busqueda.";
                return resp;
            }

        }

        public RespuestaServicios GetLevelPricing(string id)
        {
            try
            {
                RespuestaServicios resp = new RespuestaServicios();

                NetSuiteService service;
                List<Sinc_NivelPrecio> l_NivelPrecio = new List<Sinc_NivelPrecio>();

                PriceLevelSearchAdvanced searchInventory = new PriceLevelSearchAdvanced();
                searchInventory.savedSearchId = id;

                service = ConnectionManager.GetNetSuiteService();
                SearchResult result = service.search(searchInventory);

                if (result.status.isSuccess)
                {
                    var tamaño = result.totalPages;

                    for (int n = 1; n <= tamaño; n++)
                    {

                        service = ConnectionManager.GetNetSuiteService();
                        result = service.searchMoreWithId(result.searchId, n);

                        if (result.searchRowList != null && result.searchRowList.Length > 0)
                        {
                            for (int i = 0; i < result.searchRowList.Length; i++)
                            {
                                Sinc_NivelPrecio registrarNivelPrecio = new Sinc_NivelPrecio();
                                PriceLevelSearchRow resultados = (PriceLevelSearchRow) result.searchRowList[i];

                                registrarNivelPrecio.s_internalId = resultados.basic.internalId[0].searchValue.internalId;
                                registrarNivelPrecio.s_descripcion = resultados.basic.name[0].searchValue;

                                l_NivelPrecio.Add(registrarNivelPrecio);
                            }
                        }

                    }

                    resp.Estatus = "Exito";
                    resp.Valor = l_NivelPrecio;
                    return resp;
                }
                else
                {

                    resp.Estatus = "Fracaso";
                    return resp;
                }

            }
            catch (Exception ex)
            {
                RespuestaServicios resp = new RespuestaServicios();
                resp.Estatus = "Fracaso";
                return resp;
            }

        }

        public RespuestaServicios GetListServicesItemsSearchAdvanced()
        {
            try
            {
                NetSuiteService service;
                RespuestaServicios resp = new RespuestaServicios();
                Sinc_JobItems sincJobs = new Sinc_JobItems();
                List<Sinc_AssemblyItems> l_Registraritems = new List<Sinc_AssemblyItems>();
                List<Sinc_PricingItems> l_RegistrarPrecios = new List<Sinc_PricingItems>();

                ItemSearchAdvanced BusquedaItems = new ItemSearchAdvanced();

                //Criterios de Busqueda
                ItemSearchBasic myItemSearchBasic = new ItemSearchBasic();
                ItemSearch transearch = new ItemSearch();
                SearchBooleanField basics = new SearchBooleanField();

                basics.searchValue = false;
                basics.searchValueSpecified = true;

                SearchEnumMultiSelectField myEnum = new SearchEnumMultiSelectField();
                myEnum.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
                myEnum.operatorSpecified = true;
                String[] searchStringArray = new String[1];
                searchStringArray[0] = "Service";
                myEnum.searchValue = searchStringArray;
                myItemSearchBasic.type = myEnum;
                //myItemSearchBasic.isLotItem = basics;
                transearch.basic = myItemSearchBasic;

                ItemSearchRow Columns = new ItemSearchRow();
                ItemSearchRowBasic basicColumns = new ItemSearchRowBasic();
                PricingSearchRowBasic pricingJ = new PricingSearchRowBasic();

                pricingJ.currency = new SearchColumnSelectField[] { new SearchColumnSelectField() };
                pricingJ.item = new SearchColumnSelectField[] { new SearchColumnSelectField() };
                pricingJ.unitPrice = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
                pricingJ.priceLevel = new SearchColumnSelectField[] { new SearchColumnSelectField() };

                basicColumns.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };
                basicColumns.itemId = new SearchColumnStringField[] { new SearchColumnStringField() };
                basicColumns.displayName = new SearchColumnStringField[] { new SearchColumnStringField() };

                Columns.basic = basicColumns;
                Columns.pricingJoin = pricingJ;

                BusquedaItems.criteria = transearch;
                BusquedaItems.columns = Columns;

                service = ConnectionManager.GetNetSuiteService();
                SearchResult res = service.search(BusquedaItems);
                if (res.status.isSuccess)
                {
                    var tamaño = res.totalPages;

                    for (int n = 1; n <= tamaño; n++)
                    {

                        service = ConnectionManager.GetNetSuiteService();
                        res = service.searchMoreWithId(res.searchId, n);

                        SearchRow[] searchRows = res.searchRowList;
                        if (searchRows != null && searchRows.Length >= 1)
                        {

                            foreach (ItemSearchRow rows_Inventory in searchRows)
                            {
                                try
                                {
                                    //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando

                                    if (rows_Inventory.basic != null && rows_Inventory.pricingJoin != null)
                                    {
                                        Sinc_AssemblyItems RegistrarItem = new Sinc_AssemblyItems();
                                        Sinc_PricingItems RegistrarPrecio = new Sinc_PricingItems();
                                        MapCorteCaja RegistrarCorteCaja = new MapCorteCaja();

                                        RegistrarItem.s_displayName = rows_Inventory.basic.displayName[0].searchValue;
                                        RegistrarItem.s_internalId = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                        RegistrarItem.s_itemId = rows_Inventory.basic.itemId[0].searchValue;
                                        l_Registraritems.Add(RegistrarItem);

                                        RegistrarPrecio.s_currency = rows_Inventory.pricingJoin.currency[0].searchValue.internalId;
                                        RegistrarPrecio.s_internalIdpricing = rows_Inventory.pricingJoin.priceLevel[0].searchValue.internalId;
                                        RegistrarPrecio.s_iteminternalId = rows_Inventory.pricingJoin.item[0].searchValue.internalId;
                                        RegistrarPrecio.s_value = rows_Inventory.pricingJoin.unitPrice[0].searchValue;
                                        l_RegistrarPrecios.Add(RegistrarPrecio);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }

                            }
                        }
                        else
                        {
                            continue;
                        }
                    }

                    sincJobs.l_Items = l_Registraritems;
                    sincJobs.l_Pricing = l_RegistrarPrecios;

                    resp.Estatus = "Exito";
                    resp.Valor = sincJobs;
                    return resp;
                }
                else
                {
                    resp.Estatus = "Fracaso";
                    resp.Mensaje = "Hubo un problema al procesar esta busqueda. Intentelo más tarde.";
                    return resp;
                }

            }
            catch (Exception ex)
            {
                RespuestaServicios resp = new RespuestaServicios();
                resp.Estatus = "Fracaso";
                return resp;
            }

        }

        public RespuestaServicios GetListAllItemsMRP(string NS)
        {
            try
            {
                NetSuiteService service = new NetSuiteService();

                ItemSearchAdvanced searchInventory = new ItemSearchAdvanced();
                RespuestaServicios resp = new RespuestaServicios();
                List<MapListaArticulos> l_articulos = new List<MapListaArticulos>();
                searchInventory.savedSearchId = NS;

                service = ConnectionManager.GetNetSuiteService();
                SearchResult result = service.search(searchInventory);

                if (result.status.isSuccess)
                {
                    var tamaño = result.totalPages;

                    for (int n = 1; n <= tamaño; n++)
                    {

                        service = ConnectionManager.GetNetSuiteService();
                        result = service.searchMoreWithId(result.searchId, n);

                        if (result.searchRowList != null && result.searchRowList.Length > 0)
                        {
                            for (int i = 0; i <= result.searchRowList.Length; i++)
                            {
                                try
                                {
                                    ItemSearchRow resultados = (ItemSearchRow) result.searchRowList[i];

                                    if ((resultados.basic.salesDescription != null) && (resultados.basic.basePrice != null))
                                    {
                                        MapListaArticulos registrarCuenta = new MapListaArticulos();

                                        registrarCuenta.InternalID = resultados.basic.internalId[0].searchValue.internalId;
                                        registrarCuenta.ClaveArticulo = resultados.basic.itemId[0].searchValue;
                                        registrarCuenta.NombreArticulo = resultados.basic.salesDescription[0].searchValue;
                                        registrarCuenta.PrecioBase = resultados.basic.basePrice[0].searchValue;
                                        //registrarCuenta.InternalID_Location = resultados.basic.inventoryLocation[0].searchValue.internalId;

                                        //if (resultados.basic.locationQuantityAvailable != null)
                                        //{
                                        //    registrarCuenta.StockDisponible = resultados.basic.locationQuantityAvailable[0].searchValue;
                                        //}
                                        //else
                                        //{
                                        //    registrarCuenta.StockDisponible = 0;
                                        //}

                                        
                                        l_articulos.Add(registrarCuenta);
                                    }
                                    //registrarCuenta.s_iteminternalId = resultados.basic.internalId[0].searchValue.internalId;
                                    //registrarCuenta.s_Descripcion = resultados.basic.displayName[0].searchValue;

                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }

                            }
                        }

                    }

                    resp.Estatus = "Exito";
                    resp.Valor = l_articulos;
                    return resp;
                }
                else
                {

                    resp.Estatus = "Fracaso";
                    return resp;
                }

            } catch (Exception ex)
            {
                RespuestaServicios resp = new RespuestaServicios();
                resp.Estatus = "Fracaso";
                resp.Mensaje = ex.Message;
                return resp;
            }
        }

        public RespuestaServicios GetListAllStockMRP(string NS)
        {
            try
            {
                NetSuiteService service = new NetSuiteService();

                ItemSearchAdvanced searchInventory = new ItemSearchAdvanced();
                RespuestaServicios resp = new RespuestaServicios();
                List<MapListaArticulos> l_articulos = new List<MapListaArticulos>();
                searchInventory.savedSearchId = NS;

                service = ConnectionManager.GetNetSuiteService();
                SearchResult result = service.search(searchInventory);

                if (result.status.isSuccess)
                {
                    var tamaño = result.totalPages;

                    for (int n = 1; n <= tamaño; n++)
                    {

                        service = ConnectionManager.GetNetSuiteService();
                        result = service.searchMoreWithId(result.searchId, n);

                        if (result.searchRowList != null && result.searchRowList.Length > 0)
                        {
                            for (int i = 0; i < result.searchRowList.Length; i++)
                            {
                                try
                                {
                                    ItemSearchRow resultados = (ItemSearchRow)result.searchRowList[i];

                                    if (resultados.basic.salesDescription != null)
                                    {
                                        MapListaArticulos registrarCuenta = new MapListaArticulos();

                                        registrarCuenta.InternalID = resultados.basic.internalId[0].searchValue.internalId;
                                        registrarCuenta.NombreArticulo = resultados.basic.salesDescription[0].searchValue;
                                        registrarCuenta.InternalID_Location = resultados.basic.inventoryLocation[0].searchValue.internalId;
                                        registrarCuenta.StockDisponible = resultados.basic.locationQuantityAvailable[0].searchValue;
                                        registrarCuenta.PrecioBase = resultados.basic.basePrice[0].searchValue;

                                        //if (resultados.basic.locationQuantityAvailable != null)
                                        //{
                                        //    registrarCuenta.StockDisponible = resultados.basic.locationQuantityAvailable[0].searchValue;
                                        //}
                                        //else
                                        //{
                                        //    registrarCuenta.StockDisponible = 0;
                                        //}


                                        l_articulos.Add(registrarCuenta);
                                    }
                                    //registrarCuenta.s_iteminternalId = resultados.basic.internalId[0].searchValue.internalId;
                                    //registrarCuenta.s_Descripcion = resultados.basic.displayName[0].searchValue;

                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }

                            }
                        }

                    }

                    resp.Estatus = "Exito";
                    resp.Valor = l_articulos;
                    return resp;
                }
                else
                {

                    resp.Estatus = "Fracaso";
                    return resp;
                }

            }
            catch (Exception ex)
            {
                RespuestaServicios resp = new RespuestaServicios();
                resp.Estatus = "Fracaso";
                resp.Mensaje = ex.Message;
                return resp;
            }
        }

        public RespuestaServicios GetListItemsForInvoice(BusquedaCorteCaja model)
        {
            NetSuiteService service;
            RespuestaServicios resp = new RespuestaServicios();
            List<MapMRP> l_RegistrarCorteCaja = new List<MapMRP>();

            RecordRef LocationRef = new RecordRef();
            LocationRef.type = RecordType.location;
            LocationRef.internalId = model.Location_ID;
            LocationRef.typeSpecified = true;

            TransactionSearchAdvanced BusquedaTransaccion = new TransactionSearchAdvanced();

            //search Criteria
            TransactionSearchBasic tranbasic = new TransactionSearchBasic();
            TransactionSearch transearch = new TransactionSearch();
            SearchMultiSelectField field = new SearchMultiSelectField();

            SearchDateField searchDate = new SearchDateField();
            searchDate.@operator = SearchDateFieldOperator.within;
            searchDate.searchValue = model.FechaDesde;
            searchDate.searchValue2 = model.FechaHasta;
            searchDate.operatorSpecified = true;
            searchDate.searchValueSpecified = true;
            searchDate.searchValue2Specified = true;

            SearchEnumMultiSelectField searchMultiSelectField = new SearchEnumMultiSelectField();
            searchMultiSelectField.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            searchMultiSelectField.operatorSpecified = true;
            searchMultiSelectField.searchValue = new string[] { "CustInvc" };

            RecordRef[] binsLocation = new RecordRef[1];
            binsLocation[0] = LocationRef;

            field.operatorSpecified = true;
            field.searchValue = binsLocation;

            tranbasic.location = field;
            tranbasic.dateCreated = searchDate;
            tranbasic.type = searchMultiSelectField;
            //tranbasic.fulfillingTransaction
            //tranbasic.itemFulfillmentChoice
            transearch.basic = tranbasic;

            TransactionSearchRow Columns = new TransactionSearchRow();
            TransactionSearchRowBasic basicColumns = new TransactionSearchRowBasic();
            LocationSearchRowBasic locationJoins = new LocationSearchRowBasic();
            CustomerSearchRowBasic customerJoins = new CustomerSearchRowBasic();
            ItemSearchRowBasic itemJoins = new ItemSearchRowBasic();

            basicColumns.tranId = new SearchColumnStringField[] { new SearchColumnStringField() };
            basicColumns.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.entity = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.type = new SearchColumnEnumSelectField[] { new SearchColumnEnumSelectField() };
            basicColumns.dateCreated = new SearchColumnDateField[] { new SearchColumnDateField() };
            basicColumns.item = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.quantity = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };

            locationJoins.name = new SearchColumnStringField[] { new SearchColumnStringField() };
            customerJoins.altName = new SearchColumnStringField[] { new SearchColumnStringField() };
            customerJoins.entityId = new SearchColumnStringField[] { new SearchColumnStringField() };

            Columns.basic = basicColumns;
            Columns.locationJoin = locationJoins;
            Columns.customerJoin = customerJoins;
            Columns.itemJoin = itemJoins;

            BusquedaTransaccion.criteria = transearch;
            BusquedaTransaccion.columns = Columns;

            service = ConnectionManager.GetNetSuiteService();
            SearchResult res = service.search(BusquedaTransaccion);
            if (res.status.isSuccess)
            {
                var tamaño = res.totalPages;

                if (tamaño <= 1)
                {
                    SearchRow[] searchRows = res.searchRowList;
                    if (searchRows != null && searchRows.Length >= 1)
                    {

                        foreach (TransactionSearchRow rows_Inventory in searchRows)
                        {
                            try
                            {
                                if (rows_Inventory.basic.item != null)
                                {

                                    var iNS = rows_Inventory.basic.item[0].searchValue.internalId;

                                    //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando
                                    MapMRP RegItems = new MapMRP();

                                    RegItems.NS_InternalID = rows_Inventory.basic.item[0].searchValue.internalId;
                                    RegItems.Cantidad = rows_Inventory.basic.quantity[0].searchValue;
                                    RegItems.NS_ExternalID = rows_Inventory.basic.tranId[0].searchValue;

                                    l_RegistrarCorteCaja.Add(RegItems);

                                }

                            }
                            catch (Exception ex)
                            {
                                continue;
                            }
                        }

                        resp.Estatus = "Exito";
                        resp.Valor = l_RegistrarCorteCaja;
                        return resp;
                    }
                    else
                    {
                        resp.Estatus = "Exito";
                        resp.Valor = l_RegistrarCorteCaja;
                        return resp;
                    }
                }
                else
                {

                    for (int n = 1; n <= tamaño; n++)
                    {

                        service = ConnectionManager.GetNetSuiteService();
                        res = service.searchMoreWithId(res.searchId, n);

                        SearchRow[] searchRows = res.searchRowList;
                        if (searchRows != null && searchRows.Length >= 1)
                        {

                            foreach (TransactionSearchRow rows_Inventory in searchRows)
                            {
                                try
                                {
                                    if (rows_Inventory.basic.item != null)
                                    {

                                        var iNS = rows_Inventory.basic.item[0].searchValue.internalId;

                                        //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando
                                        MapMRP RegItems = new MapMRP();

                                        RegItems.NS_InternalID = rows_Inventory.basic.item[0].searchValue.internalId;
                                        RegItems.Cantidad = rows_Inventory.basic.quantity[0].searchValue;

                                        l_RegistrarCorteCaja.Add(RegItems);

                                    }

                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }
                            }
                        }

                    }

                    resp.Estatus = "Exito";
                    resp.Valor = l_RegistrarCorteCaja;
                    return resp;
                }


            }
            else
            {
                resp.Estatus = "Fracaso";
                resp.Mensaje = "Hubo un problema al procesar esta busqueda.";
                return resp;
            }

        }

        public RespuestaServicios GetListInvoiceForCustomerAdvancedVer(string NS_ID)
        {
            NetSuiteService service;
            RespuestaServicios resp = new RespuestaServicios();
            List<MapPVONetsuite> l_RegistrarCorteCaja = new List<MapPVONetsuite>();

            RecordRef CustomerRef = new RecordRef();
            CustomerRef.type = RecordType.customer;
            CustomerRef.internalId = NS_ID;
            CustomerRef.typeSpecified = true;

            TransactionSearchAdvanced BusquedaTransaccion = new TransactionSearchAdvanced();

            //search Criteria
            TransactionSearchBasic tranbasic = new TransactionSearchBasic();
            TransactionSearch transearch = new TransactionSearch();
            SearchMultiSelectField field = new SearchMultiSelectField();

            SearchEnumMultiSelectField searchMultiSelectField = new SearchEnumMultiSelectField();
            searchMultiSelectField.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            searchMultiSelectField.operatorSpecified = true;
            searchMultiSelectField.searchValue = new string[] { "CustInvc" };

            RecordRef[] binsCustomer = new RecordRef[1];
            binsCustomer[0] = CustomerRef;

            field.operatorSpecified = true;
            field.searchValue = binsCustomer;

            tranbasic.entity = field;
            tranbasic.type = searchMultiSelectField;
            transearch.basic = tranbasic;

            TransactionSearchRow Columns = new TransactionSearchRow();
            TransactionSearchRowBasic basicColumns = new TransactionSearchRowBasic();
            LocationSearchRowBasic locationJoins = new LocationSearchRowBasic();
            CustomerSearchRowBasic customerJoins = new CustomerSearchRowBasic();

            basicColumns.tranId = new SearchColumnStringField[] { new SearchColumnStringField() };
            basicColumns.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.entity = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.paymentMethod = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.paymentOption = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.account = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.customFieldList = new SearchColumnSelectCustomField[] { new SearchColumnSelectCustomField() { scriptId = "custbody_mx_txn_sat_payment_method" } };
            basicColumns.taxAmount = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.taxTotal = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.netAmountNoTax = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.netAmount = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.type = new SearchColumnEnumSelectField[] { new SearchColumnEnumSelectField() };
            basicColumns.amount = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.discountAmount = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.fxAmount = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.total = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.dateCreated = new SearchColumnDateField[] { new SearchColumnDateField() };
            basicColumns.amountRemaining = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.amountPaid = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };

            locationJoins.name = new SearchColumnStringField[] { new SearchColumnStringField() };
            customerJoins.altName = new SearchColumnStringField[] { new SearchColumnStringField() };
            customerJoins.entityId = new SearchColumnStringField[] { new SearchColumnStringField() };

            Columns.basic = basicColumns;
            Columns.locationJoin = locationJoins;
            Columns.customerJoin = customerJoins;

            BusquedaTransaccion.criteria = transearch;
            BusquedaTransaccion.columns = Columns;

            service = ConnectionManager.GetNetSuiteService();
            SearchResult res = service.search(BusquedaTransaccion);
            if (res.status.isSuccess)
            {
                var tamaño = res.totalPages;

                if (tamaño <= 1)
                {
                    SearchRow[] searchRows = res.searchRowList;
                    if (searchRows != null && searchRows.Length >= 1)
                    {

                        foreach (TransactionSearchRow rows_Inventory in searchRows)
                        {
                            try
                            {
                                if (rows_Inventory.customerJoin != null)
                                {

                                    var iNS = rows_Inventory.basic.internalId[0].searchValue.internalId;

                                    if (l_RegistrarCorteCaja.Any(l => l.NoFactura == iNS))
                                    {

                                    }
                                    else
                                    {
                                        //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando
                                        MapPVONetsuite RegistrarCorteCaja = new MapPVONetsuite();
                                        DateTime FechaEmision = new DateTime();
                                        FechaEmision = Convert.ToDateTime(rows_Inventory.basic.dateCreated[0].searchValue.Day + "/" + rows_Inventory.basic.dateCreated[0].searchValue.Month + "/" + rows_Inventory.basic.dateCreated[0].searchValue.Year);

                                        RegistrarCorteCaja.ClaveCliente = rows_Inventory.customerJoin.entityId[0].searchValue;
                                        RegistrarCorteCaja.NombreCliente = rows_Inventory.customerJoin.altName[0].searchValue;
                                        RegistrarCorteCaja.FolioFactura = rows_Inventory.basic.tranId[0].searchValue;
                                        RegistrarCorteCaja.NoFactura = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                        RegistrarCorteCaja.SubtotalFactura = Math.Round(rows_Inventory.basic.netAmount[0].searchValue - rows_Inventory.basic.taxTotal[0].searchValue, 2);
                                        RegistrarCorteCaja.IvaFactura = rows_Inventory.basic.taxTotal[0].searchValue;
                                        RegistrarCorteCaja.TotalFactura = rows_Inventory.basic.netAmount[0].searchValue;
                                        RegistrarCorteCaja.Fecha = FechaEmision;
                                        RegistrarCorteCaja.InternalID = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                        RegistrarCorteCaja.SaldoPendiente = rows_Inventory.basic.amountRemaining[0].searchValue;
                                        RegistrarCorteCaja.SaldoPagado = rows_Inventory.basic.amountPaid[0].searchValue;

                                        foreach (SearchColumnCustomField customField in rows_Inventory.basic.customFieldList)
                                        {
                                            SearchColumnSelectCustomField custField = (SearchColumnSelectCustomField)customField;
                                            RegistrarCorteCaja.FormaPago = custField.searchValue.internalId;
                                        }


                                        l_RegistrarCorteCaja.Add(RegistrarCorteCaja);
                                    }

                                }

                            }
                            catch (Exception ex)
                            {
                                continue;
                            }
                        }

                        resp.Estatus = "Exito";
                        resp.Valor = l_RegistrarCorteCaja;
                        return resp;
                    }
                    else
                    {
                        resp.Estatus = "Exito";
                        resp.Valor = l_RegistrarCorteCaja;
                        return resp;
                    }
                }
                else
                {

                    for (int n = 1; n <= tamaño; n++)
                    {

                        service = ConnectionManager.GetNetSuiteService();
                        res = service.searchMoreWithId(res.searchId, n);

                        SearchRow[] searchRows = res.searchRowList;
                        if (searchRows != null && searchRows.Length >= 1)
                        {

                            foreach (TransactionSearchRow rows_Inventory in searchRows)
                            {
                                try
                                {
                                    if (rows_Inventory.customerJoin != null)
                                    {

                                        var iNS = rows_Inventory.basic.internalId[0].searchValue.internalId;

                                        if (l_RegistrarCorteCaja.Any(l => l.NoFactura == iNS))
                                        {

                                        }
                                        else
                                        {
                                            //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando
                                            MapPVONetsuite RegistrarCorteCaja = new MapPVONetsuite();
                                            DateTime FechaEmision = new DateTime();
                                            FechaEmision = Convert.ToDateTime(rows_Inventory.basic.dateCreated[0].searchValue.Day + "/" + rows_Inventory.basic.dateCreated[0].searchValue.Month + "/" + rows_Inventory.basic.dateCreated[0].searchValue.Year);

                                            RegistrarCorteCaja.ClaveCliente = rows_Inventory.customerJoin.entityId[0].searchValue;
                                            RegistrarCorteCaja.NombreCliente = rows_Inventory.customerJoin.altName[0].searchValue;
                                            RegistrarCorteCaja.FolioFactura = rows_Inventory.basic.tranId[0].searchValue;
                                            RegistrarCorteCaja.NoFactura = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                            RegistrarCorteCaja.SubtotalFactura = Math.Round(rows_Inventory.basic.netAmount[0].searchValue - rows_Inventory.basic.taxTotal[0].searchValue, 2);
                                            RegistrarCorteCaja.IvaFactura = rows_Inventory.basic.taxTotal[0].searchValue;
                                            RegistrarCorteCaja.TotalFactura = rows_Inventory.basic.netAmount[0].searchValue;
                                            RegistrarCorteCaja.Fecha = FechaEmision;
                                            RegistrarCorteCaja.InternalID = rows_Inventory.basic.internalId[0].searchValue.internalId;


                                            foreach (SearchColumnCustomField customField in rows_Inventory.basic.customFieldList)
                                            {
                                                SearchColumnSelectCustomField custField = (SearchColumnSelectCustomField)customField;
                                                RegistrarCorteCaja.FormaPago = custField.searchValue.internalId;
                                            }


                                            l_RegistrarCorteCaja.Add(RegistrarCorteCaja);
                                        }

                                    }

                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }
                            }
                        }

                    }

                    resp.Estatus = "Exito";
                    resp.Valor = l_RegistrarCorteCaja;
                    return resp;
                }


            }
            else
            {
                resp.Estatus = "Fracaso";
                resp.Mensaje = "Hubo un problema al procesar esta busqueda.";
                return resp;
            }

        }

        public RespuestaServicios GetListPaymentForInvoice(string NS_ID)
        {
            NetSuiteService service;
            RespuestaServicios resp = new RespuestaServicios();

            LocationSearchBasic location = new LocationSearchBasic();
            List<MapFacturasPagos> l_RegistrarCorteCaja = new List<MapFacturasPagos>();

            RecordRef DocumentoRef = new RecordRef();
            DocumentoRef.type = RecordType.invoice;
            DocumentoRef.internalId = NS_ID;
            DocumentoRef.typeSpecified = true;

            SearchMultiSelectField searchMultiInvoiceSelectField = new SearchMultiSelectField();
            searchMultiInvoiceSelectField.@operator = SearchMultiSelectFieldOperator.anyOf;
            searchMultiInvoiceSelectField.operatorSpecified = true;
            searchMultiInvoiceSelectField.searchValue = new[] { DocumentoRef };

            TransactionSearchBasic Transaction = new TransactionSearchBasic();
            SearchEnumMultiSelectField searchMultiSelectField = new SearchEnumMultiSelectField();
            searchMultiSelectField.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            searchMultiSelectField.operatorSpecified = true;
            searchMultiSelectField.searchValue = new string[] { "CustPymt" , "CustCred" };
            Transaction.type = searchMultiSelectField;
            Transaction.appliedToTransaction = searchMultiInvoiceSelectField;
            //Transaction.createdFrom = searchMultiInvoiceSelectField;

            TransactionSearch BusquedaTransaccion = new TransactionSearch();
            BusquedaTransaccion.basic = Transaction;

            BusquedaTransaccion.locationJoin = location;

            service = ConnectionManager.GetNetSuiteService();

            SearchResult searchResult = service.search(BusquedaTransaccion);

            if (searchResult.status.isSuccess)
            {
                Record[] searchRows = searchResult.recordList;
                if (searchRows != null && searchRows.Length >= 1)
                {

                    foreach (CustomerPayment rows_Inventory in searchRows)
                    {

                        if (rows_Inventory != null)
                        {
                            MapFacturasPagos RegistrarCorteCaja = new MapFacturasPagos();

                            RegistrarCorteCaja.FolioPago = rows_Inventory.tranId;
                            RegistrarCorteCaja.MontoPago = rows_Inventory.applied;
                            RegistrarCorteCaja.InternalID = rows_Inventory.internalId;
                            RegistrarCorteCaja.FechaEmision = rows_Inventory.createdDate;

                            l_RegistrarCorteCaja.Add(RegistrarCorteCaja);
                        }
                    }

                    resp.Estatus = "Exito";
                    resp.Valor = l_RegistrarCorteCaja;
                    return resp;

                }
                else
                {
                    resp.Estatus = "Exito";
                    resp.Valor = l_RegistrarCorteCaja;
                    return resp;
                }
            }
            else
            {
                resp.Estatus = "Fracaso";
                return resp;
            }

        }

        public RespuestaServicios GetListStockForLocation(string NS)
        {
            try
            {
                NetSuiteService service;
                RespuestaServicios resp = new RespuestaServicios();
                List<MapListaArticulos> l_articulos = new List<MapListaArticulos>();

                ItemSearchAdvanced BusquedaItems = new ItemSearchAdvanced();

                //Criterios de Busqueda
                ItemSearchBasic myItemSearchBasic = new ItemSearchBasic();
                ItemSearch transearch = new ItemSearch();

                SearchDoubleField searchQuantityAvaible = new SearchDoubleField();
                searchQuantityAvaible.@operator = SearchDoubleFieldOperator.greaterThan;
                searchQuantityAvaible.operatorSpecified = true;
                searchQuantityAvaible.searchValue = 0;
                searchQuantityAvaible.searchValue2 = 0;
                searchQuantityAvaible.searchValueSpecified = true;
                searchQuantityAvaible.searchValue2Specified = true;

                SearchDoubleField searchPrice = new SearchDoubleField();
                searchPrice.@operator = SearchDoubleFieldOperator.greaterThan;
                searchPrice.operatorSpecified = true;
                searchPrice.searchValue = 0;
                searchPrice.searchValue2 = 0;
                searchPrice.searchValueSpecified = true;
                searchPrice.searchValue2Specified = true;

                RecordRef LocationRef = new RecordRef();
                LocationRef.type = RecordType.location;
                LocationRef.internalId = NS;
                LocationRef.typeSpecified = true;

                RecordRef LocationcedisRef = new RecordRef();
                LocationcedisRef.type = RecordType.location;
                LocationcedisRef.internalId = "2";
                LocationcedisRef.typeSpecified = true;

                RecordRef LocationmuellesRef = new RecordRef();
                LocationmuellesRef.type = RecordType.location;
                LocationmuellesRef.internalId = "117";
                LocationmuellesRef.typeSpecified = true;

                RecordRef[] binsLocation = new RecordRef[1];
                binsLocation[0] = LocationRef;
                binsLocation = binsLocation.Append(LocationcedisRef).ToArray();
                binsLocation = binsLocation.Append(LocationmuellesRef).ToArray();

                SearchMultiSelectField field = new SearchMultiSelectField();
                field.operatorSpecified = true;
                field.searchValue = binsLocation; 

                SearchEnumMultiSelectField myEnum = new SearchEnumMultiSelectField();
                myEnum.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
                myEnum.operatorSpecified = true;
                String[] searchStringArray = new String[1];
                searchStringArray[0] = "Assembly";
                searchStringArray = searchStringArray.Append("InvtPart").ToArray();
                myEnum.searchValue = searchStringArray;
                myItemSearchBasic.type = myEnum;
                myItemSearchBasic.locationQuantityAvailable = searchQuantityAvaible;
                myItemSearchBasic.inventoryLocation = field;
                myItemSearchBasic.price = searchPrice;
                transearch.basic = myItemSearchBasic;

                ItemSearchRow Columns = new ItemSearchRow();
                ItemSearchRowBasic basicColumns = new ItemSearchRowBasic();

                basicColumns.itemId = new SearchColumnStringField[] { new SearchColumnStringField() };

                basicColumns.displayName = new SearchColumnStringField[] { new SearchColumnStringField() };
                basicColumns.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };
                basicColumns.locationQuantityAvailable = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
                basicColumns.inventoryLocation = new SearchColumnSelectField[] { new SearchColumnSelectField() };
                basicColumns.basePrice = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
                //basicColumns.itemId

                Columns.basic = basicColumns;

                BusquedaItems.criteria = transearch;
                BusquedaItems.columns = Columns;

                service = ConnectionManager.GetNetSuiteService();
                SearchResult res = service.search(BusquedaItems);
                if (res.status.isSuccess)
                {
                    var tamaño = res.totalPages;

                    for (int n = 1; n <= tamaño; n++)
                    {

                        service = ConnectionManager.GetNetSuiteService();
                        res = service.searchMoreWithId(res.searchId, n);

                        SearchRow[] searchRows = res.searchRowList;
                        if (searchRows != null && searchRows.Length >= 1)
                        {

                            foreach (ItemSearchRow rows_Inventory in searchRows)
                            {
                                try
                                {
                                    //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando

                                    if (rows_Inventory.basic != null)
                                    {
                                        MapListaArticulos registrarCuenta = new MapListaArticulos();

                                        registrarCuenta.InternalID = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                        registrarCuenta.NombreArticulo = rows_Inventory.basic.displayName[0].searchValue;
                                        registrarCuenta.InternalID_Location = rows_Inventory.basic.inventoryLocation[0].searchValue.internalId;
                                        registrarCuenta.StockDisponible = rows_Inventory.basic.locationQuantityAvailable[0].searchValue;
                                        registrarCuenta.PrecioBase = rows_Inventory.basic.basePrice[0].searchValue;

                                        if (rows_Inventory.basic.itemId[0] != null)
                                        {
                                            registrarCuenta.ClaveArticulo = rows_Inventory.basic.itemId[0].searchValue;
                                        }
                                        else
                                        {
                                            registrarCuenta.ClaveArticulo = "Sin Clave";
                                        }
                                        

                                        l_articulos.Add(registrarCuenta);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }

                            }
                        }
                        else
                        {
                            continue;
                        }
                    }

                    resp.Estatus = "Exito";
                    resp.Valor = l_articulos;
                    return resp;
                }
                else
                {
                    resp.Estatus = "Fracaso";
                    resp.Mensaje = "Hubo un problema al procesar esta busqueda. Intentelo más tarde.";
                    return resp;
                }

            }
            catch (Exception ex)
            {
                RespuestaServicios resp = new RespuestaServicios();
                resp.Estatus = "Fracaso";
                return resp;
            }
        }

        public RespuestaServicios GetListInvoiceForCustomerPendingPay(string NS_ID)
        {
            NetSuiteService service;
            RespuestaServicios resp = new RespuestaServicios();
            List<MapFacturasPendientesPago> l_RegistrarFacturasPendientes = new List<MapFacturasPendientesPago>();

            RecordRef CustomerRef = new RecordRef();
            CustomerRef.type = RecordType.customer;
            CustomerRef.internalId = NS_ID;
            CustomerRef.typeSpecified = true;

            TransactionSearchAdvanced BusquedaTransaccion = new TransactionSearchAdvanced();

            //search Criteria
            TransactionSearchBasic tranbasic = new TransactionSearchBasic();
            TransactionSearch transearch = new TransactionSearch();
            SearchMultiSelectField field = new SearchMultiSelectField();

            SearchEnumMultiSelectField searchMultiSelectField = new SearchEnumMultiSelectField();
            searchMultiSelectField.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            searchMultiSelectField.operatorSpecified = true;
            searchMultiSelectField.searchValue = new string[] { "CustInvc" };

            SearchDoubleField searchAmountRemain = new SearchDoubleField();
            searchAmountRemain.@operator = SearchDoubleFieldOperator.greaterThan;
            searchAmountRemain.operatorSpecified = true;
            searchAmountRemain.searchValue = 0;
            searchAmountRemain.searchValue2 = 0;
            searchAmountRemain.searchValueSpecified = true;
            searchAmountRemain.searchValue2Specified = true;

            RecordRef[] binsCustomer = new RecordRef[1];
            binsCustomer[0] = CustomerRef;

            field.operatorSpecified = true;
            field.searchValue = binsCustomer;

            tranbasic.entity = field;
            tranbasic.type = searchMultiSelectField;
            tranbasic.amountRemaining = searchAmountRemain;
            transearch.basic = tranbasic;

            TransactionSearchRow Columns = new TransactionSearchRow();
            TransactionSearchRowBasic basicColumns = new TransactionSearchRowBasic();

            basicColumns.tranId = new SearchColumnStringField[] { new SearchColumnStringField() };
            basicColumns.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.customFieldList = new SearchColumnSelectCustomField[] { new SearchColumnSelectCustomField() { scriptId = "custbody_mx_txn_sat_payment_term" } };
            basicColumns.amount = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.amountPaid = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.amountRemaining = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.total = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.dateCreated = new SearchColumnDateField[] { new SearchColumnDateField() };
            //basicColumns.dueDate = new SearchColumnDateField[] { new SearchColumnDateField() };

            Columns.basic = basicColumns;

            BusquedaTransaccion.criteria = transearch;
            BusquedaTransaccion.columns = Columns;

            service = ConnectionManager.GetNetSuiteService();
            SearchResult res = service.search(BusquedaTransaccion);
            if (res.status.isSuccess)
            {
                var tamaño = res.totalPages;

                if (tamaño <= 1)
                {
                    SearchRow[] searchRows = res.searchRowList;
                    if (searchRows != null && searchRows.Length >= 1)
                    {

                        foreach (TransactionSearchRow rows_Inventory in searchRows)
                        {
                            try
                            {

                                    var iNS = rows_Inventory.basic.internalId[0].searchValue.internalId;

                                if (l_RegistrarFacturasPendientes.Any(l => l.NS_InternalID == iNS))
                                {

                                }
                                else
                                {
                                    //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando
                                    MapFacturasPendientesPago RegistrarFactura = new MapFacturasPendientesPago();

                                    RegistrarFactura.NS_InternalID = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                    RegistrarFactura.NS_ExternalID = rows_Inventory.basic.tranId[0].searchValue;
                                    RegistrarFactura.Importe_Adeudado = rows_Inventory.basic.amountRemaining[0].searchValue;
                                    RegistrarFactura.FechaCreated = rows_Inventory.basic.dateCreated[0].searchValue;
                                    //RegistrarFactura.Fecha_Vencimiento = rows_Inventory.basic.dueDate[0].searchValue;
                                    RegistrarFactura.Total = rows_Inventory.basic.amount[0].searchValue;
                                    

                                        foreach (SearchColumnCustomField customField in rows_Inventory.basic.customFieldList)
                                        {
                                            SearchColumnSelectCustomField custField = (SearchColumnSelectCustomField)customField;
                                            RegistrarFactura.MetodoPagoSAT = custField.searchValue.internalId;
                                        }

                                        l_RegistrarFacturasPendientes.Add(RegistrarFactura);
                                    }                         

                            }
                            catch (Exception ex)
                            {
                                continue;
                            }
                        }

                        resp.Estatus = "Exito";
                        resp.Valor = l_RegistrarFacturasPendientes;
                        return resp;
                    }
                    else
                    {
                        resp.Estatus = "Exito";
                        resp.Valor = l_RegistrarFacturasPendientes;
                        return resp;
                    }
                }
                else
                {

                    for (int n = 1; n <= tamaño; n++)
                    {

                        service = ConnectionManager.GetNetSuiteService();
                        res = service.searchMoreWithId(res.searchId, n);

                        SearchRow[] searchRows = res.searchRowList;
                        if (searchRows != null && searchRows.Length >= 1)
                        {

                            foreach (TransactionSearchRow rows_Inventory in searchRows)
                            {
                                try
                                {
                                        var iNS = rows_Inventory.basic.internalId[0].searchValue.internalId;

                                        if (l_RegistrarFacturasPendientes.Any(l => l.NS_InternalID == iNS))
                                        {

                                        }
                                        else
                                        {
                                            //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando
                                            MapFacturasPendientesPago RegistrarFactura = new MapFacturasPendientesPago();

                                            RegistrarFactura.NS_InternalID = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                            RegistrarFactura.NS_ExternalID = rows_Inventory.basic.tranId[0].searchValue;
                                            RegistrarFactura.Importe_Adeudado = rows_Inventory.basic.amountRemaining[0].searchValue;
                                            RegistrarFactura.FechaCreated = rows_Inventory.basic.dateCreated[0].searchValue;
                                            RegistrarFactura.Fecha_Vencimiento = rows_Inventory.basic.dueDate[0].searchValue;
                                            RegistrarFactura.Total = rows_Inventory.basic.amount[0].searchValue;

                                            foreach (SearchColumnCustomField customField in rows_Inventory.basic.customFieldList)
                                            {
                                                SearchColumnSelectCustomField custField = (SearchColumnSelectCustomField)customField;
                                                RegistrarFactura.MetodoPagoSAT = custField.searchValue.internalId;
                                            }

                                            l_RegistrarFacturasPendientes.Add(RegistrarFactura);
                                        }

                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }
                            }
                        }

                    }

                    resp.Estatus = "Exito";
                    resp.Valor = l_RegistrarFacturasPendientes;
                    return resp;
                }


            }
            else
            {
                resp.Estatus = "Fracaso";
                resp.Mensaje = "Hubo un problema al procesar esta busqueda.";
                return resp;
            }

        }

        public RespuestaServicios GetListPaymentApplied(BusquedaCorteCaja model)
        {
            NetSuiteService service;
            RespuestaServicios resp = new RespuestaServicios();
            List<Consulta_Pagos> l_PagosRegistrados = new List<Consulta_Pagos>();

            RecordRef LocationRef = new RecordRef();
            LocationRef.type = RecordType.location;
            LocationRef.internalId = model.Location_ID;
            LocationRef.typeSpecified = true;

            TransactionSearchAdvanced BusquedaTransaccion = new TransactionSearchAdvanced();

            //search Criteria
            TransactionSearchBasic tranbasic = new TransactionSearchBasic();
            TransactionSearch transearch = new TransactionSearch();
            SearchMultiSelectField field = new SearchMultiSelectField();

            SearchDateField searchDate = new SearchDateField();
            searchDate.@operator = SearchDateFieldOperator.within;
            searchDate.searchValue = model.FechaDesde;
            searchDate.searchValue2 = model.FechaHasta;
            searchDate.operatorSpecified = true;
            searchDate.searchValueSpecified = true;
            searchDate.searchValue2Specified = true;

            SearchEnumMultiSelectField searchMultiSelectField = new SearchEnumMultiSelectField();
            searchMultiSelectField.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            searchMultiSelectField.operatorSpecified = true;
            searchMultiSelectField.searchValue = new string[] { "_customerPayment" };

            RecordRef[] binsLocation = new RecordRef[1];
            binsLocation[0] = LocationRef;

            field.operatorSpecified = true;
            field.searchValue = binsLocation;

            if(model.Customer != null)
            {
                SearchMultiSelectField fieldcustomer = new SearchMultiSelectField();
                RecordRef CustomerRef = new RecordRef();
                CustomerRef.type = RecordType.customer;
                CustomerRef.internalId = model.Customer;
                CustomerRef.typeSpecified = true;

                RecordRef[] binsCustomer = new RecordRef[1];
                binsCustomer[0] = CustomerRef;

                fieldcustomer.operatorSpecified = true;
                fieldcustomer.searchValue = binsCustomer;

                tranbasic.entity = fieldcustomer;
            }

            tranbasic.location = field;
            tranbasic.dateCreated = searchDate;
            tranbasic.type = searchMultiSelectField;
            //tranbasic.
            //tranbasic.is
            transearch.basic = tranbasic;

            TransactionSearchRow Columns = new TransactionSearchRow();
            TransactionSearchRowBasic basicColumns = new TransactionSearchRowBasic();
            LocationSearchRowBasic locationJoins = new LocationSearchRowBasic();
            TransactionSearchRowBasic invJoins = new TransactionSearchRowBasic();
            CustomSearchRowBasic custjoin = new CustomSearchRowBasic();
            CustomerSearchRowBasic customerJoins = new CustomerSearchRowBasic();

            basicColumns.tranId = new SearchColumnStringField[] { new SearchColumnStringField() };
            basicColumns.appliedToTransaction = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.entity = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.appliedToLinkAmount = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.paymentMethod = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.dateCreated = new SearchColumnDateField[] { new SearchColumnDateField() };
            basicColumns.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.total = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.entity = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.customFieldList = new SearchColumnSelectCustomField[] { new SearchColumnSelectCustomField() { scriptId = "custbody_mx_txn_sat_payment_method" }};

            basicColumns.customFieldList = basicColumns.customFieldList.Append(new SearchColumnStringCustomField() { scriptId = "custbody_mx_cfdi_uuid" }).ToArray();

            locationJoins.name = new SearchColumnStringField[] { new SearchColumnStringField() };
            customerJoins.altName = new SearchColumnStringField[] { new SearchColumnStringField() };
            customerJoins.entityId = new SearchColumnStringField[] { new SearchColumnStringField() };

            invJoins.tranId = new SearchColumnStringField[] { new SearchColumnStringField() };
            invJoins.total = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            invJoins.tranDate = new SearchColumnDateField[] { new SearchColumnDateField() };
            invJoins.dueDate = new SearchColumnDateField[] { new SearchColumnDateField() };

            //itemJoin. = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            //itemJoin.entity = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            //itemJoin.appliedToLinkAmount = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            //itemJoin.paymentMethod = new SearchColumnSelectField[] { new SearchColumnSelectField() };

            Columns.basic = basicColumns;
            Columns.locationJoin = locationJoins;
            Columns.appliedToTransactionJoin = invJoins;
            Columns.customerJoin = customerJoins;

            BusquedaTransaccion.criteria = transearch;
            BusquedaTransaccion.columns = Columns;

            service = ConnectionManager.GetNetSuiteService();
            SearchResult res = service.search(BusquedaTransaccion);
            if (res.status.isSuccess)
            {
                SearchRow[] searchRows = res.searchRowList;
                if (searchRows != null && searchRows.Length >= 1)
                {

                    foreach (TransactionSearchRow rows_Inventory in searchRows)
                    {
                        try
                        {
                            //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando
                            var iNS = rows_Inventory.basic.internalId[0].searchValue.internalId;

                            if (l_PagosRegistrados.Any(l => l.NS_InternalID == iNS))
                            {

                            } else
                            {
                                if(rows_Inventory.basic.appliedToLinkAmount != null)
                                {
                                     Consulta_Pagos RegistrarPago = new Consulta_Pagos();

                                     RegistrarPago.NS_InternalID = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                     RegistrarPago.FolioPago = rows_Inventory.basic.tranId[0].searchValue;
                                     RegistrarPago.MontoPago = rows_Inventory.basic.total[0].searchValue;
                                     RegistrarPago.FechaEmision = rows_Inventory.basic.dateCreated[0].searchValue;
                                     RegistrarPago.Nombre_Cliente = rows_Inventory.customerJoin.entityId[0].searchValue + " - " + rows_Inventory.customerJoin.altName[0].searchValue;

                                    foreach (SearchColumnCustomField customField in rows_Inventory.basic.customFieldList)
                                    {
                                        if (customField.scriptId == "custbody_mx_txn_sat_payment_method")
                                        {
                                            SearchColumnSelectCustomField custField = (SearchColumnSelectCustomField)customField;
                                            RegistrarPago.MetodoPago = custField.searchValue.internalId;
                                        }
                                        else if (customField.scriptId == "custbody_mx_cfdi_uuid")
                                        {
                                            SearchColumnStringCustomField custField = (SearchColumnStringCustomField)customField;
                                            RegistrarPago.UUID = custField.searchValue;
                                            RegistrarPago.Timbrado = "True";
                                        }

                                    }

                                l_PagosRegistrados.Add(RegistrarPago);
                                }


                            }

                                
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }


                    }

                    resp.Estatus = "Exito";
                    resp.Valor = l_PagosRegistrados;
                    return resp;

                }
                else
                {
                    resp.Estatus = "Exito";
                    resp.Valor = l_PagosRegistrados;
                    return resp;
                }
            }
            resp.Estatus = "Fracaso";
            resp.Mensaje = "Hubo un problema al procesar esta busqueda. Intentelo más tarde.";
            return resp;

        }

        public RespuestaServicios GetItemForSalesOrders(string ClaveArticulo)
        {
            try
            {
                NetSuiteService service = new NetSuiteService();
                RespuestaServicios resp = new RespuestaServicios();
                Sinc_Item RegistrarPrecio = new Sinc_Item();

                ItemSearchAdvanced BusquedaItems = new ItemSearchAdvanced();

                //Criterios de Busqueda
                ItemSearchBasic myItemSearchBasic = new ItemSearchBasic();
                ItemSearch transearch = new ItemSearch();

                SearchEnumMultiSelectField myEnum = new SearchEnumMultiSelectField();
                myEnum.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
                myEnum.operatorSpecified = true;
                String[] searchStringArray = new String[0];
                searchStringArray = new string[] { "Assembly", "Service", "InvtPart" };
                myEnum.searchValue = searchStringArray;

                SearchStringField mystringVals = new SearchStringField();
                mystringVals.@operator = SearchStringFieldOperator.@is;
                mystringVals.operatorSpecified = true;
                mystringVals.searchValue = ClaveArticulo;

                myItemSearchBasic.type = myEnum;
                myItemSearchBasic.itemId = mystringVals;

                transearch.basic = myItemSearchBasic;

                ItemSearchRow Columns = new ItemSearchRow();
                ItemSearchRowBasic basicColumns = new ItemSearchRowBasic();

                basicColumns.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };
                basicColumns.itemId = new SearchColumnStringField[] { new SearchColumnStringField() };
                basicColumns.displayName = new SearchColumnStringField[] { new SearchColumnStringField() };
                basicColumns.type = new SearchColumnEnumSelectField[] { new SearchColumnEnumSelectField() };
               
                Columns.basic = basicColumns;

                BusquedaItems.criteria = transearch;
                BusquedaItems.columns = Columns;

                service = ConnectionManager.GetNetSuiteService();
                SearchResult res = service.search(BusquedaItems);
                if (res.status.isSuccess)
                {

                    SearchRow[] searchRows = res.searchRowList;
                    if (searchRows != null && searchRows.Length >= 1)
                    {

                        foreach (ItemSearchRow rows_Inventory in searchRows)
                        {
                            try
                            {
                                //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando

                                if (rows_Inventory.basic != null)
                                {

                                    RegistrarPrecio.s_iteminternalId = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                    RegistrarPrecio.s_displayname = rows_Inventory.basic.displayName[0].searchValue;
                                    RegistrarPrecio.s_claveArticulo = ClaveArticulo.ToUpper();
                                    RegistrarPrecio.s_Tipo = rows_Inventory.basic.type[0].searchValue;
                                }
                            }
                            catch (Exception ex)
                            {
                                continue;
                            }

                        }
                    }
                    else
                    {
                        resp.Estatus = "Fracaso";
                        resp.Mensaje = "El articulo no se pudo localizar, verifique el codigo de articulo ingresado.";
                        return resp;
                    }

                    resp.Estatus = "Exito";
                    resp.Valor = RegistrarPrecio;
                    return resp;
                }
                else
                {
                    resp.Estatus = "Fracaso";
                    resp.Mensaje = "Hubo un problema al procesar esta busqueda. Intentelo más tarde.";
                    return resp;
                }

            }
            catch (Exception ex)
            {
                RespuestaServicios resp = new RespuestaServicios();
                resp.Estatus = "Fracaso";
                return resp;
            }

        }

        public RespuestaServicios GetKardexItem(BusquedaKardex model)
        {
            NetSuiteService service;
            RespuestaServicios resp = new RespuestaServicios();
            List<Consulta_Pagos> l_PagosRegistrados = new List<Consulta_Pagos>();

            RecordRef LocationRef = new RecordRef();
            LocationRef.type = RecordType.location;
            LocationRef.internalId = model.Location_ID;
            LocationRef.typeSpecified = true;

            TransactionSearchAdvanced BusquedaTransaccion = new TransactionSearchAdvanced();

            //search Criteria
            TransactionSearchBasic tranbasic = new TransactionSearchBasic();
            TransactionSearch transearch = new TransactionSearch();
            SearchMultiSelectField field = new SearchMultiSelectField();

            SearchDateField searchDate = new SearchDateField();
            searchDate.@operator = SearchDateFieldOperator.within;
            searchDate.searchValue = model.FechaDesde;
            searchDate.searchValue2 = model.FechaHasta;
            searchDate.operatorSpecified = true;
            searchDate.searchValueSpecified = true;
            searchDate.searchValue2Specified = true;

            SearchEnumMultiSelectField searchMultiSelectField = new SearchEnumMultiSelectField();
            searchMultiSelectField.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            searchMultiSelectField.operatorSpecified = true;
            searchMultiSelectField.searchValue = new string[] { "CustInvc", "TrnfrOrd" };

            RecordRef ItemRef = new RecordRef();
            if (model.Clasificacion_Articulo == "Assembly_nL") {
               
                ItemRef.type = RecordType.assemblyItem;

            } else if (model.Clasificacion_Articulo == "Inventory")
            {

                ItemRef.type = RecordType.inventoryItem;

            }      
            ItemRef.internalId = model.Articulo;
            ItemRef.typeSpecified = true;

            SearchMultiSelectField searhItemmultiselectField = new SearchMultiSelectField();
            searhItemmultiselectField.@operator = SearchMultiSelectFieldOperator.anyOf;
            searhItemmultiselectField.operatorSpecified = true;
            searhItemmultiselectField.searchValue = new RecordRef[] {ItemRef};

            RecordRef[] binsLocation = new RecordRef[1];
            binsLocation[0] = LocationRef;

            field.operatorSpecified = true;
            field.searchValue = binsLocation;

            tranbasic.location = field;
            tranbasic.dateCreated = searchDate;
            tranbasic.type = searchMultiSelectField;
            tranbasic.item = searhItemmultiselectField;
            transearch.basic = tranbasic;

            TransactionSearchRow Columns = new TransactionSearchRow();
            TransactionSearchRowBasic basicColumns = new TransactionSearchRowBasic();
            TransactionSearchRowBasic invJoins = new TransactionSearchRowBasic();
            LocationSearchRowBasic locationJoins = new LocationSearchRowBasic();
            CustomSearchRowBasic custjoin = new CustomSearchRowBasic();
            CustomerSearchRowBasic customerJoins = new CustomerSearchRowBasic();

            basicColumns.tranDate = new SearchColumnDateField[] { new SearchColumnDateField() };
            basicColumns.item = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.type = new SearchColumnEnumSelectField[] { new SearchColumnEnumSelectField() };
            basicColumns.tranId = new SearchColumnStringField[] { new SearchColumnStringField() };
            basicColumns.entity = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.costEstimate = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.costEstimateRate = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.quantityShipRecv = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.quantityBilled = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.quantity = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.location = new SearchColumnSelectField[] { new SearchColumnSelectField() };

            locationJoins.name = new SearchColumnStringField[] { new SearchColumnStringField() };
            customerJoins.altName = new SearchColumnStringField[] { new SearchColumnStringField() };
            customerJoins.entityId = new SearchColumnStringField[] { new SearchColumnStringField() };

            //itemJoin. = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            //itemJoin.entity = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            //itemJoin.appliedToLinkAmount = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            //itemJoin.paymentMethod = new SearchColumnSelectField[] { new SearchColumnSelectField() };

            Columns.basic = basicColumns;
            Columns.locationJoin = locationJoins;
            Columns.appliedToTransactionJoin = invJoins;
            Columns.customerJoin = customerJoins;

            BusquedaTransaccion.criteria = transearch;
            BusquedaTransaccion.columns = Columns;

            service = ConnectionManager.GetNetSuiteService();
            SearchResult res = service.search(BusquedaTransaccion);
            if (res.status.isSuccess)
            {
                SearchRow[] searchRows = res.searchRowList;
                if (searchRows != null && searchRows.Length >= 1)
                {

                    foreach (TransactionSearchRow rows_Inventory in searchRows)
                    {
                        try
                        {
                            //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando
                            var iNS = rows_Inventory.basic.internalId[0].searchValue.internalId;

                            if (l_PagosRegistrados.Any(l => l.NS_InternalID == iNS))
                            {

                            }
                            else
                            {
                                if (rows_Inventory.basic.appliedToLinkAmount != null)
                                {
                                    Consulta_Pagos RegistrarPago = new Consulta_Pagos();

                                    RegistrarPago.NS_InternalID = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                    RegistrarPago.FolioPago = rows_Inventory.basic.tranId[0].searchValue;
                                    RegistrarPago.MontoPago = rows_Inventory.basic.total[0].searchValue;
                                    RegistrarPago.FechaEmision = rows_Inventory.basic.dateCreated[0].searchValue;
                                    RegistrarPago.Nombre_Cliente = rows_Inventory.customerJoin.entityId[0].searchValue + " - " + rows_Inventory.customerJoin.altName[0].searchValue;

                                    foreach (SearchColumnCustomField customField in rows_Inventory.basic.customFieldList)
                                    {
                                        if (customField.scriptId == "custbody_mx_txn_sat_payment_method")
                                        {
                                            SearchColumnSelectCustomField custField = (SearchColumnSelectCustomField)customField;
                                            RegistrarPago.MetodoPago = custField.searchValue.internalId;
                                        }
                                        else if (customField.scriptId == "custbody_mx_cfdi_uuid")
                                        {
                                            SearchColumnStringCustomField custField = (SearchColumnStringCustomField)customField;
                                            RegistrarPago.UUID = custField.searchValue;
                                            RegistrarPago.Timbrado = "True";
                                        }

                                    }

                                    l_PagosRegistrados.Add(RegistrarPago);
                                }


                            }


                        }
                        catch (Exception ex)
                        {
                            continue;
                        }


                    }

                    resp.Estatus = "Exito";
                    resp.Valor = l_PagosRegistrados;
                    return resp;

                }
                else
                {
                    resp.Estatus = "Exito";
                    resp.Valor = l_PagosRegistrados;
                    return resp;
                }
            }
            resp.Estatus = "Fracaso";
            resp.Mensaje = "Hubo un problema al procesar esta busqueda. Intentelo más tarde.";
            return resp;

        }

        public RespuestaServicios GetListCustomers(string NS)
        {
            try
            {
                NetSuiteService service;
                RespuestaServicios resp = new RespuestaServicios();
                List<MapListaArticulos> l_articulos = new List<MapListaArticulos>();

                ItemSearchAdvanced BusquedaItems = new ItemSearchAdvanced();

                //Criterios de Busqueda
                ItemSearchBasic myItemSearchBasic = new ItemSearchBasic();
                ItemSearch transearch = new ItemSearch();

                SearchDoubleField searchQuantityAvaible = new SearchDoubleField();
                searchQuantityAvaible.@operator = SearchDoubleFieldOperator.greaterThan;
                searchQuantityAvaible.operatorSpecified = true;
                searchQuantityAvaible.searchValue = 0;
                searchQuantityAvaible.searchValue2 = 0;
                searchQuantityAvaible.searchValueSpecified = true;
                searchQuantityAvaible.searchValue2Specified = true;

                SearchDoubleField searchPrice = new SearchDoubleField();
                searchPrice.@operator = SearchDoubleFieldOperator.greaterThan;
                searchPrice.operatorSpecified = true;
                searchPrice.searchValue = 0;
                searchPrice.searchValue2 = 0;
                searchPrice.searchValueSpecified = true;
                searchPrice.searchValue2Specified = true;

                RecordRef LocationRef = new RecordRef();
                LocationRef.type = RecordType.location;
                LocationRef.internalId = NS;
                LocationRef.typeSpecified = true;

                RecordRef LocationcedisRef = new RecordRef();
                LocationcedisRef.type = RecordType.location;
                LocationcedisRef.internalId = "2";
                LocationcedisRef.typeSpecified = true;

                RecordRef[] binsLocation = new RecordRef[1];
                binsLocation[0] = LocationRef;
                binsLocation = binsLocation.Append(LocationcedisRef).ToArray();

                SearchMultiSelectField field = new SearchMultiSelectField();
                field.operatorSpecified = true;
                field.searchValue = binsLocation;

                SearchEnumMultiSelectField myEnum = new SearchEnumMultiSelectField();
                myEnum.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
                myEnum.operatorSpecified = true;
                String[] searchStringArray = new String[1];
                searchStringArray[0] = "Assembly";
                searchStringArray = searchStringArray.Append("InvtPart").ToArray();
                myEnum.searchValue = searchStringArray;
                myItemSearchBasic.type = myEnum;
                myItemSearchBasic.locationQuantityAvailable = searchQuantityAvaible;
                myItemSearchBasic.inventoryLocation = field;
                myItemSearchBasic.price = searchPrice;
                transearch.basic = myItemSearchBasic;

                ItemSearchRow Columns = new ItemSearchRow();
                ItemSearchRowBasic basicColumns = new ItemSearchRowBasic();

                basicColumns.itemId = new SearchColumnStringField[] { new SearchColumnStringField() };

                basicColumns.displayName = new SearchColumnStringField[] { new SearchColumnStringField() };
                basicColumns.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };
                basicColumns.locationQuantityAvailable = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
                basicColumns.inventoryLocation = new SearchColumnSelectField[] { new SearchColumnSelectField() };
                basicColumns.basePrice = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
                //basicColumns.itemId

                Columns.basic = basicColumns;

                BusquedaItems.criteria = transearch;
                BusquedaItems.columns = Columns;

                service = ConnectionManager.GetNetSuiteService();
                SearchResult res = service.search(BusquedaItems);
                if (res.status.isSuccess)
                {
                    var tamaño = res.totalPages;

                    for (int n = 1; n <= tamaño; n++)
                    {

                        service = ConnectionManager.GetNetSuiteService();
                        res = service.searchMoreWithId(res.searchId, n);

                        SearchRow[] searchRows = res.searchRowList;
                        if (searchRows != null && searchRows.Length >= 1)
                        {

                            foreach (ItemSearchRow rows_Inventory in searchRows)
                            {
                                try
                                {
                                    //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando

                                    if (rows_Inventory.basic != null)
                                    {
                                        MapListaArticulos registrarCuenta = new MapListaArticulos();

                                        registrarCuenta.InternalID = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                        registrarCuenta.NombreArticulo = rows_Inventory.basic.displayName[0].searchValue;
                                        registrarCuenta.InternalID_Location = rows_Inventory.basic.inventoryLocation[0].searchValue.internalId;
                                        registrarCuenta.StockDisponible = rows_Inventory.basic.locationQuantityAvailable[0].searchValue;
                                        registrarCuenta.PrecioBase = rows_Inventory.basic.basePrice[0].searchValue;

                                        if (rows_Inventory.basic.itemId[0] != null)
                                        {
                                            registrarCuenta.ClaveArticulo = rows_Inventory.basic.itemId[0].searchValue;
                                        }
                                        else
                                        {
                                            registrarCuenta.ClaveArticulo = "Sin Clave";
                                        }


                                        l_articulos.Add(registrarCuenta);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }

                            }
                        }
                        else
                        {
                            continue;
                        }
                    }

                    resp.Estatus = "Exito";
                    resp.Valor = l_articulos;
                    return resp;
                }
                else
                {
                    resp.Estatus = "Fracaso";
                    resp.Mensaje = "Hubo un problema al procesar esta busqueda. Intentelo más tarde.";
                    return resp;
                }

            }
            catch (Exception ex)
            {
                RespuestaServicios resp = new RespuestaServicios();
                resp.Estatus = "Fracaso";
                return resp;
            }
        }

        public List<MapClientes> GetListCustomersAdvancedVER()
        {
            try
            {
                NetSuiteService service = new NetSuiteService();
                RespuestaServicios resp = new RespuestaServicios();
                CustomerSearchAdvanced BusquedaClientes = new CustomerSearchAdvanced();
                List<MapClientes> l_clientes = new List<MapClientes>();

                //search Criteria
                CustomerSearchBasic custbasic = new CustomerSearchBasic();
                CustomerSearch custsearch = new CustomerSearch();
                SearchMultiSelectField field = new SearchMultiSelectField();

                SearchEnumMultiSelectField customerStage = new SearchEnumMultiSelectField();
                customerStage.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
                customerStage.operatorSpecified = true;
                customerStage.searchValue = new string[] { "_customer" };

                field.operatorSpecified = true;
                custbasic.stage = customerStage;
                custsearch.basic = custbasic;

                //Columnas a consultar
                CustomerSearchRow Columns = new CustomerSearchRow();
                CustomerSearchRowBasic basicColumns = new CustomerSearchRowBasic();
                PricingSearchRowBasic pricingJoins = new PricingSearchRowBasic();

                basicColumns.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };
                basicColumns.entityId = new SearchColumnStringField[] { new SearchColumnStringField() };
                basicColumns.altName = new SearchColumnStringField[] { new SearchColumnStringField() };
                basicColumns.priceLevel = new SearchColumnSelectField[] { new SearchColumnSelectField() };
                basicColumns.customFieldList = new SearchColumnCustomField[] { new SearchColumnStringCustomField() { scriptId = "custentity_mx_rfc" } };
                basicColumns.terms = new SearchColumnSelectField[] { new SearchColumnSelectField() };
                basicColumns.daysOverdue = new SearchColumnLongField[] { new SearchColumnLongField() };
                basicColumns.entityStatus = new SearchColumnSelectField[] { new SearchColumnSelectField() };
                basicColumns.creditLimit = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };

                Columns.basic = basicColumns;
                //Columns.pricingJoin = pricingJoins;
                //Columns.customSearchJoin = 

                BusquedaClientes.criteria = custsearch;
                BusquedaClientes.columns = Columns;

                service = ConnectionManager.GetNetSuiteService();
                SearchResult result = service.search(BusquedaClientes);

                if (result.status.isSuccess)
                {
                    var tamaño = result.totalPages;

                    for (int n = 1; n <= tamaño; n++)
                    {

                        service = ConnectionManager.GetNetSuiteService();
                        result = service.searchMoreWithId(result.searchId, n);

                        if (result.searchRowList != null && result.searchRowList.Length > 0)
                        {
                            for (int i = 0; i < result.searchRowList.Length; i++)
                            {
                                try
                                {
                                    CustomerSearchRow resultados = (CustomerSearchRow)result.searchRowList[i];

                                    MapClientes registrarCuenta = new MapClientes();

                                    registrarCuenta.InternalID = resultados.basic.internalId[0].searchValue.internalId;
                                    registrarCuenta.ExternalID = resultados.basic.entityId[0].searchValue;

                                    if (resultados.basic.altName != null)
                                    {
                                        registrarCuenta.Nombre = resultados.basic.altName[0].searchValue;
                                    }
                                    else
                                    {
                                        registrarCuenta.Nombre = "Sin Nombre";
                                    }

                                    if (resultados.basic.priceLevel != null)
                                    {
                                        registrarCuenta.Nivel_Precios = resultados.basic.priceLevel[0].searchValue.internalId;
                                    }
                                    else
                                    {
                                        registrarCuenta.Nivel_Precios = "SNP";
                                    }

                                    if (resultados.basic.terms != null)
                                    {
                                        registrarCuenta.Terminos = resultados.basic.terms[0].searchValue.internalId;
                                    }
                                    else
                                    {
                                        registrarCuenta.Terminos = "ST";
                                    }

                                    if (resultados.basic.daysOverdue != null)
                                    {
                                        registrarCuenta.DiasVencimiento = resultados.basic.daysOverdue[0].searchValue.ToString();
                                    }
                                    else
                                    {
                                        registrarCuenta.DiasVencimiento = "0";
                                    }

                                    if (resultados.basic.entityStatus != null)
                                    {
                                        registrarCuenta.Estado = resultados.basic.entityStatus[0].searchValue.internalId;
                                    }
                                    else
                                    {
                                        registrarCuenta.Estado = "Sin Estatus";
                                    }

                                    if (resultados.basic.customFieldList != null)
                                    {
                                        foreach (SearchColumnCustomField customField in resultados.basic.customFieldList)
                                        {
                                            if (customField.scriptId == "custentity_mx_rfc")
                                            {
                                                SearchColumnStringCustomField custField = (SearchColumnStringCustomField)customField;
                                                registrarCuenta.RFC = custField.searchValue;
                                            }

                                        }
                                    }
                                    else
                                    {
                                        registrarCuenta.RFC = "Sin RFC";
                                    }

                                    if (resultados.basic.creditLimit != null)
                                    {
                                        registrarCuenta.limite_Credito = resultados.basic.creditLimit[0].searchValue;
                                    }
                                    else
                                    {
                                        registrarCuenta.limite_Credito = 0;
                                    }

                                    l_clientes.Add(registrarCuenta);

                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }

                            }
                        }

                    }

                    return l_clientes;
                }
                else
                {

                    resp.Estatus = "Fracaso";
                    return l_clientes;
                }

            }
            catch (Exception ex)
            {
                List<MapClientes> l_clientes = new List<MapClientes>();
                return l_clientes;
            }
        }

        #region "ViewBags"
        public List<MapClientes> GetListCustomersViewBag()
        {
            try
            {
                NetSuiteService service = new NetSuiteService();
                RespuestaServicios resp = new RespuestaServicios();
                CustomerSearchAdvanced BusquedaClientes = new CustomerSearchAdvanced();
                List<MapClientes> l_clientes = new List<MapClientes>();

                //search Criteria
                CustomerSearchBasic custbasic = new CustomerSearchBasic();
                CustomerSearch custsearch = new CustomerSearch();
                SearchMultiSelectField field = new SearchMultiSelectField();

                SearchEnumMultiSelectField customerStage = new SearchEnumMultiSelectField();
                customerStage.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
                customerStage.operatorSpecified = true;
                customerStage.searchValue = new string[] { "_customer" };

                field.operatorSpecified = true;
                custbasic.stage = customerStage;
                custsearch.basic = custbasic;

                //Columnas a consultar
                CustomerSearchRow Columns = new CustomerSearchRow();
                CustomerSearchRowBasic basicColumns = new CustomerSearchRowBasic();
                PricingSearchRowBasic pricingJoins = new PricingSearchRowBasic();

                basicColumns.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };
                basicColumns.entityId = new SearchColumnStringField[] { new SearchColumnStringField() };
                basicColumns.altName = new SearchColumnStringField[] { new SearchColumnStringField() };

                Columns.basic = basicColumns;
                //Columns.pricingJoin = pricingJoins;
                //Columns.customSearchJoin = 

                BusquedaClientes.criteria = custsearch;
                BusquedaClientes.columns = Columns;

                service = ConnectionManager.GetNetSuiteService();
                SearchResult result = service.search(BusquedaClientes);

                if (result.status.isSuccess)
                {
                    var tamaño = result.totalPages;

                    for (int n = 1; n <= tamaño; n++)
                    {

                        service = ConnectionManager.GetNetSuiteService();
                        result = service.searchMoreWithId(result.searchId, n);

                        if (result.searchRowList != null && result.searchRowList.Length > 0)
                        {
                            for (int i = 0; i < result.searchRowList.Length; i++)
                            {
                                try
                                {
                                    CustomerSearchRow resultados = (CustomerSearchRow)result.searchRowList[i];

                                    MapClientes registrarCuenta = new MapClientes();

                                    registrarCuenta.InternalID = resultados.basic.internalId[0].searchValue.internalId;
                                    registrarCuenta.ExternalID = resultados.basic.entityId[0].searchValue;

                                    if (resultados.basic.altName != null)
                                    {
                                        registrarCuenta.Nombre = resultados.basic.altName[0].searchValue;
                                    }
                                    else
                                    {
                                        registrarCuenta.Nombre = "Sin Nombre";
                                    }

                                    l_clientes.Add(registrarCuenta);

                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }

                            }
                        }

                    }

                    return l_clientes;
                }
                else
                {

                    resp.Estatus = "Fracaso";
                    return l_clientes;
                }

            }
            catch (Exception ex)
            {
                List<MapClientes> l_clientes = new List<MapClientes>();
                return l_clientes;
            }
        }

        //public List<MapClientes> GetListLocationViewBag()
        //{
        //    try
        //    {
        //        NetSuiteService service = new NetSuiteService();
        //        RespuestaServicios resp = new RespuestaServicios();
        //        LocationSearchAdvanced BusquedaClientes = new LocationSearchAdvanced();
        //        List<MapLocationVB> l_clientes = new List<MapLocationVB>();

        //        //search Criteria
        //        LocationSearchBasic custbasic = new LocationSearchBasic();
        //        LocationSearch custsearch = new LocationSearch();
        //        SearchMultiSelectField field = new SearchMultiSelectField();

        //        SearchEnumMultiSelectField customerStage = new SearchEnumMultiSelectField();
        //        customerStage.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
        //        customerStage.operatorSpecified = true;
        //        customerStage.searchValue = new string[] { "_location" };

        //        field.operatorSpecified = true;
        //        custbasic.locationType = customerStage;
        //        custsearch.basic = custbasic;

        //        //Columnas a consultar
        //        LocationSearchRow Columns = new LocationSearchRow();
        //        LocationSearchRowBasic basicColumns = new LocationSearchRowBasic();

        //        basicColumns.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };
        //        basicColumns.name = new SearchColumnStringField[] { new SearchColumnStringField() };

        //        Columns.basic = basicColumns;

        //        BusquedaClientes.criteria = custsearch;
        //        BusquedaClientes.columns = Columns;

        //        service = ConnectionManager.GetNetSuiteService();
        //        SearchResult result = service.search(BusquedaClientes);

        //        if (result.status.isSuccess)
        //        {
        //            var tamaño = result.totalPages;

        //            for (int n = 1; n <= tamaño; n++)
        //            {

        //                service = ConnectionManager.GetNetSuiteService();
        //                result = service.searchMoreWithId(result.searchId, n);

        //                if (result.searchRowList != null && result.searchRowList.Length > 0)
        //                {
        //                    for (int i = 0; i < result.searchRowList.Length; i++)
        //                    {
        //                        try
        //                        {
        //                            CustomerSearchRow resultados = (CustomerSearchRow)result.searchRowList[i];

        //                            MapClientes registrarCuenta = new MapClientes();

        //                            registrarCuenta.InternalID = resultados.basic.internalId[0].searchValue.internalId;
        //                            registrarCuenta.ExternalID = resultados.basic.entityId[0].searchValue;

        //                            if (resultados.basic.altName != null)
        //                            {
        //                                registrarCuenta.Nombre = resultados.basic.altName[0].searchValue;
        //                            }
        //                            else
        //                            {
        //                                registrarCuenta.Nombre = "Sin Nombre";
        //                            }

        //                            l_clientes.Add(registrarCuenta);

        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            continue;
        //                        }

        //                    }
        //                }

        //            }

        //            return l_clientes;
        //        }
        //        else
        //        {

        //            resp.Estatus = "Fracaso";
        //            return l_clientes;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        List<MapClientes> l_clientes = new List<MapClientes>();
        //        return l_clientes;
        //    }
        //}
        #endregion

        #region "Sales order"
        public RespuestaServicios GetListSalesOrderForLocation(BusquedaCorteCaja model)
        {
            NetSuiteService service = new NetSuiteService();
            RespuestaServicios resp = new RespuestaServicios();
            List<MapSalesOrder> l_RegistrarSO = new List<MapSalesOrder>();

            RecordRef LocationRef = new RecordRef();
            LocationRef.type = RecordType.location;
            LocationRef.internalId = model.Location_ID;
            LocationRef.typeSpecified = true;

            TransactionSearchAdvanced BusquedaTransaccion = new TransactionSearchAdvanced();

            //search Criteria
            TransactionSearchBasic tranbasic = new TransactionSearchBasic();
            TransactionSearch transearch = new TransactionSearch();
            SearchMultiSelectField field = new SearchMultiSelectField();

            SearchDateField searchDate = new SearchDateField();
            searchDate.@operator = SearchDateFieldOperator.within;
            searchDate.searchValue = model.FechaDesde;
            searchDate.searchValue2 = model.FechaHasta;
            searchDate.operatorSpecified = true;
            searchDate.searchValueSpecified = true;
            searchDate.searchValue2Specified = true;

            SearchEnumMultiSelectField searchMultiSelectField = new SearchEnumMultiSelectField();
            searchMultiSelectField.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            searchMultiSelectField.operatorSpecified = true;
            searchMultiSelectField.searchValue = new string[] { "SalesOrd" };

            RecordRef[] binsLocation = new RecordRef[1];
            binsLocation[0] = LocationRef;

            SearchBooleanField mainLines = new SearchBooleanField();
            mainLines.searchValue = false;
            mainLines.searchValueSpecified = true;

            field.operatorSpecified = true;
            field.searchValue = binsLocation;

            tranbasic.location = field;
            tranbasic.dateCreated = searchDate;
            tranbasic.type = searchMultiSelectField;
            tranbasic.mainLine = mainLines;
            transearch.basic = tranbasic;

            TransactionSearchRow Columns = new TransactionSearchRow();
            TransactionSearchRowBasic basicColumns = new TransactionSearchRowBasic();
            LocationSearchRowBasic locationJoins = new LocationSearchRowBasic();
            CustomerSearchRowBasic customerJoins = new CustomerSearchRowBasic();
            TransactionSearchRowBasic billingJoins = new TransactionSearchRowBasic();

            basicColumns.tranId = new SearchColumnStringField[] { new SearchColumnStringField() };
            basicColumns.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.entity = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.dateCreated = new SearchColumnDateField[] { new SearchColumnDateField() };
            basicColumns.createdFrom = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.billingTransaction = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.status = new SearchColumnEnumSelectField[] { new SearchColumnEnumSelectField() };

            locationJoins.name = new SearchColumnStringField[] { new SearchColumnStringField() };
            customerJoins.altName = new SearchColumnStringField[] { new SearchColumnStringField() };
            customerJoins.entityId = new SearchColumnStringField[] { new SearchColumnStringField() };
            billingJoins.tranId = new SearchColumnStringField[] { new SearchColumnStringField() };
            billingJoins.total = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            billingJoins.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };

            Columns.basic = basicColumns;
            Columns.locationJoin = locationJoins;
            Columns.customerJoin = customerJoins;
            Columns.billingTransactionJoin = billingJoins;

            BusquedaTransaccion.criteria = transearch;
            BusquedaTransaccion.columns = Columns;

            service = ConnectionManager.GetNetSuiteService();
            SearchResult res = service.search(BusquedaTransaccion);
            if (res.status.isSuccess)
            {
                var tamaño = res.totalPages;

                if (tamaño <= 1)
                {
                    SearchRow[] searchRows = res.searchRowList;
                    if (searchRows != null && searchRows.Length >= 1)
                    {

                        foreach (TransactionSearchRow rows_Inventory in searchRows)
                        {
                            try
                            {

                                if (rows_Inventory.locationJoin.name != null)
                                {
                                    var iNS = rows_Inventory.basic.internalId[0].searchValue.internalId;

                                    if (l_RegistrarSO.Any(l => l.NS_InternalID == iNS))
                                    {

                                    }
                                    else
                                    {

                                        //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando
                                        MapSalesOrder RegistrarSO = new MapSalesOrder();

                                        RegistrarSO.ClaveCliente = rows_Inventory.customerJoin.entityId[0].searchValue;
                                        RegistrarSO.NombreCliente = rows_Inventory.customerJoin.altName[0].searchValue;
                                        RegistrarSO.NS_ExternalID = rows_Inventory.basic.tranId[0].searchValue;
                                        RegistrarSO.NS_InternalID = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                        RegistrarSO.Mostrador = rows_Inventory.locationJoin.name[0].searchValue.Replace("PRODUCTO TERMINADO : ", "");
                                        RegistrarSO.FechaCreacion = rows_Inventory.basic.dateCreated[0].searchValue;
                                        RegistrarSO.Estatus = rows_Inventory.basic.status[0].searchValue;

                                        if (rows_Inventory.billingTransactionJoin.tranId != null)
                                        {
                                            RegistrarSO.Factura = rows_Inventory.billingTransactionJoin.tranId[0].searchValue;
                                            RegistrarSO.NS_InternalID_Factura = rows_Inventory.billingTransactionJoin.internalId[0].searchValue.internalId;
                                            RegistrarSO.Total_Factura = rows_Inventory.billingTransactionJoin.total[0].searchValue;
                                        }
                                        else
                                        {
                                            RegistrarSO.Factura = "Sin Factura";
                                        }
                                        
                                        
                                        l_RegistrarSO.Add(RegistrarSO);

                                    }
                                }

  
                            }
                            catch (Exception ex)
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        resp.Estatus = "Exito";
                        resp.Valor = l_RegistrarSO;
                        return resp;
                    }
                }

                resp.Estatus = "Exito";
                resp.Valor = l_RegistrarSO;
                return resp;
            }
            else
            {
                resp.Estatus = "Fracaso";
                resp.Mensaje = "Hubo un problema al procesar esta busqueda.";
                return resp;
            }

        }

        public RespuestaServicios GetListSalesOrderForLocationFilters(BusquedaCorteCaja model)
        {
            NetSuiteService service = new NetSuiteService();
            RespuestaServicios resp = new RespuestaServicios();
            List<MapSalesOrder> l_RegistrarSO = new List<MapSalesOrder>();

            TransactionSearchAdvanced BusquedaTransaccion = new TransactionSearchAdvanced();

            //search Criteria
            TransactionSearchBasic tranbasic = new TransactionSearchBasic();
            TransactionSearch transearch = new TransactionSearch();
            SearchMultiSelectField field = new SearchMultiSelectField();

            SearchDateField searchDate = new SearchDateField();
            searchDate.@operator = SearchDateFieldOperator.within;
            searchDate.searchValue = model.FechaDesde;
            searchDate.searchValue2 = model.FechaHasta;
            searchDate.operatorSpecified = true;
            searchDate.searchValueSpecified = true;
            searchDate.searchValue2Specified = true;

            SearchEnumMultiSelectField searchMultiSelectField = new SearchEnumMultiSelectField();
            searchMultiSelectField.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            searchMultiSelectField.operatorSpecified = true;
            searchMultiSelectField.searchValue = new string[] { "SalesOrd" };

            if (model.Location_ID != "")
            {
               RecordRef LocationRef = new RecordRef();
               LocationRef.type = RecordType.location;
               LocationRef.internalId = model.Location_ID;
               LocationRef.typeSpecified = true;

               RecordRef[] binsLocation = new RecordRef[1];
               binsLocation[0] = LocationRef;

               field.operatorSpecified = true;
               field.searchValue = binsLocation;

               tranbasic.location = field;
            }

            //if (model.Estatus != "")
            //{
            //    RecordRef LocationRef = new RecordRef();
            //    LocationRef.type = RecordType.location;
            //    LocationRef.internalId = model.Location_ID;
            //    LocationRef.typeSpecified = true;

            //    SearchEnumMultiSelectField enumStatus = new SearchEnumMultiSelectField();
            //    enumStatus.operatorSpecified = true;
            //    enumStatus.searchValue

            //    tranbasic.status = field;
            //}

            SearchBooleanField mainLines = new SearchBooleanField();
            mainLines.searchValue = false;
            mainLines.searchValueSpecified = true;

            tranbasic.dateCreated = searchDate;
            tranbasic.type = searchMultiSelectField;
            tranbasic.mainLine = mainLines;
            transearch.basic = tranbasic;

            TransactionSearchRow Columns = new TransactionSearchRow();
            TransactionSearchRowBasic basicColumns = new TransactionSearchRowBasic();
            LocationSearchRowBasic locationJoins = new LocationSearchRowBasic();
            CustomerSearchRowBasic customerJoins = new CustomerSearchRowBasic();
            TransactionSearchRowBasic billingJoins = new TransactionSearchRowBasic();

            basicColumns.tranId = new SearchColumnStringField[] { new SearchColumnStringField() };
            basicColumns.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.entity = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.dateCreated = new SearchColumnDateField[] { new SearchColumnDateField() };
            basicColumns.createdFrom = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.billingTransaction = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.status = new SearchColumnEnumSelectField[] { new SearchColumnEnumSelectField() };

            locationJoins.name = new SearchColumnStringField[] { new SearchColumnStringField() };
            customerJoins.altName = new SearchColumnStringField[] { new SearchColumnStringField() };
            customerJoins.entityId = new SearchColumnStringField[] { new SearchColumnStringField() };
            billingJoins.tranId = new SearchColumnStringField[] { new SearchColumnStringField() };
            billingJoins.total = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            billingJoins.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };

            Columns.basic = basicColumns;
            Columns.locationJoin = locationJoins;
            Columns.customerJoin = customerJoins;
            Columns.billingTransactionJoin = billingJoins;

            BusquedaTransaccion.criteria = transearch;
            BusquedaTransaccion.columns = Columns;

            service = ConnectionManager.GetNetSuiteService();
            SearchResult res = service.search(BusquedaTransaccion);
            if (res.status.isSuccess)
            {
                var tamaño = res.totalPages;

                if (tamaño <= 1)
                {
                    SearchRow[] searchRows = res.searchRowList;
                    if (searchRows != null && searchRows.Length >= 1)
                    {

                        foreach (TransactionSearchRow rows_Inventory in searchRows)
                        {
                            try
                            {

                                if (rows_Inventory.locationJoin.name != null)
                                {
                                    var iNS = rows_Inventory.basic.internalId[0].searchValue.internalId;

                                    if (l_RegistrarSO.Any(l => l.NS_InternalID == iNS))
                                    {

                                    }
                                    else
                                    {

                                        //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando
                                        MapSalesOrder RegistrarSO = new MapSalesOrder();

                                        RegistrarSO.ClaveCliente = rows_Inventory.customerJoin.entityId[0].searchValue;
                                        RegistrarSO.NombreCliente = rows_Inventory.customerJoin.altName[0].searchValue;
                                        RegistrarSO.NS_ExternalID = rows_Inventory.basic.tranId[0].searchValue;
                                        RegistrarSO.NS_InternalID = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                        RegistrarSO.Mostrador = rows_Inventory.locationJoin.name[0].searchValue.Replace("PRODUCTO TERMINADO : ", "");
                                        RegistrarSO.FechaCreacion = rows_Inventory.basic.dateCreated[0].searchValue;
                                        RegistrarSO.Fecha = rows_Inventory.basic.dateCreated[0].searchValue.ToString();
                                        RegistrarSO.Estatus = rows_Inventory.basic.status[0].searchValue;

                                        if (rows_Inventory.billingTransactionJoin.tranId != null)
                                        {
                                            RegistrarSO.Factura = rows_Inventory.billingTransactionJoin.tranId[0].searchValue;
                                            RegistrarSO.NS_InternalID_Factura = rows_Inventory.billingTransactionJoin.internalId[0].searchValue.internalId;
                                            RegistrarSO.Total_Factura = rows_Inventory.billingTransactionJoin.total[0].searchValue;
                                        }
                                        else
                                        {
                                            RegistrarSO.Factura = "Sin Factura";
                                        }


                                        l_RegistrarSO.Add(RegistrarSO);

                                    }
                                }


                            }
                            catch (Exception ex)
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        resp.Estatus = "Exito";
                        resp.Valor = l_RegistrarSO;
                        return resp;
                    }
                }
                else
                {
                    for (int n = 1; n <= tamaño; n++)
                    {

                        service = ConnectionManager.GetNetSuiteService();
                        res = service.searchMoreWithId(res.searchId, n);

                        SearchRow[] searchRows = res.searchRowList;
                        if (searchRows != null && searchRows.Length >= 1)
                        {

                            foreach (TransactionSearchRow rows_Inventory in searchRows)
                            {
                                try
                                {

                                    if (rows_Inventory.locationJoin.name != null)
                                    {
                                        var iNS = rows_Inventory.basic.internalId[0].searchValue.internalId;

                                        if (l_RegistrarSO.Any(l => l.NS_InternalID == iNS))
                                        {

                                        }
                                        else
                                        {

                                            //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando
                                            MapSalesOrder RegistrarSO = new MapSalesOrder();

                                            RegistrarSO.ClaveCliente = rows_Inventory.customerJoin.entityId[0].searchValue;
                                            RegistrarSO.NombreCliente = rows_Inventory.customerJoin.altName[0].searchValue;
                                            RegistrarSO.NS_ExternalID = rows_Inventory.basic.tranId[0].searchValue;
                                            RegistrarSO.NS_InternalID = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                            RegistrarSO.Mostrador = rows_Inventory.locationJoin.name[0].searchValue.Replace("PRODUCTO TERMINADO : ", "");
                                            RegistrarSO.FechaCreacion = rows_Inventory.basic.dateCreated[0].searchValue;
                                            RegistrarSO.Fecha = rows_Inventory.basic.dateCreated[0].searchValue.ToString();
                                            RegistrarSO.Estatus = rows_Inventory.basic.status[0].searchValue;

                                            if (rows_Inventory.billingTransactionJoin.tranId != null)
                                            {
                                                RegistrarSO.Factura = rows_Inventory.billingTransactionJoin.tranId[0].searchValue;
                                                RegistrarSO.NS_InternalID_Factura = rows_Inventory.billingTransactionJoin.internalId[0].searchValue.internalId;
                                                RegistrarSO.Total_Factura = rows_Inventory.billingTransactionJoin.total[0].searchValue;
                                            }
                                            else
                                            {
                                                RegistrarSO.Factura = "Sin Factura";
                                            }


                                            l_RegistrarSO.Add(RegistrarSO);

                                        }
                                    }


                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            resp.Estatus = "Exito";
                            resp.Valor = l_RegistrarSO;
                            return resp;
                        }
                    }
                }

                resp.Estatus = "Exito";
                resp.Valor = l_RegistrarSO;
                return resp;
            }
            else
            {
                resp.Estatus = "Fracaso";
                resp.Mensaje = "Hubo un problema al procesar esta busqueda.";
                return resp;
            }

        }
        #endregion
        #region "Estimaciones"
        public RespuestaServicios GetListEstimacionForLocation(BusquedaCorteCaja model)
        {
            NetSuiteService service = new NetSuiteService();
            RespuestaServicios resp = new RespuestaServicios();
            List<MapEstimaciones> l_RegistrarSO = new List<MapEstimaciones>();

            RecordRef LocationRef = new RecordRef();
            LocationRef.type = RecordType.location;
            LocationRef.internalId = model.Location_ID;
            LocationRef.typeSpecified = true;

            TransactionSearchAdvanced BusquedaTransaccion = new TransactionSearchAdvanced();

            //search Criteria
            TransactionSearchBasic tranbasic = new TransactionSearchBasic();
            TransactionSearch transearch = new TransactionSearch();
            SearchMultiSelectField field = new SearchMultiSelectField();

            SearchDateField searchDate = new SearchDateField();
            searchDate.@operator = SearchDateFieldOperator.within;
            searchDate.searchValue = model.FechaDesde;
            searchDate.searchValue2 = model.FechaHasta;
            searchDate.operatorSpecified = true;
            searchDate.searchValueSpecified = true;
            searchDate.searchValue2Specified = true;

            SearchEnumMultiSelectField searchMultiSelectField = new SearchEnumMultiSelectField();
            searchMultiSelectField.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            searchMultiSelectField.operatorSpecified = true;
            searchMultiSelectField.searchValue = new string[] { "Estimate" };

            RecordRef[] binsLocation = new RecordRef[1];
            binsLocation[0] = LocationRef;

            SearchBooleanField mainLines = new SearchBooleanField();
            mainLines.searchValue = false;
            mainLines.searchValueSpecified = true;

            field.operatorSpecified = true;
            field.searchValue = binsLocation;

            tranbasic.location = field;
            tranbasic.dateCreated = searchDate;
            tranbasic.type = searchMultiSelectField;
            tranbasic.mainLine = mainLines;
            transearch.basic = tranbasic;

            TransactionSearchRow Columns = new TransactionSearchRow();
            TransactionSearchRowBasic basicColumns = new TransactionSearchRowBasic();
            LocationSearchRowBasic locationJoins = new LocationSearchRowBasic();
            CustomerSearchRowBasic customerJoins = new CustomerSearchRowBasic();
            TransactionSearchRowBasic billingJoins = new TransactionSearchRowBasic();

            basicColumns.tranId = new SearchColumnStringField[] { new SearchColumnStringField() };
            basicColumns.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.entity = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.dateCreated = new SearchColumnDateField[] { new SearchColumnDateField() };
            basicColumns.createdFrom = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.billingTransaction = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.status = new SearchColumnEnumSelectField[] { new SearchColumnEnumSelectField() };

            locationJoins.name = new SearchColumnStringField[] { new SearchColumnStringField() };
            customerJoins.altName = new SearchColumnStringField[] { new SearchColumnStringField() };
            customerJoins.entityId = new SearchColumnStringField[] { new SearchColumnStringField() };
            billingJoins.tranId = new SearchColumnStringField[] { new SearchColumnStringField() };
            billingJoins.total = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            billingJoins.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };

            Columns.basic = basicColumns;
            Columns.locationJoin = locationJoins;
            Columns.customerJoin = customerJoins;
            //Columns.billingTransactionJoin = billingJoins;

            BusquedaTransaccion.criteria = transearch;
            BusquedaTransaccion.columns = Columns;

            service = ConnectionManager.GetNetSuiteService();
            SearchResult res = service.search(BusquedaTransaccion);
            if (res.status.isSuccess)
            {
                var tamaño = res.totalPages;

                if (tamaño <= 1)
                {
                    SearchRow[] searchRows = res.searchRowList;
                    if (searchRows != null && searchRows.Length >= 1)
                    {

                        foreach (TransactionSearchRow rows_Inventory in searchRows)
                        {
                            try
                            {

                                if (rows_Inventory.locationJoin.name != null)
                                {
                                    var iNS = rows_Inventory.basic.internalId[0].searchValue.internalId;

                                    if (l_RegistrarSO.Any(l => l.NS_InternalID == iNS))
                                    {

                                    }
                                    else
                                    {
                                        if (rows_Inventory.basic.status[0].searchValue != "closed")
                                        {
                                            //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando
                                            MapEstimaciones RegistrarSO = new MapEstimaciones();

                                            RegistrarSO.ClaveCliente = rows_Inventory.customerJoin.entityId[0].searchValue;
                                            RegistrarSO.NombreCliente = rows_Inventory.customerJoin.altName[0].searchValue;
                                            RegistrarSO.NS_ExternalID = rows_Inventory.basic.tranId[0].searchValue;
                                            RegistrarSO.NS_InternalID = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                            RegistrarSO.Mostrador = rows_Inventory.locationJoin.name[0].searchValue.Replace("PRODUCTO TERMINADO : ", "");
                                            RegistrarSO.FechaCreacion = rows_Inventory.basic.dateCreated[0].searchValue;
                                            RegistrarSO.Estatus = rows_Inventory.basic.status[0].searchValue;

                                            //if (rows_Inventory.billingTransactionJoin.tranId != null)
                                            //{
                                            //RegistrarSO.Factura = rows_Inventory.billingTransactionJoin.tranId[0].searchValue;
                                            //RegistrarSO.NS_InternalID_Factura = rows_Inventory.billingTransactionJoin.internalId[0].searchValue.internalId;
                                            //    RegistrarSO.Total_Factura = rows_Inventory.billingTransactionJoin.total[0].searchValue;
                                            //}
                                            //else
                                            //{
                                            //    RegistrarSO.Factura = "Sin Factura";
                                            //}


                                            l_RegistrarSO.Add(RegistrarSO);
                                        }
                                       

                                    }
                                }


                            }
                            catch (Exception ex)
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        resp.Estatus = "Exito";
                        resp.Valor = l_RegistrarSO;
                        return resp;
                    }
                }

                resp.Estatus = "Exito";
                resp.Valor = l_RegistrarSO;
                return resp;
            }
            else
            {
                resp.Estatus = "Fracaso";
                resp.Mensaje = "Hubo un problema al procesar esta busqueda.";
                return resp;
            }

        }
        public RespuestaServicios GetListEstimacionForLocationFilters(BusquedaCorteCaja model)
        {
            NetSuiteService service = new NetSuiteService();
            RespuestaServicios resp = new RespuestaServicios();
            List<MapEstimaciones> l_RegistrarSO = new List<MapEstimaciones>();

            TransactionSearchAdvanced BusquedaTransaccion = new TransactionSearchAdvanced();

            //search Criteria
            TransactionSearchBasic tranbasic = new TransactionSearchBasic();
            TransactionSearch transearch = new TransactionSearch();
            SearchMultiSelectField field = new SearchMultiSelectField();

            SearchDateField searchDate = new SearchDateField();
            searchDate.@operator = SearchDateFieldOperator.within;
            searchDate.searchValue = model.FechaDesde;
            searchDate.searchValue2 = model.FechaHasta;
            searchDate.operatorSpecified = true;
            searchDate.searchValueSpecified = true;
            searchDate.searchValue2Specified = true;

            SearchEnumMultiSelectField searchMultiSelectField = new SearchEnumMultiSelectField();
            searchMultiSelectField.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            searchMultiSelectField.operatorSpecified = true;
            searchMultiSelectField.searchValue = new string[] { "Estimate" };

            if (model.Location_ID != "")
            {
                RecordRef LocationRef = new RecordRef();
                LocationRef.type = RecordType.location;
                LocationRef.internalId = model.Location_ID;
                LocationRef.typeSpecified = true;

                RecordRef[] binsLocation = new RecordRef[1];
                binsLocation[0] = LocationRef;

                field.operatorSpecified = true;
                field.searchValue = binsLocation;

                tranbasic.location = field;
            }

            //if (model.Estatus != "")
            //{
            //    RecordRef LocationRef = new RecordRef();
            //    LocationRef.type = RecordType.location;
            //    LocationRef.internalId = model.Location_ID;
            //    LocationRef.typeSpecified = true;

            //    SearchEnumMultiSelectField enumStatus = new SearchEnumMultiSelectField();
            //    enumStatus.operatorSpecified = true;
            //    enumStatus.searchValue

            //    tranbasic.status = field;
            //}

            SearchBooleanField mainLines = new SearchBooleanField();
            mainLines.searchValue = false;
            mainLines.searchValueSpecified = true;

            tranbasic.dateCreated = searchDate;
            tranbasic.type = searchMultiSelectField;
            tranbasic.mainLine = mainLines;
            transearch.basic = tranbasic;

            TransactionSearchRow Columns = new TransactionSearchRow();
            TransactionSearchRowBasic basicColumns = new TransactionSearchRowBasic();
            LocationSearchRowBasic locationJoins = new LocationSearchRowBasic();
            CustomerSearchRowBasic customerJoins = new CustomerSearchRowBasic();
            TransactionSearchRowBasic billingJoins = new TransactionSearchRowBasic();

            basicColumns.tranId = new SearchColumnStringField[] { new SearchColumnStringField() };
            basicColumns.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.entity = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.dateCreated = new SearchColumnDateField[] { new SearchColumnDateField() };
            basicColumns.createdFrom = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.billingTransaction = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.status = new SearchColumnEnumSelectField[] { new SearchColumnEnumSelectField() };

            locationJoins.name = new SearchColumnStringField[] { new SearchColumnStringField() };
            customerJoins.altName = new SearchColumnStringField[] { new SearchColumnStringField() };
            customerJoins.entityId = new SearchColumnStringField[] { new SearchColumnStringField() };
            billingJoins.tranId = new SearchColumnStringField[] { new SearchColumnStringField() };
            billingJoins.total = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            billingJoins.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };

            Columns.basic = basicColumns;
            Columns.locationJoin = locationJoins;
            Columns.customerJoin = customerJoins;
            //Columns.billingTransactionJoin = billingJoins;

            BusquedaTransaccion.criteria = transearch;
            BusquedaTransaccion.columns = Columns;

            service = ConnectionManager.GetNetSuiteService();
            SearchResult res = service.search(BusquedaTransaccion);
            if (res.status.isSuccess)
            {
                var tamaño = res.totalPages;

                if (tamaño <= 1)
                {
                    SearchRow[] searchRows = res.searchRowList;
                    if (searchRows != null && searchRows.Length >= 1)
                    {

                        foreach (TransactionSearchRow rows_Inventory in searchRows)
                        {
                            try
                            {

                                if (rows_Inventory.locationJoin.name != null)
                                {
                                    var iNS = rows_Inventory.basic.internalId[0].searchValue.internalId;

                                    if (l_RegistrarSO.Any(l => l.NS_InternalID == iNS))
                                    {

                                    }
                                    else
                                    {
                                        if (rows_Inventory.basic.status[0].searchValue != "closed")
                                        {
                                            //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando
                                            MapEstimaciones RegistrarSO = new MapEstimaciones();

                                            RegistrarSO.ClaveCliente = rows_Inventory.customerJoin.entityId[0].searchValue;
                                            RegistrarSO.NombreCliente = rows_Inventory.customerJoin.altName[0].searchValue;
                                            RegistrarSO.NS_ExternalID = rows_Inventory.basic.tranId[0].searchValue;
                                            RegistrarSO.NS_InternalID = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                            RegistrarSO.Mostrador = rows_Inventory.locationJoin.name[0].searchValue.Replace("PRODUCTO TERMINADO : ", "");
                                            RegistrarSO.FechaCreacion = rows_Inventory.basic.dateCreated[0].searchValue;
                                            RegistrarSO.Fecha = rows_Inventory.basic.dateCreated[0].searchValue.ToString();
                                            RegistrarSO.Estatus = rows_Inventory.basic.status[0].searchValue;

                                            l_RegistrarSO.Add(RegistrarSO);
                                        }
                                    }
                                }


                            }
                            catch (Exception ex)
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        resp.Estatus = "Exito";
                        resp.Valor = l_RegistrarSO;
                        return resp;
                    }
                }
                else
                {
                    for (int n = 1; n <= tamaño; n++)
                    {

                        service = ConnectionManager.GetNetSuiteService();
                        res = service.searchMoreWithId(res.searchId, n);

                        SearchRow[] searchRows = res.searchRowList;
                        if (searchRows != null && searchRows.Length >= 1)
                        {

                            foreach (TransactionSearchRow rows_Inventory in searchRows)
                            {
                                try
                                {

                                    if (rows_Inventory.locationJoin.name != null)
                                    {
                                        var iNS = rows_Inventory.basic.internalId[0].searchValue.internalId;

                                        if (l_RegistrarSO.Any(l => l.NS_InternalID == iNS))
                                        {

                                        }
                                        else
                                        {

                                            //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando
                                            MapEstimaciones RegistrarSO = new MapEstimaciones();

                                            RegistrarSO.ClaveCliente = rows_Inventory.customerJoin.entityId[0].searchValue;
                                            RegistrarSO.NombreCliente = rows_Inventory.customerJoin.altName[0].searchValue;
                                            RegistrarSO.NS_ExternalID = rows_Inventory.basic.tranId[0].searchValue;
                                            RegistrarSO.NS_InternalID = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                            RegistrarSO.Mostrador = rows_Inventory.locationJoin.name[0].searchValue.Replace("PRODUCTO TERMINADO : ", "");
                                            RegistrarSO.FechaCreacion = rows_Inventory.basic.dateCreated[0].searchValue;
                                            RegistrarSO.Fecha = rows_Inventory.basic.dateCreated[0].searchValue.ToString();
                                            //RegistrarSO.Estatus = rows_Inventory.basic.status[0].searchValue;

                                            //if (rows_Inventory.billingTransactionJoin.tranId != null)
                                            //{
                                            //    RegistrarSO.Factura = rows_Inventory.billingTransactionJoin.tranId[0].searchValue;
                                            //    RegistrarSO.NS_InternalID_Factura = rows_Inventory.billingTransactionJoin.internalId[0].searchValue.internalId;
                                            //    RegistrarSO.Total_Factura = rows_Inventory.billingTransactionJoin.total[0].searchValue;
                                            //}
                                            //else
                                            //{
                                            //    RegistrarSO.Factura = "Sin Factura";
                                            //}


                                            l_RegistrarSO.Add(RegistrarSO);

                                        }
                                    }


                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            resp.Estatus = "Exito";
                            resp.Valor = l_RegistrarSO;
                            return resp;
                        }
                    }
                }

                resp.Estatus = "Exito";
                resp.Valor = l_RegistrarSO;
                return resp;
            }
            else
            {
                resp.Estatus = "Fracaso";
                resp.Mensaje = "Hubo un problema al procesar esta busqueda.";
                return resp;
            }

        }
        #endregion
        #region "Negadas"
        public RespuestaServicios GetNegadasForLocation(BusquedaCorteCaja model)
        {
            NetSuiteService service = new NetSuiteService();
            RespuestaServicios resp = new RespuestaServicios();
            List<MapNegadas> l_RegistrarSO = new List<MapNegadas>();

            RecordRef LocationRef = new RecordRef();
            LocationRef.type = RecordType.location;
            LocationRef.internalId = model.Location_ID;
            LocationRef.typeSpecified = true;

            TransactionSearchAdvanced BusquedaTransaccion = new TransactionSearchAdvanced();

            //search Criteria
            TransactionSearchBasic tranbasic = new TransactionSearchBasic();
            TransactionSearch transearch = new TransactionSearch();
            SearchMultiSelectField field = new SearchMultiSelectField();

            SearchDateField searchDate = new SearchDateField();
            searchDate.@operator = SearchDateFieldOperator.within;
            searchDate.searchValue = model.FechaDesde;
            searchDate.searchValue2 = model.FechaHasta;
            searchDate.operatorSpecified = true;
            searchDate.searchValueSpecified = true;
            searchDate.searchValue2Specified = true;

            SearchEnumMultiSelectField searchMultiSelectField = new SearchEnumMultiSelectField();
            searchMultiSelectField.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            searchMultiSelectField.operatorSpecified = true;
            searchMultiSelectField.searchValue = new string[] { "Estimate" };

            RecordRef[] binsLocation = new RecordRef[1];
            binsLocation[0] = LocationRef;

            SearchBooleanField mainLines = new SearchBooleanField();
            mainLines.searchValue = false;
            mainLines.searchValueSpecified = true;

            field.operatorSpecified = true;
            field.searchValue = binsLocation;

            tranbasic.location = field;
            tranbasic.dateCreated = searchDate;
            tranbasic.type = searchMultiSelectField;
            tranbasic.mainLine = mainLines;
            transearch.basic = tranbasic;

            TransactionSearchRow Columns = new TransactionSearchRow();
            TransactionSearchRowBasic basicColumns = new TransactionSearchRowBasic();
            LocationSearchRowBasic locationJoins = new LocationSearchRowBasic();
            CustomerSearchRowBasic customerJoins = new CustomerSearchRowBasic();
            TransactionSearchRowBasic billingJoins = new TransactionSearchRowBasic();

            basicColumns.tranId = new SearchColumnStringField[] { new SearchColumnStringField() };
            basicColumns.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.entity = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.dateCreated = new SearchColumnDateField[] { new SearchColumnDateField() };
            basicColumns.createdFrom = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.billingTransaction = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.status = new SearchColumnEnumSelectField[] { new SearchColumnEnumSelectField() };

            locationJoins.name = new SearchColumnStringField[] { new SearchColumnStringField() };
            customerJoins.altName = new SearchColumnStringField[] { new SearchColumnStringField() };
            customerJoins.entityId = new SearchColumnStringField[] { new SearchColumnStringField() };
            billingJoins.tranId = new SearchColumnStringField[] { new SearchColumnStringField() };
            billingJoins.total = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            billingJoins.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };

            Columns.basic = basicColumns;
            Columns.locationJoin = locationJoins;
            Columns.customerJoin = customerJoins;
            //Columns.billingTransactionJoin = billingJoins;

            BusquedaTransaccion.criteria = transearch;
            BusquedaTransaccion.columns = Columns;

            service = ConnectionManager.GetNetSuiteService();
            SearchResult res = service.search(BusquedaTransaccion);
            if (res.status.isSuccess)
            {
                var tamaño = res.totalPages;

                if (tamaño <= 1)
                {
                    SearchRow[] searchRows = res.searchRowList;
                    if (searchRows != null && searchRows.Length >= 1)
                    {

                        foreach (TransactionSearchRow rows_Inventory in searchRows)
                        {
                            try
                            {

                                if (rows_Inventory.locationJoin.name != null)
                                {
                                    var iNS = rows_Inventory.basic.internalId[0].searchValue.internalId;

                                    if (l_RegistrarSO.Any(l => l.NS_InternalID == iNS))
                                    {

                                    }
                                    else
                                    {
                                        if (rows_Inventory.basic.status[0].searchValue == "closed")
                                        {
                                            //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando
                                            MapNegadas RegistrarSO = new MapNegadas();

                                            RegistrarSO.ClaveCliente = rows_Inventory.customerJoin.entityId[0].searchValue;
                                            RegistrarSO.NombreCliente = rows_Inventory.customerJoin.altName[0].searchValue;
                                            RegistrarSO.NS_ExternalID = rows_Inventory.basic.tranId[0].searchValue;
                                            RegistrarSO.NS_InternalID = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                            RegistrarSO.Mostrador = rows_Inventory.locationJoin.name[0].searchValue.Replace("PRODUCTO TERMINADO : ", "");
                                            RegistrarSO.FechaCreacion = rows_Inventory.basic.dateCreated[0].searchValue;
                                            RegistrarSO.Estatus = rows_Inventory.basic.status[0].searchValue;

                                            l_RegistrarSO.Add(RegistrarSO);
                                        }
                                    }
                                }


                            }
                            catch (Exception ex)
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        resp.Estatus = "Exito";
                        resp.Valor = l_RegistrarSO;
                        return resp;
                    }
                }

                resp.Estatus = "Exito";
                resp.Valor = l_RegistrarSO;
                return resp;
            }
            else
            {
                resp.Estatus = "Fracaso";
                resp.Mensaje = "Hubo un problema al procesar esta busqueda.";
                return resp;
            }

        }
        public RespuestaServicios GetListNegadasForLocationFilters(BusquedaCorteCaja model)
        {
            NetSuiteService service = new NetSuiteService();
            RespuestaServicios resp = new RespuestaServicios();
            List<MapNegadas> l_RegistrarSO = new List<MapNegadas>();

            TransactionSearchAdvanced BusquedaTransaccion = new TransactionSearchAdvanced();

            //search Criteria
            TransactionSearchBasic tranbasic = new TransactionSearchBasic();
            TransactionSearch transearch = new TransactionSearch();
            SearchMultiSelectField field = new SearchMultiSelectField();

            SearchDateField searchDate = new SearchDateField();
            searchDate.@operator = SearchDateFieldOperator.within;
            searchDate.searchValue = model.FechaDesde;
            searchDate.searchValue2 = model.FechaHasta;
            searchDate.operatorSpecified = true;
            searchDate.searchValueSpecified = true;
            searchDate.searchValue2Specified = true;

            SearchEnumMultiSelectField searchMultiSelectField = new SearchEnumMultiSelectField();
            searchMultiSelectField.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            searchMultiSelectField.operatorSpecified = true;
            searchMultiSelectField.searchValue = new string[] { "Estimate" };

            if (model.Location_ID != "")
            {
                RecordRef LocationRef = new RecordRef();
                LocationRef.type = RecordType.location;
                LocationRef.internalId = model.Location_ID;
                LocationRef.typeSpecified = true;

                RecordRef[] binsLocation = new RecordRef[1];
                binsLocation[0] = LocationRef;

                field.operatorSpecified = true;
                field.searchValue = binsLocation;

                tranbasic.location = field;
            }

            SearchBooleanField mainLines = new SearchBooleanField();
            mainLines.searchValue = false;
            mainLines.searchValueSpecified = true;

            tranbasic.dateCreated = searchDate;
            tranbasic.type = searchMultiSelectField;
            tranbasic.mainLine = mainLines;
            transearch.basic = tranbasic;

            TransactionSearchRow Columns = new TransactionSearchRow();
            TransactionSearchRowBasic basicColumns = new TransactionSearchRowBasic();
            LocationSearchRowBasic locationJoins = new LocationSearchRowBasic();
            CustomerSearchRowBasic customerJoins = new CustomerSearchRowBasic();
            TransactionSearchRowBasic billingJoins = new TransactionSearchRowBasic();

            basicColumns.tranId = new SearchColumnStringField[] { new SearchColumnStringField() };
            basicColumns.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.entity = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.dateCreated = new SearchColumnDateField[] { new SearchColumnDateField() };
            basicColumns.createdFrom = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.billingTransaction = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.status = new SearchColumnEnumSelectField[] { new SearchColumnEnumSelectField() };

            locationJoins.name = new SearchColumnStringField[] { new SearchColumnStringField() };
            customerJoins.altName = new SearchColumnStringField[] { new SearchColumnStringField() };
            customerJoins.entityId = new SearchColumnStringField[] { new SearchColumnStringField() };
            billingJoins.tranId = new SearchColumnStringField[] { new SearchColumnStringField() };
            billingJoins.total = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            billingJoins.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };

            Columns.basic = basicColumns;
            Columns.locationJoin = locationJoins;
            Columns.customerJoin = customerJoins;
            //Columns.billingTransactionJoin = billingJoins;

            BusquedaTransaccion.criteria = transearch;
            BusquedaTransaccion.columns = Columns;

            service = ConnectionManager.GetNetSuiteService();
            SearchResult res = service.search(BusquedaTransaccion);
            if (res.status.isSuccess)
            {
                var tamaño = res.totalPages;

                if (tamaño <= 1)
                {
                    SearchRow[] searchRows = res.searchRowList;
                    if (searchRows != null && searchRows.Length >= 1)
                    {

                        foreach (TransactionSearchRow rows_Inventory in searchRows)
                        {
                            try
                            {

                                if (rows_Inventory.locationJoin.name != null)
                                {
                                    var iNS = rows_Inventory.basic.internalId[0].searchValue.internalId;

                                    if (l_RegistrarSO.Any(l => l.NS_InternalID == iNS))
                                    {

                                    }
                                    else
                                    {
                                        if (rows_Inventory.basic.status[0].searchValue == "closed")
                                        {
                                            //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando
                                            MapNegadas RegistrarSO = new MapNegadas();

                                            RegistrarSO.ClaveCliente = rows_Inventory.customerJoin.entityId[0].searchValue;
                                            RegistrarSO.NombreCliente = rows_Inventory.customerJoin.altName[0].searchValue;
                                            RegistrarSO.NS_ExternalID = rows_Inventory.basic.tranId[0].searchValue;
                                            RegistrarSO.NS_InternalID = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                            RegistrarSO.Mostrador = rows_Inventory.locationJoin.name[0].searchValue.Replace("PRODUCTO TERMINADO : ", "");
                                            RegistrarSO.FechaCreacion = rows_Inventory.basic.dateCreated[0].searchValue;
                                            RegistrarSO.Estatus = rows_Inventory.basic.status[0].searchValue;

                                            l_RegistrarSO.Add(RegistrarSO);
                                        }

                                    }
                                }


                            }
                            catch (Exception ex)
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        resp.Estatus = "Exito";
                        resp.Valor = l_RegistrarSO;
                        return resp;
                    }
                }
                else
                {
                    for (int n = 1; n <= tamaño; n++)
                    {

                        service = ConnectionManager.GetNetSuiteService();
                        res = service.searchMoreWithId(res.searchId, n);

                        SearchRow[] searchRows = res.searchRowList;
                        if (searchRows != null && searchRows.Length >= 1)
                        {

                            foreach (TransactionSearchRow rows_Inventory in searchRows)
                            {
                                try
                                {

                                    if (rows_Inventory.locationJoin.name != null)
                                    {
                                        var iNS = rows_Inventory.basic.internalId[0].searchValue.internalId;

                                        if (l_RegistrarSO.Any(l => l.NS_InternalID == iNS))
                                        {

                                        }
                                        else
                                        {

                                            if (rows_Inventory.basic.status[0].searchValue == "closed")
                                            {
                                                //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando
                                                MapNegadas RegistrarSO = new MapNegadas();

                                                RegistrarSO.ClaveCliente = rows_Inventory.customerJoin.entityId[0].searchValue;
                                                RegistrarSO.NombreCliente = rows_Inventory.customerJoin.altName[0].searchValue;
                                                RegistrarSO.NS_ExternalID = rows_Inventory.basic.tranId[0].searchValue;
                                                RegistrarSO.NS_InternalID = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                                RegistrarSO.Mostrador = rows_Inventory.locationJoin.name[0].searchValue.Replace("PRODUCTO TERMINADO : ", "");
                                                RegistrarSO.FechaCreacion = rows_Inventory.basic.dateCreated[0].searchValue;
                                                RegistrarSO.Estatus = rows_Inventory.basic.status[0].searchValue;

                                                l_RegistrarSO.Add(RegistrarSO);
                                            }

                                        }
                                    }


                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            resp.Estatus = "Exito";
                            resp.Valor = l_RegistrarSO;
                            return resp;
                        }
                    }
                }

                resp.Estatus = "Exito";
                resp.Valor = l_RegistrarSO;
                return resp;
            }
            else
            {
                resp.Estatus = "Fracaso";
                resp.Mensaje = "Hubo un problema al procesar esta busqueda.";
                return resp;
            }

        }
        #endregion

        #region "Invoice"
        public string GetTranIDInvoice(string docid)
        {
            try
            {
                NetSuiteService service = new NetSuiteService();
                RespuestaServicios resp = new RespuestaServicios();
                List<Consulta_Pagos> l_PagosRegistrados = new List<Consulta_Pagos>();

                RecordRef LocationRef = new RecordRef();
                LocationRef.type = RecordType.invoice;
                LocationRef.internalId = docid;
                LocationRef.typeSpecified = true;

                TransactionSearchAdvanced BusquedaTransaccion = new TransactionSearchAdvanced();

                //search Criteria
                TransactionSearchBasic tranbasic = new TransactionSearchBasic();
                TransactionSearch transearch = new TransactionSearch();
                SearchMultiSelectField field = new SearchMultiSelectField();

                SearchEnumMultiSelectField searchMultiSelectField = new SearchEnumMultiSelectField();
                searchMultiSelectField.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
                searchMultiSelectField.operatorSpecified = true;
                searchMultiSelectField.searchValue = new string[] { "CustInvc" };

                SearchBooleanField searchlinemain = new SearchBooleanField();
                searchlinemain.searchValue = true;
                searchlinemain.searchValueSpecified = true;

                RecordRef[] binsLocation = new RecordRef[1];
                binsLocation[0] = LocationRef;

                field.operatorSpecified = true;
                field.searchValue = binsLocation;

                tranbasic.internalId = field;
                tranbasic.type = searchMultiSelectField;
                tranbasic.mainLine = searchlinemain;
                transearch.basic = tranbasic;

                TransactionSearchRow Columns = new TransactionSearchRow();
                TransactionSearchRowBasic basicColumns = new TransactionSearchRowBasic();

                basicColumns.tranId = new SearchColumnStringField[] { new SearchColumnStringField() };

                Columns.basic = basicColumns;

                BusquedaTransaccion.criteria = transearch;
                BusquedaTransaccion.columns = Columns;

                service = ConnectionManager.GetNetSuiteService();
                SearchResult res = service.search(BusquedaTransaccion);
                if (res.status.isSuccess)
                {
                    SearchRow[] searchRows = res.searchRowList;
                    if (searchRows != null && searchRows.Length >= 1)
                    {

                        foreach (TransactionSearchRow rows_Inventory in searchRows)
                        {
                            try
                            {
                                //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando
                                var tranID = rows_Inventory.basic.tranId[0].searchValue;

                                return tranID;
                            }
                            catch (Exception ex)
                            {
                                continue;
                            }


                        }

                        return "";

                    }
                    else
                    {
                        return "";
                    }
                }


                return "";

            }
            catch (Exception ex)
            {
                return "";
            }
        }
        #endregion

        #region "Kardex Articulos"

        public RespuestaServicios GetListKardexForLocation(BusquedaKardex model)
        {
            NetSuiteService service = new NetSuiteService();
            RespuestaServicios resp = new RespuestaServicios();
            List<MapSalesOrder> l_RegistrarSO = new List<MapSalesOrder>();

            TransactionSearchAdvanced BusquedaTransaccion = new TransactionSearchAdvanced();

            //search Criteria
            TransactionSearchBasic tranbasic = new TransactionSearchBasic();
            TransactionSearch transearch = new TransactionSearch();
            SearchMultiSelectField field = new SearchMultiSelectField();

            SearchDateField searchDate = new SearchDateField();
            searchDate.@operator = SearchDateFieldOperator.within;
            searchDate.searchValue = model.FechaDesde;
            searchDate.searchValue2 = model.FechaHasta;
            searchDate.operatorSpecified = true;
            searchDate.searchValueSpecified = true;
            searchDate.searchValue2Specified = true;

            SearchEnumMultiSelectField searchMultiSelectField = new SearchEnumMultiSelectField();
            searchMultiSelectField.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            searchMultiSelectField.operatorSpecified = true;
            searchMultiSelectField.searchValue = new string[] { "Assembly", "InvtPart", "Kit" };

            SearchEnumMultiSelectField searhMultiselectineTipe = new SearchEnumMultiSelectField();
            searchMultiSelectField.@operator = SearchEnumMultiSelectFieldOperator.noneOf;
            searchMultiSelectField.operatorSpecified = true;
            searchMultiSelectField.searchValue = new string[] { "WIP"};

            RecordRef LocationRef = new RecordRef();
            LocationRef.type = RecordType.location;
            LocationRef.internalId = model.Location_ID;
            LocationRef.typeSpecified = true;

            RecordRef[] binsLocation = new RecordRef[1];
            binsLocation[0] = LocationRef;

            field.operatorSpecified = true;
            field.searchValue = binsLocation;

            //SearchBooleanField mainLines = new SearchBooleanField();
            //mainLines.searchValue = false;
            //mainLines.searchValueSpecified = true;

            tranbasic.dateCreated = searchDate;
            tranbasic.location = field;
            tranbasic.transactionLineType = searchMultiSelectField;
            tranbasic.type = searchMultiSelectField;
            //tranbasic.mainLine = mainLines;
            transearch.basic = tranbasic;

            TransactionSearchRow Columns = new TransactionSearchRow();
            TransactionSearchRowBasic basicColumns = new TransactionSearchRowBasic();
            LocationSearchRowBasic locationJoins = new LocationSearchRowBasic();
            CustomerSearchRowBasic customerJoins = new CustomerSearchRowBasic();
            TransactionSearchRowBasic billingJoins = new TransactionSearchRowBasic();
            AccountingPeriodSearchRowBasic acountingJoins = new AccountingPeriodSearchRowBasic();
            ItemSearchRowBasic itemJoins = new ItemSearchRowBasic();

            basicColumns.tranDate = new SearchColumnDateField[] { new SearchColumnDateField() };
            basicColumns.postingPeriod = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.type = new SearchColumnEnumSelectField[] { new SearchColumnEnumSelectField() };
            basicColumns.tranId = new SearchColumnStringField[] { new SearchColumnStringField() };
            basicColumns.entity = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.memo = new SearchColumnStringField[] { new SearchColumnStringField() };
            basicColumns.amount = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.quantity = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            basicColumns.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };

            locationJoins.name = new SearchColumnStringField[] { new SearchColumnStringField() };
            customerJoins.altName = new SearchColumnStringField[] { new SearchColumnStringField() };
            customerJoins.entityId = new SearchColumnStringField[] { new SearchColumnStringField() };
            billingJoins.tranId = new SearchColumnStringField[] { new SearchColumnStringField() };
            billingJoins.total = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };
            billingJoins.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            acountingJoins.periodName = new SearchColumnStringField[] { new SearchColumnStringField() };
            itemJoins.displayName= new SearchColumnStringField[] { new SearchColumnStringField() };


            Columns.basic = basicColumns;
            Columns.locationJoin = locationJoins;
            Columns.customerJoin = customerJoins;
            Columns.billingTransactionJoin = billingJoins;
            Columns.accountingPeriodJoin = acountingJoins;
            Columns.itemJoin = itemJoins;

            BusquedaTransaccion.criteria = transearch;
            BusquedaTransaccion.columns = Columns;

            service = ConnectionManager.GetNetSuiteService();
            SearchResult res = service.search(BusquedaTransaccion);
            if (res.status.isSuccess)
            {
                var tamaño = res.totalPages;

                if (tamaño <= 1)
                {
                    SearchRow[] searchRows = res.searchRowList;
                    if (searchRows != null && searchRows.Length >= 1)
                    {

                        foreach (TransactionSearchRow rows_Inventory in searchRows)
                        {
                            try
                            {
                                var iNS = rows_Inventory.basic.internalId[0].searchValue.internalId;

                                if (l_RegistrarSO.Any(l => l.NS_InternalID == iNS))
                                {

                                }
                                else
                                {

                                    //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando
                                    MapKardexArticulos RegistrarSO = new MapKardexArticulos();

                                    RegistrarSO.Fecha = rows_Inventory.basic.tranDate[0].searchValue;

                                    if (rows_Inventory.accountingPeriodJoin != null)
                                    {
                                        RegistrarSO.Periodo = rows_Inventory.accountingPeriodJoin.periodName[0].searchValue;
                                    }
                                    else
                                    {
                                        RegistrarSO.Periodo = "Sin Periodo";
                                    }


                                    RegistrarSO.TipoTransaccion = rows_Inventory.basic.type[0].searchValue;
                                    RegistrarSO.Articulo = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                   // RegistrarSO.Mostrador = rows_Inventory.locationJoin.name[0].searchValue.Replace("PRODUCTO TERMINADO : ", "");
                                   // RegistrarSO.FechaCreacion = rows_Inventory.basic.dateCreated[0].searchValue;
                                   // RegistrarSO.Fecha = rows_Inventory.basic.dateCreated[0].searchValue.ToString();
                                   // RegistrarSO.Estatus = rows_Inventory.basic.status[0].searchValue;

                                   //if (rows_Inventory.billingTransactionJoin.tranId != null)
                                   //{
                                   //   RegistrarSO.Factura = rows_Inventory.billingTransactionJoin.tranId[0].searchValue;
                                   //   RegistrarSO.NS_InternalID_Factura = rows_Inventory.billingTransactionJoin.internalId[0].searchValue.internalId;
                                   //   RegistrarSO.Total_Factura = rows_Inventory.billingTransactionJoin.total[0].searchValue;
                                   //}
                                   //else
                                   //{
                                   //   RegistrarSO.Factura = "Sin Factura";
                                   //}


                                   // l_RegistrarSO.Add(RegistrarSO);

                                }
                                


                            }
                            catch (Exception ex)
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        resp.Estatus = "Exito";
                        resp.Valor = l_RegistrarSO;
                        return resp;
                    }
                }
                else
                {
                    for (int n = 1; n <= tamaño; n++)
                    {

                        service = ConnectionManager.GetNetSuiteService();
                        res = service.searchMoreWithId(res.searchId, n);

                        SearchRow[] searchRows = res.searchRowList;
                        if (searchRows != null && searchRows.Length >= 1)
                        {

                            foreach (TransactionSearchRow rows_Inventory in searchRows)
                            {
                                try
                                {

                                    if (rows_Inventory.locationJoin.name != null)
                                    {
                                        var iNS = rows_Inventory.basic.internalId[0].searchValue.internalId;

                                        if (l_RegistrarSO.Any(l => l.NS_InternalID == iNS))
                                        {

                                        }
                                        else
                                        {

                                            //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando
                                            MapSalesOrder RegistrarSO = new MapSalesOrder();

                                            //RegistrarSO.ClaveCliente = rows_Inventory.customerJoin.entityId[0].searchValue;
                                            //RegistrarSO.NombreCliente = rows_Inventory.customerJoin.altName[0].searchValue;
                                            //RegistrarSO.NS_ExternalID = rows_Inventory.basic.tranId[0].searchValue;
                                            //RegistrarSO.NS_InternalID = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                            //RegistrarSO.Mostrador = rows_Inventory.locationJoin.name[0].searchValue.Replace("PRODUCTO TERMINADO : ", "");
                                            //RegistrarSO.FechaCreacion = rows_Inventory.basic.dateCreated[0].searchValue;
                                            //RegistrarSO.Fecha = rows_Inventory.basic.dateCreated[0].searchValue.ToString();
                                            //RegistrarSO.Estatus = rows_Inventory.basic.status[0].searchValue;

                                            //if (rows_Inventory.billingTransactionJoin.tranId != null)
                                            //{
                                            //    RegistrarSO.Factura = rows_Inventory.billingTransactionJoin.tranId[0].searchValue;
                                            //    RegistrarSO.NS_InternalID_Factura = rows_Inventory.billingTransactionJoin.internalId[0].searchValue.internalId;
                                            //    RegistrarSO.Total_Factura = rows_Inventory.billingTransactionJoin.total[0].searchValue;
                                            //}
                                            //else
                                            //{
                                            //    RegistrarSO.Factura = "Sin Factura";
                                            //}


                                            //l_RegistrarSO.Add(RegistrarSO);

                                        }
                                    }


                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            resp.Estatus = "Exito";
                            resp.Valor = l_RegistrarSO;
                            return resp;
                        }
                    }
                }

                resp.Estatus = "Exito";
                resp.Valor = l_RegistrarSO;
                return resp;
            }
            else
            {
                resp.Estatus = "Fracaso";
                resp.Mensaje = "Hubo un problema al procesar esta busqueda.";
                return resp;
            }

        }
        #endregion

        #region "Entrega"
        public RespuestaServicios PruebasProton(BusquedaCorteCaja model)
        {
            NetSuiteService service = new NetSuiteService();
            RespuestaServicios resp = new RespuestaServicios();
            List<MapSalesOrder> l_RegistrarSO = new List<MapSalesOrder>();

            TransactionSearchAdvanced BusquedaTransaccion = new TransactionSearchAdvanced();

            //search Criteria
            TransactionSearchBasic tranbasic = new TransactionSearchBasic();
            TransactionSearch transearch = new TransactionSearch();


            SearchMultiSelectField field = new SearchMultiSelectField();

            SearchEnumMultiSelectField searchMultiSelectField = new SearchEnumMultiSelectField();
            searchMultiSelectField.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            searchMultiSelectField.operatorSpecified = true;
            searchMultiSelectField.searchValue = new string[] { "CustInvc", "SalesOrd" };

            RecordRef LocationRef = new RecordRef();
            LocationRef.type = RecordType.location;
            LocationRef.internalId = model.Location_ID;
            LocationRef.typeSpecified = true;

            RecordRef[] binsLocation = new RecordRef[1];
            binsLocation[0] = LocationRef;

            SearchBooleanField mainLines = new SearchBooleanField();
            mainLines.searchValue = true;
            mainLines.searchValueSpecified = true;

            field.operatorSpecified = true;
            field.searchValue = binsLocation;

            tranbasic.location = field;
            tranbasic.type = searchMultiSelectField;
            tranbasic.mainLine = mainLines;
            transearch.basic = tranbasic;

            TransactionSearchRow Columns = new TransactionSearchRow();
            TransactionSearchRowBasic basicColumns = new TransactionSearchRowBasic();
            LocationSearchRowBasic locationJoins = new LocationSearchRowBasic();
            CustomerSearchRowBasic customerJoins = new CustomerSearchRowBasic();

            basicColumns.tranDate = new SearchColumnDateField[] { new SearchColumnDateField() };
            basicColumns.type = new SearchColumnEnumSelectField[] { new SearchColumnEnumSelectField() };
            basicColumns.entity = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.account = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.memo = new SearchColumnStringField[] { new SearchColumnStringField() };
            basicColumns.amount = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };

            locationJoins.name = new SearchColumnStringField[] { new SearchColumnStringField() };
            customerJoins.altName = new SearchColumnStringField[] { new SearchColumnStringField() };

            Columns.basic = basicColumns;
            Columns.locationJoin = locationJoins;
            Columns.customerJoin = customerJoins;

            BusquedaTransaccion.criteria = transearch;
            BusquedaTransaccion.columns = Columns;

            service = ConnectionManager.GetNetSuiteService();
            SearchResult res = service.search(BusquedaTransaccion);
            if (res.status.isSuccess)
            {
                var tamaño = res.totalPages;

                if (tamaño <= 1)
                {
                    SearchRow[] searchRows = res.searchRowList;
                    if (searchRows != null && searchRows.Length >= 1)
                    {

                        foreach (TransactionSearchRow rows_Inventory in searchRows)
                        {
                            try
                            {

                                if (rows_Inventory.locationJoin.name != null)
                                {
                                    var iNS = rows_Inventory.basic.internalId[0].searchValue.internalId;

                                    if (l_RegistrarSO.Any(l => l.NS_InternalID == iNS))
                                    {

                                    }
                                    else
                                    {

                                        //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando
                                        MapSalesOrder RegistrarSO = new MapSalesOrder();

                                        RegistrarSO.ClaveCliente = rows_Inventory.customerJoin.entityId[0].searchValue;
                                        RegistrarSO.NombreCliente = rows_Inventory.customerJoin.altName[0].searchValue;
                                        RegistrarSO.NS_ExternalID = rows_Inventory.basic.tranId[0].searchValue;
                                        RegistrarSO.NS_InternalID = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                        RegistrarSO.Mostrador = rows_Inventory.locationJoin.name[0].searchValue.Replace("PRODUCTO TERMINADO : ", "");
                                        RegistrarSO.FechaCreacion = rows_Inventory.basic.dateCreated[0].searchValue;
                                        RegistrarSO.Estatus = rows_Inventory.basic.status[0].searchValue;

                                        if (rows_Inventory.billingTransactionJoin.tranId != null)
                                        {
                                            RegistrarSO.Factura = rows_Inventory.billingTransactionJoin.tranId[0].searchValue;
                                            RegistrarSO.NS_InternalID_Factura = rows_Inventory.billingTransactionJoin.internalId[0].searchValue.internalId;
                                            RegistrarSO.Total_Factura = rows_Inventory.billingTransactionJoin.total[0].searchValue;
                                        }
                                        else
                                        {
                                            RegistrarSO.Factura = "Sin Factura";
                                        }


                                        l_RegistrarSO.Add(RegistrarSO);

                                    }
                                }


                            }
                            catch (Exception ex)
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        resp.Estatus = "Exito";
                        resp.Valor = l_RegistrarSO;
                        return resp;
                    }
                }

                resp.Estatus = "Exito";
                resp.Valor = l_RegistrarSO;
                return resp;
            }
            else
            {
                resp.Estatus = "Fracaso";
                resp.Mensaje = "Hubo un problema al procesar esta busqueda.";
                return resp;
            }

        }
        #endregion
    }

}
