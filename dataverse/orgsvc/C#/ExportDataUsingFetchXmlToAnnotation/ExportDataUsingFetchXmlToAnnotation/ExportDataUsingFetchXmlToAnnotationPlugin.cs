using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Text;
using System.Xml;

namespace PowerApps.Samples
{
    public class ExportDataUsingFetchXmlToAnnotationPlugin : IPlugin
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
                if (context.InputParameters.Contains("FetchXml"))
                {
                    var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    var organizationService = serviceFactory.CreateOrganizationService(context.UserId);
                    var fetchedRecords = FetchAllDataFromFetchXml(context.InputParameters["FetchXml"].ToString(), organizationService, tracingService);

                    var entityDataToCsvHelper = new EntityDataToCsvHelper(fetchedRecords);
                    var csvString = entityDataToCsvHelper.ConvertToCsv();
                    context.OutputParameters["AnnotationId"] = CreateAnnotationEntity(organizationService, csvString);
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw new InvalidPluginExecutionException("An error occurred in ExportDataUsingFetchXmlToAnnotationPlugin.", ex);
            }
            catch (Exception error)
            {
                tracingService.Trace(error.Message);
                throw;
            }
        }


        /// <summary>
        /// Retrieves a list or records defined by a FetchXml query
        /// </summary>
        /// <param name="fetchXml">The fetchXml query definition.</param>
        /// <param name="organizationService">The service implementing IOrganizationService interface.</param>
        /// <param name="tracingService">The tracing service to log errors.</param>
        /// <returns></returns>
        private List<Entity> FetchAllDataFromFetchXml(string fetchXml, IOrganizationService organizationService, ITracingService tracingService)
        {
            int numberOfSuccessfulFetches = 0;
            try
            {
                List<Entity> finalValue = new List<Entity>();
                EntityCollection result = organizationService.RetrieveMultiple(new FetchExpression(fetchXml));
                numberOfSuccessfulFetches++;
                finalValue.AddRange(result.Entities);

                while (result.MoreRecords)
                {
                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(fetchXml);
                    var attributes = xmlDoc.DocumentElement.Attributes;
                    if (result.PagingCookie != null)
                    {
                        var cookieAttribute = xmlDoc.CreateAttribute("paging-cookie");
                        cookieAttribute.Value = result.PagingCookie;
                        attributes.SetNamedItem(cookieAttribute);
                    }

                    var pageAttribute = attributes.GetNamedItem("page");
                    if (pageAttribute != null)
                    {
                        pageAttribute.Value = Convert.ToString(Convert.ToInt32(pageAttribute.Value) + 1);
                    }
                    else
                    {
                        pageAttribute = xmlDoc.CreateAttribute("page");
                        pageAttribute.Value = "2";
                    }

                    attributes.SetNamedItem(pageAttribute);

                    using (var stringWriter = new StringWriter())
                    using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                    {
                        xmlDoc.WriteTo(xmlTextWriter);
                        xmlTextWriter.Flush();
                        result = organizationService.RetrieveMultiple(new FetchExpression(stringWriter.GetStringBuilder().ToString()));
                        numberOfSuccessfulFetches++;
                        finalValue.AddRange(result.Entities);
                    }
                }

                tracingService.Trace($"Number of pages fetched : {numberOfSuccessfulFetches}");
                return finalValue;
            }
            catch (Exception ex)
            {
                tracingService.Trace($"Number of pages fetched successfully : {numberOfSuccessfulFetches}");
                tracingService.Trace("Failure In Fetching All Data from Fetch Xml : " + ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Creates an annotation record
        /// </summary>
        /// <param name="organizationService">The service implementing IOrganizationService interface.</param>
        /// <param name="csvString">A string containing the data of a CSV file</param>
        /// <returns>The id of the annotation record.</returns>
        private Guid CreateAnnotationEntity(IOrganizationService organizationService, string csvString)
        {
            Entity attachment = new Entity("annotation");
            attachment["subject"] = "Export Data Using FetchXml To Csv";
            attachment["documentbody"] = Convert.ToBase64String(Encoding.ASCII.GetBytes(csvString));
            attachment["filename"] = "exportdatausingfetchxml.csv";
            attachment["mimetype"] = "text/csv";
            return organizationService.Create(attachment);
        }
    }
}
