using NetsuiteLibrary.SuiteTalk;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetsuiteLibrary.SuiteTalk_Helpers
{
    public class DimensionHelper
    {
        public string GetSubsidiaryJSONByName(string subsidiaryName)
        {
            // NetSuite Service class
            NetSuiteService service;

            // Search results
            SearchResult searchResults;

            // Subsidiary record as JSON
            string subsidiaryJSON;

            subsidiaryJSON = string.Empty;

            // Authenticate and get the NetSuite service
            service = ConnectionManager.GetNetSuiteService();

            // Subsidary search object
            SubsidiarySearch subsidiarySearch = new SubsidiarySearch();
            SubsidiarySearchBasic subsidiarySearchBasic = new SubsidiarySearchBasic();
            SearchStringField subsidiaryNameField = new SearchStringField();

            // Filter subsidiary search by name
            subsidiaryNameField.@operator = SearchStringFieldOperator.contains;
            subsidiaryNameField.operatorSpecified = true;
            subsidiaryNameField.searchValue = subsidiaryName;
            subsidiarySearchBasic.name = subsidiaryNameField;

            subsidiarySearch.basic = subsidiarySearchBasic;

            // Perform the search
            searchResults = service.search(subsidiarySearch);

            // Loop through the results on the record list
            subsidiaryJSON = JsonConvert.SerializeObject((Subsidiary)searchResults.recordList.FirstOrDefault());

            return subsidiaryJSON;
        }

        public Subsidiary GetSubsidiaryByName(string subsidiaryName)
        {
            // NetSuite Service class
            NetSuiteService service;

            // Search results
            SearchResult searchResults;

            // Subsidiary record
            Subsidiary subsidiary = new Subsidiary();

            // Authenticate and get the NetSuite service
            service = ConnectionManager.GetNetSuiteService();

            // Subsidary search object
            SubsidiarySearch subsidiarySearch = new SubsidiarySearch();
            SubsidiarySearchBasic subsidiarySearchBasic = new SubsidiarySearchBasic();
            SearchStringField subsidiaryNameField = new SearchStringField();

            // Filter subsidiary search by name
            subsidiaryNameField.@operator = SearchStringFieldOperator.@is;
            subsidiaryNameField.operatorSpecified = true;
            subsidiaryNameField.searchValue = subsidiaryName;
            subsidiarySearchBasic.name = subsidiaryNameField;

            subsidiarySearch.basic = subsidiarySearchBasic;

            // Perform the search
            searchResults = service.search(subsidiarySearch);

            subsidiary = (Subsidiary)searchResults.recordList.FirstOrDefault();

            return subsidiary;
        }

        public Subsidiary GetSubsidiaryById(string subsidiaryId)
        {
            NetSuiteService service;
            service = ConnectionManager.GetNetSuiteService();

            RecordRef recordRef = new RecordRef();
            recordRef.type = RecordType.subsidiary;
            recordRef.typeSpecified = true;
            recordRef.internalId = subsidiaryId;

            Subsidiary subsidiary = (Subsidiary) service.get(recordRef).record;

            return subsidiary;
        }

        public RecordRef GetDepartment(string departmentName)
        {
            NetSuiteService service;
            DepartmentSearch departmentSearch = new DepartmentSearch();
            DepartmentSearchBasic departmentSearchBasic = new DepartmentSearchBasic();
            SearchStringField departmentNameField = new SearchStringField();

            departmentNameField.@operator = SearchStringFieldOperator.contains;
            departmentNameField.operatorSpecified = true;
            departmentNameField.searchValue = departmentName;

            departmentSearchBasic.name = departmentNameField;
            departmentSearch.basic = departmentSearchBasic;

            service = ConnectionManager.GetNetSuiteService();

            SearchResult searchResult = service.search(departmentSearch);
            Department department = (Department) searchResult.recordList.FirstOrDefault();

            RecordRef departmentRef = new RecordRef();
            departmentRef.internalId = department.internalId;
            departmentRef.type = RecordType.department;
            departmentRef.typeSpecified = true;

            return departmentRef;
        }

        public RecordRef GetLocation(string locationName)
        {
            NetSuiteService service;
            LocationSearch locationSearch = new LocationSearch();
            LocationSearchBasic locationSearchBasic = new LocationSearchBasic();
            SearchStringField locationNameField = new SearchStringField();

            locationNameField.@operator = SearchStringFieldOperator.contains;
            locationNameField.operatorSpecified = true;
            locationNameField.searchValue = locationName;

            locationSearchBasic.name = locationNameField;
            locationSearch.basic = locationSearchBasic;

            service = ConnectionManager.GetNetSuiteService();

            SearchResult searchResult = service.search(locationSearch);
            Location location = (Location) searchResult.recordList.FirstOrDefault();

            RecordRef locationRef = new RecordRef();
            locationRef.internalId = location.internalId;
            locationRef.type = RecordType.location;
            locationRef.typeSpecified = true;

            return locationRef;
        }

        public List<Location> GetAllLocation()
        {
            NetSuiteService service;

            List<Location> recordList = new List<Location>();
            LocationSearch locationSearch = new LocationSearch();
            LocationSearchBasic locationSearchBasic = new LocationSearchBasic();
            SearchEnumMultiSelectField locationStage = new SearchEnumMultiSelectField();

            locationStage.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            locationStage.operatorSpecified = true;
            locationStage.searchValue = new string[] { "_location" };

            locationSearchBasic.locationType = locationStage;
            locationSearch.basic = locationSearchBasic;

            service = ConnectionManager.GetNetSuiteService();
            SearchResult searchResult = service.search(locationSearch);

            recordList = searchResult.recordList.Cast<Location>().ToList();
            return recordList;
        }

        public RecordRef GetClass(string classificationName)
        {
            NetSuiteService service;
            ClassificationSearch classSearch = new ClassificationSearch();
            ClassificationSearchBasic classSearchBasic = new ClassificationSearchBasic();
            SearchStringField classNameField = new SearchStringField();

            classNameField.@operator = SearchStringFieldOperator.contains;
            classNameField.operatorSpecified = true;
            classNameField.searchValue = classificationName;

            classSearchBasic.name = classNameField;
            classSearch.basic = classSearchBasic;

            service = ConnectionManager.GetNetSuiteService();

            SearchResult searchResult = service.search(classSearch);
            Classification classification = (Classification) searchResult.recordList.FirstOrDefault();

            RecordRef classRef = new RecordRef();
            classRef.internalId = classification.internalId;
            classRef.type = RecordType.classification;
            classRef.typeSpecified = true;

            return classRef;
        }
    }

}
