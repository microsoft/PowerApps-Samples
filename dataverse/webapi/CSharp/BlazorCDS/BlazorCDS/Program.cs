using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BlazorCDS.Models;

namespace BlazorCDS
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            // Get configuration data about the Global Discovery API set in wwwroot/appsettings.json
            var GDSWebApiConfig = builder.Configuration.GetSection("GDSWebAPI");
            var gdsResourceUrl = "https://globaldisco.crm.dynamics.com";

            // Get configuration data about the Web API set in wwwroot/appsettings.json
            var CDSWebApiConfig = builder.Configuration.GetSection("CDSWebAPI");
            var version = CDSWebApiConfig.GetSection("Version").Value;
            var timeoutSeconds = int.Parse(CDSWebApiConfig.GetSection("TimeoutSeconds").Value);

            // Create an named definition of an HttpClient that can be created in a component page
            builder.Services.AddHttpClient("GDSClient", client =>
            {
                client.BaseAddress = new Uri(gdsResourceUrl);
                client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
            });

            // Create an named definition of an HttpClient that can be created in a component page
            builder.Services.AddHttpClient("CDSClient", client =>
            {
                // See https://learn.microsoft.com/powerapps/developer/common-data-service/webapi/compose-http-requests-handle-errors                
                client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
                client.DefaultRequestHeaders.Add("OData-Version", "4.0");
                client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            });

            builder.Services.AddMsalAuthentication(options =>
            {
                builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);

                // Add access to Global Discovery Service to the scope of the access token when the user signs in                
                options.ProviderOptions.DefaultAccessTokenScopes.Add($"{gdsResourceUrl}/user_impersonation");
            });

            builder.Services.AddSingleton(typeof(AppState), new AppState());

            await builder.Build().RunAsync();
        }
    }
}
