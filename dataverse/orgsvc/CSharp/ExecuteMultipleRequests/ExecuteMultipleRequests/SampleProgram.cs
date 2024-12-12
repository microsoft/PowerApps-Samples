using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System;

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
                    //////////////////////////////////////////////
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate

                    // Create an ExecuteMultipleRequest object.
                   var requestWithResults = new ExecuteMultipleRequest()
                    {
                        // Assign settings that define execution behavior: continue on error, return responses. 
                        Settings = new ExecuteMultipleSettings()
                        {
                            ContinueOnError = false,
                            ReturnResponses = true
                        },
                        // Create an empty organization request collection.
                        Requests = new OrganizationRequestCollection()
                    };

                    // Create several (local, in memory) entities in a collection. 
                    EntityCollection input = GetCollectionOfEntitiesToCreate();

                    // Add a CreateRequest for each entity to the request collection.
                    foreach (var entity in input.Entities)
                    {
                        var createRequest = new CreateRequest { Target = entity };
                        requestWithResults.Requests.Add(createRequest);
                    }

                    // Execute all the requests in the request collection using a single web method call.
                    var responseWithResults =
                        (ExecuteMultipleResponse)service.Execute(requestWithResults);

                    // Display the results returned in the responses.
                    foreach (var responseItem in responseWithResults.Responses)
                    {
                        // A valid response.
                        if (responseItem.Response != null)
                            DisplayResponse(requestWithResults.Requests[responseItem.RequestIndex], responseItem.Response);

                        // An error has occurred.
                        else if (responseItem.Fault != null)
                            DisplayFault(requestWithResults.Requests[responseItem.RequestIndex],
                                responseItem.RequestIndex, responseItem.Fault);
                    }
                    #endregion Execute Multiple with Results


                    #region Execute Multiple with No Results

                    var requestWithNoResults = new ExecuteMultipleRequest()
                    {
                        // Set the execution behavior to not continue after the first error is received
                        // and to not return responses.
                        Settings = new ExecuteMultipleSettings()
                        {
                            ContinueOnError = false,
                            ReturnResponses = false
                        },
                        Requests = new OrganizationRequestCollection()
                    };

                    // Update the entities that were previously created.
                    EntityCollection update = GetCollectionOfEntitiesToUpdate();

                    foreach (var entity in update.Entities)
                    {
                        var updateRequest = new UpdateRequest { Target = entity };
                        requestWithNoResults.Requests.Add(updateRequest);
                    }

                    ExecuteMultipleResponse responseWithNoResults =
                        (ExecuteMultipleResponse)service.Execute(requestWithNoResults);

                    // There should be no responses unless there was an error. Only the first error 
                    // should be returned. That is the behavior defined in the settings.
                    if (responseWithNoResults.Responses.Count > 0)
                    {
                        foreach (var responseItem in responseWithNoResults.Responses)
                        {
                            if (responseItem.Fault != null)
                                DisplayFault(requestWithNoResults.Requests[responseItem.RequestIndex],
                                    responseItem.RequestIndex, responseItem.Fault);
                        }
                    }
                    else
                    {
                        Console.WriteLine("All account records have been updated successfully.");
                    }

                    #endregion Execute Multiple with No Results


                    #region Execute Multiple with Continue On Error

                    var requestWithContinueOnError = new ExecuteMultipleRequest()
                    {
                        // Set the execution behavior to continue on an error and not return responses.
                        Settings = new ExecuteMultipleSettings()
                        {
                            ContinueOnError = true,
                            ReturnResponses = false
                        },
                        Requests = new OrganizationRequestCollection()
                    };

                    // Update the entities but introduce some bad attribute values so we get errors.
                    EntityCollection updateWithErrors = GetCollectionOfEntitiesToUpdateWithErrors();

                    foreach (var entity in updateWithErrors.Entities)
                    {
                        UpdateRequest updateRequest = new UpdateRequest { Target = entity };
                        requestWithContinueOnError.Requests.Add(updateRequest);
                    }

                    var responseWithContinueOnError =
                        (ExecuteMultipleResponse)service.Execute(requestWithContinueOnError);

                    // There should be no responses except for those that contain an error. 
                    if (responseWithContinueOnError.Responses.Count > 0)
                    {
                        if (responseWithContinueOnError.Responses.Count < requestWithContinueOnError.Requests.Count)
                        {
                            Console.WriteLine("Response collection contain a mix of successful response objects and errors.");
                        }
                        foreach (var responseItem in responseWithContinueOnError.Responses)
                        {
                            if (responseItem.Fault != null)
                                DisplayFault(requestWithContinueOnError.Requests[responseItem.RequestIndex],
                                    responseItem.RequestIndex, responseItem.Fault);
                        }
                    }
                    else
                    {
                        // No errors means all transactions are successful.
                        Console.WriteLine("All account records have been updated successfully.");
                    }

                    #endregion Execute Multiple with Continue On Error

                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up
                }
                #endregion Demonstrate

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
