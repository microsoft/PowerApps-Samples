using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Owin;

[assembly: OwinStartup(typeof(ExternalWebApiConsumingPortalOAuthTokenSample.App_Start.Startup))]
namespace ExternalWebApiConsumingPortalOAuthTokenSample.App_Start
{
    public class Startup
    {
		// README file attached in root folder of this solution
        public void Configuration(IAppBuilder app)
        {
			//Registering DynamicsPortalBearerAuthenticationProvider for the path /api/external
			app.Map("/api/external", builder =>
            {
                var options = new OAuthBearerAuthenticationOptions
                {
                    AuthenticationType = DynamicsPortalBearerAuthenticationProvider.AuthenticationType,
                    AuthenticationMode = AuthenticationMode.Active,
                    Provider = new DynamicsPortalBearerAuthenticationProvider()
                };
                builder.UseOAuthBearerAuthentication(options);
            });
        }
    }

    public class DynamicsPortalBearerAuthenticationProvider : IOAuthBearerAuthenticationProvider
    {
        public const string AuthenticationType = "DynamicsPortal";

        private static RsaSecurityKey _signingKey;
        private static string _signingKeyPlainText;

        public DynamicsPortalBearerAuthenticationProvider()
        {
            LoadSigningKey();
        }

        /// <summary>
        /// Handles applying the authentication challenge to the response message.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public Task ApplyChallenge(OAuthChallengeContext context)
        {
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Handles processing OAuth bearer token.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual Task RequestToken(OAuthRequestTokenContext context)
        {
            var idContext = new OAuthValidateIdentityContext(context.OwinContext, null, null);
            this.ValidateIdentity(idContext);
            return Task.FromResult<int>(0);
        }

        /// <summary>
        /// Handles validating the identity produced from an OAuth bearer token.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual Task ValidateIdentity(OAuthValidateIdentityContext context)
        {
			try
			{
				if (!context.Request.Headers.ContainsKey("Authorization"))
				{
					return Task.FromResult<object>(null);
				}

				// Retrieve the JWT token in Authorization Header
				var jwt = context.Request.Headers["Authorization"].Replace("Bearer ", string.Empty);

				var handler = new JwtSecurityTokenHandler();

				var token = new JwtSecurityToken(jwt);

				var claimIdentity = new ClaimsIdentity(token.Claims, DefaultAuthenticationTypes.ExternalBearer);

				var param = new TokenValidationParameters
				{
					ValidateAudience = true, // Make this false if token was generated without clientId
					ValidAudience = "8ebd73587a354f948ec-93260410010c", //Replace with Client Id Registered on CRM. Token should have been fetched with the same clientId.
					ValidateIssuer = true,
					IssuerSigningKey = _signingKey,
					IssuerValidator = (issuer, securityToken, parameters) =>
					{
						var allowed = GetAllowedPortal().Trim().ToLowerInvariant();

						if (issuer.ToLowerInvariant().Equals(allowed))
						{
							return issuer;
						}

						throw new Exception("Token Issuer is not a known Portal");
					}
				};

				SecurityToken validatedToken = null;

				handler.ValidateToken(token.RawData, param, out validatedToken);

				var claimPrincipal = new ClaimsPrincipal(claimIdentity);
				context.Response.Context.Authentication.User = claimPrincipal;
				context.Validated(claimIdentity);
			}
			catch(Exception exception)
			{
				System.Diagnostics.Debug.WriteLine(exception);
				return null;
			}
			return Task.FromResult<object>(null);
		}

        public static string GetAllowedPortal()
        {
            return ConfigurationManager.AppSettings["Microsoft.Dynamics.AllowedPortal"];
        }

        public static string GetAllowedPortalSigningKeyPlainText()
        {
            return _signingKeyPlainText;
        }

        private static string GetAllowedPortalSigningKey()
        {
            return ConfigurationManager.AppSettings["Microsoft.Dynamics.AllowedPortalSigningKey"];
        }

        private static void LoadSigningKey()
        {
            var val = GetAllowedPortalSigningKey();

            if (string.IsNullOrEmpty(val))
            {
                var portalUrl = GetAllowedPortal();
                WebClient client = new WebClient();
                val = client.DownloadString("https://" + portalUrl + "/_services/auth/publickey");
            }

            _signingKeyPlainText = val;

            var x = new PemReader(new StringReader(val));

            var y = (RsaKeyParameters) x.ReadObject();

            var rsaInfo = new RSAParameters
            {
                Modulus = y.Modulus.ToByteArrayUnsigned(),
                Exponent = y.Exponent.ToByteArrayUnsigned()
            };

            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(rsaInfo);

            _signingKey = new RsaSecurityKey(rsa);
        }
    }

}