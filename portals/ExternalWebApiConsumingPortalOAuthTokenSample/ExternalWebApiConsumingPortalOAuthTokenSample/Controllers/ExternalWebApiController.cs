using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using ExternalWebApiConsumingPortalOAuthTokenSample.App_Start;

namespace ExternalWebApiConsumingPortalOAuthTokenSample.Controllers
{
    [Authorize]
    [HostAuthentication(DynamicsPortalBearerAuthenticationProvider.AuthenticationType)]
    [RoutePrefix("api/external")]
    public class ExternalWebApiController : ApiController
    {
        [HttpGet]
        [Route("ping")]
        public string Ping()
        {
            return "Pong";
        }

        [HttpGet]
        [Route("whoami")]
        public string WhoAmI()
        {
            var user = HttpContext.Current.User;

            if (!(user is ClaimsPrincipal claim)) return "Unknown User";

            var claimInfo = claim.FindFirst("lp_sdes");

            var portalClaims = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ClaimInfo>>(claimInfo.Value);

            return
                $"{claim.FindFirst("given_name").Value} {claim.FindFirst("family_name").Value} (Email={claim.FindFirst("email").Value}, CustomerId={portalClaims[0].info.customerId}, UserName={portalClaims[0].info.userName})";
        }

        public class LastPaymentDate
        {
            public int day { get; set; }
            public int month { get; set; }
            public int year { get; set; }
        }

        public class RegistrationDate
        {
            public int day { get; set; }
            public int month { get; set; }
            public int year { get; set; }
        }

        public class Info
        {
            public object cstatus { get; set; }
            public string ctype { get; set; }
            public string customerId { get; set; }
            public object balance { get; set; }
            public object socialId { get; set; }
            public string imei { get; set; }
            public string userName { get; set; }
            public object companySize { get; set; }
            public object accountName { get; set; }
            public object role { get; set; }
            public LastPaymentDate lastPaymentDate { get; set; }
            public RegistrationDate registrationDate { get; set; }
        }

        public class ClaimInfo
        {
            public string type { get; set; }
            public Info info { get; set; }
        }
    }
}