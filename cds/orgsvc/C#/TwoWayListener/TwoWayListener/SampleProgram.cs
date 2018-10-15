using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.ServiceBus;

namespace PowerApps.Samples
{
    /// <summary>
    /// Creates a two-way endpoint listening for messages from the Windows Azure Service
    /// Bus.
    /// </summary>
    public class TwoWayListener
    {
        #region How-To Sample Code

        /// <summary>
        /// The Execute method is called when a message is posted to the Azure Service
        /// Bus.
        /// </summary>
        [ServiceBehavior]
        private class TwoWayEndpoint : ITwoWayServiceEndpointPlugin
        {
            #region ITwoWayServiceEndpointPlugin Member

            /// <summary>
            /// This method is called when a message is posted to the Azure Service Bus.
            /// </summary>
            /// <param name="context">Data for the request.</param>
            /// <returns>A 'Success' string.</returns>
            public string Execute(RemoteExecutionContext context)
            {
                Utility.Print(context);

                // This is the return value that can be used within Dynamics
                return "Success";
            }

            #endregion
        }

        /// <summary>
        /// Prompts for required information and hosts a service until the user ends the 
        /// session.
        /// </summary>
        public void Run()
        {
            ServiceBusEnvironment.SystemConnectivity.Mode = ConnectivityMode.Http;

            // The one specified when creating the azure bus
            Console.Write("Enter your Azure service namespace: ");
            string serviceNamespace = Console.ReadLine();

            // The shared access key policy name
            Console.Write("Enter your shared access policy name: ");
            string sharedAccesKeyName = Console.ReadLine();

            // The primary of they access key policy specificied above
            Console.Write("Enter your shared access policy key: ");
            string sharedAccessKey = Console.ReadLine();

            // Input the same path that was specified in the Service Bus Configuration dialog
            // when registering the Azure-aware plug-in with the Plug-in Registration tool.
            Console.Write("Enter your endpoint path: ");
            string servicePath = Console.ReadLine();

            // Leverage the Azure API to create the correct URI.
            Uri address = ServiceBusEnvironment.CreateServiceUri(
                Uri.UriSchemeHttps,
                serviceNamespace,
                servicePath);

            Console.WriteLine("The service address is: " + address);

            // Create the shared secret credentials object for the endpoint matching the 
            // Azure access control services issuer 
            var sharedSecretServiceBusCredential = new TransportClientEndpointBehavior()
            {
                TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(sharedAccesKeyName, sharedAccessKey)
            };

            // Using an HTTP binding instead of a SOAP binding for this endpoint.
            WS2007HttpRelayBinding binding = new WS2007HttpRelayBinding();
            binding.Security.Mode = EndToEndSecurityMode.Transport;

            // Create the service host for Azure to post messages to.
            ServiceHost host = new ServiceHost(typeof(TwoWayEndpoint));
            host.AddServiceEndpoint(typeof(ITwoWayServiceEndpointPlugin), binding, address);

            // Create the ServiceRegistrySettings behavior for the endpoint.
            var serviceRegistrySettings = new ServiceRegistrySettings(DiscoveryType.Public);

            // Add the service bus credentials to all endpoints specified in configuration.

            foreach (var endpoint in host.Description.Endpoints)
            {
                endpoint.Behaviors.Add(serviceRegistrySettings);
                endpoint.Behaviors.Add(sharedSecretServiceBusCredential);
            }

            // Begin listening for messages posted to Azure.
            host.Open();

            Console.WriteLine(Environment.NewLine + "Listening for messages from Azure" +
                Environment.NewLine + "Press [Enter] to exit");

            // Keep the listener open until Enter is pressed.
            Console.ReadLine();

            Console.Write("Closing the service host...");
            host.Close();
            Console.WriteLine(" done.");
        }

        /// <summary>
        /// Containts methods to display the RemoteExecutionContext provided when a 
        /// message is posted to the Azure Service Bus.
        /// </summary>
        private static class Utility
        {
            /// <summary>
            /// Writes out the RemoteExecutionContext to the Console.
            /// </summary>
            /// <param name="context"></param>
            public static void Print(RemoteExecutionContext context)
            {
                Console.WriteLine("----------");
                if (context == null)
                {
                    Console.WriteLine("Context is null.");
                    return;
                }

                Console.WriteLine("UserId: {0}", context.UserId);
                Console.WriteLine("OrganizationId: {0}", context.OrganizationId);
                Console.WriteLine("OrganizationName: {0}", context.OrganizationName);
                Console.WriteLine("MessageName: {0}", context.MessageName);
                Console.WriteLine("Stage: {0}", context.Stage);
                Console.WriteLine("Mode: {0}", context.Mode);
                Console.WriteLine("PrimaryEntityName: {0}", context.PrimaryEntityName);
                Console.WriteLine("SecondaryEntityName: {0}", context.SecondaryEntityName);

                Console.WriteLine("BusinessUnitId: {0}", context.BusinessUnitId);
                Console.WriteLine("CorrelationId: {0}", context.CorrelationId);
                Console.WriteLine("Depth: {0}", context.Depth);
                Console.WriteLine("InitiatingUserId: {0}", context.InitiatingUserId);
                Console.WriteLine("IsExecutingOffline: {0}", context.IsExecutingOffline);
                Console.WriteLine("IsInTransaction: {0}", context.IsInTransaction);
                Console.WriteLine("IsolationMode: {0}", context.IsolationMode);
                Console.WriteLine("Mode: {0}", context.Mode);
                Console.WriteLine("OperationCreatedOn: {0}", context.OperationCreatedOn.ToString());
                Console.WriteLine("OperationId: {0}", context.OperationId);
                Console.WriteLine("PrimaryEntityId: {0}", context.PrimaryEntityId);
                Console.WriteLine("OwningExtension LogicalName: {0}", context.OwningExtension.LogicalName);
                Console.WriteLine("OwningExtension Name: {0}", context.OwningExtension.Name);
                Console.WriteLine("OwningExtension Id: {0}", context.OwningExtension.Id);
                Console.WriteLine("SharedVariables: {0}", (context.SharedVariables == null
                    ? "NULL" : SerializeParameterCollection(context.SharedVariables)));
                Console.WriteLine("InputParameters: {0}", (context.InputParameters == null
                    ? "NULL" : SerializeParameterCollection(context.InputParameters)));
                Console.WriteLine("OutputParameters: {0}", (context.OutputParameters == null
                    ? "NULL" : SerializeParameterCollection(context.OutputParameters)));
                Console.WriteLine("PreEntityImages: {0}", (context.PreEntityImages == null
                    ? "NULL" : SerializeEntityImageCollection(context.PreEntityImages)));
                Console.WriteLine("PostEntityImages: {0}", (context.PostEntityImages == null
                    ? "NULL" : SerializeEntityImageCollection(context.PostEntityImages)));
                Console.WriteLine("----------");
            }

            /// <summary>
            /// Writes out the attributes of an entity.
            /// </summary>
            /// <param name="e">The entity to serialize.</param>
            /// <returns>A human readable representation of the entity.</returns>
            private static string SerializeEntity(Entity e)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0} LogicalName: {1}{0} EntityId: {2}{0} Attributes: [",
                    Environment.NewLine,
                    e.LogicalName,
                    e.Id);
                foreach (KeyValuePair<string, object> parameter in e.Attributes)
                {
                    sb.AppendFormat("{0}: {1}; ", parameter.Key, parameter.Value);
                }
                sb.Append("]");
                return sb.ToString();
            }

            /// <summary>
            /// Flattens a collection into a delimited string.
            /// </summary>
            /// <param name="parameterCollection">The values must be of type Entity 
            /// to print the values.</param>
            /// <returns>A string representing the collection passed in.</returns>
            private static string SerializeParameterCollection(ParameterCollection parameterCollection)
            {
                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<string, object> parameter in parameterCollection)
                {
                    Entity e = parameter.Value as Entity;
                    if (e != null)
                    {
                        sb.AppendFormat("{0}: {1}", parameter.Key, SerializeEntity(e));
                    }
                    else
                    {
                        sb.AppendFormat("{0}: {1}; ", parameter.Key, parameter.Value);
                    }
                }
                return sb.ToString();
            }

            /// <summary>
            /// Flattens a collection into a delimited string.
            /// </summary>
            /// <param name="entityImageCollection">The collection to flatten.</param>
            /// <returns>A string representation of the collection.</returns>
            private static string SerializeEntityImageCollection(EntityImageCollection entityImageCollection)
            {
                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<string, Entity> entityImage in entityImageCollection)
                {
                    sb.AppendFormat("{0}{1}: {2}", Environment.NewLine, entityImage.Key,
                        SerializeEntity(entityImage.Value));
                }
                return sb.ToString();
            }
        }

        #endregion How-To Sample Code

        /// <summary>
        /// Standard Main() method used by most SDK samples.
        /// </summary>
        static public void Main()
        {
            try
            {
                TwoWayListener app = new TwoWayListener();
                app.Run();
            }
            catch (FaultException<ServiceEndpointFault> ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine("Message: {0}", ex.Detail.Message);
                Console.WriteLine("Inner Fault: {0}",
                    null == ex.InnerException.Message ? "No Inner Fault" : "Has Inner Fault");
            }
            catch (System.TimeoutException ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine("Message: {0}", ex.Message);
                Console.WriteLine("Stack Trace: {0}", ex.StackTrace);
                Console.WriteLine("Inner Fault: {0}",
                    null == ex.InnerException.Message ? "No Inner Fault" : ex.InnerException.Message);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine(ex.Message);

                // Display the details of the inner exception.
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);

                    FaultException<ServiceEndpointFault> fe = ex.InnerException
                        as FaultException<ServiceEndpointFault>;
                    if (fe != null)
                    {
                        Console.WriteLine("Message: {0}", fe.Detail.Message);
                        Console.WriteLine("Inner Fault: {0}",
                            null == ex.InnerException.Message ? "No Inner Fault" : "Has Inner Fault");
                    }
                }
            }

            finally
            {
                Console.WriteLine("Press <Enter> to exit.");
                Console.ReadLine();
            }
        }
    }
}