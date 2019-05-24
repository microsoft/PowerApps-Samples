using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        //Associated solution package that contains custom actions used by this sample
        const string customSolutionFilename = "WebAPIFunctionsandActions_1_0_0_0_managed.zip";
        const string customSolutionName = "WebAPIFunctionsandActions";
        static string customSolutionID = null;
        //Centralized collection of entity URIs used to manage lifetimes
        static List<string> entityUris = new List<string>();
        //A set of variables to hold the state of and URIs for primary entity instances.
        static string incident1Uri, opportunity1Uri, contact1Uri;
        //Objects associated with created URIs, where required.
        static JObject contact1;
        static Guid myUserId;  //CRM User ID for current user.
        private static string solutionName;
        private static bool prompt = true;

        /// <summary> Creates the CRM entity instances used by this sample. </summary>
        private static void CreateRequiredRecords(HttpClient httpClient)
        {
            //Create a parent account, an associated incident with three associated tasks 
            //(required for CalculateTotalTimeIncident).
            Console.WriteLine("----Creating Required Records---- -");
            JObject account1 = new JObject();
            string account1Uri;
            account1["name"] = "Fourth Coffee";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, httpClient.BaseAddress + "accounts");
            request.Content = new StringContent(account1.ToString(), Encoding.UTF8, "application/json");
            HttpResponseMessage response = httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead).Result;
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                Console.WriteLine("Parent Account is created");
                account1Uri = response.Headers.GetValues("OData-EntityId").FirstOrDefault();
                entityUris.Add(account1Uri);
            }
            else
            { throw new Exception(string.Format("Failed to Create Parent account", response.Content)); }

            JArray tasks = new JArray(new JObject[]
            {
                JObject.Parse(@"{subject: 'Task 1', actualdurationminutes: 30}"),
                JObject.Parse(@"{subject: 'Task 2', actualdurationminutes: 30}"),
                JObject.Parse(@"{subject: 'Task 3', actualdurationminutes: 30}")
            }
            );
            JObject incident1 = new JObject();
            incident1["title"] = "Sample Case";
            incident1["customerid_account@odata.bind"] = account1Uri;
            incident1["Incident_Tasks"] = tasks;
            request = new HttpRequestMessage(HttpMethod.Post, httpClient.BaseAddress + "incidents");
            request.Content = new StringContent(incident1.ToString(), Encoding.UTF8, "application/json");
            response = httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead).Result;
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                Console.WriteLine("Incidents are Created");
                incident1Uri = response.Headers.GetValues("OData-EntityId").FirstOrDefault();
                entityUris.Add(incident1Uri);
            }
            else
            { throw new Exception(string.Format("Failed to create Incidents", response.Content)); }

            ////Close the associated tasks (so that they represent completed work).
            JObject incidentTaskRefs;
            //Property value set used to mark tasks as completed. 
            JObject completeCode = JObject.Parse(@"{statecode: 1, statuscode: 5}");
            response = httpClient.GetAsync(incident1Uri + "/Incident_Tasks/$ref",
                HttpCompletionOption.ResponseContentRead).Result;
            if (response.IsSuccessStatusCode)
            {
                incidentTaskRefs = JObject.Parse(response.Content.ReadAsStringAsync().Result);
            }
            else
            { throw new Exception(string.Format("Failed to close the associated tasks", response.Content)); }

            //Create another account and associated opportunity (required for CloseOpportunityAsWon).
            JObject account2 = new JObject();
            string account2Uri;
            account2["name"] = "Coho Winery";
            account2["opportunity_customer_accounts"] = JArray.Parse(@"[{ name: 'Opportunity to win' }]");
            request = new HttpRequestMessage(HttpMethod.Post, "accounts");
            request.Content = new StringContent(account2.ToString(), Encoding.UTF8, "application/json");
            response = httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead).Result;
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                Console.WriteLine("Another Account is created and associated Opportunity");
                account2Uri = response.Headers.GetValues("OData-EntityId").FirstOrDefault();
                entityUris.Add(account2Uri);
            }
            else
            { throw new Exception(string.Format("Failed to create another account and associated opportunity", response.Content)); }
            //Retrieve the URI to the opportunity.
            JObject custOpporRefs;
            response = httpClient.GetAsync(account2Uri + "/opportunity_customer_accounts/$ref",
                HttpCompletionOption.ResponseContentRead).Result;
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Retrieving the URI to the Opportunity");
                custOpporRefs = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                opportunity1Uri = custOpporRefs["value"][0]["@odata.id"].ToString();
            }
            else
            { throw new Exception(string.Format("Failed to retrieve the URI to the opportunity", response.Content)); }

            //Create a contact to use with custom action sample_AddNoteToContact 
            contact1 = JObject.Parse(@"{firstname: 'Jon', lastname: 'Fogg'}");
            request = new HttpRequestMessage(HttpMethod.Post, "contacts");
            request.Content = new StringContent(contact1.ToString(), Encoding.UTF8, "application/json");
            response = httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead).Result;
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                Console.WriteLine("Contact record is created");
                contact1Uri = response.Headers.GetValues("OData-EntityId").FirstOrDefault();
                entityUris.Add(contact1Uri);
            }
            else
            { throw new Exception(string.Format("Failed to create a contact to use with custom action", response.Content)); }
        }

        /// <summary> Deletes the CRM entity instance sample data created by this sample. </summary>
        /// <param name="prompt">True to prompt the user for confirmation and display results; 
        ///   otherwise False to execute silently.</param>
        /// <returns>Number of entity instances deleted</returns>
        private static int DeleteRequiredRecords(HttpClient httpClient, bool prompt)
        {
            if (entityUris.Count == 0) return 0;
            if (prompt)
            {
                Console.Write("\nDo you want these sample entity records deleted? (y/n) [y]: ");
                String answer = Console.ReadLine();
                answer = answer.Trim();
                if (!(answer.StartsWith("y") || answer.StartsWith("Y") ||
                    answer == String.Empty))
                { return 0; }
            }
            HttpResponseMessage response;
            int successCnt = 0, failCnt = 0;
            HttpContent lastBadResponseContent = null;
            foreach (string ent in entityUris)
            {
                response = httpClient.DeleteAsync(ent).Result;
                if (response.IsSuccessStatusCode)
                { successCnt++; }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {; } //Entity may have been deleted by another user or via cascade delete.
                else //Failed to delete
                {
                    failCnt++;
                    lastBadResponseContent = response.Content;
                }
            }
            entityUris.Clear();
            if (failCnt > 0)
            {
                //Throw last failure.
                throw new Exception(string.Format("Failed to delete records", lastBadResponseContent));
            }
            if (prompt)
            { Console.WriteLine("Deleted {0} records!", successCnt); }
            return successCnt;
        }
    }
}
