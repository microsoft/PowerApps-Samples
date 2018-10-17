using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        //centralized collection of absolute URIs for created entity instances
        private static List<string> entityUris = new List<string>();
        private static bool prompt = true;

        //account1 represents 'Contoso Ltd (sample)' and 
        // contact1 represents 'Yvonne McKey (sample)'.
        static JObject account1 = new JObject();
        static JObject contact1;
        static string account1Uri, contact1Uri;


        private static void CreateRequireRecords(HttpClient httpClient)
        {
            Console.WriteLine("Create sample data:");
            //Create reusable JSON strings for various account and contact tasks.
            string task1Json = @"{subject: 'Task 1', description: 'Task 1 description'}";
            string task2Json = @"{subject: 'Task 2', description: 'Task 2 description'}";
            string task3Json = @"{subject: 'Task 3', description: 'Task 3 description'}";
            //Define the JSON representation for the account and related records  

            account1 = JObject.Parse(
                    "{ " +
                      "name: 'Contoso, Ltd. (sample)', " +
                      "primarycontactid: " +
                      "{ " +
                          "firstname: 'Yvonne', lastname: 'McKay(sample)', jobtitle: 'Coffee Master'," +
                              " annualincome: 45000, " +
                          "Contact_Tasks: [" +
                              task1Json + "," +
                              task2Json + "," +
                              task3Json + "]" +
                      "}, " +
                      "Account_Tasks: [" +
                          task1Json + "," +
                          task2Json + "," +
                          task3Json + "]," +
                      "contact_customer_accounts: [ " +
                      "{" +
                          "firstname: 'Susanna', lastname: 'Stubberod (sample)', jobtitle: 'Senior Purchaser'," +
                              " annualincome: 52000, " +
                          "Contact_Tasks: [" +
                              task1Json + "," +
                              task2Json + "," +
                              task3Json + "]," +
                      "}, " +
                      "{" +
                          "firstname: 'Nancy', lastname: 'Anderson (sample)', jobtitle: 'Activities Manager'," +
                              " annualincome: 55500, " +
                          "Contact_Tasks: [" +
                              task1Json + "," +
                              task2Json + "," +
                              task3Json + "]," +
                      "}, " +
                      "{" +
                          "firstname: 'Maria', lastname: 'Cambell (sample)', jobtitle: 'Accounts Manager'," +
                              " annualincome: 31000, " +
                          "Contact_Tasks: [" +
                              task1Json + "," +
                              task2Json + "," +
                              task3Json + "]," +
                      "}, " +
                      "{" +
                          "firstname: 'Nancy', lastname: 'Anderson (sample)', jobtitle: 'Logistics Specialist'," +
                              " annualincome: 63500, " +
                          "Contact_Tasks: [" +
                              task1Json + "," +
                              task2Json + "," +
                              task3Json + "]," +
                      "}, " +
                      "{" +
                          "firstname: 'Scott', lastname: 'Konersmann (sample)', jobtitle: 'Accounts Manager'," +
                              " annualincome: 38000, " +
                          "Contact_Tasks: [" +
                              task1Json + "," +
                              task2Json + "," +
                              task3Json + "]," +
                      "}, " +
                      "{" +
                          "firstname: 'Robert', lastname: 'Lyon (sample)', jobtitle: 'Senior Technician'," +
                              " annualincome: 78000, " +
                          "Contact_Tasks: [" +
                              task1Json + "," +
                              task2Json + "," +
                              task3Json + "]," +
                      "}, " +
                      "{" +
                          "firstname: 'Paul', lastname: 'Cannon (sample)', jobtitle: 'Ski Instructor'," +
                              " annualincome: 68500, " +
                          "Contact_Tasks: [" +
                              task1Json + "," +
                              task2Json + "," +
                              task3Json + "]," +
                      "}, " +
                      "{" +
                          "firstname: 'Rene', lastname: 'Valdes (sample)', jobtitle: 'Data Analyst III'," +
                              " annualincome: 86000, " +
                          "Contact_Tasks: [" +
                              task1Json + "," +
                              task2Json + "," +
                              task3Json + "]," +
                      "}, " +
                      "{" +
                          "firstname: 'Jim', lastname: 'Glynn (sample)', jobtitle: " +
                              "'Senior International Sales Manager', annualincome: 81400, " +
                          "Contact_Tasks: [" +
                              task1Json + "," +
                              task2Json + "," +
                              task3Json + "]," +
                      "} ]" +
                    "}"
                  );

            //Create account and related records with deep insert request

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, httpClient.BaseAddress + "accounts");
            request.Content = new StringContent(account1.ToString());
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            HttpResponseMessage response = httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead).Result;

            if (response.IsSuccessStatusCode)
            {
                account1Uri = response.Headers.GetValues("OData-EntityId").FirstOrDefault();
                entityUris.Add(account1Uri);
                Console.WriteLine("Account 'Contoso, Ltd. (sample)' created with 1 primary " +
                    "contact and 9 associated contacts.");
            }
            else
            {
                throw new Exception(string.Format("Account 'Contoso, Ltd. (sample)' created with 1 primary " +
                    "contact and 9 associated contacts.", response.Content));
            }

            //Retrieve primary contact record and uri.  Most of the subsequent queries are 
            //performed using this contact.
            string uri = account1Uri + "/primarycontactid/$ref";  //Retrieve the account URI only.
            response = httpClient.GetAsync(uri).Result;
            if (response.IsSuccessStatusCode) //200
            {
                JObject contactRef = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                contact1Uri = contactRef["@odata.id"].ToString();
                entityUris.Add(contact1Uri);
                Console.WriteLine("Has primary contact 'Yvonne McKay (sample)' with URI: {0}\n", contact1Uri);
            }
            else
            {
                throw new Exception(string.Format("Has primary contact 'Yvonne McKay (sample)' with URI: {0}\n", response.Content));
            }
        }
        // <summary> Deletes the CRM entity instance sample data created by this sample. </summary>
        /// <param name="prompt">True to prompt the user for confirmation and display results; 
        ///   otherwise False to execute silently.</param>
        /// <returns>Number of entity instances deleted</returns>
        private static int DeleteRequiredRecords(HttpClient client, bool prompt)
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
                response = client.DeleteAsync(ent).Result;
                if (response.IsSuccessStatusCode) //200-299
                { successCnt++; }
                else if (response.IsSuccessStatusCode) //404
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
                throw new Exception(string.Format("Failed to Delete records", lastBadResponseContent));
            }
            if (prompt)
            { Console.WriteLine("Deleted {0} records!", successCnt); }
            return successCnt;
        }
       
        /// <summary> Displays formatted entity collections to the console. </summary>
        /// <param name="label">Descriptive text output before collection contents </param>
        /// <param name="collection"> JObject containing array of entities to output by property </param>
        /// <param name="properties"> Array of properties within each entity to output. </param>
        private static void DisplayFormattedEntities(string label, JArray entities, string[] properties)
        {
            Console.Write(label);
            int lineNum = 0;
            foreach (JObject entity in entities)
            {
                lineNum++;
                List<string> propsOutput = new List<string>();
                //Iterate through each requested property and output either formatted value if one 
                //exists, otherwise output plain value.
                foreach (string prop in properties)
                {
                    string propValue;
                    string formattedProp = prop + "@OData.Community.Display.V1.FormattedValue";
                    if (null != entity[formattedProp])
                    { propValue = entity[formattedProp].ToString(); }
                    else
                    { propValue = entity[prop].ToString(); }
                    propsOutput.Add(propValue);
                }
                Console.Write("\n\t{0}) {1}", lineNum, String.Join(", ", propsOutput));
            }
            Console.Write("\n");
        }
        ///<summary>Overloaded helper version of method that unpacks 'collection' parameter.</summary>
        private static void DisplayFormattedEntities(string label, JObject collection, string[] properties)
        {
            JToken valArray;
            //Parameter collection contains an array of entities in 'value' member.
            if (collection.TryGetValue("value", out valArray))
            {
                DisplayFormattedEntities(label, (JArray)valArray, properties);
            }
            //Otherwise it just represents a single entity.
            else
            {
                JArray singleton = new JArray(collection);
                DisplayFormattedEntities(label, singleton, properties);
            }
        }

    }
}
