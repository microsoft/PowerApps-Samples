using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
            if (String.IsNullOrEmpty(config))
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
                    // Download the target URI using a Web client. Any .NET class that uses the
                    // HTTP or HTTPS protocols and a DNS lookup should work.
                    using (WebClient client = new WebClient())
                    {
                        byte[] responseBytes = client.DownloadData(webAddress);
                        string response = Encoding.UTF8.GetString(responseBytes);
                        tracingService.Trace(response);

                        // For demonstration purposes, throw an exception so that the response
                        // is shown in the trace dialog of the Microsoft Dynamics CRM user interface.
                        throw new InvalidPluginExecutionException("WebClientPlugin completed successfully.");
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
