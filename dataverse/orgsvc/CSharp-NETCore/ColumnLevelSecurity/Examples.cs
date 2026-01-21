using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;
using System.Text;

namespace PowerPlatform_Dataverse_CodeSamples
{
    internal static class Examples
    {
        // <snippetGetSecuredColumns>
        /// <summary>
        /// Generates a CSV file containing the names of secured columns for all tables 
        /// in the organization.
        /// </summary>
        /// <remarks>This method queries the organization's metadata to identify columns 
        /// marked as secured (i.e., columns with the <c>IsSecured</c> property set to 
        /// <see langword="true"/>). The resulting CSV file contains two columns: "Table" 
        /// and "Column", representing the schema names of the tables and their secured 
        /// columns, respectively. <para> Ensure that the provided 
        /// <paramref name="filepath"/> is writable and that the user has appropriate 
        /// permissions to access the specified directory. </para></remarks>
        /// <param name="service">The <see cref="IOrganizationService"/> instance used to 
        /// retrieve metadata from the organization.</param>
        /// <param name="filepath">The directory path where the CSV file will be saved. 
        /// Must be a valid and accessible file path.</param>
        /// <param name="filename">The name of the CSV file to be created. Defaults to 
        /// "SecuredColumns.csv" if not specified.</param>
        static internal void GetSecuredColumns(IOrganizationService service,
            string filepath, string filename = "SecuredColumns.csv")
        {
            EntityQueryExpression query = new()
            {
                Properties = new MetadataPropertiesExpression(
                    "SchemaName",
                    "Attributes"),
                Criteria = new MetadataFilterExpression(),
                AttributeQuery = new()
                {
                    Properties = new MetadataPropertiesExpression(
                        "SchemaName",
                        "AttributeTypeName"),
                    Criteria = new MetadataFilterExpression()
                    {
                        Conditions = {
                            {
                                new MetadataConditionExpression(
                                    "IsSecured",
                                    MetadataConditionOperator.Equals,
                                    true)
                            }
                        }
                    }
                }
            };

            RetrieveMetadataChangesRequest request = new()
            {
                Query = query
            };

            var response = (RetrieveMetadataChangesResponse)service.Execute(request);


            // Create a StringBuilder to hold the CSV data
            StringBuilder csvContent = new();

            string[] columns = {
                "Table",
                "Column" };

            // Add headers
            csvContent.AppendLine(string.Join(",", columns));

            foreach (var table in response.EntityMetadata)
            {
                foreach (var column in table.Attributes)
                {
                    string[] values = {
                        table.SchemaName,
                        column.SchemaName
                    };

                    // Add values
                    csvContent.AppendLine(string.Join(",", values));
                }
            }

            File.WriteAllText(
                Path.Combine(filepath, filename),
                csvContent.ToString());
        }
        // </snippetGetSecuredColumns>
        // <snippetDumpColumnSecurityInfo>
        /// <summary>
        /// Exports column security information for all entities in the organization to a 
        /// CSV file.
        /// </summary>
        /// <remarks>This method retrieves metadata about entity attributes, including 
        /// security-related properties, and writes the information to a CSV file. The output 
        /// file contains details such as whether columns are secured, can be secured for 
        /// create, update, or read operations, and other relevant metadata.</remarks>
        /// <param name="service">The <see cref="IOrganizationService"/> instance used to 
        /// retrieve metadata from the organization.</param>
        /// <param name="filepath">The directory path where the CSV file will be saved. This 
        /// must be a valid, writable directory.</param>
        /// <param name="filename">The name of the CSV file to create. Defaults to 
        /// "ColumnSecurityInfo.csv" if not specified.</param>
        static internal void DumpColumnSecurityInfo(IOrganizationService service,
            string filepath, string filename = "ColumnSecurityInfo.csv")
        {
            EntityQueryExpression query = new()
            {
                Properties = new MetadataPropertiesExpression("SchemaName", "Attributes"),
                Criteria = new MetadataFilterExpression
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions =
                     {
                         new MetadataConditionExpression(
                             "IsPrivate",
                             MetadataConditionOperator.Equals,
                             false),
                     }
                },
                AttributeQuery = new()
                {
                    Properties = new MetadataPropertiesExpression(
                        "SchemaName",
                        "AttributeTypeName",
                        "IsPrimaryName",
                        "IsSecured",
                        "CanBeSecuredForCreate",
                        "CanBeSecuredForUpdate",
                        "CanBeSecuredForRead"),
                    Criteria = new MetadataFilterExpression()
                    {
                        Conditions = {
                            { // Exclude Virtual columns
                                new MetadataConditionExpression(
                                "AttributeTypeName",
                                MetadataConditionOperator.NotEquals,
                                AttributeTypeDisplayName.VirtualType)
                            }
                        }
                    }
                }
            };

            RetrieveMetadataChangesRequest request = new()
            {
                Query = query
            };

            var response = (RetrieveMetadataChangesResponse)service.Execute(request);


            // Create a StringBuilder to hold the CSV data
            StringBuilder csvContent = new();

            string[] columns = {
                "Column",
                "Type",
                "IsPrimaryName",
                "IsSecured",
                "CanBeSecuredForCreate",
                "CanBeSecuredForUpdate",
                "CanBeSecuredForRead" };

            // Add headers
            csvContent.AppendLine(string.Join(",", columns));

            foreach (var table in response.EntityMetadata)
            {
                foreach (AttributeMetadata column in table.Attributes)
                {
                    string[] values = {
                        $"{table.SchemaName}.{column.SchemaName}",
                        column.AttributeTypeName.Value,
                        column.IsPrimaryName?.ToString() ?? "False",
                        column.IsSecured?.ToString() ?? "False",
                        column.CanBeSecuredForCreate?.ToString() ?? "False",
                        column.CanBeSecuredForUpdate.ToString() ?? "False",
                        column.CanBeSecuredForRead.ToString() ?? "False"
                    };

                    // Add values
                    csvContent.AppendLine(string.Join(",", values));
                }
            }

            File.WriteAllText(
                Path.Combine(filepath, filename),
                csvContent.ToString());
        }
       // </snippetDumpColumnSecurityInfo>
       // <snippetGetSecuredColumnList>
       /// <summary>
       /// Retrieves a list of secured columns managed by the specified field security 
       /// profile.
       /// </summary>
       /// <remarks>This method queries the Dataverse field permission table to identify 
       /// columns that are secured by the field security profile with ID 
       /// <c>572329c1-a042-4e22-be47-367c6374ea45</c>. The returned list contains fully 
       /// qualified column names in the format <c>TableName.ColumnName</c>, sorted 
       /// alphabetically.</remarks>
       /// <param name="service">The <see cref="IOrganizationService"/> instance used to 
       /// interact with the Dataverse service.</param>
       /// <returns>A sorted list of strings representing the fully qualified names of 
       /// secured columns.</returns>
       /// <exception cref="Exception">Thrown if the calling user does not have read 
       /// access to the field permission table or if an error occurs while retrieving 
       /// field permissions.</exception>
        static internal List<string> GetSecuredColumnList(IOrganizationService service)
        {
            QueryExpression query = new("fieldpermission")
            {
                ColumnSet = new ColumnSet("entityname", "attributelogicalname"),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                    {
                      // Field security profile with ID '572329c1-a042-4e22-be47-367c6374ea45' 
                      // manages access for system administrators. It always contains
                      // references to each secured column

                        new ConditionExpression("fieldsecurityprofileid", ConditionOperator.Equal,
                            new Guid("572329c1-a042-4e22-be47-367c6374ea45"))
                    }
                }
            };

            EntityCollection fieldPermissions;

            try
            {
                fieldPermissions = service.RetrieveMultiple(query);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {

                if (ex.Detail.ErrorCode.Equals(-2147220960))
                {
                    string message = "The calling user doesn't have read access to the fieldpermission table";

                    throw new Exception(message);
                }

                else
                {
                    throw new Exception($"Dataverse error retrieving field permissions: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving field permissions: {ex.Message}", ex);
            }

            List<string> values = [];
            foreach (var fieldpermission in fieldPermissions.Entities)
            {
                string tableName = fieldpermission.GetAttributeValue<string>("entityname")!;
                string columnName = fieldpermission.GetAttributeValue<string>("attributelogicalname")!;
                values.Add($"{tableName}.{columnName}");
            }
            values.Sort();
            return values;
        }
        // </snippetGetSecuredColumnList>
        // <snippetSetColumnIsSecured>
        /// <summary>
        /// Updates the security status of a column in a Dataverse table.
        /// </summary>
        /// <remarks>This method retrieves the current definition of the specified column 
        /// and updates its security status only if the provided value differs from the 
        /// current value. If the column is already set to the specified security status, 
        /// no update request is sent.</remarks>
        /// <param name="service">The <see cref="IOrganizationService"/> instance used to 
        /// interact with the Dataverse service.</param>
        /// <param name="tableLogicalName">The logical name of the table containing the 
        /// column to be updated. Cannot be null or empty.</param>
        /// <param name="columnLogicalName">The logical name of the column whose security 
        /// status is to be updated. Cannot be null or empty.</param>
        /// <param name="value">A <see langword="true"/> value indicates that the column 
        /// should be secured; otherwise, <see langword="false"/>.</param>
        /// <param name="solutionUniqueName">The unique name of the solution in which the 
        /// column update should be applied. Cannot be null or empty.</param>
        /// <exception cref="Exception">Thrown if an error occurs while retrieving or 
        /// updating the column definition.</exception>
        static internal void SetColumnIsSecured(
            IOrganizationService service,
            string tableLogicalName,
            string columnLogicalName,
            bool value,
            string solutionUniqueName)
        {

            // Update request requires the entire column definition,
            // So retrieving that first

            RetrieveAttributeRequest retrieveRequest = new()
            {
                EntityLogicalName = tableLogicalName,
                LogicalName = columnLogicalName
            };

            AttributeMetadata columnDefinition;

            try
            {
                var retrieveResponse = (RetrieveAttributeResponse)service.Execute(retrieveRequest);

                columnDefinition = retrieveResponse.AttributeMetadata;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving column definition: {ex.Message}", ex);
            }

            if (!columnDefinition.IsSecured.HasValue || columnDefinition.IsSecured.Value != value)
            {
                // Set the IsSecured property to value
                columnDefinition.IsSecured = value;

                UpdateAttributeRequest updateRequest = new()
                {
                    EntityName = tableLogicalName,
                    Attribute = columnDefinition,
                    MergeLabels = true,
                    SolutionUniqueName = solutionUniqueName
                };

                try
                {
                    service.Execute(updateRequest);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error updating column definition: {ex.Message}", ex);
                }
            }
            else
            {
                //Don't send a request to set the value to what it already is.
            }
        }
        // </snippetSetColumnIsSecured>
        // <snippetRetrieveColumnId>
        /// <summary>
        /// Retrieves the unique identifier (MetadataId) of a column in a specified 
        /// Dataverse table.
        /// </summary>
        /// <remarks>
        /// This method queries the organization's metadata to locate the specified column 
        /// within the given table and returns its MetadataId. If the table or column is 
        /// not found, an exception is thrown.
        /// </remarks>
        /// <param name="service">The <see cref="IOrganizationService"/> instance used to 
        /// retrieve metadata from the organization.</param>
        /// <param name="tableLogicalName">The logical name of the table containing the 
        /// column. Must not be null or empty.</param>
        /// <param name="columnLogicalName">The logical name of the column whose MetadataId 
        /// is to be retrieved. Must not be null or empty.</param>
        /// <returns>The <see cref="Guid"/> representing the MetadataId of the specified 
        /// column.</returns>
        /// <exception cref="Exception">Thrown if the table or column is not found in the 
        /// metadata.</exception>
        private static Guid RetrieveColumnId(
            IOrganizationService service,
            string tableLogicalName,
            string columnLogicalName)
        {

            EntityQueryExpression query = new()
            {
                Properties = new MetadataPropertiesExpression("Attributes"),
                Criteria = new MetadataFilterExpression(filterOperator: LogicalOperator.Or)
                {
                    Conditions = {
                        {
                            new MetadataConditionExpression(
                                propertyName:"LogicalName",
                                conditionOperator: MetadataConditionOperator.Equals,
                                value:tableLogicalName)
                        }
                    },
                },
                AttributeQuery = new AttributeQueryExpression
                {
                    Properties = new MetadataPropertiesExpression("MetadataId"),
                    Criteria = new MetadataFilterExpression(filterOperator: LogicalOperator.And)
                    {
                        Conditions = {
                            {
                                new MetadataConditionExpression(
                                propertyName:"LogicalName",
                                conditionOperator: MetadataConditionOperator.Equals,
                                value:columnLogicalName)
                            }
                        }
                    }
                }
            };

            RetrieveMetadataChangesRequest request = new()
            {
                Query = query
            };

            var response = (RetrieveMetadataChangesResponse)service.Execute(request);

            Guid columnId;

            if (response.EntityMetadata.Count == 1)
            {

                if (response.EntityMetadata[0].Attributes.Length == 1)
                {

                    // Nullable property will not be null when retrieved. It is set by the system.
                    columnId = response.EntityMetadata[0].Attributes[0].MetadataId!.Value;
                }
                else
                {
                    throw new Exception($"Column {columnLogicalName} not found in {tableLogicalName}.");
                }
            }
            else
            {

                throw new Exception($"Table {tableLogicalName} not found");
            }
            return columnId;
        }
        // </snippetRetrieveColumnId>
        // <snippetGrantColumnAccess>
        /// <summary>
        /// Grants access to a secured column for a specified principal in Dataverse.
        /// </summary>
        /// <remarks>This method allows you to share read and/or update permissions for a 
        /// secured column in a Dataverse table with a specific principal (user or team). 
        /// The column must be configured as a secured field in Dataverse.</remarks>
        /// <param name="service">The <see cref="IOrganizationService"/> instance used to 
        /// interact with Dataverse.</param>
        /// <param name="record">A reference to the record (entity instance) containing the 
        /// secured column.</param>
        /// <param name="columnLogicalName">The logical name of the secured column to grant 
        /// access to.</param>
        /// <param name="principal">A reference to the principal (user or team) to whom 
        /// access is being granted.</param>
        /// <param name="readAccess"><see langword="true"/> to grant read access to the 
        /// secured column; otherwise, <see langword="false"/>.</param>
        /// <param name="updateAccess"><see langword="true"/> to grant update access to the 
        /// secured column; otherwise, <see langword="false"/>.</param>
        /// <exception cref="Exception">Thrown if the column has already been shared or if 
        /// an error occurs during the operation.</exception>
        static internal void GrantColumnAccess(
            IOrganizationService service,
            EntityReference record,
            string columnLogicalName,
            EntityReference principal,
            bool readAccess,
            bool updateAccess)
        {
            // This information should come from cached metadata,
            // but for this sample it is retrieved each time.
            Guid columnId = RetrieveColumnId(
                service: service,
                tableLogicalName: record.LogicalName,
                columnLogicalName: columnLogicalName);

            // https://learn.microsoft.com/power-apps/developer/data-platform/reference/entities/principalobjectattributeaccess
            Entity poaa = new("principalobjectattributeaccess")
            {
                //Unique identifier of the shared secured field
                ["attributeid"] = columnId,
                //Unique identifier of the entity instance with shared secured field
                ["objectid"] = record,
                //Unique identifier of the principal to which secured field is shared
                ["principalid"] = principal,
                // Read permission for secured field instance
                ["readaccess"] = readAccess,
                //Update permission for secured field instance
                ["updateaccess"] = updateAccess
            };

            try
            {
                service.Create(poaa);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                if (ex.Detail.ErrorCode.Equals(-2147158773))
                {
                    throw new Exception("The column has already been shared");
                }

                throw new Exception($"Dataverse error in GrantColumnAccess: {ex.Message}");

            }
            catch (Exception ex)
            {
                throw new Exception($"Error in GrantColumnAccess: {ex.Message}");
            }
        }
        // </snippetGrantColumnAccess>
        // <snippetModifyColumnAccess>
        /// <summary>
        /// Modifies access permissions for a secure column in a table for a specified 
        /// principal.
        /// </summary>
        /// <remarks>This method updates or creates a record in the 
        /// PrincipalObjectAttributeAccess table to reflect the specified access 
        /// permissions. If no matching record is found, an exception is thrown.</remarks>
        /// <param name="service">The <see cref="IOrganizationService"/> instance used to 
        /// interact with the organization service.</param>
        /// <param name="record">An <see cref="EntityReference"/> representing the record 
        /// containing the secure column.</param>
        /// <param name="columnLogicalName">The logical name of the secure column whose 
        /// access permissions are being modified.</param>
        /// <param name="principal">An <see cref="EntityReference"/> representing the 
        /// principal (user or team) for whom access permissions are being 
        /// modified.</param>
        /// <param name="readAccess">A <see langword="bool"/> indicating whether read 
        /// access to the secure column should be granted (<see langword="true"/>) or 
        /// revoked (<see langword="false"/>).</param>
        /// <param name="updateAccess">A <see langword="bool"/> indicating whether update 
        /// access to the secure column should be granted (<see langword="true"/>) or 
        /// revoked (<see langword="false"/>).</param>
        /// <exception cref="Exception">Thrown if no matching 
        /// PrincipalObjectAttributeAccess record is found for the specified column, 
        /// record, and principal.</exception>
        static internal void ModifyColumnAccess(
            IOrganizationService service,
            EntityReference record,
            string columnLogicalName,
            EntityReference principal,
            bool readAccess,
            bool updateAccess)
        {

            // This information should come from cached metadata,
            // but for this sample it is retrieved each time.
            Guid columnId = RetrieveColumnId(
                service: service,
                tableLogicalName: record.LogicalName,
                columnLogicalName: columnLogicalName);

            // Retrieve the record
            QueryExpression query = new("principalobjectattributeaccess")
            {
                ColumnSet = new ColumnSet(
                    "principalobjectattributeaccessid",
                    "readaccess",
                    "updateaccess"),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    // There can only be one record or zero records matching these criteria.
                    Conditions = {
                        {
                            new ConditionExpression(
                                attributeName:"attributeid",
                                conditionOperator: ConditionOperator.Equal,
                                value:columnId)
                        },
                        {
                            new ConditionExpression(
                                attributeName:"objectid",
                                conditionOperator: ConditionOperator.Equal,
                                value:record.Id)
                        },
                        {
                            new ConditionExpression(
                                attributeName:"principalid",
                                conditionOperator: ConditionOperator.Equal,
                                value:principal.Id)
                        },
                        {
                            new ConditionExpression(
                                attributeName:"principalidtype",
                                conditionOperator: ConditionOperator.Equal,
                                value:principal.LogicalName)
                        }
                    }
                }
            };

            EntityCollection queryResults = service.RetrieveMultiple(query);

            if (queryResults.Entities.Count == 1)
            {
                // Update the record that granted access to the secure column
                Entity retrievedPOAARecord = queryResults.Entities[0];
                // Get the current values and only update if different
                bool currentRead = retrievedPOAARecord.GetAttributeValue<bool>("readaccess");
                bool currentUpdate = retrievedPOAARecord.GetAttributeValue<bool>("updateaccess");

                Entity POAAForUpdate = new("principalobjectattributeaccess", retrievedPOAARecord.Id);

                if (currentRead != readAccess)
                {
                    POAAForUpdate.Attributes.Add("readaccess", readAccess);
                }
                if (currentUpdate != updateAccess)
                {
                    POAAForUpdate.Attributes.Add("updateaccess", updateAccess);
                }

                // Don't update if nothing there is nothing to change
                if (POAAForUpdate.Attributes.Count > 0)
                {
                    // Update the principalobjectattributeaccess record
                    service.Update(POAAForUpdate);
                }
            }
            else
            {
                throw new Exception("No matching PrincipalObjectAttributeAccess record found.");
            }
        }
        // </snippetModifyColumnAccess>
        // <snippetRevokeColumnAccess>
        /// <summary>
        /// Revokes access to a secure column for a specified principal in a given record.
        /// </summary>
        /// <remarks>This method removes the access granted to a secure column for the 
        /// specified principal. If no matching access record is found, an exception is 
        /// thrown.</remarks>
        /// <param name="service">The <see cref="IOrganizationService"/> instance used to 
        /// interact with the Dataverse service.</param>
        /// <param name="record">An <see cref="EntityReference"/> representing the record 
        /// containing the secure column.</param>
        /// <param name="columnLogicalName">The logical name of the secure column for which 
        /// access is being revoked.</param>
        /// <param name="principal">An <see cref="EntityReference"/> representing the 
        /// principal (user or team) whose access to the secure column is being 
        /// revoked.</param>
        /// <exception cref="Exception">Thrown if no matching 
        /// PrincipalObjectAttributeAccess record is found for the specified column, 
        /// record, and principal.</exception>
        internal static void RevokeColumnAccess(IOrganizationService service,
            EntityReference record,
            string columnLogicalName,
            EntityReference principal)
        {

            // This information should come from cached metadata,
            // but for this sample it is retrieved each time.
            Guid columnId = RetrieveColumnId(
                service: service,
                tableLogicalName: record.LogicalName,
                columnLogicalName: columnLogicalName);


            QueryExpression query = new("principalobjectattributeaccess")
            {
                ColumnSet = new ColumnSet("principalobjectattributeaccessid"),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    // These conditions return one or zero records
                    Conditions = {
                        {
                            new ConditionExpression(
                                attributeName:"attributeid",
                                conditionOperator: ConditionOperator.Equal,
                                value:columnId)
                        },
                        {
                            new ConditionExpression(
                                attributeName:"objectid",
                                conditionOperator: ConditionOperator.Equal,
                                value:record.Id)
                        },
                        {
                            new ConditionExpression(
                                attributeName:"principalid",
                                conditionOperator: ConditionOperator.Equal,
                                value:principal.Id)
                        },
                        {
                            new ConditionExpression(
                                attributeName:"principalidtype",
                                conditionOperator: ConditionOperator.Equal,
                                value:principal.LogicalName)
                        }
                    }
                }
            };

            EntityCollection queryResults = service.RetrieveMultiple(query);

            if (queryResults.Entities.Count == 1)
            {
                // Delete the record that granted access to the secure column
                service.Delete("principalobjectattributeaccess", queryResults.Entities[0].Id);
            }
            else
            {
                throw new Exception("No matching PrincipalObjectAttributeAccess record found.");
            }
        }
        // </snippetRevokeColumnAccess>
        // <snippetGetUnmaskedExampleRows>
        /// <summary>
        /// Retrieves a collection of example entities with unmasked data.
        /// </summary>
        /// <remarks>This method queries the "sample_example" entity and retrieves specific 
        /// columns, including sensitive data such as government ID and date of birth. The 
        /// query results are ordered by the "sample_name" column in descending order. The 
        /// method uses the "UnMaskedData" optional parameter to ensure that sensitive data 
        /// is returned unmasked. For more information on optional parameters, see <see 
        /// href="https://learn.microsoft.com/power-apps/developer/data-platform/optional-parameters">Optional 
        /// Parameters in Dataverse</see>.</remarks>
        /// <param name="service">The <see cref="IOrganizationService"/> instance used to 
        /// execute the query.</param>
        /// <returns>An <see cref="EntityCollection"/> containing the retrieved entities. 
        /// The collection includes unmasked data for the specified columns.</returns>
        internal static EntityCollection GetUnmaskedExampleRows(IOrganizationService service)
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

            RetrieveMultipleRequest request = new()
            {
                Query = query,
                // This example uses 'UnMaskedData' as an optional parameter
                // https://learn.microsoft.com/power-apps/developer/data-platform/optional-parameters
                ["UnMaskedData"] = true
            };


            var response = (RetrieveMultipleResponse)service.Execute(request);

            return response.EntityCollection;
        }
        // </snippetGetUnmaskedExampleRows>
        // <snippetAddPrivilegesToRole>
        /// <summary>
        /// Adds the specified privileges to a role in the organization.
        /// </summary>
        /// <remarks>This method retrieves the specified privileges by name and associates 
        /// them with the given role. The privileges must already exist in the organization, 
        /// and the role must be valid.</remarks>
        /// <param name="service">The <see cref="IOrganizationService"/> instance used to 
        /// interact with the organization service.</param>
        /// <param name="roleId">The unique identifier of the role to which the privileges 
        /// will be added.</param>
        /// <param name="depth">The <see cref="PrivilegeDepth"/> level that specifies the 
        /// scope of the privileges being added.</param>
        /// <param name="privilegeNames">An array of privilege names to be added to the 
        /// role. Each name must correspond to an existing privilege in the 
        /// organization.</param>
        /// <exception cref="Exception">Thrown if an unexpected error occurs while adding 
        /// privileges to the role.</exception>
        internal static void AddPrivilegesToRole(IOrganizationService service,
            Guid roleId,
            PrivilegeDepth depth,
            string[] privilegeNames)
        {

            // The ID of the role without the privileges
            Guid RoleId = roleId;

            // Retrieve the privileges
            var query = new QueryExpression
            {
                EntityName = "privilege",
                ColumnSet = new ColumnSet("privilegeid", "name")
            };
            query.Criteria.AddCondition("name", ConditionOperator.In, privilegeNames);

            DataCollection<Entity> privilegeRecords =
               service.RetrieveMultiple(query).Entities;

            // Define a list to hold the RolePrivileges we'll need to add
            List<RolePrivilege> rolePrivileges = [];

            //Populate the RolePrivileges parameter
            foreach (Entity privilege in privilegeRecords)
            {
                RolePrivilege rolePrivilege = new(
                   (int)depth,
                   privilege.GetAttributeValue<Guid>("privilegeid"));

                rolePrivileges.Add(rolePrivilege);
            }

            // Prepare the request
            AddPrivilegesRoleRequest request = new()
            {
                Privileges = [.. rolePrivileges],
                RoleId = RoleId
            };

            try
            {
                // Send the request
                service.Execute(request);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding privileges to role: {ex.Message}", ex);
            }
        }
        // </snippetAddPrivilegesToRole>
    }
}
