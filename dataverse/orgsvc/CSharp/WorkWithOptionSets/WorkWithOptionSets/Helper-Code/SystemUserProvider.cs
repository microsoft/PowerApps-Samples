// =====================================================================
//  This file is part of the Microsoft Dynamics CRM SDK code samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  This source code is intended only as a supplement to Microsoft
//  Development Tools and/or on-line documentation.  See these other
//  materials for detailed information regarding Microsoft code samples.
//
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//  PARTICULAR PURPOSE.
// =====================================================================

//<snippetSystemUserProvider>
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Threading;

// These namespaces are found in the Microsoft.Xrm.Sdk.dll assembly
// located in the SDK\bin folder of the SDK download.
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;

// This namespace is found in the Microsoft.Crm.Sdk.Proxy.dll assembly
// located in the SDK\bin folder of the SDK download.
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;

namespace PowerApps.Samples
{
    /// <summary>
    /// This class contains methods which retrieve the IDs of several fictitious Microsoft Dynamics
    /// CRM system users.  Several SDK samples require these additional user accounts in order to run.
    /// </summary>
    /// <remarks>For On-premises and IFD deployments, if these users do not exist they are created in
    /// Active Directory. This assumes that the system user account under which the application runs has
    /// system administrator privileges. Since it is not possible to programmatically create user accounts
    /// in Microsoft account, when running this code against a Microsoft Dynamics CRM Online server, you will have
    /// to manually add these users.</remarks>
    public class SystemUserProvider
    {
        public static Guid RetrieveSalesManager(CrmServiceClient service)
        {
            String ldapPath = String.Empty;
            return RetrieveSystemUser("kcook", "Kevin", "Cook", "Sales Manager", service, ref ldapPath);
        }
        public static Guid RetrieveSalesManager(CrmServiceClient service, ref String ldapPath)
        {
            return RetrieveSystemUser("kcook", "Kevin", "Cook", "Sales Manager", service, ref ldapPath);
        }

        public static List<Guid> RetrieveSalespersons(CrmServiceClient service, ref String ldapPath)
        {
            List<Guid> reps = new List<Guid>();

            reps.Add(RetrieveSystemUser("nanderson", "Nancy", "Anderson", "Salesperson", service, ref ldapPath));
            reps.Add(RetrieveSystemUser("dbristol", "David", "Bristol", "Salesperson", service, ref ldapPath));

            return reps;
        }

        public static List<Guid> RetrieveDelegates(CrmServiceClient service,
            ref String ldapPath)
        {
            List<Guid> delegates = new List<Guid>();

            delegates.Add(RetrieveSystemUser("dwilson", "Dan", "Wilson", "Delegate", service, ref ldapPath));
            delegates.Add(RetrieveSystemUser("canderson", "Christen", "Anderson", "Delegate", service, ref ldapPath));
            return delegates;
        }

        public static Guid RetrieveVPSales(CrmServiceClient service, ref String ldapPath)
        {
            return RetrieveSystemUser("mtucker", "Michael", "Tucker", "Vice President of Sales", service, ref ldapPath);
        }

        public static Guid RetrieveMarketingManager(CrmServiceClient service)
        {
            String ldapPath = String.Empty;
            return RetrieveMarketingManager(service, ref ldapPath);
        }

        public static Guid RetrieveMarketingManager(CrmServiceClient service, ref String ldapPath)
        {
            return RetrieveSystemUser("ssmith", "Samantha", "Smith", "Marketing Manager", service, ref ldapPath);
        }

        public static Guid RetrieveAUserWithoutAnyRoleAssigned(CrmServiceClient service)
        {
            String ldapPath = String.Empty;
            return RetrieveSystemUser("dpark", "Dan", "Park", "", service, ref ldapPath);
        }

        /// <summary>
        /// Retrieves the requested SystemUser record.  If the record does not exist, a new
        /// Microsoft Dynamics CRM SystemUser record is created and an associated Active
        /// Directory account is created, if it doesn't currently exist.
        /// </summary>
        /// <param name="userName">The username field as set in Microsoft Dynamics CRM</param>
        /// <param name="firstName">The first name of the system user to be retrieved</param>
        /// <param name="lastName">The last name of the system user to be retrieved</param>
        /// <param name="roleStr">The string representing the Microsoft Dynamics CRM security
        /// role for the user</param>
        /// <param name="serviceProxy">The OrganizationServiceProxy object to your Microsoft
        /// Dynamics CRM environment</param>
        /// <param name="ldapPath">The LDAP path for your network - you can either call
        /// ConsolePromptForLDAPPath() to prompt the user or provide a value in code</param>
        /// <returns></returns>
        public static Guid RetrieveSystemUser(String userName, String firstName,
            String lastName, String roleStr, CrmServiceClient service,
            ref String ldapPath)
        {
            String domain;
            Guid userId = Guid.Empty;

            if (service == null)
                throw new ArgumentNullException("service");

            if (String.IsNullOrWhiteSpace(userName))
                throw new ArgumentNullException("UserName");

            if (String.IsNullOrWhiteSpace(firstName))
                throw new ArgumentNullException("FirstName");

            if (String.IsNullOrWhiteSpace(lastName))
                throw new ArgumentNullException("LastName");

            // Obtain the current user's information.
            WhoAmIRequest who = new WhoAmIRequest();
            WhoAmIResponse whoResp = (WhoAmIResponse)service.Execute(who);
            Guid currentUserId = whoResp.UserId;

            SystemUser currentUser =
                service.Retrieve(SystemUser.EntityLogicalName, currentUserId, new ColumnSet("domainname")).ToEntity<SystemUser>();

            // Extract the domain and create the LDAP object.
            String[] userPath = currentUser.DomainName.Split(new char[] { '\\' });
            if (userPath.Length > 1)
                domain = userPath[0] + "\\";
            else
                domain = String.Empty;



            SystemUser existingUser = GetUserIdIfExist(service, domain, userName, firstName, lastName);


            if (existingUser != null)
            {
                userId = existingUser.SystemUserId.Value;

                if (!String.IsNullOrWhiteSpace(roleStr))
                {
                    // Check to make sure the user is assigned the correct role.
                    Role role = RetrieveRoleByName(service, roleStr);

                    // Associate the user with the role when needed.
                    if (!UserInRole(service, userId, role.Id))
                    {
                        AssociateRequest associate = new AssociateRequest()
                        {
                            Target = new EntityReference(SystemUser.EntityLogicalName, userId),
                            RelatedEntities = new EntityReferenceCollection()
                        {
                            new EntityReference(Role.EntityLogicalName, role.Id)
                        },
                            Relationship = new Relationship("systemuserroles_association")
                        };
                        service.Execute(associate);
                    }
                }
            }
            else
            {
                Console.WriteLine("User Not Found. Manually create user in office 365");
            }

            return userId;
        }

        /// <summary>
        /// Helper method to check if system user already exist with either given username or first and last name.
        /// </summary>
        /// <param name="serviceProxy">The OrganizationServiceProxy object to your Microsoft
        /// Dynamics CRM environment</param>
        /// <param name="domainName">Current domain name of the account.</param>
        /// <param name="userName">The username field as set in Microsoft Dynamics CRM</param>
        /// <param name="firstName">The first name of the system user to be retrieved</param>
        /// <param name="lastName">The last name of the system user to be retrieved</param>
        /// <returns></returns>
        public static SystemUser GetUserIdIfExist(CrmServiceClient service,
            String domainName, String userName, String firstName, String lastName)
        {
            QueryExpression userQuery = new QueryExpression
            {
                EntityName = SystemUser.EntityLogicalName,
                ColumnSet = new ColumnSet("systemuserid"),
                Criteria =
                {
                    FilterOperator = LogicalOperator.Or,
                    Filters =
                    {   
                        new FilterExpression
                        {
                            FilterOperator = LogicalOperator.And,
                            Conditions =
                            {
                                new ConditionExpression("domainname", ConditionOperator.Equal, domainName + userName)
                            }
                        },
                        new FilterExpression
                        {
                            FilterOperator = LogicalOperator.And,
                            Conditions = 
                            {
                                new ConditionExpression("firstname", ConditionOperator.Equal, firstName),
                                new ConditionExpression("lastname", ConditionOperator.Equal, lastName)
                            }
                        }
                    }

                }
            };

            DataCollection<Entity> existingUsers = (DataCollection<Entity>)service.RetrieveMultiple(userQuery).Entities;

            if (existingUsers.Count > 0)
                return existingUsers[0].ToEntity<SystemUser>();
            return null;
        }

       
        
        /// <summary>
        /// Helper method to prompt the user for the LDAP path for the network
        /// </summary>
        /// <returns>The LDAP path for your network</returns>
       

        private static bool UserInRole(CrmServiceClient service,
            Guid userId, Guid roleId)
        {
            // Establish a SystemUser link for a query.
            LinkEntity systemUserLink = new LinkEntity()
            {
                LinkFromEntityName = SystemUserRoles.EntityLogicalName,
                LinkFromAttributeName = "systemuserid",
                LinkToEntityName = SystemUser.EntityLogicalName,
                LinkToAttributeName = "systemuserid",
                LinkCriteria =
                {
                    Conditions = 
                    {
                        new ConditionExpression(
                            "systemuserid", ConditionOperator.Equal, userId)
                    }
                }
            };

            // Build the query.
            QueryExpression query = new QueryExpression()
            {
                EntityName = Role.EntityLogicalName,
                ColumnSet = new ColumnSet("roleid"),
                LinkEntities = 
                {
                    new LinkEntity()
                    {
                        LinkFromEntityName = Role.EntityLogicalName,
                        LinkFromAttributeName = "roleid",
                        LinkToEntityName = SystemUserRoles.EntityLogicalName,
                        LinkToAttributeName = "roleid",
                        LinkEntities = {systemUserLink}
                    }
                },
                Criteria =
                {
                    Conditions = 
                    {
                        new ConditionExpression("roleid", ConditionOperator.Equal, roleId)
                    }
                }
            };

            // Retrieve matching roles.
            EntityCollection ec = service.RetrieveMultiple(query);

            if (ec.Entities.Count > 0)
                return true;

            return false;
        }

        private static Role RetrieveRoleByName(CrmServiceClient service,
            String roleStr)
        {
            QueryExpression roleQuery = new QueryExpression
            {
                EntityName = Role.EntityLogicalName,
                ColumnSet = new ColumnSet("roleid"),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression("name", ConditionOperator.Equal, roleStr)
                    }
                }
            };

            return service.RetrieveMultiple(roleQuery).Entities[0].ToEntity<Role>();
        }
    }
}
//</snippetSystemUserProvider>