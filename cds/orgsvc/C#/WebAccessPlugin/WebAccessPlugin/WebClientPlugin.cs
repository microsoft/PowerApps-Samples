using Microsoft.Xrm.Sdk;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace PowerApps.Samples
{
  public sealed class WebClientPlugin : IPlugin
  {
    private string webAddress;

    /// <summary>
    /// The plug-in constructor.
    /// </summary>
    /// <param name="config">The Web address to access. An empty or null string
    /// defaults to accessing www.bing.com. The Web address can use the HTTP or
    /// HTTPS protocol.</param>
    public WebClientPlugin(string config)
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
    /// Main execute method that is required by the IPlugin interface. Uses the WebClient 
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
          // Download the target URI using a ShortWebClient derived WebClient with shorter timeout 
          // Any .NET class that uses the HTTP or HTTPS protocols and a DNS lookup should work.
          using (CustomWebClient client = new CustomWebClient())
          {

            byte[] responseBytes = client.DownloadData(webAddress);
            string response = Encoding.UTF8.GetString(responseBytes);
            tracingService.Trace(response);
            //Log success in the Plugin Trace Log:
            tracingService.Trace("WebClientPlugin completed successfully.");
          }
        }

        catch (WebException exception)
        {
          string str = string.Empty;
          if (exception.Response != null)
          {
            using (StreamReader reader =
                new StreamReader(exception.Response.GetResponseStream()))
            {
              str = reader.ReadToEnd();
            }
            exception.Response.Close();
          }
          if (exception.Status == WebExceptionStatus.Timeout)
          {
            throw new InvalidPluginExecutionException(
                "The timeout elapsed while attempting to issue the request.", exception);
          }
          throw new InvalidPluginExecutionException(String.Format(CultureInfo.InvariantCulture,
              "A Web exception occurred while attempting to issue the request. {0}: {1}",
              exception.Message, str), exception);

          /*
           Expected in the Plugin Trace log when failed:

           Downloading the target URI: http://www.bing.com
           Exception: Microsoft.Xrm.Sdk.InvalidPluginExecutionException: The timeout elapsed while attempting to issue the request. ---> System.Net.WebException: The operation has timed out
             at System.Net.WebClient.DownloadDataInternal(Uri address, WebRequest& request)
             at System.Net.WebClient.DownloadData(Uri address)
             at PowerApps.Samples.WebClientPlugin.Execute(IServiceProvider serviceProvider)
             --- End of inner exception stack trace ---
             at PowerApps.Samples.WebClientPlugin.Execute(IServiceProvider serviceProvider)
          */
        }
      }
      catch (Exception e)
      {
        tracingService.Trace("Exception: {0}", e.ToString());
        throw;
      }
    }
  }
  /// <summary>
  /// A class derived from WebClient with 15 second timeout and KeepAlive disabled
  /// </summary>
  public class CustomWebClient : WebClient
  {
    protected override WebRequest GetWebRequest(Uri address)
    {
      HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
      if (request != null)
      {
        request.Timeout = 15000; //15 Seconds
        request.KeepAlive = false;
        
      }
      return request;
    }
  }
}
