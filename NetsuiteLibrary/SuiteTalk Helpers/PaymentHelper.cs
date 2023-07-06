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

    public class PaymentHelper
    {
        public CustomerPayment GetCustomerPaymentByID(string id)
        {
            NetSuiteService service;
            CustomerPayment payment;

            RecordRef recordRef = new RecordRef();
            recordRef.type = RecordType.customerPayment;
            recordRef.typeSpecified = true;
            recordRef.internalId = id;

            service = ConnectionManager.GetNetSuiteService();
            payment = (CustomerPayment) service.get(recordRef).record;

            return payment;
        }

        public PaymentMethod GetPaymenthMethodId(string custId)
        {
            NetSuiteService service;
            PaymentMethod payment;

            RecordRef recordRef = new RecordRef();
            recordRef.type = RecordType.paymentMethod;
            recordRef.typeSpecified = true;
            recordRef.internalId = custId;

            service = ConnectionManager.GetNetSuiteService();
            payment = (PaymentMethod)service.get(recordRef).record;

            return payment;
        }

        public List<RecordRef> GetAllPaymenthMethod()
        {
            NetSuiteService service;
            List<RecordRef> l_MetodosPago = new List<RecordRef>();
            GetSelectValueResult result = new GetSelectValueResult();
            GetSelectValueFieldDescription desc = new GetSelectValueFieldDescription();

            desc.field = "paymentMethod";
            desc.recordType = RecordType.salesOrder;
            desc.recordTypeSpecified = true;

            service = ConnectionManager.GetNetSuiteService();

            result = service.getSelectValue(desc,0);

            l_MetodosPago = result.baseRefList.Cast<RecordRef>().ToList();
            return l_MetodosPago;
        }
        
        public RespuestaServicios InsertCustomerPayment(AltaPagos Model)
        {
            try
            {
                //Declaracion de Respuesta
                RespuestaServicios resp = new RespuestaServicios();

                //Declaracion de servicio
                NetSuiteService service;

                //Declaracion de alta de ingreso
                CustomerPayment customerPayment = new CustomerPayment();

                //Creamos lista de campos personalizados
                List<SelectCustomFieldRef> l_customField = new List<SelectCustomFieldRef>();

                //Referencia Cliente
                RecordRef customerRef = new RecordRef();
                customerRef.type = RecordType.customer;
                customerRef.typeSpecified = true;
                customerRef.internalId = Model.InternalID_Customer;

                //Referencia Metodo de Pago
                RecordRef paymentMethodRef = new RecordRef();
                paymentMethodRef.type = RecordType.paymentMethod;
                paymentMethodRef.typeSpecified = true;
                paymentMethodRef.internalId = Model.InternalID_MetodoPago;

                //Campo Forma Pago SAT
                ListOrRecordRef regForPagoSelectValue = new ListOrRecordRef();
                regForPagoSelectValue.internalId = Model.MetodoPagoSAT;
                SelectCustomFieldRef selectForPagoCustomFieldRef = new SelectCustomFieldRef();
                selectForPagoCustomFieldRef.internalId = "2063";
                selectForPagoCustomFieldRef.value = regForPagoSelectValue;
                selectForPagoCustomFieldRef.scriptId = "custbody_mx_txn_sat_payment_method";

                l_customField.Add(selectForPagoCustomFieldRef);

                if (Model.IndicadorPlantilla == "True")
                {
                //Campo Plantilla Timbrado
                ListOrRecordRef custSelectValue = new ListOrRecordRef();
                custSelectValue.internalId = "11";
                SelectCustomFieldRef selectCustomFieldRef = new SelectCustomFieldRef();
                selectCustomFieldRef.value = custSelectValue;
                selectCustomFieldRef.scriptId = "custbody_psg_ei_template";

                l_customField.Add(selectCustomFieldRef);
                }

                //Campo Uso de CFDI SAT
                //ListOrRecordRef regUsoCFDISelectValue = new ListOrRecordRef();
                //regUsoCFDISelectValue.internalId = ""; //pagos 20 siempre sera CP01
                //SelectCustomFieldRef selectUsoCFDICustomFieldRef = new SelectCustomFieldRef();
                //selectUsoCFDICustomFieldRef.internalId = "2061";
                //selectUsoCFDICustomFieldRef.value = regUsoCFDISelectValue;
                //selectUsoCFDICustomFieldRef.scriptId = "custbody_mx_cfdi_usage";

                //l_customField.Add(selectUsoCFDICustomFieldRef);

                //Referencia Localizacion
                RecordRef LocationRef = new DimensionHelper().GetLocation(Model.Localizacion);

                //Declaracion de pagos
                List<CustomerPaymentApply> ArrayPagos = new List<CustomerPaymentApply>();
                CustomerPaymentApplyList l_apply = new CustomerPaymentApplyList();

                //Mapeo de conceptos de pago
                foreach (DetalleAltaPagos RegDetallePago in Model.DetallePagos)
                {
                    CustomerPaymentApply applypay = new CustomerPaymentApply();
                    //Invoice InvoiceRef = new InvoiceHelper().GetInvoiceByID(RegDetallePago.InternalID_Factura);

                    applypay.apply = true;
                    applypay.doc = long.Parse(RegDetallePago.InternalID_Factura);
                    applypay.docSpecified = true;
                    applypay.amount = RegDetallePago.Monto;
                    applypay.amountSpecified = true;
                    applypay.applyDate = DateTime.Now;
                    applypay.applyDateSpecified = true;
                    applypay.applySpecified = true;

                    ArrayPagos.Add(applypay);
                }

                l_apply.apply = ArrayPagos.ToArray();
                l_apply.replaceAll = true;

                //Campos de encabezado
                customerPayment.memo = Model.memo;
                customerPayment.tranDate = Model.FechaPago;
                customerPayment.tranDateSpecified = true;
                customerPayment.customer = customerRef;
                customerPayment.location = LocationRef;
                customerPayment.paymentMethod = paymentMethodRef;

                CustomFieldRef[] customFieldRef = new CustomFieldRef[0];
                customFieldRef = l_customField.ToArray();

                if ((Model.TipoMetodoPago == "Transferencia") || (Model.TipoMetodoPago == "Tarjeta Credito") || (Model.TipoMetodoPago == "Tarjeta Debito") || (Model.TipoMetodoPago == "Cheque"))
                {
                    //Banco Beneficiario
                    ListOrRecordRef custBancosBeneficiario = new ListOrRecordRef();
                    custBancosBeneficiario.internalId = Model.NS_ID_BancoBeneficiario;
                    SelectCustomFieldRef selectBancoBeneficiarioFieldRef = new SelectCustomFieldRef();
                    //selectBancoBeneficiarioFieldRef.internalId = "1213";
                    selectBancoBeneficiarioFieldRef.value = custBancosBeneficiario;
                    selectBancoBeneficiarioFieldRef.scriptId = "custbody_maf_bnk_benf";
                    customFieldRef = customFieldRef.Append(selectBancoBeneficiarioFieldRef).ToArray();

                    //Banco Ordenante
                    ListOrRecordRef custBancosOrdenante = new ListOrRecordRef();
                    custBancosOrdenante.internalId = Model.NS_ID_BancoOrdenante;
                    SelectCustomFieldRef selectBancoOrdenanteFieldRef = new SelectCustomFieldRef();
                    //selectBancoOrdenanteFieldRef.internalId = "1213";
                    selectBancoOrdenanteFieldRef.value = custBancosOrdenante;
                    selectBancoOrdenanteFieldRef.scriptId = "custbody_maf_bnk_ord";
                    customFieldRef = customFieldRef.Append(selectBancoOrdenanteFieldRef).ToArray();

                    //RFC Banco Ordenante
                    StringCustomFieldRef stringCustomFieldRFCBancOrd = new StringCustomFieldRef();
                    stringCustomFieldRFCBancOrd.scriptId = "custbody_mx_cfdi_issuer_entity_rfc";
                    stringCustomFieldRFCBancOrd.value = Model.RFCBanco_Ordenante;
                    customFieldRef = customFieldRef.Append(stringCustomFieldRFCBancOrd).ToArray();

                    //Cuenta Ordenante
                    StringCustomFieldRef stringCustomFieldCtaOrd = new StringCustomFieldRef();
                    stringCustomFieldCtaOrd.scriptId = "custbody_mx_cfdi_payer_account";
                    stringCustomFieldCtaOrd.value = Model.NumCuenta_Ordenante;
                    customFieldRef = customFieldRef.Append(stringCustomFieldCtaOrd).ToArray();               

                    //RFC Banco Beneficiario
                    StringCustomFieldRef stringCustomFieldRefBancBenf = new StringCustomFieldRef();
                    stringCustomFieldRefBancBenf.scriptId = "custbody_mx_cfdi_recipient_entity_rfc";
                    stringCustomFieldRefBancBenf.value = Model.RFCBanco_Beneficiario;
                    customFieldRef = customFieldRef.Append(stringCustomFieldRefBancBenf).ToArray();

                    //Cuenta Beneficiario
                    StringCustomFieldRef stringCustomFieldCtaBen = new StringCustomFieldRef();
                    stringCustomFieldCtaBen.scriptId = "custbody_mx_cfdi_recipient_account";
                    stringCustomFieldCtaBen.value = Model.NumCuenta_Beneficiario;
                    customFieldRef = customFieldRef.Append(stringCustomFieldCtaBen).ToArray();

                    //Cuenta Beneficiario
                    StringCustomFieldRef stringCustomFieldPayID = new StringCustomFieldRef();
                    stringCustomFieldPayID.scriptId = "custbody_mx_cfdi_payment_id";
                    stringCustomFieldPayID.value = Model.NumTransaccion;
                    customFieldRef = customFieldRef.Append(stringCustomFieldPayID).ToArray();

                    if ((Model.TipoMetodoPago == "Tarjeta Credito") || (Model.TipoMetodoPago == "Tarjeta Debito")){
                        //escenario Autorizacion terminal
                        StringCustomFieldRef stringCustomFieldterminal = new StringCustomFieldRef();
                        stringCustomFieldterminal.scriptId = "custbody_maf_aut_tarj";
                        stringCustomFieldterminal.value = Model.Terminal;
                        customFieldRef = customFieldRef.Append(stringCustomFieldterminal).ToArray();
                    }

                }


                //customerPayment.ccApproved = true;
                customerPayment.applyList = l_apply;
                customerPayment.customFieldList = customFieldRef;
                

                //Lista de items solicitados
                service = ConnectionManager.GetNetSuiteService();

                WriteResponse writeResponse = service.add(customerPayment);

                if (writeResponse.status.isSuccess)
                {
                    resp.Estatus = "Exito";
                    resp.Valor = writeResponse.baseRef;
                    return resp;
                }
                else
                {
                    resp.Estatus = "Fracaso";
                    resp.Mensaje = string.Format(writeResponse.status.statusDetail[0].message);
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

        public List<Invoice> GetListPaymentFromLocationID(string id)
        {

            NetSuiteService service;

            LocationSearchBasic location = new LocationSearchBasic();

            RecordRef Location = new RecordRef();
            Location.type = RecordType.location;
            Location.internalId = id;
            Location.typeSpecified = true;
            RecordRef[] custRecordRefArray = new RecordRef[1];
            custRecordRefArray[0] = Location;
            SearchMultiSelectField searchLocation = new SearchMultiSelectField();
            searchLocation.@operator = SearchMultiSelectFieldOperator.anyOf;
            searchLocation.operatorSpecified = true;
            searchLocation.searchValue = custRecordRefArray;
            location.internalId = searchLocation;

            TransactionSearchBasic Transaction = new TransactionSearchBasic();
            SearchEnumMultiSelectField searchMultiSelectField = new SearchEnumMultiSelectField();
            searchMultiSelectField.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            searchMultiSelectField.operatorSpecified = true;
            searchMultiSelectField.searchValue = new string[] { "_customerPayment" };
            Transaction.type = searchMultiSelectField;
            //Transaction.dateCreated = 

            TransactionSearch BusquedaTransaccion = new TransactionSearch();
            BusquedaTransaccion.basic = Transaction;
            BusquedaTransaccion.locationJoin = location;

            service = ConnectionManager.GetNetSuiteService();

            SearchResult searchResult = service.search(BusquedaTransaccion);

            List<Invoice> recordList = searchResult.recordList.Cast<Invoice>().ToList();

            return recordList;
        }

    }

}
