using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;
using System.Threading.Tasks;

namespace PowerPlatform_Dataverse_CodeSamples
{
    internal static class Helpers
    {

        /// <summary>
        /// Creates sample string columns in a specified table within a Microsoft Dataverse environment.
        /// </summary>
        /// <remarks>This method checks whether each column already exists in the specified table. If a
        /// column exists, it logs a message indicating that the column is already present. If a column does not exist,
        /// it creates the column using the provided settings and associates it with the specified solution.</remarks>
        /// <param name="service">The <see cref="IOrganizationService"/> instance used to interact with the Dataverse environment.</param>
        /// <param name="tableLogicalName">The logical name of the table where the columns will be created. This value cannot be null or empty.</param>
        /// <param name="solutionUniqueName">The unique name of the solution to associate the created columns with. This value cannot be null or empty.</param>
        /// <param name="customizationPrefix">The customization prefix to use for the schema names of the columns. This value cannot be null or empty.</param>
        /// <param name="columnSettings">An array of <see cref="ColumnSettings"/> objects that define the configuration for each column to be
        /// created. Each object specifies the column's name, display name, and description.</param>
        internal static void CreateSampleColumns(
            IOrganizationService service,
            string tableLogicalName,
            string solutionUniqueName,
            string customizationPrefix,
            ColumnSettings[] columnSettings)
        {
            foreach (var column in columnSettings)
            {
                string schemaName = $"{customizationPrefix}_{column.Name}";
                string logicalName = schemaName.ToLower();

                try
                {                    

                    Guid? columnId = Helpers.GetColumnID(service,
                                    tableLogicalName: tableLogicalName,
                                    columnLogicalName: logicalName);

                    if (columnId.HasValue)
                    {
                        Console.WriteLine($"\t☑ Column {schemaName} already exists.");
                    }
                    else
                    {
                        Console.WriteLine($"\tCreating {schemaName} column...");

                        CreateSampleColumn(
                           service,
                           tableLogicalName,
                           schemaName,
                           column.DisplayName,
                           column.Description,
                           solutionUniqueName);
                        Console.WriteLine($"\t☑ Created {schemaName} column");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\t❌ Error creating {schemaName} column : {ex.Message}");
                }
            }
        }


        /// <summary>
        /// Creates a sample column in the specified table.
        /// </summary>
        /// <param name="service">The authenticated <see cref="IOrganizationService"/> instance used to execute the request.</param>
        /// <param name="tableLogicalName">Logical name of the table</param>
        /// <param name="schemaName">Schema name for the column</param>
        /// <param name="displayName">Display name for the column</param>
        /// <param name="description">Description of the column</param>
        /// <param name="solutionUniqueName">Unique name of the solution</param>
        /// <returns>GUID of the created column</returns>
        /// <exception cref="Exception">Throws exception if column creation fails</exception>
        internal static Guid CreateSampleColumn(
            IOrganizationService service,
            string tableLogicalName,
            string schemaName,
            string displayName,
            string description,
            string solutionUniqueName)
        {
            StringAttributeMetadata column = new()
            {
                SchemaName = schemaName,
                RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                MaxLength = 100,
                FormatName = StringFormatName.Text,
                DisplayName = new Label(displayName, 1033),
                Description = new Label(description, 1033)
            };

            CreateAttributeRequest request = new()
            {
                Attribute = column,
                EntityName = tableLogicalName,
                SolutionUniqueName = solutionUniqueName
            };


            try
            {
                var response = (CreateAttributeResponse)service.Execute(request);


                return response.AttributeId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating sample column: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates a custom table in Dataverse with the specified schema name, solution, and customization prefix.
        /// </summary>
        /// <remarks>This method creates a user-owned table with a primary attribute named using the
        /// provided customization prefix. The table is associated with the specified solution and includes metadata
        /// such as display names and descriptions.</remarks>
        /// <param name="service">The authenticated <see cref="IOrganizationService"/> instance used to execute the request.</param>
        /// <param name="schemaName">The schema name of the table to be created. This must be unique within the Dataverse environment.</param>
        /// <param name="solutionUniqueName">The unique name of the solution to associate the table with.</param>
        /// <param name="customizationPrefix">The prefix to use for customizing the schema name of the table's primary attribute.</param>
        /// <returns>A <see cref="Guid"/> representing the unique identifier of the created table.</returns>
        /// <exception cref="Exception">Thrown if an error occurs while creating the table. The exception message includes details about the
        /// failure.</exception>
        internal static Guid CreateSampleTable(
            IOrganizationService service,
            string schemaName,
            string solutionUniqueName,
            string customizationPrefix)
        {
            // Define the entity metadata
            EntityMetadata entityMetadata = new()
            {
                SchemaName = schemaName,
                DisplayName = new Label("Example table", 1033),
                DisplayCollectionName = new Label("Example table records", 1033),
                Description = new Label("A custom table created for the column-level security sample code", 1033),
                OwnershipType = OwnershipTypes.UserOwned,
                IsActivity = false
            };

            // Define the primary attribute for the entity
            StringAttributeMetadata primaryAttribute = new()
            {
                SchemaName = customizationPrefix + "_Name",
                RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                MaxLength = 100,
                FormatName = StringFormatName.Text,
                DisplayName = new Label("Name", 1033),
                Description = new Label("The primary attribute for the sample_Example table", 1033)
            };

            // Create the entity request
            CreateEntityRequest request = new()
            {
                Entity = entityMetadata,
                PrimaryAttribute = primaryAttribute,
                ["SolutionUniqueName"] = solutionUniqueName
            };

            try
            {
                var response = (CreateEntityResponse)service.Execute(request);
                return response.EntityId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating sample table: {ex.Message}", ex);
            }
        }


        /// <summary>
        /// Retrieves the unique identifier (ID) of a column in a specified Dataverse table.
        /// </summary>
        /// <remarks>This method uses the Dataverse service to retrieve metadata about a column in a
        /// table. If the column does not exist, the method returns <see langword="null"/>. Ensure that the <paramref
        /// name="tableLogicalName"/> and <paramref name="columnLogicalName"/> parameters are valid and correspond to
        /// existing entities in Dataverse.</remarks>
        /// <param name="service">The <see cref="IOrganizationService"/> instance used to execute the request.</param>
        /// <param name="tableLogicalName">The logical name of the table containing the column. This value cannot be null or empty.</param>
        /// <param name="columnLogicalName">The logical name of the column whose ID is to be retrieved. This value cannot be null or empty.</param>
        /// <returns>A <see cref="Guid"/> representing the unique identifier of the column, or <see langword="null"/> if the
        /// column does not exist.</returns>
        /// <exception cref="Exception">Thrown if an unexpected error occurs during the operation.</exception>
        internal static Guid? GetColumnID(IOrganizationService service, string tableLogicalName, string columnLogicalName)
        {

            RetrieveAttributeRequest request = new()
            {
                EntityLogicalName = tableLogicalName,
                LogicalName = columnLogicalName,

            };

            try
            {
                var response = (RetrieveAttributeResponse)service.Execute(request);
                return response.AttributeMetadata.MetadataId;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {

                if (ex.Detail.ErrorCode.Equals(-2147220969))
                {
                    // The column doesn't exist
                    return null;
                }
                throw new Exception($"Unexpected Dataverse error retrieving data about sample table {columnLogicalName} column: {ex.Message}", ex);

            }
            catch (Exception e)
            {
                throw new Exception($"Unexpected error retrieving data about sample table {columnLogicalName} column: {e.Message}", e);
            }

        }

        /// <summary>
        /// Retrieves the unique identifier (ID) of a table in Dataverse based on its logical name.
        /// </summary>
        /// <remarks>This method uses the Dataverse <see cref="RetrieveEntityRequest"/> to fetch metadata
        /// about the specified table. If the table does not exist, the method returns <see langword="null"/>. If an
        /// unexpected error occurs, an exception is thrown.</remarks>
        /// <param name="service">The <see cref="IOrganizationService"/> instance used to execute the request.</param>
        /// <param name="tableLogicalName">The logical name of the table whose ID is to be retrieved. This value cannot be null or empty.</param>
        /// <returns>A <see cref="Guid"/> representing the unique identifier of the table if it exists; otherwise, <see
        /// langword="null"/>.</returns>
        /// <exception cref="Exception">Thrown if an unexpected error occurs while retrieving the table metadata.</exception>
        internal static Guid? GetTableID(IOrganizationService service, string tableLogicalName)
        {

            RetrieveEntityRequest request = new()
            {
                LogicalName = tableLogicalName,
                EntityFilters = EntityFilters.Entity
            };

            try
            {

                var response = (RetrieveEntityResponse)service.Execute(request);
                return response.EntityMetadata.MetadataId;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {

                if (ex.Detail.ErrorCode.Equals(-2147220969))
                {
                    // The table doesn't exist
                    return null;
                }
                throw new Exception($"Unexpected Dataverse error retrieving data about sample table: {ex.Message}", ex);

            }
            catch (Exception e)
            {
                throw new Exception($"Unexpected error retrieving data about sample table: {e.Message}", e);
            }

        }

        /// <summary>
        /// Retrieves the unique identifier (ID) of a record in a specified table based on a column value.
        /// </summary>
        /// <remarks>This method performs a query using the specified table and column to locate a record
        /// with a matching value. If multiple records match the criteria, only the ID of the first record is
        /// returned.</remarks>
        /// <param name="service">The <see cref="IOrganizationService"/> instance used to interact with the data source.</param>
        /// <param name="tableLogicalName">The logical name of the table (entity) to query. This value cannot be null or empty.</param>
        /// <param name="columnLogicalName">The logical name of the column to filter by. This value cannot be null or empty.</param>
        /// <param name="uniqueStringValue">The unique string value to match against the specified column. This value cannot be null or empty.</param>
        /// <returns>A <see cref="Guid"/> representing the ID of the first matching record, or <see langword="null"/> if no
        /// matching record is found.</returns>
        internal static Guid? GetRecordID(
            IOrganizationService service,
            string tableLogicalName,
            string columnLogicalName,
            string uniqueStringValue)
        {

            QueryExpression query = new(tableLogicalName)
            {
                ColumnSet = new ColumnSet(columnLogicalName),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions = {
                         new ConditionExpression(
                             columnLogicalName,
                             ConditionOperator.Equal,
                             uniqueStringValue)
                     }
                }
            };

            EntityCollection results = service.RetrieveMultiple(query);
            if (results.Entities.Any())
            {
                return results.Entities[0].Id;
            }

            return null;
        }




        /// <summary>
        /// Manages the association of a security role with an application user in the system.
        /// </summary>
        /// <remarks>This method checks whether the specified application user is already associated with
        /// the given security role. If the association does not exist, the method creates it. The operation is logged
        /// to the console.</remarks>
        /// <param name="systemAdminService">The <see cref="IOrganizationService"/> instance with administrative privileges used to execute the
        /// operation.</param>
        /// <param name="securityRoleId">The unique identifier of the security role to be managed.</param>
        /// <param name="appUserId">The unique identifier of the application user to associate with the security role.</param>
        /// <param name="roleName">The name of the security role, used for logging purposes.</param>
        internal static void ManageRoleForAppUser(IOrganizationService systemAdminService,
            Guid securityRoleId,
            Guid appUserId,
            string roleName)
        {
            var relationshipQueryCollection = new RelationshipQueryCollection();

            var relatedRolesQuery = new QueryExpression("role")
            {
                ColumnSet = new ColumnSet("roleid"),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions = {
                        { new ConditionExpression("roleid", ConditionOperator.Equal, securityRoleId)   }
                    }
                }
            };
            relationshipQueryCollection.Add(new Relationship("systemuserroles_association"), relatedRolesQuery);

            RetrieveRequest retrieveRequest = new()
            {
                ColumnSet = new ColumnSet("systemuserid"),
                Target = new EntityReference("systemuser", appUserId),
                RelatedEntitiesQuery = relationshipQueryCollection
            };

            RetrieveResponse retrieveResponse = (RetrieveResponse)systemAdminService.Execute(retrieveRequest);
            var relatedRoles = retrieveResponse.Entity.RelatedEntities[new Relationship("systemuserroles_association")];
            if (relatedRoles.Entities.Count > 0)
            {
                // The application user already has the role
                Console.WriteLine($"\t☑ Application user already has {roleName}.");
            }
            else
            {
                Console.WriteLine($"\tAssociating {roleName} to application user...");

                // Associate the application user with the role
                AssociateRequest associateRequest = new()
                {
                    Target = new EntityReference("systemuser", appUserId),
                    RelatedEntities =
                        [
                            new EntityReference("role", securityRoleId)
                        ],
                    Relationship = new Relationship("systemuserroles_association")
                };

                systemAdminService.Execute(associateRequest);

                Console.WriteLine($"\t☑ Associated {roleName} to application user.");

            }
        }


        /// <summary>
        /// Manages the association between a field security profile and an application user.
        /// </summary>
        /// <remarks>This method checks whether the specified application user is already associated with
        /// the given field security profile. If the association does not exist, it creates the association.</remarks>
        /// <param name="systemAdminService">The <see cref="IOrganizationService"/> instance with system administrator privileges, used to execute the
        /// operation.</param>
        /// <param name="fieldSecurityProfileId">The unique identifier of the field security profile to associate with the application user.</param>
        /// <param name="appUserId">The unique identifier of the application user to associate with the field security profile.</param>
        /// <param name="fieldSecurityProfileName">The name of the field security profile, used for logging purposes.</param>
        internal static void ManageFieldSecurityProfileForAppUser(IOrganizationService systemAdminService,
                Guid fieldSecurityProfileId,
                Guid appUserId,
                string fieldSecurityProfileName)
        {


            // Create a query to return the related field security profile
            var relationshipQueryCollection = new RelationshipQueryCollection();

            var relatedFieldSecurityProfilesQuery = new QueryExpression("fieldsecurityprofile")
            {
                ColumnSet = new ColumnSet("fieldsecurityprofileid"),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions = {
                        {
                            new ConditionExpression(
                               attributeName: "fieldsecurityprofileid",
                               conditionOperator: ConditionOperator.Equal,
                               value: fieldSecurityProfileId)
                        }
                    }
                }
            };
            relationshipQueryCollection.Add(
                key: new Relationship("systemuserprofiles_association"),
                value: relatedFieldSecurityProfilesQuery);

            RetrieveRequest retrieveRequest = new()
            {
                ColumnSet = new ColumnSet("systemuserid"),
                Target = new EntityReference("systemuser", appUserId),
                RelatedEntitiesQuery = relationshipQueryCollection
            };
            // Retrieve the application user with related field security profile
            RetrieveResponse retrieveResponse = (RetrieveResponse)systemAdminService.Execute(retrieveRequest);
            var relatedFieldSecurityProfiles = retrieveResponse.Entity.RelatedEntities[new Relationship("systemuserprofiles_association")];
            if (relatedFieldSecurityProfiles.Entities.Count > 0)
            {
                // The application user is already associated.
                Console.WriteLine($"\t☑ Application user is already associated with the {fieldSecurityProfileName}.");
            }
            else
            {
                // Associate the application user with the field security profile
                AssociateRequest associateRequest = new()
                {
                    Target = new EntityReference("systemuser", appUserId),
                    RelatedEntities =
                        [
                            new EntityReference("fieldsecurityprofile", fieldSecurityProfileId)
                        ],
                    Relationship = new Relationship("systemuserprofiles_association")
                };

                systemAdminService.Execute(associateRequest);

                Console.WriteLine($"\t☑ Associated {fieldSecurityProfileName} to application user.");
            }
        }

        /// <summary>
        /// Verifies that the specified application user has all required row-level privileges  for records in the
        /// "sample_example" entity.
        /// </summary>
        /// <remarks>This method checks whether the application user has all necessary access rights, 
        /// including AppendTo, Append, Create, Read, Share, Delete, Assign, and Write, for  each record in the
        /// "sample_example" entity. If the user lacks any of these privileges,  an exception is thrown.</remarks>
        /// <param name="systemAdminService">The <see cref="IOrganizationService"/> instance with system administrator privileges,  used to query and
        /// verify access rights.</param>
        /// <param name="appUserId">The unique identifier of the application user whose privileges are being verified.</param>
        /// <exception cref="Exception">Thrown if the application user does not have all required privileges for any of the  records in the
        /// "sample_example" entity.</exception>
        internal static void VerifyAppUserRowLevelPrivileges(IOrganizationService systemAdminService, Guid appUserId)
        {
            QueryExpression accessQuery = new("sample_example")
            {
                ColumnSet = new ColumnSet("sample_name")
            };
            EntityCollection accessResults = systemAdminService.RetrieveMultiple(accessQuery);
            if (accessResults.Entities.Count > 0)
            {
                foreach (var foundRecord in accessResults.Entities)
                {
                    RetrievePrincipalAccessRequest request = new()
                    {
                        Target = new EntityReference("sample_example", foundRecord.Id),
                        Principal = new EntityReference("systemuser", appUserId)
                    };

                    RetrievePrincipalAccessResponse response =
                        (RetrievePrincipalAccessResponse)systemAdminService.Execute(request);

                    AccessRights appUserRights = response.AccessRights;

                    if (
                        appUserRights.HasFlag(AccessRights.AppendToAccess) &&
                        appUserRights.HasFlag(AccessRights.AppendAccess) &&
                        appUserRights.HasFlag(AccessRights.CreateAccess) &&
                        appUserRights.HasFlag(AccessRights.ReadAccess) &&
                        appUserRights.HasFlag(AccessRights.ShareAccess) &&
                        appUserRights.HasFlag(AccessRights.DeleteAccess) &&
                        appUserRights.HasFlag(AccessRights.AssignAccess) &&
                        appUserRights.HasFlag(AccessRights.WriteAccess)
                        )
                    {
                        // Do nothing. The application user has all privileges.
                    }
                    else
                    {
                        throw new Exception($"\tApp user does not have all privileges for the records.");
                    }
                }
                // App user has all privileges for the record
                Console.WriteLine($"\t☑ Verified that the application user " +
                    $"has all privileges for the {accessResults.Entities.Count} " +
                    $"sample records created.");
            }
        }

        /// <summary>
        /// Retrieves a collection of sample records from the organization service.
        /// </summary>
        /// <remarks>This method queries the "sample_example" entity and retrieves specific columns,
        /// including "sample_name", "sample_email", "sample_governmentid", "sample_telephonenumber", and
        /// "sample_dateofbirth". The results are ordered by "sample_name" in descending order.</remarks>
        /// <param name="service">The <see cref="IOrganizationService"/> instance used to execute the query.</param>
        /// <returns>An <see cref="EntityCollection"/> containing the retrieved sample records.</returns>
        /// <exception cref="Exception">Thrown if an error occurs while retrieving the records. The exception message includes details about the
        /// failure.</exception>
        internal static EntityCollection GetExampleRows(IOrganizationService service)
        {
            QueryExpression query = new("sample_example")
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

            try
            {
                return service.RetrieveMultiple(query);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sample records: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Displays a formatted table of example rows in the console, based on the provided entity collection.
        /// </summary>
        /// <remarks>This method formats the data into a table with predefined column headers and outputs
        /// it to the console. The table does not include a row count and is indented for improved
        /// readability.</remarks>
        /// <param name="results">A collection of entities containing attributes such as name, email, government ID, telephone number,  and
        /// date of birth. Each entity is expected to have these attributes with corresponding keys.</param>
        internal static void ShowExampleRows(EntityCollection results)
        {
            var table = new ConsoleTables.ConsoleTable(
                "Name",
                "Email",
                "Government ID",
                "Telephone Number",
                "Date of Birth");

            // Don't need to the count.
            table.Options.EnableCount = false;

            foreach (Entity result in results.Entities)
            {
                string name = result.GetAttributeValue<string>("sample_name");
                string email = result.GetAttributeValue<string>("sample_email");
                string id = result.GetAttributeValue<string>("sample_governmentid");
                string phoneNumber = result.GetAttributeValue<string>("sample_telephonenumber");
                string dob = result.GetAttributeValue<string>("sample_dateofbirth");

                table.Rows.Add([name, email, id, phoneNumber, dob]);
            }

            // Manually format the table output with indentation
            var tableOutput = table.ToString();
            var indentedTableOutput = "\t" + tableOutput.Replace("\n", "\n\t");
            Console.WriteLine(indentedTableOutput);

        }

        /// <summary>
        /// Displays the field security profile settings in a formatted table.
        /// </summary>
        /// <remarks>The table includes columns for field names, read permissions, and update permissions.
        /// Each row represents a specific field and its associated permissions. The output is indented for better
        /// readability in the console.</remarks>
        internal static void ShowFieldSecurityProfileSettings()
        {
            var permissionsTable = new ConsoleTables.ConsoleTable(
                            "Column",
                            "Can Read",
                            "Can Update");

            // Don't need to the count.
            permissionsTable.Options.EnableCount = false;

            permissionsTable.Rows.Add(["Email", "Allowed", "Allowed"]);
            permissionsTable.Rows.Add(["Government ID", "Not Allowed", "Not Allowed"]);
            permissionsTable.Rows.Add(["Telephone Number", "Allowed", "Allowed"]);
            permissionsTable.Rows.Add(["Date of Birth", "Allowed", "Not Allowed"]);

            // Write intented table:
            var tableOutput = permissionsTable.ToString();
            var indentedTableOutput = "\t" + tableOutput.Replace("\n", "\n\t");
            Console.WriteLine(indentedTableOutput);
        }

        /// <summary>
        /// Displays a formatted table of field security profile settings, including permissions for reading,  reading
        /// unmasked data, and updating specific fields.
        /// </summary>
        /// <remarks>This method outputs a table to the console, showing the permissions for various
        /// fields such as  "Email", "Government ID", "Telephone Number", and "Date of Birth". Each row in the table
        /// indicates  whether the field can be read, read unmasked, or updated, along with specific conditions for
        /// unmasked  data access.</remarks>
        internal static void ShowUpdatedFieldSecurityProfileSettings()
        {
            var permissionsTable = new ConsoleTables.ConsoleTable(
                            "Column",
                            "Can Read",
                            "Can Read Unmasked",
                            "Can Update");

            // Don't need to the count.
            permissionsTable.Options.EnableCount = false;

            permissionsTable.Rows.Add(["Email", "Allowed", "All records", "Allowed"]);
            permissionsTable.Rows.Add(["Government ID", "Allowed", "One record", "Not Allowed"]);
            permissionsTable.Rows.Add(["Telephone Number", "Allowed", "Not Allowed", "Allowed"]);
            permissionsTable.Rows.Add(["Date of Birth", "Allowed", "All records", "Not Allowed"]);

            // Write intented table:
            var tableOutput = permissionsTable.ToString();
            var indentedTableOutput = "\t" + tableOutput.Replace("\n", "\n\t");
            Console.WriteLine(indentedTableOutput);
        }

        /// <summary>
        /// Waits synchronously for a specified number of seconds, displaying a countdown in the console to indicate cache update progress.
        /// </summary>
        /// <param name="waitSeconds">The number of seconds to wait. Defaults to 30 seconds.</param>
        internal static void WaitForCacheUpdate(int waitSeconds = 30)
        {
            Console.WriteLine("");
            for (int i = waitSeconds; i >= 0; i--)
            {
                Console.Write($"\rWaiting {i} seconds for the cache to update...");
                Task.Delay(1000).Wait();
            }
            Console.Write($"\rWaited {waitSeconds} seconds for the cache to update.");
            Console.WriteLine("");
        }

    }

    /// <summary>
    /// Represents the settings for a column, including its name, display name, and description.
    /// </summary>
    /// <remarks>This struct is used to define metadata for a column, such as its internal name, the name
    /// displayed to users, and a description providing additional context or information about the column.</remarks>
    internal struct ColumnSettings
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
    }


}

