using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerApps.Samples.Search.Types;
using PowerPlatform.Dataverse.CodeSamples.types;
using System.ServiceModel;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates working with Dataverse search APIs.
    /// </summary>
    /// <remarks>Set the appropriate Url and Username values for your test
    /// environment in the appsettings.json file before running this program.
    /// You will be prompted in the default browser to enter a password.</remarks>
    /// <see cref="https://learn.microsoft.com/power-apps/developer/data-platform/xrm-tooling/use-connection-strings-xrm-tooling-connect#connection-string-parameters"/>
    /// <permission cref="https://github.com/microsoft/PowerApps-Samples/blob/master/LICENSE"
    /// <author>Jim Daly</author>
    class Program
    {

        /// <summary>
        /// Contains the application's configuration settings. 
        /// </summary>
        IConfiguration Configuration { get; }


        /// <summary>
        /// Constructor. Loads the application configuration settings from a JSON file.
        /// </summary>
        Program()
        {

            // Get the path to the appsettings file. If the environment variable is set,
            // use that file path. Otherwise, use the runtime folder's settings file.
            string? path = Environment.GetEnvironmentVariable("DATAVERSE_APPSETTINGS");
            if (path == null) path = "appsettings.json";

            // Load the app's configuration settings from the JSON file.
            Configuration = new ConfigurationBuilder()
                .AddJsonFile(path, optional: false, reloadOnChange: true)
                .Build();
        }

        static void Main()
        {
            Program app = new();

            // Create a Dataverse service client using the default connection string.
            ServiceClient serviceClient =
                new(app.Configuration.GetConnectionString("default"));            

            // Determine the search status
            SearchStatus searchStatus = CheckSearchStatus(service: serviceClient);

            switch (searchStatus)
            {
                case SearchStatus.NotProvisioned:

                    Console.WriteLine("Search not provisioned.");
                    Console.WriteLine("Do you want to enable search for this environent? (y/n) [y]: ");
                    string answer = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(answer) && answer.StartsWith("y", StringComparison.OrdinalIgnoreCase))
                    {
                        EnableSearch(service: serviceClient);

                        Console.WriteLine("Search provisioned. Run this sample again to use the Search APIs.");
                    }

                    break;
                case SearchStatus.ProvisioningInProgress:

                    Console.WriteLine("Search provisioning is still in progress. Try again later");

                    break;
                case SearchStatus.Provisioned:

                    // Demonstrate query
                    OutputSearchQuery(service: serviceClient, searchTerm: "Contoso");
                    // Demonstrate suggest
                    OutputSearchSuggest(service: serviceClient, searchTerm: "Cont");
                    // Demonstrate autocomplete
                    OutputAutoComplete(service: serviceClient, "Con");
                    // Demonstrate search status
                    OutputSearchStatus(service: serviceClient);
                    // Demonstrate Search Statistics
                    OutputSearchStatistics(service: serviceClient);
                    break;
            }
        }
        /// <summary>
        /// Demonstrate query API
        /// </summary>
        /// <param name="service">The authenticated IOrganizationService instance to use.</param>
        /// <param name="searchTerm">The term to search for</param>
        /// <returns></returns>
        static void OutputSearchQuery(IOrganizationService service, string searchTerm)
        {
            Console.WriteLine("OutputSearchQuery START\n");

            searchqueryRequest request = new() { 
                search = searchTerm,
                count = true,
                top = 7,
                entities = JsonConvert.SerializeObject(new List<SearchEntity>()
                {
                    new SearchEntity()
                    {
                        Name = "account",
                        SelectColumns = new List<string>() { "name", "createdon" },
                        SearchColumns = new List<string>() { "name" },
                        Filter = "statecode eq 0"
                    },
                    new SearchEntity()
                    {
                        Name = "contact",
                        SelectColumns = new List<string>() { "fullname", "createdon" },
                        SearchColumns = new List<string>() { "fullname" },
                        Filter = "statecode eq 0"
                    }
                }),
                orderby = JsonConvert.SerializeObject(new List<string>() { "createdon desc" }),
                filter = "createdon gt 2022-08-15"

            };


            var searchqueryResponse = (searchqueryResponse)service.Execute(request);
 
            var queryResults = JsonConvert.DeserializeObject<SearchQueryResults>(searchqueryResponse.response);
  

            Console.WriteLine($"\tCount:{queryResults.Count}");
            Console.WriteLine("\tValue:");
            queryResults.Value.ForEach(result =>
            {

                Console.WriteLine($"\t\tId:{result.Id}");
                Console.WriteLine($"\t\tEntityName:{result.EntityName}");
                Console.WriteLine($"\t\tObjectTypeCode:{result.ObjectTypeCode}");
                Console.WriteLine("\t\tAttributes:");
                foreach (string key in result.Attributes.Keys)
                {
                    Console.WriteLine($"\t\t\t{key}:{result.Attributes[key]}");
                }
                Console.WriteLine("\t\tHighlights:");
                foreach (string key in result.Highlights.Keys)
                {
                    Console.WriteLine($"\t\t\t{key}:");

                    foreach (string value in result.Highlights[key])
                    {
                        Console.WriteLine($"\t\t\t\t{value}:");
                    }
                }
                Console.WriteLine($"\t\tScore:{result.Score}\n");

            });
            Console.WriteLine("OutputSearchQuery END\n");
        }

        /// <summary>
        /// Demonstrate suggest API
        /// </summary>
        /// <param name="service">The authenticated IOrganizationService instance to use.</param>
        /// <param name="searchTerm">The term to use</param>
        /// <returns></returns>
        static void OutputSearchSuggest(IOrganizationService service, string searchTerm)
        {
            Console.WriteLine("OutputSearchSuggest START\n");

            searchsuggestRequest request = new()
            {
                search = searchTerm,
                top = 3
            };

            var searchsuggestResponse = (searchsuggestResponse)service.Execute(request);

            SearchSuggestResults results = JsonConvert.DeserializeObject<SearchSuggestResults>(searchsuggestResponse.response);

            results.Value?.ForEach(suggestion =>
            {
                Console.WriteLine($"\tText:{suggestion.Text}");
                Console.WriteLine("\tDocument: ");
                foreach (string key in suggestion.Document.Keys)
                {
                    Console.WriteLine($"\t\t{key}: {suggestion.Document[key]}");
                }
                Console.WriteLine();
            });

            Console.WriteLine("OutputSearchSuggest END\n");
        }



        /// <summary>
        /// Demonstrate autocomplete API
        /// </summary>
        /// <param name="service">The authenticated IOrganizationService instance to use.</param>
        /// <param name="searchTerm">The term to use</param>
        /// <returns></returns>
        static void OutputAutoComplete(IOrganizationService service, string searchTerm)
        {
            Console.WriteLine("OutputAutoComplete START\n");

            searchautocompleteRequest request = new()
            {
                search = searchTerm,
                filter = null,
                fuzzy = true,
                entities = JsonConvert.SerializeObject(new List<SearchEntity>()
                    {
                        new SearchEntity()
                        {
                            Name = "account",
                            SelectColumns = new List<string>() { "name", "createdon" },
                            SearchColumns = new List<string>() { "name" },
                        }
                    }
                )
            };

            var searchautocompleteResponse = (searchautocompleteResponse)service.Execute(request);

            SearchAutoCompleteResults results = JsonConvert.DeserializeObject<SearchAutoCompleteResults>(searchautocompleteResponse.response);

            Console.WriteLine($"\tSearch: {request.search}");
            Console.WriteLine($"\tValue: {results.Value}");

            Console.WriteLine("\nOutputAutoComplete END\n");
        }

        /// <summary>
        /// Demonstrate status API
        /// </summary>
        /// <param name="service">The authenticated IOrganizationService instance to use.</param>
        /// <returns></returns>
        static void OutputSearchStatus(IOrganizationService service)
        {
            Console.WriteLine("OutputSearchStatus START\n");

            var searchStatusResponse = (searchstatusResponse)service.Execute(new searchstatusRequest());

            JObject ResponseObj = (JObject)JsonConvert.DeserializeObject(searchStatusResponse.response);

            SearchStatusResult results = ResponseObj["value"].ToObject<SearchStatusResult>();

            Console.WriteLine($"\tStatus: {results.Status}");
            Console.WriteLine($"\tLockboxStatus: {results.LockboxStatus}");
            Console.WriteLine($"\tCMKStatus: {results.CMKStatus}");

            if (results.Status == SearchStatus.Provisioned)
            {
                // There will be no results if status is notprovisioned
                if (results.EntityStatusInfo?.Count > 0)
                {
                    Console.WriteLine("\tEntity Status Results:\n");
                    results.EntityStatusInfo.ForEach(result =>
                    {
                        Console.WriteLine($"\t\tentitylogicalname: {result.EntityLogicalName}");
                        Console.WriteLine($"\t\tobjecttypecode: {result.ObjectTypeCode}");
                        Console.WriteLine($"\t\tprimarynamefield: {result.PrimaryNameField}");
                        Console.WriteLine($"\t\tlastdatasynctimestamp: {result.LastDataSyncTimeStamp}");
                        Console.WriteLine($"\t\tlastprincipalobjectaccesssynctimestamp: {result.LastPrincipalObjectAccessSyncTimeStamp}");
                        Console.WriteLine($"\t\tentitystatus: {result.EntityStatus}");

                        Console.WriteLine($"\t\tsearchableindexedfieldinfomap:");
                        result.SearchableIndexedFieldInfoMap.ToList().ForEach(searchableindexedfield =>
                        {
                            Console.WriteLine($"\t\t\t{searchableindexedfield.Key}\t indexfieldname:{searchableindexedfield.Value.IndexFieldName}");
                        });
                        Console.WriteLine("\n");
                    });
                }
            }

            Console.WriteLine("OutputSearchStatus END\n");
        }

        /// <summary>
        /// Demonstrate statistics API
        /// </summary>
        /// <param name="service">The authenticated IOrganizationService instance to use.</param>
        /// <returns></returns>
        static void OutputSearchStatistics(IOrganizationService service)
        {
            Console.WriteLine("OutputSearchStatistics START\n");

            var searchstatisticsResponse = (searchstatisticsResponse)service.Execute(new searchstatisticsRequest());

            JObject ResponseObj = (JObject)JsonConvert.DeserializeObject(value: searchstatisticsResponse.response);

            SearchStatisticsResult results = ResponseObj["value"].ToObject<SearchStatisticsResult>();

            Console.WriteLine($"\tStorageSizeInBytes: {results.StorageSizeInByte}");
            Console.WriteLine($"\tStorageSizeInMb: {results.StorageSizeInMb}");
            Console.WriteLine($"\tDocumentCount: {results.DocumentCount}");

            Console.WriteLine("\nOutputSearchStatistics END");
        }


        /// <summary>
        /// Enables search for the environment
        /// </summary>
        /// <param name="service">The authenticated IOrganizationService instance to use.</param>
        /// <returns></returns>
        static void EnableSearch(IOrganizationService service)
        {
            var whoAmIResponse = (WhoAmIResponse)service.Execute(new WhoAmIRequest());

            Organization organization = new()
            {
                Id = whoAmIResponse.OrganizationId,
                IsExternalSearchIndexEnabled = true
            };

            service.Update(organization);
        }


        /// <summary>
        /// Returns the search status
        /// </summary>
        /// <param name="service">The authenticated IOrganizationService instance to use.</param>
        /// <returns>The SearchStatus</returns>
        static SearchStatus CheckSearchStatus(IOrganizationService service)
        {
            var searchStatusResponse = (searchstatusResponse)service.Execute(new searchstatusRequest());

            JObject ResponseObj = (JObject)JsonConvert.DeserializeObject(searchStatusResponse.response);

            SearchStatusResult results = ResponseObj["value"].ToObject<SearchStatusResult>();

            return results.Status;

        }


    }
}