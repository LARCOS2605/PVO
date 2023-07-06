using NetsuiteLibrary.SuiteTalk;
using System.Linq;
using System.Collections.Generic;
using NetsuiteLibrary.Clases;

namespace NetsuiteLibrary.SuiteTalk_Helpers
{
    public class AccountHelper
    {
        public Account GetAccountByName(string accountName)
        {
            NetSuiteService service;
            AccountSearch accountSearch = new AccountSearch();
            AccountSearchBasic accountSearchBasic = new AccountSearchBasic();
            SearchStringField accountNameField = new SearchStringField();

            accountNameField.@operator = SearchStringFieldOperator.contains;
            accountNameField.operatorSpecified = true;
            accountNameField.searchValue = accountName;

            accountSearchBasic.name = accountNameField;
            accountSearch.basic = accountSearchBasic;

            service = ConnectionManager.GetNetSuiteService();

            SearchResult searchResult = service.search(accountSearch);

            Account account = (Account) searchResult.recordList.FirstOrDefault();

            return account;
        }

        public Account GetAccountByNameAndType(string accountName, AccountType accountType)
        {
            NetSuiteService service;
            AccountSearch accountSearch = new AccountSearch();
            AccountSearchBasic accountSearchBasic = new AccountSearchBasic();
            SearchStringField accountNameField = new SearchStringField();
            SearchEnumMultiSelectField accountTypeField = new SearchEnumMultiSelectField();

            accountNameField.@operator = SearchStringFieldOperator.contains;
            accountNameField.operatorSpecified = true;
            accountNameField.searchValue = accountName;

            accountTypeField.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            accountTypeField.operatorSpecified = true;
            accountTypeField.searchValue = new[] { accountType.ToString()};

            accountSearchBasic.name = accountNameField;
            accountSearchBasic.type = accountTypeField;
            accountSearch.basic = accountSearchBasic;

            service = ConnectionManager.GetNetSuiteService();

            SearchResult searchResult = service.search(accountSearch);

            Account account = (Account)searchResult.recordList.FirstOrDefault();

            return account;
        }

    }
}
