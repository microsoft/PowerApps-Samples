using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public class OAuthMessageHandler : DelegatingHandler
    {
        private readonly ServiceConfig config;
        private readonly UserPasswordCredential _credential = null;
        private readonly AuthenticationContext _authContext;

        public OAuthMessageHandler(ServiceConfig configParam,
                HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            config = configParam;

            if (config.UserPrincipalName != null && config.Password != null)
            {
                _credential = new UserPasswordCredential(config.UserPrincipalName, config.Password);
            }
            _authContext = new AuthenticationContext(config.Authority, false);


        }
        /// <summary>
        /// Will refresh the ADAL AccessToken when it expires.
        /// </summary>
        /// <returns></returns>
        private AuthenticationHeaderValue GetAuthHeader()
        {
            AuthenticationResult authResult;
            if (_credential == null)
            {
                authResult = _authContext.AcquireTokenAsync(
                    config.Url, config.ClientId, 
                    new Uri(config.RedirectUrl), 
                    new PlatformParameters(PromptBehavior.Auto)).Result;
            }
            else
            {
                authResult = _authContext.AcquireTokenAsync(config.Url, config.ClientId, _credential).Result;
            }
            return new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
        }

        /// <summary>
        /// Overrides the default HttpClient.SendAsync operation so that authentication can be done.
        /// </summary>
        /// <param name="request">The request to send</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override Task<HttpResponseMessage> SendAsync(
                  HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                request.Headers.Authorization = GetAuthHeader();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}
