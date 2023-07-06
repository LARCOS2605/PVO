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

    public class EstimacionHelper
    {
        public RespuestaServicios CancelEstimacion(string id)
        {
            try
            {
                NetSuiteService service = new NetSuiteService();
                RespuestaServicios resp = new RespuestaServicios();
                Estimate consulta = GetEstimateByID(id);


                Estimate estimacion = new Estimate();
                
                estimacion.internalId = id;

                RecordRef estatus_estimate = new RecordRef();
                estatus_estimate.type = RecordType.customerStatus;
                estatus_estimate.typeSpecified = true;
                estatus_estimate.internalId = "14";
                estimacion.entityStatus = estatus_estimate;

                service = ConnectionManager.GetNetSuiteService();

                WriteResponse writeResponse = service.update(estimacion);
                if (writeResponse.status.isSuccess)
                {
                    resp.Estatus = "Exito";
                    resp.Valor = writeResponse.baseRef;
                    return resp;
                }
                else
                {
                    resp.Estatus = "Fracaso";
                    var Error = ConnectionManager.GetStatusDetails(writeResponse.status);
                    resp.Mensaje = string.Format("Hubo un problema al cancelar la Estimación. Detalle: {0}", Error);
                    return resp;
                }
            } catch (Exception ex)
            {
                RespuestaServicios resp = new RespuestaServicios();
                resp.Estatus = "Fracaso";
                resp.Mensaje = String.Format("Hubo un Problema al cancelar la estimación. Detalles: {0}.", ex.Message);
                return resp;
            }
        }

        public RespuestaServicios InsertEstimacion(AltaEstimacion altaOrden)
        {
            try
            {
                //Declaracion de Respuesta
                RespuestaServicios resp = new RespuestaServicios();

                //Declaracion de servicio
                NetSuiteService service;

                //Declaracion de alta de ingreso
                Estimate SalesEstimacion = new Estimate();

                //Declaracion de Mercancia
                EstimateItemList l_salesorder = new EstimateItemList();
                List<EstimateItem> ArrayItems = new List<EstimateItem>();

                //Creamos una Referencia Al Customer
                //Customer customer = new CustomerHelper().GetCustomerById(altaOrden.InternalID_Customer);
                RecordRef customerRef = new RecordRef();
                customerRef.type = RecordType.customer;
                customerRef.typeSpecified = true;
                customerRef.internalId = altaOrden.InternalID_Customer;
                //customerRef.externalId = customer.externalId;

                //Creamos una Referencia a la Ubicacion de la venta
                RecordRef LocationRef = new RecordRef();
                LocationRef.type = RecordType.location;
                LocationRef.typeSpecified = true;
                LocationRef.internalId = altaOrden.Ubicacion_Venta;

                #region "Items"

                foreach (DetalleEstimacion RegMerc in altaOrden.DetalleInventario)
                {

                    //Declaracion de mercancia o concepto
                    EstimateItem item = new EstimateItem();
                    RecordRef ItemRef = new RecordRef();

                    if(RegMerc.TipoMaterial == "Assembly")
                    {
                        //ItemRef.type = RecordType.lotNumberedAssemblyItem;
                        //ItemRef.typeSpecified = true;
                        //ItemRef.internalId = RegMerc.InternalID;

                        //RecordRef Issueinventory = new RecordRef();
                        //Issueinventory.typeSpecified = true;
                        //Issueinventory.internalId = RegMerc.IssuesInventoryID;

                        ////Declaracion de Inventario
                        //InventoryDetail detalleInventario = new InventoryDetail();
                        //InventoryAssignmentList asignarInventario = new InventoryAssignmentList();
                        //InventoryAssignment nuevoInventario = new InventoryAssignment();

                        //nuevoInventario.issueInventoryNumber = Issueinventory;
                        //nuevoInventario.quantity = RegMerc.Cantidad;
                        //nuevoInventario.quantitySpecified = true;
                        //nuevoInventario.internalId = ItemRef.internalId;

                        //asignarInventario.inventoryAssignment = new[] { nuevoInventario };
                        //detalleInventario.inventoryAssignmentList = asignarInventario;

                        //item.item = ItemRef;
                        //item.quantity = RegMerc.Cantidad;
                        //item.quantitySpecified = true;
                        //item.inventoryLocation = LocationRef;
                        //item.inventoryDetail = detalleInventario;

                        //ArrayItems.Add(item);

                    }
                    else if(RegMerc.TipoMaterial == "Inventory")
                    {
                        ItemRef.type = RecordType.inventoryItem;
                        ItemRef.typeSpecified = true;
                        ItemRef.internalId = RegMerc.InternalID;

                        item.item = ItemRef;
                        item.quantity = RegMerc.Cantidad;
                        item.quantitySpecified = true;
                        //item.inventoryLocation = LocationRef;

                        ArrayItems.Add(item);

                    }
                    else if(RegMerc.TipoMaterial == "Assembly_nL")
                    {
                        ItemRef.type = RecordType.assemblyItem;
                        ItemRef.typeSpecified = true;
                        ItemRef.internalId = RegMerc.InternalID;

                        item.item = ItemRef;
                        item.quantity = RegMerc.Cantidad;
                        item.quantitySpecified = true;
                        //item.inventoryLocation = LocationRef;

                        ArrayItems.Add(item);
                    }
                    else if (RegMerc.TipoMaterial == "Service")
                    {
                        ItemRef.type = RecordType.serviceResaleItem;
                        ItemRef.typeSpecified = true;
                        ItemRef.internalId = RegMerc.InternalID;

                        RecordRef ItemRef_Price = new RecordRef();
                        ItemRef_Price.type = RecordType.priceLevel;
                        ItemRef_Price.typeSpecified = true;
                        ItemRef_Price.internalId = "1";

                        item.item = ItemRef;
                        item.price = ItemRef_Price;
                        item.quantity = RegMerc.Cantidad;
                        item.quantitySpecified = true;

                        ArrayItems.Add(item);
                    }



                }

                l_salesorder.item = ArrayItems.ToArray();

                #endregion

                //Creamos lista de campos personalizados
                List<SelectCustomFieldRef> l_customField = new List<SelectCustomFieldRef>();

                //Campo Metodo Pago SAT
                ListOrRecordRef custSelectValue = new ListOrRecordRef();
                custSelectValue.internalId = altaOrden.InternalID_MetodoPagoSAT;
                SelectCustomFieldRef selectCustomFieldRef = new SelectCustomFieldRef();
                selectCustomFieldRef.internalId = "2064";
                selectCustomFieldRef.value = custSelectValue;
                selectCustomFieldRef.scriptId = "custbody_mx_txn_sat_payment_term";

                l_customField.Add(selectCustomFieldRef);

                //Campo Forma Pago SAT
                ListOrRecordRef regForPagoSelectValue = new ListOrRecordRef();
                regForPagoSelectValue.internalId = altaOrden.InternalID_FormaPagoSAT;
                SelectCustomFieldRef selectForPagoCustomFieldRef = new SelectCustomFieldRef();
                selectForPagoCustomFieldRef.internalId = "2063";
                selectForPagoCustomFieldRef.value = regForPagoSelectValue;
                selectForPagoCustomFieldRef.scriptId = "custbody_mx_txn_sat_payment_method";

                l_customField.Add(selectForPagoCustomFieldRef);

                //Campo Uso de CFDI SAT
                ListOrRecordRef regUsoCFDISelectValue = new ListOrRecordRef();
                regUsoCFDISelectValue.internalId = altaOrden.InternalID_UsoCFDI;
                SelectCustomFieldRef selectUsoCFDICustomFieldRef = new SelectCustomFieldRef();
                selectUsoCFDICustomFieldRef.internalId = "2061";
                selectUsoCFDICustomFieldRef.value = regUsoCFDISelectValue;
                selectUsoCFDICustomFieldRef.scriptId = "custbody_mx_cfdi_usage";

                l_customField.Add(selectUsoCFDICustomFieldRef);

                //Campo Template Timbrado
                //ListOrRecordRef TemplateTimbradoSelectValue = new ListOrRecordRef();
                //TemplateTimbradoSelectValue.internalId = "108";
                //SelectCustomFieldRef TemplateCustomFieldRef = new SelectCustomFieldRef();
                //TemplateCustomFieldRef.internalId = "1444";
                //TemplateCustomFieldRef.value = TemplateTimbradoSelectValue;
                //TemplateCustomFieldRef.scriptId = "custbody_psg_ei_template";

                //l_customField.Add(TemplateCustomFieldRef);

                //Campo Envio de Datos electronicos Timbrado
                //ListOrRecordRef MethodSendDocSelectValue = new ListOrRecordRef();
                //MethodSendDocSelectValue.internalId = "104";
                //MethodSendDocSelectValue.typeId = "153";
                //SelectCustomFieldRef MethodSendDocCustomFieldRef = new SelectCustomFieldRef();
                //MethodSendDocCustomFieldRef.internalId = "1452";
                //MethodSendDocCustomFieldRef.value = MethodSendDocSelectValue;
                //MethodSendDocCustomFieldRef.scriptId = "custbody_psg_ei_sending_method";

                //l_customField.Add(MethodSendDocCustomFieldRef);

                //Campo Arreglo Custom Field
                CustomFieldRef[] customFieldRef = new CustomFieldRef[0];
                customFieldRef = l_customField.ToArray();

                //Campos de encabezado
                SalesEstimacion.memo = altaOrden.memo;
                SalesEstimacion.entity = customerRef;
                SalesEstimacion.location = LocationRef;
                SalesEstimacion.itemList = l_salesorder;
                SalesEstimacion.tranDate = DateTime.Now;
                SalesEstimacion.customFieldList = customFieldRef;
                //SalesOrderalta.currency = currencyRef;

                //Conexión NetSuite
                service = ConnectionManager.GetNetSuiteService();
                WriteResponse writeResponse = service.add(SalesEstimacion);

                if (writeResponse.status.isSuccess)
                {
                    resp.Estatus = "Exito";
                    resp.Valor = writeResponse.baseRef;
                    return resp;
                } else
                {
                    resp.Estatus = "Fracaso";
                    var Error = ConnectionManager.GetStatusDetails(writeResponse.status);
                    resp.Valor = string.Format("Hubo un problema al crear la estimación. Detalle: {0}", Error);
                    return resp;
                }

            }
            catch (Exception ex)
            {
                RespuestaServicios resp = new RespuestaServicios();
                resp.Estatus = "Fracaso";
                resp.Valor = String.Format("Hubo un Problema al Crear laEstimación. Detalles: {0}.", ex.Message);
                return resp;
            }
    
        }

        public Estimate GetEstimateByID(string id)
        {
            NetSuiteService service;
            Estimate estimacion;

            RecordRef recordRef = new RecordRef();
            recordRef.type = RecordType.estimate;
            recordRef.typeSpecified = true;
            recordRef.internalId = id;

            service = ConnectionManager.GetNetSuiteService();
            estimacion = (Estimate)service.get(recordRef).record;

            return estimacion;
        }

        public RespuestaServicios TransformSalesOrderToInvoice(string SalesOrder)
        {
            try
            {
                RespuestaServicios resp = new RespuestaServicios();
                NetSuiteService service = new NetSuiteService();
                InitializeRef salesOrderRef = new InitializeRef();
                InitializeRecord records = new InitializeRecord();

                service = ConnectionManager.GetNetSuiteService();

                salesOrderRef.internalId = SalesOrder;
                salesOrderRef.type = InitializeRefType.salesOrder;
                salesOrderRef.typeSpecified = true;

                records.reference = salesOrderRef;
                records.type = InitializeType.invoice;

                ReadResponse leerRespuesta = service.initialize(records);

                Invoice invoice = (Invoice)leerRespuesta.record;

                //Creamos lista de campos personalizados
                List<SelectCustomFieldRef> l_customField = new List<SelectCustomFieldRef>();

                //Campo Metodo Pago SAT
                ListOrRecordRef custSelectValue = new ListOrRecordRef();
                //custSelectValue.internalId = "9";
                custSelectValue.internalId = "102";
                SelectCustomFieldRef selectCustomFieldRef = new SelectCustomFieldRef();
                //selectCustomFieldRef.internalId = "2064";
                selectCustomFieldRef.value = custSelectValue;
                selectCustomFieldRef.scriptId = "custbody_psg_ei_template";

                l_customField.Add(selectCustomFieldRef);

                CustomFieldRef[] customFieldRef = new CustomFieldRef[0];
                customFieldRef = l_customField.ToArray();
                invoice.customFieldList = customFieldRef;

                RecordRef customer = new RecordRef();
                customer.internalId = invoice.entity.internalId;
                customer.type = RecordType.customer;
                customer.typeSpecified = true;

                invoice.entity = customer;

                service = ConnectionManager.GetNetSuiteService();
                WriteResponse writeResponse = service.add(invoice);

                if (writeResponse.status.isSuccess)
                {
                    resp.Estatus = "Exito";
                    resp.Valor = writeResponse.baseRef;
                    return resp;
                }
                else
                {
                    resp.Estatus = "Fracaso";
                    resp.Mensaje = string.Format("Hubo un problema al crear la Factura de Venta. Detalle: {0}", writeResponse.status.statusDetail[0].message);
                    return resp;
                }

            }catch (Exception ex){
                RespuestaServicios resp = new RespuestaServicios();
                resp.Estatus = "Fracaso";
                resp.Valor = String.Format("Hubo un Problema al Crear la Factura de Venta. Detalles: {0}.", ex.Message);
                return resp;
            }
            
        }

        public WriteResponse DeleteSalesOrder(string custId)
        {
            NetSuiteService service;
            RecordRef SalesOrderRef = new RecordRef();

            service = ConnectionManager.GetNetSuiteService();

            SalesOrderRef.internalId = custId;
            SalesOrderRef.type = RecordType.salesOrder;
            SalesOrderRef.typeSpecified = true;

            WriteResponse writeResponse = service.delete(SalesOrderRef, null);

            return writeResponse;
        }

        public WriteResponse UpdateSalesOrder(string custId)
        {
            SalesOrder salesOrder = new SalesOrder();
            salesOrder.internalId = custId;
            NetSuiteService service;

            RecordRef Cliente = new RecordRef();
            Cliente.internalId = "7236";
            Cliente.type = RecordType.customer;
            Cliente.typeSpecified = true;

            salesOrder.memo = ".";
            salesOrder.entity = Cliente;

            //ListOrRecordRef custSelectValue = new ListOrRecordRef();
            //custSelectValue.internalId = "4";

            //SelectCustomFieldRef selectCustomFieldRef = new SelectCustomFieldRef();
            //selectCustomFieldRef.internalId = "2064";
            //selectCustomFieldRef.value = custSelectValue;
            //selectCustomFieldRef.scriptId = "custbody_mx_txn_sat_payment_term";

            //List<CustomFieldRef> l_value = new List<CustomFieldRef>();
            //l_value.Add(selectCustomFieldRef);

            //CustomFieldRef[] customFieldRef = new CustomFieldRef[0];
            //customFieldRef = l_value.ToArray();
            //salesOrder.customFieldList = customFieldRef;

            service = ConnectionManager.GetNetSuiteService();
                
            WriteResponse response = service.update(salesOrder);

           return response;
        }

        public RespuestaServicios TransformSalesOrderToReturnAutorization(string SalesOrder)
        {
            try
            {
                RespuestaServicios resp = new RespuestaServicios();
                NetSuiteService service = new NetSuiteService();
                InitializeRef invoiceRef = new InitializeRef();
                InitializeRecord records = new InitializeRecord();

                service = ConnectionManager.GetNetSuiteService();

                invoiceRef.internalId = SalesOrder;
                invoiceRef.type = InitializeRefType.invoice;
                invoiceRef.typeSpecified = true;

                records.reference = invoiceRef;
                records.type = InitializeType.returnAuthorization;

                ReadResponse leerRespuesta = service.initialize(records);

                ReturnAuthorization retornoMercancia = (ReturnAuthorization) leerRespuesta.record;

                retornoMercancia.customFieldList = retornoMercancia.customFieldList.Where(customsfields => customsfields.scriptId != "custbody_psg_ei_template").ToArray();

                //retornoMercancia.customFieldList[0].internalId;

                service = ConnectionManager.GetNetSuiteService();
                WriteResponse writeResponse = service.add(retornoMercancia);

                if (writeResponse.status.isSuccess)
                {
                    resp.Estatus = "Exito";
                    resp.Valor = writeResponse.baseRef;
                    return resp;
                }
                else
                {
                    resp.Estatus = "Fracaso";
                    resp.Mensaje = string.Format("Hubo un problema al crear la Factura de Venta. Detalle: {0}", writeResponse.status.statusDetail[0].message);
                    return resp;
                }

            }
            catch (Exception ex)
            {
                RespuestaServicios resp = new RespuestaServicios();
                resp.Estatus = "Fracaso";
                resp.Valor = String.Format("Hubo un Problema al Crear la Factura de Venta. Detalles: {0}.", ex.Message);
                return resp;
            }

        }

    }

}
