namespace NetsuiteLibrary.SuiteTalk_Helpers
{
    using NetsuiteLibrary.Clases;
    using NetsuiteLibrary.SuiteTalk;
    using System;
    using System.Collections.Generic;

    public class ItemFulFillmentHelper
    {
        public RespuestaServicios GetInventoryNumber(string Ubicacion, string AssemblyID)
        {
            try
            {
                //Declaracion de Respuesta
                RespuestaServicios resp = new RespuestaServicios();

                NetSuiteService service;
                ActualizaStock registrarStock = new ActualizaStock();
                List<ActualizaStock> l_RegistrarStock = new List<ActualizaStock>();

                //RecordRef LocationRef = new DimensionHelper().GetLocation(Ubicacion);

                RecordRef LocationRef = new RecordRef();
                LocationRef.type = RecordType.location;
                LocationRef.typeSpecified = true;
                LocationRef.internalId = Ubicacion;

                //LotNumberedAssemblyItem ItemInventoryRef = new ItemHelper().GetLotNumberedAssemblyItemByID(AssemblyID);
                RecordRef recordRefItem = new RecordRef();
                recordRefItem.type = RecordType.inventoryItem;
                recordRefItem.typeSpecified = true;
                recordRefItem.internalId = AssemblyID;

                InventoryNumberSearchAdvanced advanced = new InventoryNumberSearchAdvanced();

                //search Criteria
                InventoryNumberSearchBasic itembasic = new InventoryNumberSearchBasic();
                InventoryNumberSearch inventory = new InventoryNumberSearch();
                SearchMultiSelectField field = new SearchMultiSelectField();
                SearchMultiSelectField fieldItem = new SearchMultiSelectField();

                RecordRef[] binsLocation = new RecordRef[1];
                RecordRef[] binsItem = new RecordRef[1];

                binsLocation[0] = LocationRef;
                binsItem[0] = recordRefItem;

                field.operatorSpecified = true;
                field.searchValue = binsLocation;
                fieldItem.operatorSpecified = true;
                fieldItem.searchValue = binsItem;

                //inventory.basic
                itembasic.location = field;
                itembasic.item = fieldItem;
                inventory.basic = itembasic;

                //Columns
                InventoryNumberSearchRow Columns = new InventoryNumberSearchRow();
                InventoryNumberSearchRowBasic basicColumns = new InventoryNumberSearchRowBasic();

                basicColumns.internalId = new SearchColumnSelectField[] { new SearchColumnSelectField() };
                basicColumns.inventoryNumber = new SearchColumnStringField[] { new SearchColumnStringField() };
                basicColumns.item = new SearchColumnSelectField[] { new SearchColumnSelectField() };
                basicColumns.location = new SearchColumnSelectField[] { new SearchColumnSelectField() };
                basicColumns.quantityavailable = new SearchColumnDoubleField[] { new SearchColumnDoubleField() };

                Columns.basic = basicColumns;
                Columns.itemJoin = new ItemSearchRowBasic();

                advanced.criteria = inventory;
                advanced.columns = Columns;

                service = ConnectionManager.GetNetSuiteService();
                SearchResult res = service.search(advanced);


                if (res.status.isSuccess)
                {
                    SearchRow[] searchRows = res.searchRowList;
                    if (searchRows != null && searchRows.Length >= 1)
                    {

                        foreach (InventoryNumberSearchRow rows_Inventory in searchRows)
                        {

                            if (rows_Inventory.basic.inventoryNumber != null && rows_Inventory.basic.inventoryNumber.Length >= 1)
                            {
                                registrarStock.IssuesID = rows_Inventory.basic.internalId[0].searchValue.internalId;
                                registrarStock.InventoryID = rows_Inventory.basic.inventoryNumber[0].searchValue;
                                registrarStock.quantityavailable = rows_Inventory.basic.quantityavailable[0].searchValue;
                                registrarStock.item = rows_Inventory.basic.item[0].searchValue.internalId;
                                registrarStock.location = rows_Inventory.basic.location[0].searchValue.internalId;
                                l_RegistrarStock.Add(registrarStock);
                            }
                        }

                        resp.Estatus = "Exito";
                        resp.Valor = l_RegistrarStock;


                    }
                    else
                    {
                        resp.Estatus = "Fracaso";
                    }
                }

                return resp;
            } catch (Exception ex)
            {
                RespuestaServicios resp = new RespuestaServicios();
                resp.Estatus = "Fracaso";
                return resp;
            }

            
        }

        public RespuestaServicios GetInventoryAvaibleforLocation(string Ubicacion, string AssemblyID, string TipoProducto, string Pricingrefdata)
        {
            try
            {
                RespuestaServicios resp = new RespuestaServicios();
                NetSuiteService service = new NetSuiteService();
                ActualizaStock registrarStock = new ActualizaStock();

                if (TipoProducto == "Inventory")
                {
                    RecordRef recordRef = new RecordRef();
                    recordRef.type = RecordType.inventoryItem;
                    recordRef.typeSpecified = true;
                    recordRef.internalId = AssemblyID;

                    service = ConnectionManager.GetNetSuiteService();
                    InventoryItem itemVenta = (InventoryItem)service.get(recordRef).record;

                    InventoryItemLocations[] l_itemLocation = itemVenta.locationsList.locations;
                    foreach (InventoryItemLocations location in l_itemLocation)
                    {
                        if (location.location == Ubicacion)
                        {
                            registrarStock.quantityavailable = location.quantityAvailable;
                            registrarStock.item = AssemblyID;
                            registrarStock.location = location.location;
                        } else if (location.location == "2")
                        {
                            registrarStock.StockPlanta = location.quantityAvailable;
                        } else if (location.location == "117")
                        {
                            registrarStock.StockMuelles = location.quantityAvailable;
                        }
                    }

                    if (itemVenta.pricingMatrix == null)
                    {
                        registrarStock.Precio = 0.00;
                        registrarStock.Precio = 0.00;
                        registrarStock.DescripcionPrecio = "Sin Precio";

                    }
                    else
                    {

                        PricingMatrix l_Pricing = itemVenta.pricingMatrix;
                        foreach (Pricing datosPrecio in l_Pricing.pricing)
                        {
                            if (datosPrecio.currency.name == "MXN" && datosPrecio.priceLevel.internalId == Pricingrefdata)
                            {
                                registrarStock.Precio = datosPrecio.priceList[0].value;
                                registrarStock.DescripcionPrecio = datosPrecio.priceLevel.name;
                            }
                            else if (datosPrecio.currency.name == "MXN" && datosPrecio.priceLevel.internalId == "1")
                            {
                                registrarStock.PrecioLista = datosPrecio.priceList[0].value;
                            }
                        }
                    }



                }
                else if (TipoProducto == "Assembly_nL")
                {
                    RecordRef recordRef = new RecordRef();
                    recordRef.type = RecordType.assemblyItem;
                    recordRef.typeSpecified = true;
                    recordRef.internalId = AssemblyID;

                    service = ConnectionManager.GetNetSuiteService();
                    AssemblyItem itemVenta = (AssemblyItem)service.get(recordRef).record;

                    InventoryItemLocations[] l_itemLocation = itemVenta.locationsList.locations;

                    foreach (InventoryItemLocations location in l_itemLocation)
                    {
                        if (location.location == Ubicacion)
                        {
                            registrarStock.quantityavailable = location.quantityAvailable;
                            registrarStock.item = AssemblyID;
                            registrarStock.location = location.location;

                        }
                        else if (location.location == "2")
                        {
                            registrarStock.StockPlanta = location.quantityAvailable;
                        }
                        else if (location.location == "117")
                        {
                            registrarStock.StockMuelles = location.quantityAvailable;
                        }


                    }

                    if (itemVenta.pricingMatrix == null)
                    {
                        registrarStock.Precio = 0.00;
                        registrarStock.PrecioLista = 0.00;
                        registrarStock.DescripcionPrecio = "Sin Precio";
                    }
                    else
                    {

                        PricingMatrix l_Pricing = itemVenta.pricingMatrix;
                        foreach (Pricing datosPrecio in l_Pricing.pricing)
                        {
                            if (datosPrecio.currency.name == "MXN" && datosPrecio.priceLevel.internalId == Pricingrefdata)
                            {
                                registrarStock.Precio = datosPrecio.priceList[0].value;
                                registrarStock.DescripcionPrecio = datosPrecio.priceLevel.name;
                            } else if (datosPrecio.currency.name == "MXN" && datosPrecio.priceLevel.internalId == "1")
                                {
                                    registrarStock.PrecioLista = datosPrecio.priceList[0].value;
                                }
                        }
                    }
                }
                else if (TipoProducto == "Assembly")
                {
                    RecordRef recordRef = new RecordRef();
                    recordRef.type = RecordType.lotNumberedAssemblyItem;
                    recordRef.typeSpecified = true;
                    recordRef.internalId = AssemblyID;

                    service = ConnectionManager.GetNetSuiteService();
                    LotNumberedAssemblyItem itemVenta = (LotNumberedAssemblyItem)service.get(recordRef).record;

                    LotNumberedInventoryItemLocations[] l_itemLocation = itemVenta.locationsList.locations;

                    foreach (LotNumberedInventoryItemLocations location in l_itemLocation)
                    {
                        if (location.location == Ubicacion)
                        {
                            registrarStock.quantityavailable = location.quantityAvailable;
                            registrarStock.item = AssemblyID;
                            registrarStock.location = location.location;

                        }

                    }

                    if (itemVenta.pricingMatrix == null)
                    {
                        registrarStock.Precio = 0.00;
                        registrarStock.DescripcionPrecio = "Sin Precio";

                    }
                    else
                    {

                        PricingMatrix l_Pricing = itemVenta.pricingMatrix;
                        foreach (Pricing datosPrecio in l_Pricing.pricing)
                        {
                            if (datosPrecio.currency.name == "MXN" && datosPrecio.priceLevel.internalId == Pricingrefdata)
                            {
                                registrarStock.Precio = datosPrecio.priceList[0].value;
                                registrarStock.DescripcionPrecio = datosPrecio.priceLevel.name;
                            }
                        }
                    }
                }
                else if (TipoProducto == "Service")
                {
                    RecordRef recordRef = new RecordRef();
                    recordRef.type = RecordType.serviceResaleItem;
                    recordRef.typeSpecified = true;
                    recordRef.internalId = AssemblyID;

                    service = ConnectionManager.GetNetSuiteService();
                    ServiceResaleItem itemVenta = (ServiceResaleItem) service.get(recordRef).record;

                    if (itemVenta != null)
                    {
                        //registrarStock.quantityavailable = 1;
                        registrarStock.item = AssemblyID;
                        registrarStock.location = Ubicacion;

                        //LOS SERVICIOS TENDRAN UNICAMENTE EL BASE PRICE
                        PricingMatrix l_Pricing = itemVenta.pricingMatrix;
                        foreach (Pricing datosPrecio in l_Pricing.pricing)
                        {
                            if (datosPrecio.currency.name == "MXN" && datosPrecio.priceLevel.internalId == "1")
                            {
                                registrarStock.Precio = datosPrecio.priceList[0].value;
                                registrarStock.PrecioLista = datosPrecio.priceList[0].value;
                                registrarStock.DescripcionPrecio = datosPrecio.priceLevel.name;
                            }
                        }
                    }
                    else
                    {
                        RecordRef recordRefitem = new RecordRef();
                        recordRefitem.type = RecordType.serviceSaleItem;
                        recordRefitem.typeSpecified = true;
                        recordRefitem.internalId = AssemblyID;

                        service = ConnectionManager.GetNetSuiteService();
                        ServiceSaleItem itemVentaRs = (ServiceSaleItem)service.get(recordRefitem).record;

                        //registrarStock.quantityavailable = 1;
                        registrarStock.item = AssemblyID;
                        registrarStock.location = Ubicacion;

                        //LOS SERVICIOS TENDRAN UNICAMENTE EL BASE PRICE
                        PricingMatrix l_Pricing = itemVentaRs.pricingMatrix;
                        foreach (Pricing datosPrecio in l_Pricing.pricing)
                        {
                            if (datosPrecio.currency.name == "MXN" && datosPrecio.priceLevel.internalId == "1")
                            {
                                registrarStock.Precio = datosPrecio.priceList[0].value;
                                registrarStock.PrecioLista = datosPrecio.priceList[0].value;
                                registrarStock.DescripcionPrecio = datosPrecio.priceLevel.name;
                            }
                        }
                    }


                }

                    //Declaracion de Respuesta


                resp.Estatus = "Exito";
                resp.Valor = registrarStock;
                return resp;
            }
            catch (Exception ex)
            {
                RespuestaServicios resp = new RespuestaServicios();
                resp.Estatus = "Fracaso";
                return resp;
            }


        }

        public WriteResponse UpdateItemFulFillment(string custId)
        {
            SalesOrder salesOrder = new SalesOrder();
            salesOrder.internalId = custId;
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
            salesOrder.customFieldList = customFieldRef;

            service = ConnectionManager.GetNetSuiteService();

            WriteResponse response = service.update(salesOrder);

            return response;
        }
    }
}


