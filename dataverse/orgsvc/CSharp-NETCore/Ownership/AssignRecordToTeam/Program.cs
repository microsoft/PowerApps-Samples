using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using MyApp.DataModel;

internal class Program
{
    /// <summary>
    /// Assign ownership of an entity record.
    /// </summary>
    /// <param name="service">Authenticated web service connection.</param>
    /// <param name="entity1">Entity record to assign.</param>
    /// <param name="entity2">Entity that is to own the record.</param>
    /// <see cref="https://learn.microsoft.com/power-apps/developer/data-platform/security-sharing-assigning"/>
    static public void AssignRecord(IOrganizationService service,
        EntityReference entity1, EntityReference entity2)
    {
        var entityRecord = service.Retrieve(entity1.LogicalName, entity1.Id,
            new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
        entityRecord["ownerid"] = new EntityReference(entity2.LogicalName, entity2.Id);
        service.Update(entityRecord);
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

        // Delete any created table rows and then dispose the service connection.
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

        // Create an account.
        Account setupAccount = new Account { Name = "Example Account" };
        setupAccount.Id = service.Create(setupAccount);
        Console.WriteLine("Created account '{0}'.", setupAccount.Name);

        entityStore.Add("Example Account", 
            new EntityReference("account", setupAccount.Id));

        // Retrieve the default business unit needed to create the team and role.
        var queryDefaultBusinessUnit = new QueryExpression
        {
            EntityName = BusinessUnit.EntityLogicalName,
            ColumnSet = new ColumnSet("businessunitid"),
            Criteria = new FilterExpression()
        };

        queryDefaultBusinessUnit.Criteria.AddCondition("parentbusinessunitid",
            ConditionOperator.Null);

        var defaultBusinessUnit = (BusinessUnit)service.RetrieveMultiple(
            queryDefaultBusinessUnit).Entities[0];

        // Create a team.
        var setupTeam = new Team
        {
            Name = "Team First Coffee",
            BusinessUnitId = new EntityReference(BusinessUnit.EntityLogicalName,
                defaultBusinessUnit.Id)
        };

        setupTeam.Id = service.Create(setupTeam);
        Console.WriteLine("Created team '{0}'.", setupTeam.Name);

        entityStore.Add("Team First Coffee", 
            new EntityReference("team", setupTeam.Id));

        // Create a role.
        var setupRole = new Role
        {
            Name = "Custom Role",
            BusinessUnitId = new EntityReference(BusinessUnit.EntityLogicalName,
                defaultBusinessUnit.Id)
        };

        setupRole.Id = service.Create(setupRole);
        Console.WriteLine("Created role '{0}'.", setupRole.Name);

        entityStore.Add("Custom Role", new EntityReference("role", setupRole.Id));

        // Execute  a query to find the prvReadAccount privilege.
        var queryReadAccountPrivilege = new QueryExpression
        {
            EntityName = Privilege.EntityLogicalName,
            ColumnSet = new ColumnSet("privilegeid", "name"),
            Criteria = new FilterExpression()
        };
        queryReadAccountPrivilege.Criteria.AddCondition("name",
            ConditionOperator.Equal, "prvReadAccount");

        Entity readAccountPrivilege = service.RetrieveMultiple(
            queryReadAccountPrivilege)[0];
        Console.WriteLine("Retrieved '{0}'.", readAccountPrivilege.Attributes["name"]);

        // Add the prvReadAccount privilege to the example role to assure the
        // team can read accounts.
        var addPrivilegesRequest = new AddPrivilegesRoleRequest
        {
            RoleId = setupRole.Id,
            Privileges = new[]
            {
                    // Grant prvReadAccount privilege.
                    new RolePrivilege
                    {
                        PrivilegeId = readAccountPrivilege.Id
                    }
                }
        };
        service.Execute(addPrivilegesRequest);

        Console.WriteLine("Added privilege to role.");

        // Add the role to the team.
        service.Associate(
                   Team.EntityLogicalName,
                   setupTeam.Id,
                   new Relationship("teamroles_association"),
                   new EntityReferenceCollection() {
                           new EntityReference(Role.EntityLogicalName, setupRole.Id) });

        Console.WriteLine("Assigned team to role.");

        // It takes some time for the privileges to propagate to the team. Delay the
        // application until the privilege has been assigned.
        bool teamLacksPrivilege = true;
        while (teamLacksPrivilege)
        {
            var retrieveTeamPrivilegesRequest =
                new RetrieveTeamPrivilegesRequest
                {
                    TeamId = setupTeam.Id,
                };

            var retrieveTeamPrivilegesResponse =
                (RetrieveTeamPrivilegesResponse)service.Execute(
                retrieveTeamPrivilegesRequest);

            foreach (RolePrivilege rp in
                retrieveTeamPrivilegesResponse.RolePrivileges)
            {
                if (rp.PrivilegeId == readAccountPrivilege.Id)
                {
                    teamLacksPrivilege = false;
                    break;
                }
                else
                {
                    System.Threading.Thread.CurrentThread.Join(500);
                }
            }
        }

        return;
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
        var accountRef = entityStore["Example Account"];
        var teamRef    = entityStore["Team First Coffee"];

        AssignRecord(service, accountRef, teamRef);
        Console.WriteLine("'{0}' is now owned by '{1}'.", accountRef.Name, teamRef.Name);

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
            Console.WriteLine("Cleanup(): entityStore collection is null, cleanup aborted.");
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
