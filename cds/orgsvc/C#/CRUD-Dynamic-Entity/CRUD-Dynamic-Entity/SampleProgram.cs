using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Configuration;
using System.ServiceModel;
using Microsoft.Xrm.Sdk.Query;

namespace PowerApps.Samples
{
    public class SampleProgram
    {
        static void Main(string[] args)
        {

            try
            {
                //You must specify connection information in cds/App.config to run this sample.
                using (CrmServiceClient csc = new CrmServiceClient(GetConnectionStringFromAppConfig("Connect")))
                {
                    if (csc.IsReady)
                    {
                        IOrganizationService service = csc.OrganizationServiceProxy;

                        #region Sample Code
                        //////////////////////////////////////////////
                        #region Demonstrate
                        
                        // Instantiate an account object.

                        Entity newAccount = new Entity("account");

                        // Set the required attributes. For account, only the name is required. 

                        newAccount["name"] = "Fourth Coffee";

                        //Set any other attribute values.

                        newAccount["address2_postalcode"] = "98074";

                        // Create an account record named Fourth Coffee.

                        Guid accountid = service.Create(newAccount);
                      
                        Console.WriteLine("Created {0} entity named {1}.", newAccount.LogicalName, newAccount["name"]);

                        // Create a column set to define which attributes should be retrieved.                       
                        ColumnSet attributes = new ColumnSet("name", "ownerid");

                        // Retrieve the account and its name and ownerid attributes.
                        newAccount = service.Retrieve(newAccount.LogicalName, accountid, attributes);

                        Console.WriteLine("Retrieved Entity");
                        
                        /*
                        IMPORTANT: 
                        Do not update an entity using a retrieved entity instance.
                        Always instantiate a new Entity and 
                        set the primary key value to match the entity you want to update.
                        Only set the attribute values you are changing.
                        */

                        Entity accountToUpdate = new Entity("account");
                        accountToUpdate["accountid"] = accountid;

                        // Update the address 1 postal code attribute.
                        accountToUpdate["address1_postalcode"] = "98052";

                        // The address 2 postal code was set accidentally, so set it to null.
                        accountToUpdate["address2_postalcode"] = null;

                        // Shows use of Money.
                        accountToUpdate["revenue"] = new Money(5000000);

                        // Shows use of boolean.
                        accountToUpdate["creditonhold"] = false;

                        // Perform the update.
                        service.Update(accountToUpdate);

                        Console.WriteLine("Updated Entity");

                        //Delete the entity
                        service.Delete("account", accountid);

                        Console.WriteLine("Deleted Entity");

                        #endregion Demonstrate
                        //////////////////////////////////////////////
                        #endregion Sample Code

                        Console.WriteLine("The sample completed successfully");
                        return;
                    }
                    else
                    {
                        const string UNABLE_TO_LOGIN_ERROR = "Unable to Login to Dynamics CRM";
                        if (csc.LastCrmError.Equals(UNABLE_TO_LOGIN_ERROR))
                        {
                            Console.WriteLine("Check the connection string values in cds/App.config.");
                            throw new Exception(csc.LastCrmError);
                        }
                        else
                        {
                            throw csc.LastCrmException;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                SampleHelpers.HandleException(ex);
            }


            finally
            {

                Console.WriteLine("Press <Enter> to exit.");
                Console.ReadLine();
            }

        }
        /// <summary>
        /// Gets a named connection string from App.config
        /// </summary>
        /// <param name="name">The name of the connection string to return</param>
        /// <returns>The named connection string</returns>
        static string GetConnectionStringFromAppConfig(string name)
        {
            //Verify cds/App.config contains a valid connection string with the name.
            try
            {
                return ConfigurationManager.ConnectionStrings[name].ConnectionString;
            }
            catch (Exception)
            {
                Console.WriteLine("You must set connection data in cds/App.config before running this sample.");
                return string.Empty;
            }
        }
    }
}
