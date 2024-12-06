using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using MyApp.DataModel;


namespace PowerPlatform_Dataverse_CodeSamples
{
    internal class Program
    {
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

            // In Dataverse, delete any created table rows and dispose the service connection.
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

            var contact = new Contact
            {
                FirstName = "John",
                LastName = "Doe"
            };
            entityStore.Add("John Doe",
                new EntityReference("contact", service.Create(contact)));
            Console.WriteLine("Created contact '{0} {1}'", contact.FirstName,
                contact.LastName);

            var account1 = new Account
            {
                Name = "Example Account 1"
            };
            entityStore.Add("account 1",
                new EntityReference("account", service.Create(account1)));
            Console.WriteLine("Created account '{0}'", account1.Name);

            var account2 = new Account
            {
                Name = "Example Account 2"
            };
            entityStore.Add("account 2",
                new EntityReference("account", service.Create(account2)));
            Console.WriteLine("Created account '{0}'", account2.Name);

            var account3 = new Account
            {
                Name = "Example Account 3"
            };
            entityStore.Add("account 3",
                new EntityReference("account", service.Create(account3)));
            Console.WriteLine("Created account '{0}'", account3.Name);
        }

        /// <summary>
        /// Associate three accounts to a contact.
        /// </summary>
        /// <param name="service">Authenticated web service connection.</param>
        /// <param name="entityStore">Contains references for three accounts and
        /// one contact that were created in Setup().</param>
        /// <returns>True if successful; otherwise false.</returns>
        static public bool Run(IOrganizationService service,
            Dictionary<string, EntityReference> entityStore)
        {
            // Define a delegate that associates and disassociates entities.
            Action<IOrganizationService, EntityReference, string, EntityReferenceCollection>
                AssociateAndDisassociate = (service, primaryEntity, relationship, relatedEntities) =>
                {
                    // Create a relationship between a contact and an account.
                    var relation = new Relationship(relationship);

                    // Associate the "John Doe" contact with the three accounts
                    service.Associate(primaryEntity.LogicalName, primaryEntity.Id,
                        relation, relatedEntities);

                    Console.WriteLine($"The entities have been associated.");

                    // Disassociate the "John Doe" contact with the three accounts.
                    service.Disassociate(primaryEntity.LogicalName, primaryEntity.Id,
                        relation, relatedEntities);

                    Console.WriteLine($"The entities have been disassociated.");
                };

            // Create a collection of account entities that will be associated to the contact.
            var relatedEntities = new EntityReferenceCollection();
            relatedEntities.Add(entityStore["account 1"]);
            relatedEntities.Add(entityStore["account 2"]);
            relatedEntities.Add(entityStore["account 3"]);

            // Invoke the delegate.
            AssociateAndDisassociate(service, entityStore["John Doe"], "account_primary_contact", relatedEntities);

            return true;
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
                Console.WriteLine(
                    $"Cleanup(): web service connection not available, cleanup aborted.");
                return;
            }

            if (entityStore == null)
            {
                Console.WriteLine($"Cleanup(): entityStore is null, cleanup aborted.");
                Console.WriteLine($"Cleanup(): run Setup() prior to Cleanup().");
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
                    Console.WriteLine(
                        $"Cleanup(): exception deleting {key}\n\t{ex.Message}");
                    continue;
                }
            }

            // Output a list of entities that could not be deleted.
            if (entityStore.Count > 0)
            {
                Console.WriteLine(
                    $"Cleanup(): the following entities could not be deleted:");

                foreach (var item in entityStore)
                {
                    Console.WriteLine($"Cleanup(): name={item.Key}, " +
                        $"logical name={item.Value.LogicalName}, ID={item.Value.Id}");
                }
            }
        }
    }
}