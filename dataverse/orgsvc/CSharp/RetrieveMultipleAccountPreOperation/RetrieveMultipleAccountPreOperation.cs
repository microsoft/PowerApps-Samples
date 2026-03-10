using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Xml.Linq;

namespace RetrieveMultipleExample
{
    public class RetrieveMultipleAccountPreOperation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the tracing service
            ITracingService tracingService =
            (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            // The InputParameters collection contains all the data passed in the message request.  
            if (context.InputParameters.Contains("Query"))
            {
                try
                {
                    var thisQuery = context.InputParameters["Query"];
                    var fetchExpressionQuery = thisQuery as FetchExpression;
                    var queryExpressionQuery = thisQuery as QueryExpression;
                    var queryByAttributeQuery = thisQuery as QueryByAttribute;

                    if (fetchExpressionQuery != null)
                    {
                        tracingService.Trace("Found FetchExpression Query");

                        XDocument fetchXmlDoc = XDocument.Parse(fetchExpressionQuery.Query);
                        //The required entity element
                        var entityElement = fetchXmlDoc.Descendants("entity").FirstOrDefault();
                        var entityName = entityElement.Attributes("name").FirstOrDefault().Value;

                        //Only applying to the account entity
                        if (entityName == "account")
                        {
                            tracingService.Trace("Query on Account confirmed");

                            //Get all filter elements
                            var filterElements = entityElement.Descendants("filter");

                            //Find any existing statecode conditions
                            var stateCodeConditions = from c in filterElements.Descendants("condition")
                                                      where c.Attribute("attribute").Value.Equals("statecode")
                                                      select c;

                            if (stateCodeConditions.Count() > 0)
                            {
                                tracingService.Trace("Removing existing statecode filter conditions.");
                            }
                            //Remove statecode conditions
                            stateCodeConditions.ToList().ForEach(x => x.Remove());


                            //Add the condition you want in a new filter
                            entityElement.Add(
                                new XElement("filter",
                                    new XElement("condition",
                                        new XAttribute("attribute", "statecode"),
                                        new XAttribute("operator", "neq"), //not equal
                                        new XAttribute("value", "1") //Inactive
                                        )
                                    )
                                );
                        }


                        fetchExpressionQuery.Query = fetchXmlDoc.ToString();

                    }
                    if (queryExpressionQuery != null)
                    {
                        tracingService.Trace("Found Query Expression Query");
                        if (queryExpressionQuery.EntityName.Equals("account"))
                        {
                            tracingService.Trace("Query on Account confirmed");

                            //Recursively remove any conditions referring to the statecode attribute
                            foreach (FilterExpression fe in queryExpressionQuery.Criteria.Filters)
                            {
                                //Remove any existing criteria based on statecode attribute
                                RemoveAttributeConditions(fe, "statecode", tracingService);
                            }

                            //Define the filter
                            var stateCodeFilter = new FilterExpression();
                            stateCodeFilter.AddCondition("statecode", ConditionOperator.NotEqual, 1);
                            //Add it to the Criteria
                            queryExpressionQuery.Criteria.AddFilter(stateCodeFilter);
                        }

                    }
                    if (queryByAttributeQuery != null)
                    {
                        tracingService.Trace("Found Query By Attribute Query");
                        //Query by attribute doesn't provide a complex query model that 
                        // can be manipulated
                    }
                }

                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in RetrieveMultipleAccountPreOperation.", ex);
                }

                catch (Exception ex)
                {
                    tracingService.Trace("RetrieveMultipleAccountPreOperation: {0}", ex.ToString());
                    throw;
                }
            }
        }

        /// <summary>
        /// Removes any conditions using a specific named attribute
        /// </summary>
        /// <param name="filter">The filter that may have a condition using the attribute</param>
        /// <param name="attributeName">The name of the attribute that should not be used in a condition</param>
        /// <param name="tracingService">The tracing service to use</param>
        private void RemoveAttributeConditions(FilterExpression filter, string attributeName, ITracingService tracingService)
        {

            List<ConditionExpression> conditionsToRemove = new List<ConditionExpression>();

            foreach (ConditionExpression ce in filter.Conditions)
            {
                if (ce.AttributeName.Equals(attributeName))
                {
                    conditionsToRemove.Add(ce);
                }
            }

            conditionsToRemove.ForEach(x =>
            {
                filter.Conditions.Remove(x);
                tracingService.Trace("Removed existing statecode filter conditions.");
            });

            foreach (FilterExpression fe in filter.Filters)
            {
                RemoveAttributeConditions(fe, attributeName, tracingService);
            }
        }


    }
}
