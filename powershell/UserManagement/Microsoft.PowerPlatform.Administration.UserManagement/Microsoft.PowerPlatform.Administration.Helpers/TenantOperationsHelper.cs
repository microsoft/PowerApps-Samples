// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Discovery;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security;
using Microsoft.Crm.Sdk.Messages;
using System.Runtime.CompilerServices;

namespace Microsoft.PowerPlatform.Administration.Helpers
{
    public class TenantOperationsHelper
    {
        private Logger _logger;

        /// <summary>
        /// Gets all users that have role 'role Name' from a specific instance, all instances across specified geo or all geos
        /// </summary>
        /// <param name="userName">name of calling user</param>
        /// <param name="securePassword">password as a secure string</param>
        /// <param name="roleName">role name</param>
        /// <param name="environmentUrl">instance url</param>
        /// <param name="geo">geo to retrieve instances from</param>
        /// <param name="processAllEnvironments">get reports from all instances or only a few</param>
        /// <param name="logFileLocation">location to write logs to</param>
        public void GetUsersWithRoleAssignment(string userName, string password, string roleName, string environmentUrl, string geo, bool processAllEnvironments, string logFileLocation)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException(userName, nameof(userName));
            }

            _logger = new Logger(logFileLocation);
            _logger.CreateRoleAssignmentReportsDirectory();
            _logger.LogGeneric($"userName : {userName}, roleName : {roleName}, environmentUrl : {environmentUrl}, geo : {geo}, processAllEnvironments : {processAllEnvironments}");

            EnvironmentOperationsHelper environmentOperationsHelper = new EnvironmentOperationsHelper(_logger);

            if (!string.IsNullOrWhiteSpace(environmentUrl))
            {
                environmentOperationsHelper.GetAllUsersWithRoleAssignmentFromEnvironment(userName, password, environmentUrl, null, roleName);
            }
            else if (processAllEnvironments)
            {
                var environments = GetAllEnvironmentsByGeo(userName, password, geo);

                foreach (var environment in environments)
                {
                    environmentOperationsHelper.GetAllUsersWithRoleAssignmentFromEnvironment(userName, password, string.Empty, environment, roleName);
                }
            }
            else
            {
                _logger.LogGeneric($"No environments supplied to process.");
            }
        }

        /// <summary>
        /// Remove role assignments from list of users in one or more environments
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="roleName"></param>
        /// <param name="userListFilePath"></param>
        /// <param name="environmentUrl"></param>
        /// <param name="geo"></param>
        /// <param name="processAllEnvironments"></param>
        /// <param name="logFileLocation"></param>
        public void RemoveRoleAssignmentFromUsers(string userName, string password, string roleName, string userListFilePath, string environmentUrl, string geo, bool processAllEnvironments, string logFileLocation)
        {
            _logger = new Logger(logFileLocation);
            _logger.CreateRoleRemovalLogsDirectory();

            EnvironmentOperationsHelper environmentOperationsHelper = new EnvironmentOperationsHelper(_logger);

            IList<string> userPrincipals = GetUserPrincipalsFromInput(userListFilePath);

            if (!string.IsNullOrWhiteSpace(environmentUrl))
            {
                environmentOperationsHelper.RemoveRolesFromUsersInEnvironment(userName, password, roleName, environmentUrl, userPrincipals);
            }
            else if (processAllEnvironments)
            {
                var environments = GetAllEnvironmentsByGeo(userName, password, geo);

                foreach (var environment in environments)
                {
                    environmentOperationsHelper.RemoveRolesFromUsersInEnvironment(userName, password, roleName, environment, userPrincipals);
                }
            }
            else
            {
                _logger.LogGeneric($"No environments supplied to process.");
            }
        }

        /// <summary>
        /// Add role assignments from list of users in one or more environments
        /// </summary>  
        /// <param name="userName"></param>
        /// <param name="securePassword"></param>
        /// <param name="roleName"></param>
        /// <param name="userListFilePath"></param>
        /// <param name="environmentUrl"></param>
        /// <param name="geo"></param>
        /// <param name="processAllEnvironments"></param>
        /// <param name="logFileLocation"></param>
        public void AddRoleToUsers(string userName, string password, string roleName, string userListFilePath, string environmentUrl, string geo, bool processAllEnvironments, string logFileLocation)
        {
            _logger = new Logger(logFileLocation);
            _logger.CreateAddRoleLogsDirectory();

            EnvironmentOperationsHelper environmentOperationsHelper = new EnvironmentOperationsHelper(_logger);

            IList<string> userPrincipals = GetUserPrincipalsFromInput(userListFilePath);

            if (!string.IsNullOrWhiteSpace(environmentUrl))
            {
                environmentOperationsHelper.AddRoleToUsersInEnvironment(userName, password, roleName, environmentUrl, userPrincipals);
            }
            else if (processAllEnvironments)
            {
                var environments = GetAllEnvironmentsByGeo(userName, password, geo);

                foreach (var environment in environments)
                {
                    environmentOperationsHelper.AddRoleToUsersInEnvironment(userName, password, roleName, environment, userPrincipals);
                }
            }
            else
            {
                _logger.LogGeneric($"No environments supplied to process.");
            }
        }

        /// <summary>
        /// Bulk assign records to users in one or more environments
        /// </summary>  
        /// <param name="userName"></param>
        /// <param name="securePassword"></param>
        /// <param name="userListFilePath"></param>
        /// <param name="environmentUrl"></param>
        /// <param name="geo"></param>
        /// <param name="processAllEnvironments"></param>
        /// <param name="logFileLocation"></param>
        public void BulkAssignRecordsToUsers(string userName, string password, string userListFilePath, string environmentUrl, string geo, bool processAllEnvironments, string logFileLocation)
        {
            _logger = new Logger(logFileLocation);
            _logger.CreateUserRecordsAssignmentsLogsDirectory();

            EnvironmentOperationsHelper environmentOperationsHelper = new EnvironmentOperationsHelper(_logger);

            IList<string> userPrincipals = GetUserPrincipalsFromInput(userListFilePath);

            if (!string.IsNullOrWhiteSpace(environmentUrl))
            {
                environmentOperationsHelper.BulkAssignUserRecordsInEnvironment(userName, password, environmentUrl, userPrincipals);
            }
            else if (processAllEnvironments)
            {
                var environments = GetAllEnvironmentsByGeo(userName, password, geo);

                foreach (var environment in environments)
                {
                    environmentOperationsHelper.BulkAssignUserRecordsInEnvironment(userName, password, environment, userPrincipals);
                }
            }
            else
            {
                _logger.LogGeneric($"No environments supplied to process.");
            }
        }

        private IList<string> GetUserPrincipalsFromInput(string userListFilePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userListFilePath))
                {
                    _logger.LogException($"Please supply valid file path for the list of users");
                }

                var userNames = File.ReadLines(userListFilePath);

                return userNames.Select(x => x.Trim()).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogException($"Error trying to retrieve users from file {userListFilePath}. Exception {ex}");
            }

            return new List<string>();
        }

        private List<OrganizationDetail> GetAllEnvironmentsByGeo(string userName, string password, string geo)
        {
            var cloud = GetCloudByGeo(geo);

            var environments = GetAllEnvironments(userName, password, cloud);

            if (!string.IsNullOrWhiteSpace(geo))
            {
                environments = environments.Where(x => string.Equals(x.Geo, geo, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            _logger.LogGeneric($"Retrieved all environments in geo {geo}. Count {environments.Count}");

            return environments;
        }

        private List<OrganizationDetail> GetAllEnvironments(string userName, string password, Cloud cloud)
        {
            if (cloud == Cloud.Unknown)
            {
                throw new ArgumentOutOfRangeException("Cloud.Unknown cannot be used as a parameter for this method.");
            }

            try
            {
                var type = typeof(Cloud);
                var memInfo = type.GetMember(cloud.ToString());
                var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                Uri targeturl = new Uri(((DescriptionAttribute)attributes[0]).Description);

                var clientId = "51f81489-12ee-4a9e-aaae-a2591f45987d";
                var redirectUrl = "app://58145B91-0C36-4500-8554-080854F2AC97";

                var creds = new System.ServiceModel.Description.ClientCredentials();
                creds.UserName.UserName = userName;
                creds.UserName.Password = password;
                Uri appReplyUri = new Uri(redirectUrl);

                var organizations = CrmServiceClient.DiscoverGlobalOrganizations(
                      discoveryServiceUri: new Uri($"{targeturl}/api/discovery/v2.0/Instances"),
                      clientCredentials: creds,
                      user: null,
                      clientId: clientId,
                      redirectUri: appReplyUri,
                      tokenCachePath: "",
                      isOnPrem: false,
                      authority: string.Empty,
                      promptBehavior: PromptBehavior.Auto);

                _logger.LogGeneric($"Retrieved all global environments. Count : {organizations.Count}");

                return organizations.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogException($"Exception retrieving environments. Exception {ex}");
                throw;
            }
        }

        private Cloud GetCloudByGeo(string geo)
        {
            if (string.IsNullOrWhiteSpace(geo))
            {
                return Cloud.Commercial;
            }

            switch (geo.ToUpper())
            {
                case "GCC": return Cloud.GCC; break;
                case "USG": return Cloud.USG; break;
                case "DOD": return Cloud.DOD; break;
                case "CHN": return Cloud.CHINA; break;
                default: return Cloud.Commercial; break;
            }
        }
    }
}
