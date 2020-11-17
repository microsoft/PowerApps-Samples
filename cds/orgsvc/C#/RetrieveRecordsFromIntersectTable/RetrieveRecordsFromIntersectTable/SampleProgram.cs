using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
   public partial class SampleProgram
    {
        [STAThread] // Added to support UX
        static void Main(string[] args)
        {
            CrmServiceClient service = null;
            try
            {
                service = SampleHelpers.Connect("Connect");
                // Service implements IOrganizationService interface 
                if (service.IsReady)
                {
                    #region Sample Code
                    /////////////////////////////////////////////
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate

                    //Create Query Expression.
                    var query = new QueryExpression()
                    {
                        EntityName = "role",
                        ColumnSet = new ColumnSet("name"),
                        LinkEntities =
                        {
                            new LinkEntity
                            {
                                LinkFromEntityName = Role.EntityLogicalName,
                                LinkFromAttributeName = "roleid",
                                LinkToEntityName = SystemUserRoles.EntityLogicalName,
                                LinkToAttributeName = "roleid",
                                LinkCriteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression
                                        {
                                            AttributeName = "systemuserid",
                                            Operator = ConditionOperator.Equal,
                                            Values = { userId }
                                        }
                                    }
                                }
                            }
                        }
                    };

                    // Obtain results from the query expression.
                    EntityCollection ec = service.RetrieveMultiple(query);

                    // Display results.
                    for (int i = 0; i < ec.Entities.Count; i++)
                    {
                        Console.WriteLine("Query Expression Retrieved: {0}", ((Role)ec.Entities[i]).Name);
                    }

                    #endregion
                    #region Retrieve records from an intersect table via Fetch

                    // Setup Fetch XML.
                    StringBuilder linkFetch = new StringBuilder();
                    linkFetch.Append("<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" distinct=\"true\">");
                    linkFetch.Append("<entity name=\"role\">");
                    linkFetch.Append("<attribute name=\"name\"/>");
                    linkFetch.Append("<link-entity name=\"systemuserroles\" from=\"roleid\" to=\"roleid\" visible=\"false\" intersect=\"true\">");
                    linkFetch.Append("<filter type=\"and\">");
                    linkFetch.Append("<condition attribute=\"systemuserid\" operator=\"eq\" value=\"" + userId + "\"/>");
                    linkFetch.Append("</filter>");
                    linkFetch.Append("</link-entity>");
                    linkFetch.Append("</entity>");
                    linkFetch.Append("</fetch>");

                    // Build fetch request and obtain results.
                    var efr = new RetrieveMultipleRequest()
                    {
                        Query = new FetchExpression(linkFetch.ToString())
                    };
                    EntityCollection entityResults = ((RetrieveMultipleResponse)service.Execute(efr)).EntityCollection;

                    // Display results.
                    foreach (var e in entityResults.Entities)
                    {
                        Console.WriteLine("Fetch Retrieved: {0}", e.Attributes["name"]);
                    }

                    #endregion
                    #region Retrieve records from an intersect table via LINQ

                    // Obtain the Organization Context.
                    OrganizationServiceContext context = new OrganizationServiceContext(service);

                    // Create Linq Query.
                    var roles = (from r in context.CreateQuery<Role>()
                                 join s in context.CreateQuery<SystemUserRoles>() on r.RoleId equals s.RoleId
                                 where s.SystemUserId == userId
                                 orderby r.RoleId
                                 select r.Name);

                    // Display results.
                    foreach (var role in roles)
                    {
                        Console.WriteLine("Linq Retrieved: {0}", role);
                    }
                    #endregion Demonstrate
                    #region Clean up
                    // Provides option to delete the ChangeTracking solution
                    CleanUpSample(service);
                    #endregion Clean up
                    //////////////////////////////////////////////
                    #endregion Sample Code

                }
                else
                {
                    const string UNABLE_TO_LOGIN_ERROR = "Unable to Login to Microsoft Dataverse";
                    if (service.LastCrmError.Equals(UNABLE_TO_LOGIN_ERROR))
                    {
                        Console.WriteLine("Check the connection string values in cds/App.config.");
                        throw new Exception(service.LastCrmError);
                    }
                    else
                    {
                        throw service.LastCrmException;
                    }
                }

            }
            catch (Exception ex)
            {
                SampleHelpers.HandleException(ex);
            }

            finally
            {
                if (service != null)
                    service.Dispose();

                Console.WriteLine("Press <Enter> to exit.");
                Console.ReadLine();
            }
        }
    }
}
