using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Http;
using System.Web.Http.Cors;
using ExternalWebApiConsumingPortalOAuthTokenSample.App_Start;

namespace ExternalWebApiConsumingPortalOAuthTokenSample
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var allowedPortal = DynamicsPortalBearerAuthenticationProvider.GetAllowedPortal();

            var cors = new EnableCorsAttribute("https://" + allowedPortal, "*", "*");

            config.EnableCors(cors);

            config.MapHttpAttributeRoutes();
        }
    }
}
