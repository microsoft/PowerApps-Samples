// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace Microsoft.PowerPlatform.Administration.Helpers
{
    public class SystemUserHepler
    {
        private Logger _logger;

        public SystemUserHepler(Logger logger) 
        {
            _logger = logger;
        }

        public Guid GetSystemUserId(CrmServiceClient service,
            String userPrincipalName, string filePath)
        {
            QueryExpression userQuery = new QueryExpression
            {
                EntityName = SystemUser.EntityLogicalName,
                ColumnSet = new ColumnSet("systemuserid"),
                Criteria =
                {
                    Filters =
                    {
                        new FilterExpression
                        {
                            Conditions =
                            {
                                new ConditionExpression("domainname", ConditionOperator.Equal, userPrincipalName)
                            }
                        }
                    }

                }
            };

            DataCollection<Entity> existingUsers = (DataCollection<Entity>)service.RetrieveMultiple(userQuery).Entities;

            if (existingUsers.Count > 0)
            {
                var systemUser = existingUsers[0].ToEntity<SystemUser>();
                return systemUser.SystemUserId.HasValue ? systemUser.SystemUserId.Value : Guid.Empty;
            }

            return Guid.Empty;  
        }
    }
}
