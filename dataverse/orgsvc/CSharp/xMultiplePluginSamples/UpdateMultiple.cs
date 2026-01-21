using Microsoft.Xrm.Sdk;
using System;

namespace xMultiplePluginSamples
{
    /// <summary>
    /// Plugin development guide: https://learn.microsoft.com/powerapps/developer/common-data-service/plug-ins
    /// Best practices and guidance: https://learn.microsoft.com/powerapps/developer/common-data-service/best-practices/business-logic/
    /// </summary>
    public class UpdateMultiple : PluginBase
    {
        public UpdateMultiple(string unsecureConfiguration, string secureConfiguration)
            : base(typeof(UpdateMultiple))
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

            if (context.PrimaryEntityName == "sample_example" && context.MessageName == "UpdateMultiple" && context.Stage == 20)
                {

                // Verify input parameters
                if (context.InputParameters.Contains("Targets") && context.InputParameters["Targets"] is EntityCollection entityCollection)
                {
                    // Verify expected entity images from step registration
                    if (context.PreEntityImagesCollection.Length == entityCollection.Entities.Count)
                    {
                        int count = 0;
                        foreach (Entity entity in entityCollection.Entities)
                        {
                            EntityImageCollection entityImages = context.PreEntityImagesCollection[count];

                            // Verify expected entity image from step registration
                            if (entityImages.TryGetValue("example_preimages", out Entity preImage))
                            {
                                bool entityContainsSampleName = entity.Contains("sample_name");
                                bool entityImageContainsSampleName = preImage.Contains("sample_name");
                                bool entityImageContainsSampleDescription = preImage.Contains("sample_description");

                                if (entityContainsSampleName && entityImageContainsSampleName && entityImageContainsSampleDescription)
                                {
                                    // Verify that the entity 'sample_name' values are different
                                    if (entity["sample_name"] != preImage["sample_name"])
                                    {
                                        string oldName = (string)preImage["sample_name"];
                                        string newName = (string)entity["sample_name"];
                                        
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

                                        // Not tracing Success for brevity. There is a limit to what tracelog can display.
                                        // localPluginContext.Trace($"Appended to 'sample_description': \"{message}\" for item {count} ");
                                    }
                                    else
                                    {
                                        localPluginContext.Trace($"Expected entity and preImage 'sample_name' values to be different. Both are {entity["sample_name"]} for item {count}");
                                    }
                                }
                                else
                                {
                                    if (!entityContainsSampleName)
                                        localPluginContext.Trace($"Expected entity sample_name attribute not found for item {count}.");
                                    if (!entityImageContainsSampleName)
                                        localPluginContext.Trace($"Expected preImage entity sample_name attribute not found for item {count}.");
                                    if (!entityImageContainsSampleDescription)
                                        localPluginContext.Trace($"Expected preImage entity sample_description attribute not found for item {count}.");
                                }
                            }
                            else
                            {
                                localPluginContext.Trace($"Expected PreEntityImage: 'example_preimages' not found for item {count}.");
                            }

                            count++;
                        }
                    }
                    else
                    {
                        localPluginContext.Trace($"Expected PreEntityImagesCollection to contain Entity images for each Entity.");
                    }
                }
                else
                {
                    if (!context.InputParameters.Contains("Targets"))
                        localPluginContext.Trace($"Expected InputParameter: 'Targets' not found.");
                    if (!(context.InputParameters["Targets"] is EntityCollection))
                        localPluginContext.Trace($"Expected InputParameter: 'Targets' is not EntityCollection.");
                }
            }
            else
            {
                if (context.PrimaryEntityName != "sample_example")
                    localPluginContext.Trace($"Expected PrimaryEntityName: 'sample_example'. Actual: '{context.PrimaryEntityName}'.");

                if (context.MessageName != "UpdateMultiple")
                    localPluginContext.Trace($"Expected MessageName: 'UpdateMultiple'. Actual: '{context.MessageName}'.");

                if (context.Stage != 20)
                    localPluginContext.Trace($"Expected Stage: 20. Actual: {context.Stage}.");
            }
        }
    }
}
