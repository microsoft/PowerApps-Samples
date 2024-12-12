using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;

namespace PowerPlatform_Dataverse_CodeSamplesPower
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

            // Add a logger using the 'Logging' configuration section in the
            // appsettings.json file, and send the logs to the console.
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
                    builder.AddConsole()
                           .AddConfiguration(app.Configuration.GetSection("Logging")));

            // Create a Dataverse service client using the default connection string.
            ServiceClient serviceClient = new ServiceClient(
                dataverseConnectionString: app.Configuration.GetConnectionString("default"),
                logger: loggerFactory.CreateLogger<Program>());

            // Send a WhoAmI message request to the Organization service to obtain  
            // information about the logged on user.
            WhoAmIResponse resp = (WhoAmIResponse)serviceClient.Execute(new WhoAmIRequest());
            Console.WriteLine("\nUser ID is {0}.", resp.UserId);

            // Pause program execution before resource cleanup.
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
            serviceClient.Dispose();
        }
    }
}