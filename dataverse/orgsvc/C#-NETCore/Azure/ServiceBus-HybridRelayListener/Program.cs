using Microsoft.Azure.Relay;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace RelayListener
{
    /// <summary>
    /// An Azure ServiceBus listener app that listens for the Dataverse remote
    /// execution context posted to a ServiceBus hybrid relay connection endpoint.
    /// </summary>
    /// <remarks>You must configure a hybrid connection in the Azure portal before
    /// running this sample.</remarks>
    class Program
    {

        static async Task Main(string[] args)
        {
            // TODO Set the shared access key value and connection URL as configured in Azure. 
            var tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(
                "RootManageSharedAccessKey", "<shared-access-key>");
            var listener = new HybridConnectionListener(
                new Uri("sb://<namespace>.servicebus.windows.net/<hybrid-connection>"), tokenProvider);

            // An anonymous method that is called when a request is received by the listener.
            listener.RequestHandler = (context) =>
            {
                string requestBody = String.Empty;

                using (var reader = new StreamReader(context.Request.InputStream, Encoding.UTF8))
                {
                    requestBody = reader.ReadToEnd();
                    Console.WriteLine($"Received request body");
                }

                // Send a response string back to Dataverse. A custom Azure-aware plug-in 
                // could make use of the string data.
                var response = JsonConvert.SerializeObject(new { Message = "Hello from the Relay Listener!" });
                var responseJson = JsonConvert.SerializeObject(response);
                var responseBytes = Encoding.UTF8.GetBytes(responseJson);

                context.Response.StatusCode = (System.Net.HttpStatusCode)200;
                context.Response.Headers.Add("Content-Type", "application/json");
                context.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
                context.Response.Close();

                // Deserialize the request body into a RemoteExecutionContext object.
                RemoteExecutionContext remoteContext;

                using (var stringReader = new StringReader(requestBody))
                {
                    using (var xmlReader = XmlReader.Create(stringReader))
                    {
                        var serializer = new DataContractSerializer(typeof(RemoteExecutionContext));

                        var result = serializer.ReadObject(xmlReader);
                        if (result is RemoteExecutionContext tempContext)
                        {
                            remoteContext = tempContext;
                        }
                        else
                        {
                            // Handle the null case appropriately
                            remoteContext = new RemoteExecutionContext(); // or throw an exception
                        }
                    }
                }

                // Output the RemoteExecutionContext data values to the console.
                Utility.Print(remoteContext);
            };

            await listener.OpenAsync();
            Console.WriteLine("Listener opened, press any key to exit...");
            Console.ReadKey();
            await listener.CloseAsync();
        }
    }
}
