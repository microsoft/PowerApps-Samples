using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Newtonsoft.Json.Linq;

namespace PowerPlatform.Dataverse.CodeSamples
{
    internal class Utility
    {
        private static readonly string SensorDataSchemaName = "contoso_SensorData";
        private static readonly string SensorDataLogicalName = SensorDataSchemaName.ToLower();
        private static readonly string DeviceIdSchemaName = "contoso_DeviceId";
        private static readonly string SensorTypeSchemaName = "contoso_SensorType";
        private static readonly string ValueSchemaName = "contoso_Value";
        private static readonly string TimeStampSchemaName = "contoso_TimeStamp";
        private static readonly string EnergyConsumptionSchemaName = "contoso_EnergyConsumption";


        /// <summary>
        /// Creates the contoso_SensorData table used in this sample.
        /// </summary>
        /// <param name="client">Authenticated ServiceClient instance.</param>
        internal static void CreateSensorDataEntity(ServiceClient client)
        {
            Console.WriteLine($"Creating {SensorDataSchemaName} table...");

            // Using Web API via ServiceClient.ExecuteWebRequest because SDK doesn't yet have
            // EntityMetadata.TableType property
            JObject entityMetadataObject = new()
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
                { "TableType", "Elastic" }, // This makes it an elastic table
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

            Dictionary<string, List<string>> customHeaders = new()
            {
                ["Content-Type"] = new List<string>() { "application/json" }
            };

            client.ExecuteWebRequest(HttpMethod.Post, "EntityDefinitions", entityMetadataObject.ToString(), customHeaders);

            Console.WriteLine($"\t{SensorDataSchemaName} table created.");

            Console.WriteLine($"Creating {DeviceIdSchemaName} column...");
            CreateAttributeRequest createDeviceIdAttributeRequest = new()
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
            Console.WriteLine($"\t{DeviceIdSchemaName} column created.");

            Console.WriteLine($"Creating {ValueSchemaName} column...");
            CreateAttributeRequest createValueAttributeRequest = new()
            {
                EntityName = SensorDataSchemaName.ToLower(),
                Attribute = new IntegerAttributeMetadata
                {
                    SchemaName = ValueSchemaName,
                    DisplayName = new Label("Value", 1033),
                }
            };
            client.Execute(createValueAttributeRequest);
            Console.WriteLine($"\t{ValueSchemaName} column created.");

            Console.WriteLine($"Creating {TimeStampSchemaName} column...");
            CreateAttributeRequest createTimeStampAttributeRequest = new()
            {
                EntityName = SensorDataSchemaName.ToLower(),
                Attribute = new DateTimeAttributeMetadata()
                {
                    SchemaName = TimeStampSchemaName,
                    DisplayName = new Label("Time Stamp", 1033),
                }
            };
            client.Execute(createTimeStampAttributeRequest);
            Console.WriteLine($"\t{TimeStampSchemaName} column created.");

            Console.WriteLine($"Creating {EnergyConsumptionSchemaName} column...");
            CreateAttributeRequest createSensorDataAttributeRequest = new()
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
            Console.WriteLine($"\t{EnergyConsumptionSchemaName} column created.\n");
        }

        /// <summary>
        /// Deletes the contoso_SensorData table created for the sample
        /// </summary>
        /// <param name="service">An authenticated client that implemnents the IOrganizationService interface.</param>
        internal static void DeleteSensorDataEntity(IOrganizationService service)
        {
            Console.WriteLine($"Deleting {SensorDataLogicalName} table...");
            try
            {
                DeleteEntityRequest deleteEntityRequest = new()
                {
                    LogicalName = SensorDataLogicalName,
                };
                service.Execute(deleteEntityRequest);
                Console.WriteLine($"\t{SensorDataLogicalName} table deleted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}