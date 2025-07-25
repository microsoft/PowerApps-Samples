using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System.Diagnostics;
using System.ServiceModel;
using System.Text;

namespace PowerPlatform_Dataverse_CodeSamples
{
    /// <summary>
    /// Configuration class for sample settings
    /// </summary>
    public class SampleSettings
    {
        public string CustomizationPrefix { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
        public string PublisherUniqueName { get; set; } = string.Empty;
        public string SolutionUniqueName { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string FieldSecurityProfileName { get; set; } = string.Empty;
        public bool DeleteCreatedObjects { get; set; }

        // Computed properties
        public string TableSchemaName => $"{CustomizationPrefix}_{TableName}";
        public string TableLogicalName => TableSchemaName.ToLower();
    }

    internal class Program
    {
        /// <summary>
        /// Contains the application's configuration settings.
        /// </summary>
        static IConfiguration Configuration { get; }

        /// <summary>
        /// Sample configuration settings loaded from appsettings.json
        /// </summary>
        static SampleSettings Settings { get; }

        /// <summary>
        /// Constructor. Loads the application settings from a JSON configuration file.
        /// </summary>
        static Program()
        {

            // Get the path to the appsettings file. If the environment variable is set,
            // use that file path. Otherwise, use the runtime folder's settings file.
            string? path = Environment.GetEnvironmentVariable("DATAVERSE_APPSETTINGS",
                EnvironmentVariableTarget.User);
            if (path == null) path = "appsettings.json";

            // Load the app's configuration settings from the JSON file.
            Configuration = new ConfigurationBuilder()
                .AddJsonFile(path, optional: false, reloadOnChange: true)
                .Build();

            // Load sample settings from configuration
            Settings = new SampleSettings();
            Configuration.GetSection("SampleSettings").Bind(Settings);

            // Enables checkbox output in console sample code
            Console.OutputEncoding = Encoding.UTF8;
        }

        static void Main(string[] args)
        {
            // Start the stopwatch to measure duration of sample
            Stopwatch stopwatch = Stopwatch.StartNew();

            // Collection of references to records to delete when the sample completes.
            Dictionary<string, object> entityStore;


            // Service client for the application user
            ServiceClient ApplicationUserClient;
            // Service client for the system administrator
            ServiceClient SystemAdministratorClient;

            try
            {
                // Initialize both users
                ApplicationUserClient =
                 new(Configuration.GetConnectionString("applicationuser"));

                SystemAdministratorClient =
                new(Configuration.GetConnectionString("default"));

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing service clients: {ex.Message}");
                Console.WriteLine("Make sure the connection strings in appsettings.json are correct.");
                throw;
            }

            Console.WriteLine("\r\nColumn-level security sample started...");

            // Force strong consistency for this sample.
            // This sample applies changes to configurations that can change the output
            // When APIs are used immediately after. This setting is usually not required.
            SystemAdministratorClient.ForceServerMetadataCacheConsistency = true;
            ApplicationUserClient.ForceServerMetadataCacheConsistency = true;

            Console.WriteLine("\r\nSetting up the sample:\r\n");

            Setup(SystemAdministratorClient, ApplicationUserClient, out entityStore);

            Console.WriteLine("\r\nRunning the sample:\r\n");
            Run(SystemAdministratorClient, ApplicationUserClient, entityStore);

            Console.WriteLine("\r\nCleaning up the sample:\r\n");
            Cleanup(SystemAdministratorClient, entityStore);

            Console.WriteLine("\r\nColumn-level security sample completed.");

            // Stop the stopwatch
            stopwatch.Stop();

            // Get the elapsed time as a TimeSpan object
            TimeSpan ts = stopwatch.Elapsed;

            // Format and display the elapsed time
            string elapsedTime = $"{ts.Minutes} minutes and {ts.Seconds} seconds";
            Console.WriteLine("This sample ran for " + elapsedTime);
        }

        private static void Setup(IOrganizationService systemAdminService,
            IOrganizationService appUserService,
            out Dictionary<string, object> entityStore)
        {
            // Used to track any records created by this
            // program so they can be deleted at the end.
            entityStore = [];

            #region Create publisher

            Guid? publisherId = Helpers.GetRecordID(systemAdminService,
                tableLogicalName: "publisher",
                columnLogicalName: "uniquename",
                uniqueStringValue: Settings.PublisherUniqueName);

            if (!publisherId.HasValue)
            {
                Console.WriteLine($"\tCreating {Settings.PublisherUniqueName} publisher...");
                // Create a new publisher
                Entity publisher = new("publisher")
                {
                    ["uniquename"] = Settings.PublisherUniqueName,
                    ["friendlyname"] = "Column-level security sample publisher",
                    ["customizationprefix"] = Settings.CustomizationPrefix,
                    ["customizationoptionvalueprefix"] = 72700,
                    ["description"] = "This publisher was created from sample code"
                };

                try
                {
                    publisherId = systemAdminService.Create(publisher);

                    // Add the publisher to the entity store for later deletion
                    entityStore.Add(
                        Settings.PublisherUniqueName,
                        new EntityReference("publisher", publisherId.Value));
                    Console.WriteLine($"\t☑ {Settings.PublisherUniqueName} created.");

                }
                catch (Exception ex)
                {
                    throw new Exception($"Error creating publisher: {ex.Message}", ex);
                }
            }
            else
            {
                Console.WriteLine($"\t☑ {Settings.PublisherUniqueName} exists.");
            }

            #endregion Create publisher

            #region Create solution
            Guid? solutionId = Helpers.GetRecordID(systemAdminService,
               tableLogicalName: "solution",
               columnLogicalName: "uniquename",
               uniqueStringValue: Settings.SolutionUniqueName);

            if (!solutionId.HasValue)
            {
                Console.WriteLine($"\tCreating {Settings.SolutionUniqueName} solution...");

                Entity solution = new("solution")
                {
                    ["uniquename"] = Settings.SolutionUniqueName,
                    ["friendlyname"] = "Column-Level Security Sample Solution",
                    ["publisherid"] = new EntityReference("publisher", publisherId.Value),
                    ["version"] = "1.0.0.0"
                };

                try
                {
                    solutionId = systemAdminService.Create(solution);

                    entityStore.Add(
                        Settings.SolutionUniqueName,
                        new EntityReference("solution", solutionId.Value));
                    Console.WriteLine($"\t☑ {Settings.SolutionUniqueName} created.");
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error creating solution: {ex.Message}", ex);
                }

            }
            else
            {
                Console.WriteLine($"\t☑ {Settings.SolutionUniqueName} exists.");
            }

            #endregion Create solution

            #region Create custom table

            Guid? tableId = Helpers.GetTableID(systemAdminService, Settings.TableLogicalName);
            if (tableId == null)
            {
                Console.WriteLine($"\tCreating {Settings.TableSchemaName} table...");

                tableId = Helpers.CreateSampleTable(systemAdminService,
                    schemaName: Settings.TableSchemaName,
                    solutionUniqueName: Settings.SolutionUniqueName,
                    customizationPrefix: Settings.CustomizationPrefix);

                entityStore.Add(Settings.TableSchemaName, tableId);
                Console.WriteLine($"\t☑ {Settings.TableSchemaName} table created.");

            }
            else
            {
                Console.WriteLine($"\t☑ {Settings.TableSchemaName} table exists.");
            }

            #endregion Create custom table

            #region Create columns

            ColumnSettings[] columnSettings =
            {
                new() {
                    Name = "Email",
                    DisplayName = "Email",
                    Description = "A sample column containing email addresses.",
                },
                new() {
                    Name = "GovernmentId",
                    DisplayName = "Government ID",
                    Description = "A sample column containing government ID values.",
                },
                new() {
                    Name = "TelephoneNumber",
                    DisplayName = "Telephone Number",
                    Description = "A sample column containing telephone numbers."
                },
                new() {
                    Name = "DateOfBirth",
                    DisplayName = "Date of Birth",
                    Description = "A sample column containing dates of birth."
                }
            };

            Helpers.CreateSampleColumns(systemAdminService,
                tableLogicalName: Settings.TableLogicalName,
                columnSettings: columnSettings,
                solutionUniqueName: Settings.SolutionUniqueName,
                customizationPrefix: Settings.CustomizationPrefix);

            #endregion Create columns

            #region Remove any existing sample data
            // Remove any rows that exist in the sample_example table.
            QueryExpression query = new(Settings.TableLogicalName)
            {
                ColumnSet = new ColumnSet("sample_exampleid")
            };
            EntityCollection results = systemAdminService.RetrieveMultiple(query);
            if (results.Entities.Count > 0)
            {
                foreach (var foundRecord in results.Entities)
                {
                    systemAdminService.Delete(foundRecord.LogicalName, foundRecord.Id);
                }

                Console.WriteLine($"\t☑ Deleted {results.Entities.Count} sample_Example records.");
            }

            #endregion Remove any existing sample data

            #region Add sample data
            // Add three rows of sample data to the sample_example table

            Dictionary<string, string>[] records =
            [
                new() {
                    { "name", "Jayden Phillips" },
                    { "email", "jayden@adatum.com" },
                    { "governmentid", "166-67-5353" },
                    { "telephonenumber", "(736) 555-9012" },
                    { "dateofbirth", "3/25/1974" }
                },
                new() {
                    { "name", "Benjamin Stuart" },
                    { "email", "benjamin@adventure-works.com" },
                    { "governmentid", "211-16-7508" },
                    { "telephonenumber", "(195) 555-7901" },
                    { "dateofbirth", "6/18/1984" }
                },
                new() {
                    { "name", "Avery Howard" },
                    { "email", "avery@alpineskihouse.com" },
                    { "governmentid", "346-20-1720" },
                    { "telephonenumber", "(152) 555-5591" },
                    { "dateofbirth", "9/4/1994" }
                }
            ];

            foreach (var record in records)
            {
                Entity newRecord = new(Settings.TableLogicalName)
                {
                    ["sample_name"] = record["name"],
                    ["sample_email"] = record["email"],
                    ["sample_governmentid"] = record["governmentid"],
                    ["sample_telephonenumber"] = record["telephonenumber"],
                    ["sample_dateofbirth"] = record["dateofbirth"]
                };
                try
                {
                    systemAdminService.Create(newRecord);

                }
                catch (Exception ex)
                {
                    throw new Exception($"Error creating sample record: {ex.Message}", ex);
                }
            }
            Console.WriteLine($"\t☑ Created {records.Length} sample records.");
            #endregion Add sample data

            #region Create security role
            // Grant the application user access to the sample_Example table
            // By associating them with a security role

            // Check if the security role already exists
            Guid? securityRoleId = Helpers.GetRecordID(systemAdminService,
                tableLogicalName: "role",
                columnLogicalName: "name",
                uniqueStringValue: Settings.RoleName);

            // Get information about the application user
            var appUserIsResponse = (WhoAmIResponse)appUserService.Execute(new WhoAmIRequest());
            Guid appUserId = appUserIsResponse.UserId;
            Guid appUserBUId = appUserIsResponse.BusinessUnitId;

            // Create the security role if it doesn't exist
            if (!securityRoleId.HasValue)
            {
                Console.WriteLine($"\tCreating {Settings.RoleName} security role...");
                Entity securityRole = new("role")
                {
                    ["name"] = Settings.RoleName,
                    ["description"] = "This role was created from sample code",
                    ["businessunitid"] = new EntityReference("businessunit", appUserBUId)
                };

                CreateRequest createRequest = new()
                {
                    Target = securityRole,
                    // Include this role in the solution
                    ["SolutionUniqueName"] = Settings.SolutionUniqueName
                };

                securityRoleId = ((CreateResponse)systemAdminService.Execute(createRequest)).id;

                entityStore.Add(
                    Settings.RoleName,
                    new EntityReference("role", securityRoleId.Value));

                Console.WriteLine($"\t☑ {Settings.RoleName} created.");
            }
            else
            {
                Console.WriteLine($"\t☑ {Settings.RoleName} exists.");
            }

            // All the row-level privileges for the sample_Example table
            string[] privileges =
            [
                "prvAppendTosample_Example",
                "prvAppendsample_Example",
                "prvCreatesample_Example",
                "prvReadsample_Example",
                "prvSharesample_Example",
                "prvDeletesample_Example",
                "prvAssignsample_Example",
                "prvWritesample_Example"
            ];

            try
            {
                Examples.AddPrivilegesToRole(systemAdminService,
                    roleId: securityRoleId.Value,
                    depth: PrivilegeDepth.Global,  //Provides organization-level access
                    privilegeNames: privileges);
                Console.WriteLine($"\t☑ Privileges added to {Settings.RoleName}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding privileges to role: {ex.Message}", ex);
            }

            #endregion Create security role

            // Make sure application user is associated with the security role for access to sample_example table
            Helpers.ManageRoleForAppUser(systemAdminService, securityRoleId.Value, appUserId, Settings.RoleName);

            // Verify the row-level privileges that the application user has on the 
            // rows of data in the sample_example table.
            Helpers.VerifyAppUserRowLevelPrivileges(systemAdminService, appUserId);

            #region Create field security profile
            // Make sure no fieldsecurityprofile exists from previous sample run
            Guid? fieldsecurityProfileId = Helpers.GetRecordID(
                service: systemAdminService,
                tableLogicalName: "fieldsecurityprofile",
                columnLogicalName: "name",
                uniqueStringValue: Settings.FieldSecurityProfileName);

            if (fieldsecurityProfileId.HasValue)
            {
                systemAdminService.Delete("fieldsecurityprofile", fieldsecurityProfileId.Value);
            }

            // Create new field security profile

            Entity fieldSecurityProfile = new("fieldsecurityprofile")
            {
                ["name"] = Settings.FieldSecurityProfileName,
                ["description"] = "A field security profile created for the column-level security sample."
            };

            try
            {
                CreateRequest createprofileRequest = new()
                {
                    Target = fieldSecurityProfile,
                    ["SolutionUniqueName"] = Settings.SolutionUniqueName
                };

                var createProfileResponse = (CreateResponse)systemAdminService.Execute(createprofileRequest);

                fieldsecurityProfileId = createProfileResponse.id;

                // Make this available to code in the Run method
                entityStore.Add(
                    Settings.FieldSecurityProfileName,
                    new EntityReference("fieldsecurityprofile", fieldsecurityProfileId.Value));

                Console.WriteLine($"\t☑ Created {Settings.FieldSecurityProfileName}");

            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating field security profile {ex.Message}", ex);
            }

            #endregion Create field security profile

            #region Associate appUser to field security profile

            // Check whether the app user is already associated to
            // the field security profile, and associate them if not.

            Helpers.ManageFieldSecurityProfileForAppUser(
               systemAdminService: systemAdminService,
               fieldSecurityProfileId: fieldsecurityProfileId.Value,
               appUserId: appUserId,
               fieldSecurityProfileName: Settings.FieldSecurityProfileName);

            #endregion Associate appUser to field security profile

            // Pause to let the cache catch up to the changes
            Helpers.WaitForCacheUpdate();
        }

        private static void Run(IOrganizationService systemAdminService,
            IOrganizationService appUserService,
            Dictionary<string, object> entityStore)
        {
            #region Determine whether a column can be secured

            // Creates a CSV file with data about columns that are secured
            // or are limited in how they can be secured.  Any column not included can be secured.
            Examples.DumpColumnSecurityInfo(
                service: systemAdminService,
                filepath: Environment.CurrentDirectory);

            string path = Path.Combine(Environment.CurrentDirectory, "ColumnSecurityInfo.csv");

            Console.WriteLine($"\t☑ Dumped column security information into this file:");
            Console.WriteLine($"\t\\bin\\Debug\\net8.0\\ColumnSecurityInfo.csv\r\n");

            #endregion Determine whether a column can be secured

            #region Discover which columns are already secured 

            List<string> columns = Examples.GetSecuredColumnList(systemAdminService);
            Console.WriteLine("\tThese are the secured columns in this environment:");
            foreach (string column in columns)
            {
                Console.WriteLine($"\t-{column}");
            }

            #endregion Discover which columns are already secured 

            #region Secure a column

            //Retrieve the data as the application user to show unsecured behavior
            Console.WriteLine("\r\n\tBefore columns are secured, the application user can see this data:");

            Helpers.ShowExampleRows(Helpers.GetExampleRows(appUserService));

            Console.Write("\r\n\tSecuring columns...");

            Examples.SetColumnIsSecured(
                systemAdminService,
                Settings.TableLogicalName,
                "sample_email",
                true,
                Settings.SolutionUniqueName);

            Console.Write("☑ Email,");

            Examples.SetColumnIsSecured(
               systemAdminService,
               Settings.TableLogicalName,
               "sample_governmentid",
               true,
               Settings.SolutionUniqueName);

            Console.Write("☑ Government ID,");

            Examples.SetColumnIsSecured(
               systemAdminService,
               Settings.TableLogicalName,
               "sample_telephonenumber",
               true,
               Settings.SolutionUniqueName);

            Console.Write("☑ Telephone Number,");

            Examples.SetColumnIsSecured(
               systemAdminService,
               Settings.TableLogicalName,
               "sample_dateofbirth",
               true,
               Settings.SolutionUniqueName);

            Console.Write(" and ☑ Date of Birth.\r\n");

            //Retrieve the data as the application user to show secured behavior

            Console.WriteLine("\r\n\tAfter columns are secured, the application user can see this data:");
            Helpers.ShowExampleRows(Helpers.GetExampleRows(appUserService));

            #endregion Secure a column

            #region Manage read access to secured column

            // Get information about the application user
            var appUserIsResponse = (WhoAmIResponse)appUserService.Execute(new WhoAmIRequest());
            // Need an a reference to the user to grant access
            EntityReference appUserReference = new("systemuser", appUserIsResponse.UserId);

            // Retrieve the three records

            QueryExpression query = new(Settings.TableLogicalName)
            {
                ColumnSet = new ColumnSet(
                   "sample_name",
                   "sample_email",
                   "sample_governmentid",
                   "sample_telephonenumber",
                   "sample_dateofbirth"),
                Criteria = new FilterExpression(),
                Orders = {
                    {
                        new OrderExpression(
                            "sample_name",
                            OrderType.Descending)
                    }
                }
            };

            EntityCollection results = systemAdminService.RetrieveMultiple(query);

            EntityReference jaydenPhillips = results.Entities[0].ToEntityReference();
            EntityReference benjaminStuart = results.Entities[1].ToEntityReference();
            EntityReference averyHoward = results.Entities[2].ToEntityReference();

            Examples.GrantColumnAccess(
                service: systemAdminService,
                record: jaydenPhillips,
                columnLogicalName: "sample_email",
                principal: appUserReference,
                readAccess: true,
                updateAccess: false);

            Examples.GrantColumnAccess(
                service: systemAdminService,
                record: benjaminStuart,
                columnLogicalName: "sample_governmentid",
                principal: appUserReference,
                readAccess: true,
                updateAccess: false);

            Examples.GrantColumnAccess(
                service: systemAdminService,
                record: averyHoward,
                columnLogicalName: "sample_telephonenumber",
                principal: appUserReference,
                readAccess: true,
                updateAccess: false);


            //Retrieve the data as the application user to show secured behavior
            Console.WriteLine("\r\n\tAfter granting access to selected fields, the application user can see this data:");
            Helpers.ShowExampleRows(Helpers.GetExampleRows(appUserService));

            #endregion Manage read access to secured column

            #region Manage write access to secured column
            Console.WriteLine("\tDemonstrate error when attempting update without update access:");
            // Try to update email column without update access:
            Console.WriteLine("\tTry to update the Email column for the Jayden Phillips record");

            Entity jp = new(Settings.TableLogicalName, jaydenPhillips.Id)
            {
                ["sample_email"] = "jaydenp@adatum.com"
            };

            try
            {
                // This fails
                appUserService.Update(jp);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                if (ex.Detail.ErrorCode.Equals(-2147158777))
                {
                    Console.WriteLine($"\t☑ Expected error:");
                    ConsoleColor originalColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\t{ex.Message}");
                    // Caller user with Id {id} does not have update permissions to a Email secured field on
                    // entity Example table. The requested operation could not be completed.
                    Console.ForegroundColor = originalColor;
                }
                else
                {
                    throw new Exception($"Unexpected Dataverse error updating record: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error updating record: {ex.Message}", ex);
            }

            Console.WriteLine("\r\n\tDemonstrate success when attempting update with update access:");

            Console.WriteLine("\tGrant write access to the email column for Jayden Phillips record:");

            Examples.ModifyColumnAccess(
                service: systemAdminService,
                record: jaydenPhillips,
                columnLogicalName: "sample_email",
                principal: appUserReference,
                readAccess: true,
                updateAccess: true); // Grant update access


            Console.WriteLine("\tTry to update the Email column for the Jayden Phillips record again:");

            try
            {
                // This succeeds
                appUserService.Update(jp);
                Console.WriteLine("\t☑ Successfully updated record.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error updating record: {ex.Message}", ex);
            }



            #endregion Manage write access to secured column

            #region Remove access to fields

            Console.WriteLine("\r\n\tRevoking access to selected fields...");
            // Removing access granted earlier

            Examples.RevokeColumnAccess(
                service: systemAdminService,
                record: jaydenPhillips,
                columnLogicalName: "sample_email",
                principal: appUserReference);

            Examples.RevokeColumnAccess(
                service: systemAdminService,
                record: benjaminStuart,
                columnLogicalName: "sample_governmentid",
                principal: appUserReference);

            Examples.RevokeColumnAccess(
                service: systemAdminService,
                record: averyHoward,
                columnLogicalName: "sample_telephonenumber",
                principal: appUserReference);

            //Retrieve the data as the application user to show return to original behavior.
            Console.WriteLine("\tAfter access to selected fields is revoked, the application user can not see any data.");

            #endregion Remove access to fields

            #region Provide access to specific groups        

            Console.WriteLine($"\tThe {Settings.FieldSecurityProfileName} was created during Setup and the application user was associated with it.");

            Console.WriteLine($"\tAdd field permissions to the {Settings.FieldSecurityProfileName}");

            // Field security profile was created and associated with the App User in Setup.
            // See [#region Create field security profile] and Helpers.ManageFieldSecurityProfileForAppUser
            var fieldSecurityProfileRef = (EntityReference)entityStore[Settings.FieldSecurityProfileName];
            // Define related field permissions
            // You can't set canreadunmasked column values until you create
            // attributemaskingrule records for each column. This will be demonstrated later.
            var fieldPermissionsForCreateList = new List<Entity>() {
                     new("fieldpermission"){
                         ["attributelogicalname"] = "sample_email",
                         ["canread"] = new OptionSetValue(4),  // Allowed
                         ["canupdate"] = new OptionSetValue(4) // Allowed
                     },
                     new("fieldpermission"){
                         ["attributelogicalname"] = "sample_governmentid",
                         ["canread"] = new OptionSetValue(0),  // Not Allowed
                         ["canupdate"] = new OptionSetValue(0) // Not Allowed
                     },
                     new("fieldpermission"){
                         ["attributelogicalname"] = "sample_telephonenumber",
                         ["canread"] = new OptionSetValue(4),  // Allowed
                         ["canupdate"] = new OptionSetValue(4) // Allowed
                     },
                     new("fieldpermission"){
                         ["attributelogicalname"] = "sample_dateofbirth",
                         ["canread"] = new OptionSetValue(4),  // Allowed
                         ["canupdate"] = new OptionSetValue(0) // Not Allowed
                     }
                };

            // These are needed later when configuring masking
            Dictionary<string, Guid> fieldPermissionIds = [];

            fieldPermissionsForCreateList.ForEach(fp =>
            {
                // These are common values for all records in this group:
                fp["fieldsecurityprofileid"] = fieldSecurityProfileRef;
                fp["entityname"] = Settings.TableLogicalName;
                fp["cancreate"] = new OptionSetValue(4); // Allowed
                fp["canreadunmasked"] = new OptionSetValue(0); // Not Allowed
                // CanReadUnMasked must be 0 (Default value) until masking is applied.

                try
                {
                    CreateRequest request = new()
                    {
                        Target = fp
                    };
                    // Add to the solution
                    request["SolutionUniqueName"] = Settings.SolutionUniqueName;

                    var createResponse = (CreateResponse)systemAdminService.Execute(request);
                    Guid fieldPermissionsId = createResponse.id;

                    // Will need these later
                    fieldPermissionIds.Add(
                        key: fp.GetAttributeValue<string>("attributelogicalname"),
                        value: fieldPermissionsId);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error creating field permissions: {ex.Message}", ex);
                }
            });

            Console.WriteLine("\tNew field permissions:");

            Helpers.ShowFieldSecurityProfileSettings();


            #endregion Provide access to specific groups

            #region Show change due to field security profile

            #region Show Read access limited

            // Wait 30 seconds for the cache to catch up
            Helpers.WaitForCacheUpdate();
            Console.WriteLine("\r\n\tThe Government ID column now appears null for all rows.");
            Helpers.ShowExampleRows(Helpers.GetExampleRows(appUserService));

            #endregion Show Read access limited

            #region Show Write access limited

            Console.WriteLine("\tAttempt to update Date of Birth column data without access:");

            jp = new(Settings.TableLogicalName, jaydenPhillips.Id)
            {
                ["sample_dateofbirth"] = "3/25/1977"
            };

            try
            {
                // This fails by design
                appUserService.Update(jp);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                if (ex.Detail.ErrorCode.Equals(-2147158777))
                {
                    Console.WriteLine($"\t☑ Expected error:");
                    ConsoleColor originalColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\t{ex.Message}");
                    // Caller user with Id {id} does not have update permissions to a Date of Birth secured field on
                    // entity Example table. The requested operation could not be completed.
                    Console.ForegroundColor = originalColor;
                }
                else
                {
                    throw new Exception($"Unexpected Dataverse error updating record: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error updating record: {ex.Message}", ex);
            }


            #endregion Show Write access limited

            #endregion Show change due to field security profile

            #region Masking

            Console.WriteLine("\r\n\tDemonstrate how to enable masking");

            #region Create attribute masking rules

            var attributeMaskingRuleList = new List<Entity>();

            // Retrieve ID values for existing masking rules

            // Email_HideName
            Guid? email_HideNameMaskingRuleId = Helpers.GetRecordID(
                service: systemAdminService,
                tableLogicalName: "maskingrule",
                columnLogicalName: "name",
                uniqueStringValue: "Email_HideName");

            if (email_HideNameMaskingRuleId.HasValue)
            {
                attributeMaskingRuleList.Add(new("attributemaskingrule")
                {
                    ["uniquename"] = "sample_example_sample_email",
                    ["attributelogicalname"] = "sample_email",
                    ["maskingruleid"] = new EntityReference(
                        "maskingrule",
                        email_HideNameMaskingRuleId.Value)
                });
            }
            // SocialSecurityNumber_ShowLastFourDigits
            Guid? socialSecurityNumberMaskingRuleId = Helpers.GetRecordID(
                service: systemAdminService,
                tableLogicalName: "maskingrule",
                columnLogicalName: "name",
                uniqueStringValue: "SocialSecurityNumber_ShowLastFourDigits");

            if (socialSecurityNumberMaskingRuleId.HasValue)
            {
                attributeMaskingRuleList.Add(new("attributemaskingrule")
                {
                    ["uniquename"] = "sample_example_sample_governmentid",
                    ["attributelogicalname"] = "sample_governmentid",
                    ["maskingruleid"] = new EntityReference(
                        "maskingrule",
                        socialSecurityNumberMaskingRuleId.Value)
                });
            }

            // Date_Slash
            Guid? dateMaskingRuleId = Helpers.GetRecordID(
                service: systemAdminService,
                tableLogicalName: "maskingrule",
                columnLogicalName: "name",
                uniqueStringValue: "Date_Slash");

            if (dateMaskingRuleId.HasValue)
            {
                attributeMaskingRuleList.Add(new("attributemaskingrule")
                {
                    ["uniquename"] = "sample_example_sample_dateofbirth",
                    ["attributelogicalname"] = "sample_dateofbirth",
                    ["maskingruleid"] = new EntityReference(
                        "maskingrule",
                        dateMaskingRuleId.Value)
                });
            }

            attributeMaskingRuleList.ForEach(amr =>
            {
                amr["entityname"] = Settings.TableLogicalName;

                try
                {
                    CreateRequest request = new()
                    {
                        Target = amr
                    };
                    request["SolutionUniqueName"] = Settings.SolutionUniqueName;

                    systemAdminService.Execute(request);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error creating attribute masking rule: {ex.Message}", ex);
                }
            });

            #endregion Create attribute masking rules

            #region Update fieldpermissions canreadunmasked values

            // These fieldpermission IDs values were cached when they were created earlier

            Guid emailfieldPermissionId = fieldPermissionIds["sample_email"];
            Guid governmentidfieldPermissionId = fieldPermissionIds["sample_governmentid"];
            Guid dateofbirthfieldPermissionId = fieldPermissionIds["sample_dateofbirth"];

            List<Entity> fieldPermissionUpdateList =
            [
                // Allthough these records were created with canread values
                // When canreadunmasked is set, you must also include a canread value!

                new("fieldpermission", emailfieldPermissionId) {
                    ["canreadunmasked"] = new OptionSetValue(3), // All records
                    ["canread"] = new OptionSetValue(4) // Allowed
                },
                new("fieldpermission", governmentidfieldPermissionId) {
                    ["canreadunmasked"] = new OptionSetValue(1), // One record
                    ["canread"] = new OptionSetValue(4) // Allowed
                },
                new("fieldpermission", dateofbirthfieldPermissionId) {
                    ["canreadunmasked"] = new OptionSetValue(3), // All records
                    ["canread"] = new OptionSetValue(4) // Allowed
                },
            ];

            fieldPermissionUpdateList.ForEach(fieldPermissionUpdate =>
            {
                UpdateRequest request = new()
                {
                    Target = fieldPermissionUpdate,
                    ["SolutionUniqueName"] = Settings.SolutionUniqueName,
                };

                try
                {
                    systemAdminService.Execute(request);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error updating field permissions: {ex.Message}", ex);
                }

            });

            Console.WriteLine("\r\n\tThis tables shows the fieldpermission changes made to enable displaying masked data.");

            Helpers.ShowUpdatedFieldSecurityProfileSettings();

            #endregion Update fieldpermissions canreadunmasked values

            // Wait 30 seconds for the cache to catch up
            Helpers.WaitForCacheUpdate();

            Console.WriteLine("\r\n\tNow masked values are returned:");
            Helpers.ShowExampleRows(Helpers.GetExampleRows(appUserService));

            #endregion Masking

            #region Show unmasked values

            Console.WriteLine("\tThe unmasked values for Email and Date of Birth can be retrieved for all records");

            // This GetUnmaskedExampleRows example uses 'UnMaskedData' as an optional parameter
            // https://learn.microsoft.com/power-apps/developer/data-platform/optional-parameters
            Helpers.ShowExampleRows(Examples.GetUnmaskedExampleRows(appUserService));

            Console.WriteLine("\tThe unmasked values for Government ID can only be retrieved individually:");

            QueryExpression exampleQuery = new()
            {
                ColumnSet = new ColumnSet("sample_exampleid"),
                Criteria = new FilterExpression()
            };

            EntityCollection exampleCollection = appUserService.RetrieveMultiple(query);

            foreach (var sample_example in exampleCollection.Entities)
            {
                RetrieveRequest request = new()
                {
                    ColumnSet = new ColumnSet("sample_name", "sample_governmentid"),
                    Target = new EntityReference(Settings.TableLogicalName, sample_example.Id),
                    ["UnMaskedData"] = true
                };

                try
                {
                    var response = (RetrieveResponse)appUserService.Execute(request);
                    Entity record = response.Entity;
                    string name = record.GetAttributeValue<string>("sample_name");
                    string governmentid = record.GetAttributeValue<string>("sample_governmentid");
                    Console.WriteLine($"\t\t-Name: {name}\tGovernment ID: {governmentid}");
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error retrieving unmasked Government ID values: {ex.Message}", ex);
                }
            }

            #endregion Show unmasked value

            #region Export solution

            #region Export unmanaged
            Console.WriteLine($"\r\n\tExporting unmanaged solution...");

            ExportSolutionRequest exportUnmanagedSolutionRequest = new()
            {
                SolutionName = Settings.SolutionUniqueName,
                Managed = false
            };

            var exportUnmanagedSolutionResponse = (ExportSolutionResponse)systemAdminService.Execute(exportUnmanagedSolutionRequest);

            string filename = $"{Settings.SolutionUniqueName}.zip";

            string filePath = Path.Combine(Environment.CurrentDirectory, filename);

            Console.WriteLine($"\t☑ Exported unmanaged solution to this file:");
            Console.WriteLine($"\t\\bin\\Debug\\net8.0\\{filename}\r\n");

            // Save solution
            using (var fs = File.Create(filePath))
            {
                fs.Write(
                    exportUnmanagedSolutionResponse.ExportSolutionFile,
                    0,
                    exportUnmanagedSolutionResponse.ExportSolutionFile.Length);
            }

            #endregion Export unmanaged


            #region Export managed 

            Console.WriteLine($"\r\n\tExporting managed solution...");

            ExportSolutionRequest exportManagedSolutionRequest = new()
            {
                SolutionName = Settings.SolutionUniqueName,
                Managed = true
            };

            var exportManagedSolutionResponse = (ExportSolutionResponse)systemAdminService.Execute(exportManagedSolutionRequest);

            filename = $"{Settings.SolutionUniqueName}_managed.zip";

            filePath = Path.Combine(Environment.CurrentDirectory, filename);

            Console.WriteLine($"\t☑ Exported managed solution to this file:");
            Console.WriteLine($"\t\\bin\\Debug\\net8.0\\{filename}\r\n");

            // Save solution
            using (var fs = File.Create(filePath))
            {
                fs.Write(
                    exportManagedSolutionResponse.ExportSolutionFile,
                    0,
                    exportManagedSolutionResponse.ExportSolutionFile.Length);
            }

            #endregion Export managed 

            #endregion Export solution
        }

        private static void Cleanup(IOrganizationService service,
            Dictionary<string, object> entityStore)
        {
            if (Settings.DeleteCreatedObjects)
            {
                if (entityStore.TryGetValue(Settings.RoleName, out object? roleReferenceValue))
                {
                    var roleReference = (EntityReference)roleReferenceValue;
                    service.Delete(roleReference.LogicalName, roleReference.Id);
                    Console.WriteLine($"\t☑ {Settings.RoleName} deleted.");
                }

                if (entityStore.TryGetValue(Settings.FieldSecurityProfileName, out object? profileReferenceValue))
                {
                    var columnSecurityProfileReference = (EntityReference)profileReferenceValue;
                    service.Delete(columnSecurityProfileReference.LogicalName, columnSecurityProfileReference.Id);
                    Console.WriteLine($"\t☑ {Settings.FieldSecurityProfileName} deleted.");
                }

                if (entityStore.ContainsKey(Settings.TableSchemaName))
                {
                    DeleteEntityRequest request = new()
                    {
                        LogicalName = Settings.TableLogicalName
                    };

                    try
                    {
                        service.Execute(request);
                        Console.WriteLine($"\t☑ {Settings.TableSchemaName} table deleted.");
                        // Columns and data in them is deleted with the table.
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error deleting sample table: {ex.Message}", ex);
                    }
                }

                if (entityStore.TryGetValue(Settings.SolutionUniqueName, out object? solutionReferenceValue))
                {
                    var solutionReference = (EntityReference)solutionReferenceValue;
                    service.Delete(solutionReference.LogicalName, solutionReference.Id);
                    Console.WriteLine($"\t☑ {Settings.SolutionUniqueName} deleted.");
                }

                if (entityStore.TryGetValue(Settings.PublisherUniqueName, out object? publisherReferenceValue))
                {
                    var publisherReference = (EntityReference)publisherReferenceValue;
                    service.Delete(publisherReference.LogicalName, publisherReference.Id);
                    Console.WriteLine($"\t☑ {Settings.PublisherUniqueName} deleted.");
                }
            }
        }
    }
}
