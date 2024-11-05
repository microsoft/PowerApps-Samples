using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using MyApp.DataModel;

namespace PowerPlatform_Dataverse_CodeSamples
{
    internal class Program
    {
        /// <summary>
        /// Contains the application's configuration settings. 
        /// </summary>
        IConfiguration Configuration { get; }

        /// <summary>
        /// Constructor. Loads the application settings from a JSON configuration file.
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
            // Entity name and reference collection.
            Dictionary<string,EntityReference> entityStore;

            Program app = new();

            // Create a Dataverse service client using the default connection string.
            ServiceClient serviceClient =
                new(app.Configuration.GetConnectionString("default"));

            // Pre-create any table rows that Run() requires.
            app.Setup(serviceClient, out entityStore);

            // Execute the main logic of this program
            app.Run(serviceClient, entityStore);

            // Pause program execution before resource cleanup.
            Console.WriteLine("Press any key to undo environment data changes.");
            Console.ReadKey();

            // In Dataverse, delete any created table rows and then dispose the service connection.
            app.Cleanup(serviceClient, entityStore);
            serviceClient.Dispose();
        }

        /// <summary>
        /// The main logic of this program being demonstrated.
        /// </summary>
        /// <param name="service">Authenticated web service connection.</param>
        /// <param name="entityStore">Entity name and reference collection.</param>
        /// <returns>True if successful; otherwise false.</returns>
        public bool Run(IOrganizationService service, 
            Dictionary<string, EntityReference> entityStore)
        {
            // Use the OrganizationServiceContext class and create a LINQ query.
            var orgContext = new OrganizationServiceContext(service);

            // Retrieve the contact table rows that were created during setup.
            List<Contact> contacts = (from c in orgContext.CreateQuery<Contact>()
                                      where c.Address1_City == "Sammamish"
                                      select new Contact
                                      {
                                          ContactId = c.ContactId,
                                          FirstName = c.FirstName,
                                          LastName = c.LastName
                                      }).ToList<Contact>();
            Console.Write(contacts.Count + " contacts retrieved, ");

            // Create an Activity Party (in-memory) object for each contact.
            var activityParty1 = new ActivityParty
            {
                PartyId = new EntityReference(Contact.EntityLogicalName,
                    contacts[0].ContactId .Value),
            };

            var activityParty2 = new ActivityParty
            {
                PartyId = new EntityReference(Contact.EntityLogicalName,
                    contacts[1].ContactId.Value),
            };

            var activityParty3 = new ActivityParty
            {
                PartyId = new EntityReference(Contact.EntityLogicalName,
                    contacts[2].ContactId.Value),
            };

            // Create a Letter activity and set From and To fields to the
            // respective Activity Party rows.

            var _letterBody =
@"Greetings, Mr. Andreshak,\n\n
This is a sample letter activity as part of the SDK Samples.\n\n
Sincerely,\n
Mary Kay Andersen\n\n
cc: Denise Smith";

            var letter = new Letter
            {
                RegardingObjectId = new EntityReference(Contact.EntityLogicalName,
                    contacts[2].ContactId.Value),
                Subject = "Sample Letter Activity",
                ScheduledEnd = DateTime.Now + TimeSpan.FromDays(5),
                Description = _letterBody,
                From = new ActivityParty[] { activityParty1 },
                To = new ActivityParty[] { activityParty3, activityParty2 }
            };

            // Add the letter activity to the context.
            orgContext.AddObject(letter);

            // Commit the context changes to Dataverse.
            try
            {
                var results = orgContext.SaveChanges(SaveChangesOptions.None);

                //TODO Check the status of the results.
                if (results.Count > 0 && results[0].Error == null)
                {
                    entityStore.Add(letter.Subject,
                        new EntityReference("letter", (Guid)letter.ActivityId));
                    return true;
                }
                else
                {
                    Console.WriteLine("Run(): an error ocurred creating the Letter Activity: \n\t" + results[0].Error.Message);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Run(): an error ocurred creating the Letter Activity: \n\t"+ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Initializes any pre-existing data and resources required by the Run() method.
        /// </summary>
        /// <param name="service">Authenticated web service connection.</param>
        /// <param name="entityStore">Entity name and reference collection.</param>
        public void Setup(IOrganizationService service, out Dictionary<string, 
            EntityReference> entityStore)
        {
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

            entityStore = new Dictionary<string, EntityReference>();

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
        /// Dispose of any data and resources created by the this program.
        /// </summary>
        /// <param name="service">Authenticated web service connection.</param>
        /// <param name="entityStore">Entity name and reference collection.</param>
        public void Cleanup(ServiceClient service, 
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
                catch (Exception)
                {
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
