using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PowerApps.Samples
{
    public class IsSystemAdmin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the tracing service
            ITracingService tracingService =
            (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is EntityReference)
            {

                // Obtain the organization service reference which you will need for  
                // web service calls.  
                IOrganizationServiceFactory serviceFactory =
                    (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                try
                {
                    //Get the userid 
                    Guid userid = ((EntityReference)context.InputParameters["Target"]).Id;

                    //Query systemuserroles first

                    // 627090FF-40A3-4053-8790-584EDC5BE201 is a fixed guid for the System Administrator role
                    ///<seealso cref="https://learn.microsoft.com/powerapps/developer/data-platform/security-roles#standard-role-templates"/>

                    string systemUserRolesFetchXml = $@"<fetch mapping='logical' >
                      <entity name='systemuserroles'>
                        <attribute name='roleid'/>
                        <filter type='and'>
                          <condition attribute='systemuserid' operator='eq' value='{{0}}' /> 
                        </filter>
                        <link-entity name='role' alias='role' to='roleid' link-type='inner'>
                          <filter type='and'>
                            <condition alias='role' attribute='roletemplateid' operator='eq' value='627090FF-40A3-4053-8790-584EDC5BE201' /> </filter>
                        </link-entity>
                      </entity>
                    </fetch>";

                    FetchExpression systemuserrolesQuery = new FetchExpression(string.Format(systemUserRolesFetchXml, userid));

                    EntityCollection systemuserrolesResults = service.RetrieveMultiple(systemuserrolesQuery);

                    if (systemuserrolesResults.Entities.Count > 0)
                    {
                        context.OutputParameters["HasRole"] = true;
                    }
                    else
                    {
                        tracingService.Trace("System Administrator Role not found in systemuserroles");

                        //The user may have the role due to an indirect association from team membership.

                        string teamMemberShipFetchXml = $@"<fetch mapping='logical' >
                          <entity name='teamroles'>
                            <attribute name='roleid'/>
                            <link-entity name='teammembership' to='teamid' from='teamid' link-type='inner'>
                              <filter type='and'>
                                <condition attribute='systemuserid' operator='eq' value='{{0}}' />
                              </filter>
                            </link-entity>
                            <link-entity name='role' alias='role' to='roleid' from='roleid' link-type='inner'>
                              <filter type='and'>
                                <condition alias='role' attribute='roletemplateid' operator='eq' value='627090FF-40A3-4053-8790-584EDC5BE201' />
                              </filter>
                            </link-entity>
                          </entity>
                        </fetch>";

                        FetchExpression teammembershipQuery = new FetchExpression(string.Format(systemUserRolesFetchXml, userid));

                        EntityCollection teammembershipResults = service.RetrieveMultiple(systemuserrolesQuery);
                        if (systemuserrolesResults.Entities.Count > 0)
                        {
                            context.OutputParameters["HasRole"] = true;
                        }
                        else
                        {
                            tracingService.Trace("System Administrator Role not found in teamroles");
                            context.OutputParameters["HasRole"] = false;
                        }

                    }

                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in IsSystemAdmin.", ex);
                }

                catch (Exception ex)
                {
                    tracingService.Trace("IsSystemAdmin: {0}", ex.ToString());
                    throw;
                }

            }
        }
    }
}
