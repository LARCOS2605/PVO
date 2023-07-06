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

    public class CreditMemoHelper
    {

        public WriteResponse TransformInvoiceToCreditMemo(string invoiceID)
        {
            NetSuiteService service;
            InitializeRef invoiceRef = new InitializeRef();
            InitializeRecord records = new InitializeRecord();

            service = ConnectionManager.GetNetSuiteService();

            //Obtenemos Factura de Ejemplo
            Invoice invoice = new InvoiceHelper().GetInvoiceByID(invoiceID);

            invoiceRef.internalId = invoiceID;
            invoiceRef.type = InitializeRefType.invoice;
            invoiceRef.typeSpecified = true;

            records.reference = invoiceRef;
            records.type = InitializeType.creditMemo;

            service = ConnectionManager.GetNetSuiteService();

            ReadResponse leerRespuesta = service.initialize(records);

            CreditMemo creditMemo = (CreditMemo) leerRespuesta.record;

            RecordRef ItemProntoPago = new RecordRef();
            ItemProntoPago.internalId = "36189";
            ItemProntoPago.type = RecordType.serviceSaleItem;
            ItemProntoPago.typeSpecified = true;

            CreditMemoItemList l_creditmemo = new CreditMemoItemList();
            List<CreditMemoItem> ArrayItems = new List<CreditMemoItem>();

            CreditMemoItem item = new CreditMemoItem();

            item.item = ItemProntoPago;
            item.quantity = 1;
            item.quantitySpecified = true;

            double MontoProntoPago = Math.Round(invoice.total * 0.10,2);
            item.amount = MontoProntoPago;
            item.amountSpecified = true;
            ArrayItems.Add(item);
            l_creditmemo.item = ArrayItems.ToArray();

            creditMemo.itemList = l_creditmemo;

            //creditMemo.itemList = Array.Empty();

            //invoice.memo = "Transacción por WS";
            service = ConnectionManager.GetNetSuiteService();
            WriteResponse writeResponse = service.add(creditMemo);

            return writeResponse;
        }

        public CreditMemo GetCreditMemoByID(string id)
        {
            NetSuiteService service = new NetSuiteService();
            CreditMemo invoice;

            RecordRef recordRef = new RecordRef();
            recordRef.type = RecordType.creditMemo;
            recordRef.typeSpecified = true;
            recordRef.internalId = id;

            service = ConnectionManager.GetNetSuiteService();
            invoice = (CreditMemo)service.get(recordRef).record;

            return invoice;
        }

        //public TransformSalesOrderToInvoice
    }

}
