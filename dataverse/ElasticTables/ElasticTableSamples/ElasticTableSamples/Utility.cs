using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Newtonsoft.Json.Linq;

namespace PowerPlatform.Dataverse.CodeSamples
{
    internal class Utility
    {
        private static string SensorDataSchemaName = "contoso_SensorData";
        private static string DeviceIdSchemaName = "contoso_DeviceId";
        private static string SensorTypeSchemaName = "contoso_SensorType";
        private static string ValueSchemaName = "contoso_Value";
        private static string TimeStampSchemaName = "contoso_TimeStamp";
        private static string EnergyConsumptionSchemaName = "contoso_EnergyConsumption";
        private static string SensorDataLogicalName = SensorDataSchemaName.ToLower();

        internal static void CreateSensorDataEntity(ServiceClient client)
        {
            Console.WriteLine($"Creating {SensorDataSchemaName} entity.. \n");
            EntityMetadata entityMetadata = new EntityMetadata
            {
                SchemaName = SensorDataSchemaName,
                DisplayName = new Label("SensorData", 1033),
                DisplayCollectionName = new Label("SensorData", 1033),
                Description = new Label("Stores IoT data emitted from devices", 1033),
                OwnershipType = OwnershipTypes.UserOwned,
                // TableType = "Elastic",
                IsActivity = false,
                CanCreateCharts = new BooleanManagedProperty(false),
            };

            AttributeMetadata primaryAttributeMetadata = new AttributeMetadata
            {
                SchemaName = SensorTypeSchemaName,
                DisplayName = new Label("Sensor Type", 1033),
                Description = new Label("Type of sensor emitting data", 1033),
            };
            JObject primaryAttributeMetadataObject = JObject.FromObject(primaryAttributeMetadata);
            primaryAttributeMetadataObject["AttributeType"] = "String";
            primaryAttributeMetadataObject["IsPrimaryName"] = true;

            JObject entityMetadataObject = new JObject()
            {
                { "SchemaName", SensorDataSchemaName },
                { "Description", new JObject()
                    {
                        { "@odata.type",  "Microsoft.Dynamics.CRM.Label" },
                        { "LocalizedLabels", new JArray()
                            {
                                new JObject()
                                {
                                    {"@odata.type", "Microsoft.Dynamics.CRM.LocalizedLabel" },
                                    { "Label", "Stores IoT data emitted from devices" },
                                    { "LanguageCode", 1033 }
                                }
                            }
                        }
                    }
                },
                { "DisplayCollectionName", new JObject()
                    {
                        { "@odata.type", "Microsoft.Dynamics.CRM.Label" },
                        { "LocalizedLabels", new JArray()
                            {
                                new JObject()
                                {
                                    {"@odata.type", "Microsoft.Dynamics.CRM.LocalizedLabel" },
                                    { "Label", "Sensor Data" },
                                    { "LanguageCode", 1033 }
                                }
                            }
                        }
                    }
                },
                { "DisplayName", new JObject()
                    {
                        { "@odata.type", "Microsoft.Dynamics.CRM.Label" },
                        { "LocalizedLabels", new JArray()
                            {
                                new JObject()
                                {
                                    { "@odata.type", "Microsoft.Dynamics.CRM.LocalizedLabel" },
                                    { "Label", "Sensor Data" },
                                    { "LanguageCode", 1033}
                                }
                            }
                        }
                    }
                },
                { "OwnershipType", "UserOwned" },
                { "TableType", "Elastic" },
                { "IsActivity", false },
                { "CanCreateCharts", new JObject()
                    {
                        { "Value", false },
                        { "CanBeChanged", true },
                        { "ManagedPropertyLogicalName", "cancreatecharts" }
                    }
                },
                { "HasActivities", false },
                { "HasNotes", false },
                { "Attributes", new JArray()
                    {
                        new JObject()
                        {
                            { "AttributeType", "String" },
                            { "AttributeTypeName", new JObject()
                                {
                                    { "Value", "StringType" }
                                }
                            },
                            { "Description", new JObject()
                                {
                                    { "@odata.type", "Microsoft.Dynamics.CRM.Label" },
                                    { "LocalizedLabels", new JArray()
                                        {
                                            new JObject()
                                            {
                                                { "@odata.type", "Microsoft.Dynamics.CRM.LocalizedLabel"},
                                                { "Label", "Type of sensor emitting data" },
                                                { "LanguageCode", 1033 }
                                            }
                                        }
                                    }
                                }
                            },
                            { "DisplayName", new JObject()
                                {
                                    { "@odata.type", "Microsoft.Dynamics.CRM.Label" },
                                    { "LocalizedLabels", new JArray()
                                        {
                                            new JObject()
                                            {
                                                { "@odata.type", "Microsoft.Dynamics.CRM.LocalizedLabel"},
                                                { "Label", "Sensor Type" },
                                                { "LanguageCode", 1033}
                                            }
                                        }
                                    }
                                }
                            },
                            { "IsPrimaryName", true },
                            { "RequiredLevel", new JObject()
                                {
                                    { "Value", "None" },
                                    { "CanBeChanged", true},
                                    { "ManagedPropertyLogicalName", "canmodifyrequirementlevelsettings"}
                                }
                            },
                            { "SchemaName", SensorTypeSchemaName},
                            { "@odata.type", "Microsoft.Dynamics.CRM.StringAttributeMetadata"},
                            { "FormatName", new JObject()
                                {
                                    { "Value", "Text"}
                                }
                            },
                            { "MaxLength", 100 }
                        }
                    }
                }
            };

            Dictionary<string, List<string>> customHeaders = new Dictionary<string, List<string>>();
            customHeaders["Content-Type"] = new List<string>() { "application/json" };
            client.ExecuteWebRequest(HttpMethod.Post, $"EntityDefinitions", entityMetadataObject.ToString(), customHeaders);

            Console.WriteLine($"Created {SensorDataSchemaName} entity.\n");

            Console.WriteLine($"Creating {DeviceIdSchemaName} attribute..\n");
            CreateAttributeRequest createDeviceIdAttributeRequest = new CreateAttributeRequest
            {
                EntityName = SensorDataSchemaName.ToLower(),
                Attribute = new StringAttributeMetadata
                {
                    SchemaName = DeviceIdSchemaName,
                    DisplayName = new Label("Device Id", 1033),
                    MaxLength = 1000,
                }
            };
            client.Execute(createDeviceIdAttributeRequest);
            Console.WriteLine($"Created {DeviceIdSchemaName} attribute.\n");

            Console.WriteLine($"Creating {ValueSchemaName} attribute..\n");
            CreateAttributeRequest createValueAttributeRequest = new CreateAttributeRequest
            {
                EntityName = SensorDataSchemaName.ToLower(),
                Attribute = new IntegerAttributeMetadata
                {
                    SchemaName = ValueSchemaName,
                    DisplayName = new Label("Value", 1033),
                }
            };
            client.Execute(createValueAttributeRequest);
            Console.WriteLine($"Created {ValueSchemaName} attribute.\n");

            Console.WriteLine($"Creating {TimeStampSchemaName} attribute..\n");
            CreateAttributeRequest createTimeStampAttributeRequest = new CreateAttributeRequest
            {
                EntityName = SensorDataSchemaName.ToLower(),
                Attribute = new DateTimeAttributeMetadata()
                {
                    SchemaName = TimeStampSchemaName,
                    DisplayName = new Label("Time Stamp", 1033),
                }
            };
            client.Execute(createTimeStampAttributeRequest);
            Console.WriteLine($"Created {TimeStampSchemaName} attribute.\n");

            Console.WriteLine($"Creating {EnergyConsumptionSchemaName} attribute..\n");
            CreateAttributeRequest createSensorDataAttributeRequest = new CreateAttributeRequest
            {
                EntityName = SensorDataSchemaName.ToLower(),
                Attribute = new StringAttributeMetadata
                {
                    SchemaName = EnergyConsumptionSchemaName,
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                    MaxLength = 1000,
                    FormatName = StringFormatName.Json,
                    DisplayName = new Label("Energy Consumption", 1033),
                    Description = new Label("Stores unstructured energy consumption data as reported by device", 1033)
                }
            };

            client.Execute(createSensorDataAttributeRequest);
            Console.WriteLine($"Created {EnergyConsumptionSchemaName} attribute.\n");
        }

        internal static void DeleteSensorDataEntity(ServiceClient client)
        {
            Console.WriteLine($"Deleting {SensorDataLogicalName} entity..\n");
            try
            {
                DeleteEntityRequest deleteEntityRequest = new DeleteEntityRequest()
                {
                    LogicalName = SensorDataLogicalName,
                };
                client.Execute(deleteEntityRequest);
                Console.WriteLine($"Deleted {SensorDataLogicalName} entity.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
