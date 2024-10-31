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
            EntityCollection entityStore;

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
        /// <param name="entityStore">Collection of entities created in Dataverse.</param>
        /// <returns>True if successful; otherwise false.</returns>
        public bool Run(IOrganizationService service, EntityCollection entityStore)
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

                //TODO Add ID to letter entity before adding to entityStore
                entityStore.Entities.Add(letter);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Run(): an error ocurred creating the Letter Activity: \r\t"+ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Initializes any pre-existing data and resources required by the Run() method.
        /// </summary>
        /// <param name="service">Authenticated web service connection.</param>
        /// <param name="entityStore">Collection of entities created in Dataverse.</param>
        public void Setup(IOrganizationService service, out EntityCollection entityStore)
        {
            var contact1 = new Entity("contact")    
            {
                ["FirstName"] = "Mary Kay",
                ["LastName"] = "Andersen",
                ["Address1_Line1"] = "23 Market St.",
                ["Address1_City"] = "Sammamish",
                ["Address1_StateOrProvince"] = "MT",
                ["Address1_PostalCode"] = "99999",
                ["Telephone1"] = "12345678",
                ["EMailAddress1"] = "marykay@contoso.com"
            };

            var contact2 = new Entity("contact")
            {
                ["FirstName"] = "Joe",
                ["LastName"] = "Andreshak",
                ["Address1_Line1"] = "23 Market St.",
                ["Address1_City"] = "Sammamish",
                ["Address1_StateOrProvince"] = "MT",
                ["Address1_PostalCode"] = "99999",
                ["Telephone1"] = "12345678",
                ["EMailAddress1"] = "joe@contoso.com"
            };

            var contact3 = new Entity("contact")
            {
                ["FirstName"] = "Denise",
                ["LastName"] = "Smith",
                ["Address1_Line1"] = "23 Market St.",
                ["Address1_City"] = "Sammamish",
                ["Address1_StateOrProvince"] = "MT",
                ["Address1_PostalCode"] = "99999",
                ["Telephone1"] = "12345678",
                ["EMailAddress1"] = "denise@contoso.com"
            };

            entityStore = new EntityCollection();

            try
            {
                contact1.Id = service.Create(contact1);
                entityStore.Entities.Add(contact1);

                contact2.Id = service.Create(contact2);
                entityStore.Entities.Add(contact2);

                contact3.Id = service.Create(contact3);
                entityStore.Entities.Add(contact3);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Setup(): an error ocurred creating table row data. \r\t"+ex.Message);
                Console.WriteLine($"Setup(): some contacts could not be created.");
                throw;
            }

        }

        /// <summary>
        /// Dispose of any data and resources created by the this program.
        /// </summary>
        /// <param name="service">Authenticated web service connection.</param>
        /// <param name="entityStore">Collection of entities created in Dataverse.</param>
        public void Cleanup(ServiceClient service, EntityCollection entityStore)
        {
            // Do some checking of the passed parameter values.
            if (service == null || service.IsReady == false)
            {
                Console.WriteLine("Cleanup(): web service connection is not available, cleanup aborted.");
                return;
            }

            if (entityStore == null)
            {
                Console.WriteLine("Cleanup(): entity store collection is null, cleanup aborted.");
                Console.WriteLine("Cleanup(): be sure to run Setup() prior to Cleanup().");
                return;
            }

            // Delete in Dataverse each entity in the entity store.
            foreach (Entity entity in entityStore.Entities)
            {
                try
                {
                    service.Delete(entity.LogicalName, entity.Id);
                    entityStore.Entities.Remove(entity);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            // Output a list of entities that could not be deleted.
            if(entityStore.Entities.Count > 0)
            {
                Console.WriteLine("Cleanup(): the following entities could not be deleted:");
                foreach (Entity entity in entityStore.Entities)
                {
                    Console.WriteLine($"Cleanup(): logical name={entity.LogicalName}, ID={entity.Id}");
                }
            }
        }
    }
}
