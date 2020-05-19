using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;

namespace PowerApps.Samples
{
    class Program
    {
        //Get configuration data from App.config connectionStrings
        static readonly string connectionString = ConfigurationManager.ConnectionStrings["Connect"].ConnectionString;
        static readonly ServiceConfig config = new ServiceConfig(connectionString);

        static void Main()
        {
            //List of URIs for records created in this sample
            List<Uri> entityUris = new List<Uri>();

            //true to prompt iser for record deletion; otherwise false
            bool deleteCreatedRecords = true;
            
            try
            {
                using (CDSWebApiService svc = new CDSWebApiService(config))
                {
                    Console.WriteLine("--Starting conditional operations demonstration--\n");

                    #region Create required records
                    // Create an account record
                    var account1 = new JObject {
                        { "name", "Contoso Ltd" },
                        { "telephone1", "555-0000" }, //Phone number value increments with each update attempt
                        { "revenue", 5000000},
                        { "description", "Parent company of Contoso Pharmaceuticals, etc."} };

                    Uri account1Uri = svc.PostCreate("accounts", account1);
                    entityUris.Add(account1Uri); // Track any created records

                    // Retrieve the account record that was just created.
                    string queryOptions = "?$select=name,revenue,telephone1,description";
                    var retrievedaccount1 = svc.Get(account1Uri.ToString() + queryOptions );

                    // Store the ETag value from the retrieved record
                    string initialAcctETagVal = retrievedaccount1["@odata.etag"].ToString();

                    Console.WriteLine("Created and retrieved the initial account, shown below:");
                    Console.WriteLine(retrievedaccount1.ToString(Formatting.Indented));
                    #endregion Create required records

                    #region Conditional GET
                    Console.WriteLine("\n** Conditional GET demonstration **");

                    // Attempt to retrieve the account record using a conditional GET defined by a message header with
                    // the current ETag value.
                    retrievedaccount1 = svc.GetAsync(
                        path: account1Uri.ToString() + queryOptions,
                        headers: new Dictionary<string, List<string>> {
                            { "If-None-Match", new List<string> {initialAcctETagVal}}}
                    ).Result;

                    if( svc.LastStatusCode == HttpStatusCode.NotModified) // Expected result
                    {
                        Console.WriteLine("Account record retrieved using ETag: {0}", initialAcctETagVal);
                        Console.WriteLine("Expected outcome: Entity was not modified so nothing was returned.");
                    }
                    else // Not expected, something was returned
                    {
                        Console.WriteLine("Instance retrieved using ETag: {0}", initialAcctETagVal);
                        Console.WriteLine(retrievedaccount1.ToString(Formatting.Indented));
                    }

                    // Modify the account instance by updating the telephone1 attribute
                    svc.Put(account1Uri, "telephone1", "555-0001");
                    Console.WriteLine("\n\bAccount telephone number updated to '555-0001'.\n");

                    // Re-attempt to retrieve using conditional GET defined by a message header with
                    // the current ETag value.
                    retrievedaccount1  = svc.GetAsync(
                        path: account1Uri.ToString() + queryOptions,
                        headers: new Dictionary<string, List<string>> {
                            { "If-None-Match", new List<string> {initialAcctETagVal}}}
                    ).Result;

                    if (svc.LastStatusCode == HttpStatusCode.NotModified) // Not expected
                    {
                        Console.WriteLine("Unexpected outcome: Entity was modified so something should be returned.");
                    }
                    else // Expected, returned status is HttpStatusCode.OK
                    {
                        Console.WriteLine("Modified account record retrieved using ETag: {0}", initialAcctETagVal);
                        Console.WriteLine("Notice the update ETag value and telephone number");
                    }

                    // Save the updated ETag value
                    var updatedAcctETagVal = retrievedaccount1["@odata.etag"].ToString();

                    // Display ty updated record
                    Console.WriteLine(retrievedaccount1.ToString(Formatting.Indented));
                    #endregion Conditional GET

                    #region Optimistic concurrency on delete and update
                    Console.WriteLine("\n** Optimistic concurrency demonstration **");

                    // Attempt to delete original account (if matches original ETag value).
                    // If you replace "initialAcctETagVal" with "updatedAcctETagVal", the delete will succeed.
                    // However, we want the delete to fail for now.
                    Console.WriteLine("Attempting to delete the account using the original ETag value");
                    svc.Delete(
                        uri: account1Uri, 
                        headers: new Dictionary<string, List<string>> {
                            { "If-Match", new List<string> {initialAcctETagVal}}}
                    );

                    if (svc.LastStatusCode == HttpStatusCode.PreconditionFailed) // 412; Precondition failed error expected
                    {
                        Console.WriteLine("Expected Error: The version of the account record no longer matches the original ETag.");
                        Console.WriteLine("\tAccount not deleted using ETag '{0}', status code: '{1}'.",
                            initialAcctETagVal, (int)svc.LastStatusCode);
                    }
                    else if ( ((int)svc.LastStatusCode>=200) && ((int)svc.LastStatusCode <= 299) ) // 200-299; not expected
                    {
                        Console.WriteLine("Account deleted!");
                    }
                    else
                    {
                        throw new Exception("Failed to delete original account");
                    }

                    //Attempt to update account (if matches original ETag value).
                    JObject accountUpdate = new JObject() {
                        { "telephone1", "555-0002" }, 
                        { "revenue", 6000000 }
                    };

                    Console.WriteLine("Attempting to update the account using the original ETag value");
                    svc.Patch(
                        uri: account1Uri,
                        body: accountUpdate,
                        headers: new Dictionary<string, List<string>> {
                            { "If-Match", new List<string> {initialAcctETagVal}}}
                    );

                    if (svc.LastStatusCode == HttpStatusCode.PreconditionFailed) // 412; //Precondition failed error expected
                    {
                        Console.WriteLine("Expected Error: The version of the existing record doesn't match the ETag property provided.");
                        Console.WriteLine("\tAccount not updated using ETag '{0}', status code: '{1}'.",
                          initialAcctETagVal, (int)svc.LastStatusCode);
                    }
                    else if (svc.LastStatusCode == HttpStatusCode.NoContent)  // 204; not expected
                    {
                        Console.WriteLine("Account updated using ETag: {0}, status code: '{1}'.",
                        initialAcctETagVal, (int)svc.LastStatusCode);
                    }
                    else
                    {
                        throw new Exception("Failed to update account (if orgiginal ETag value matches)");
                    }

                    // Reattempt update if matches current ETag value.
                    accountUpdate["telephone1"] = "555-0003";

                    Console.WriteLine("Attempting to update the account using the current ETag value");
                    svc.Patch(
                        uri: account1Uri,
                        body: accountUpdate,
                        headers: new Dictionary<string, List<string>> {
                            { "If-Match", new List<string> {updatedAcctETagVal}}}
                    );

                    if (svc.LastStatusCode == HttpStatusCode.NoContent) // 204; expected
                    {
                        Console.WriteLine("\nAccount successfully updated using ETag: {0}, status code: '{1}'.",
                        updatedAcctETagVal, (int)svc.LastStatusCode);
                    }
                    else if (svc.LastStatusCode == HttpStatusCode.PreconditionFailed) // 412; not expected
                    {
                        Console.WriteLine("Unexpected status code: '{0}'", (int)svc.LastStatusCode);
                    }
                    else
                    {
                        throw new Exception("Failed to update if matches current ETag value");
                    }

                    // Retrieve and output current account state.
                    retrievedaccount1 = svc.GetAsync(account1Uri.ToString() + queryOptions).Result;
                    Console.WriteLine("\nBelow is the final state of the account");
                    Console.WriteLine(retrievedaccount1.ToString(Formatting.Indented));
                    #endregion Optimistic concurrency on delete and update

                    #region Delete created records  
                    // Delete all the created sample entities.  Note that explicit deletion is not required  
                    // for contact tasks because these are automatically cascade-deleted with owner.  

                    if (deleteCreatedRecords)
                    {
                        Console.Write("\nDo you want these entity records deleted? (y/n) [y]: ");
                        String answer = Console.ReadLine();
                        answer = answer.Trim();
                        if (!(answer.StartsWith("y") || answer.StartsWith("Y") || answer == string.Empty))
                        { entityUris.Clear(); }
                        else
                        {
                            Console.WriteLine("\nDeleting created records.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nDeleting created records.");
                    }

                    foreach (Uri entityUrl in entityUris)
                    {
                        svc.Delete(entityUrl);
                    }
                    #endregion Delete created records 
                }
            }
             
            catch (Exception)
            {
                throw;
            }
        }
    }
}
