using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel;

namespace xMultiplePluginSamples
{
    /// <summary>
    /// Plugin development guide: https://learn.microsoft.com/powerapps/developer/common-data-service/plug-ins
    /// Best practices and guidance: https://learn.microsoft.com/powerapps/developer/common-data-service/best-practices/business-logic/
    /// </summary>
    public class FollowupPluginSingle : PluginBase
    {
        public FollowupPluginSingle(string unsecureConfiguration, string secureConfiguration)
            : base(typeof(FollowupPluginSingle))
        {
            // TODO: Implement your custom configuration handling
            // https://learn.microsoft.com/powerapps/developer/common-data-service/register-plug-in#set-configuration-data
        }

        // Entry point for custom business logic execution
        protected override void ExecuteDataversePlugin(ILocalPluginContext localPluginContext)
        {
            if (localPluginContext == null)
            {
                throw new ArgumentNullException(nameof(localPluginContext));
            }

            // Update default PluginBase.cs and replace IPluginExecutionContext with IPluginExecutionContext4.
            IPluginExecutionContext4 context = localPluginContext.PluginExecutionContext;

            // Custom business logic starts here

            // Check for the entity and stage on which the plugin would be registered
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity entity && context.Stage == 40)
            {

                // Check for entity name on which this plugin would be registered
                if (entity.LogicalName == "sample_example")
                {
                    try
                    {

                        // Create a task activity to follow up with the sample_example in 7 days. 
                        Entity followup = new Entity("task");

                        followup["subject"] = "Example task record";
                        followup["description"] = "Description of example task record.";
                        followup["scheduledstart"] = DateTime.Now.AddDays(7);
                        followup["scheduledend"] = DateTime.Now.AddDays(7);
                        followup["regardingobjectid"] =
                        new EntityReference(entity.LogicalName, entity.Id);


                        // Create the task in Dataverse.
                        localPluginContext.Trace("FollowupPluginSingle: Creating the task activity...");
                        localPluginContext.InitiatingUserService.Create(followup);
                        localPluginContext.Trace("FollowupPluginSingle: Success");

                    }

                    catch (FaultException<OrganizationServiceFault> ex)
                    {
                        throw new InvalidPluginExecutionException($"FollowupPluginSingle OrganizationServiceFault: {ex.Message}", ex);
                    }

                    catch (Exception ex)
                    {
                        throw new InvalidPluginExecutionException($"FollowupPluginSingle Exception: {ex.Message}", ex);
                    }
                }
                else
                {
                    localPluginContext.Trace($"Expected Entity.LogicalName: 'sample_example'. Actual: '{entity.LogicalName}'");
                }
            }
            else
            {
                if (!context.InputParameters.Contains("Target"))
                    localPluginContext.Trace("Expected InputParameter 'Target' not found.");
                if (!(context.InputParameters["Target"] is Entity))
                    localPluginContext.Trace("Expected InputParameter 'Target' is not an Entity.");
                if (context.Stage != 40)
                    localPluginContext.Trace($"Expected Stage: 40. Actual: {context.Stage}.");
            }



        }
    }
}
