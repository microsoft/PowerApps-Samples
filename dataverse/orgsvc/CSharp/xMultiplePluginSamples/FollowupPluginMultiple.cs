using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel;

namespace xMultiplePluginSamples
{
    /// <summary>
    /// Plugin development guide: https://learn.microsoft.com/powerapps/developer/common-data-service/plug-ins
    /// Best practices and guidance: https://learn.microsoft.com/powerapps/developer/common-data-service/best-practices/business-logic/
    /// </summary>
    public class FollowupPluginMultiple : PluginBase
    {
        public FollowupPluginMultiple(string unsecureConfiguration, string secureConfiguration)
            : base(typeof(FollowupPluginMultiple))
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

            // Check for the Targets parameter containing the entities
            if (context.InputParameters.Contains("Targets") && context.InputParameters["Targets"] is EntityCollection entityCollection)
            {

                // Check for entity name on which this plugin would be registered
                if (entityCollection.EntityName == "sample_example")
                {
                    int count = 0;
                    foreach (Entity entity in entityCollection.Entities)
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
                            new EntityReference(entityCollection.EntityName, entity.Id);
                            // Create the task in Dataverse.
                            localPluginContext.InitiatingUserService.Create(followup);
                        }

                        catch (FaultException<OrganizationServiceFault> ex)
                        {
                            throw new InvalidPluginExecutionException($"FollowupPluginSingle item:{count} OrganizationServiceFault: {ex.Message}", ex);
                        }

                        catch (Exception ex)
                        {
                            throw new InvalidPluginExecutionException($"FollowupPluginSingle item:{count} Exception: {ex.Message}", ex);
                        }
                        count++;
                    }
                    localPluginContext.Trace($"FollowupPluginMultiple created {count} follow up tasks.");
                }
                else
                {
                    localPluginContext.Trace($"Expected EntityCollection.EntityName: 'sample_example'. Actual: '{entityCollection.EntityName}'");
                }
            }
            else
            {
                if (!context.InputParameters.Contains("Targets"))
                    localPluginContext.Trace("Expected InputParameter 'Targets' not found.");
                if (!(context.InputParameters["Targets"] is EntityCollection))
                    localPluginContext.Trace("Expected InputParameter 'Targets' is not EntityCollection.");
            }


        }
    }
}
