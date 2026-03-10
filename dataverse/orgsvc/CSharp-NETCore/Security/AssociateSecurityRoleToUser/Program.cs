using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using MyApp.DataModel;

namespace PowerPlatform_Dataverse_CodeSamples
{
    internal class Program
    {
        private static Guid _userId;

        // <AssociateSecurityRole>
        /// <summary>
        /// Associate a user with a security role.
        /// </summary>
        /// <param name="service">Authenticated web service connection.</param>
        /// <param name="securityRole">Dataverse security role.</param>
        /// <param name="user">A system user.</param>
        static public void AssociateSecurityRole(IOrganizationService service, string securityRole, Guid user)
        {
            Role targetRole = GetRoleByName(service, securityRole);

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

        // <DisassociateSecurityRole>
        /// <summary>
        /// Disassociate a user with a security role.
        /// </summary>
        /// <param name="service">Authenticated web service connection.</param>
        /// <param name="securityRole">Dataverse security role.</param>
        /// <param name="user">A system user.</param>
        static public void DisassociateSecurityRole(IOrganizationService service, string securityRole, Guid user)
        {
            Role targetRole = GetRoleByName(service, securityRole);

            // Disassociate the user with the role.
            if (targetRole.Id != Guid.Empty && user != Guid.Empty)
            {
                service.Disassociate("systemuser", user,
                    new Relationship("systemuserroles_association"),
                    new EntityReferenceCollection()
                    {
                        new EntityReference(Role.EntityLogicalName, targetRole.Id)
                    }
                );
            }
        }
        // </DisassociateSecurityRole>

        // <GetRoleByName>
        /// <summary>
        /// Retrieve a security role using its name attribute.
        /// </summary>
        /// <param name="service">Authenticated web service connection.param>
        /// <param name="securityRole">Dataverse security role name.</param>
        /// <returns>Dataverse security role.</returns>
        /// <exception cref="Exception">General exception when role name not found.</exception>
        private static Role GetRoleByName(IOrganizationService service, string securityRole)
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

            return targetRole;
        }
        // </GetRoleByName>

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
        /// <param name="entityStore">Not used.</param>
        static public void Setup(IOrganizationService service, out Dictionary<string,
            EntityReference> entityStore)
        {
            entityStore = new Dictionary<string, EntityReference>();

            // This sample does not require any setup. It uses an existing
            // system user and role.
        }

        /// <summary>
        /// The main logic of this program being demonstrated.
        /// </summary>
        /// <param name="service">Authenticated web service connection.</param>
        /// <param name="entityStore">Not used.</param>
        /// <returns>True if successful; otherwise false.</returns>
        static public bool Run(IOrganizationService service,
            Dictionary<string, EntityReference> entityStore)
        {
            // Retrieve information about the logged on user.
            Console.Write("Discovering who you are...");
            WhoAmIRequest request = new WhoAmIRequest();
            WhoAmIResponse response = (WhoAmIResponse)service.Execute(request);
            Console.WriteLine("done.");

            _userId = response.UserId;

            if (service != null)
            {
                Console.Write("Associating your system user record with role 'Basic User'..");
                AssociateSecurityRole(service, "Basic User", _userId);
                Console.WriteLine("done.");
                Console.WriteLine("\nUse the Power Platform admin center to verify that you now have");
                Console.WriteLine("the 'Basic User' role before continuing this program's execution.");
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
        /// <param name="entityStore">Not used.</param>
        static public void Cleanup(ServiceClient service,
            Dictionary<string, EntityReference> entityStore)
        {
            // Do some checking of the passed parameter values.
            if (service == null || service.IsReady == false)
            {
                Console.WriteLine("Cleanup(): web service connection is not available, cleanup aborted.");
                return;
            }

            DisassociateSecurityRole(service, "Basic User", _userId);
        }   
    }
}
