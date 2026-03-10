// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Microsoft.PowerPlatform.Administration.Helpers
{
    public class RoleManagementOperationsHelper
    {
        private Logger _logger;
        public RoleManagementOperationsHelper(Logger logger)
        {
            _logger = logger;
        }

        // Define the fetch attributes.
        int fetchRecordCountPerPage = 5000;
        int fetchPageNumber = 1;
        string fetchPagingCookie = null;

        public string fetchUsersWithRole = @"<fetch version='1.0' 
                                                    mapping='logical' 
                                                    output-format='xml-platform'>
                    <entity name='systemuser'>
                        <attribute name='domainname' alias='systemuserdomainname' />
                        <attribute name='systemuserid' alias='sysuserid' />
                        <attribute name='isdisabled' alias='IsDisabled' />
                        <attribute name='islicensed' alias='IsLicensed' />
                        <attribute name='accessmode' alias='Accessmode' />
                        <filter>
                          <condition attribute='applicationid' operator='null' />
                        </filter>
                        <link-entity name='systemuserroles' from='systemuserid' to='systemuserid' link-type='inner' alias='userroles' intersect='true'>
                          <link-entity name='role' from='roleid' to='roleid' alias='roles' intersect='true'>
                            <filter>
                              <condition attribute='name' operator='eq' value='{0}' />
                            </filter>
                          </link-entity>
                        </link-entity>
                      </entity>
                    </fetch>";

        public void GetAllUsersWithRoleFromEnvironment(CrmServiceClient service, string roleName)
        {
            QueryExpression query = new QueryExpression();
            query.NoLock = true;

            string fetchXml = string.Format(fetchUsersWithRole, roleName);

            var reportInitialized = false;
            string reportFilePath = string.Empty;

            while (true)
            {
                string xml = CreateXml(fetchXml, fetchPagingCookie, fetchPageNumber, fetchRecordCountPerPage);

                var result = service.GetEntityDataByFetchSearchEC(xml);

                var entities = result.Entities;

                if (!result.Entities.Any())
                {
                    _logger.LogGeneric($"No users with role {roleName} found in org {service.CrmConnectOrgUriActual}.");
                    return;
                }
                if (reportInitialized == false)
                {
                    reportFilePath = _logger.GetUsersWithRoleAssignmentsReportPath(_logger.RoleAssignmentsReportsDir, service.ConnectedOrgFriendlyName.Replace(' ', '_'), roleName.Replace(' ', '_'));
                    _logger.InitializeRoleAssignmentReport(reportFilePath);
                    reportInitialized = true;
                }


                foreach (var entity in entities)
                {
                    LogRoleAssignment(reportFilePath, entity);
                }

                // Check for morerecords
                if (result.MoreRecords)
                {
                    _logger.LogGeneric(string.Format("\n Environment : {0} - fetching roles page {1}\n*****", service.ConnectedOrgFriendlyName, fetchPageNumber));

                    fetchPageNumber++;

                    fetchPagingCookie = result.PagingCookie;
                }
                else
                {
                    break;
                }
            }
        }

        public void AssignUserRecordsFromSourceToTargetUser(CrmServiceClient service, Guid sourceSystemUserId, Guid targetSystemUserId, string filePath)
        {
            var reassignObjectsOwnerRequest = new ReassignObjectsOwnerRequest { FromPrincipal = new EntityReference("systemuser", sourceSystemUserId), ToPrincipal = new EntityReference("systemuser", targetSystemUserId) };
            service.Execute(reassignObjectsOwnerRequest);

            _logger.LogToFile(filePath, $"Records from user with system user id {sourceSystemUserId} are reassigned to user with system user id {targetSystemUserId} in org {service.CrmConnectOrgUriActual}.");
        }

        private void LogRoleAssignment(string filepath, Entity user)
        {
            var attributeValues = user.Attributes.Values.ToArray();

            var accessmode = (Microsoft.Xrm.Sdk.AliasedValue)attributeValues[0];

            var accessmodeString = Enum.GetName(typeof(AccessMode), ((Microsoft.Xrm.Sdk.OptionSetValue)accessmode.Value).Value).ToString();

            var userPrincipalName = ((Microsoft.Xrm.Sdk.AliasedValue)attributeValues[2]).Value;

            var systemUserId = attributeValues[1];

            var isDisabled = ((Microsoft.Xrm.Sdk.AliasedValue)attributeValues[4]).Value;

            var isLicensed = ((Microsoft.Xrm.Sdk.AliasedValue)attributeValues[5]).Value;

            if (string.Equals(userPrincipalName.ToString(), "crmoln2@microsoft.com", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogGeneric($"Skip logging crmoln2@microsoft.com, a platform user");
                return;
            }

            _logger.LogToFile(filepath, new string[] { $"{systemUserId}, {userPrincipalName}, {accessmodeString}, {isDisabled}, {isLicensed}" });
        }

        private string CreateXml(string xml, string cookie, int page, int count)
        {
            StringReader stringReader = new StringReader(xml);
            XmlTextReader reader = new XmlTextReader(stringReader);

            XmlDocument doc = new XmlDocument();
            doc.Load(reader);

            return CreateXml(doc, cookie, page, count);
        }

        private string CreateXml(XmlDocument doc, string cookie, int page, int count)
        {
            XmlAttributeCollection attrs = doc.DocumentElement.Attributes;

            if (cookie != null)
            {
                XmlAttribute pagingAttr = doc.CreateAttribute("paging-cookie");
                pagingAttr.Value = cookie;
                attrs.Append(pagingAttr);
            }

            XmlAttribute pageAttr = doc.CreateAttribute("page");
            pageAttr.Value = System.Convert.ToString(page);
            attrs.Append(pageAttr);

            XmlAttribute countAttr = doc.CreateAttribute("count");
            countAttr.Value = System.Convert.ToString(count);
            attrs.Append(countAttr);

            StringBuilder sb = new StringBuilder(1024);
            StringWriter stringWriter = new StringWriter(sb);

            XmlTextWriter writer = new XmlTextWriter(stringWriter);
            doc.WriteTo(writer);
            writer.Close();

            return sb.ToString();
        }

        public Guid GetRoleByName(CrmServiceClient service, string roleName)
        {
            QueryExpression roleQuery = new QueryExpression
            {
                EntityName = Role.EntityLogicalName,
                ColumnSet = new ColumnSet("roleid"),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression("name", ConditionOperator.Equal, roleName)
                    }
                }
            };

            var role = service.RetrieveMultiple(roleQuery);
            Role roleEntity;

            if (role.Entities.Any())
            {

                roleEntity = service.RetrieveMultiple(roleQuery).Entities[0].ToEntity<Role>();

                if (roleEntity == null)
                {
                    return Guid.Empty;
                }

                return roleEntity.RoleId.Value;
            }
            else
            {
                _logger.LogGeneric($"No role found with name {roleName}");
            }

            return Guid.Empty;
        }

        public void RemoveRoleFromUser(CrmServiceClient service, string roleName, Guid roleId, Guid systemUserId, string userPrincipalName, string logFilePath)
        {
            service.Disassociate("systemuser",
            systemUserId,
            new Relationship("systemuserroles_association"),
            new EntityReferenceCollection() { new EntityReference("role", roleId) });
            _logger.LogToFile(logFilePath, $"Role {roleName} is removed from systemuser {userPrincipalName}");
        }

        public void AssignRoleToUser(CrmServiceClient service, string roleName, Guid roleId, Guid systemUserId, string userPrincipalName, string logFilePath)
        {
            service.Associate("systemuser",
            systemUserId,
            new Relationship("systemuserroles_association"),
            new EntityReferenceCollection() { new EntityReference("role", roleId) });
            _logger.LogToFile(logFilePath, $"Role {roleName} is added to systemuser {userPrincipalName}");
        }
    }
}



