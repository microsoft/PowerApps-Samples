using Microsoft.Xrm.Sdk;
using System;

namespace xMultiplePluginSamples
{
    /// <summary>
    /// Plugin development guide: https://learn.microsoft.com/powerapps/developer/common-data-service/plug-ins
    /// Best practices and guidance: https://learn.microsoft.com/powerapps/developer/common-data-service/best-practices/business-logic/
    /// </summary>
    public class UpdateSingle : PluginBase
    {
        public UpdateSingle(string unsecureConfiguration, string secureConfiguration)
            : base(typeof(UpdateSingle))
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

            // Verify correct registration 
            if (context.PrimaryEntityName == "sample_example" && context.MessageName == "Update" && context.Stage == 20)
            {
                // Verify input parameters
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity entity)
                {

                    // Verify expected entity image from step registration
                    if (context.PreEntityImages.TryGetValue("example_preimage", out Entity preImage))
                    {

                        bool entityContainsSampleName = entity.Contains("sample_name");
                        bool entityImageContainsSampleName = preImage.Contains("sample_name");
                        bool entityImageContainsSampleDescription = preImage.Contains("sample_description");

                        if (entityContainsSampleName && entityImageContainsSampleName && entityImageContainsSampleDescription)
                        {
                            // Verify that the entity 'sample_name' values are different
                            if (entity["sample_name"] != preImage["sample_name"])
                            {
                                string newName = (string)entity["sample_name"];
                                string oldName = (string)preImage["sample_name"];
                                string message = $"\\r\\n - 'sample_name' changed from '{oldName}' to '{newName}'.";

                                // If the 'sample_description' is included in the update, do not overwrite it, just append to it.
                                if (entity.Contains("sample_description"))
                                {

                                    entity["sample_description"] = entity["sample_description"] += message;

                                }
                                else // The sample description is not included in the update, overwrite with current value + addition.
                                {
                                    entity["sample_description"] = preImage["sample_description"] += message;
                                }

                                // Success:
                                localPluginContext.Trace($"Appended to 'sample_description': \"{message}\" ");
                            }
                            else
                            {
                                localPluginContext.Trace($"Expected entity and preImage 'sample_name' values to be different. Both are {entity["sample_name"]}");
                            }
                        }
                        else
                        {
                            if (!entityContainsSampleName)
                                localPluginContext.Trace("Expected entity sample_name attribute not found.");
                            if (!entityImageContainsSampleName)
                                localPluginContext.Trace("Expected preImage entity sample_name attribute not found.");
                            if (!entityImageContainsSampleDescription)
                                localPluginContext.Trace("Expected preImage entity sample_description attribute not found.");
                        }
                    }
                    else
                    {
                        localPluginContext.Trace($"Expected PreEntityImage: 'example_preimage' not found.");
                    }
                }
                else
                {
                    if (!context.InputParameters.Contains("Target"))
                        localPluginContext.Trace($"Expected InputParameter: 'Target' not found.");
                    if (!(context.InputParameters["Target"] is Entity))
                        localPluginContext.Trace($"Expected InputParameter: 'Target' is not Entity.");
                }
            }
            else
            {
                if (context.PrimaryEntityName != "sample_example")
                    localPluginContext.Trace($"Expected PrimaryEntityName: 'sample_example'. Actual: '{context.PrimaryEntityName}'.");
                if (context.MessageName != "Update")
                    localPluginContext.Trace($"Expected MessageName: 'Update'. Actual: '{context.MessageName}'.");
                if (context.Stage != 20)
                    localPluginContext.Trace($"Expected Stage: 20. Actual: {context.Stage}.");
            }
        }
    }
}
