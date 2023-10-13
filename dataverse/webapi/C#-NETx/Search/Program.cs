using Newtonsoft.Json.Linq;
using PowerApps.Samples;
using PowerApps.Samples.Methods;
using PowerApps.Samples.Search.Messages;
using PowerApps.Samples.Search.Types;

namespace Search
{
    internal class Program
    {
        static async Task Main()
        {
            Config config = App.InitializeApp();

            var service = new Service(config);          

            // Determine the search status
            SearchStatus searchStatus = await CheckSearchStatus(service: service);

            switch (searchStatus)
            {
                case SearchStatus.NotProvisioned:

                    Console.WriteLine("Search not provisioned.");
                    Console.WriteLine("Do you want to enable search for this environent? (y/n) [y]: ");
                    string answer = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(answer) && answer.StartsWith("y", StringComparison.OrdinalIgnoreCase))
                    {
                        await EnableSearch(service: service);

                        Console.WriteLine("Search provisioned. Run this sample again to use the Search APIs.");
                    }

                    break;
                case SearchStatus.ProvisioningInProgress:

                    Console.WriteLine("Search provisioning is still in progress. Try again later");

                    break;
                case SearchStatus.Provisioned:

                    // Demonstrate query
                    await OutputSearchQuery(service: service, searchTerm: "Contoso");
                    // Demonstrate suggest
                    await OutputSearchSuggest(service: service, searchTerm: "Cont");
                    // Demonstrate autocomplete
                    await OutputAutoComplete(service: service, "Con");
                    // Demonstrate search status
                    await OutputSearchStatus(service: service);    
                    // Demonstrate Search Statistics
                    await OutputSearchStatistics(service: service);
                    break;
            }
        }

        /// <summary>
        /// Demonstrate query API
        /// </summary>
        /// <param name="service">The authenticated WebAPIService client to use.</param>
        /// <param name="searchTerm">The term to search for</param>
        /// <returns></returns>
         static async Task OutputSearchQuery(Service service, string searchTerm)
        {
            Console.WriteLine("OutputSearchQuery START\n");
            var searchRequest = new QueryParameters()
            {
                Search = searchTerm,
                Count = true,
                Top = 7,
                Entities = new List<SearchEntity>()
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
                },
                OrderBy = new List<string>()
                {
                    "createdon desc"
                },
                Filter = "createdon gt 2022-08-15"
            };

            var request = new SearchQueryRequest(searchRequest);
            var response = await service.SendAsync<SearchQueryResponse>(request);

            Console.WriteLine($"\tCount:{response.Count}");
            Console.WriteLine("\tValue:");
            response.Value.ForEach(result =>
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
        /// <param name="service">The authenticated WebAPIService client to use.</param>
        /// <param name="searchTerm">The term to use</param>
        /// <returns></returns>
        static async Task OutputSearchSuggest(Service service, string searchTerm)
        {
            Console.WriteLine("OutputSearchSuggest START\n");

            SuggestParameters suggestParameters = new()
            {
                Search = searchTerm,
                Top = 3
            };

            SearchSuggestRequest request = new(suggestParameters);

            var response = await service.SendAsync<SearchSuggestResponse>(request);

            response.Value?.ForEach(suggestion =>
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
        /// <param name="service">The authenticated WebAPIService client to use.</param>
        /// <param name="searchTerm">The term to use</param>
        /// <returns></returns>
        static async Task OutputAutoComplete(Service service, string searchTerm)
        {
            Console.WriteLine("OutputAutoComplete START\n");

            var autoCompleteRequest = new AutocompleteParameters()
            {
                Search = searchTerm,
                Filter = null,
                Fuzzy = true,
                Entities = new List<SearchEntity>()
                {
                    new SearchEntity()
                    {
                        Name = "account",
                        SelectColumns = new List<string>() { "name", "createdon" },
                        SearchColumns = new List<string>() { "name" },
                    }
                },
            };

            var request = new SearchAutoCompleteRequest(autoCompleteRequest);

            var response = await service.SendAsync<SearchAutoCompleteResponse>(request);

            Console.WriteLine($"\tSearch: {request.Search}");
            Console.WriteLine($"\tValue: {response.Value}");

            Console.WriteLine("\nOutputAutoComplete END\n");

        }

        /// <summary>
        /// Demonstrate status API
        /// </summary>
        /// <param name="service">The authenticated WebAPIService client to use.</param>
        /// <returns></returns>
        static async Task OutputSearchStatus(Service service)
        {
            Console.WriteLine("OutputSearchStatus START\n");

            var response = await service.SendAsync<SearchStatusResponse>(new SearchStatusRequest());

            Console.WriteLine($"\tStatus: {response.Status}");
            Console.WriteLine($"\tLockboxStatus: {response.LockboxStatus}");
            Console.WriteLine($"\tCMKStatus: {response.CMKStatus}");

            if (response.Status == SearchStatus.Provisioned)
            {
                // There will be no results if status is notprovisioned
                if (response.EntityStatusInfo?.Count > 0)
                {
                    Console.WriteLine("\tEntity Status Results:\n");
                    response.EntityStatusInfo.ForEach(result =>
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
        /// <param name="service">The authenticated WebAPIService client to use.</param>
        /// <returns></returns>
        static async Task OutputSearchStatistics(Service service)
        {
            Console.WriteLine("OutputSearchStatistics START\n");

            var response = await service.SendAsync<SearchStatisticsResponse>(new SearchStatisticsRequest());

            Console.WriteLine($"\tStorageSizeInBytes: {response.StorageSizeInBytes}");
            Console.WriteLine($"\tStorageSizeInMb: {response.StorageSizeInMb}");
            Console.WriteLine($"\tDocumentCount: {response.DocumentCount}");

            Console.WriteLine("\nOutputSearchStatistics END");
        }

        /// <summary>
        /// Enables search for the environment
        /// </summary>
        /// <param name="service">The authenticated WebAPIService client to use.</param>
        /// <returns></returns>
        static async Task EnableSearch(Service service)
        {

            EntityReference organizationReference = new("organizations", service.OrganizationId);
            JObject organizationRecord = new() {
                {"isexternalsearchindexenabled",true }
            };

            await service.Update(organizationReference, organizationRecord);

        }

        /// <summary>
        /// Returns the search status
        /// </summary>
        /// <param name="service">The authenticated WebAPIService client to use.</param>
        /// <returns>The SearchStatus</returns>
        static async Task<SearchStatus> CheckSearchStatus(Service service)
        {
            var response = await service.SendAsync<SearchStatusResponse>(new SearchStatusRequest());

            return response.Status;
        }

    }
}