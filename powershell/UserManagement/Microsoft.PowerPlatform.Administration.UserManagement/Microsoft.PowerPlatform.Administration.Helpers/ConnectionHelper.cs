// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Sdk.Discovery;
using Microsoft.Xrm.Tooling.Connector;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel.Description;

namespace Microsoft.PowerPlatform.Administration.Helpers
{
    public class ConnectionHelper
    {
        private Logger _logger;

        public ConnectionHelper(Logger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Creates crm service client using user credentials
        /// </summary>
        /// <param name="userPrincipalName">userPrincipalName</param>
        /// <param name="password">password</param>
        /// <param name="organizationDetial">organizationDetial</param>
        /// <returns>crm service client object</returns>
        public CrmServiceClient GetCrmServiceClient(string userPrincipalName, string password, OrganizationDetail organizationDetial)
        {
            var webServiceEndpoint = organizationDetial.Endpoints.Where(x => x.Key == EndpointType.WebApplication).FirstOrDefault();

            return this.GetCrmServiceClient(userPrincipalName, password, webServiceEndpoint.Value);
        }

        /// <summary>
        /// Initiates a crm service client connection with user credentials
        /// </summary>
        /// <param name="userPrincipalName">userPrincipalName</param>
        /// <param name="password">password</param>
        /// <param name="instanceUrl">instanceUrl</param>
        /// <returns>Crm service client object</returns>
        public CrmServiceClient GetCrmServiceClient(string userPrincipalName, string password, string instanceUrl)
        {
            CrmServiceClient service = null;
            var tokenCache = Path.Combine(_logger.LogLocation, "TokenCache");

            var connectionString = "AuthType=OAuth;userPrincipalName={0};Password={1};Url={2};AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;RedirectUri=app://58145B91-0C36-4500-8554-080854F2AC97;TokenCacheStorePath={3};LoginPrompt=Auto";
                
            var formattedconnectionString = string.Format(connectionString, userPrincipalName, password, instanceUrl, tokenCache);

            service = new CrmServiceClient(formattedconnectionString);

            if (!string.IsNullOrEmpty(service.LastCrmError))
            {
                _logger.LogException($"Error connecting to instance {instanceUrl}. Error : {service.LastCrmError}");
                throw new System.Exception($"Error connecting to instance {instanceUrl}. Error : {service.LastCrmError}");
            }
            else
            {
                _logger.LogGeneric($"Connected to instance {service.ConnectedOrgFriendlyName}");
            }

            return service;
        }
    }
}