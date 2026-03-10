using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
   public partial class SampleProgram
    {
        [STAThread] // Added to support UX

        static void Main(string[] args)
        {

            CrmServiceClient service = null;
            try
            {

                service = SampleHelpers.Connect("Connect");
                if (service.IsReady)
                {
                    #region Sample Code
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate
                    // Grab all Solution Components for a solution.
                    QueryByAttribute componentQuery = new QueryByAttribute
                    {
                        EntityName = SolutionComponent.EntityLogicalName,
                        ColumnSet = new ColumnSet("componenttype", "objectid", "solutioncomponentid", "solutionid"),
                        Attributes = { "solutionid" },

                        // In your code, this value would probably come from another query.
                        Values = { _primarySolutionId }
                    };

                    IEnumerable<SolutionComponent> allComponents =
                        service.RetrieveMultiple(componentQuery).Entities.Cast<SolutionComponent>();

                    foreach (SolutionComponent component in allComponents)
                    {
                        // For each solution component, retrieve all dependencies for the component.
                        RetrieveDependentComponentsRequest dependentComponentsRequest =
                            new RetrieveDependentComponentsRequest
                            {
                                ComponentType = component.ComponentType.Value,
                                ObjectId = component.ObjectId.Value
                            };
                        RetrieveDependentComponentsResponse dependentComponentsResponse =
                            (RetrieveDependentComponentsResponse)service.Execute(dependentComponentsRequest);

                        // If there are no dependent components, we can ignore this component.
                        if (dependentComponentsResponse.EntityCollection.Entities.Any() == false)
                            continue;

                        // If there are dependencies upon this solution component, and the solution
                        // itself is managed, then you will be unable to delete the solution.
                        Console.WriteLine("Found {0} dependencies for Component {1} of type {2}",
                            dependentComponentsResponse.EntityCollection.Entities.Count,
                            component.ObjectId.Value,
                            component.ComponentType.Value
                            );
                        //A more complete report requires more code
                        foreach (Dependency d in dependentComponentsResponse.EntityCollection.Entities)
                        {
                            DependencyReport(service, d);
                        }
                    }

                    //Find out if any dependencies on a  specific global option set would prevent it from being deleted
                    // Use the RetrieveOptionSetRequest message to retrieve  
                    // a global option set by it's name.
                    RetrieveOptionSetRequest retrieveOptionSetRequest =
                        new RetrieveOptionSetRequest
                        {
                            Name = _globalOptionSetName
                        };

                    // Execute the request.
                    RetrieveOptionSetResponse retrieveOptionSetResponse =
                        (RetrieveOptionSetResponse)service.Execute(
                        retrieveOptionSetRequest);
                    _globalOptionSetId = retrieveOptionSetResponse.OptionSetMetadata.MetadataId;
                    if (_globalOptionSetId != null)
                    {
                        //Use the global OptionSet MetadataId with the appropriate componenttype
                        // to call RetrieveDependenciesForDeleteRequest
                        RetrieveDependenciesForDeleteRequest retrieveDependenciesForDeleteRequest = new RetrieveDependenciesForDeleteRequest
                        {
                            ComponentType = (int)componenttype.OptionSet,
                            ObjectId = (Guid)_globalOptionSetId
                        };

                        RetrieveDependenciesForDeleteResponse retrieveDependenciesForDeleteResponse =
                         (RetrieveDependenciesForDeleteResponse)service.Execute(retrieveDependenciesForDeleteRequest);
                        Console.WriteLine("");
                        foreach (Dependency d in retrieveDependenciesForDeleteResponse.EntityCollection.Entities)
                        {

                            if (d.DependentComponentType.Value == 2)//Just testing for Attributes
                            {
                                String attributeLabel = "";
                                RetrieveAttributeRequest retrieveAttributeRequest = new RetrieveAttributeRequest
                                {
                                    MetadataId = (Guid)d.DependentComponentObjectId
                                };
                                RetrieveAttributeResponse retrieveAttributeResponse = (RetrieveAttributeResponse)service.Execute(retrieveAttributeRequest);

                                AttributeMetadata attmet = retrieveAttributeResponse.AttributeMetadata;

                                attributeLabel = attmet.DisplayName.UserLocalizedLabel.Label;

                                Console.WriteLine("An {0} named {1} will prevent deleting the {2} global option set.",
                               (componenttype)d.DependentComponentType.Value,
                               attributeLabel,
                               _globalOptionSetName);
                            }
                        }
                    }

                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clen up

                    //DeleteRequiredRecords(promptForDelete);
                }
                #endregion Demonstrate
                #endregion Sample Code

                else
                {
                    const string UNABLE_TO_LOGIN_ERROR = "Unable to Login to Microsoft Dataverse";
                    if (service.LastCrmError.Equals(UNABLE_TO_LOGIN_ERROR))
                    {
                        Console.WriteLine("Check the connection string values in cds/App.config.");
                        throw new Exception(service.LastCrmError);
                    }
                    else
                    {
                        throw service.LastCrmException;
                    }
                }
            }
            catch (Exception ex)
            {
                SampleHelpers.HandleException(ex);
            }

            finally
            {
                if (service != null)
                    service.Dispose();

                Console.WriteLine("Press <Enter> to exit.");
                Console.ReadLine();
            }

        }
    }
}
