using Microsoft.Xrm.Sdk;
using System;

namespace xMultiplePluginSamples
{
    /// <summary>
    /// Plugin development guide: https://learn.microsoft.com/powerapps/developer/common-data-service/plug-ins
    /// Best practices and guidance: https://learn.microsoft.com/powerapps/developer/common-data-service/best-practices/business-logic/
    /// </summary>
    public class CreateMultiplePreOp : PluginBase
    {
        public CreateMultiplePreOp(string unsecureConfiguration, string secureConfiguration)
            : base(typeof(CreateMultiplePreOp))
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

            if (context.PrimaryEntityName == "sample_example" && context.MessageName == "CreateMultiple" && context.Stage == 20)
            {
                // If the tag value is not present, there is no reason to continue.
                if (context.SharedVariables.TryGetValue("tag", out string tagValue))
                {
                    if (context.InputParameters.Contains("Targets") && context.InputParameters["Targets"] is EntityCollection entityCollection)
                    {
                        if (entityCollection.EntityName == "sample_example")
                        {
                            int count = 0;
                            string value = $"'tag' value for Create = '{tagValue}'.";
                            foreach (Entity entity in entityCollection.Entities)
                            {
                                entity["sample_description"] = value;
                                count++;
                            }
                            // One trace for all items
                            localPluginContext.Trace($"Set 'sample_description' to: \"{value}\" for {count} records.");
                        }
                        else
                        {
                            localPluginContext.Trace($"Expected EntityCollection.EntityName: 'sample_example'. Actual: {entityCollection.EntityName}");
                        }
                    }
                    else
                    {
                        localPluginContext.Trace($"Expected InputParameter 'Targets' not found.");
                    }
                }
                else
                {
                    localPluginContext.Trace($"Expected SharedVariable 'tag' not found.");
                }
            }
            else
            {
                if (context.PrimaryEntityName != "sample_example")
                    localPluginContext.Trace($"Expected PrimaryEntityName: 'sample_example'. Actual: '{context.PrimaryEntityName}'.");

                if (context.MessageName != "CreateMultiple")
                    localPluginContext.Trace($"Expected MessageName: 'CreateMultiple'. Actual: '{context.MessageName}'.");

                if (context.Stage != 20)
                    localPluginContext.Trace($"Expected Stage: 20. Actual: {context.Stage}.");
            }
        }
    }
}
