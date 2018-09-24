using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace PowerApps.Samples
{
  class SampleProgram
  {
    static void Main(string[] args)
    {
      // The URL to the CDS environment you want to connect with
      // i.e. https://yourOrg.crm.dynamics.com
      string resource = "https://<your org>.dynamics.com";

      // Azure Active Directory registered app clientid for Microsoft samples
      var clientId = "51f81489-12ee-4a9e-aaae-a2591f45987d";
      // Azure Active Directory registered app Redirect URI for Microsoft samples
      var redirectUri = new Uri("app://58145B91-0C36-4500-8554-080854F2AC97");
      string authority = "https://login.microsoftonline.com/common";


      var context = new AuthenticationContext(authority, false);

      //Display prompt allowing user to select account to use.
      var platformParameters = new PlatformParameters(
        PromptBehavior.SelectAccount
        );

      //Get the token based on the credentials entered
      var token = context.AcquireTokenAsync(resource,
        clientId,
        redirectUri,
        platformParameters,
        UserIdentifier.AnyUser)
        .Result;

      try
      {
        Guid UserId = GetUserId(resource, token.AccessToken);
        Console.WriteLine("Your UserId is : {0}", UserId);
      }
      catch (Exception ex)
      {
        Console.WriteLine("Error: {0}", ex.Message);
        return;
      }
      Console.WriteLine("Press any key to exit");
      Console.ReadLine();
    }

    static Guid GetUserId(string resource, string accessToken)
    {
      using (var client = new HttpClient())
      {
        client.BaseAddress = new Uri(resource + "/api/data/v9.0/");
        client.DefaultRequestHeaders.Authorization =
          new AuthenticationHeaderValue("Bearer", accessToken);
        client.Timeout = new TimeSpan(0, 2, 0);
        client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
        client.DefaultRequestHeaders.Add("OData-Version", "4.0");
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        // Use the WhoAmI function
        var response = client.GetAsync("WhoAmI").Result;

        if (response.IsSuccessStatusCode)
        {
          //Get the response content and parse it.  
          JObject body = JObject
            .Parse(response.Content.ReadAsStringAsync().Result);
          Guid userId = (Guid)body["UserId"];
          return userId;
        }
        else
        {
          throw new Exception(string.Format(
            "The WhoAmI request failed with a status of '{0}'",
             response.ReasonPhrase));

        }
      }
    }
  }
}
