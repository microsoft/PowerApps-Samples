using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace PowerApps.Samples
{
  class Program
  {
    static void Main(string[] args)
    {
      try
      {
        string connectionString = ConfigurationManager.ConnectionStrings["Connect"].ConnectionString;
        //This sample does not use the Url set in the connection string, just the credentials.
        string clientId = "51f81489-12ee-4a9e-aaae-a2591f45987d"; //This clientid is provided for running samples only.
        string username = SampleHelpers.GetParameterValueFromConnectionString(connectionString, "Username");
        string password = SampleHelpers.GetParameterValueFromConnectionString(connectionString, "Password");

        List<Instance> instances = GetInstances(clientId, username, password);

        if (instances.Count > 1) {
          Console.WriteLine("Available Instances:");
          instances.ForEach(x => {
            Console.WriteLine("UniqueName:{0} ApiUrl:{1} Url:{2}", x.UniqueName, x.ApiUrl, x.Url);
          });
        }
        else
        {
          Console.WriteLine("No instances found.");
        }

      }
      catch (TypeInitializationException tiex)
      {
        StringBuilder errorMessageBuilder  = new StringBuilder();

        errorMessageBuilder.Append("Make sure the ADALListener ");
        errorMessageBuilder.Append("shared listener in App.Config system.diagnostics > ");
        errorMessageBuilder.Append("sharedlisteners is commented out before running this sample.");
        Console.WriteLine(errorMessageBuilder.ToString());
        SampleHelpers.DisplayException(tiex);
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
    /// <summary>
    /// Uses the global web api discovery service to return instances
    /// </summary>
    /// <param name="clientId">The Azure AD client (app) registration</param>
    /// <param name="username">The user name</param>
    /// <param name="password">The password</param>
    /// <returns>A List of Instances</returns>
    static List<Instance> GetInstances(string clientId, string username, string password)
    {

      string GlobalDiscoUrl = "https://globaldisco.crm.dynamics.com/";
      AuthenticationContext authContext = new AuthenticationContext(" https://login.microsoftonline.com", false);

      UserCredential cred = new UserCredential(username, password);
      AuthenticationResult authResult = authContext.AcquireToken(GlobalDiscoUrl, clientId, cred);

      HttpClient client = new HttpClient();
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
      client.Timeout = new TimeSpan(0, 2, 0);
      client.BaseAddress = new Uri(GlobalDiscoUrl);

      HttpResponseMessage response = client.GetAsync("api/discovery/v1.0/Instances", HttpCompletionOption.ResponseHeadersRead).Result;


      if (response.IsSuccessStatusCode)
      {
        //Get the response content and parse it.
        string result = response.Content.ReadAsStringAsync().Result;
        JObject body = JObject.Parse(result);
        JArray values = (JArray)body.GetValue("value");

        if (!values.HasValues)
        {
          return new List<Instance>();
        }

        return JsonConvert.DeserializeObject<List<Instance>>(values.ToString());
      }
      else
      {
        throw new Exception(response.ReasonPhrase);
      }
    }
  }

  /// <summary>
  /// Object returned by the discovery service
  /// </summary>
  class Instance
  {
    public string Id { get; set; }
    public string UniqueName { get; set; }
    public string UrlName { get; set; }
    public string FriendlyName { get; set; }
    public int State { get; set; }
    public string Version { get; set; }
    public string Url { get; set; }
    public string ApiUrl { get; set; }
    public DateTime LastUpdated { get; set; }
  }
}
