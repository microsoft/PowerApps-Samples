using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PowerApps.Samples;
using PowerApps.Samples.Batch;
using PowerApps.Samples.Messages;
using PowerApps.Samples.Methods;
using PowerApps.Samples.Types;

namespace FunctionsAndActions
{
    internal class Program
    {
        static async Task Main()
        {
            Config config = App.InitializeApp();

            var service = new Service(config);

            List<EntityReference> recordsToDelete = new();
            bool deleteCreatedRecords = true;

            Console.WriteLine("--Starting Functions and Actions sample--");

            #region Section 1: Unbound Functions: WhoAmI

            Console.WriteLine("Starting Section 1: Unbound Functions: WhoAmI");
            Console.WriteLine();

            //Unbound Functions

            //WhoAmI
            //Look at the definition of the WhoAmIRequest and WhoAmIResponse classes
            var WhoIAm = await service.SendAsync<WhoAmIResponse>(new WhoAmIRequest());
            Console.WriteLine("WhoAmI tells us:");
            Console.WriteLine($"WhoAmIResponse.BusinessUnitId:{WhoIAm.BusinessUnitId}");
            Console.WriteLine($"WhoAmIResponse.UserId:{WhoIAm.UserId}");
            Console.WriteLine($"WhoAmIResponse.OrganizationId:{WhoIAm.OrganizationId}");
            Console.WriteLine();

            #endregion Section 1: Unbound Functions: WhoAmI

            #region Section 2: Unbound Functions: FormatAddress

            Console.WriteLine("Starting Section 2: Unbound Functions: FormatAddress");
            Console.WriteLine();

            //FormatAddress
            //Look at the definition of the FormatAddressRequest and FormatAddressResponse classes
            Console.WriteLine("FormatAddress builds the full address according to country/regional format specific requirements.");
            Console.WriteLine("USA Formatted Address:");

            FormatAddressRequest formatUSAddressRequest = new(
                line1: "123 Maple St.",
                city: "Seattle",
                stateOrProvince: "WA",
                postalCode: "98007",
                country: "USA");
            var formatUSAddressResponse = await service.SendAsync<FormatAddressResponse>(formatUSAddressRequest);
            
            Console.WriteLine(formatUSAddressResponse.Address);
            //123 Maple St.
            //Seattle, WA 98007
            //USA
            Console.WriteLine();

            Console.WriteLine("JAPAN Formatted Address:");
            FormatAddressRequest formatJapanAddressRequest = new(
                line1: "1-2-3 Sakura",
                city: "Nagoya",
                stateOrProvince: "Aichi",
                postalCode: "455-2345",
                country: "JAPAN");
            var formatJapanAddressResponse = await service.SendAsync<FormatAddressResponse>(formatJapanAddressRequest);
            
            Console.WriteLine(formatJapanAddressResponse.Address);
            //455-2345
            //Aichi
            //Nagoya
            //1-2-3 Sakura
            //JAPAN
            Console.WriteLine();
            
            #endregion Section 2: Unbound Functions: FormatAddress

            #region Section 3: Unbound Functions: InitializeFrom

            Console.WriteLine("Starting Section 3: Unbound Functions: InitializeFrom");
            Console.WriteLine();
            // InitializeFrom returns an entity with default values set based on mapping configuration
            // for each organization.
            // See https://learn.microsoft.com/power-apps/developer/data-platform/webapi/create-entity-web-api#create-a-new-record-from-another-record
            JObject originalAccount = new() {

                {"accountcategorycode", 1 }, //Preferred Customer
                {"address1_addresstypecode", 3 }, //Primary
                {"address1_city", "Redmond" },
                {"address1_country", "USA" },
                {"address1_line1", "123 Maple St." },
                {"address1_name", "Corporate Headquarters" },
                {"address1_postalcode", "98000" },
                {"address1_shippingmethodcode", 4 }, //UPS
                {"address1_stateorprovince", "WA" },
                {"address1_telephone1", "555-1234" },
                {"customertypecode", 3 }, //Customer
                {"description", "Contoso is a business consulting company." },
                {"emailaddress1", "info@contoso.com" },
                {"industrycode", 7 }, //Consulting
                {"name", "Contoso Consulting" },
                {"numberofemployees", 150 },
                {"ownershipcode", 2 }, //Private
                {"preferredcontactmethodcode", 2 }, //Email
                {"telephone1", "(425) 555-1234"  }

            };

            // Create the original record
            EntityReference originalAccountReference = await service.Create("accounts", originalAccount);
            recordsToDelete.Add(originalAccountReference); // To delete later

            InitializeFromRequest initializeFromRequest = new(
                entityMoniker: originalAccountReference,
                targetEntityName: "account",
                targetFieldType: TargetFieldType.ValidForCreate);

            var initializeFromResponse = await service.SendAsync<InitializeFromResponse>(initializeFromRequest);

            Console.WriteLine("New data based on original record:");
            Console.WriteLine($"{initializeFromResponse.Record.ToString(Formatting.Indented)}");
            Console.WriteLine();
            //Output when all columns are mapped for account_parent_account relationship using Generate Mappings:
            /*
            {
              "@odata.context": "[Organization URI]/api/data/v9.2/$metadata#accounts/$entity",
              "@odata.type": "#Microsoft.Dynamics.CRM.account",
              "territorycode": 1,
              "address2_freighttermscode": 1,
              "address2_shippingmethodcode": 1,
              "address1_telephone1": "555-1234",
              "accountclassificationcode": 1,
              "creditonhold": false,
              "donotbulkemail": false,
              "donotsendmm": false,
              "emailaddress1": "info@contoso.com",
              "address1_line1": "123 Maple St.",
              "customertypecode": 3,
              "ownershipcode": 2,
              "businesstypecode": 1,
              "donotpostalmail": false,
              "donotbulkpostalmail": false,
              "name": "Contoso Consulting",
              "address1_city": "Redmond",
              "description": "Contoso is a business consulting company.",
              "donotemail": false,
              "address2_addresstypecode": 1,
              "donotphone": false,
              "statuscode": 1,
              "address1_name": "Corporate Headquarters",
              "followemail": true,
              "preferredcontactmethodcode": 2,
              "numberofemployees": 150,
              "industrycode": 7,
              "telephone1": "(425) 555-1234",
              "address1_shippingmethodcode": 4,
              "donotfax": false,
              "address1_addresstypecode": 3,
              "customersizecode": 1,
              "marketingonly": false,
              "accountratingcode": 1,
              "shippingmethodcode": 1,
              "address1_country": "USA",
              "participatesinworkflow": false,
              "accountcategorycode": 1,
              "address1_postalcode": "98000",
              "address1_stateorprovince": "WA",
              "ownerid@odata.bind": "systemusers()",
              "parentaccountid@odata.bind": "accounts(fe9873ac-2f1b-ed11-b83e-00224837179f)"
            }          

            */
            JObject newAccount = initializeFromResponse.Record;

            //Set different properties for new record
            newAccount["name"] = "Contoso Consulting Chicago Branch";
            newAccount["address1_city"] = "Chicago";
            newAccount["address1_line1"] = "456 Elm St.";
            newAccount["address1_name"] = "Chicago Branch Office";
            newAccount["address1_postalcode"] = "60007";
            newAccount["address1_stateorprovince"] = "IL";
            newAccount["address1_telephone1"] = "(312) 555-3456";
            newAccount["numberofemployees"] = 12;

            // ownerid is set when Generate Mappings used.
            // It should not be mapped. Error will occur if included in POST request.
            newAccount.Remove("ownerid@odata.bind");

            Console.WriteLine("New Record:");
            Console.WriteLine($"{newAccount.ToString(Formatting.Indented)}");
            Console.WriteLine();

            /*

            {
              "@odata.context": "[Organization URI]/api/data/v9.2/$metadata#accounts/$entity",
              "@odata.type": "#Microsoft.Dynamics.CRM.account",
              "territorycode": 1,
              "address2_freighttermscode": 1,
              "address2_shippingmethodcode": 1,
              "address1_telephone1": "(312) 555-3456",
              "accountclassificationcode": 1,
              "creditonhold": false,
              "donotbulkemail": false,
              "donotsendmm": false,
              "emailaddress1": "info@contoso.com",
              "address1_line1": "456 Elm St.",
              "customertypecode": 3,
              "ownershipcode": 2,
              "address1_addresstypecode": 3,
              "donotpostalmail": false,
              "donotbulkpostalmail": false,
              "name": "Contoso Consulting Chicago Branch",
              "address1_city": "Chicago",
              "description": "Contoso is a business consulting company.",
              "donotemail": false,
              "address2_addresstypecode": 1,
              "donotphone": false,
              "businesstypecode": 1,
              "statuscode": 1,
              "address1_name": "Chicago Branch Office",
              "followemail": true,
              "preferredcontactmethodcode": 2,
              "numberofemployees": 12,
              "industrycode": 7,
              "telephone1": "(425) 555-1234",
              "address1_shippingmethodcode": 4,
              "donotfax": false,
              "customersizecode": 1,
              "marketingonly": false,
              "accountratingcode": 1,
              "shippingmethodcode": 1,
              "address1_country": "USA",
              "participatesinworkflow": false,
              "accountcategorycode": 1,
              "address1_postalcode": "60007",
              "address1_stateorprovince": "IL",
              "parentaccountid@odata.bind": "accounts(561c9519-331b-ed11-b83e-00224837179f)"
            }
            */

            //Create the new record with default values copied from the original
            EntityReference newAccountReference = await service.Create("accounts", newAccount);
            recordsToDelete.Add(newAccountReference); //To delete later

            #endregion Section 3: Unbound Functions: InitializeFrom

            #region Section 4: Unbound Functions: RetrieveCurrentOrganization

            Console.WriteLine("Starting Section 4: Unbound Functions: RetrieveCurrentOrganization");
            Console.WriteLine();
            // RetrieveCurrentOrganization function retrieves data about the current organization.
            // See https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/retrievecurrentorganization?view=dataverse-latest

            RetrieveCurrentOrganizationRequest retrieveCurrentOrganizationRequest =
                new(accessType: EndpointAccessType.Default);

            var retrieveCurrentOrganizationResponse =
                await service.SendAsync<RetrieveCurrentOrganizationResponse>(retrieveCurrentOrganizationRequest);

            Console.WriteLine("Data returned with RetrieveCurrentOrganizationResponse:");
            Console.WriteLine($"{JsonConvert.SerializeObject(retrieveCurrentOrganizationResponse.Detail, Formatting.Indented)}");
            Console.WriteLine();

            /* Output looks like this:
                {
                  "OrganizationId": "3b0181ba-1a67-4816-9ef8-51b4c6ac7330",
                  "FriendlyName": "SAM trial",
                  "OrganizationVersion": "9.2.22074.142",
                  "EnvironmentId": "fb837b9e-fdb7-4999-ba5f-65bd1d761197",
                  "DatacenterId": "dcacf239-fc5b-462d-8198-eefe2da03e13",
                  "Geo": "NA",
                  "TenantId": "9046e390-97ae-44e0-8ca5-677ef6b115ef",
                  "UrlName": "yourorg",
                  "UniqueName": "unq9505972cb89e4dcdac6f258037d13",
                  "Endpoints": {
                    "Count": 3,
                    "IsReadOnly": false,
                    "Keys": [
                      "WebApplication",
                      "OrganizationService",
                      "OrganizationDataService"
                    ],
                    "Values": [
                      "https://yourorg.crm.dynamics.com/",
                      "https://yourorg.api.crm.dynamics.com/XRMServices/2011/Organization.svc",
                      "https://yourorg.api.crm.dynamics.com/XRMServices/2011/OrganizationData.svc"
                    ]
                  },
                  "State": "Enabled"
                }
 
            */

            #endregion Section 4: Unbound Functions: RetrieveCurrentOrganization

            #region Section 5: Unbound Functions: RetrieveTotalRecordCount

            Console.WriteLine("Starting Section 5: Unbound Functions: RetrieveTotalRecordCount");
            Console.WriteLine();
            // RetrieveTotalRecordCount Function Returns data on the total number of records for specific entities.
            // The data retrieved will be from a snapshot within last 24 hours.
            // See https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/retrievetotalrecordcount?view=dataverse-latest

            RetrieveTotalRecordCountRequest retrieveTotalRecordCountRequest = new(
                entityNames: new string[] { "account", "contact" });

            var retrieveTotalRecordCountResponse = 
                await service.SendAsync<RetrieveTotalRecordCountResponse>(retrieveTotalRecordCountRequest);

            Console.WriteLine("The number of records for each table according to RetrieveTotalRecordCount:");

            var recordCountCollection = retrieveTotalRecordCountResponse.EntityRecordCountCollection;

            for (int i = 0; i < recordCountCollection.Keys.Count; i++)
            {
                Console.WriteLine($"\t{recordCountCollection.Keys[i]}:{recordCountCollection.Values[i]}");
            }
            Console.WriteLine();

            #endregion Section 5: Unbound Functions: RetrieveTotalRecordCount

            #region Section 6: Bound Functions: IsSystemAdmin

            Console.WriteLine("Starting Section 6: Bound Functions: IsSystemAdmin");
            Console.WriteLine();

            // IsSystemAdmin is a Custom API that is bound to the systemuser table.
            // Because this Custom API is not likely to be in the environment
            // The ManageIsSystemAdminFunction will install it by importing the 
            // managed solution found in Resources/IsSystemAdminFunction_1_0_0_0_managed.zip
            // See: https://learn.microsoft.com/power-apps/developer/data-platform/org-service/samples/issystemadmin-customapi-sample-plugin


            await ManageIsSystemAdminFunction(service: service, recordsToDelete: recordsToDelete);


            //Get top 10 user records that don't start with # character
            RetrieveMultipleResponse retrieveMultipleUsersResponse =
                await service.RetrieveMultiple("systemusers?" +
                "$select=fullname&$filter=not startswith(fullname,'%23')&$top=10");

            Console.WriteLine("Top 10 users and whether they have System Administrator role.");
            
            //Test each for the System Administrator role:
            retrieveMultipleUsersResponse.Records.ToList().ForEach(user => {

                IsSystemAdminRequest isSystemAdminRequest = new(
                    systemUserId: (Guid)user["systemuserid"]);

                //Force these calls to be executed synchronously:
                var isSystemAdminResponse =
                service.SendAsync<IsSystemAdminResponse>(isSystemAdminRequest).GetAwaiter().GetResult();

                string message = $"{user["fullname"]} " +
                $"{(isSystemAdminResponse.HasRole ? "HAS" : "does not have")} " +
                $"the System Administrator role.";

                Console.WriteLine($"\t{message}");
            });
            Console.WriteLine();

            #endregion Section 6: Bound Functions: IsSystemAdmin

            #region Section 7: Unbound Actions: GrantAccess

            Console.WriteLine("Starting Section 7: Unbound Actions: GrantAccess");
            Console.WriteLine();

            // GrantAccess is an action used to share a record with another user
            // See: https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/grantaccess?view=dataverse-latest

            JObject accountRecord = new() {
                {"name","Account to Share" }
            };
            EntityReference accountToShareRef = 
                await service.Create(
                    entitySetName: "accounts", 
                    record: accountRecord);

            recordsToDelete.Add(accountToShareRef); // To delete later.

            // Get an enabled user other than current user

            string firstUserQuery = "systemusers" +
                $"?$filter=systemuserid ne {WhoIAm.UserId} " +
                    "and isdisabled eq false " +
                    "and not startswith(fullname,'%23')" +
                "&$select=systemuserid,fullname&$top=1";

            RetrieveMultipleResponse firstUser = await service.RetrieveMultiple(
                queryUri: firstUserQuery);

            var otherUser = (JObject)firstUser.Records.FirstOrDefault();

            if (otherUser != null)
            {
                Console.WriteLine($"Testing user: {otherUser["fullname"]}");

                //Create a reference to the user;
                EntityReference userReference = new(
                    entitySetName: "systemusers",
                    id: (Guid)otherUser["systemuserid"]);

                //Test the other user's access to the record using RetrievePrincipalAccess
                RetrievePrincipalAccessRequest retrievePrincipalAccessRequest1 = new(
                    principal: userReference,
                    target: accountToShareRef);

                var retrievePrincipalAccessResponse1 =
                    await service.SendAsync<RetrievePrincipalAccessResponse>(retrievePrincipalAccessRequest1);

                Console.WriteLine($"Current users access: {retrievePrincipalAccessResponse1.AccessRights}");

                //If they don't already have Delete Access, grant it to them.
                if (!retrievePrincipalAccessResponse1.AccessRights.HasFlag(AccessRights.DeleteAccess))
                {
                    //Create request to share the record with the user
                    GrantAccessRequest grantAccessRequest = new(
                        target: accountToShareRef.AsJObject("account", "accountid"),
                        principalAccess: new PrincipalAccess()
                        {
                            AccessMask = AccessRights.DeleteAccess,
                            Principal = userReference.AsJObject("systemuser", "systemuserid")
                        });
                    
                    // Send the request
                    await service.SendAsync(grantAccessRequest);

                    //Test the other user's access to the record again
                    RetrievePrincipalAccessRequest retrievePrincipalAccessRequest2 = new(
                        principal: userReference,
                        target: accountToShareRef);

                    var retrievePrincipalAccessResponse2 =
                        await service.SendAsync<RetrievePrincipalAccessResponse>(retrievePrincipalAccessRequest2);

                    if (retrievePrincipalAccessResponse2.AccessRights.HasFlag(AccessRights.DeleteAccess))
                    {
                        Console.WriteLine($"{otherUser["fullname"]} was granted DeleteAccess");
                        Console.WriteLine();
                    }
                }
            }

            #endregion Section 7: Unbound Actions: GrantAccess

            #region Section 8: Bound Actions: AddPrivilegesRole

            Console.WriteLine("Starting Section 8: Bound Actions: AddPrivilegesRole");
            Console.WriteLine();

            // AddPrivilegesRole adds a set of existing privileges to an existing role.
            // See https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/addprivilegesrole?view=dataverse-latest

            //Create a role

            JObject role = new() {
                //Role must be associated to a business unit
                { "businessunitid@odata.bind", $"businessunits({WhoIAm.BusinessUnitId})"},
                { "name", "Test Role" }
            };


            EntityReference roleReference = await service.Create("roles", role);
            recordsToDelete.Add(roleReference); //To delete later

            JObject retrievedRole = await service.Retrieve(
                entityReference: roleReference, 
                query: "?$select=roleid&$expand=roleprivileges_association($select=name)");

            JArray privilegesBefore = (JArray)retrievedRole["roleprivileges_association"];

            Console.WriteLine($"Number of privileges in new role: {privilegesBefore.Count()}");

            foreach (JObject privilege in privilegesBefore.Cast<JObject>())
            {
                Console.WriteLine($"\t{privilege["name"]}");
            }
            Console.WriteLine();

            /*
             Number of privileges in new role: 9
                prvReadSharePointData
                prvReadSdkMessage
                prvWriteSharePointData
                prvReadSdkMessageProcessingStepImage
                prvReadSdkMessageProcessingStep
                prvReadPluginAssembly
                prvCreateSharePointData
                prvReadPluginType
                prvReadSharePointDocument 
            */

            //Add privileges to the role

            //Retrieve the prvCreateAccount and prvReadAccount privileges
            RetrieveMultipleResponse rolequery = 
                await service.RetrieveMultiple(
                queryUri: "privileges" +
                "?$select=name" +
                "&$filter=name eq 'prvCreateAccount' or " +
                "name eq 'prvReadAccount'");

            //Prepare the parameters
            List<RolePrivilege> rolePrivileges = new();

            rolequery.Records.ToList().ForEach(privilege =>
            {
                rolePrivileges.Add(new RolePrivilege
                {
                    BusinessUnitId = WhoIAm.BusinessUnitId,
                    Depth = PrivilegeDepth.Basic,
                    PrivilegeId = (Guid)privilege["privilegeid"],
                    PrivilegeName = (string)privilege["name"]
                });
            });

            //Prepare the request
            AddPrivilegesRoleRequest request = new(
                roleId: roleReference.Id.Value,
                privileges: rolePrivileges);

            //Add new privileges prvCreateAccount and prvReadAccount
            await service.SendAsync(request);

            //Retrieve the role privileges again
            retrievedRole = 
                await service.Retrieve(
                    entityReference: roleReference, 
                    query: "?$select=roleid&$expand=roleprivileges_association($select=name)");

            JArray privilegesAfter = (JArray)retrievedRole["roleprivileges_association"];

            Console.WriteLine($"Number of privileges after: {privilegesAfter.Count()}");

            foreach (JObject privilege in privilegesAfter.Cast<JObject>())
            {
                Console.WriteLine($"\t{privilege["name"]}");
            }
            Console.WriteLine();

            /*

            Number of privileges after: 11
                    prvReadAccount          <- New
                    prvReadSharePointData
                    prvReadSdkMessage
                    prvCreateAccount        <- New
                    prvWriteSharePointData
                    prvReadSdkMessageProcessingStepImage
                    prvReadSdkMessageProcessingStep
                    prvReadPluginAssembly
                    prvCreateSharePointData
                    prvReadPluginType
                    prvReadSharePointDocument 
            */

            #endregion Section 8: Bound Actions: AddPrivilegesRole

            #region Section 9: Delete sample records  
            Console.WriteLine("Starting Section 9: Delete sample records");
            Console.WriteLine();
            // Delete all the created sample records.

            if (!deleteCreatedRecords)
            {
                Console.Write("\nDo you want these entity records deleted? (y/n) [y]: ");
                string answer = Console.ReadLine();
                answer = answer.Trim();
                if (!(answer.StartsWith("y") || answer.StartsWith("Y") || answer == string.Empty))
                { recordsToDelete.Clear(); }
                else
                {
                    Console.WriteLine("\nDeleting created records.");
                }
            }
            else
            {
                Console.WriteLine("\nDeleting created records.");
            }

            List<HttpRequestMessage> deleteRequests = new();

            foreach (EntityReference recordToDelete in recordsToDelete)
            {
                deleteRequests.Add(new DeleteRequest(recordToDelete));
            }

            BatchRequest batchRequest = new(service.BaseAddress)
            {
                Requests = deleteRequests
            };

            if (deleteRequests.Count > 0)
            {
                await service.SendAsync(batchRequest);
            }

            #endregion Section 9: Delete sample records

            Console.WriteLine("--Functions and Actions sample complete --");
        }

        /// <summary>
        /// Detects whether the Custom API is installed and installs it.
        /// </summary>
        /// <param name="service">The service</param>
        /// <param name="recordsToDelete">References to records to delete.</param>
        /// <returns></returns>
        private static async Task ManageIsSystemAdminFunction(Service service, List<EntityReference> recordsToDelete)
        {

            // See if it is already there
            RetrieveMultipleResponse isSystemAdminResponse =
                await service.RetrieveMultiple(queryUri: "sdkmessages?$select=name&$filter=name eq 'sample_IsSystemAdmin'");
            if (isSystemAdminResponse.Records.Count > 0)
            {
                //It is already there. Don't remove it.
                return;
            }
            Console.WriteLine("sample_IsSystemAdmin function not found.");
            Console.WriteLine("Importing IsSystemAdminFunction_1_0_0_0_managed.zip...");
            Console.WriteLine();

            // Import it if it isn't there
            ImportSolutionParameters importSolutionParameters = new()
            {
                CustomizationFile = File.ReadAllBytes("Resources\\IsSystemAdminFunction_1_0_0_0_managed.zip")
            };

            ImportSolutionRequest importSolutionRequest = new(importSolutionParameters);

            await service.SendAsync(importSolutionRequest);

            EntityReference solutionReference = await GetReferenceToIsSystemAdminFunctionSolution(service);

            // Delete the solution at the end of the sample if it was added.
            recordsToDelete.Add(solutionReference); // To delete later

        }

        public static async Task<EntityReference> GetReferenceToIsSystemAdminFunctionSolution(Service service)
        {

            RetrieveMultipleResponse isSystemAdminSolutionResponse =
               await service.RetrieveMultiple(queryUri: "solutions?$select=solutionid&$filter=uniquename eq 'IsSystemAdminFunction'");
            if (isSystemAdminSolutionResponse.Records.Count > 0)
            {
                return new EntityReference("solutions", (Guid)isSystemAdminSolutionResponse.Records.FirstOrDefault()["solutionid"]);
            }
            else
            {
                throw new Exception("IsSystemAdminFunction solution not found.");
            }
        }
    }
}