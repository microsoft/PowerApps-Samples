using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        static void Main(string[] args)
        {
            try
            {
                //Get configuration data from App.config connectionStrings
                string connectionString = ConfigurationManager.ConnectionStrings["Connect"].ConnectionString;

                using (HttpClient client = SampleHelpers.GetHttpClient(
                    connectionString,
                    SampleHelpers.clientId,
                    SampleHelpers.redirectUrl,
                    "v9.0"))
                {
                    /// <summary> Creates the CRM entity instance used by this sample. </summary>

                    // Create a CRM account record.
                    Console.WriteLine("\nCreate sample data");
                    account.Add("name", "Contoso Ltd");
                    account.Add("telephone1", "555-0000"); //Phone number value will increment with each update attempt
                    account.Add("revenue", 5000000);
                    account.Add("description", "Parent company of Contoso Pharmaceuticals, etc.");

                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress + "accounts");
                    request.Content = new StringContent(account.ToString(), Encoding.UTF8, "application/json");


                    HttpResponseMessage response = client.SendAsync(request, HttpCompletionOption.ResponseContentRead).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        accountUri = response.Headers.GetValues("OData-EntityId").FirstOrDefault();
                        entityUri = accountUri;
                    }
                    else
                    {
                        throw new Exception(string.Format("Failed to create accounts", response.Content));
                    }

                    //Retrieve the account record you created.
                    queryOptions = "?$select=name,revenue,telephone1,description";
                    //JObject entity = null;

                    HttpResponseMessage response1 = client.GetAsync(entityUri + queryOptions).Result;
                    if (response1.IsSuccessStatusCode) //200
                    {
                        account = JObject.Parse(response1.Content.ReadAsStringAsync().Result);
                        Console.WriteLine("Account entity created:");
                        Console.WriteLine(account.ToString(Newtonsoft.Json.Formatting.Indented));
                        initialAcctETagVal = account["@odata.etag"].ToString();
                    }
                    
                    #region Conditional GET            
                    Console.WriteLine("\n--Conditional GET section started--");
                    // Attempt to retrieve using conditional GET with current ETag value.
                    request = new HttpRequestMessage(HttpMethod.Get, accountUri + queryOptions);

                    // Retrieve only if it doesn't match previously retrieved version.
                    request.Headers.Add("If-None-Match", initialAcctETagVal);
                    response = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result;

                    if (response.StatusCode == HttpStatusCode.NotModified)  // 304; expected.
                    {
                        Console.WriteLine("Instance retrieved using ETag: {0}", initialAcctETagVal);
                        Console.WriteLine("Expected outcome: Entity was not modified so nothing was returned.");
                    }

                    else if (response.StatusCode == HttpStatusCode.OK)  // 200; not expected
                    {
                        Console.WriteLine("Instance retrieved using ETag: {0}", initialAcctETagVal);
                        account = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                        Console.WriteLine(account.ToString(Formatting.Indented));
                    }

                    else
                    {
                        throw new Exception(string.Format("Failed to retrieve", response.Content));
                    }

                    // Modify the account instance by updating telephone1

                    string accountPhoneUri = string.Format("{0}/{1}", accountUri, "telephone1");
                    JObject phoneProperty = new JObject();
                    phoneProperty.Add("value", "555-0001");
                    request = new HttpRequestMessage(HttpMethod.Put, accountPhoneUri);
                    request.Content = new StringContent(phoneProperty.ToString(), Encoding.UTF8, "application/json");
                    response = client.SendAsync(request, HttpCompletionOption.ResponseContentRead).Result;
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        Console.WriteLine("\nAccount telephone number updated.");
                    }
                    else
                    {
                        throw new Exception(string.Format("Failed to update the account telephone number", response.Content));
                    }

                    // Reattempt conditional GET with original ETag value.             
                    request = new HttpRequestMessage(HttpMethod.Get, accountUri + queryOptions);
                    request.Headers.Add("If-None-Match", initialAcctETagVal);
                    response = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result;

                    if (response.StatusCode == HttpStatusCode.OK) //200; expected
                    {
                        Console.WriteLine("Instance retrieved using ETag: {0}", initialAcctETagVal);
                    }
                    else if (response.StatusCode == HttpStatusCode.NotModified) // 304; not expected
                    {
                        Console.WriteLine("Unexpected status code: '{0}'.", (int)response.StatusCode);
                    }
                    else
                    { throw new Exception(string.Format("Failed to get original ETag value", response.Content)); }

                    // Retrieve and output current account state.
                    account = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    updatedAcctETagVal = account["@odata.etag"].ToString(); // Capture updated ETag
                    Console.WriteLine(account.ToString(Formatting.Indented));

                    #endregion Conditional GET

                    #region Optimistic concurrency on delete and update
                    Console.WriteLine("\n--Optimistic concurrency section started--");

                    // Attempt to delete original account (if matches original ETag value).
                    request = new HttpRequestMessage(HttpMethod.Delete, accountUri);
                    request.Headers.Add("If-Match", initialAcctETagVal); // If you replace "initialAcctETagVal" with "updatedAcctETagVal", 
                                                                         // delete will succeed.
                    response = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result;

                    if (response.StatusCode == HttpStatusCode.PreconditionFailed) // 412; Precondition failed error expected
                    {
                        Console.WriteLine("Expected Error: The version of the existing record doesn't match the property provided.");
                        Console.WriteLine("\tAccount not deleted using ETag '{0}', status code: '{1}'.",
                            initialAcctETagVal, (int)response.StatusCode);
                    }
                    else if (response.IsSuccessStatusCode) // 200-299; not expected
                    {
                        Console.WriteLine("Account deleted!");
                    }

                    else
                    {
                        throw new Exception(string.Format("Failed to delete original ccount", response.Content));
                    }

                    //Attempt to update account (if matches original ETag value).
                    JObject accountUpdate = new JObject();
                    accountUpdate.Add("telephone1", "555-0002");
                    accountUpdate.Add("revenue", 6000000);
                    request = new HttpRequestMessage(new HttpMethod("PATCH"), accountUri);
                    request.Content = new StringContent(accountUpdate.ToString(),
                        Encoding.UTF8, "application/json");
                    request.Headers.Add("If-Match", initialAcctETagVal);
                    response = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result;

                    if (response.StatusCode == HttpStatusCode.PreconditionFailed) // 412; //Precondition failed error expected
                    {
                        Console.WriteLine("Expected Error: The version of the existing record doesn't match the property provided.");
                        Console.WriteLine("\tAccount not updated using ETag '{0}', status code: '{1}'.",
                          initialAcctETagVal, (int)response.StatusCode);
                    }
                    else if (response.StatusCode == HttpStatusCode.NoContent)  // 204; not expected
                    {
                        Console.WriteLine("Account updated using ETag: {0}, status code: '{1}'.",
                        initialAcctETagVal, (int)response.StatusCode);
                    }
                    else
                    {
                        throw new Exception(string.Format("Failed to update account (if orgiginal ETag value matches)", response.Content));
                    }

                    // Reattempt update if matches current ETag value.
                    accountUpdate["telephone1"] = "555-0003";
                    request = new HttpRequestMessage(new HttpMethod("PATCH"), accountUri);
                    request.Content = new StringContent(accountUpdate.ToString(),
                        Encoding.UTF8, "application/json");
                    request.Headers.Add("If-Match", updatedAcctETagVal);
                    response = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result;

                    if (response.StatusCode == HttpStatusCode.NoContent) // 204; expected
                    {
                        Console.WriteLine("\nAccount successfully updated using ETag: {0}, status code: '{1}'.",
                        updatedAcctETagVal, (int)response.StatusCode);
                    }
                    else if (response.StatusCode == HttpStatusCode.PreconditionFailed) // 412; not expected
                    {
                        Console.WriteLine("Unexpected status code: '{0}'", (int)response.StatusCode);
                    }
                    else
                    {
                        throw new Exception(string.Format("FAile to update if matches current ETag value", response.Content));
                    }

                    // Retrieve and output current account state.
                    account = GetCurrentRecord(client,accountUri, queryOptions);
                    updatedAcctETagVal = account["@odata.etag"].ToString(); //Capture updated ETag
                    Console.WriteLine(account.ToString(Formatting.Indented));

                    #endregion Optimistic concurrency on delete and update

                    #region Controlling upsert operations
                    Console.WriteLine("\n--Controlling upsert operations section started--");
                    //Attempt to insert without update some properties for this account
                    accountUpdate = new JObject();
                    accountUpdate.Add("telephone1", "555-0004");
                    accountUpdate.Add("revenue", 7500000);

                    request = new HttpRequestMessage(new HttpMethod("PATCH"), accountUri);
                    request.Content = new StringContent(accountUpdate.ToString(),
                        Encoding.UTF8, "application/json");
                    //Perform operation only if matching resource does not exist. 
                    request.Headers.Add("If-None-Match", "*");
                    response = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result;

                    if (response.StatusCode == HttpStatusCode.PreconditionFailed) // 412; expected
                    {
                        Console.WriteLine("Expected Error: A record with matching key values already exists.");
                        Console.WriteLine("\tAccount not updated using ETag '{0}, status code: '{1}'.",
                            initialAcctETagVal, (int)response.StatusCode);
                    }
                    else if (response.StatusCode == HttpStatusCode.NoContent) // 204; unexpected
                    {
                        Console.WriteLine("Account updated using If-None-Match '*'");
                    }

                    else
                    {
                        throw new Exception(string.Format("Failed to perform operations", response.Content));
                    }

                    //Attempt to perform same update without creation. 
                    accountUpdate["telephone1"] = "555-0005";
                    request = new HttpRequestMessage(new HttpMethod("PATCH"), accountUri);
                    request.Content = new StringContent(accountUpdate.ToString(),
                        Encoding.UTF8, "application/json");
                    //Perform operation only if matching resource exists. 
                    request.Headers.Add("If-Match", "*");
                    response = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result;

                    if (response.StatusCode == HttpStatusCode.NoContent)  // 204; expected
                    {
                        Console.WriteLine("Account updated using If-Match '*'");
                    }
                    else if (response.StatusCode == HttpStatusCode.PreconditionFailed) // 412; not expected
                    {
                        Console.WriteLine("Account not updated using If-Match '*', status code: '{0}'.",
                          (int)response.StatusCode);
                    }
                    else
                    { throw new Exception(string.Format("Failed to perform operations", response.Content)); }

                    //Retrieve and output current account state.
                    account = GetCurrentRecord(client,accountUri, queryOptions);
                    Console.WriteLine(account.ToString(Formatting.Indented));

                    // Delete the account record
                    HttpResponseMessage deleteResponse;
                    deleteResponse = client.DeleteAsync(accountUri).Result;
                    if (deleteResponse.IsSuccessStatusCode) // 200-299
                    {
                        Console.WriteLine("\nAccount was deleted.");
                    }
                    else if (deleteResponse.StatusCode == HttpStatusCode.NotFound)
                    // 404; entity record may have been deleted by another user.
                    {
                        Console.WriteLine("Account could not be found.");
                    }
                    else // Failed to delete
                    {
                        // Throw last failure.
                        throw new Exception(string.Format("Failed to delete account record", response.Content));
                    }

                    // Attempt to update it
                    accountUpdate["telephone1"] = "555-0006";
                    request = new HttpRequestMessage(new HttpMethod("PATCH"), accountUri);
                    request.Content = new StringContent(accountUpdate.ToString(),
                        Encoding.UTF8, "application/json");

                    // Perform operation only if matching resource exists. 
                    request.Headers.Add("If-Match", "*");
                    response = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result;

                    if (response.StatusCode == HttpStatusCode.PreconditionFailed ||
                             response.StatusCode == HttpStatusCode.NotFound)  // 412 or 404; expected
                    {
                        Console.WriteLine("Expected Error: Account with Id = {0} does not exist.", account["accountid"]);
                        Console.WriteLine("Account not updated because it does not exist, status code: '{0}'.",
                          (int)response.StatusCode);
                    }
                    else if (response.StatusCode == HttpStatusCode.NoContent)  // 204; not expected                                                                       
                    {
                        Console.WriteLine("Account upserted using If-Match '*'");
                    }
                    else
                    {
                        throw new Exception(string.Format("Failed to perform operations", response.Content));
                    }

                    #endregion Controlling upsert operations

               
                }
            }
            catch (Exception ex)
            {
                SampleHelpers.DisplayException(ex);
                throw;
            }
            finally
            {
                Console.WriteLine("Press <Enter> to exit the program.");
                Console.ReadLine();
            }
        }
    }
}
