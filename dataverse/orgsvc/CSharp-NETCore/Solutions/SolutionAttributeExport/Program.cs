using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.IO;
using System.Linq;

namespace PowerPlatform.Dataverse.CodeSamples
{
    internal class Program
    {
        /// <summary>
        /// Contains the application's configuration settings. 
        /// </summary>
        private IConfiguration Configuration { get; }

        private static readonly string outputDir = @"C:\temp\";
        private static readonly string path = "appsettings.json";

        static void Main(string[] args)
        {
            Program app = new();

            // Grab the settings from the appsettings.json file
            var connectionString = app.Configuration.GetConnectionString("default");
            var publisherPrefix = app.Configuration.GetSection("SolutionSettings").GetValue<string>("publisherPrefix");
            var attributeSuffix = app.Configuration.GetSection("SolutionSettings").GetValue<string>("attributeSuffix");
            var solutionUniqueName = app.Configuration.GetSection("SolutionSettings").GetValue<string>("solutionUniqueName");
            var solutionFriendlyName = app.Configuration.GetSection("SolutionSettings").GetValue<string>("solutionFriendlyName");
            var solutionDescription = app.Configuration.GetSection("SolutionSettings").GetValue<string>("solutionDescription");

            ServiceClient serviceClient = app.CreateServiceClient(connectionString);
            Solution solution = app.CreateUnmanagedSolution(serviceClient, solutionUniqueName, solutionFriendlyName, solutionDescription);
            app.FindCustomFieldsAndAddToUnmanagedSolution(serviceClient, publisherPrefix, attributeSuffix, solution.UniqueName);
            app.ExportManagedSolution(serviceClient, solution.UniqueName);
        }

        /// <summary>
        /// Constructor. Loads the application configuration settings from a JSON file.
        /// </summary>
        Program()
        {
            // Load the app's configuration settings from the JSON file.
            Configuration = new ConfigurationBuilder()
                .AddJsonFile(path, optional: false, reloadOnChange: true)
                .Build();
        }

        private ServiceClient CreateServiceClient(string? connectionString)
        {
            if (connectionString is null)
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            // Create a Dataverse service client using the default connection string.
            Console.Write("Connecting to Dataverse environment...");
            ServiceClient serviceClient = new(connectionString);

            if (!serviceClient.IsReady)
            {
                throw serviceClient.LastException;
            }
            Console.WriteLine("done.");

            return serviceClient;
        }

        private Solution CreateUnmanagedSolution(ServiceClient serviceClient, string solutionUniqueName, string solutionFriendlyName, string solutionDescription)
        {

            // Retrieve the Default Publisher which has a constant GUID value.
            var defaultPublisher = serviceClient.Retrieve(
                "publisher",
                new Guid("{d21aab71-79e7-11dd-8874-00188b01e34f}"),
                new ColumnSet(new string[] { "friendlyname" }));

            // Create a new unmanaged solution
            Solution solution = new()
            {
                UniqueName = solutionUniqueName,
                Version = "1.0.0.0",
                FriendlyName = solutionFriendlyName,
                Description = solutionDescription,
                PublisherId = new EntityReference("publisher", defaultPublisher.Id)
            };

            // Check whether the solution already exists
            QueryExpression queryCheckForSolution = new()
            {
                EntityName = "solution",
                ColumnSet = new ColumnSet(),
                Criteria = new FilterExpression
                {
                    Conditions = {
                        new ConditionExpression("uniquename", ConditionOperator.Equal, solution.UniqueName)
                    }
                }
            };

            // Attempt to retrieve the solution
            Console.Write($"Attempting to retrieve the {solution.UniqueName} solution...");
            EntityCollection solutionQueryResults = serviceClient.RetrieveMultiple(queryCheckForSolution);
            Console.WriteLine("done.");


            // Create the solution if it doesn't already exist
            Entity? solutionResults = solutionQueryResults.Entities.FirstOrDefault();

            if (solutionResults == null)
            {
                Console.Write($"Solution {solution.UniqueName} not found. Creating...");
                serviceClient.Create(solution);
                Console.WriteLine("done.");
            }
            else
            {
                Console.WriteLine($"Solution {solution.UniqueName} already exists.");
            }

            return solution;
        }

        private void FindCustomFieldsAndAddToUnmanagedSolution(ServiceClient serviceClient, string publisherPrefix, string attributeSuffix, string solutionUniqueName)
        {
            // Retrieve all the fields/attributes in the system.
            Console.Write("Attempting to retrieve all the entity metadata...");
            RetrieveAllEntitiesResponse entitiesResponse =
                (RetrieveAllEntitiesResponse)serviceClient.Execute(
                    new RetrieveAllEntitiesRequest
                    {
                        EntityFilters = EntityFilters.Attributes,
                        RetrieveAsIfPublished = false
                    });
            Console.WriteLine("done.");

            if (entitiesResponse == null)
            {
                throw new Exception("Unable to retrieve entity metadata.");
            }

            ExecuteMultipleRequest multipleRequest = new ExecuteMultipleRequest
            {
                Requests = new OrganizationRequestCollection(),
                Settings = new ExecuteMultipleSettings
                {
                    ContinueOnError = false,
                    ReturnResponses = true
                }
            };

            // For all the entities that start with the given publisher prefix, find all the attributes that end with the given suffix.
            // Add each one to the solution.
            entitiesResponse.EntityMetadata.Where(e => e.LogicalName.StartsWith(publisherPrefix)).ToList().ForEach(
                e => e.Attributes.Where(a => a.LogicalName.EndsWith(attributeSuffix)).ToList().ForEach(
                    a => {
                        AddSolutionComponentRequest addAttributeToSolutionRequest = new()
                        {
                            ComponentType = 2, // Attribute in the ComponentType enum
                            ComponentId = (Guid)a.MetadataId,
                            SolutionUniqueName = solutionUniqueName,
                            IncludedComponentSettingsValues = { },
                            DoNotIncludeSubcomponents = false,
                            AddRequiredComponents = false
                        };

                        Console.Write($"Attempting to add {a.LogicalName} in {a.EntityLogicalName} to the list of requests...");
                        multipleRequest.Requests.Add(addAttributeToSolutionRequest);
                        Console.WriteLine("done.");
                    }));

            Console.Write($"Attempting to execute the list of requests...");
            serviceClient.Execute(multipleRequest);
            Console.WriteLine("done.");
        }

        private void ExportManagedSolution(ServiceClient serviceClient, string solutionUniqueName)
        {
            // Export or package a solution
            ExportSolutionRequest exportSolutionRequest = new();
            exportSolutionRequest.Managed = true;
            exportSolutionRequest.SolutionName = solutionUniqueName;

            ExportSolutionResponse exportSolutionResponse = (ExportSolutionResponse)serviceClient.Execute(exportSolutionRequest);

            byte[] exportXml = exportSolutionResponse.ExportSolutionFile;
            string filename = solutionUniqueName + ".zip";
            string outputPath = outputDir + filename;
            File.WriteAllBytes(outputPath, exportXml);

            Console.WriteLine($"Solution exported to {outputPath}.");
        }
    }
}
