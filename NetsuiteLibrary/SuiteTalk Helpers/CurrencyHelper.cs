using NetsuiteLibrary.SuiteTalk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetsuiteLibrary.SuiteTalk_Helpers
{
    public class CurrencyHelper
    {
        public Currency GetCurrencyBySymbol(string currencySymbol)
        {
            // NetSuite Service class
            NetSuiteService service;

            // Search results
            GetAllResult searchResults;

            // Currency record
            GetAllRecord currencyGetAll = new GetAllRecord();
            currencyGetAll.recordType = GetAllRecordType.currency;
            currencyGetAll.recordTypeSpecified = true;

            // Authenticate and get the NetSuite service
            service = ConnectionManager.GetNetSuiteService();

            // Perform the search
            searchResults = service.getAll(currencyGetAll);

            List<Currency> records = searchResults.recordList.ToList().Cast<Currency>().ToList();

            Currency currency = records.Find(x => x.symbol == currencySymbol);
            
            return currency;
        }

        public RecordRef GetCurrencyRefBySymbol(string currencySymbol)
        {
            // NetSuite Service class
            NetSuiteService service;

            // Search results
            GetAllResult searchResults;

            // Currency record
            GetAllRecord currencyGetAll = new GetAllRecord();
            currencyGetAll.recordType = GetAllRecordType.currency;
            currencyGetAll.recordTypeSpecified = true;

            // Authenticate and get the NetSuite service
            service = ConnectionManager.GetNetSuiteService();

            // Perform the search
            searchResults = service.getAll(currencyGetAll);

            List<Currency> records = searchResults.recordList.ToList().Cast<Currency>().ToList();

            Currency currency = records.Find(x => x.symbol == currencySymbol);

            RecordRef currencyRef = new RecordRef();
            currencyRef.internalId = currency.internalId;
            currencyRef.type = RecordType.currency;
            currencyRef.typeSpecified = true;

            return currencyRef;
        }

        public List<Currency> GetAllCurrencies()
        {
            // NetSuite Service class
            NetSuiteService service;

            // Search results
            GetAllResult searchResults;

            // Currency record
            GetAllRecord currencyGetAll = new GetAllRecord();
            currencyGetAll.recordType = GetAllRecordType.currency;
            currencyGetAll.recordTypeSpecified = true;

            // Authenticate and get the NetSuite service
            service = ConnectionManager.GetNetSuiteService();

            // Perform the search
            searchResults = service.getAll(currencyGetAll);

            List<Currency> records = searchResults.recordList.ToList().Cast<Currency>().ToList();

            return records;
        }
    }

}
