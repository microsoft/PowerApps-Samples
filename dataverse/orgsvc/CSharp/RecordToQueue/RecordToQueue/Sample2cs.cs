using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
   public class Sample2cs

    {
        private static  RetrieveUsers(CrmServiceClient service)
        {
            var users = service.RetrieveMultiple(new QueryExpression(SystemUser.EntityLogicalName)
            {
                ColumnSet = new ColumnSet("systemuserid", "fullname"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression("sytemuserid", ConditionOperator.EqualUserId)
                    }
                }
            }).Entities[0].ToEntity<SystemUser>();
        }
    }
}
