using Microsoft.Identity.Client;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;

namespace DataverseMauiApp;

public class EnvironmentsViewModel : ObservableCollection<Environment>
{
  IPublicClientApplication _publicClient;
  HttpClient _httpClient;

  public EnvironmentsViewModel(IPublicClientApplication publicClient, HttpClient httpClient)
  {
    _publicClient = publicClient ?? throw new ArgumentNullException(nameof(publicClient));
    _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
  }

  internal async Task RefreshAsync()
  {
    // https://learn.microsoft.com/en-us/power-apps/developer/data-platform/discovery-service

    // Get auth token
    var scopes = new string[] {
      "https://globaldisco.crm.dynamics.com//.default",
    };
    Debug.WriteLine("Getting auth token");
    var authResponse = await _publicClient.AcquireTokenSilent(scopes, PublicClientApplication.OperatingSystemAccount)
      .ExecuteAsync().ConfigureAwait(false);

    // Get environments
    Debug.WriteLine("Getting environments");
    var requestMessage = new HttpRequestMessage(HttpMethod.Get, BuildUrl());
    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.AccessToken);

    var httpResponse = await _httpClient.SendAsync(requestMessage);
    var responseContent = await httpResponse.Content.ReadAsStringAsync();

    Debug.WriteLine("Deserializing environments");
    var responseJson = JsonSerializer.Deserialize<JsonElement>(responseContent);

    UpdateEnvironments(responseJson);

    Debug.WriteLine("Refresh complete");
  }

  private void UpdateEnvironments(JsonElement jsonElement)
  {
    Clear();
    foreach (var environmentJson in jsonElement.GetProperty("value").EnumerateArray())
    {
      var environment = environmentJson.Deserialize<Environment>();
      /* Filter
      if (environment.OrganizationType != OrganizationType.CustomerTest)
        continue;
      */
      Items.Add(environment);
    }
    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
  }

  private Uri BuildUrl()
  {
    return new Uri($"https://globaldisco.crm.dynamics.com/api/discovery/v2.0/Instances");
  }
}
