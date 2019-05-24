using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

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


                    Console.WriteLine("--Section 1 started--");
                    string queryOptions;
                    contact1.Add("firstname", "Rafel");
                    contact1.Add("lastname", "Shillo");

                    HttpRequestMessage createrequest1 = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress + "contacts");
                    createrequest1.Content = new StringContent(contact1.ToString());
                    createrequest1.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                    

                    HttpResponseMessage createResponse1 = client.SendAsync(createrequest1, HttpCompletionOption.ResponseHeadersRead).Result;
                    if (createResponse1.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Contact '{0} {1}' created.", contact1.GetValue("firstname"), contact1.GetValue("lastname"));
                        contact1Uri = createResponse1.Headers.GetValues("OData-EntityId").FirstOrDefault();
                        entityUris.Add(contact1Uri);
                        Console.WriteLine("Contact URI: {0}", contact1Uri);

                    }
                    else
                    {
                        throw new Exception(string.Format("Failed to Post Records", createResponse1.ReasonPhrase));
                    }

                    JObject contact1Add = new JObject();
                    contact1Add.Add("annualincome", 80000);
                    contact1Add.Add("jobtitle", "Jumior Developer");

                    HttpRequestMessage updaterequest1 = new HttpRequestMessage(new HttpMethod("PATCH"), contact1Uri);
                    updaterequest1.Content = new StringContent(contact1Add.ToString(), Encoding.UTF8, "application/json");

                    HttpResponseMessage updateresponse1 = client.SendAsync(updaterequest1, HttpCompletionOption.ResponseContentRead).Result;
                    if (updateresponse1.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Contact '{0} {1}' updated with jobtitle" + "and annual income", contact1.GetValue("firstname"), contact1.GetValue("lastname"));
                    }
                    else
                    {
                        Console.WriteLine("Failed to update contact for reason: {0}",
                        updateresponse1.ReasonPhrase);

                    }

                    // Retrieve the contact with its explicitly initialized properties.
                    //fullname is a read-only calculated value.
                    queryOptions = "?$select=fullname,annualincome,jobtitle,description";

                    HttpResponseMessage retrieveResponse1 = client.GetAsync(contact1Uri + queryOptions, HttpCompletionOption.ResponseHeadersRead).Result;

                    if (retrieveResponse1.IsSuccessStatusCode) //200
                    {
                        retrievedcontact1 = JObject.Parse(retrieveResponse1.Content.ReadAsStringAsync().Result);
                        Console.WriteLine("Contact '{0}' retrieved: \n\tAnnual income: {1}" +
                            "\n\tJob title: {2} \n\tDescription: {3}.",
                            // Can use either indexer or GetValue method (or a mix of two)
                            retrievedcontact1.GetValue("fullname"),
                            retrievedcontact1["annualincome"],
                            retrievedcontact1["jobtitle"],
                            retrievedcontact1["description"]);   //description is initialized empty.
                    }
                    else
                    {
                        Console.WriteLine("Failed to retrieve contact for reason: {0}",
                            retrieveResponse1.ReasonPhrase);
                        throw new Exception(string.Format("Failed to retrieve contact for reason: {0}", retrieveResponse1.Content));
                    }

                    //Modify specific properties and then update entity instance.
                    JObject contact1Update = new JObject();
                    contact1Update.Add("jobtitle", "Senior Developer");
                    contact1Update.Add("annualincome", 95000);
                    contact1Update.Add("description", "Assignment to-be-determined");
                    HttpRequestMessage updateRequest2 = new HttpRequestMessage(
                        new HttpMethod("PATCH"), contact1Uri);
                    updateRequest2.Content = new StringContent(contact1Update.ToString(),
                        Encoding.UTF8, "application/json");

                    HttpResponseMessage updateResponse2 = client.SendAsync(updateRequest2, HttpCompletionOption.ResponseContentRead).Result;
                    if (updateResponse2.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Contact '{0}' updated:", retrievedcontact1["fullname"]);
                        Console.WriteLine("\tJob title: {0}", contact1Update["jobtitle"]);
                        Console.WriteLine("\tAnnual income: {0}", contact1Update["annualincome"]);
                        Console.WriteLine("\tDescription: {0}", contact1Update["description"]);
                    }
                    else
                    {
                        Console.WriteLine("Failed to update contact for reason: {0}",
                            updateResponse2.ReasonPhrase);
                        throw new Exception(string.Format("Failed to update contact for reason: {0}", updateResponse2.Content));
                    }

                    //// Change just one property 
                    string phone1 = "555-0105";
                    // Create unique identifier by appending property name 
                    string contactPhoneUri =
                            string.Format("{0}/{1}", contact1Uri, "telephone1");
                    JObject phoneValue = new JObject();
                    phoneValue.Add("value", phone1);   //Updates must use keyword "value". 

                    HttpRequestMessage updateRequest3 =
                        new HttpRequestMessage(HttpMethod.Put, contactPhoneUri);
                    updateRequest3.Content = new StringContent(phoneValue.ToString(),
                        Encoding.UTF8, "application/json");
                    HttpResponseMessage updateResponse3 = client.SendAsync(updateRequest3, HttpCompletionOption.ResponseContentRead).Result;
                    if (updateResponse3.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Contact '{0}' phone number updated.",
                            retrievedcontact1["fullname"]);
                    }
                    else
                    {
                        Console.WriteLine("Failed to update the contact phone number for reason: {0}.",
                            updateResponse3.ReasonPhrase);
                        throw new Exception(string.Format("Failed to update the contact phone number for reason: {0}.", updateResponse3.Content));
                    }

                    //Now retrieve just the single property.
                    JObject retrievedProperty1;
                    HttpResponseMessage retrieveResponse2 = client.GetAsync(contactPhoneUri, HttpCompletionOption.ResponseContentRead).Result;
                    if (retrieveResponse2.IsSuccessStatusCode)
                    {
                        retrievedProperty1 = JObject.Parse(retrieveResponse2.Content.ReadAsStringAsync().Result);
                        Console.WriteLine("Contact's telephone# is: {0}.",
                            retrievedProperty1["value"]);
                    }
                    else
                    {
                        Console.WriteLine("Failed to retrieve the contact phone number for reason: {0}.",
                            retrieveResponse2.ReasonPhrase);
                        throw new Exception(string.Format("Failed to retrieve the contact phone number for reason: {0}.", retrieveResponse2.Content));
                    }

                    /// <summary>  
                    /// Demonstrates creation of entity instance and simultaneous association to another, 
                    ///  existing entity. 
                    /// </summary>
                    /// 

                    Console.WriteLine("\n--Section 2 started--");
                    string queryOptions1;  //select, expand and filter clauses
                                           //Create a new account and associate with existing contact in one operation. 
                    account1.Add("name", "Contoso Ltd");
                    account1.Add("telephone1", "555-5555");
                    account1.Add("primarycontactid@odata.bind", contact1Uri);

                    HttpRequestMessage createRequest2 = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress + "accounts");
                    createRequest2.Content = new StringContent(account1.ToString(), Encoding.UTF8, "application/json");

                    HttpResponseMessage createResponse2 = client.SendAsync(createRequest2, HttpCompletionOption.ResponseContentRead).Result;
                    if (createResponse2.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Account '{0}' created.", account1.GetValue("name"));
                        account1Uri = createResponse2.Headers.GetValues("OData-EntityId").
                            FirstOrDefault();
                        entityUris.Add(account1Uri);
                    }
                    else
                    {
                        Console.WriteLine("Failed to create account for reason: {0}.",
                            createResponse2.ReasonPhrase);
                        throw new Exception(string.Format("Failed to create account for reason: {0}.", createResponse2.Content));
                    }

                    //Retrieve account name and primary contact info
                    queryOptions1 =
                      "?$select=name,&$expand=primarycontactid($select=fullname,jobtitle,annualincome)";
                    HttpResponseMessage retrieveResponse3 = client.GetAsync(account1Uri + queryOptions1, HttpCompletionOption.ResponseHeadersRead).Result;
                    if (retrieveResponse3.IsSuccessStatusCode)
                    {
                        retrievedAccount1 = JObject.Parse(retrieveResponse3.Content.ReadAsStringAsync().Result);
                        Console.WriteLine("Account '{0}' has primary contact '{1}':",
                            retrievedAccount1["name"],
                            retrievedAccount1.GetValue("primarycontactid")["fullname"]
                            );
                        Console.WriteLine("\tJob title: {0} \n\tAnnual income: {1}",
                            retrievedAccount1.GetValue("primarycontactid")["jobtitle"],
                            retrievedAccount1["primarycontactid"]["annualincome"]
                            );
                    }
                    else
                    {
                        Console.WriteLine("Failed to retrieve account for reason: {0}.",
                            retrieveResponse3.ReasonPhrase);
                        throw new Exception(string.Format("Failed to retrieve account for reason: {0}.", retrieveResponse3.Content));
                    }

                    /// <summary>  
                    /// Demonstrates creation of entity instance and related entities in a single operation.  
                    /// </summary>
                    /// 
                    Console.WriteLine("\n--Section 3 started--");
                    string queryOptions2;  //select, expand and filter clauses
                                           //Create the following entries in one operation: an account, its 
                                           // associated primary contact, and open tasks for that contact.  These 
                                           // entity types have the following relationships:
                                           //    Accounts 
                                           //       |---[Primary] Contact (N-to-1)
                                           //              |---Tasks (1-to-N)

                    //Build the Account object inside-out, starting with most nested type(s)
                    JArray tasks = new JArray();
                    JObject task1 = new JObject();
                    task1.Add("subject", "Sign invoice");
                    task1.Add("description", "Invoice #12321");
                    task1.Add("scheduledend", DateTimeOffset.Parse("4/19/2016"));
                    tasks.Add(task1);
                    JObject task2 = new JObject();
                    task2.Add("subject", "Setup new display");
                    task2.Add("description", "Theme is - Spring is in the air");
                    task2.Add("scheduledstart", DateTimeOffset.Parse("4/20/2016"));
                    tasks.Add(task2);
                    JObject task3 = new JObject();
                    task3.Add("subject", "Conduct training");
                    task3.Add("description", "Train team on making our new blended coffee");
                    task3.Add("scheduledstart", DateTimeOffset.Parse("6/1/2016"));
                    tasks.Add(task3);

                    contact2.Add("firstname", "Susie");
                    contact2.Add("lastname", "Curtis");
                    contact2.Add("jobtitle", "Coffee Master");
                    contact2.Add("annualincome", 48000);
                    //Add related tasks using corresponding navigation property
                    contact2.Add("Contact_Tasks", tasks);

                    account2.Add("name", "Fourth Coffee");
                    //Add related contacts using corresponding navigation property
                    account2.Add("primarycontactid", contact2);

                    HttpRequestMessage createRequest3 =
                        new HttpRequestMessage(HttpMethod.Post, client.BaseAddress + "accounts");
                    createRequest3.Content = new StringContent(account2.ToString(), Encoding.UTF8, "application/json");

                    HttpResponseMessage createResponse3 = client.SendAsync(createRequest3, HttpCompletionOption.ResponseContentRead).Result;
                    if (createResponse3.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Account '{0}' created.",
                            account2.GetValue("name"));
                        account2Uri = createResponse3.Headers.GetValues("OData-EntityId").
                            FirstOrDefault();
                        entityUris.Add(account2Uri);
                    }
                    else
                    {
                        Console.WriteLine("Failed to create account for reason: {0}.",
                            createResponse3.ReasonPhrase);
                        throw new Exception(string.Format("Failed to create account for reason: {0}.", createResponse3.Content));
                    }

                    //Retrieve account, primary contact info, and assigned tasks for contact.
                    //CRM only supports querying-by-expansion one level deep, so first query 
                    // account-primary contact.
                    queryOptions2 =
                      "?$select=name,&$expand=primarycontactid($select=fullname,jobtitle,annualincome)";

                    HttpResponseMessage retrieveResponse4 = client.GetAsync(account2Uri + queryOptions2, HttpCompletionOption.ResponseHeadersRead).Result;
                    if (retrieveResponse4.IsSuccessStatusCode)
                    {
                        retrievedAccount2 = JObject.Parse(retrieveResponse4.Content.ReadAsStringAsync().Result);
                        Console.WriteLine("Account '{0}' has primary contact '{1}':",
                            retrievedAccount2.GetValue("name"),
                            retrievedAccount2.GetValue("primarycontactid")["fullname"]);
                        Console.WriteLine("\tJob title: {0} \n\tAnnual income: {1}",
                            retrievedAccount2["primarycontactid"]["jobtitle"],
                            retrievedAccount2["primarycontactid"]["annualincome"]);
                    }
                    else
                    {
                        Console.WriteLine("Failed to retrieve the account/contact info for " +
                            "reason: {0}.", retrieveResponse4.ReasonPhrase);
                        throw new Exception(string.Format("Failed to retrieve the account/contact info for " +
                            "reason: {0}.", retrieveResponse4.Content));
                    }

                    //Next retrieve same contact and its assigned tasks.
                    //Don't have a saved URI for contact 'Susie Curtis', so create one 
                    // from base address and entity ID.
                    string queryOptions3;
                    string contact2Uri = string.Format("{0}contacts({1})",
                     client.BaseAddress,
                     retrievedAccount2["primarycontactid"]["contactid"]);
                    entityUris.Add(contact2Uri);
                    queryOptions3 = "?$select=fullname,&$expand=Contact_Tasks($select=subject,description,scheduledstart,scheduledend)";

                    HttpResponseMessage retrieveResponse5 = client.GetAsync(contact2Uri + queryOptions3, HttpCompletionOption.ResponseHeadersRead).Result;
                    if (retrieveResponse5.IsSuccessStatusCode)
                    {
                        retrievedcontact2 = JObject.Parse(retrieveResponse5.Content.ReadAsStringAsync().Result);
                        Console.WriteLine("Contact '{0}' has the following assigned tasks:",
                            retrievedcontact2["fullname"]);
                        foreach (JToken tk in retrievedcontact2["Contact_Tasks"])
                        {
                            Console.WriteLine(
                                "Subject: {0}, \n\tDescription: {1}\n\tStart: {2}\n\tEnd: {3}\n",
                                tk["subject"],
                                tk["description"],
                                tk["scheduledstart"].Value<DateTime>().ToString("d"),
                                tk["scheduledend"].Value<DateTime>().ToString("d")
                                );
                        }
                    }
                    else
                    {
                        Console.WriteLine("Failed to retrieve the contact info for reason: {0}.",
                            retrieveResponse5.ReasonPhrase);
                        throw new Exception(string.Format("Failed to retrieve the contact info for reason: {0}.", retrieveResponse5.Content));
                    }

                    /// <summary>  
                    /// Demonstrates associating and disassociating of existing entity instances. 
                    /// </summary>
                    /// 
                    string queryOptions4;  //select, expand and filter clauses
                    Console.WriteLine("\n--Section 4 started--");
                    //Add 'Peter Cambel' to the contact list of 'Fourth Coffee', 
                    // a 1-to-N relationship.
                    JObject rel1 = new JObject();   //relationship object for msg content
                    rel1.Add("@odata.id", contact1Uri);
                    Uri navUri1 = new Uri(String.Format("{0}/{1}/$ref", account2Uri,
                        "contact_customer_accounts"));

                    HttpRequestMessage assocRequest1 = new HttpRequestMessage(HttpMethod.Post, navUri1);
                    assocRequest1.Content = new StringContent(rel1.ToString(),
                        Encoding.UTF8, "application/json");

                    HttpResponseMessage assocResponse1 = client.SendAsync(assocRequest1, HttpCompletionOption.ResponseContentRead).Result;
                    if (assocResponse1.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Contact '{0}' associated to account '{1}'.",
                            retrievedcontact1["fullname"], account2["name"]);
                    }
                    else
                    {
                        Console.WriteLine("Failed to associate account/contact entities " +
                            "for reason: {0}.", assocResponse1.ReasonPhrase);
                        throw new Exception(string.Format("Failed to associate account/contact entities " +
                            "for reason: {0}.", assocResponse1.Content));
                    }

                    //Retrieve and output all contacts for account 'Fourth Coffee'.
                    JObject retrievedContactList1;
                    queryOptions4 = "?$select=fullname,jobtitle";

                    HttpResponseMessage retrieveResponse6 = client.GetAsync(account2Uri + "/contact_customer_accounts" + queryOptions4, HttpCompletionOption.ResponseHeadersRead).Result;
                    if (retrieveResponse6.IsSuccessStatusCode)
                    {
                        retrievedContactList1 = JObject.Parse(retrieveResponse6.Content.ReadAsStringAsync().Result);
                        Console.WriteLine("Contact list for account '{0}':",
                            retrievedAccount2["name"]);
                        foreach (JToken ct in retrievedContactList1["value"])
                        {
                            Console.WriteLine("\tName: {0}, Job title: {1}",
                                ct["fullname"], ct["jobtitle"]);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Failed to retrieve the account/contact info" +
                            " for reason: {0}.", retrieveResponse6.ReasonPhrase);
                        throw new Exception(string.Format("Failed to retrieve the account/contact info" +
                            " for reason: {0}.", retrieveResponse6.Content));
                    }

                    //Dissociate the contact from the account.  For a collection-valued 
                    // navigation property, must append URI of referenced entity.
                    string dis1Uri = navUri1 + "?$id=" + contact1Uri;
                    //Equivalently, could have dissociated from the other end of the 
                    // relationship, using the single-valued navigation ref, located in 
                    // the contact 'Peter Cambel'.  This dissociation URI has a simpler form:
                    // [Org URI]/api/data/v8.2/contacts([contactid#])/parentcustomerid_account/$ref 

                    HttpResponseMessage disassocResponse1 = client.DeleteAsync(dis1Uri).Result;
                    if (disassocResponse1.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Contact '{0}' dissociated from account '{1}'.",
                            retrievedcontact1["fullname"], account2["name"]);
                    }
                    else
                    {
                        Console.WriteLine("Failed to disassociate entities for reason: {0}.",
                            disassocResponse1.ReasonPhrase);
                        throw new Exception(string.Format("Failed to disassociate entities for reason: {0}.", disassocResponse1.Content));
                    }

                    //Associate an opportunity to a competitor, an N-to-N relationship. 
                    //First, create the required entity instances.
                    string comp1Uri, oppor1Uri;
                    JObject comp1 = new JObject();
                    comp1.Add("name", "Adventure Works");
                    comp1.Add("strengths",
                        "Strong promoter of private tours for multi-day outdoor adventures");
                    JObject oppor1 = new JObject();
                    oppor1["name"] = "River rafting adventure";
                    oppor1["description"] = "Sales team on a river-rafting offsite and team building";

                    HttpRequestMessage createRequest4 =
                        new HttpRequestMessage(HttpMethod.Post, client.BaseAddress + "competitors");
                    createRequest4.Content = new StringContent(comp1.ToString(), Encoding.UTF8, "application/json");

                    HttpResponseMessage createResponse4 = client.SendAsync(createRequest4, HttpCompletionOption.ResponseContentRead).Result;
                    if (createResponse4.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Competitor '{0}' created.", comp1["name"]);
                        comp1Uri = createResponse4.Headers.GetValues("OData-EntityId").
                            FirstOrDefault();
                        entityUris.Add(comp1Uri);
                    }
                    else
                    {
                        Console.WriteLine("Failed to create competitor for reason: {0}",
                            createResponse4.ReasonPhrase);
                        throw new Exception(string.Format("Failed to create competitor for reason: {0}", createResponse4.Content));
                    }

                    HttpRequestMessage createRequest5 =
                        new HttpRequestMessage(HttpMethod.Post, client.BaseAddress + "opportunities");
                    createRequest5.Content = new StringContent(oppor1.ToString(), Encoding.UTF8, "application/json");

                    HttpResponseMessage createResponse5 = client.SendAsync(createRequest5, HttpCompletionOption.ResponseContentRead).Result;
                    if (createResponse5.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Opportunity '{0}' created.", oppor1["name"]);
                        oppor1Uri = createResponse5.Headers.GetValues("OData-EntityId").
                            FirstOrDefault();
                        entityUris.Add(oppor1Uri);
                    }
                    else
                    {
                        Console.WriteLine("Failed to create opportunity for reason: {0}",
                            createResponse5.ReasonPhrase);
                        throw new Exception(string.Format("Failed to create opportunity for reason: {0}", createResponse5.Content));
                    }

                    //Associate opportunity to competitor via opportunitycompetitors_association.
                    // navigation property.
                    JObject rel2 = new JObject();
                    rel2.Add("@odata.id", comp1Uri);
                    Uri navUri2 = new Uri(String.Format("{0}/{1}/$ref", oppor1Uri,
                        "opportunitycompetitors_association"));
                    HttpRequestMessage assocRequest2 = new HttpRequestMessage(HttpMethod.Post, navUri2);
                    assocRequest2.Content = new StringContent(rel2.ToString(), Encoding.UTF8, "application/json");

                    HttpResponseMessage assocResponse2 = client.SendAsync(assocRequest2, HttpCompletionOption.ResponseContentRead).Result;
                    if (assocResponse2.IsSuccessStatusCode)
                    {
                        Console.WriteLine(
                            "Opportunity '{0}' associated with competitor '{1}'.",
                            oppor1["name"], comp1["name"]);
                    }
                    else
                    {
                        Console.WriteLine("Failed to associate competitor/opportunity" +
                            "entities for reason: {0}.", assocResponse2.ReasonPhrase);
                        throw new Exception(string.Format("Failed to associate competitor/opportunity" +
                            "entities for reason: {0}.", assocResponse2.Content));
                    }

                    //Retrieve all opportunities for competitor 'Adventure Works'.
                    JObject retrievedOpporList1;
                    string queryOptions5;
                    //Select only 'name' to limit returned competitor properties. 
                    queryOptions5 = "?$select=name,&$expand=opportunitycompetitors_association($select=name,description)";

                    HttpResponseMessage retrieveResponse7 = client.GetAsync(comp1Uri + queryOptions5, HttpCompletionOption.ResponseHeadersRead).Result;
                    if (retrieveResponse7.IsSuccessStatusCode)
                    {
                        retrievedOpporList1 = JObject.Parse(retrieveResponse7.Content.ReadAsStringAsync().Result);
                        Console.WriteLine("Competitor '{0}' has the following opportunities:",
                            retrievedOpporList1["name"]);
                        foreach (JToken op in
                            retrievedOpporList1["opportunitycompetitors_association"])
                        {
                            Console.WriteLine("\tName: {0}, \n\tDescription: {1}",
                                op["name"], op["description"]);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Failed to retrieve the competitor/opportunity"
                            + " info for reason: {0}.",
                            retrieveResponse7.ReasonPhrase);
                        throw new Exception(string.Format("Failed to retrieve the competitor/opportunity"
                            + " info for reason: {0}.", retrieveResponse7.Content));
                    }

                    //Dissociate opportunity from competitor.  
                    string dis2Uri = navUri2 + "?$id=" + comp1Uri;
                    HttpResponseMessage disassocResponse2 = client.DeleteAsync(dis2Uri).Result;
                    if (disassocResponse2.IsSuccessStatusCode)
                    {
                        Console.WriteLine(
                            "Opportunity '{0}' disassociated from competitor '{1}'.",
                            oppor1["name"], comp1["name"]);
                    }
                    else
                    {
                        Console.WriteLine("Failed to disassociate entities for reason: {0}.",
                            disassocResponse1.ReasonPhrase);
                        throw new Exception(string.Format("Failed to disassociate entities for reason: {0}.", disassocResponse1.Content));
                    }

                    #region Section 5: Delete sample entities  
                    Console.WriteLine("\n--Section 5 started--");
                    //Delete all the created sample entities.  Note that explicit deletion is not required  
                    // for contact tasks because these are automatically cascade-deleted with owner.  
                    Console.Write("\nDo you want these entity records deleted? (y/n) [y]: ");
                    String answer = Console.ReadLine();
                    answer = answer.Trim();
                    if (!(answer.StartsWith("y") || answer.StartsWith("Y") || answer == String.Empty))
                    { entityUris.Clear(); }

                    HttpResponseMessage deleteResponse1;
                    int successCnt = 0, notFoundCnt = 0, failCnt = 0;
                    HttpContent lastBadResponseContent = null;
                    foreach (string ent in entityUris)
                    {
                        deleteResponse1 = client.DeleteAsync(ent).Result;
                        if (deleteResponse1.IsSuccessStatusCode) //200-299 
                        {
                            Console.WriteLine("Entity deleted: \n{0}.", ent);
                            successCnt++;
                        }
                        else if (deleteResponse1.StatusCode == HttpStatusCode.NotFound) //404 
                        {
                            //May have been deleted by another user or via cascade operation 
                            Console.WriteLine("Entity not found: {0}.", ent);
                            notFoundCnt++;
                        }
                        else
                        {
                            Console.WriteLine("Failed to delete: {0}.", ent);
                            failCnt++;
                            lastBadResponseContent = deleteResponse1.Content;
                        }
                    }
                    Console.WriteLine("Entities deleted: {0}, not found: {1}, delete " +
                        "failures: {2}. \n", successCnt, notFoundCnt, failCnt);
                    entityUris.Clear();
                    if (failCnt > 0)
                    {
                        //Throw last failure 
                        throw new Exception(string.Format("Failed to delete entities for reason: {0}.", disassocResponse1.Content));
                    }
                    #endregion Section 5: Delete sample entities
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
