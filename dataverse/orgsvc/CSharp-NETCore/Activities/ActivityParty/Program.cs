using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using MyApp.DataModel;

namespace PowerPlatform_Dataverse_CodeSamples
{
    internal class Program
    {
        /// <summary>
        /// Create a letter activity.
        /// </summary>
        /// <param name="service">Authenticated web service connection.</param>
        /// <param name="entityStore">Entity name and reference collection.</param>
        /// <param name="fromContacts">Contains contacts that the letter is being sent from.</param>
        /// <param name="toContacts">Contains contacts that the letter is being sent to.</param>
        /// <returns></returns>
        static public bool CreateLetter(IOrganizationService service,
            Dictionary<string, EntityReference> entityStore,
            EntityReferenceCollection fromContacts,
            EntityReferenceCollection toContacts)
        {
            // Use the OrganizationServiceContext to track and execute the entity operations.
            var orgContext = new OrganizationServiceContext(service);

            // Create an activity party for each From: contact.
            var fromActivityPartys = new ActivityParty[fromContacts.Count];

            foreach (var contact in fromContacts)
            {
                var activityParty = new ActivityParty
                {
                    PartyId = contact
                };
                fromActivityPartys.Append(activityParty);
            }

            // Create an activity party for each To: contact.
            var toActivityPartys = new ActivityParty[toContacts.Count];
            
            foreach (var contact in toContacts)
            {
                var activityParty = new ActivityParty
                {
                    PartyId = contact
                };
                toActivityPartys.Append(activityParty);
            }

            // Create a letter activity.
            var letter = new Letter
            {
                RegardingObjectId = toContacts[0],
                Subject = "Sample Letter Activity",
                ScheduledEnd = DateTime.Now + TimeSpan.FromDays(5),
                Description = File.ReadAllText("letter.txt"),
                From = fromActivityPartys,
                To = toActivityPartys
            };

            // Add the letter activity to the context.
            orgContext.AddObject(letter);

            try
            {
                // Commit the context changes to Dataverse.
                SaveChangesResultCollection results =
                    orgContext.SaveChanges(SaveChangesOptions.None);

                // Check for success and handle failure.
                if (results.Count > 0 && results[0].Error == null)
                {
                    CreateResponse response = (CreateResponse)results[0].Response;

                    entityStore.Add(letter.Subject,
                        new EntityReference("letter", response.id));

                    Console.WriteLine($"CreateLetter(): letter activity created with ID {response.id}");
                    return true;
                }
                else
                {
                    Console.WriteLine(
                        "CreateLetter(): an error ocurred creating the letter activity: \n\t" +
                        results[0].Error.Message);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "CreateLetter(): an exception ocurred creating the letter activity: \n\t" + ex.Message);
                return false;
            }
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
            Dictionary<string,EntityReference> entityStore;

            // Create a Dataverse service client using the default connection string.
            ServiceClient serviceClient =
                new(Configuration.GetConnectionString("default"));

            // Pre-create any table rows that the Run() method requires.
            Setup(serviceClient, out entityStore);

            // Execute the main logic of this program.
            Run(serviceClient, entityStore);

            // Pause program execution before resource cleanup.
            Console.WriteLine("Press any key to undo environment data changes.");
            Console.ReadKey();

            // In Dataverse, delete any created table rows and then dispose the service connection.
            Cleanup(serviceClient, entityStore);
            serviceClient.Dispose();
        }

        /// <summary>
        /// Executes the code being demonstrated by this program.
        /// </summary>
        /// <param name="service">Authenticated web service connection.</param>
        /// <param name="entityStore">Entity name and reference collection.</param>
        /// <returns>True if successful; otherwise false.</returns>
        static public bool Run(IOrganizationService service, 
            Dictionary<string, EntityReference> entityStore)
        {
            return CreateLetter(service, entityStore, 
                new EntityReferenceCollection { entityStore["contact1"] },
                new EntityReferenceCollection { entityStore["contact2"], entityStore["contact3"] });
        }

        /// <summary>
        /// Creates any pre-existing entity records required by the Run() method.
        /// </summary>
        /// <param name="service">Authenticated web service connection.</param>
        /// <param name="entityStore">Entity name and reference collection.</param>
        static public void Setup(IOrganizationService service, out Dictionary<string, 
            EntityReference> entityStore)
        {
            // Used to track any entities created by this program.
            entityStore = new Dictionary<string, EntityReference>();

            // Create three contacts to be used for the letter.
            var contact1 = new Entity("contact")    
            {
                ["firstname"] = "Mary Kay",
                ["lastname"] = "Andersen",
                ["address1_line1"] = "23 Market St.",
                ["address1_city"] = "Sammamish",
                ["address1_stateorprovince"] = "MT",
                ["address1_postalcode"] = "99999",
                ["telephone1"] = "12345678",
                ["emailaddress1"] = "marykay@contoso.com"
            };

            var contact2 = new Entity("contact")
            {
                ["firstname"] = "Joe",
                ["lastname"] = "Andreshak",
                ["address1_line1"] = "23 Market St.",
                ["address1_city"] = "Sammamish",
                ["address1_stateorprovince"] = "MT",
                ["address1_postalcode"] = "99999",
                ["telephone1"] = "12345678",
                ["emailaddress1"] = "joe@contoso.com"
            };

            var contact3 = new Entity("contact")
            {
                ["firstname"] = "Denise",
                ["lastname"] = "Smith",
                ["address1_line1"] = "23 Market St.",
                ["address1_city"] = "Sammamish",
                ["address1_stateorprovince"] = "MT",
                ["address1_postalcode"] = "99999",
                ["telephone1"] = "12345678",
                ["emailaddress1"] = "denise@contoso.com"
            };

            try
            {
                contact1.Id = service.Create(contact1);
                entityStore.Add("contact1", new EntityReference("contact",contact1.Id));

                contact2.Id = service.Create(contact2);
                entityStore.Add("contact2", new EntityReference("contact", contact2.Id));

                contact3.Id = service.Create(contact3);
                entityStore.Add("contact3", new EntityReference("contact", contact3.Id));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Setup(): an error ocurred creating table row data. \n\t"+ex.Message);
                Console.WriteLine($"Setup(): some contacts could not be created.");
                throw;
            }

        }

        /// <summary>
        /// Delete any entity records (table rows) created by this program.
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
            keysToDelete.Reverse();

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
