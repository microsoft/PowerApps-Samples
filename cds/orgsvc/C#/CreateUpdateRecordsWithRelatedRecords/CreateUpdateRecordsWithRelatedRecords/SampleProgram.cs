using Microsoft.Xrm.Sdk;
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
                    //Define the account for which we will add letters                
                    var accountToCreate = new Account
                    {
                        Name = "Example Account"
                    };

                    //Define the IDs of the related letters we will create
                    _letterIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

                    //This acts as a container for each letter we create. Note that we haven't
                    //define the relationship between the letter and account yet.
                    var relatedLettersToCreate = new EntityCollection
                    {
                        EntityName = Letter.EntityLogicalName,
                        Entities =
                        {
                            new Letter{Subject = "Letter 1", ActivityId = _letterIds[0]},
                            new Letter{Subject = "Letter 2", ActivityId = _letterIds[1]},
                            new Letter{Subject = "Letter 3", ActivityId = _letterIds[2]}
                        }
                    };

                    //Creates the reference between which relationship between Letter and
                    //Account we would like to use.
                    var letterRelationship = new Relationship("Account_Letters");

                    //Adds the letters to the account under the specified relationship
                    accountToCreate.RelatedEntities.Add(letterRelationship, relatedLettersToCreate);

                    //Passes the Account (which contains the letters)
                    _accountId = service.Create(accountToCreate);

                    Console.WriteLine("An account and {0} letters were created.", _letterIds.Length);


                    //Now we run through many of the same steps as the above "Create" example
                    var accountToUpdate = new Account
                    {
                        Name = "Example Account - Updated",
                        AccountId = _accountId
                    };

                    var relatedLettersToUpdate = new EntityCollection
                    {
                        EntityName = Letter.EntityLogicalName,
                        Entities =
                        {
                            new Letter{Subject = "Letter 1 - Updated", ActivityId = _letterIds[0]},
                            new Letter{Subject = "Letter 2 - Updated", ActivityId = _letterIds[1]},
                            new Letter{Subject = "Letter 3 - Updated", ActivityId = _letterIds[2]}
                        }
                    };

                    accountToUpdate.RelatedEntities.Add(letterRelationship, relatedLettersToUpdate);

                    //This will update the account as well as all of the related letters
                    service.Update(accountToUpdate);
                    Console.WriteLine("An account and {0} letters were updated.", _letterIds.Length);
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
