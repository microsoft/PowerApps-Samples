using Microsoft.Xrm.Sdk;
using System;
using System.Globalization;
using System.Net.Http;

namespace PowerApps.Samples
{
  public class HttpClientPlugin : IPlugin
  {
    private string webAddress;

    /// <summary>
    /// The plug-in constructor.
    /// </summary>
    /// <param name="config">The Web address to access. An empty or null string
    /// defaults to accessing www.bing.com. The Web address can use the HTTP or
    /// HTTPS protocol.</param>
    public HttpClientPlugin(string config)
    {
      if (string.IsNullOrEmpty(config))
      {
        webAddress = "http://www.bing.com";
      }
      else
      {
        webAddress = config;
      }
    }

    /// <summary>
    /// Main execute method that is required by the IPlugin interface. Uses the HttpClient 
    /// .NET class to access the target Web address.
    /// </summary>
    /// <param name="serviceProvider">The service provider from which you can obtain the
    /// tracing service, plug-in execution context, organization service, and more.</param>
    public void Execute(IServiceProvider serviceProvider)
    {
      //Extract the tracing service for use in plug-in debugging.
      ITracingService tracingService =
          (ITracingService)serviceProvider.GetService(typeof(ITracingService));

      try
      {
        tracingService.Trace("Downloading the target URI: " + webAddress);

        try
        {
          // Download the target URI using a Web client. Any .NET class that uses the
          // HTTP or HTTPS protocols and a DNS lookup should work.
          using (HttpClient client = new HttpClient())
          {
            client.Timeout = TimeSpan.FromMilliseconds(15000); //15 seconds
            client.DefaultRequestHeaders.ConnectionClose = true; //Set KeepAlive to false
            

            HttpResponseMessage response =  client.GetAsync(webAddress).Result; //Make sure it is synchonrous
            response.EnsureSuccessStatusCode();

            string responseText = response.Content.ReadAsStringAsync().Result; //Make sure it is synchonrous
            tracingService.Trace(responseText);
            //Log success in the Plugin Trace Log:
            tracingService.Trace("HttpClientPlugin completed successfully.");
          }
        }

        catch (AggregateException aex)
        {
          tracingService.Trace("Inner Exceptions:");

          foreach (Exception ex  in aex.InnerExceptions) {
            tracingService.Trace("  Exception: {0}", ex.ToString());
          }

          throw new InvalidPluginExecutionException(string.Format(CultureInfo.InvariantCulture,
              "An exception occurred while attempting to issue the request.", aex));
        }
      }
      catch (Exception e)
      {
        tracingService.Trace("Exception: {0}", e.ToString());
        throw;
      }
    }
  }
}
