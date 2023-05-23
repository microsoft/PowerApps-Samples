using Microsoft.PowerPlatform.Dataverse.Client;
using Newtonsoft.Json.Linq;

namespace PowerPlatform.Dataverse.CodeSamples
{
    public static class ElasticUtility
    {
        /// <summary>
        /// Creates the table used by projects in this solution.
        /// </summary>
        /// <param name="serviceClient">The ServiceClient instance.</param>
        /// <param name="tableSchemaName">The SchemaName of the table to create.</param>
        public static void CreateExampleTable(ServiceClient serviceClient, string tableSchemaName)
        {
            JObject entityMetadataObject = new JObject()
            {
                { "SchemaName", tableSchemaName },
                { "Description", new JObject()
                    {
                        { "@odata.type",  "Microsoft.Dynamics.CRM.Label" },
                        { "LocalizedLabels", new JArray()
                            {
                                new JObject()
                                {
                                    {"@odata.type", "Microsoft.Dynamics.CRM.LocalizedLabel" },
                                    { "Label", "A table for code samples." },
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
                                    { "Label", "Examples" },
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
                                    { "Label", "Example" },
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
                                                { "Label", "The name of the example record." },
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
                                                { "Label", "Example name" },
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
                            { "SchemaName", "sample_Name"},
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
            serviceClient.ExecuteWebRequest(HttpMethod.Post, "EntityDefinitions", entityMetadataObject.ToString(), customHeaders);
        }
    }
}
