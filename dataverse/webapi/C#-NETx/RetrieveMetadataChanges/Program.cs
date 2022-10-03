using PowerApps.Samples;
using PowerApps.Samples.Metadata.Messages;
using PowerApps.Samples.Metadata.Types;

namespace RetrieveMetadataChanges
{
    internal class Program
    {
        static async Task Main()
        {
            Config config = App.InitializeApp();

            var service = new Service(config);

            var query = new EntityQueryExpression
            {
                Properties = new MetadataPropertiesExpression("SchemaName", "Attributes", "IsAuditEnabled", "CreatedOn", "OwnershipType", "OneToManyRelationships"),
                Criteria = new MetadataFilterExpression(LogicalOperator.Or)
                {
                    Conditions = new List<MetadataConditionExpression>
                        {
                        {
                            new MetadataConditionExpression
                            {
                                PropertyName = "SchemaName",
                                ConditionOperator = MetadataConditionOperator.Equals,
                                Value = new PowerApps.Samples.Metadata.Types.Object
                                {
                                    Type = ObjectType.String,
                                    Value = "Account"
                                }
                            }
                        }
                        //{
                        //    new MetadataConditionExpression
                        //    {
                        //        PropertyName = "LogicalName",
                        //        ConditionOperator = MetadataConditionOperator.In,
                        //        Value = new PowerApps.Samples.Metadata.Types.Object
                        //        {
                        //            Type = ObjectType.StringArray,
                        //            Value = "['contact','task']"
                        //        }
                        //    }
                        //},
                        //{
                        //    new MetadataConditionExpression
                        //    {
                        //        PropertyName = "AutoCreateAccessTeams",
                        //        ConditionOperator = MetadataConditionOperator.Equals,
                        //        Value = new PowerApps.Samples.Metadata.Types.Object
                        //        {
                        //            Type = ObjectType.Bool,
                        //            Value = true.ToString()
                        //        }
                        //    }
                        //},
                        //{
                        //    new MetadataConditionExpression
                        //    {
                        //        PropertyName = "IsAuditEnabled",
                        //        ConditionOperator = MetadataConditionOperator.Equals,
                        //        Value = new PowerApps.Samples.Metadata.Types.Object
                        //        {
                        //            Type = ObjectType.Bool,
                        //            Value = true.ToString()
                        //        }
                        //    }
                        //},
                        //{
                        //    new MetadataConditionExpression
                        //    {
                        //        PropertyName = "CreatedOn",
                        //        ConditionOperator = MetadataConditionOperator.GreaterThan,
                        //        Value = new PowerApps.Samples.Metadata.Types.Object
                        //        {
                        //            Type = ObjectType.DateTime,
                        //            Value = DateTime.Parse("1/1/2022").ToString()
                        //        }
                        //    }
                        //},
                        //{
                        //    new MetadataConditionExpression
                        //    {
                        //        PropertyName = "DataProviderId",
                        //        ConditionOperator = MetadataConditionOperator.Equals,
                        //        Value = new PowerApps.Samples.Metadata.Types.Object
                        //        {
                        //            Type = ObjectType.Guid,
                        //            Value = "1d9bde74-9ebd-4da9-8ff5-aa74945b9f74"
                        //        }
                        //    }
                        //},
                        //{
                        //    new MetadataConditionExpression
                        //    {
                        //        PropertyName = "OwnershipType",
                        //        ConditionOperator = MetadataConditionOperator.Equals,
                        //        Value = new PowerApps.Samples.Metadata.Types.Object
                        //        {
                        //            Type = ObjectType.EntityOwnerShipType,
                        //            Value = OwnershipTypes.UserOwned.ToString()
                        //        }
                        //    }
                        //}

                    }
                },
                AttributeQuery = new AttributeQueryExpression
                {
                    Properties = new MetadataPropertiesExpression("SchemaName", "RequiredLevel"),
                    Criteria = new MetadataFilterExpression(LogicalOperator.Or)
                    {
                        Conditions = new List<MetadataConditionExpression>
                        {
                             {
                                new MetadataConditionExpression
                                {
                                    PropertyName = "RequiredLevel",
                                    ConditionOperator = MetadataConditionOperator.Equals,
                                    Value = new PowerApps.Samples.Metadata.Types.Object
                                    {
                                        Type = ObjectType.AttributeRequiredLevel,
                                        Value = AttributeRequiredLevel.SystemRequired.ToString()
                                    }
                                }
                            }
                        }
                    }
                },
                RelationshipQuery = new RelationshipQueryExpression { 
                    Properties = new MetadataPropertiesExpression("SchemaName", "RelationshipType", "RelationshipBehavior", "ReferencingAttribute", "ReferencingEntity"),
                    Criteria= new MetadataFilterExpression(LogicalOperator.Or)
                    { 
                        Conditions = new List<MetadataConditionExpression> 
                        {
                            {
                                new MetadataConditionExpression
                                {
                                    PropertyName = "RelationshipType",
                                    ConditionOperator = MetadataConditionOperator.Equals,
                                    Value = new PowerApps.Samples.Metadata.Types.Object
                                    {
                                        Type = ObjectType.RelationshipType,
                                        Value = RelationshipType.OneToManyRelationship.ToString()
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var request = new RetrieveMetadataChangesRequest
            {
                Query = query,
                AppModuleId = Guid.Parse("4bab325a-222b-450e-9fb9-991b507ab627")
            };


            var response = await service.SendAsync<RetrieveMetadataChangesResponse>(request);

            Console.WriteLine($"ServerVersionStamp : {response.ServerVersionStamp}");

            response.EntityMetadata[0].OneToManyRelationships.ForEach(omr => {

                    Console.WriteLine($"{omr.SchemaName}: ReferencingEntity {omr.ReferencingEntity}");
              
            });

            //var request2 = new RetrieveMetadataChangesRequest { 
            //    Query = query,
            //    ClientVersionStamp = response.ServerVersionStamp,
            //    DeletedMetadataFilters = DeletedMetadataFilters.All
            
            //};

            //var response2 = await service.SendAsync<RetrieveMetadataChangesResponse>(request2);

            //response2.EntityMetadata.ForEach(em => {

            //    Console.WriteLine($"{em.SchemaName}");

            //});

        }
    }
}