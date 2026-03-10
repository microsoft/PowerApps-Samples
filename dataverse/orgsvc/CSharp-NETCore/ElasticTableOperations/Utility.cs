using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

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
        /// <param name="service">Authenticated IOrganizationService instance.</param>
        internal static void CreateSensorDataEntity(IOrganizationService service)
        {
            Console.WriteLine($"Creating {SensorDataSchemaName} table...");

            EntityMetadata entityMetadata = new()
            {
                SchemaName = SensorDataSchemaName,
                Description = new Label("Stores IoT data emitted from devices", 1033),
                DisplayCollectionName = new Label("Sensor Data", 1033),
                DisplayName = new Label("Sensor Data", 1033),
                OwnershipType = OwnershipTypes.UserOwned,
                TableType = "Elastic",
                CanCreateCharts = new BooleanManagedProperty(false)
            };

            StringAttributeMetadata primaryColumn = new()
            {
                Description = new Label("Type of sensor emitting data", 1033),
                DisplayName = new Label("Sensor Type", 1033),
                SchemaName = SensorTypeSchemaName,
                FormatName = StringFormatName.Text,
                MaxLength = 100
            };

            CreateEntityRequest createEntityRequest = new()
            {
                Entity = entityMetadata,
                PrimaryAttribute = primaryColumn,
                HasActivities = false,
                HasNotes = false,
                HasFeedback = false
            };

            service.Execute(createEntityRequest);

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
            service.Execute(createDeviceIdAttributeRequest);
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
            service.Execute(createValueAttributeRequest);
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
            service.Execute(createTimeStampAttributeRequest);
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

            service.Execute(createSensorDataAttributeRequest);
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