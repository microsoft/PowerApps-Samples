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
                    // Associate the accounts to the contact record.

                    // Create a collection of the entities that will be 
                    // associated to the contact.
                    var relatedEntities = new EntityReferenceCollection();
                    relatedEntities.Add(new EntityReference(Account.EntityLogicalName, _account1Id));
                    relatedEntities.Add(new EntityReference(Account.EntityLogicalName, _account2Id));
                    relatedEntities.Add(new EntityReference(Account.EntityLogicalName, _account3Id));

                    // Create an object that defines the relationship between the contact and account.
                    var relationship = new Relationship("account_primary_contact");


                    //Associate the contact with the 3 accounts.
                    service.Associate(Contact.EntityLogicalName, _contactId, relationship,
                        relatedEntities);

                    Console.WriteLine("The entities have been associated.");

                    //Disassociate the records.
                    service.Disassociate(Contact.EntityLogicalName, _contactId, relationship,
                        relatedEntities);

                    Console.WriteLine("The entities have been disassociated.");
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
