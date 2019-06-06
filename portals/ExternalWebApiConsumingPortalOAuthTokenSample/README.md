# Use Portal OAuth token with an external Web API

This sample is an ASP.NET based project and is used to validate the ID token issued by Microsoft Dynamics 365 for Customer Engagement Portal. 

## Setup

1. Replace the value of `portalUrl` with your Portal URL in the application setting `Microsoft.Dynamics.AllowedPortal`.
```
<add key="Microsoft.Dynamics.AllowedPortal" value="portalUrl"/>
```
2. If the token was fetched from Portal using a ClientId, replace the value of ValidAudience in Startup.cs (L. 103) with a valid ClientId that is registered with Dynamics 365.
3. If the token was not fetched using a ClientId, set the value of ValidateAudience in Startup.cs to `false` . (L. 102)
4. Build the project to fetch all packages from Nuget Store.

## Token Validation

1. Make a `GET` request to ServerURL (e.g: `http://localhost:60717/api/external/WhoAmI`) with Authorization Header having the value "Bearer TokenFetchedFromPortal".
For example: `Key: Authorization Value: Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJ`.
2. You can use a request composer tool like Postman to test this.
3. To decode the ID token, you can visit https://jwt.io.

## Working

* This Project uses a custom BearerAuthenticationProvider called **DynamicsPortalBearerAuthenticationProvider**.
* This provider is registered in StartUp.cs with route `/api/external`.
* All actions for the routes starting with `api/external` are defined in **Controllers/ExternalWebApiController.cs** file. (RoutePrefix used)
* All ID tokens are digitally signed by Portal using its private key.
* **DynamicsPortalBearerAuthenticationProvider** uses the Portal's public key endpoint (/_services/auth/publickey) to validate the token.
* **DynamicsPortalBearerAuthenticationProvider** also validates the audience and issuer of the ID token.
