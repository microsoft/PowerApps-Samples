using Microsoft.Xrm.Sdk;
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
                    //////////////////////////////////////////////
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate

                    Console.WriteLine("Entering:RetrieveMultipleWithRelatedEntityColumns");
                    
                    //Create multiple accounts with primary contacts  
                    var contact = new Entity("contact");
                    contact.Attributes["firstname"] = "ContactFirstName";
                    contact.Attributes["lastname"] = "ContactLastName";
                    Guid contactId = service.Create(contact);

                    var account = new Entity("account");
                    account["name"] = "Test Account1";
                    EntityReference primaryContactId = new EntityReference("contact", contactId);
                    account["primarycontactid"] = primaryContactId;

                    Guid accountId1 = service.Create(account);
                    account["name"] = "Test Account2";
                    Guid accountId2 = service.Create(account);
                    account["name"] = "Test Account3";
                    Guid accountId3 = service.Create(account);

                    //Create a query expression specifying the link entity alias and the columns of the link entity that you want to return  
                    var qe = new QueryExpression();
                    qe.EntityName = "account";
                    qe.ColumnSet = new ColumnSet();
                    qe.ColumnSet.Columns.Add("name");

                    qe.LinkEntities.Add(new LinkEntity("account", "contact", "primarycontactid", "contactid", JoinOperator.Inner));
                    qe.LinkEntities[0].Columns.AddColumns("firstname", "lastname");
                    qe.LinkEntities[0].EntityAlias = "primarycontact";

                    EntityCollection ec = service.RetrieveMultiple(qe);

                    Console.WriteLine("Retrieved {0} entities", ec.Entities.Count);
                    foreach (Entity act in ec.Entities)
                    {
                        Console.WriteLine("account name: {0}", act["name"]);
                        Console.WriteLine("primary contact first name: {0}", act.GetAttributeValue<AliasedValue>("primarycontact.firstname").Value);
                        Console.WriteLine("primary contact first name: {0}", act.GetAttributeValue<AliasedValue>("primarycontact.lastname").Value);
                    }

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
            #endregion Sample Code
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
