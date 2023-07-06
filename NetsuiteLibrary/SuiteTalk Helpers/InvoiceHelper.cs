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

    public class InvoiceHelper
    {
        public Invoice GetInvoiceByID(string id)
        {
            NetSuiteService service;
            Invoice invoice;

            RecordRef recordRef = new RecordRef();
            recordRef.type = RecordType.invoice;
            recordRef.typeSpecified = true;
            recordRef.internalId = id;

            service = ConnectionManager.GetNetSuiteService();
            invoice = (Invoice)service.get(recordRef).record;

            return invoice;
        }

        public List<Invoice> GetListInvoiceFromCustomerID(string id)
        {

            NetSuiteService service;

            CustomerSearchBasic customer = new CustomerSearchBasic();

            RecordRef Custom = new RecordRef();
            Custom.type = RecordType.customer;
            Custom.internalId = id;
            Custom.typeSpecified = true;
            RecordRef[] custRecordRefArray = new RecordRef[1];
            custRecordRefArray[0] = Custom;
            SearchMultiSelectField searchCustomer = new SearchMultiSelectField();
            searchCustomer.@operator = SearchMultiSelectFieldOperator.anyOf;
            searchCustomer.operatorSpecified = true;
            searchCustomer.searchValue = custRecordRefArray;
            customer.internalId = searchCustomer;

            TransactionSearchBasic Transaction = new TransactionSearchBasic();
            SearchEnumMultiSelectField searchMultiSelectField = new SearchEnumMultiSelectField();
            searchMultiSelectField.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            searchMultiSelectField.operatorSpecified = true;
            searchMultiSelectField.searchValue = new string[] { "_invoice" };
            Transaction.type = searchMultiSelectField;

            TransactionSearch BusquedaTransaccion = new TransactionSearch();
            BusquedaTransaccion.basic = Transaction;
            BusquedaTransaccion.customerJoin = customer;

            service = ConnectionManager.GetNetSuiteService();

            SearchResult searchResult = service.search(BusquedaTransaccion);

            List<Invoice> recordList = searchResult.recordList.Cast<Invoice>().ToList();

            return recordList;
        }

        public List<SalesOrder> GetListSalesOrderFromCustomerID(string id)
        {

            NetSuiteService service;

            CustomerSearchBasic customer = new CustomerSearchBasic();

            RecordRef Custom = new RecordRef();
            Custom.type = RecordType.customer;
            Custom.internalId = id;
            Custom.typeSpecified = true;
            RecordRef[] custRecordRefArray = new RecordRef[1];
            custRecordRefArray[0] = Custom;
            SearchMultiSelectField searchCustomer = new SearchMultiSelectField();
            searchCustomer.@operator = SearchMultiSelectFieldOperator.anyOf;
            searchCustomer.operatorSpecified = true;
            searchCustomer.searchValue = custRecordRefArray;
            customer.internalId = searchCustomer;

            TransactionSearchBasic Transaction = new TransactionSearchBasic();
            SearchEnumMultiSelectField searchMultiSelectField = new SearchEnumMultiSelectField();
            searchMultiSelectField.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            searchMultiSelectField.operatorSpecified = true;
            searchMultiSelectField.searchValue = new string[] { "_salesOrder" };
            Transaction.type = searchMultiSelectField;

            TransactionSearch BusquedaTransaccion = new TransactionSearch();
            BusquedaTransaccion.basic = Transaction;
            BusquedaTransaccion.customerJoin = customer;

            service = ConnectionManager.GetNetSuiteService();

            SearchResult searchResult = service.search(BusquedaTransaccion);

            List<SalesOrder> recordList = searchResult.recordList.Cast<SalesOrder>().ToList();

            return recordList;
        }

        public List<Invoice> GetAllInvoice()
        {
            NetSuiteService service = new NetSuiteService();
            List<Invoice> recordList = new List<Invoice>();
            TransactionSearch transactionsSearch = new TransactionSearch();
            TransactionSearchBasic transactionSearchBasic = new TransactionSearchBasic();
            transactionSearchBasic.type = new SearchEnumMultiSelectField();
            transactionSearchBasic.type.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            transactionSearchBasic.type.operatorSpecified = true;
            transactionSearchBasic.type.searchValue = new string[] { "_invoice" };
            transactionsSearch.basic = transactionSearchBasic;

            //SearchPreferences prefrence = new SearchPreferences();
            //prefrence.bodyFieldsOnly = false;

            //service.searchPreferences = prefrence;
            service = ConnectionManager.GetNetSuiteService();
            SearchResult searchResult = service.search(transactionsSearch);

            var tamaño = searchResult.totalPages;

            if (tamaño <= 1)
            {
                recordList = searchResult.recordList.Cast<Invoice>().ToList();
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
                            try
                            {
                                if (searchResult.recordList[i] is Invoice)
                                {
                                    Invoice objItem = (Invoice)searchResult.recordList[i];
                                    recordList.Add(objItem);
                                }
                            }
                            catch
                            {
                                continue;
                            }

                        }
                    }

                }
            }

            return recordList;
        }

        public double GetBalancePaidInvoice(string id)
        {
            NetSuiteService service;
            long GetId = long.Parse(id);
            double amount = 0.0;

            TransactionSearchAdvanced tsa = new TransactionSearchAdvanced()
            {
                columns = new TransactionSearchRow()
                {
                    basic = new TransactionSearchRowBasic()
                    {
                        total = new SearchColumnDoubleField[] { new SearchColumnDoubleField() },
                        amount = new SearchColumnDoubleField[] { new SearchColumnDoubleField() },
                        amountPaid = new SearchColumnDoubleField[] { new SearchColumnDoubleField() },
                        amountRemaining = new SearchColumnDoubleField[] { new SearchColumnDoubleField() }
                    }
                },
                criteria = new TransactionSearch()
                {
                    basic = new TransactionSearchBasic()
                    {
                        mainLine = new SearchBooleanField()
                        {
                            searchValue = true,
                            searchValueSpecified = true
                        },
                        type = new SearchEnumMultiSelectField()
                        {
                            @operator = SearchEnumMultiSelectFieldOperator.anyOf,
                            operatorSpecified = true,
                            searchValue = new string[] { "_invoice" }
                        },
                        internalIdNumber = new SearchLongField()
                        {
                            @operator = SearchLongFieldOperator.equalTo,
                            operatorSpecified = true,
                            searchValue = GetId,
                            searchValueSpecified = true
                        }
                    }
                }
            };
            service = ConnectionManager.GetNetSuiteService();
            SearchResult sr = service.search(tsa);

            if (sr.status.isSuccess)
            {
                SearchRow[] searchRows = sr.searchRowList;
                if (searchRows != null && searchRows.Length >= 1)
                {
                    TransactionSearchRow tranRow = (TransactionSearchRow) searchRows[0];
                    if (tranRow.basic.amountRemaining != null && tranRow.basic.amountRemaining.Length >= 1)
                    {
                        amount = tranRow.basic.amountRemaining[0].searchValue;
                        return amount;
                    }
                }
                else
                {
                    return amount;
                }
            }
            else
            {
                return amount;
            }

            return amount;
        }

        public BalanceTerms GetBalancePaidInvoiceandTerms(string id)
        {
            NetSuiteService service;
            long GetId = long.Parse(id);
            BalanceTerms response = new BalanceTerms();
            double amount = 0.0;

            TransactionSearchAdvanced tsa = new TransactionSearchAdvanced()
            {
                columns = new TransactionSearchRow()
                {
                    basic = new TransactionSearchRowBasic()
                    {
                        total = new SearchColumnDoubleField[] { new SearchColumnDoubleField() },
                        amount = new SearchColumnDoubleField[] { new SearchColumnDoubleField() },
                        amountPaid = new SearchColumnDoubleField[] { new SearchColumnDoubleField() },
                        amountRemaining = new SearchColumnDoubleField[] { new SearchColumnDoubleField() },
                        customFieldList = new SearchColumnSelectCustomField[] { new SearchColumnSelectCustomField() { scriptId = "custbody_mx_txn_sat_payment_term" } }
                    }
                },
                criteria = new TransactionSearch()
                {
                    basic = new TransactionSearchBasic()
                    {
                        mainLine = new SearchBooleanField()
                        {
                            searchValue = true,
                            searchValueSpecified = true
                        },
                        type = new SearchEnumMultiSelectField()
                        {
                            @operator = SearchEnumMultiSelectFieldOperator.anyOf,
                            operatorSpecified = true,
                            searchValue = new string[] { "_invoice" }
                        },
                        internalIdNumber = new SearchLongField()
                        {
                            @operator = SearchLongFieldOperator.equalTo,
                            operatorSpecified = true,
                            searchValue = GetId,
                            searchValueSpecified = true
                        }
                    }
                }
            };
            service = ConnectionManager.GetNetSuiteService();
            SearchResult sr = service.search(tsa);

            if (sr.status.isSuccess)
            {
                SearchRow[] searchRows = sr.searchRowList;
                if (searchRows != null && searchRows.Length >= 1)
                {
                    TransactionSearchRow tranRow = (TransactionSearchRow)searchRows[0];
                    if (tranRow.basic.amountRemaining != null && tranRow.basic.amountRemaining.Length >= 1)
                    {
                        amount = tranRow.basic.amountRemaining[0].searchValue;
                        response.amount = amount;

                        if (tranRow.basic.customFieldList != null)
                        {
                        foreach (SearchColumnCustomField customField in tranRow.basic.customFieldList)
                        {
                            try
                            {
                            SearchColumnSelectCustomField custField = (SearchColumnSelectCustomField)customField;
                            response.ns_terms = custField.searchValue.internalId;
                            }
                            catch
                            {
                                response.ns_terms = "0";
                            }

                        }
                        }

                        return response;
                    }
                }
                else
                {
                    response.amount = amount;
                    return response;
                }
            }
            else
            {
                response.amount = amount;
                return response;
            }

            response.amount = amount;
            return response;
        }

        public string GetTermsSAT(string id)
        {
            NetSuiteService service;
            long GetId = long.Parse(id);
            BalanceTerms response = new BalanceTerms();
            string terminos = "";

            TransactionSearchAdvanced tsa = new TransactionSearchAdvanced()
            {
                columns = new TransactionSearchRow()
                {
                    basic = new TransactionSearchRowBasic()
                    {
                        customFieldList = new SearchColumnSelectCustomField[] { new SearchColumnSelectCustomField() { scriptId = "custbody_mx_txn_sat_payment_term" } }
                    }
                },
                criteria = new TransactionSearch()
                {
                    basic = new TransactionSearchBasic()
                    {
                        mainLine = new SearchBooleanField()
                        {
                            searchValue = true,
                            searchValueSpecified = true
                        },
                        type = new SearchEnumMultiSelectField()
                        {
                            @operator = SearchEnumMultiSelectFieldOperator.anyOf,
                            operatorSpecified = true,
                            searchValue = new string[] { "_invoice" }
                        },
                        internalIdNumber = new SearchLongField()
                        {
                            @operator = SearchLongFieldOperator.equalTo,
                            operatorSpecified = true,
                            searchValue = GetId,
                            searchValueSpecified = true
                        }
                    }
                }
            };
            service = ConnectionManager.GetNetSuiteService();
            SearchResult sr = service.search(tsa);

            if (sr.status.isSuccess)
            {
                SearchRow[] searchRows = sr.searchRowList;
                if (searchRows != null && searchRows.Length >= 1)
                {
                    TransactionSearchRow tranRow = (TransactionSearchRow)searchRows[0];
                    if (tranRow.basic != null)
                    {
                        foreach (SearchColumnCustomField customField in tranRow.basic.customFieldList)
                        {
                            try
                            {
                                SearchColumnSelectCustomField custField = (SearchColumnSelectCustomField)customField;
                                terminos = custField.searchValue.internalId;
                            }
                            catch
                            {
                                terminos = "";
                            }

                        }

                        return terminos;
                    }
                    else
                    {
                        return terminos;
                    }
                }
                else
                {
                    
                    return terminos;
                }
            }
            else
            {
                return terminos;
            }

            return terminos;
        }

        public WriteResponse UpdateInvoice(string custId)
        {
            Invoice invoice = new Invoice();
            invoice.internalId = custId;
            NetSuiteService service;

            ListOrRecordRef custSelectValue = new ListOrRecordRef();
            custSelectValue.internalId = "4";

            SelectCustomFieldRef selectCustomFieldRef = new SelectCustomFieldRef();
            selectCustomFieldRef.internalId = "2064";
            selectCustomFieldRef.value = custSelectValue;
            selectCustomFieldRef.scriptId = "custbody_mx_txn_sat_payment_term";

            List<CustomFieldRef> l_value = new List<CustomFieldRef>();
            l_value.Add(selectCustomFieldRef);

            CustomFieldRef[] customFieldRef = new CustomFieldRef[0];
            customFieldRef = l_value.ToArray();
            invoice.customFieldList = customFieldRef;

            service = ConnectionManager.GetNetSuiteService();

            WriteResponse response = service.update(invoice);

            return response;
        }

        public RespuestaServicios GetInvoiceFile(string custId)
        {
            try
            {
                RespuestaServicios respuesta = new RespuestaServicios();
                NetSuiteService service;
                SuiteTalk.File factura;

                RecordRef recordRef = new RecordRef();
                recordRef.type = RecordType.file;
                recordRef.typeSpecified = true;
                recordRef.internalId = custId;

                service = ConnectionManager.GetNetSuiteService();
                factura = (SuiteTalk.File)service.get(recordRef).record;

                Record fileDownloaded = (Record)factura;

                var fileUrl = factura.url;

                // creates a new file
                SuiteTalk.File NewFile = (SuiteTalk.File)fileDownloaded;
                string fName = NewFile.name;
                byte[] b1 = null;
                b1 = NewFile.content;

                //saves the file in the specified folder. FileStream requires System.IO namespace
                FileStream fs1 = null;
                fs1 = new FileStream("C:\\temp\\" + fName, FileMode.Create);
                fs1.Write(b1, 0, b1.Length);
                fs1.Close();
                fs1 = null;

                respuesta.Estatus = "Exito";
                respuesta.Valor = fName;

                return respuesta;
            }catch (Exception ex)
            {
                RespuestaServicios respuesta = new RespuestaServicios();
                respuesta.Estatus = "Fracaso";
                respuesta.Valor = ex.Message;
                return respuesta;
            }
            
        }

        public List<Invoice> GetListInvoiceFromSalesOrder(string id)
        {

            NetSuiteService service;

            TransactionSearchBasic salesOrder = new TransactionSearchBasic();

            RecordRef salesorderRef = new RecordRef();
            salesorderRef.type = RecordType.salesOrder;
            salesorderRef.internalId = id;
            salesorderRef.typeSpecified = true;
            RecordRef[] custRecordRefArray = new RecordRef[1];
            custRecordRefArray[0] = salesorderRef;
            SearchMultiSelectField searchSO = new SearchMultiSelectField();
            searchSO.@operator = SearchMultiSelectFieldOperator.anyOf;
            searchSO.operatorSpecified = true;
            searchSO.searchValue = custRecordRefArray;

            TransactionSearchBasic Transaction = new TransactionSearchBasic();
            SearchEnumMultiSelectField searchMultiSelectField = new SearchEnumMultiSelectField();
            searchMultiSelectField.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            searchMultiSelectField.operatorSpecified = true;
            searchMultiSelectField.searchValue = new string[] { "_invoice" };
            Transaction.type = searchMultiSelectField;

            SearchMultiSelectField searchfromMultiSelectField = new SearchMultiSelectField();
            searchfromMultiSelectField.@operator = SearchMultiSelectFieldOperator.anyOf;
            searchfromMultiSelectField.operatorSpecified = true;
            searchfromMultiSelectField.searchValue = custRecordRefArray;
            Transaction.createdFrom = searchfromMultiSelectField; 

            TransactionSearch BusquedaTransaccion = new TransactionSearch();
            BusquedaTransaccion.basic = Transaction;

            service = ConnectionManager.GetNetSuiteService();

            SearchResult searchResult = service.search(BusquedaTransaccion);

            List<Invoice> recordList = searchResult.recordList.Cast<Invoice>().ToList();

            return recordList;
        }

        public RespuestaServicios GetSATFieldsForInvoice(string id)
        {
            NetSuiteService service;
            RespuestaServicios respuesta = new RespuestaServicios();
            long GetId = long.Parse(id);

            TransactionSearchAdvanced tsa = new TransactionSearchAdvanced()
            {
                columns = new TransactionSearchRow()
                {
                    basic = new TransactionSearchRowBasic()
                    {
                        internalId = new SearchColumnSelectField[] {new SearchColumnSelectField() },
                        customFieldList = new SearchColumnCustomField[] { new SearchColumnStringCustomField() { scriptId ="custbody_mx_cfdi_uuid"},
                                                                          new SearchColumnSelectCustomField() { scriptId = "custbody_edoc_generated_pdf"},
                                                                          new SearchColumnSelectCustomField() { scriptId = "custbody_psg_ei_certified_edoc"}}
                    }
                },
                criteria = new TransactionSearch()
                {
                    basic = new TransactionSearchBasic()
                    {
                        mainLine = new SearchBooleanField()
                        {
                            searchValue = true,
                            searchValueSpecified = true
                        },
                        type = new SearchEnumMultiSelectField()
                        {
                            @operator = SearchEnumMultiSelectFieldOperator.anyOf,
                            operatorSpecified = true,
                            searchValue = new string[] { "_invoice" }
                        },
                        internalIdNumber = new SearchLongField()
                        {
                            @operator = SearchLongFieldOperator.equalTo,
                            operatorSpecified = true,
                            searchValue = GetId,
                            searchValueSpecified = true
                        }
                    }
                }
            };
            service = ConnectionManager.GetNetSuiteService();
            SearchResult sr = service.search(tsa);

            if (sr.status.isSuccess)
            {
                SearchRow[] searchRows = sr.searchRowList;
                if (searchRows != null && searchRows.Length >= 1)
                {
                    TransactionSearchRow tranRow = (TransactionSearchRow)searchRows[0];
                    DatosSat RegistrarInformacion = new DatosSat();

                    if(tranRow.basic.customFieldList != null)
                    {
                    foreach (SearchColumnCustomField customField in tranRow.basic.customFieldList)
                    {
                        if (customField.scriptId == "custbody_mx_cfdi_uuid")
                        {
                            SearchColumnStringCustomField custField = (SearchColumnStringCustomField) customField;

                            if (custField.searchValue == null)
                            {
                                respuesta.Mensaje = "Aun no esta timbrado";
                                respuesta.Estatus = "Fracaso";
                                return respuesta;
                            }
                            else
                            {
                                RegistrarInformacion.UUID = custField.searchValue;
                            }

                        } else if (customField.scriptId == "custbody_edoc_generated_pdf")
                        {
                            SearchColumnSelectCustomField custField = (SearchColumnSelectCustomField)customField;
                            RegistrarInformacion.NS_PDF = custField.searchValue.internalId;


                        } else if (customField.scriptId == "custbody_psg_ei_certified_edoc")
                        {
                            SearchColumnSelectCustomField custField = (SearchColumnSelectCustomField)customField;
                            RegistrarInformacion.NS_XML = custField.searchValue.internalId;

                        }
                    }

                    respuesta.Valor = RegistrarInformacion;
                    respuesta.Estatus = "Exito";
                    return respuesta;
                    }
                    else
                    {
                        respuesta.Estatus = "Fracaso";
                        return respuesta;
                    }


                }
                else
                {
                    respuesta.Estatus = "Fracaso";
                    return respuesta;
                }
            }
            else
            {
                respuesta.Estatus = "Fracaso";
                return respuesta;
            }
        }

        public RespuestaServicios GetSATFieldsForAllInvoice()
        {
            NetSuiteService service;
            RespuestaServicios respuesta = new RespuestaServicios();
            //long GetId = long.Parse(id);
            List<DatosSat> l_datosSat = new List<DatosSat>();

            TransactionSearchAdvanced tsa = new TransactionSearchAdvanced()
            {
                columns = new TransactionSearchRow()
                {
                    basic = new TransactionSearchRowBasic()
                    {
                        internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() },
                        tranId = new SearchColumnStringField[] { new SearchColumnStringField() },

                        customFieldList = new SearchColumnCustomField[] { new SearchColumnStringCustomField() { scriptId ="custbody_mx_cfdi_uuid"},
                                                                          new SearchColumnSelectCustomField() { scriptId = "custbody_edoc_generated_pdf"},
                                                                          new SearchColumnSelectCustomField() { scriptId = "custbody_psg_ei_certified_edoc"}}
                    }
                },
                criteria = new TransactionSearch()
                {
                    basic = new TransactionSearchBasic()
                    {
                        mainLine = new SearchBooleanField()
                        {
                            searchValue = true,
                            searchValueSpecified = true
                        },
                        type = new SearchEnumMultiSelectField()
                        {
                            @operator = SearchEnumMultiSelectFieldOperator.anyOf,
                            operatorSpecified = true,
                            searchValue = new string[] { "CustInvc" }
                        }
                    }
                }
            };
            service = ConnectionManager.GetNetSuiteService();
            SearchResult sr = service.search(tsa);

            if (sr.status.isSuccess)
            {
                SearchRow[] searchRows = sr.searchRowList;
                if (searchRows != null && searchRows.Length >= 1)
                {
                    foreach (TransactionSearchRow rows_Inventory in searchRows)
                    { 
                        TransactionSearchRow tranRow = (TransactionSearchRow) rows_Inventory;
                        var iNS = rows_Inventory.basic.internalId[0].searchValue.internalId;

                        if (tranRow.basic.customFieldList != null)
                        {
                        if (l_datosSat.Any(n => n.NS_InternalID == iNS))
                        {

                        }
                        else
                        {
                            DatosSat RegistrarInformacion = new DatosSat();

                    foreach (SearchColumnCustomField customField in tranRow.basic.customFieldList)
                    {
                        if (customField.scriptId == "custbody_mx_cfdi_uuid")
                        {
                            SearchColumnStringCustomField custField = (SearchColumnStringCustomField)customField;

                            if (custField.searchValue == null)
                            {
                                continue;
                            }
                            else
                            {
                                RegistrarInformacion.UUID = custField.searchValue;
                            }

                        }
                        else if (customField.scriptId == "custbody_edoc_generated_pdf")
                        {
                            SearchColumnSelectCustomField custField = (SearchColumnSelectCustomField)customField;
                            RegistrarInformacion.NS_PDF = custField.searchValue.internalId;


                        }
                        else if (customField.scriptId == "custbody_psg_ei_certified_edoc")
                        {
                            SearchColumnSelectCustomField custField = (SearchColumnSelectCustomField)customField;
                            RegistrarInformacion.NS_XML = custField.searchValue.internalId;

                        }
                    }

                        RegistrarInformacion.NS_InternalID = tranRow.basic.internalId[0].searchValue.internalId;
                        RegistrarInformacion.NS_ExternalID = tranRow.basic.tranId[0].searchValue;
                        l_datosSat.Add(RegistrarInformacion);
                        }
                        }




                    }
                        
                    respuesta.Valor = l_datosSat;
                    respuesta.Estatus = "Exito";
                    return respuesta;
                }
                else
                {
                    respuesta.Estatus = "Fracaso";
                    return respuesta;
                }
            }
            else
            {
                respuesta.Estatus = "Fracaso";
                return respuesta;
            }
        }

        public RespuestaServicios GetSATInvoice(string id)
        {
            NetSuiteService service;
            RespuestaServicios respuesta = new RespuestaServicios();
            long GetId = long.Parse(id);

            TransactionSearchAdvanced tsa = new TransactionSearchAdvanced()
            {
                columns = new TransactionSearchRow()
                {
                    basic = new TransactionSearchRowBasic()
                    {
                        internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() },
                        customFieldList = new SearchColumnCustomField[] { new SearchColumnStringCustomField() { scriptId = "custbody_mx_cfdi_uuid"},
                                                                          new SearchColumnSelectCustomField() { scriptId = "custbody_edoc_generated_pdf"},
                                                                          new SearchColumnSelectCustomField() { scriptId = "custbody_psg_ei_certified_edoc"},
                                                                          new SearchColumnStringCustomField() { scriptId = "custbody_mx_cfdi_serie"},
                                                                          new SearchColumnStringCustomField() { scriptId = "custbody_mx_cfdi_folio"},
                                                                          new SearchColumnStringCustomField() { scriptId = "custbody_mx_cfdi_certify_timestamp"}}
                    }
                },
                criteria = new TransactionSearch()
                {
                    basic = new TransactionSearchBasic()
                    {
                        mainLine = new SearchBooleanField()
                        {
                            searchValue = true,
                            searchValueSpecified = true
                        },
                        type = new SearchEnumMultiSelectField()
                        {
                            @operator = SearchEnumMultiSelectFieldOperator.anyOf,
                            operatorSpecified = true,
                            searchValue = new string[] { "_invoice" }
                        },
                        internalIdNumber = new SearchLongField()
                        {
                            @operator = SearchLongFieldOperator.equalTo,
                            operatorSpecified = true,
                            searchValue = GetId,
                            searchValueSpecified = true
                        }
                    }
                }
            };
            service = ConnectionManager.GetNetSuiteService();
            SearchResult sr = service.search(tsa);

            if (sr.status.isSuccess)
            {
                SearchRow[] searchRows = sr.searchRowList;
                if (searchRows != null && searchRows.Length >= 1)
                {
                    TransactionSearchRow tranRow = (TransactionSearchRow)searchRows[0];
                    DatosSat RegistrarInformacion = new DatosSat();

                    if (tranRow.basic.customFieldList != null)
                    {
                        foreach (SearchColumnCustomField customField in tranRow.basic.customFieldList)
                        {
                            if (customField.scriptId == "custbody_mx_cfdi_uuid")
                            {
                                SearchColumnStringCustomField custField = (SearchColumnStringCustomField)customField;
                                RegistrarInformacion.UUID = custField.searchValue;

                            }
                            else if (customField.scriptId == "custbody_edoc_generated_pdf")
                            {
                                SearchColumnSelectCustomField custField = (SearchColumnSelectCustomField)customField;
                                RegistrarInformacion.NS_PDF = custField.searchValue.internalId;

                            }
                            else if (customField.scriptId == "custbody_psg_ei_certified_edoc")
                            {
                                SearchColumnSelectCustomField custField = (SearchColumnSelectCustomField)customField;
                                RegistrarInformacion.NS_XML = custField.searchValue.internalId;

                            }
                            else if (customField.scriptId == "custbody_mx_cfdi_serie")
                            {
                                SearchColumnStringCustomField custField = (SearchColumnStringCustomField)customField;
                                RegistrarInformacion.Serie = custField.searchValue;
                            }
                            else if (customField.scriptId == "custbody_mx_cfdi_folio")
                            {
                                SearchColumnStringCustomField custField = (SearchColumnStringCustomField)customField;
                                RegistrarInformacion.Folio = custField.searchValue;
                            }
                            else if (customField.scriptId == "custbody_mx_cfdi_certify_timestamp")
                            {
                                SearchColumnStringCustomField custField = (SearchColumnStringCustomField)customField;
                                RegistrarInformacion.FechaTimbrado = custField.searchValue;

                            }
                        }
                        respuesta.Valor = RegistrarInformacion;
                        respuesta.Estatus = "Exito";
                        return respuesta;
                    }
                    else
                    {
                        respuesta.Estatus = "Fracaso";
                        return respuesta;
                    }


                }
                else
                {
                    respuesta.Estatus = "Fracaso";
                    return respuesta;
                }
            }
            else
            {
                respuesta.Estatus = "Fracaso";
                return respuesta;
            }
        }
    } 
}


