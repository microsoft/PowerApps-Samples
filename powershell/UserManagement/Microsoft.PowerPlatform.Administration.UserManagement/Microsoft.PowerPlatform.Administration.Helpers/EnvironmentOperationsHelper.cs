// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.Xrm.Sdk.Discovery;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.PowerPlatform.Administration.Helpers
{
    public class EnvironmentOperationsHelper
    {
        private Logger _logger;

        public EnvironmentOperationsHelper(Logger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Generates a report for all users with given role name in the environment
        /// </summary>
        /// <param name="userName">user Name</param>
        /// <param name="password">password</param>
        /// <param name="environmentUrl">environment Url</param>
        /// <param name="organizationDetail">organization details retrieved from global discovery </param>
        /// <param name="roleName">role Name</param>
        public void GetAllUsersWithRoleAssignmentFromEnvironment(string userName, string password, string environmentUrl, OrganizationDetail organizationDetail, string roleName)
        {
            var connectionHelper = new ConnectionHelper(_logger);
            CrmServiceClient service;

            try
            {
                if (!string.IsNullOrWhiteSpace(environmentUrl))
                {
                    service = connectionHelper.GetCrmServiceClient(userName, password, environmentUrl);
                }
                else if (organizationDetail != null)
                {
                    service = connectionHelper.GetCrmServiceClient(userName, password, organizationDetail);
                }
                else
                {
                    _logger.LogGeneric($"No environment is supplied to connect ");
                    return;
                }

                GetAllUsersWithRoleAssignmentFromEnvironment(service, roleName);
            }
            catch (Exception ex)
            {
                var environmentName = !string.IsNullOrWhiteSpace(environmentUrl) ? environmentUrl : organizationDetail.FriendlyName;
                _logger.LogException($"Exception trying to retrieve user role assignments from environment : {environmentName}. Exception : {ex}");
            }
        }

        private void GetAllUsersWithRoleAssignmentFromEnvironment(CrmServiceClient service, string roleName)
        {
            var roleManagementOperationsHelper = new RoleManagementOperationsHelper(_logger);

            var roleId = roleManagementOperationsHelper.GetRoleByName(service, roleName);

            if (roleId == Guid.Empty)
            {
                _logger.LogGeneric($"No role found with name {roleName} in the environment : {service.ConnectedOrgFriendlyName}");
                return;
            }

            roleManagementOperationsHelper.GetAllUsersWithRoleFromEnvironment(service, roleName);
        }

        /// <summary>
        /// remove given role name from users in the environment
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="roleName"></param>
        /// <param name="organizationDetail"></param>
        /// <param name="userPrincipalNames"></param>
        public void RemoveRolesFromUsersInEnvironment(string userName, string password, string roleName, OrganizationDetail organizationDetail, IList<string> userPrincipalNames)
        {
            var webServiceEndpoint = organizationDetail.Endpoints.Where(x => x.Key == EndpointType.WebApplication).FirstOrDefault();

            RemoveRolesFromUsersInEnvironment(userName, password, roleName, webServiceEndpoint.Value, userPrincipalNames);
        }

        /// <summary>
        /// Identifies all the users that have role in the environment, and removes them
        /// </summary>
        /// <param name="userName">user Name</param>
        /// <param name="password">password</param>
        /// <param name="roleName">role Name</param>
        /// <param name="environmentUrl">environment Url</param>
        /// <param name="userPrincipalNames">list of userPrincipalNames</param>
        public void RemoveRolesFromUsersInEnvironment(string userName, string password, string roleName, string environmentUrl, IList<string> userPrincipalNames)
        {
            try
            {
                var connectionHelper = new ConnectionHelper(_logger);
                var service = connectionHelper.GetCrmServiceClient(userName, password, environmentUrl);
                var roleManagementOperationsHelper = new RoleManagementOperationsHelper(_logger);

                var roleId = roleManagementOperationsHelper.GetRoleByName(service, roleName);

                if (roleId == Guid.Empty)
                {
                    _logger.LogGeneric($"No role found with name {roleName} in the environment : {environmentUrl}");
                    return;
                }

                var systemUserHelper = new SystemUserHepler(_logger);

                var filePath = _logger.GetRoleRemovalLogPath(_logger.RoleRemovalLogsDir, service.ConnectedOrgFriendlyName.Replace(' ', '_'), roleName.Replace(' ', '.'));

                foreach (var userPrincipal in userPrincipalNames)
                {
                    try
                    {
                        var systemUserId = systemUserHelper.GetSystemUserId(service, userPrincipal, filePath);
                        if (systemUserId == Guid.Empty)
                        {
                            _logger.LogToFile(filePath, $"System user record not found for {userPrincipal}. No role removed");
                            continue;
                        }

                        roleManagementOperationsHelper.RemoveRoleFromUser(service, roleName, roleId, systemUserId, userPrincipal, filePath);
                    }
                    catch (Exception ex)
                    {
                        // exption is recorded & moved on to next user.
                        _logger.LogException($"Unbale to remove role {roleName} from user {userPrincipal}. Exception : {ex}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogException($"Exception while retrieving users with role name {roleName} from environment {environmentUrl} : \n {ex.ToString()} ");
                return;
            }
        }

        /// <summary>
        /// Add given role to users in environment.
        /// </summary>
        /// <param name="userName">user Name</param>
        /// <param name="password">password</param>
        /// <param name="roleName">role Name</param>
        /// <param name="environmentUrl">environment Url</param>
        /// <param name="userPrincipalNames">user Principal Names.</param>
        public void AddRoleToUsersInEnvironment(string userName, string password, string roleName, string environmentUrl, IList<string> userPrincipalNames)
        {
            try
            {
                var connectionHelper = new ConnectionHelper(this._logger);
                var service = connectionHelper.GetCrmServiceClient(userName, password, environmentUrl);
                var roleManagementOperationsHelper = new RoleManagementOperationsHelper(_logger);

                var roleId = roleManagementOperationsHelper.GetRoleByName(service, roleName);

                if (roleId == Guid.Empty)
                {
                    _logger.LogGeneric($"No role found with name {roleName} in the environment : {environmentUrl}");
                    return;
                }

                var systemUserHelper = new SystemUserHepler(_logger);
                var filePath = _logger.GetAddRolesLogPath(_logger.AddRoleLogsDir, service.ConnectedOrgFriendlyName.Replace(' ', '_'), roleName.Replace(' ', '.'));

                foreach (var userPrincipal in userPrincipalNames)
                {
                    try
                    {
                        var systemUserId = systemUserHelper.GetSystemUserId(service, userPrincipal, filePath);
                        if (systemUserId == Guid.Empty)
                        {
                            _logger.LogToFile(filePath, $"System user record not found for {userPrincipal}. No role added");
                            continue;
                        }

                        roleManagementOperationsHelper.AssignRoleToUser(service, roleName, roleId, systemUserId, userPrincipal, filePath);
                    }
                    catch (Exception ex)
                    {
                        // exption is recorded & moved on to next user.
                        _logger.LogException($"Unbale to assign role {roleName} from user {userPrincipal}. Exception : {ex}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogException($"Exception while retrieving users with role name {roleName} from environment {environmentUrl} : \n {ex.ToString()} ");
                return;
            }
        }

        /// <summary>
        /// Add given role to users in environment.
        /// </summary>
        /// <param name="userName">user Name</param>
        /// <param name="password">password</param>
        /// <param name="roleName">role Name</param>
        /// <param name="organizationDetail">organization Detail</param>
        /// <param name="userPrincipalNames">user Principal Names.</param>
        public void AddRoleToUsersInEnvironment(string userName, string password, string roleName, OrganizationDetail organizationDetail, IList<string> userPrincipalNames)
        {
            var webServiceEndpoint = organizationDetail.Endpoints.Where(x => x.Key == EndpointType.WebApplication).FirstOrDefault();

            AddRoleToUsersInEnvironment(userName, password, roleName, webServiceEndpoint.Value, userPrincipalNames);
        }

        /// <summary>
        /// Bulk assign user records from source user to target user in environment.
        /// </summary>
        /// <param name="userName">user Name</param>
        /// <param name="password">password</param>
        /// <param name="environmentUrl">environment Url</param>
        /// <param name="upnList">user Principal Names.</param>
        public void BulkAssignUserRecordsInEnvironment(string userName, string password, string environmentUrl, IList<string> upnList)
        {
            try
            {
                var connectionHelper = new ConnectionHelper(this._logger);
                var service = connectionHelper.GetCrmServiceClient(userName, password, environmentUrl);
                var roleManagementOperationsHelper = new RoleManagementOperationsHelper(_logger);

                var systemUserHelper = new SystemUserHepler(_logger);
                foreach (var userPrincipals in upnList)
                {
                    try
                    {
                        var userPrincipalNames = userPrincipals.Split(',').Select(s => s.Trim()).ToArray(); ;
                        var sourceUserPrincipal = userPrincipalNames[0];
                        var targetUserPrincipal = userPrincipalNames[1];
                        var filePath = _logger.GetUserRecordsAssignmentsFromUserReportPath(_logger.UserRecordsAssignmentsLogsDir, service.ConnectedOrgFriendlyName.Replace(' ', '_'), sourceUserPrincipal);
                        var sourceSystemUserId = systemUserHelper.GetSystemUserId(service, sourceUserPrincipal, filePath);
                        if (sourceSystemUserId == Guid.Empty)
                        {
                            _logger.LogToFile(filePath, $"System user record not found for {sourceUserPrincipal}.");
                            continue;
                        }
                        var targetSystemUserId = systemUserHelper.GetSystemUserId(service, targetUserPrincipal, filePath);
                        if (targetSystemUserId == Guid.Empty)
                        {
                            _logger.LogToFile(filePath, $"System user record not found for {targetUserPrincipal}.");
                            continue;
                        }

                        roleManagementOperationsHelper.AssignUserRecordsFromSourceToTargetUser(service, sourceSystemUserId, targetSystemUserId, filePath);
                    }
                    catch (Exception ex)
                    {
                        // exption is recorded & moved on to next user.
                        _logger.LogException($"Unbale to assign user records from source user to target user. Exception : {ex}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogException($"Exception while assigning user records from environment {environmentUrl} : \n {ex.ToString()} ");
                return;
            }
        }

        /// <summary>
        /// Bulk assign user records from source user to target user in environment.
        /// </summary>
        /// <param name="userName">user Name</param>
        /// <param name="password">password</param>
        /// <param name="environmentUrl">environment Url</param>
        /// <param name="sourceUserPrincipalNames">source user principal names.</param>
        /// <param name="targetUserPrincipalNames">target user upns.</param>
        /// <param name="userPrincipalNames">user Principal Names.</param>
        public void BulkAssignUserRecordsInEnvironment(string userName, string password, OrganizationDetail organizationDetail, IList<string> userPrincipalNames)
        {
            var webServiceEndpoint = organizationDetail.Endpoints.Where(x => x.Key == EndpointType.WebApplication).FirstOrDefault();

            BulkAssignUserRecordsInEnvironment(userName, password, webServiceEndpoint.Value, userPrincipalNames);
        }
    }

    /// <summary>
    /// Cloud information for Discovery service.
    /// </summary>
    public enum Cloud
    {
        Unknown,
        [Description("https://globaldisco.crm.dynamics.com")]
        Commercial,
        [Description("https://globaldisco.crm9.dynamics.com")]
        GCC,
        [Description("https://globaldisco.crm.microsoftdynamics.us")]
        USG,
        [Description("https://globaldisco.crm.appsplatform.us")]
        DOD,
        [Description("https://globaldisco.crm.dynamics.cn")]
        CHINA
    }
}

