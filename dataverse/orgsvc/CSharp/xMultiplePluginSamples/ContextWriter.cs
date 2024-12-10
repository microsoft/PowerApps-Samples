using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using System.Text;

namespace xMultiplePluginSamples
{
    /// <summary>
    /// Plugin development guide: https://learn.microsoft.com/powerapps/developer/common-data-service/plug-ins
    /// Best practices and guidance: https://learn.microsoft.com/powerapps/developer/common-data-service/best-practices/business-logic/
    /// </summary>
    public class ContextWriter : PluginBase
    {
        public ContextWriter(string unsecureConfiguration, string secureConfiguration)
            : base(typeof(ContextWriter))
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

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"MessageName: {context.MessageName}");
            sb.AppendLine($"PrimaryEntityName: {context.PrimaryEntityName}");
            sb.AppendLine("InputParameters:");
            foreach (string key in context.InputParameters.Keys.OrderBy(k => k))
            {
                switch (key)
                {
                    case "Target":
                        sb.AppendLine($"\tTarget (Entity) :");
                        Entity target = (Entity)context.InputParameters["Target"];
                        sb.AppendLine($"\t\tLogicalName : {target.LogicalName}");
                        sb.AppendLine("\t\tAttributes :");
                        foreach (string attributeKey in target.Attributes.Keys.OrderBy(k => k))
                        {
                            sb.AppendLine($"\t\t\t{attributeKey} : {target.Attributes[attributeKey]}");
                        }

                        break;
                    case "Targets":
                        sb.AppendLine($"\tTargets (EntityCollection) :");
                        EntityCollection targets = (EntityCollection)context.InputParameters["Targets"];
                        sb.AppendLine($"\t\tEntityName : {targets.EntityName}");
                        sb.AppendLine($"\t\tEntities.Count : {targets.Entities.Count}");

                        // Only sampling first record to avoid filling up limited trace space
                        sb.AppendLine($"\t\ttargets.Entities[0] :");
                        Entity firstTarget = targets.Entities[0];
                        sb.AppendLine($"\t\t\tLogicalName : {firstTarget.LogicalName}");
                        sb.AppendLine("\t\t\tAttributes :");
                        foreach (string attributeKey in firstTarget.Attributes.Keys.OrderBy(k => k))
                        {
                            sb.AppendLine($"\t\t\t\t{attributeKey} : {firstTarget.Attributes[attributeKey]}");
                        }
                        break;
                    default:
                        sb.AppendLine($"\t{key} : {context.InputParameters[key]}");
                        break;
                }
            }
            sb.AppendLine("SharedVariables:");
            foreach (string key in context.SharedVariables.Keys.OrderBy(k => k))
            {
                sb.AppendLine($"\t{key} : {context.SharedVariables[key]}");
            }
            sb.AppendLine("PreEntityImages:");
            foreach (string key in context.PreEntityImages.Keys.OrderBy(k => k))
            {
                if (context.PreEntityImages.TryGetValue(key, out Entity preEntity))
                {
                    sb.AppendLine($"\t{key} :");
                    sb.AppendLine($"\t\tLogicalName : {preEntity.LogicalName}");
                    sb.AppendLine("\t\tAttributes :");
                    foreach (string attributeKey in preEntity.Attributes.Keys)
                    {
                        sb.AppendLine($"\t\t\t{attributeKey} : {preEntity.Attributes[attributeKey]}");
                    }
                }
            }
            sb.AppendLine("PostEntityImages:");
            foreach (string key in context.PostEntityImages.Keys.OrderBy(k => k))
            {
                sb.AppendLine($"\t{key} :");
                Entity postEntity = context.PostEntityImages[key];
                sb.AppendLine($"\t\tLogicalName : {postEntity.LogicalName}");
                sb.AppendLine("\t\tAttributes :");
                foreach (string attributeKey in postEntity.Attributes.Keys)
                {
                    sb.AppendLine($"\t\t\t{attributeKey} : {postEntity.Attributes[attributeKey]}");

                }
            }

            // Only sampling first image to avoid filling up limited trace space
            sb.AppendLine("PreEntityImagesCollection[0]:");
            if (context.PreEntityImagesCollection.Length > 0)
            {
                EntityImageCollection firstPreEntityImage = context.PreEntityImagesCollection[0];

                foreach (string key in firstPreEntityImage.Keys.OrderBy(k => k))
                {
                    sb.AppendLine($"\t\t{key} :");
                    Entity preEntity = firstPreEntityImage[key];
                    sb.AppendLine($"\t\t\tLogicalName : {preEntity.LogicalName}");
                    sb.AppendLine("\t\t\tAttributes :");
                    foreach (string attributeKey in preEntity.Attributes.Keys.OrderBy(k => k))
                    {
                        sb.AppendLine($"\t\t\t\t{attributeKey} : {preEntity.Attributes[attributeKey]}");
                    }
                }
            }

            // Only sampling first image to avoid filling up limited trace space
            sb.AppendLine("PostEntityImagesCollection[0]:");
            if (context.PostEntityImagesCollection.Length > 0)
            {
                EntityImageCollection firstPostEntityImage = context.PostEntityImagesCollection[0];

                foreach (string key in firstPostEntityImage.Keys.OrderBy(k => k))
                {
                    sb.AppendLine($"\t\t{key} :");
                    Entity postEntity = firstPostEntityImage[key];
                    sb.AppendLine($"\t\t\tLogicalName : {postEntity.LogicalName}");
                    sb.AppendLine("\t\t\tAttributes :");
                    foreach (string attributeKey in postEntity.Attributes.Keys.OrderBy(k => k))
                    {
                        sb.AppendLine($"\t\t\t\t{attributeKey} : {postEntity.Attributes[attributeKey]}");
                    }
                }
            }

            sb.AppendLine("OutputParameters:");
            foreach (string key in context.OutputParameters.Keys.OrderBy(k => k))
            {
                switch (key)
                {
                    case "id":
                        sb.AppendLine($"\tid: {context.OutputParameters["id"]}");
                        break;
                    case "Ids":
                        Guid[] outputIds = (Guid[])context.OutputParameters["Ids"];
                        if (outputIds.Length > 0)
                        {
                            sb.AppendLine($"\tIds[0]: {((Guid[])context.OutputParameters["Ids"])[0]}");
                        }
                        break;
                    default:
                        sb.AppendLine($"\t{key} : {context.InputParameters[key]}");
                        break;
                }
            }

            localPluginContext.Trace(sb.ToString());


        }
    }
}
