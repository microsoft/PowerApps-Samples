# Quick Start Sample: Retrieve Microsoft Dataverse environments using Online Management API 

The C# sample demonstrates how to authenticate to the Online Management API and then retrieve all Dataverse environments from your Office 365 tenant.

The sample uses the authentication [helper code](sample-authentication-helper.md) to easily authenticate to Online Management API using the OAuth 2.0 protocol and pass in the access token in header of your request.

## What this sample does?

The sample performs the following tasks:

1. Uses the **ConnectToAPI** method to connect to the Online Management API.

    a. Calls the **DiscoverAuthority** method in the authentication helper code, and passes in the service URL to get the authority information.

    b. Uses an HttpClient instance to connect to Online Management API service.

    c. Specifies the API service base address and the max period of execution time.
1. Uses the **RetrieveInstancesAsync** method to execute a http request to retrieve all Customer Enagement instances in your Office 365 tenant, and then displays the reponse.

## Run this sample
Before you can run this sample, make sure that you have:
- One of the admin roles in your Office 365 tenant. See [Office 365 Admin roles](get-started-online-management-api.md#office-365-admin-roles)
- Visual Studio 2015 or later; Internet connectivity is required to download/restore assemblies in the NuGet package.
- .NET Framework 4.6.2

To run the sample:
1. Clone or download this repository.
2. Double-click the 'dataverse\online-management-api\RetrieveInstances\SampleAppForOnlineAdminAPI.sln' file to open the solution in Visual Studio.
3. In the **Programs.cs** file, specify a different service URL if the region is not North America. For a list of service URL values for worldwide regions, see [Service URL](get-started-online-management-api.md#service-url).
    ```csharp
    //TODO: Change this value if your Office 365 tenant is in a different region than North America

    private static string _serviceUrl = "https://admin.services.crm.dynamics.com";
    ```
4. In the **HelperCode** > **AuthenticationHelper.cs** file, update the values of the `_clientId` and `_redirectURL` values appropriately.

    ```csharp
    // TODO: Substitute your app registration values here.
    // These values are obtained on registering your application with the 
    // Azure Active Directory.
    private static string _clientId = "<GUID>";    //e.g. "e5cf0024-a66a-4f16-85ce-99ba97a24bb2"
    private static string _redirectUrl = "<Url>";  //e.g. "app://e5cf0024-a66a-4f16-85ce-99ba97a24bb2"
    ```
5. Save changes, and then start debugging by pressing F5 or select **Debug** > **Start Debugging**.


### Related Topics  

[Get started with Online Management API](https://learn.microsoft.com/powerapps/developer/common-data-service/online-management-api/get-started-online-management-api)

[Online Management API Reference](https://learn.microsoft.com/rest/api/admin.services.crm.dynamics.com/)