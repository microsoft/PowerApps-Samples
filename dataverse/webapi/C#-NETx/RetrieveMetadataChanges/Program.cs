using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerApps.Samples;
using PowerApps.Samples.Batch;
using PowerApps.Samples.Messages;
using PowerApps.Samples.Metadata.Messages;
using PowerApps.Samples.Metadata.Types;
using PowerApps.Samples.Methods;

namespace RetrieveMetadataChanges
{
    internal class Program
    {
        static async Task Main()
        {
            Config config = App.InitializeApp();

            var service = new Service(config);
             
            
            await TestDeletedMetadataFilters(service);

            return;

            // A simple list of column definitions to represent the cache
            List<ComplexAttributeMetadata> cachedAttributes = new();
            string clientVersionStamp = string.Empty;
            // The name of a column to create when demonstrating changes
            string choiceColumnSchemaName = "sample_ChoiceColumnForSample";
            // Language code value from usersettingscollection.
            int? userLanguagePreference = await RetrieveUserUILanguageCode(service);

            #region Define query

            // Define query for all Picklist Choice columns from Contact table
            var query = new EntityQueryExpression
            {
                Properties = new MetadataPropertiesExpression("LogicalName", "Attributes"),
                Criteria = new MetadataFilterExpression(filterOperator: LogicalOperator.And)
                {
                    Conditions = new List<MetadataConditionExpression>{
                        {
                            new MetadataConditionExpression(
                                propertyName:"LogicalName",
                                conditionOperator: MetadataConditionOperator.Equals,
                                value: new PowerApps.Samples.Metadata.Types.Object{
                                        Type = ObjectType.String,
                                        Value = "contact"
                                    }
                                )
                        }
                    }
                },
                AttributeQuery = new AttributeQueryExpression
                {
                    Properties = new MetadataPropertiesExpression("LogicalName", "OptionSet", "AttributeTypeName"),
                    Criteria = new MetadataFilterExpression(filterOperator: LogicalOperator.And)
                    {
                        Conditions = new List<MetadataConditionExpression>{
                            {
                                // Only Picklist Option type
                                new MetadataConditionExpression(
                                    propertyName:"AttributeTypeName",
                                    conditionOperator:MetadataConditionOperator.Equals,
                                    value: new PowerApps.Samples.Metadata.Types.Object(
                                        type:ObjectType.AttributeTypeDisplayName,
                                        value:"PicklistType")
                                    )
                            }
                        }
                    }
                }
            };

            // Return only user language if they have a preference.
            if (userLanguagePreference.HasValue)
            {
                query.LabelQuery = new LabelQueryExpression
                {
                    FilterLanguages = new List<int>() { userLanguagePreference.Value }
                };
            }

            #endregion Define query

            #region Initialize cache

            var initialRequest = new RetrieveMetadataChangesRequest { Query = query };

            var initialResponse = await service.SendAsync<RetrieveMetadataChangesResponse>(initialRequest);

            Console.WriteLine($"Columns in initial response:{initialResponse.EntityMetadata.FirstOrDefault().Attributes.Count()}");

            // Initialize the cache
            cachedAttributes = initialResponse.EntityMetadata.FirstOrDefault().Attributes.ToList();

            // Set the client version
            clientVersionStamp = initialResponse.ServerVersionStamp;

            #endregion Initialize cache

            #region Add Choice column

            Console.WriteLine($"\nAdding a new choice column named {choiceColumnSchemaName}...");
            // Add a new Choice column
            var choiceColumn = new PicklistAttributeMetadata()
            {
                SchemaName = choiceColumnSchemaName,
                DisplayName = new Label("Choice column for sample", 1033),
                RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                Description = new Label("Description", 1033),
                OptionSet = new OptionSetMetadata
                {
                    IsGlobal = false,
                    OptionSetType = OptionSetType.Picklist,
                    Options = new List<OptionMetadata>
                    {
                        new OptionMetadata{
                            Label = new ("Choice 1", 1033)
                        },
                        new OptionMetadata{
                            Label = new ("Choice 2", 1033)
                        },
                        new OptionMetadata{
                            Label = new ("Choice 3", 1033)
                        }
                    }
                }
            };

            var createChoiceColumnRequest = new CreateAttributeRequest(
                entityLogicalName: "contact", 
                attributeMetadata: choiceColumn);

            await service.SendAsync(createChoiceColumnRequest);

            Console.WriteLine($"\nCreated Choice column: {choiceColumnSchemaName}");

            #endregion Add Choice column

            #region Detect added column

            // Detect changes
            var secondRequest = new RetrieveMetadataChangesRequest
            {
                Query = query, //Same query as before
                // This time passing client version stamp value from previous request
                ClientVersionStamp = clientVersionStamp,
                DeletedMetadataFilters = DeletedMetadataFilters.All
            };

            RetrieveMetadataChangesResponse secondResponse;

            try
            {
                secondResponse = await service.SendAsync<RetrieveMetadataChangesResponse>(secondRequest);
                // Re-set the client version stamp
                clientVersionStamp = secondResponse.ServerVersionStamp;
            }
            catch (ServiceException ex)
            {
                // Check for ErrorCodes.ExpiredVersionStamp (0x80044352)
                // Message: Version stamp associated with the client has expired. Please perform a full sync.
                // Will occur when the timestamp exceeds the Organization.ExpireSubscriptionsInDays value, which is 90 by default.
                if (ex.ODataError.Error.Code == "0x80044352")
                {
                    // TODO
                    // Add code to re-initialize cache
                    return;

                }
                else
                {
                    throw ex;
                }
            }

            // There should be only one representing the choice column just added
            Console.WriteLine($"\nColumns in second response:{secondResponse.EntityMetadata.FirstOrDefault().Attributes.Count}");

            // Update cache to add new items.
            secondResponse.EntityMetadata.FirstOrDefault().Attributes.ToList().ForEach(att =>
            {
                if (!cachedAttributes.Contains(att))
                {
                    cachedAttributes.Add(att);
                }
            });

            // List the current cached Choice columns
            Console.WriteLine($"\nThe current cached choice columns:");
            cachedAttributes
                .ForEach(att =>
                {
                    Console.WriteLine($"\t{att.LogicalName}");
                });
            #endregion Detect added column

            #region Delete Choice Column
            Console.WriteLine($"\nDeleting the choice column named {choiceColumnSchemaName}...");

            var deleteChoiceColumnRequest = new DeleteAttributeRequest(
                entityLogicalName: "contact",
                logicalName: choiceColumnSchemaName.ToLower());

            await service.SendAsync(deleteChoiceColumnRequest);

            Console.WriteLine($"\nDeleted choice column: {choiceColumnSchemaName}");

            #endregion Delete Choice Column

            #region Detect deleted column

            var thirdRequest = new RetrieveMetadataChangesRequest
            {
                Query = query,
                // This time passing client version stamp value from previous request
                ClientVersionStamp = clientVersionStamp,
                DeletedMetadataFilters = DeletedMetadataFilters.Attribute
            };


            RetrieveMetadataChangesResponse thirdResponse;
            try
            {
                thirdResponse = await service.SendAsync<RetrieveMetadataChangesResponse>(thirdRequest);
                // Re-set the client version stamp
                clientVersionStamp = thirdResponse.ServerVersionStamp;

            }
            catch (ServiceException ex)
            {
                // Check for ErrorCodes.ExpiredVersionStamp (0x80044352)
                // Message: Version stamp associated with the client has expired. Please perform a full sync.
                // Will occur when the timestamp exceeds the Organization.ExpireSubscriptionsInDays value, which is 90 by default.
                if (ex.ODataError.Error.Code == "0x80044352")
                {
                    // TODO
                    // Add code to re-initialize cache
                    return;

                }
                else
                {
                    throw ex;
                }
            }

            // Remove deleted choice column from the cache
            thirdResponse.DeletedMetadata[DeletedMetadataFilters.Attribute]
                .ToList()
                .ForEach(guidCollection =>
                {
                    guidCollection.Items.ForEach(id => {
                        cachedAttributes.RemoveAll(a => a.MetadataId == id);
                    });
                });

            // List the current cached options again.
            // The deleted choice column is no longer cached.
            Console.WriteLine($"\nThe current cached choice columns:");
            cachedAttributes
                .ForEach(att =>
                {
                    Console.WriteLine($"\t{att.LogicalName}");
                });

            Console.WriteLine($"\nNotice that '{choiceColumnSchemaName.ToLower()}' is no longer included.");

            #endregion Detect deleted column

        }
        protected static async Task<int?> RetrieveUserUILanguageCode(Service service) {

            var whoIAm = await service.SendAsync<WhoAmIResponse>(new WhoAmIRequest());

            RetrieveMultipleResponse response =
                await service.RetrieveMultiple($"usersettingscollection?" +
                $"$select=uilanguageid&$filter=systemuserid eq {whoIAm.UserId}&$top=1&$count=true");

            if (response.Count > 0) {
                return (int)response.Records[0]["uilanguageid"];
            }
            return null;        
        }



        static async Task TestDeletedMetadataFilters(Service service) {

            string clientVersionStamp = string.Empty;

            List<EntityReference> recordsToDelete = new();
            bool deleteCreatedRecords = true;

            string ExamplePublisherCustomizationPrefix = "sample";
            string ExampleSolutionUniqueName = "examplesolution";
            int PublisherCustomizationOptionValuePrefix = 72700;

            Console.WriteLine("--Starting Metadata Operations sample--");

            #region Section 0: Create Publisher and Solution

            Console.WriteLine("Starting Section 0: Create Publisher and Solution");
            Console.WriteLine();

            // Create a publisher
            JObject examplepublisher = new() {
                    {"friendlyname","Example Publisher" },
                    {"uniquename","examplepublisher" },
                    {"description","An example publisher for samples" },
                    {"customizationprefix",ExamplePublisherCustomizationPrefix },
                    {"customizationoptionvalueprefix",PublisherCustomizationOptionValuePrefix }
                };

            EntityReference examplePublisherReference = await service.Create(
                entitySetName: "publishers",
                record: examplepublisher);

            Console.WriteLine($"Created publisher {examplepublisher["friendlyname"]}");

            recordsToDelete.Add(examplePublisherReference);

            // Create a solution related to the publisher
            JObject exampleSolution = new() {
                    {"friendlyname","Example Solution" },
                    {"uniquename",ExampleSolutionUniqueName },
                    {"description","An example solution for samples" },
                    {"version","1.0.0.0" },
                    {"publisherid@odata.bind",examplePublisherReference.Path }
                };

            EntityReference exampleSolutionReference = await service.Create(
                entitySetName: "solutions",
                record: exampleSolution);

            Console.WriteLine($"Created solution {exampleSolution["friendlyname"]}");
            Console.WriteLine();

            //Must be deleted before publisher, so Insert to top of list!
            recordsToDelete.Insert(0, exampleSolutionReference);

            #endregion Section 0: Create Publisher and Solution

            // Create Entity
            #region Section 1: Create, Retrieve and Update Table

            Console.WriteLine("Starting Section 1: Create, Retrieve and Update Table");
            Console.WriteLine();

            var bankAccountEntitySchemaName = $"{ExamplePublisherCustomizationPrefix}_BankAccount";
            var bankAccountEntityLogicalName = bankAccountEntitySchemaName.ToLower();

            EntityMetadata bankAccountEntityMetadata = new()
            {
                SchemaName = bankAccountEntitySchemaName, //Required
                DisplayName = new Label("Bank Account", 1033),//Required
                DisplayCollectionName = new Label("Bank Accounts", 1033),//Required
                HasNotes = false, //Required
                HasActivities = false, //Required
                Description = new Label(
                        "A table to store information about customer bank accounts", 1033),
                OwnershipType = OwnershipTypes.UserOwned,
                PrimaryNameAttribute = $"{ExamplePublisherCustomizationPrefix}_Name".ToLower(),
                Attributes = new List<AttributeMetadata>() {
                        {
                            new StringAttributeMetadata(){
                            SchemaName = $"{ExamplePublisherCustomizationPrefix}_Name", //Required
                            MaxLength = 100,//Required
                            DisplayName = new Label("Account Name", 1033), //Required
                            RequiredLevel = new AttributeRequiredLevelManagedProperty(
                                AttributeRequiredLevel.None),
                            Description = new Label(
                                "The primary attribute for the Bank Account entity.", 1033),
                            IsPrimaryName = true //Required                            
                            }
                        }
                    }
            };

            CreateEntityRequest createEntityRequest = new(
                entityMetadata: bankAccountEntityMetadata,
                solutionUniqueName: ExampleSolutionUniqueName);

            Console.WriteLine($"Sending request to create the {bankAccountEntitySchemaName} table...");

            var createEntityResponse =
                await service.SendAsync<CreateEntityResponse>(createEntityRequest);

            Console.WriteLine($"Created {bankAccountEntitySchemaName} table.");

            // Delete the entity first, so Insert at top of list
            recordsToDelete.Insert(0, createEntityResponse.TableReference);

            // Retrieve the entire entity definition with no filter or $expanded navigation properties
            // This data is used to update the entity definition later in the sample
            RetrieveEntityDefinitionRequest retrieveEntityDefinitionRequest =
                new(logicalName: bankAccountEntityLogicalName);
            var retrieveEntityDefinitionResponse =
                await service.SendAsync<RetrieveEntityDefinitionResponse>(retrieveEntityDefinitionRequest);

            EntityMetadata retrievedBankAccountEntity = retrieveEntityDefinitionResponse.EntityMetadata;


            #endregion Section 1: Create, Retrieve and Update Table

            #region Section 2: Create, Retrieve and Update Columns
            // Create Attribute
            PicklistAttributeMetadata choiceColumn = new()
            {
                SchemaName = $"{ExamplePublisherCustomizationPrefix}_Choice",
                DisplayName = new Label("Sample Choice", 1033),
                RequiredLevel = new AttributeRequiredLevelManagedProperty(
                                       AttributeRequiredLevel.None),
                Description = new Label("Choice Attribute", 1033),
                OptionSet = new OptionSetMetadata
                {
                    IsGlobal = false,
                    OptionSetType = OptionSetType.Picklist,
                    Options = new List<OptionMetadata> {
                                   new OptionMetadata
                                   {
                                       Label = new Label("Bravo",1033),
                                       Value = int.Parse($"{PublisherCustomizationOptionValuePrefix}0000")
                                   },
                                   new OptionMetadata
                                   {
                                       Label = new Label("Delta",1033),
                                       Value = int.Parse($"{PublisherCustomizationOptionValuePrefix}0001")
                                   },
                                   new OptionMetadata
                                   {
                                       Label = new Label("Alpha",1033),
                                       Value = int.Parse($"{PublisherCustomizationOptionValuePrefix}0002")
                                   },
                                   new OptionMetadata
                                   {
                                       Label = new Label("Charlie",1033),
                                       Value = int.Parse($"{PublisherCustomizationOptionValuePrefix}0003")
                                   },
                                   new OptionMetadata
                                   {
                                       Label = new Label("Foxtrot",1033),
                                       Value = int.Parse($"{PublisherCustomizationOptionValuePrefix}0004")
                                   }
                               }
                }
            };

            CreateAttributeRequest createChoiceColumnRequest = new(
                entityLogicalName: bankAccountEntityLogicalName,
                attributeMetadata: choiceColumn,
                solutionUniqueName: ExampleSolutionUniqueName
                );

            // Create Choice column
            var createChoiceColumnResponse =
                await service.SendAsync<CreateAttributeResponse>(createChoiceColumnRequest);
            Console.WriteLine($"Created Choice column with id:{createChoiceColumnResponse.AttributeId}");
            Console.WriteLine();
            #endregion Section 2: Create, Retrieve and Update Columns

            // Create Relationship

            #region Section 5: Create and retrieve a one-to-many relationship
            // This creates a lookup column on the contact table to associate a contact to a bank account.

            Console.WriteLine("Starting Section 5: Create and retrieve a one-to-many relationship");
            Console.WriteLine();

            #region Create 1:N relationship

            Console.WriteLine("Creating a one-to-many relationship...");

            OneToManyRelationshipMetadata oneToManyRelationshipMetadata = new()
            {
                SchemaName = $"{ExamplePublisherCustomizationPrefix}_BankAccount_Contacts",
                ReferencedAttribute = retrievedBankAccountEntity.PrimaryIdAttribute,
                ReferencedEntity = bankAccountEntityLogicalName,
                ReferencingEntity = "contact",
                Lookup = new LookupAttributeMetadata()
                {
                    SchemaName = $"{ExamplePublisherCustomizationPrefix}_BankAccountId",
                    DisplayName = new Label("Bank Account", 1033),
                    Description =
                         new Label("The bank account this contact has access to.", 1033)
                },
                AssociatedMenuConfiguration = new AssociatedMenuConfiguration
                {
                    Behavior = AssociatedMenuBehavior.UseLabel,
                    Group = AssociatedMenuGroup.Details,
                    Label = new Label("Cardholders", 1033),
                    Order = 10000
                },
                CascadeConfiguration = new CascadeConfiguration
                {
                    Assign = CascadeType.NoCascade,
                    Share = CascadeType.NoCascade,
                    Unshare = CascadeType.NoCascade,
                    RollupView = CascadeType.NoCascade,
                    Reparent = CascadeType.NoCascade,
                    Delete = CascadeType.RemoveLink,
                    Merge = CascadeType.Cascade
                }

            };

            CreateRelationshipRequest createOneToManyRequest = new(
                relationship: oneToManyRelationshipMetadata,
                solutionUniqueName: ExampleSolutionUniqueName);

            // Send the request to create the new one-to-many relationship
            var createOneToManyResponse =
                await service.SendAsync<CreateRelationshipResponse>(createOneToManyRequest);

            Console.WriteLine($"Created one-to-many relationship: {createOneToManyResponse.RelationshipReference.Path}");

            // Insert at top to be deleted first. Otherwise will block deletion of the bank account table
            // because it creates a lookup on the contact table
            recordsToDelete.Insert(0, createOneToManyResponse.RelationshipReference);

            #endregion Create 1:N relationship

            #region Retrieve 1:N relationship

            RetrieveRelationshipRequest retrieveOneToManyRelationshipRequest = new(
                type: RelationshipType.OneToManyRelationship,
                metadataId: createOneToManyResponse.RelationshipReference.Id);

            // Retrieve the relationship
            var retrieveOneToManyRelationshipResponse =
                await service.SendAsync<RetrieveRelationshipResponse<OneToManyRelationshipMetadata>>(retrieveOneToManyRelationshipRequest);

            Console.WriteLine($"Retrieved relationship: {retrieveOneToManyRelationshipResponse.RelationshipMetadata.SchemaName}");
            Console.WriteLine();
            #endregion Retrieve 1:N relationship

            #endregion Section 5: Create and retrieve a one-to-many relationship

            // Create Label



            // Create OptionSet

            InsertOptionValueParameters insertOptionValueParameters = new()
            {
                EntityLogicalName = bankAccountEntityLogicalName,
                AttributeLogicalName = choiceColumn.SchemaName.ToLower(),
                Label = new Label("Echo", 1033),
                Value = int.Parse($"{PublisherCustomizationOptionValuePrefix}0005"),
                SolutionUniqueName = ExampleSolutionUniqueName
            };

            await service.SendAsync(new InsertOptionValueRequest(parameters: insertOptionValueParameters));
            Console.WriteLine("Added new option with label 'Echo'");

            // RetrieveMetadataChanges

            // Define query for all Picklist Choice columns from Contact table
            var query = new EntityQueryExpression
            {
                Properties = new MetadataPropertiesExpression("LogicalName", "Attributes"),
                Criteria = new MetadataFilterExpression(filterOperator: LogicalOperator.And)
                {
                    Conditions = new List<MetadataConditionExpression>{
                        {
                            new MetadataConditionExpression(
                                propertyName:"CreatedOn",
                                conditionOperator: MetadataConditionOperator.GreaterThan,
                                value: new PowerApps.Samples.Metadata.Types.Object{
                                        Type = ObjectType.DateTime,
                                        Value = "10/06/2022"
                                    }
                                )
                        }
                    }
                },
                AttributeQuery = new AttributeQueryExpression
                {
                    Properties = new MetadataPropertiesExpression("LogicalName", "OptionSet", "AttributeTypeName"),
                    Criteria = new MetadataFilterExpression(filterOperator: LogicalOperator.And)
                    {
                        Conditions = new List<MetadataConditionExpression>{
                            {
                                // Only Picklist Option type
                                new MetadataConditionExpression(
                                    propertyName:"CreatedOn",
                                    conditionOperator:MetadataConditionOperator.GreaterThan,
                                    value: new PowerApps.Samples.Metadata.Types.Object(
                                        type:ObjectType.DateTime,
                                        value:"10/06/2022")
                                    )
                            }
                        }
                    }
                }, 
                RelationshipQuery = new RelationshipQueryExpression { 
                    Properties = new MetadataPropertiesExpression("SchemaName"),
                    Criteria = new MetadataFilterExpression(filterOperator: LogicalOperator.And)
                    {
                        Conditions = new List<MetadataConditionExpression>{
                            {
                                // Only Picklist Option type
                                new MetadataConditionExpression(
                                    propertyName:"SchemaName",
                                    conditionOperator:MetadataConditionOperator.Equals,
                                    value: new PowerApps.Samples.Metadata.Types.Object(
                                        type:ObjectType.String,
                                        value:$"{ExamplePublisherCustomizationPrefix}_BankAccount_Contacts")
                                    )
                            }
                        }
                    }
                }
            };

            var initialRequest = new RetrieveMetadataChangesRequest { Query = query };

            var initialResponse = await service.SendAsync<RetrieveMetadataChangesResponse>(initialRequest);

            clientVersionStamp = initialResponse.ServerVersionStamp;


            // Delete OptionSet
            // RetrieveMetadataChanges
            // Delete Label
            // RetrieveMetadataChanges
            // Delete Relationship
            // RetrieveMetadataChanges
            // Delete Attribute
            // RetrieveMetadataChanges
            // Delete Entity
            // RetrieveMetadataChanges


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
                    Console.WriteLine("\nDeleting created records...");
                }
            }
            else
            {
                Console.WriteLine("\nDeleting created records...");
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


        }
    }
}