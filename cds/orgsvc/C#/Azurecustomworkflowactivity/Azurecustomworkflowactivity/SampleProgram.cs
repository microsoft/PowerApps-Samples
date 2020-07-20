using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;

namespace PowerApps.Samples
{
    /// <summary>
    /// This class posts the CDS execution context to the Azure Service Bus.
    /// </summary>
    public class AzureAwareWorkflowActivity : CodeActivity
    {
        /// <summary>
        /// This method is called when the workflow executes.
        /// </summary>
        /// <param name="executionContext">The data for the event triggering
        /// the workflow.</param>
        protected override void Execute(CodeActivityContext executionContext)
        {
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            IServiceEndpointNotificationService endpointService =
                     executionContext.GetExtension<IServiceEndpointNotificationService>();
            endpointService.Execute(ServiceEndpoint.Get(executionContext), context);
        }

        /// <summary>
        /// Enables the service endpoint GUID to be provided when this activity is added as a 
        /// step in a workflow.
        /// </summary>
        [RequiredArgument]
        [ReferenceTarget("serviceendpoint")]
        [Input("Input id")]
        public InArgument<EntityReference> ServiceEndpoint { get; set; }
    }
}
