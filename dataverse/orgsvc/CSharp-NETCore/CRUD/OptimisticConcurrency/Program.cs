using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates using optimistic concurrency with RowVersion for update and delete operations.
    /// </summary>
    /// <remarks>
    /// This sample shows how to use the ConcurrencyBehavior.IfRowVersionMatches option
    /// to ensure that updates and deletes only succeed when the row version matches.
    /// Set the appropriate Url and Username values for your test environment
    /// in the appsettings.json file before running this program.
    /// You will be prompted in the default browser to enter a password.
    /// </remarks>
    /// <see cref="https://learn.microsoft.com/power-apps/developer/data-platform/optimistic-concurrency"/>
    class Program
    {
        /// <summary>
        /// Contains the application's configuration settings.
        /// </summary>
        IConfiguration Configuration { get; }

        /// <summary>
        /// Stores entities created by Setup for use in Run and Cleanup.
        /// </summary>
        List<EntityReference> entityStore = new();

        /// <summary>
        /// Constructor. Loads the application configuration settings from a JSON file.
        /// </summary>
        Program()
        {
            // Get the path to the appsettings file. If the environment variable is set,
            // use that file path. Otherwise, use the runtime folder's settings file.
            string? path = Environment.GetEnvironmentVariable("DATAVERSE_APPSETTINGS");
            path ??= "appsettings.json";

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

            if (!serviceClient.IsReady)
            {
                throw new Exception("Failed to connect to Dataverse");
            }

            // Run the sample
            app.Setup(serviceClient);
            app.Run(serviceClient);
            app.Cleanup(serviceClient);

            serviceClient.Dispose();
        }

        /// <summary>
        /// Creates sample data required for the demonstration.
        /// </summary>
        /// <param name="service">The authenticated IOrganizationService instance.</param>
        void Setup(IOrganizationService service)
        {
            Console.WriteLine("\n--Setup--");

            // Create an account record
            Entity account = new("account")
            {
                ["name"] = "Fourth Coffee",
                ["creditlimit"] = new Money(50000)
            };

            Guid accountId = service.Create(account);
            entityStore.Add(new EntityReference("account", accountId));

            Console.WriteLine($"Created account '{account["name"]}' with credit limit of {((Money)account["creditlimit"]).Value}.");
        }

        /// <summary>
        /// Demonstrates optimistic concurrency using RowVersion.
        /// </summary>
        /// <param name="service">The authenticated IOrganizationService instance.</param>
        void Run(IOrganizationService service)
        {
            Console.WriteLine("\n--Run--");

            Guid accountId = entityStore[0].Id;

            // Retrieve the account with the current row version
            Entity account = service.Retrieve(
                entityName: "account",
                id: accountId,
                columnSet: new ColumnSet("name", "creditlimit")
            );

            Console.WriteLine($"Retrieved account. Row version: {account.RowVersion}");

            // Create an in-memory account object from the retrieved account
            // Include the RowVersion to enable optimistic concurrency checking
            Entity updatedAccount = new()
            {
                LogicalName = account.LogicalName,
                Id = account.Id,
                RowVersion = account.RowVersion
            };

            // Update just the credit limit
            updatedAccount["creditlimit"] = new Money(1000000);

            // Create update request with concurrency behavior
            UpdateRequest updateRequest = new()
            {
                Target = updatedAccount,
                ConcurrencyBehavior = ConcurrencyBehavior.IfRowVersionMatches
            };

            // Execute the update
            UpdateResponse updateResponse = (UpdateResponse)service.Execute(updateRequest);
            Console.WriteLine($"Updated account '{account["name"]}' credit limit to {((Money)updatedAccount["creditlimit"]).Value}.");

            // Retrieve the account again to get the new row version
            account = service.Retrieve(
                entityName: "account",
                id: accountId,
                columnSet: new ColumnSet()
            );

            Console.WriteLine($"New row version after update: {account.RowVersion}");

            // Store the row version for cleanup
            entityStore[0].RowVersion = account.RowVersion;
        }

        /// <summary>
        /// Deletes sample data created by Setup, demonstrating optimistic concurrency on delete.
        /// </summary>
        /// <param name="service">The authenticated IOrganizationService instance.</param>
        void Cleanup(IOrganizationService service)
        {
            Console.WriteLine("\n--Cleanup--");
            Console.WriteLine("Do you want to delete the created records? (y/n) [y]: ");
            string? answer = Console.ReadLine();

            bool deleteRecords = string.IsNullOrEmpty(answer) ||
                                answer.Equals("y", StringComparison.OrdinalIgnoreCase);

            if (deleteRecords)
            {
                // Delete the account record using optimistic concurrency
                // The delete will only succeed if the row version matches
                EntityReference accountToDelete = entityStore[0];

                DeleteRequest deleteRequest = new()
                {
                    Target = accountToDelete,
                    ConcurrencyBehavior = ConcurrencyBehavior.IfRowVersionMatches
                };

                try
                {
                    service.Execute(deleteRequest);
                    Console.WriteLine("Account record deleted successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Delete failed: {ex.Message}");

                    // If delete with concurrency check failed, try regular delete
                    Console.WriteLine("Attempting regular delete without concurrency check...");
                    service.Delete(accountToDelete.LogicalName, accountToDelete.Id);
                    Console.WriteLine("Account record deleted.");
                }
            }
            else
            {
                Console.WriteLine("Cleanup skipped.");
            }
        }
    }
}
