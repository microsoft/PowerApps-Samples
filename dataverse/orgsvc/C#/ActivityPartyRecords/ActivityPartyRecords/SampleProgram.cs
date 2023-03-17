using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
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
                    ////////////////////////////////////
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate

                    var orgContext = new OrganizationServiceContext(service);
                    // Retrieve the Contact records that we created previously.
                    List<Contact> contacts = (from c in orgContext.CreateQuery<Contact>()
                                              where c.Address1_City == "Sammamish"
                                              select new Contact
                                              {
                                                  ContactId = c.ContactId,
                                                  FirstName = c.FirstName,
                                                  LastName = c.LastName
                                              }).ToList<Contact>();
                    Console.Write("Contacts retrieved, ");

                    // Create an Activity Party record for each Contact.
                    var activityParty1 = new ActivityParty
                    {
                        PartyId = new EntityReference(Contact.EntityLogicalName,
                            contacts[0].ContactId.Value),
                    };

                    var activityParty2 = new ActivityParty
                    {
                        PartyId = new EntityReference(Contact.EntityLogicalName,
                            contacts[1].ContactId.Value),
                    };

                    var activityParty3 = new ActivityParty
                    {
                        PartyId = new EntityReference(Contact.EntityLogicalName,
                            contacts[2].ContactId.Value),
                    };

                    Console.Write("ActivityParty instances created, ");

                    // Create Letter Activity and set From and To fields to the
                    // respective Activity Party entities.
                    var letter = new Letter
                    {
                        RegardingObjectId = new EntityReference(Contact.EntityLogicalName,
                            contacts[2].ContactId.Value),
                        Subject = "Sample Letter Activity",
                        ScheduledEnd = DateTime.Now + TimeSpan.FromDays(5),
                        Description = @"Greetings, Mr. Andreshak,

This is a sample letter activity as part of the SDK Samples.

Sincerely,
Mary Kay Andersen

cc: Denise Smith",
                        From = new ActivityParty[] { activityParty1 },
                        To = new ActivityParty[] { activityParty3, activityParty2 }
                    };
                    orgContext.AddObject(letter);
                    orgContext.SaveChanges();

                    Console.WriteLine("and Letter Activity created.");

                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up

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
