# Sample: Blazor WebAssembly with Global Discovery

This sample shows how to use the Global Discovery service from a Blazor WebAssembly application.

This sample is a modification of the steps in the [Tutorial: Create an ASP.NET Core Blazor WebAssembly app using Microsoft Dataverse](https://learn.microsoft.com/power-apps/developer/data-platform/walkthrough-blazor-webassembly-single-tenant)

You can try this sample online here: [https://blazorcds.mohsinonxrm.com/](https://blazorcds.mohsinonxrm.com/)

## How to run this sample

1. Download or clone the [Samples](https://github.com/microsoft/PowerApps-Samples) repo so that you have a local copy.
1. Follow the instructions in [Setup](#setup) to create an Azure Active Directory app registration and run the sample.

## What this sample does

This sample demonstrates how to provide a select (drop-down) control in a web application to allow users to switch environments that they have access to.

## How this sample works

The `Pages/EnvironmentChooser.razor` file provides the user interface to allow users to select from available environments based on their credentials after they have logged in.

This page depends on a named definition of an HttpClient (`GDSClient`) in  Program.cs that is configured to access the Global Discovery Service. Access to the Global Discovery Service is added to the available scopes when the user logs in. When the user selects a different environment, the `SelectedEnvUrl` in `Models/AppState.cs` changes.

Access to the account records is provided by `Pages/FetchAccounts.razor` with an access token that depends on the selected environment.

### Setup

To run this sample you must first configure an Azure Active Directory application on your tenant and update the `BlazorCDS\wwwroot\appsettings.json` file to replace the placeholder `ClientId` value with the application (client) Id. Use the following steps:

#### Create an app registration

1. Go to [Azure](https://portal.azure.com/).
1. Select **Azure Active Directory**.
1. Go To **App registrations**.
1. Select **New Registration**.
1. Enter the following:

   |Field|Value|
   |---------|---------|
   |Name|Your choice. You might use: `BlazorGlobalDiscoverySample`|
   |Supported account types:|Accounts in this organization directory only (Single Tenant)|
   |Redirect URI (optional)|`https://localhost:44363/authentication/login-callback`|

1. Click **Register**.
1. Copy the **Application (client) ID** value. You will need this when you [Update the project](#update-the-project).

#### Configure Authentication

1. Go to **Authentication**.
1. Under **Implicit grant and hybrid flows**, select both of the following options:

   - Access tokens (used for implicit flows)
   - ID tokens (used for implicit and hybrid flows)

1. Click **Save**.

#### Configure Permissions

1. Go to **API permissions**.
1. Click **Add a permission**.
1. In the **Request API permissions** fly-out, under the**Microsoft APIs** tab, select **Dynamics CRM**.
1. Under **Select permissions**, select **user_impersonation**.
1. Click **Add permissions**.
1. Under **Configured permissions**, click **Grant admin consent** for {Your tenant name.}
1. In the **Grant admin consent** confirmation dialog, click **Yes**.

#### Update the project

1. Open the BlazorCDS Visual Studio solution file (BlazorCDS.sln) with Visual Studio.
1. In **Solution Explorer**, expand `wwwroot` and open the `appsettings.json` file.
1. Replace the placeholder `ClientId` value (`11111111-1111-1111-1111-111111111111`) with the application (client) ID value you copied in [Create an app registration](#create-an-app-registration).

#### Run the sample

In Visual Studio, click **IIS Express** to run the sample.

### Demonstrate

1. In the browser window that opens with the app, click **Log in**.
1. Enter your credentials and you will see a select control with the label: **Choose an Environment**. The select control should list all the environments you have access to with the credentials you provided.
1. Click **Fetch Accounts**. This should display a list of account records you have access to view in the selected environment.
1. If you have more than one environment to choose from, select a different environment and the list of account records will be refreshed to represent the records available in that environment.

### Clean up

This sample makes no changes to data in your environment.

### See Also

[Discover user organizations](https://learn.microsoft.com/power-apps/developer/data-platform/discovery-service)<br />
[Sample: Global Discovery Service (C#)](https://learn.microsoft.com/power-apps/developer/data-platform/sample-global-discovery-service-csharp)
