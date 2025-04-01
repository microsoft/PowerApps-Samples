using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.PowerPlatform.Dataverse.Client.Utils;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using MyApp.DataModel;

namespace PowerPlatform_Dataverse_CodeSamples
{
    internal class Program
    {
        // <AssociateSecurityRole>
        /// <summary>
        /// Associate a user with a security role.
        /// </summary>
        /// <param name="service">Authenticated web service connection.</param>
        /// <param name="securityRole">Dataverse security role.</param>
        /// <param name="user">A system user.</param>
        static public void AssociateSecurityRole(ServiceClient service, string securityRole, Guid user)
        {
            // Create a query to find the role by name.
            QueryExpression query = new QueryExpression
            {
                EntityName = Role.EntityLogicalName,
                ColumnSet = new ColumnSet("roleid"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression
                        {
                            AttributeName = "name",
                            Operator = ConditionOperator.Equal,
                            Values = {securityRole}
                        }
                    }
                }
            };

            Role targetRole;

            // Retrieve the role.
            EntityCollection roles = service.RetrieveMultiple(query);
            if (roles.Entities.Count > 0)
            {
                targetRole = service.RetrieveMultiple(query).Entities[0].ToEntity<Role>();
            }
            else
            {
                throw new Exception(String.Format("Role named '{0}' not found", securityRole));
            }

            // Associate the user with the role.
            if (targetRole.Id != Guid.Empty && user != Guid.Empty)
            {
                service.Associate("systemuser", user,
                    new Relationship("systemuserroles_association"),
                    new EntityReferenceCollection()
                    {
                        new EntityReference(Role.EntityLogicalName, targetRole.Id)
                    }
                );
            }
        }
        // </AssociateSecurityRole>

        /// <summary>
        /// Contains the application's configuration settings. 
        /// </summary>
        static IConfiguration Configuration { get; }

        /// <summary>
        /// Constructor. Loads the application settings from a JSON configuration file.
        /// </summary>
        static Program()
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
            // Entity name and reference collection.
            Dictionary<string, EntityReference> entityStore;

            // Create a Dataverse service client using the default connection string.
            ServiceClient serviceClient =
                new(Configuration.GetConnectionString("default"));

            // Pre-create any table rows that Run() requires.
            Setup(serviceClient, out entityStore);

            // Execute the main logic of this program
            Run(serviceClient, entityStore);

            // Pause program execution before resource cleanup.
            Console.WriteLine("Press any key to undo environment data changes.");
            Console.ReadKey();

            // In Dataverse, delete any created table rows and then dispose the service connection.
            Cleanup(serviceClient, entityStore);
            serviceClient.Dispose();
        }

        /// <summary>
        /// Initializes any pre-existing data and resources required by the Run() method.
        /// </summary>
        /// <param name="service">Authenticated web service connection.</param>
        /// <param name="entityStore">Entity name and reference collection.</param>
        static public void Setup(IOrganizationService service, out Dictionary<string,
            EntityReference> entityStore)
        {
            entityStore = new Dictionary<string, EntityReference>();

            // This sample does not require any setup. It uses and existing
            // system user and role.
        }

        /// <summary>
        /// The main logic of this program being demonstrated.
        /// </summary>
        /// <param name="service">Authenticated web service connection.</param>
        /// <param name="entityStore">Entity name and reference collection.</param>
        /// <returns>True if successful; otherwise false.</returns>
        static public bool Run(IOrganizationService service,
            Dictionary<string, EntityReference> entityStore)
        {
            // Retrieve information about the logged on user.
            Console.Write("Discovering who you are...");
            WhoAmIRequest request = new WhoAmIRequest();
            WhoAmIResponse response = (WhoAmIResponse)service.Execute(request);
            Console.WriteLine("done.");

            ServiceClient? clientService = service as ServiceClient;
            if (clientService != null)
            {
                Console.Write("Associating your system user record with role 'Basic User'..");
                AssociateSecurityRole(clientService, "Basic User", response.UserId);
                Console.WriteLine("done.");
                Console.WriteLine("\nUse the Power Platform admin center to see that you now have");
                Console.WriteLine("the 'Basic User' role. Afterwards, remove the role if desired.");
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Dispose of any data and resources created by the this program.
        /// </summary>
        /// <param name="service">Authenticated web service connection.</param>
        /// <param name="entityStore">Entity name and reference collection.</param>
        static public void Cleanup(ServiceClient service,
            Dictionary<string, EntityReference> entityStore)
        {
            // Do some checking of the passed parameter values.
            if (service == null || service.IsReady == false)
            {
                Console.WriteLine("Cleanup(): web service connection is not available, cleanup aborted.");
                return;
            }

            if (entityStore == null)
            {
                Console.WriteLine("Cleanup(): entref store collection is null, cleanup aborted.");
                Console.WriteLine("Cleanup(): be sure to run Setup() prior to Cleanup().");
                return;
            }

            // Collect the keys of entities to be deleted.
            var keysToDelete = new List<string>(entityStore.Keys);

            // Delete in Dataverse each entity in the entity store.
            foreach (var key in keysToDelete)
            {
                var entref = entityStore[key];
                try
                {
                    service.Delete(entref.LogicalName, entref.Id);
                    entityStore.Remove(key);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Cleanup(): exception deleting {key}\n\t{ex.Message}");
                    continue;
                }
            }

            // Output a list of entities that could not be deleted.
            if (entityStore.Count > 0)
            {
                Console.WriteLine("Cleanup(): the following entities could not be deleted:");
                foreach (var item in entityStore)
                {
                    Console.WriteLine($"Cleanup(): name={item.Key}, " +
                        $"logical name={item.Value.LogicalName}, ID={item.Value.Id}");
                }
            }
        }
    }
}
