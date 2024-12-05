using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using MyApp.DataModel;

namespace PowerPlatform_Dataverse_CodeSamples
{
    internal class Program
    {
        static void AssociateRecords(IOrganizationService service,
            Dictionary<string, EntityReference> entityStore)
        {
            // Associate three accounts to a contact record.

            // Create a collection of the entities that will be 
            // associated to the contact.
            var relatedEntities = new EntityReferenceCollection();
            relatedEntities.Add(new EntityReference(Account.EntityLogicalName,
                entityStore["account 1"].Id));
            relatedEntities.Add(new EntityReference(Account.EntityLogicalName,
                entityStore["account 2"].Id));
            relatedEntities.Add(new EntityReference(Account.EntityLogicalName,
                entityStore["account 3"].Id));

            // Create an object that defines the relationship between the contact and account.
            var relationship = new Relationship("account_primary_contact");

            //Associate the contact with the 3 accounts.
            service.Associate(Contact.EntityLogicalName, entityStore["John Doe"].Id,
                relationship, relatedEntities);

            Console.WriteLine("The entities have been associated.");

            //Disassociate the records.
            service.Disassociate(Contact.EntityLogicalName, entityStore["John Doe"].Id,
                relationship, relatedEntities);

            Console.WriteLine("The entities have been disassociated.");
        }

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

            var setupContact = new Contact
            {
                FirstName = "John",
                LastName = "Doe"
            };
            entityStore.Add("John Doe",new EntityReference("contact",
                service.Create(setupContact)));
            Console.WriteLine("Created {0} {1}", setupContact.FirstName,
                setupContact.LastName);

            // Instantiate an account entity record and set its property values.
            var setupAccount1 = new Account
            {
                Name = "Example Account 1"
            };
            entityStore.Add("account 1", new EntityReference("account",
                service.Create(setupAccount1)));
            Console.WriteLine("Created {0}", setupAccount1.Name);

            var setupAccount2 = new Account
            {
                Name = "Example Account 2"
            };
            entityStore.Add("account 2", new EntityReference("account",
                service.Create(setupAccount2)));
            Console.WriteLine("Created {0}", setupAccount2.Name);

            var setupAccount3 = new Account
            {
                Name = "Example Account 3"
            };
            entityStore.Add("account 3", new EntityReference("account",
                service.Create(setupAccount3)));
            Console.WriteLine("Created {0}", setupAccount3.Name);
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
            AssociateRecords(service, entityStore);

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
