using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        /// <summary>
        /// Function to set up the sample.
        /// </summary>
        /// <param name="service">Specifies the service to connect to.</param>
        ///
        private static void SetUpSample(CrmServiceClient service)
        {
            // Check that the current version is greater than the minimum version
            if (!SampleHelpers.CheckVersion(service, new Version("7.1.0.0")))
            {
                //The environment version is lower than version 7.1.0.0
                return;
            }
        }

        /// <summary>
        /// Creates entities in parallel
        /// </summary>
        /// <param name="svc">The CrmServiceClient instance to use</param>
        /// <param name="entities">A List of entities to create.</param>
        /// <returns></returns>
        private static ConcurrentBag<EntityReference> CreateEntities(CrmServiceClient svc, List<Entity> entities)
        {
            var createdEntityReferences = new ConcurrentBag<EntityReference>();

            Parallel.ForEach(entities,
                new ParallelOptions() { MaxDegreeOfParallelism = maxDegreeOfParallelism },
                () =>
                {
                    //Clone the CrmServiceClient for each thread
                    return svc.Clone();
                },
                (entity, loopState, index, threadLocalSvc) =>
                {
                    // In each thread, create entities and add them to the ConcurrentBag
                    // as EntityReferences
                    createdEntityReferences.Add(
                        new EntityReference(
                            entity.LogicalName,
                            threadLocalSvc.Create(entity)
                            )
                        );

                    return threadLocalSvc;
                },
                (threadLocalSvc) =>
                {
                    //Dispose the cloned CrmServiceClient instance
                    if (threadLocalSvc != null)
                    {
                        threadLocalSvc.Dispose();
                    }
                });

            //Return the ConcurrentBag of EntityReferences
            return createdEntityReferences;
        }

        /// <summary>
        /// Deletes a list of entity references
        /// </summary>
        /// <param name="svc">The CrmServiceClient instance to use</param>
        /// <param name="entityReferences">A List of entity references to delete.</param>
        private static void DeleteEntities(CrmServiceClient svc, List<EntityReference> entityReferences)
        {
            Parallel.ForEach(entityReferences,
                new ParallelOptions() { MaxDegreeOfParallelism = maxDegreeOfParallelism },
                () =>
                {
                    //Clone the CrmServiceClient for each thread
                    return svc.Clone();
                },
                (er, loopState, index, threadLocalSvc) =>
                {
                    // In each thread, delete the entities
                    threadLocalSvc.Delete(er.LogicalName, er.Id);

                    return threadLocalSvc;
                },
                (threadLocalSvc) =>
                {
                    //Dispose the cloned CrmServiceClient instance
                    if (threadLocalSvc != null)
                    {
                        threadLocalSvc.Dispose();
                    }
                });
        }

        /// <summary>
        /// Gets web service connection information from the app.config file.
        /// If there is more than one available, the user is prompted to select
        /// the desired connection configuration by name.
        /// </summary>
        /// <returns>A string containing web service connection configuration information.</returns>
        private static String GetServiceConfiguration()
        {
            // Get available connection strings from app.config.
            int count = ConfigurationManager.ConnectionStrings.Count;

            // Create a filter list of connection strings so that we have a list of valid
            // connection strings for Common Data Service only.
            List<KeyValuePair<String, String>> filteredConnectionStrings =
                new List<KeyValuePair<String, String>>();

            for (int a = 0; a < count; a++)
            {
                if (isValidConnectionString(ConfigurationManager.ConnectionStrings[a].ConnectionString))
                    filteredConnectionStrings.Add
                        (new KeyValuePair<string, string>
                            (ConfigurationManager.ConnectionStrings[a].Name,
                            ConfigurationManager.ConnectionStrings[a].ConnectionString));
            }

            // No valid connections strings found. Write out and error message.
            if (filteredConnectionStrings.Count == 0)
            {
                Console.WriteLine("An app.config file containing at least one valid Common Data Service " +
                    "connection string configuration must exist in the run-time folder.");
                Console.WriteLine("\nThere are several commented out example connection strings in " +
                    "the provided app.config file. Uncomment one of them and modify the string according " +
                    "to your Common Data Service installation. Then re-run the sample.");
                return null;
            }

            // If one valid connection string is found, use that.
            if (filteredConnectionStrings.Count == 1)
            {
                return filteredConnectionStrings[0].Value;
            }

            // If more than one valid connection string is found, let the user decide which to use.
            if (filteredConnectionStrings.Count > 1)
            {
                Console.WriteLine("The following connections are available:");
                Console.WriteLine("------------------------------------------------");

                for (int i = 0; i < filteredConnectionStrings.Count; i++)
                {
                    Console.Write("\n({0}) {1}\t",
                    i + 1, filteredConnectionStrings[i].Key);
                }

                Console.WriteLine();

                Console.Write("\nType the number of the connection to use (1-{0}) [{0}] : ",
                    filteredConnectionStrings.Count);
                String input = Console.ReadLine();
                int configNumber;
                if (input == String.Empty) input = filteredConnectionStrings.Count.ToString();
                if (!Int32.TryParse(input, out configNumber) || configNumber > count ||
                    configNumber == 0)
                {
                    Console.WriteLine("Option not valid.");
                    return null;
                }

                return filteredConnectionStrings[configNumber - 1].Value;
            }
            return null;
        }

        /// <summary>
        /// Verifies if a connection string is valid for Common Data Service.
        /// </summary>
        /// <returns>True for a valid string, otherwise False.</returns>
        private static Boolean isValidConnectionString(String connectionString)
        {
            // At a minimum, a connection string must contain one of these arguments.
            if (connectionString.Contains("Url=") ||
                connectionString.Contains("Server=") ||
                connectionString.Contains("ServiceUri="))
                return true;

            return false;
        }
    }
}