using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PowerPlatform_Dataverse_CodeSamples
{
    /// <summary>
    /// Demonstrates connecting to the Dataverse Organization service and executing
    /// several common message requests such as Create, Update, Retrieve, and Delete.
    /// </summary>
    /// <remarks>Set the appropriate Url and Username values for your test
    /// environment in the appsettings.json file before running this program.
    /// You will be prompted in the default browser to enter a password.</remarks>
    /// <see cref="https://learn.microsoft.com/power-apps/developer/data-platform/xrm-tooling/use-connection-strings-xrm-tooling-connect#connection-string-parameters"/>
    /// <permission cref="https://github.com/microsoft/PowerApps-Samples/blob/master/LICENSE"
    /// <author>Peter Hecke</author>
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

        static void Main(string[] args)
        {
            Program app = new();

            // Create a Dataverse service client using the default connection string.
            ServiceClient serviceClient =
                new( app.Configuration.GetConnectionString("default") );

            // Create an in-memory account named Nightmare Coffee.
            Entity account = new("account");
            account["name"] = "Nightmare Coffee";

            // Now create that account in Dataverse. Note that the Dataverse
            // created account ID is being stored in the in-memory account
            // for later use with the Update() call.
            account.Id = serviceClient.Create(account);

            // In Dataverse, update the account's name and set it's postal code.
            account["name"] = "Fourth Coffee";
            account["address2_postalcode"] = "98052";
            serviceClient.Update(account);

            // Retrieve the updated account from Dataverse.
            Entity retrievedAccount = serviceClient.Retrieve(
                entityName: account.LogicalName,
                id: account.Id,
                columnSet: new ColumnSet("name", "address2_postalcode")
            );

            Console.WriteLine("Retrieved account name: {0}, postal code: {1}",
                retrievedAccount["name"], retrievedAccount["address2_postalcode"]);

            // Pause program execution before resource cleanup.
            Console.WriteLine("Press any key to undo environment data changes.");
            Console.ReadKey();

            // In Dataverse, delete the created account, and then dispose the connection.
            serviceClient.Delete(account.LogicalName, account.Id);
            serviceClient.Dispose();
        }
    }
}