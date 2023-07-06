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

    public class CashHelper
    {
        public RecordRef InsertCashSale()
        {
            //Declaracion de servicio
            NetSuiteService service;

            //Declaracion de alta de ingreso
            CashSale cashSalealta = new CashSale();

            Customer customer = new CustomerHelper().GetCustomerById("335");

            //Referencia
            RecordRef customerRef = new RecordRef();

            customerRef.type = RecordType.customer;
            customerRef.typeSpecified = true;
            customerRef.internalId = customer.internalId;
            customerRef.externalId = customer.externalId;

            RecordRef LocationRef = new DimensionHelper().GetLocation("INVENTARIO DE PRODUCTO TERMINADO : ACAPULCO");

            //Declaracion de mercancia o concepto
            CashSaleItem item = new CashSaleItem();
            CashSaleItemList l_cashSales = new CashSaleItemList();

            InventoryItem items = new ItemHelper().GetItemByID("428");

            //ItemRef Datos
            RecordRef ItemRef = new RecordRef();
            ItemRef.type = RecordType.inventoryItem;
            ItemRef.typeSpecified = true;
            ItemRef.internalId = items.internalId;
            ItemRef.externalId = items.externalId;

            item.item = ItemRef;
            item.quantity = 1;
            l_cashSales.item = new[] { item };

            //Campos de encabezado
            cashSalealta.memo = "prueba3";
            cashSalealta.entity = customerRef;
            cashSalealta.location = LocationRef;
            cashSalealta.itemList = l_cashSales;
            cashSalealta.tranDate = DateTime.Now;

            //Lista de items solicitados
            service = ConnectionManager.GetNetSuiteService();

            WriteResponse writeResponse = service.add(cashSalealta);

            return (RecordRef)writeResponse.baseRef;
        }

        public CashSale GetCashSaleByID(string id)
        {
            NetSuiteService service;
            CashSale cashSale;

            RecordRef recordRef = new RecordRef();
            recordRef.type = RecordType.cashSale;
            recordRef.typeSpecified = true;
            recordRef.internalId = id;

            service = ConnectionManager.GetNetSuiteService();
            cashSale = (CashSale) service.get(recordRef).record;

            return cashSale;
        }

        public WriteResponse DeleteCashSale(string custId)
        {
            NetSuiteService service;
            RecordRef cashSaleRef = new RecordRef();

            service = ConnectionManager.GetNetSuiteService();

            cashSaleRef.internalId = custId;
            cashSaleRef.type = RecordType.cashSale;
            cashSaleRef.typeSpecified = true;

            WriteResponse writeResponse = service.delete(cashSaleRef, null);

            return writeResponse;
        }

        public WriteResponse TransformCashSaleCustomTransaction(string cashSaleid)
        {
            NetSuiteService service;
            CashSale cashSale = new CashHelper().GetCashSaleByID(cashSaleid);
            InitializeRef cashSaleRef = new InitializeRef();
            InitializeRecord records = new InitializeRecord();

            service = ConnectionManager.GetNetSuiteService();

            cashSaleRef.internalId = cashSale.internalId;
            cashSaleRef.type = InitializeRefType.cashSale;
            cashSaleRef.typeSpecified = true;

            //cashSale.memo = ""

            records.reference = cashSaleRef;
            records.type = InitializeType.salesOrder;

            ReadResponse leerRespuesta = service.initialize(records);

            Invoice invoice = (Invoice) leerRespuesta.record;

            //invoice.memo = "Transacción por WS";

            WriteResponse writeResponse = service.add(invoice);

            return writeResponse;
        }

        //public TransformSalesOrderToInvoice
    }

}
