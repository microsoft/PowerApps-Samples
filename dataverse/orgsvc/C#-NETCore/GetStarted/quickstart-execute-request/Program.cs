using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;

class Program
{
    // TODO Enter your Dataverse environment's URL and logon info.
    static string url = "https://org619826b5.crm.dynamics.com";
    static string userName = "someone@myenv.onmicrosoft.com";
    static string password = "password";

    // This service connection string uses the info provided above.
    // The AppId and RedirectUri are provided for sample code testing.
    static string connectionString = $@"
    AuthType = OAuth;
    Url = {url};
    UserName = {userName};
    Password = {password};
    AppId = 51f81489-12ee-4a9e-aaae-a2591f45987d;
    RedirectUri = app://58145B91-0C36-4500-8554-080854F2AC97;
    LoginPrompt=Auto;
    RequireNewInstance = True";

    static void Main(string[] args)
    {
        using (ServiceClient serviceClient = new(connectionString))
        {
            if (serviceClient.IsReady)
            {
                WhoAmIResponse response = 
                    (WhoAmIResponse)serviceClient.Execute(new WhoAmIRequest());

                Console.WriteLine("User ID is {0}.", response.UserId);
            }
            else
            {
                Console.WriteLine(
                    "A web service connection was not established.");
            }
        }

        // Pause the console so it does not close.
        Console.WriteLine("Press the <Enter> key to exit.");
        Console.ReadLine();
    }
}