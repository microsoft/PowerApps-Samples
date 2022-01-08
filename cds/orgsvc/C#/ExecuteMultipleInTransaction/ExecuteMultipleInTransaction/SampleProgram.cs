using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        [STAThread] // Required to support the interactive login experience
        static void Main(string[] args)
        {
            CrmServiceClient service = null;
            try
            {
                service = SampleHelpers.Connect("Connect");
                if (service.IsReady)
                {
                    // Create any entity records that the demonstration code requires
                    SetUpSample(service);
                    #region Demonstrate
                    // Create an ExecuteTransactionRequest object.
                    var requestToCreateRecords = new ExecuteTransactionRequest()
                    {
                        // Create an empty organization request collection.
                        Requests = new OrganizationRequestCollection(),
                        ReturnResponses = true
                    };

                    // Create several (local, in memory) entities in a collection. 
                    EntityCollection input = GetCollectionOfEntitiesToCreate();

                    // Add a CreateRequest for each entity to the request collection.
                    foreach (var entity in input.Entities)
                    {
                        CreateRequest createRequest = new CreateRequest { Target = entity };
                        requestToCreateRecords.Requests.Add(createRequest);
                    }

                   
                        var responseForCreateRecords =
                            (ExecuteTransactionResponse)service.Execute(requestToCreateRecords);

                        int i = 0;
                        // Display the results returned in the responses.
                        foreach (var responseItem in responseForCreateRecords.Responses)
                        {
                            if (responseItem != null)
                                DisplayResponse(requestToCreateRecords.Requests[i], responseItem);
                            i++;
                        }


                    Console.WriteLine("Create request failed for the account{0} and the reason being: {1}");
                            
             
                    var requestForUpdates = new ExecuteTransactionRequest()
                    {
                        Requests = new OrganizationRequestCollection()
                    };

                    // Update the entities that were previously created.
                    EntityCollection update = GetCollectionOfEntitiesToUpdate();

                    foreach (var entity in update.Entities)
                    {
                        UpdateRequest updateRequest = new UpdateRequest { Target = entity };
                        requestForUpdates.Requests.Add(updateRequest);
                    }

                    
                        ExecuteTransactionResponse responseForUpdates =
                            (ExecuteTransactionResponse)service.Execute(requestForUpdates);
                        Console.WriteLine("Entity records are updated.");
                    
                        Console.WriteLine("Update request failed for the account{0} and the reason being: {1}");
                    #endregion Demonstrate

                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up
                }
               
            
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
