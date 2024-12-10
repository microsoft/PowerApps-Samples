using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;

/// <summary>
/// Confidential client authentication followed by a web service request.
/// Use of a client secret in the app registration for authentication.
/// </summary>
/// <remarks>You must create an Entra ID app registration for this sample to work.</remarks>
/// <see cref="https://learn.microsoft.com/power-apps/developer/data-platform/walkthrough-register-app-azure-active-directory"/>
class Program
{
    // TODO Enter your Dataverse environment's URL, ClientId, and Secret.
    static string url = "https://yourorg.crm.dynamics.com";

    static string connectionString = $@"
    AuthType = ClientSecret;
    Url = {url};
    ClientId = 345917f8-00d5-460e-bc1a-000000000000;
    Secret = 98q8Q~000000000~L9uayrinXGaPnfvl.l2w.abT";

    static void Main()
    {
        // ServiceClient class implements IOrganizationService interface
        IOrganizationService service = new ServiceClient(connectionString);

        var response = (WhoAmIResponse)service.Execute(new WhoAmIRequest());

        Console.WriteLine($"Application ID is {response.UserId}.");

        // Pause the console so it does not close.
        Console.WriteLine("Press the <Enter> key to exit.");
        Console.ReadLine();
    }
}