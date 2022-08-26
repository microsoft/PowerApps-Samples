using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;

namespace PowerApps.Samples
{
    /// <summary>
    /// This program demonstrates use of conditional operations with the 
    /// Common Data Service Web API.
    /// </summary>
    class Program
    {
        //Get environment configuration data from the connection string in the App.config file.
        static readonly string connectionString = 
            ConfigurationManager.ConnectionStrings["Connect"].ConnectionString;
        static readonly ServiceConfig config = new ServiceConfig(connectionString);

        static void Main()
        {
            // Save the URIs for entity records created in this sample. so they
            // can be deleted later.
            List<Uri> entityUris = new List<Uri>();

            try
            {
                // Use the wrapper class that handles message processing, error handling, and more.
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
                    var retrievedaccount1 = svc.Get(account1Uri.ToString() + queryOptions);

                    // Store the ETag value from the retrieved record
                    string initialAcctETagVal = retrievedaccount1["@odata.etag"].ToString();

                    Console.WriteLine("Created and retrieved the initial account, shown below:");
                    Console.WriteLine(retrievedaccount1.ToString(Formatting.Indented));
                    #endregion Create required records

                    #region Conditional GET
                    Console.WriteLine("\n** Conditional GET demonstration **");

                    // Attempt to retrieve the account record using a conditional GET defined by a message header with
                    // the current ETag value.
                    try
                    {
                        retrievedaccount1 = svc.Get(
                            path: account1Uri.ToString() + queryOptions,
                            headers: new Dictionary<string, List<string>> {
                            { "If-None-Match", new List<string> {initialAcctETagVal}}}
                        );

                        // Not expected; the returned response contains content.
                        Console.WriteLine("Instance retrieved using ETag: {0}", initialAcctETagVal);
                        Console.WriteLine(retrievedaccount1.ToString(Formatting.Indented));
                    }
                    catch (ServiceException e) {
                        if (e.StatusCode == (int)HttpStatusCode.NotModified) // Expected result
                        {
                            Console.WriteLine("Account record retrieved using ETag: {0}", initialAcctETagVal);
                            Console.WriteLine("Expected outcome: Entity was not modified so nothing was returned.");
                        }
                        else { throw e; }

                    }

                    // Modify the account instance by updating the telephone1 attribute
                    svc.Put(account1Uri.ToString(), "telephone1", "555-0001");
                    Console.WriteLine("\n\bAccount telephone number updated to '555-0001'.\n");

                    // Re-attempt to retrieve using conditional GET defined by a message header with
                    // the current ETag value.
                    try
                    {
                        retrievedaccount1 = svc.Get(
                            path: account1Uri.ToString() + queryOptions,
                            headers: new Dictionary<string, List<string>> {
                            { "If-None-Match", new List<string> {initialAcctETagVal}}}
                        );

                        // Expected result
                        Console.WriteLine("Modified account record retrieved using ETag: {0}", initialAcctETagVal);
                        Console.WriteLine("Notice the update ETag value and telephone number");
                    }
                    catch (ServiceException e)
                    {
                        if (e.StatusCode == (int)HttpStatusCode.NotModified) // Not expected
                        {
                            Console.WriteLine("Unexpected outcome: Entity was modified so something should be returned.");
                        }
                        else { throw e; }
                    }

                    // Save the updated ETag value
                    var updatedAcctETagVal = retrievedaccount1["@odata.etag"].ToString();

                    // Display ty updated record
                    Console.WriteLine(retrievedaccount1.ToString(Formatting.Indented));
                    #endregion Conditional GET

                    #region Optimistic concurrency on delete and update
                    Console.WriteLine("\n** Optimistic concurrency demonstration **");

                    // Attempt to delete original account (if matches original ETag value).
                    // If you replace "initialAcctETagVal" with "updatedAcctETagVal", the delete will
                    // succeed. However, we want the delete to fail for now to demonstrate use of the ETag.
                    Console.WriteLine("Attempting to delete the account using the original ETag value");

                    try
                    {
                        svc.Delete(
                            path: account1Uri.ToString(),
                            headers: new Dictionary<string, List<string>> {
                               { "If-Match", new List<string> {initialAcctETagVal}}}
                        );

                        // Not expected; this code should not execute.
                        Console.WriteLine("Account deleted");
                    }
                    catch (ServiceException e)
                    {
                        if (e.StatusCode == (int)HttpStatusCode.PreconditionFailed) // Expected result
                        {
                            Console.WriteLine("Expected Error: The version of the account record no" +
                                " longer matches the original ETag.");
                            Console.WriteLine("\tAccount not deleted using ETag '{0}', status code: '{1}'.",
                                initialAcctETagVal, e.StatusCode);
                        }
                        else { throw e; }
                    }

                    Console.WriteLine("Attempting to update the account using the original ETag value");
                    JObject accountUpdate = new JObject() {
                        { "telephone1", "555-0002" },
                        { "revenue", 6000000 }
                    };

                    try
                    {
                        svc.Patch(
                            uri: account1Uri,
                            body: accountUpdate,
                            headers: new Dictionary<string, List<string>> {
                            { "If-Match", new List<string> {initialAcctETagVal}}}
                        );

                        // Not expected; this code should not execute.
                        Console.WriteLine("Account updated using original ETag {0}", initialAcctETagVal);
                    }
                    catch (ServiceException e)
                    {
                        if (e.StatusCode == (int)HttpStatusCode.PreconditionFailed) // Expected error
                        {
                            Console.WriteLine("Expected Error: The version of the existing record doesn't "
                                + "match the ETag property provided.");
                            Console.WriteLine("\tAccount not updated using ETag '{0}', status code: '{1}'.",
                              initialAcctETagVal, (int)e.StatusCode);
                        }
                        else { throw e; }
                    }

                    // Reattempt update if matches current ETag value.
                    accountUpdate["telephone1"] = "555-0003";
                    Console.WriteLine("Attempting to update the account using the current ETag value");
                    try
                    {
                        svc.Patch(
                            uri: account1Uri,
                            body: accountUpdate,
                            headers: new Dictionary<string, List<string>> {
                                { "If-Match", new List<string> { updatedAcctETagVal }} }
                        );

                        // Expected program flow; this code should execute.
                        Console.WriteLine("\nAccount successfully updated using ETag: {0}.",
                            updatedAcctETagVal);
                    }
                    catch (ServiceException e)
                    {
                        if (e.StatusCode == (int)HttpStatusCode.PreconditionFailed) // Not expected
                        {
                            Console.WriteLine("Unexpected status code: '{0}'", (int)e.StatusCode);
                        }
                        else { throw e; }
                    }

                    // Retrieve and output current account state.
                    retrievedaccount1 = svc.Get(account1Uri.ToString() + queryOptions);

                    Console.WriteLine("\nBelow is the final state of the account");
                    Console.WriteLine(retrievedaccount1.ToString(Formatting.Indented));
                    #endregion Optimistic concurrency on delete and update

                    #region Delete created records

                    // Delete (or keep) all the created entity records.
                    Console.Write("\nDo you want these entity records deleted? (y/n) [y]: ");
                    String answer = Console.ReadLine().Trim();

                    if (!(answer.StartsWith("y") || answer.StartsWith("Y") || answer == string.Empty))
                        entityUris.Clear();

                    foreach (Uri entityUrl in entityUris) svc.Delete(entityUrl.ToString());

                    #endregion Delete created records 

                    Console.WriteLine("--Conditional operations demonstration Completed--");
                    Console.WriteLine("Press any key to close");
                    Console.ReadLine();
                }
            }
            catch (ServiceException e)
            {
                Console.WriteLine("Message send response: status code {0}, {1}",
                    e.StatusCode, e.ReasonPhrase);
            }
        }
    }
}
