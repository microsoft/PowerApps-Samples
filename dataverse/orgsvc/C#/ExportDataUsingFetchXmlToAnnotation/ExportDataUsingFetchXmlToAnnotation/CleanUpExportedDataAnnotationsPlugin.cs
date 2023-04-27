using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace PowerApps.Samples
{
    public class CleanUpExportedDataAnnotationsPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the tracing service
            var tracingService =
            (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            var context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            try
            {
                var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                var organizationService = serviceFactory.CreateOrganizationService(context.UserId);

                var queryExpression = new QueryExpression("annotation");
                queryExpression.Criteria.Filters.Add(new FilterExpression()
                {
                    Conditions = {
                    new ConditionExpression("subject", ConditionOperator.Equal, "Export Data Using FetchXml To Csv"),
                    new ConditionExpression("filename", ConditionOperator.Equal, "exportdatausingfetchxml.csv")
                }
                });

                var recordsToDelete = organizationService.RetrieveMultiple(queryExpression);
                
                foreach(var record in recordsToDelete.Entities)
                {
                    organizationService.Delete("annotation", record.Id);
                }
                tracingService.Trace("Records deleted successfully");
            }
            catch(Exception error)
            {
                tracingService.Trace(error.Message);
                throw;
            }
        }
    }
}
