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

    public class TransferOrderHelper
    {
        public TransferOrder GetTransferOrderByID(string id)
        {
            NetSuiteService service;
            TransferOrder transferOrder;

            RecordRef recordRef = new RecordRef();
            recordRef.type = RecordType.transferOrder;
            recordRef.typeSpecified = true;
            recordRef.internalId = id;

            service = ConnectionManager.GetNetSuiteService();
            transferOrder = (TransferOrder) service.get(recordRef).record;

            return transferOrder;
        }

        public ItemReceipt GetItemReceiptByID(string id)
        {
            NetSuiteService service;
            ItemReceipt itemReceipt;

            RecordRef recordRef = new RecordRef();
            recordRef.type = RecordType.itemReceipt;
            recordRef.typeSpecified = true;
            recordRef.internalId = id;

            service = ConnectionManager.GetNetSuiteService();
            itemReceipt = (ItemReceipt) service.get(recordRef).record;

            return itemReceipt;
        }

        public RespuestaServicios InsertTransferOrder()
        {
            try
            {
                //Declaracion de Respuesta
                RespuestaServicios resp = new RespuestaServicios();

                //Declaracion de servicio
                NetSuiteService service;

                //Declaracion de alta de ingreso
                TransferOrder transferOrder = new TransferOrder();
                TransferOrderItemList l_itemlist = new TransferOrderItemList();
                List<TransferOrderItem> l_transferOrder = new List<TransferOrderItem>();

                //Referencia Ubicacion Origen
                RecordRef ubicacion_O = new RecordRef();
                ubicacion_O.type = RecordType.location;
                ubicacion_O.typeSpecified = true;
                //ubicacion_O.internalId = Model.InternalID_Mostrador_Origen;
                //Pruebas
                ubicacion_O.internalId = "2";

                //Referencia Ubicacion Destino
                RecordRef ubicacion_D = new RecordRef();
                ubicacion_D.type = RecordType.location;
                ubicacion_D.typeSpecified = true;
                //ubicacion_D.internalId = Model.InternalID_Mostrador_Destino;
                //Pruebas
                ubicacion_D.internalId = "10";

                //Referencia IncoTerms 
                RecordRef incoterms = new RecordRef();
                incoterms.type = RecordType.interCompanyTransferOrder;
                incoterms.typeSpecified = true;
                incoterms.internalId = "6";

                //Referencia Subsidiaria
                RecordRef subsidiaria = new RecordRef();
                subsidiaria.type = RecordType.subsidiary;
                subsidiaria.typeSpecified = true;
                subsidiaria.internalId = "2";

                //Mapeo Datos Transferencia
                transferOrder.transferLocation = ubicacion_D;
                transferOrder.location = ubicacion_O;
                transferOrder.memo = "orden de traslado pvo maf 2.0";
                transferOrder.incoterm = incoterms;
                transferOrder.orderStatus = TransferOrderOrderStatus._pendingApproval;
                transferOrder.subsidiary = subsidiaria;

                //****************************** Para la prueba con datos correctos netsuite ******************************
                //Mapeo de conceptos de pago
                //foreach (DetalleTransferOrder RegMercancia in Model.DetalleTransfer)
                //{
                //    TransferOrderItem item = new TransferOrderItem();

                //    RecordRef ItemRef = new RecordRef();

                //    if (RegMercancia.TipoMaterial == "Assembly")
                //    {
                //        ItemRef.type = RecordType.lotNumberedAssemblyItem;
                //        ItemRef.typeSpecified = true;
                //        ItemRef.internalId = RegMercancia.InternalID;
                //    }
                //    else if (RegMercancia.TipoMaterial == "Inventory")
                //    {
                //        ItemRef.type = RecordType.inventoryItem;
                //        ItemRef.typeSpecified = true;
                //        ItemRef.internalId = RegMercancia.InternalID;
                //    }

                //    item.item = ItemRef;
                //    item.lineSpecified = true;
                //    item.quantity = RegMercancia.Cantidad;

                //    l_transferOrder.Add(item);
                //}

                //Para pruebas
                TransferOrderItem item = new TransferOrderItem();
                RecordRef ItemRef = new RecordRef();
                ItemRef.type = RecordType.assemblyItem;
                ItemRef.typeSpecified = true;
                ItemRef.internalId = "12953";
                
                item.item = ItemRef;
                item.lineSpecified = true;
                item.quantity = 5;
                item.quantitySpecified = true;

                l_transferOrder.Add(item);
                l_itemlist.item = l_transferOrder.ToArray();

                transferOrder.itemList = l_itemlist;

                service = ConnectionManager.GetNetSuiteService();
                WriteResponse writeResponse = service.add(transferOrder);

                if (writeResponse.status.isSuccess)
                {
                    resp.Estatus = "Exito";
                    resp.Valor = writeResponse.baseRef;
                    return resp;
                }
                else
                {
                    resp.Estatus = "Fracaso";
                    resp.Valor = string.Format("Hubo un problema al crear la orden de compra. Detalle: {0}", writeResponse.status.statusDetail[0].message);
                    return resp;
                }
            }
            catch (Exception ex)
            {
                RespuestaServicios resp = new RespuestaServicios();
                resp.Estatus = "Fracaso";
                resp.Mensaje = String.Format("Hubo un Problema al Crear la Orden de Venta. Detalles: {0}.", ex.Message);
                return resp;
            }

        }

        public RespuestaServicios TransformTransferOrderToReceipt(MapRecepcionMercancia model)
        {
            try
            {
                RespuestaServicios resp = new RespuestaServicios();
                NetSuiteService service;
                InitializeRef transferRef = new InitializeRef();
                InitializeRecord records = new InitializeRecord();

                service = ConnectionManager.GetNetSuiteService();

                transferRef.internalId = model.Internal_ID;
                transferRef.type = InitializeRefType.interCompanyTransferOrder;
                transferRef.typeSpecified = true;

                records.reference = transferRef;
                records.type = InitializeType.itemReceipt;

                //ContactRole

                ReadResponse leerRespuesta = service.initialize(records);

                ItemReceipt itemReceipt = (ItemReceipt) leerRespuesta.record;
                List<ItemReceiptItem> l_items = new List<ItemReceiptItem>();

                foreach (ItemReceiptItem ValidaCantidades in itemReceipt.itemList.item)
                {

                    var foundQuantity = model.l_Detalle.FirstOrDefault(item => item.InternalID == ValidaCantidades.item.internalId);

                    ValidaCantidades.quantity = Convert.ToDouble(foundQuantity.Cantidad);
                    ValidaCantidades.quantitySpecified = true;
                }

                //ItemReceiptItemList arrayItems = new ItemReceiptItemList();
                //arrayItems.item = l_items.ToArray();
                //itemReceipt.itemList = arrayItems;
                itemReceipt.memo = model.Nota;

                service = ConnectionManager.GetNetSuiteService();
                WriteResponse writeResponse = service.add(itemReceipt);

                if (writeResponse.status.isSuccess)
                {
                    resp.Estatus = "Exito";
                    resp.Valor = writeResponse.baseRef;
                    return resp;
                }
                else
                {
                    resp.Estatus = "Fracaso";
                    resp.Mensaje = string.Format("Hubo un problema al crear la recepción de articulos. Detalle: {0}", writeResponse.status.statusDetail[0].message);
                    return resp;
                }

            }
            catch (Exception ex)
            {
                RespuestaServicios resp = new RespuestaServicios();
                resp.Estatus = "Fracaso";
                resp.Mensaje = String.Format("Hubo un Problema al Crear la Factura de Venta. Detalles: {0}.", ex.Message);
                return resp;
            }

        }

        public RespuestaServicios GetListTransferForLocationAdvancedVer(BusquedaCorteCaja model)
        {
            NetSuiteService service = new NetSuiteService();
            RespuestaServicios resp = new RespuestaServicios();
            List<MapTransferenciaOrden> l_RegistrarOrdenTransferencia = new List<MapTransferenciaOrden>();

            RecordRef LocationRef = new RecordRef();
            LocationRef.type = RecordType.location;
            LocationRef.internalId = model.Location_ID;
            LocationRef.typeSpecified = true;

            TransactionSearchAdvanced BusquedaTransaccion = new TransactionSearchAdvanced();

            //search Criteria
            TransactionSearchBasic tranbasic = new TransactionSearchBasic();
            TransactionSearch transearch = new TransactionSearch();
            SearchMultiSelectField field = new SearchMultiSelectField();
            SearchMultiSelectField fieldStatus = new SearchMultiSelectField();

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
            searchMultiSelectField.searchValue = new string[] { "transferOrder" };

            RecordRef[] binsLocation = new RecordRef[1];
            binsLocation[0] = LocationRef;

            field.operatorSpecified = true;
            field.searchValue = binsLocation;

            //SearchEnumMultiSelectField searchMultiSelectstatusField = new SearchEnumMultiSelectField();
            //searchMultiSelectstatusField.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            //searchMultiSelectstatusField.operatorSpecified = true;
            //searchMultiSelectstatusField.searchValue = new string[] { "pendingReceipt" };

            tranbasic.transferLocation = field;
            tranbasic.dateCreated = searchDate;
            tranbasic.type = searchMultiSelectField;
            //tranbasic.status = searchMultiSelectstatusField;
            //tranbasic.
            //tranbasic.is
            transearch.basic = tranbasic;

            TransactionSearchRow Columns = new TransactionSearchRow();
            TransactionSearchRowBasic basicColumns = new TransactionSearchRowBasic();
            LocationSearchRowBasic locationJoins = new LocationSearchRowBasic();

            basicColumns.tranId = new SearchColumnStringField[] { new SearchColumnStringField() };
            basicColumns.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.entity = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.paymentMethod = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.paymentOption = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.account = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.transferLocation = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.location = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.inventoryLocation = new SearchColumnSelectField[] { new SearchColumnSelectField() };
            basicColumns.status = new SearchColumnEnumSelectField[] { new SearchColumnEnumSelectField() };

            locationJoins.name = new SearchColumnStringField[] { new SearchColumnStringField() };

            Columns.basic = basicColumns;
            Columns.locationJoin = locationJoins;

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

                                if (l_RegistrarOrdenTransferencia.Any(l => l.NS_InternalID == iNS))
                                {

                                }
                                else
                                {

                                    if ((rows_Inventory.basic.status[0].searchValue == "pendingReceipt") || (rows_Inventory.basic.status[0].searchValue == "received") || (rows_Inventory.basic.status[0].searchValue == "pendingReceiptPartFulfilled") || (rows_Inventory.basic.status[0].searchValue == "partiallyFulfilled") || (rows_Inventory.basic.status[0].searchValue == "pendingFulfillment"))
                                    {
                                        //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando
                                        MapTransferenciaOrden RegistrarCorteCaja = new MapTransferenciaOrden();

                                        RegistrarCorteCaja.NS_InternalID = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                        RegistrarCorteCaja.Num_Transaccion = rows_Inventory.basic.tranId[0].searchValue;
                                        RegistrarCorteCaja.NS_InternalID_Origen = rows_Inventory.basic.transferLocation[0].searchValue.internalId;
                                        RegistrarCorteCaja.NS_InternalID_Destino = rows_Inventory.basic.location[0].searchValue.internalId;
                                        RegistrarCorteCaja.Estatus = rows_Inventory.basic.status[0].searchValue;

                                        l_RegistrarOrdenTransferencia.Add(RegistrarCorteCaja);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                continue;
                            }


                        }

                        resp.Estatus = "Exito";
                        resp.Valor = l_RegistrarOrdenTransferencia;
                        return resp;

                    }
                    else
                    {
                        resp.Estatus = "Exito";
                        resp.Valor = l_RegistrarOrdenTransferencia;
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

                                    if (l_RegistrarOrdenTransferencia.Any(l => l.NS_InternalID == iNS))
                                    {

                                    }
                                    else
                                    {

                                        if ((rows_Inventory.basic.status[0].searchValue == "pendingReceipt") || (rows_Inventory.basic.status[0].searchValue == "received") || (rows_Inventory.basic.status[0].searchValue == "pendingReceiptPartFulfilled") || (rows_Inventory.basic.status[0].searchValue == "partiallyFulfilled") || (rows_Inventory.basic.status[0].searchValue == "pendingFulfillment"))
                                        {
                                            //Para validar que los pagos sean correctos, se validara a que documento se estan aplicando
                                            MapTransferenciaOrden RegistrarCorteCaja = new MapTransferenciaOrden();

                                            RegistrarCorteCaja.NS_InternalID = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                            RegistrarCorteCaja.Num_Transaccion = rows_Inventory.basic.tranId[0].searchValue;
                                            RegistrarCorteCaja.NS_InternalID_Origen = rows_Inventory.basic.transferLocation[0].searchValue.internalId;
                                            RegistrarCorteCaja.NS_InternalID_Destino = rows_Inventory.basic.location[0].searchValue.internalId;
                                            RegistrarCorteCaja.Estatus = rows_Inventory.basic.status[0].searchValue;

                                            l_RegistrarOrdenTransferencia.Add(RegistrarCorteCaja);
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
                    resp.Valor = l_RegistrarOrdenTransferencia;
                    return resp;
                }


            }
            resp.Estatus = "Fracaso";
            resp.Mensaje = "Hubo un problema al procesar esta busqueda. Intentelo más tarde.";
            return resp;

        }


    }

}
