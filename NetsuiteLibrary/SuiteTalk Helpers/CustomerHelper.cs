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
    using System.Net;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.VisualBasic;
    using NetsuiteLibrary.Clases;
    using NetsuiteLibrary.SuiteTalk;

    public class CustomerHelper
    {
        public WriteResponse CreateCustomer()
        {
            NetSuiteService service;
            RecordRef subsidiaryRef = new RecordRef();
            DimensionHelper subsidiaryHelper = new DimensionHelper();
            Customer customer = new Customer();
            List<CustomerStatus> statusList = new List<CustomerStatus>();
            RecordRef entityStatus = new RecordRef();
            CustomerStatus customerStatus;
            CustomerAddressbookList customerAddressbookList = new CustomerAddressbookList();
            CustomerAddressbook customerAddressBook = new CustomerAddressbook();
            List<CustomerAddressbook> customerAddressBookArray = new List<CustomerAddressbook>();
            Address address = new Address();
            Subsidiary subsidiary = subsidiaryHelper.GetSubsidiaryByName("Parent Company");
            CurrencyHelper currencyHelper = new CurrencyHelper();
            Currency currency;
            RecordRef currencyRef = new RecordRef();
            List<CustomerCategory> categoryList = new List<CustomerCategory>();
            CustomerCategory category;
            RecordRef categoryRef = new RecordRef();
            List<CustomFieldRef> customFieldList = new List<CustomFieldRef>();
            StringCustomFieldRef mxRFCFieldRef = new StringCustomFieldRef();

            subsidiaryRef.type = RecordType.subsidiary;
            subsidiaryRef.typeSpecified = true;
            subsidiaryRef.internalId = subsidiary.internalId;

            currency = currencyHelper.GetCurrencyBySymbol("MXN");
            currencyRef.type = RecordType.currency;
            currencyRef.typeSpecified = true;
            currencyRef.internalId = currency.internalId;

            statusList = GetEntityStatus();
            customerStatus = statusList.Find(x => x.name == "Closed Won");
            entityStatus.type = RecordType.customerStatus;
            entityStatus.typeSpecified = true;
            entityStatus.internalId = customerStatus.internalId;

            categoryList = GetCategory();
            category = categoryList.Find(x => x.name == "Corporate");
            categoryRef.type = RecordType.customerCategory;
            categoryRef.typeSpecified = true;
            categoryRef.internalId = category.internalId;

            customer.isPerson = false;
            customer.companyName = "PSLAD Company";
            customer.entityStatus = entityStatus;
            customer.phone = "8332280362";
            customer.altPhone = "8332280363";
            customer.email = "test@test.com";
            customer.currency = currencyRef;
            customer.subsidiary = subsidiaryRef;
            customer.externalId = "4";

            address.country = Country._mexico;
            address.state = "Toluca";
            address.city = "EDOMX";
            address.zip = "91090";
            address.addr1 = "JOSE MANCISIDOR 789";
            address.countrySpecified = true;

            customerAddressBook.addressbookAddress = address;
            customerAddressBook.defaultBilling = true;
            customerAddressBook.defaultBillingSpecified = true;
            customerAddressBook.defaultShipping = true;
            customerAddressBook.defaultShippingSpecified = true;

            customerAddressBookArray.Add(customerAddressBook);
            customerAddressbookList.addressbook = customerAddressBookArray.ToArray();

            mxRFCFieldRef.scriptId = "custentity_mx_rfc";
            mxRFCFieldRef.value = "SIGR280101MR2";
            Contact s = new Contact();

            customFieldList.Add(mxRFCFieldRef);

            customer.customFieldList = customFieldList.ToArray();
       
            ContactAccessRolesList n = new ContactAccessRolesList();
            //s.contactRoles
            //customer.contactRolesList = n;

            //s.company = 
            ////customer.contactRolesList


            service = ConnectionManager.GetNetSuiteService();

            WriteResponse writeResponse = service.add(customer);

            return writeResponse;
        }

        public RespuestaServicios InsertCustomer(AltaCustomer altaCliente)
        {
            try
            {
                //Declaracion de Respuesta
                RespuestaServicios resp = new RespuestaServicios();
                Address address = new Address();
                CustomerAddressbookList customerAddressbookList = new CustomerAddressbookList();
                CustomerAddressbook customerAddressBook = new CustomerAddressbook();
                List<CustomerAddressbook> customerAddressBookArray = new List<CustomerAddressbook>();

                ContactAccessRoles customerPago = new ContactAccessRoles();
                List<ContactAccessRoles> customerPagoArray = new List<ContactAccessRoles>();

                //Declaracion de servicio
                NetSuiteService service;

                //Declaracion de alta de ingreso
                Customer ClienteAlta = new Customer();

                //Creamos lista de campos personalizados
                List<SelectCustomFieldRef> l_customField = new List<SelectCustomFieldRef>();
                List<StringCustomFieldRef> l_adrresbook = new List<StringCustomFieldRef>();

                if (altaCliente.PersonaFisica == "true")
                {
                    ClienteAlta.companyName = altaCliente.Nombre_Empresa.ToUpper();
                    //Nombre
                    ClienteAlta.firstName = ".";

                    //Apellido
                    ClienteAlta.lastName = "";

                    ClienteAlta.isPerson = false;
                    ClienteAlta.isPersonSpecified = true;
                }
                else if (altaCliente.PersonaFisica == "false")
                {
                    //Nombre
                    ClienteAlta.firstName = altaCliente.Nombre.ToUpper();

                    //Apellido
                    ClienteAlta.lastName = altaCliente.Apellido.ToUpper();
                    ClienteAlta.companyName = altaCliente.Nombre.ToUpper() + " " + altaCliente.Apellido.ToUpper();

                    ClienteAlta.isPerson = true;
                    ClienteAlta.isPersonSpecified = true;
                }

                //Email
                ClienteAlta.email = altaCliente.Correo;

                //Telefono
                ClienteAlta.phone = altaCliente.Telefono;

                //Celular
                ClienteAlta.mobilePhone = altaCliente.Celular;

                //Creamos una Referencia al Regimen Fiscal
                ListOrRecordRef custSelectValue = new ListOrRecordRef();
                custSelectValue.internalId = altaCliente.InternalID_RegimenFiscal;
                SelectCustomFieldRef selectCustomFieldRef = new SelectCustomFieldRef();
                selectCustomFieldRef.value = custSelectValue;
                selectCustomFieldRef.scriptId = "custentity_mx_sat_industry_type";

                l_customField.Add(selectCustomFieldRef);

                //Creamos una Referencia a la Ubicacion del Mostrador
                ListOrRecordRef custSelectValueMos = new ListOrRecordRef();
                custSelectValueMos.internalId = altaCliente.InternalId_Mostrador;
                SelectCustomFieldRef selectCustomFieldMos = new SelectCustomFieldRef();
                selectCustomFieldMos.value = custSelectValueMos;
                selectCustomFieldMos.scriptId = "custentity_maf_most_asig";

                l_customField.Add(selectCustomFieldMos);

                //Paquete de documentos
                ListOrRecordRef custSelectValuePaquete = new ListOrRecordRef();
                custSelectValuePaquete.internalId = "3";
                SelectCustomFieldRef selectCustomFieldPaq = new SelectCustomFieldRef();
                selectCustomFieldPaq.value = custSelectValuePaquete;
                selectCustomFieldPaq.internalId = "1445";
                selectCustomFieldPaq.scriptId = "custentity_psg_ei_entity_edoc_standard";

                l_customField.Add(selectCustomFieldPaq);

                //Contacto
                //RecordRef entityContact = new RecordRef();
                //entityContact.type = RecordType.contact;
                //entityContact.typeSpecified = true;
                //entityContact.internalId = "11527";
         
                //customerPago.contact = entityContact;
                //customerPago.giveAccess = true;
                //customerPago.giveAccessSpecified = true;
                //customerPago.email = "prueba@prueba.com";
                
                //customerPagoArray.Add(customerPago);

                //ContactAccessRolesList RolContact = new ContactAccessRolesList();
                //RolContact.contactRoles = customerPagoArray.ToArray();

                //ClienteAlta.contactRolesList = RolContact;

                l_customField.Add(selectCustomFieldPaq);

                //Campo Arreglo Custom Field
                CustomFieldRef[] customFieldRef = new CustomFieldRef[0];
                customFieldRef = l_customField.ToArray();
               
                //pdf
                BooleanCustomFieldRef stringCustomFieldPdf = new BooleanCustomFieldRef();
                stringCustomFieldPdf.scriptId = "custentity_edoc_gen_trans_pdf";
                stringCustomFieldPdf.value = true;

                customFieldRef = customFieldRef.Append(stringCustomFieldPdf).ToArray();

                //book número
                StringCustomFieldRef stringCustomAdrres = new StringCustomFieldRef();
                stringCustomAdrres.scriptId = "custrecord_streetnum";
                stringCustomAdrres.value = altaCliente.NumExtFis;

                l_adrresbook.Add(stringCustomAdrres);

                //book calle
                StringCustomFieldRef stringCustomStreet = new StringCustomFieldRef();
                stringCustomStreet.scriptId = "custrecord_streetname";
                stringCustomStreet.value = altaCliente.CalleFis.ToUpper();

                l_adrresbook.Add(stringCustomStreet);

                //book colonia
                StringCustomFieldRef stringCustomColonia = new StringCustomFieldRef();
                stringCustomColonia.scriptId = "custrecord_colonia";
                stringCustomColonia.value = altaCliente.ColoniaFis;

                l_adrresbook.Add(stringCustomColonia);

                //book municipio
                StringCustomFieldRef stringCustomVillage = new StringCustomFieldRef();
                stringCustomVillage.scriptId = "custrecord_village";
                stringCustomVillage.value = altaCliente.MunicipioFis;

                l_adrresbook.Add(stringCustomVillage);
                                             
                //Nexus
                CustomerTaxRegistrationList l_nexus = new CustomerTaxRegistrationList();
                CustomerTaxRegistration ejemplo = new CustomerTaxRegistration();
                RecordRef entityNexus = new RecordRef();

                entityNexus.type = RecordType.nexus;
                entityNexus.typeSpecified = true;
                entityNexus.internalId = "1";
                ejemplo.taxRegistrationNumber = altaCliente.Rfc.ToUpper();
                ejemplo.nexus = entityNexus;
                ejemplo.nexusCountry = Country._mexico;
                ejemplo.nexusCountrySpecified = true;

                l_nexus.customerTaxRegistration = new[] { ejemplo };
                ClienteAlta.taxRegistrationList = l_nexus;

                //Nombre SAT Razon Social
                StringCustomFieldRef stringCustomFieldSat = new StringCustomFieldRef();
                stringCustomFieldSat.scriptId = "custentity_mx_sat_registered_name";
                stringCustomFieldSat.value = altaCliente.Nombre_Sat.ToUpper();

                customFieldRef = customFieldRef.Append(stringCustomFieldSat).ToArray();

                //Comentarios
                if (altaCliente.Comentarios is null)
                {
                    ClienteAlta.comments = altaCliente.Comentarios;
                }
                else
                {
                    ClienteAlta.comments = altaCliente.Comentarios.ToUpper();
                }
               

                //Subsidary
                RecordRef subsidiaryRef = new RecordRef();
                subsidiaryRef.type = RecordType.subsidiary;
                subsidiaryRef.typeSpecified = true;
                subsidiaryRef.internalId = "2";

                ClienteAlta.subsidiary = subsidiaryRef;

                //Estatus Cliente
                RecordRef estatusRef = new RecordRef();
                estatusRef.type = RecordType.customerStatus;
                estatusRef.typeSpecified = true;
                estatusRef.internalId = altaCliente.InternalidPago;

                ClienteAlta.entityStatus = estatusRef;

                //Datos de diección
                //Campo Arreglo Custom Field
                CustomFieldRef[] customFieldAdress = new CustomFieldRef[0];
                customFieldAdress = l_adrresbook.ToArray();

                address.country = Country._mexico;
                address.state = altaCliente.Internalid_estado;
                address.city = altaCliente.CiudadFis;
                address.zip = altaCliente.CpFis;
                address.addressee = altaCliente.Destinatario;
                //if (altaCliente.Atencion is null)
                //{
                //    address.attention = altaCliente.Atencion;
                //}
                //else
                //{
                //    address.attention = altaCliente.Atencion.ToUpper();
                //}
                address.countrySpecified = true;
                address.customFieldList = customFieldAdress;
               
                customerAddressBook.addressbookAddress = address;
                customerAddressBook.defaultBilling = true;
                customerAddressBook.defaultBillingSpecified = true;
                customerAddressBook.defaultShipping = true;
                customerAddressBook.defaultShippingSpecified = true;

                customerAddressBookArray.Add(customerAddressBook);
                customerAddressbookList.addressbook = customerAddressBookArray.ToArray();

                ClienteAlta.addressbookList = customerAddressbookList;

                ClienteAlta.customFieldList = customFieldRef;

                //Conexión NetSuite
                service = ConnectionManager.GetNetSuiteService();
                WriteResponse writeResponse = service.add(ClienteAlta);
         
                if (writeResponse.status.isSuccess)
                {
                        resp.Estatus = "Exito";
                        resp.Valor = writeResponse.baseRef;
                        
                }
                else
                {
                    resp.Estatus = "Fracaso";
                    var Error = ConnectionManager.GetStatusDetails(writeResponse.status);
                    resp.Mensaje = string.Format("Hubo un problema al crear el cliente. Detalle: {0}", Error);

                }
                return resp; 

            }
            catch (Exception ex)
            {
                RespuestaServicios resp = new RespuestaServicios();
                resp.Estatus = "Fracaso";
                resp.Mensaje = String.Format("Hubo un problema al crear el Cliente. Detalles: {0}.", ex.Message);
                return resp;
            }

        }
        public string UpdateCustomerTest()
        {
            try
            {
                //Declaracion de servicio
                NetSuiteService service = new NetSuiteService();

                Customer customer = new Customer();
                customer.internalId = "12132";
              

                ContactAccessRoles customerPago = new ContactAccessRoles();
                ContactAccessRolesList customerPagosdatos = new ContactAccessRolesList();
                List<ContactAccessRoles> customerPagoArray = new List<ContactAccessRoles>();

                //RecordRef recordRef = new RecordRef();
                //recordRef.type = RecordType.customer;
                //recordRef.typeSpecified = true;
                //recordRef.internalId = "11727";
                List<SelectCustomFieldRef> l_customField = new List<SelectCustomFieldRef>();
                ListOrRecordRef regForPagoSelectValue = new ListOrRecordRef();
                regForPagoSelectValue.internalId = "12233";
                SelectCustomFieldRef selectForPagoCustomFieldRef = new SelectCustomFieldRef();
                selectForPagoCustomFieldRef.value = regForPagoSelectValue;
                selectForPagoCustomFieldRef.scriptId = "custrecord_psg_ei_email_recipient_cont";


                l_customField.Add(selectForPagoCustomFieldRef);

                CustomFieldRef[] customFieldRef = new CustomFieldRef[0];
                customFieldRef = l_customField.ToArray();

                customer.customFieldList = customFieldRef;


                //RecordRef entityContact = new RecordRef();
                //entityContact.type = RecordType.contact;
                //entityContact.typeSpecified = true;
                //entityContact.internalId = "12233";

                //customerPago.contact = entityContact;
                //customerPago.giveAccess = true;
                //customerPago.giveAccessSpecified = true;
                //customerPago.email = "prueba@prueba.com";
                //customerPago.password = "maf2022$$";
                //customerPago.password2 = "maf2022$$";
                ////customerPago.
                //customerPagoArray.Add(customerPago);

                //customerPagosdatos.contactRoles = customerPagoArray.ToArray();
                //customer.contactRolesList = customerPagosdatos;
                //customer.

                //Lista de items solicitados
                service = ConnectionManager.GetNetSuiteService();

                WriteResponse writeResponse = service.update(customer);

                if (writeResponse.status.isSuccess)
                {

                    return "";
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public WriteResponse CreateIndividualCustomer()
        {
            NetSuiteService service;
            RecordRef subsidiaryRef = new RecordRef();
            DimensionHelper subsidiaryHelper = new DimensionHelper();
            Customer customer = new Customer();
            List<CustomerStatus> statusList = new List<CustomerStatus>();
            RecordRef entityStatus = new RecordRef();
            CustomerStatus customerStatus;
            CustomerAddressbookList customerAddressbookList = new CustomerAddressbookList();
            CustomerAddressbook customerAddressBook = new CustomerAddressbook();
            List<CustomerAddressbook> customerAddressBookArray = new List<CustomerAddressbook>();
            Address address = new Address();
            Subsidiary subsidiary = subsidiaryHelper.GetSubsidiaryByName("Alvarez Automotriz");
            CurrencyHelper currencyHelper = new CurrencyHelper();
            Currency currency;
            RecordRef currencyRef = new RecordRef();
            List<CustomerCategory> categoryList = new List<CustomerCategory>();
            CustomerCategory category;
            RecordRef categoryRef = new RecordRef();
            List<CustomFieldRef> customFieldList = new List<CustomFieldRef>();
            List<CustomFieldRef> customadressFieldList = new List<CustomFieldRef>();
            StringCustomFieldRef mxRFCFieldRef = new StringCustomFieldRef();
            List<StringCustomFieldRef> l_customField = new List<StringCustomFieldRef>();

            subsidiaryRef.type = RecordType.subsidiary;
            subsidiaryRef.typeSpecified = true;
            subsidiaryRef.internalId = "2";

            currency = currencyHelper.GetCurrencyBySymbol("MXN");
            currencyRef.type = RecordType.currency;
            currencyRef.typeSpecified = true;
            currencyRef.internalId = currency.internalId;

            statusList = GetEntityStatus();
            customerStatus = statusList.Find(x => x.name == "Closed Won");
            entityStatus.type = RecordType.customerStatus;
            entityStatus.typeSpecified = true;
            entityStatus.internalId = customerStatus.internalId;

            //categoryList = GetCategory();
            //category = categoryList.Find(x => x.name == "Corporate");
            //categoryRef.type = RecordType.customerCategory;
            //categoryRef.typeSpecified = true;
            //categoryRef.internalId = category.internalId;

            customer.isPerson = true;
            customer.companyName = "JG Devs";
            customer.firstName = "Diana Beneviento";
            customer.entityStatus = entityStatus;
            customer.phone = "5542434027";
            customer.altPhone = "5542434027";
            customer.email = "db_JG_Devs@jgdevs.com";
            customer.currency = currencyRef;
            customer.subsidiary = subsidiaryRef;

            address.country = Country._mexico;
            address.state = "Toluca";
            address.city = "EDOMX";
            address.zip = "91090";
            address.addr1 = "JOSE MANCISIDOR 789";
            address.addressee = "Diana Beneviento";
            address.countrySpecified = true;

            StringCustomFieldRef streetnumFieldRef = new StringCustomFieldRef();
            streetnumFieldRef.scriptId = "custrecord_streetnum";
            streetnumFieldRef.value = "321";

            StringCustomFieldRef streetnameFieldRef = new StringCustomFieldRef();
            streetnameFieldRef.scriptId = "custrecord_streetname";
            streetnameFieldRef.value = "Izcoatl";
            customadressFieldList.Add(streetnameFieldRef);
            customadressFieldList.Add(streetnumFieldRef);

            CustomFieldRef[] customFieldRef = new CustomFieldRef[0];
            customFieldRef = customadressFieldList.ToArray();

            //
            ListOrRecordRef custSelectValue = new ListOrRecordRef();
            custSelectValue.internalId = "11";
            custSelectValue.name = "TLALNEPANTLA";
            SelectCustomFieldRef selectCustomFieldRef = new SelectCustomFieldRef();
            selectCustomFieldRef.internalId = "1209";
            selectCustomFieldRef.value = custSelectValue;
            selectCustomFieldRef.scriptId = "custentity_maf_most_asig";

            customFieldRef = customFieldRef.Append(selectCustomFieldRef).ToArray();

            address.customFieldList = customFieldRef;

            customerAddressBook.addressbookAddress = address;
            customerAddressBook.defaultBilling = true;
            customerAddressBook.defaultBillingSpecified = true;
            customerAddressBook.defaultShipping = true;
            customerAddressBook.defaultShippingSpecified = true;

            customerAddressBookArray.Add(customerAddressBook);
            customerAddressbookList.addressbook = customerAddressBookArray.ToArray();

            mxRFCFieldRef.scriptId = "custentity_mx_rfc";
            mxRFCFieldRef.value = "SIGR280101MR2";

            customFieldList.Add(mxRFCFieldRef);

            customer.customFieldList = customFieldList.ToArray();
            customer.addressbookList = customerAddressbookList;

            service = ConnectionManager.GetNetSuiteService();

            WriteResponse writeResponse = service.add(customer);

            return writeResponse;
        }

        public WriteResponse UpdateCustomer(string customerId)
        {
            NetSuiteService service;

            Customer customer = new Customer();
            customer.phone = "8999235519";
            customer.internalId = customerId;

            service = ConnectionManager.GetNetSuiteService();

            WriteResponse writeResponse = service.update(customer);

            return writeResponse;
        }

        public WriteResponse DeleteCustomer(string custId)
        {
            NetSuiteService service;
            RecordRef customerRef = new RecordRef();

            service = ConnectionManager.GetNetSuiteService();

            customerRef.internalId = custId;
            customerRef.type = RecordType.customer;
            customerRef.typeSpecified = true;

            WriteResponse writeResponse = service.delete(customerRef, null/* TODO Change to default(_) if this is not a reference type */);

            return writeResponse;
        }

        public List<CustomerStatus> GetEntityStatus()
        {
            NetSuiteService service;
            SearchResult searchResults;

            CustomerStatusSearch customerStatusSearch = new CustomerStatusSearch();
            CustomerStatusSearchBasic customerStatusSearchBasic = new CustomerStatusSearchBasic();
            List<CustomerStatus> statusList = new List<CustomerStatus>();

            customerStatusSearch.basic = customerStatusSearchBasic;

            service = ConnectionManager.GetNetSuiteService();

            // Perform the search
            searchResults = service.search(customerStatusSearch);

            // Loop through the results on the record list
            foreach (var record in searchResults.recordList)
                statusList.Add((CustomerStatus)record);

            return statusList;
        }

        public List<CustomerCategory> GetCategory()
        {
            NetSuiteService service;
            SearchResult searchResults;

            CustomerCategorySearch customerCategorySearch = new CustomerCategorySearch();
            CustomerCategorySearchBasic customerCategorySearchBasic = new CustomerCategorySearchBasic();
            List<CustomerCategory> categoryList = new List<CustomerCategory>();

            customerCategorySearch.basic = customerCategorySearchBasic;

            service = ConnectionManager.GetNetSuiteService();

            // Perform the search
            searchResults = service.search(customerCategorySearch);

            // Loop through the results on the record list
            foreach (var record in searchResults.recordList)
                categoryList.Add((CustomerCategory) record);

            return categoryList;
        }

        public Customer GetCustomerById(string custId)
        {
            NetSuiteService service = new NetSuiteService();
            Customer customer;

            RecordRef recordRef = new RecordRef();
            recordRef.type = RecordType.customer;
            recordRef.typeSpecified = true;
            recordRef.internalId = custId;

            service = ConnectionManager.GetNetSuiteService();
            customer = (Customer) service.get(recordRef).record;

            return customer;
        }

        public Customer GetCustomerByName(string customerName)
        {
            // NetSuite Service class
            NetSuiteService service = new NetSuiteService();

            // Search results
            SearchResult searchResults;

            Customer customer = new Customer();

            // Authenticate and get the NetSuite service
            service = ConnectionManager.GetNetSuiteService();

            // Subsidary search object
            CustomerSearch customerSearch = new CustomerSearch();
            CustomerSearchBasic customerSearchBasic = new CustomerSearchBasic();
            SearchStringField customerNameField = new SearchStringField();

            // Filter customer search by name
            customerNameField.@operator = SearchStringFieldOperator.contains;
            customerNameField.operatorSpecified = true;
            customerNameField.searchValue = customerName;
            customerSearchBasic.entityId = customerNameField;
            
            customerSearch.basic = customerSearchBasic;

            // Perform the search
            searchResults = service.search(customerSearch);

            customer = (Customer)searchResults.recordList.FirstOrDefault();

            return customer;
        }

        public List<Customer> GetAllCustomers()
        {
            NetSuiteService service = new NetSuiteService();
            List<Customer> recordList = new List<Customer>();
            CustomerSearch customerSearch = new CustomerSearch();
            CustomerSearchBasic customerSearchBasic = new CustomerSearchBasic();
            SearchEnumMultiSelectField customerStage = new SearchEnumMultiSelectField();

            customerStage.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
            customerStage.operatorSpecified = true;
            customerStage.searchValue = new string[] { "_customer" };

            customerSearchBasic.stage = customerStage;
            //customerSearchBasic.itemPricingLevel = 
            customerSearch.basic = customerSearchBasic;

            SearchPreferences prefrence = new SearchPreferences();
            prefrence.bodyFieldsOnly = false;

            service.searchPreferences = prefrence;
            service = ConnectionManager.GetNetSuiteService();

            SearchResult searchResult = service.search(customerSearch);

            var tamaño = searchResult.totalPages;

            if (tamaño <= 1)
            {
               recordList = searchResult.recordList.Cast<Customer>().ToList();
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
                            if (searchResult.recordList[i] is Customer)
                            {
                                Customer objItem = (Customer) searchResult.recordList[i];
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

        public void CustomerCycle()
        {
            // Create
            Console.WriteLine("Create");
            CustomerHelper customerHelper = new CustomerHelper();

            WriteResponse createWriteResponse = customerHelper.CreateCustomer();
            RecordRef createdCustomerRef = (RecordRef) createWriteResponse.baseRef;

            Console.WriteLine(createdCustomerRef.internalId);

            // Read
            Console.WriteLine("Read");
            Customer customer = customerHelper.GetCustomerById(createdCustomerRef.internalId);

            // Update 
            Console.WriteLine("Update");
            customerHelper.UpdateCustomer("createdCustomerRef.internalId");
            Console.WriteLine(customer.companyName);
            Console.WriteLine(customer.phone);

            // Delete
            Console.WriteLine("Delete");
            WriteResponse deleteWriteResponse = customerHelper.DeleteCustomer(createdCustomerRef.internalId);
            RecordRef deletedCustomerRef = (RecordRef) deleteWriteResponse.baseRef;

            Console.WriteLine(deleteWriteResponse.status.statusDetail);
        }

        public RespuestaServicios InsertContact(string internalid,string fistname,string email)
        {
            try
            {
                //Declaracion de servicio
                NetSuiteService service = new NetSuiteService();
                RespuestaServicios resp = new RespuestaServicios();
                //Contacto
                Contact contacto = new Contact();

                RecordRef recordSub = new RecordRef();
                recordSub.type = RecordType.subsidiary;
                recordSub.typeSpecified = true;
                recordSub.internalId = "2";

                RecordRef recordCustContact = new RecordRef();
                recordCustContact.type = RecordType.customer;
                recordCustContact.typeSpecified = true;
                recordCustContact.internalId = internalid;

                contacto.company = recordCustContact;
                contacto.firstName =fistname;
                contacto.email = email;
                contacto.subsidiary = recordSub;

                //Lista de items solicitados
                service = ConnectionManager.GetNetSuiteService();

                WriteResponse writeResponse = service.add(contacto);

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
                resp.Mensaje = string.Format(ex.Message);
                return resp;
            }
        }
        //public string UpdateCustomerTests()
        //{
        //    try
        //    {
        //        //Declaracion de servicio
        //        NetSuiteService service = new NetSuiteService();

        //        Customer customer = new Customer();
        //        customer.internalId = "12334";

        //        ContactAccessRoles customerPago = new ContactAccessRoles();
        //        ContactAccessRolesList customerPagosdatos = new ContactAccessRolesList();
        //        List<ContactAccessRoles> customerPagoArray = new List<ContactAccessRoles>();

        //        //RecordRef recordRef = new RecordRef();
        //        //recordRef.type = RecordType.customer;
        //        //recordRef.typeSpecified = true;
        //        //recordRef.internalId = "11727";

        //        //List<SelectCustomFieldRef> l_customField = new List<SelectCustomFieldRef>();
        //        //ListOrRecordRef regForPagoSelectValue = new ListOrRecordRef();
        //        //regForPagoSelectValue.internalId = "12233";
        //        //SelectCustomFieldRef selectForPagoCustomFieldRef = new SelectCustomFieldRef();
        //        //selectForPagoCustomFieldRef.value = regForPagoSelectValue;
        //        //selectForPagoCustomFieldRef.scriptId = "custrecord_psg_ei_email_recipient_cont";

        //        //l_customField.Add(selectForPagoCustomFieldRef);
        //        //CustomFieldRef[] customFieldRef = new CustomFieldRef[0];
        //        //customFieldRef = l_customField.ToArray();

        //        //customer.customFieldList = customFieldRef;

        //        RecordRef entityContact = new RecordRef();
        //        entityContact.type = RecordType.contact;
        //        entityContact.typeSpecified = true;
        //        entityContact.internalId = "12433";

        //        RecordRef entityContactRole = new RecordRef();
        //        entityContactRole.type = RecordType.account;
        //        entityContactRole.typeSpecified = true;
        //        entityContactRole.internalId = "14";

        //        customerPago.contact = entityContact;
        //        customerPago.giveAccess = false;
        //        customerPago.giveAccessSpecified = true;
        //        customerPago.email = "prueba@prueba.com";
        //        customerPago.role = entityContactRole;
        //        customerPagoArray.Add(customerPago);

        //        customerPagosdatos.contactRoles = customerPagoArray.ToArray();
        //        customer.contactRolesList = customerPagosdatos;

        //        //CustomRecord rec = new CustomRecord();

        //        //RecordRef recType = new RecordRef();
        //        //recType.internalId = "10";
        //        //recType.type = RecordType.customRecord;
        //        //recType.typeSpecified = true;

        //        //rec.recType = recType;
        //        //rec.name = "recmachcustrecord_psg_ei_email_recipient_cust";

        //        //CustomFieldRef[] customFieldArray = new CustomFieldRef[1];

        //        //ListOrRecordRef regContacto = new ListOrRecordRef();
        //        //regContacto.internalId = "12433";
        //        //SelectCustomFieldRef selectForPagoCustomFieldRef = new SelectCustomFieldRef();
        //        //selectForPagoCustomFieldRef.value = regContacto;
        //        //selectForPagoCustomFieldRef.scriptId = "custrecord_psg_ei_email_recipient_cont";
        //        //customFieldArray[0] = selectForPagoCustomFieldRef;

        //        //rec.customFieldList = customFieldArray;

        //        //customer.customForm = rec;

        //        //Lista de items solicitados
        //        service = ConnectionManager.GetNetSuiteService();

        //        WriteResponse writeResponse = service.update(customer);

        //        if (writeResponse.status.isSuccess)
        //        {

        //            return "";
        //        }
        //        else
        //        {
        //            return "";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return "";
        //    }
        //}

        public Customer GetCustomerform(string custId)
        {
            NetSuiteService service = new NetSuiteService();
            Customer customer;

            GetSelectValueFieldDescription fieldDescription = new GetSelectValueFieldDescription();
            fieldDescription.recordType = RecordType.customer;
            fieldDescription.recordTypeSpecified = true;
            fieldDescription.field = "customform";
            GetSelectValueResult result = service.getSelectValue(fieldDescription, 1);

            RecordRef recordRef = new RecordRef();
            recordRef.type = RecordType.customer;
            recordRef.typeSpecified = true;
            recordRef.internalId = custId;

            service = ConnectionManager.GetNetSuiteService();
            customer = (Customer)service.get(recordRef).record;

            return customer;
        }

    }

}
