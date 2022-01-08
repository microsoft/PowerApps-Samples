using Microsoft.Xrm.Sdk;
using System;

namespace PowerApps.Samples
{
    /// <summary>
    /// A custom plug-in that can post the execution context of the current message to the Windows
    /// Azure Service Bus. The plug-in also demonstrates tracing which assist with
    /// debugging for plug-ins that are registered in the sandbox.
    /// </summary>
    /// <remarks>This sample requires that a service endpoint be created first, and its ID passed
    /// to the plug-in constructor through the unsecure configuration parameter when the plug-in
    /// step is registered.</remarks>
    public sealed class SandboxPlugin : IPlugin
    {
        private Guid serviceEndpointId;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SandboxPlugin(string config)
        {
            if (String.IsNullOrEmpty(config) || !Guid.TryParse(config, out serviceEndpointId))
            {
                throw new InvalidPluginExecutionException("Service endpoint ID should be passed as config.");
            }
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            // Retrieve the execution context.
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Extract the tracing service.
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            if (tracingService == null)
                throw new InvalidPluginExecutionException("Failed to retrieve the tracing service.");

            IServiceEndpointNotificationService cloudService = (IServiceEndpointNotificationService)serviceProvider.GetService(typeof(IServiceEndpointNotificationService));
            if (cloudService == null)
                throw new InvalidPluginExecutionException("Failed to retrieve the service bus service.");

            try
            {
                tracingService.Trace("Posting the execution context.");
                string response = cloudService.Execute(new EntityReference("serviceendpoint", serviceEndpointId), context);
                if (!String.IsNullOrEmpty(response))
                {
                    tracingService.Trace("Response = {0}", response);
                }
                tracingService.Trace("Done.");
            }
            catch (Exception e)
            {
                tracingService.Trace("Exception: {0}", e.ToString());
                throw;
            }
        }
    }
}
