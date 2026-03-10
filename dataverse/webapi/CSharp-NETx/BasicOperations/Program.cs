using Newtonsoft.Json.Linq;
using PowerApps.Samples;
using PowerApps.Samples.Messages;
using PowerApps.Samples.Methods;
using System.Text;

namespace BasicOperations
{
    internal class Program
    {
        static async Task Main()
        {
            Config config = App.InitializeApp();

            var service = new Service(config);

            List<EntityReference> recordsToDelete = new();
            bool deleteCreatedRecords = true;

            Console.WriteLine("--Starting Basic Operations sample--");

            #region Section 1: Basic Create and Update operations

            // This sample begins by using Service.SendAsyc with
            // HttpRequestMessage and HttpResponseMessage but
            // will then use the corresponding Messages/
            // *Request and *Response classes and the 
            // corresponding methods found in Methods.

            Console.WriteLine("--Section 1 started--");

            // Create a contact with HttpRequestMessage
            var contactRafelShillo = new JObject
                        {
                            { "firstname", "Rafel" },
                            { "lastname", "Shillo" }
                        };

            HttpRequestMessage createRequest = new(
                method: HttpMethod.Post,
                requestUri: new Uri(
                    uriString: "contacts",
                    uriKind: UriKind.Relative))
            {
                Content = new StringContent(
                    content: contactRafelShillo.ToString(),
                    encoding: Encoding.UTF8,
                    mediaType: "application/json")
            };

            HttpResponseMessage createResponse =
                await service.SendAsync(request: createRequest);

            var rafelShilloUri = new Uri(
                uriString: createResponse.Headers.GetValues("OData-EntityId").FirstOrDefault());

            // EntityReference helps manage references to records
            // It has a constructor that accepts a Uri
            var rafelShilloReference = new EntityReference(uri: rafelShilloUri.ToString());

            recordsToDelete.Add(rafelShilloReference); //To Delete later

            Console.WriteLine($"Contact URI: {rafelShilloUri}");
            Console.WriteLine($"Contact relative Uri: {rafelShilloReference.Path}");

            // Update a contact with HttpRequestMessage
            JObject rafelShilloUpdate1 = new()
            {
                { "annualincome", 80000 },
                { "jobtitle", "Junior Developer" }
            };

            HttpRequestMessage rafelShilloUpdateRequest1 = new(
             method: HttpMethod.Patch,
             requestUri: rafelShilloUri)
            {
                Content = new StringContent(
                        content: rafelShilloUpdate1.ToString(),
                        encoding: Encoding.UTF8,
                        mediaType: "application/json")
            };
            rafelShilloUpdateRequest1.Headers.Add("If-Match", "*"); //Ensure no Create

            //Response has no content
            await service.SendAsync(request: rafelShilloUpdateRequest1);

            Console.WriteLine(
            $"Contact '{contactRafelShillo["firstname"]} {contactRafelShillo["lastname"]}' " +
            $"updated with jobtitle and annual income");

            // Retrieve a contact with HttpRequestMessage
            HttpRequestMessage retrieveRequest = new(
             method: HttpMethod.Get,
             requestUri: rafelShilloUri + "?$select=fullname,annualincome,jobtitle,description");
            retrieveRequest.Headers.Add("Prefer", "odata.include-annotations=\"*\"");

            HttpResponseMessage retrieveResponse =
                await service.SendAsync(request: retrieveRequest);
            JObject retrievedRafelShillo1 =
                JObject.Parse(await retrieveResponse.Content.ReadAsStringAsync());

            Console.WriteLine($"Contact '{retrievedRafelShillo1["fullname"]}' retrieved: \n" +
            $"\tAnnual income: {retrievedRafelShillo1["annualincome@OData.Community.Display.V1.FormattedValue"]}\n" +
            $"\tJob title: {retrievedRafelShillo1["jobtitle"]} \n" +
            // Description is initialized empty.
            $"\tDescription: {retrievedRafelShillo1["description"]}");

            // Modify specific properties and then update contact record.
            JObject rafelShilloUpdate2 = new()
            {
                { "jobtitle", "Senior Developer" },
                { "annualincome", 95000 },
                { "description", "Assignment to-be-determined" }
            };

            // Update using Messages/UpdateRequest
            UpdateRequest rafelShilloUpdateRequest2 = new(
                entityReference: rafelShilloReference,
                record: rafelShilloUpdate2);

            //Response has no content
            await service.SendAsync(request: rafelShilloUpdateRequest2);

            Console.WriteLine($"Contact '{retrievedRafelShillo1["fullname"]}' updated:\n" +
            $"\tJob title: {rafelShilloUpdate2["jobtitle"]}\n" +
            $"\tAnnual income: {rafelShilloUpdate2["annualincome"]}\n" +
            $"\tDescription: {rafelShilloUpdate2["description"]}\n");

            // Change just one property with HttpRequestMessage
            string telephone1 = "555-0105";

            JObject telephone1Update = new() {
                { "value",telephone1}
            };

            HttpRequestMessage updateRafelShilloTelephone1Request = new(
                method: HttpMethod.Put,
                requestUri: rafelShilloUri + "/telephone1")
            {
                Content = new StringContent(
                        content: telephone1Update.ToString(),
                        encoding: Encoding.UTF8,
                        mediaType: "application/json")
            };

            //Response has no content
            await service.SendAsync(request: updateRafelShilloTelephone1Request);

            Console.WriteLine($"Contact '{retrievedRafelShillo1["fullname"]}' " +
                $"phone number updated.");

            // Now retrieve just the single property with HttpRequestMessage
            HttpRequestMessage retrieveRafelShilloTelephone1Request = new(
                method: HttpMethod.Get,
                requestUri: rafelShilloUri + "/telephone1");

            HttpResponseMessage retrieveRafelShilloTelephone1Response =
                await service.SendAsync(request: retrieveRafelShilloTelephone1Request);

            JObject retrieveRafelShilloTelephone1ResponseContent =
                JObject.Parse(await retrieveRafelShilloTelephone1Response.Content.ReadAsStringAsync());
            Console.WriteLine($"Contact's telephone # is: {retrieveRafelShilloTelephone1ResponseContent["value"]}.");


            #endregion Section 1: Basic Create and Update operations

            #region Section 2: Create record associated to another
            /// <summary>  
            /// Demonstrates creation of records and simultaneous association to another, 
            ///  existing record. 
            /// </summary>
            /// 

            Console.WriteLine("\n--Section 2 started--");

            // Create a new account and associate with existing contact in one operation. 
            var accountContoso = new JObject
            {
                { "name", "Contoso Ltd" },
                { "telephone1", "555-5555" },
                // EntityReference.Path provides a relative URL
                { "primarycontactid@odata.bind", rafelShilloReference.Path }
            };

            //Create using Methods/Create method
            EntityReference accountContosoReference =
                await service.Create(
                entitySetName: "accounts",
                record: accountContoso);

            recordsToDelete.Add(accountContosoReference); //To delete later

            Console.WriteLine($"Account '{accountContoso["name"]}' created.");
            Console.WriteLine($"Account URI: {accountContosoReference.Path}");

            // Retrieve account name and primary contact info
            // Using Methods/Retrieve method
            JObject retrievedAccountContoso =
                await service.Retrieve(
                entityReference: accountContosoReference,
                query: "?$select=name,&$expand=primarycontactid($select=fullname,jobtitle,annualincome)",
                includeAnnotations: true);

            Console.WriteLine($"Account '{retrievedAccountContoso["name"]}' has primary contact " +
                $"'{retrievedAccountContoso["primarycontactid"]["fullname"]}':");
            Console.WriteLine($"\tJob title: {retrievedAccountContoso["primarycontactid"]["jobtitle"]} \n" +
                $"\tAnnual income: {retrievedAccountContoso["primarycontactid"]["annualincome@OData.Community.Display.V1.FormattedValue"]}");

            #endregion Section 2: Create record associated to another

            #region Section 3: Create related entities
            /// <summary>  
            /// Demonstrates creation of entity instance and related entities in a single operation.  
            /// </summary>
            /// 
            Console.WriteLine("\n--Section 3 started--");
            //Create the following entries in one operation: an account, its 
            // associated primary contact, and open tasks for that contact.  These 
            // entity types have the following relationships:
            //    Accounts 
            //       |---[Primary] Contact (N-to-1)
            //              |---Tasks (1-to-N)

            JObject accountFourthCoffee = new() {
                { "name", "Fourth Coffee" },
                { "primarycontactid", new JObject(){
                        { "firstname", "Susie" },
                        { "lastname", "Curtis" },
                        { "jobtitle", "Coffee Master" },
                        { "annualincome", 48000 },
                        { "Contact_Tasks", new JArray(){
                                new JObject
                                {
                                    { "subject", "Sign invoice" },
                                    { "description", "Invoice #12321" },
                                    { "scheduledstart", new DateTimeOffset(
                                        year:2023,
                                        month:4,
                                        day:19,
                                        hour:3,
                                        minute:0,
                                        second:0,
                                        new TimeSpan(
                                            hours:7,0,0))},
                                    { "scheduledend", new DateTimeOffset(
                                        year:2023,
                                        month:4,
                                        day:19,
                                        hour:4,
                                        minute:0,
                                        second:0,
                                        new TimeSpan(
                                            hours:7,0,0))},
                                    { "scheduleddurationminutes", 60 }
                                },
                                new JObject
                                {
                                    { "subject", "Setup new display" },
                                    { "description", "Theme is - Spring is in the air" },
                                    { "scheduledstart", new DateTimeOffset(
                                        year:2023,
                                        month:4,
                                        day:20,
                                        hour:3,0,0,
                                        offset: new TimeSpan(7,0,0))},
                                    { "scheduledend", new DateTimeOffset(
                                        year:2023,
                                        month:4,
                                        day:20,
                                        hour:4,0,0,
                                        new TimeSpan(
                                            hours:7,0,0))},
                                    { "scheduleddurationminutes", 60 }
                                },
                                new JObject
                                {
                                    { "subject", "Conduct training" },
                                    { "description", "Train team on making our new blended coffee" },
                                    { "scheduledstart", new DateTimeOffset(
                                        year:2023,
                                        month:4,
                                        day:21,
                                        hour:3,0,0,
                                        new TimeSpan(
                                            hours:7,0,0))},
                                    { "scheduledend", new DateTimeOffset(
                                        year:2023,
                                        month:4,
                                        day:21,
                                        hour:4,0,0,
                                        new TimeSpan(
                                            hours:7,0,0))},
                                    { "scheduleddurationminutes", 60 }
                                }
                            }
                        }
                    }
                }
            };

            EntityReference accountFourthCoffeeReference =
                await service.Create(
                    entitySetName: "accounts",
                    record: accountFourthCoffee);

            recordsToDelete.Add(accountFourthCoffeeReference); //To delete later

            Console.WriteLine($"Account '{accountFourthCoffee["name"]}  created.");

            // Retrieve account, primary contact info, and assigned tasks for contact.
            // Dataverse only supports querying-by-expansion one level deep, so first query 
            // account-primary contact.

            JObject retrievedAccountFourthCoffee = await service.Retrieve(
                entityReference: accountFourthCoffeeReference,
                query: "?$select=name&$expand=primarycontactid($select=fullname,jobtitle,annualincome)",
                //Including annotations so formatted values can be returned.
                includeAnnotations: true);

            Console.WriteLine($"Account '{retrievedAccountFourthCoffee["name"]}' " +
                    $"has primary contact '{retrievedAccountFourthCoffee["primarycontactid"]["fullname"]}':");

            Console.WriteLine($"\tJob title: {retrievedAccountFourthCoffee["primarycontactid"]["jobtitle"]} \n" +
                //Using @OData.Community.Display.V1.FormattedValue to access formatted value
                $"\tAnnual income: {retrievedAccountFourthCoffee["primarycontactid"]["annualincome@OData.Community.Display.V1.FormattedValue"]}");

            // Next retrieve same contact and their assigned tasks.
            // Don't have a saved URI for contact 'Susie Curtis', so create one 
            // from entitySet Name and Id (and add it to recordsToDelete collection for cleanup).
            EntityReference contactSusieCurtisReference = new(
                entitySetName: "contacts",
                id: (Guid)retrievedAccountFourthCoffee["primarycontactid"]["contactid"]);

            recordsToDelete.Add(contactSusieCurtisReference);

            //Retrieve the contact
            JObject retrievedContactSusieCurtis =
                await service.Retrieve(
                    entityReference: contactSusieCurtisReference,
                    query: "?$select=fullname&$expand=Contact_Tasks($select=subject,description,scheduledstart,scheduledend)",
                    includeAnnotations: true);

            Console.WriteLine($"Contact '{retrievedContactSusieCurtis["fullname"]}' has the following assigned tasks:");
            foreach (JObject task in retrievedContactSusieCurtis["Contact_Tasks"].Cast<JObject>())
            {
                Console.WriteLine(
                    $"Subject: {task["subject"]}, \n" +
                    $"\tDescription: {task["description"]}\n" +
                    $"\tStart: {task["scheduledstart@OData.Community.Display.V1.FormattedValue"]}\n" +
                    $"\tEnd: {task["scheduledend@OData.Community.Display.V1.FormattedValue"]}\n");
            }

            #endregion Section 3: Create related entities

            #region Section 4: Associate and Disassociate entities
            /// <summary>  
            /// Demonstrates associating and disassociating of existing entity instances. 
            /// </summary>
            Console.WriteLine("\n--Section 4 started--");

            // Add 'Rafel Shillo' to the contact list of 'Fourth Coffee', 
            // a 1-to-N relationship.
            // Using HttpRequestMessage rather than Messages/AssociateRequest
            HttpRequestMessage associateContactToAccountRequest = new(
                method: HttpMethod.Post,
                requestUri: $"{accountFourthCoffeeReference.Path}/contact_customer_accounts/$ref")
            {
                Content = new StringContent(
                        content: new JObject() {
                            { "@odata.id", $"{service.BaseAddress}{rafelShilloReference.Path}"} //Must use absolute Uri
                        }.ToString(),
                        encoding: Encoding.UTF8,
                        mediaType: "application/json")
            };

            //Response has no content
            await service.SendAsync(request: associateContactToAccountRequest);

            // Retrieve and output all contacts for account 'Fourth Coffee'.
            // Using HttpRequestMessage rather than Messages/RetrieveMultipleRequest
            HttpRequestMessage fourthCoffeeContactsRequest = new(
                method: HttpMethod.Get,
                requestUri: $"{accountFourthCoffeeReference.Path}/contact_customer_accounts?$select=fullname,jobtitle");

            HttpResponseMessage fourthCoffeeContactsResponse =
                await service.SendAsync(request: fourthCoffeeContactsRequest);

            JObject fourthCoffeeContactsResponseContents =
                JObject.Parse(
                    await fourthCoffeeContactsResponse.Content.ReadAsStringAsync());
            JArray fourthCoffeeContacts =
                (JArray)fourthCoffeeContactsResponseContents.GetValue("value");

            Console.WriteLine($"Contact list for account '{retrievedAccountFourthCoffee["name"]}':");

            fourthCoffeeContacts.ToList().ForEach(contact =>
            {
                Console.WriteLine($"\tName: {contact["fullname"]}, Job title: {contact["jobtitle"]}");
            });

            // Disassociate the contact from the account.
            // Using HttpRequestMessage rather than Messages/DisassociateRequest
            HttpRequestMessage disassociateContactFromAccountRequest = new(
                method: HttpMethod.Delete,
                requestUri: $"{accountFourthCoffeeReference.Path}/contact_customer_accounts({rafelShilloReference.Id})/$ref");

            await service.SendAsync(request: disassociateContactFromAccountRequest);


            // Create role and assign it to systemuser using systemuserroles_association

            // Get your BusinessUnitId and UserId using WhoAmI
            var whoIAm = await service.SendAsync<WhoAmIResponse>(new WhoAmIRequest());

            //Define a new security role
            JObject exampleSecurityRole = new() {
                //Security Role must be associated to a business unit when it is created.
                { "businessunitid@odata.bind", $"businessunits({whoIAm.BusinessUnitId})"},
                { "name", "Example Security Role"}

            };
            //Create the security role
            EntityReference exampleSecurityRoleRef = await service.Create("roles", exampleSecurityRole);

            recordsToDelete.Add(exampleSecurityRoleRef); //To delete later


            // Associate role to systemuser via systemuserroles_association.
            // collection-valued navigation property.
            HttpRequestMessage associateRoleToMeRequest = new()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(
                    uriString: $"systemusers({whoIAm.UserId})/systemuserroles_association/$ref",
                    uriKind: UriKind.Relative),
                Content = new StringContent(
                        content: new JObject() {
                                    { "@odata.id", $"{service.BaseAddress}{exampleSecurityRoleRef.Path}"}
                        }.ToString(),
                        encoding: Encoding.UTF8,
                        mediaType: "application/json")
            };

            // The Messages/AssociateRequest class could be used instead to simplify the HttpRequestMessage above:
            // AssociateRequest associateRoleToMeRequest = new(
            //    baseAddress: service.BaseAddress,
            //    entityWithCollection: new EntityReference("systemusers", whoIAm.UserId),
            //    collectionName: "systemuserroles_association",
            //    entityToAdd: exampleSecurityRoleRef);

            //Response has no content
            await service.SendAsync(associateRoleToMeRequest);

            Console.WriteLine($"Security Role '{exampleSecurityRole["name"]}' associated with to your user account.");

            //Retrieve the new security role as part via the systemuserroles_association
            RetrieveMultipleRequest retrieveRoleViaUserRequest = new(
                queryUri: $"systemusers({whoIAm.UserId})/systemuserroles_association?$select=name&$filter=roleid eq {exampleSecurityRoleRef.Id}"
                );

            var retrieveRoleViaUserResponse = await service.SendAsync<RetrieveMultipleResponse>(retrieveRoleViaUserRequest);

            Console.WriteLine($"Retrieved role: {retrieveRoleViaUserResponse.Records.FirstOrDefault()["name"]}");



            // Disassociate role to systemuser via systemuserroles_association.
            // collection-valued navigation property.
            HttpRequestMessage disassociateRoleToMeRequest = new()
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(
                    uriString: $"systemusers({whoIAm.UserId})/systemuserroles_association({exampleSecurityRoleRef.Id})/$ref",
                    uriKind: UriKind.Relative)
            };

            // The Messages/DisassociateRequest class could be used instead to simplify the HttpRequestMessage above:
            // DisassociateRequest disassociateRoleToMeRequest = new(
            //    entityWithCollection: new EntityReference("systemusers", whoIAm.UserId),
            //    collectionName: "systemuserroles_association", 
            //    entityToRemove: exampleSecurityRoleRef);

            //Response has no content
            await service.SendAsync(disassociateRoleToMeRequest);


            #endregion Section 4: Associate and Disassociate entities

            #region Section 5: Delete sample entities  
            Console.WriteLine("\n--Section 5 started--");
            // Delete all the created sample records.  Note that explicit deletion is not required  
            // for contact tasks because these are automatically cascade-deleted with owner.  

            if (!deleteCreatedRecords)
            {
                Console.Write("\nDo you want these entity records deleted? (y/n) [y]: ");
                String answer = Console.ReadLine();
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

            foreach (EntityReference recordToDelete in recordsToDelete)
            {
                // Delete a record with HttpRequestMessage
                // See PowerApps.Samples.Messages DeleteRequest class use in later samples.
                // See Service.Delete method.
                HttpRequestMessage deleteRequest = new(
                    method: HttpMethod.Delete,
                    requestUri: new Uri(
                        uriString: recordToDelete.Path,
                        uriKind: UriKind.Relative)
                    );

                await service.SendAsync(request: deleteRequest);
            }
            #endregion Section 5: Delete sample entities

            Console.WriteLine("--Basic Operations sample complete--");
        }
    }
}