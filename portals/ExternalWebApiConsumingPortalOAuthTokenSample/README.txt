Description:

ExternalApiConsumingPortalOAuthTokenSample is an ASP.NET based project, used to validate the ID token issued by Portal. 

Setup:
* Replace the value of the appSetting Microsoft.Dynamics.AllowedPortal with your portal url.
<add key="Microsoft.Dynamics.AllowedPortal" value="YourPortalURL"/>
* If the token was fetched from Portal using a ClientId, replace the value of ValidAudience in Startup.cs (L. 103) with the valid clientId registered on CRM.
* If the token was not fetched using a ClientId, set the value of ValidateAudience in Startup.cs to false (L. 102)
* Build the project to fetch all packages from Nuget Store

Token Validation:
* Make a GET request to ServerURL (e.g: http://localhost:60717/api/external/WhoAmI) with Authorization Header having the value "Bearer TokenFetchedFromPortal"
 eg: Key: Authorization Value: Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJ
* You can use Postman to test this.
* For decoding the ID token you can visit https://jwt.io
 

Working:
* This Project uses a custom BearerAuthenticationProvider - DynamicsPortalBearerAuthenticationProvider.
* This provider is registered in StartUp.cs with route "/api/external"
* All actions for the routes starting with "api/external" are defined in ExternalWebApiController (RoutePrefix used)
* All ID tokens are digitally signed by Portal using its private key.
* DynamicsPortalBearerAuthenticationProvider uses the Portal's public key endpoint (/_services/auth/publickey) to validate the token.
* DynamicsPortalBearerAuthenticationProvider also validates the audience and issuer of the ID token.