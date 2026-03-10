using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        private static bool prompt = true;
        /// <summary>
        /// Function to set up the sample.
        /// </summary>
        /// <param name="service">Specifies the service to connect to.</param>
        /// 
        private static void SetUpSample(CrmServiceClient service)
        {
            // Check that the current version is greater than the minimum version
            if (!SampleHelpers.CheckVersion(service, new Version("9.0.0.0")))
            {
                //The environment version is lower than version 9.0.0.0
                return;
            }

            CreateRequiredRecords(service);
        }

        private static void CleanUpSample(CrmServiceClient service)
        {
            DeleteRequiredRecords(service, prompt);
        }

        /// <summary>
        /// This method creates any entity records that this sample requires.
        /// Creates the email activity.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            // TODO Set ldappath and domain values only for an on-premise or IFD environment.
            string ldappath = String.Empty;
            string domain = String.Empty;

            // TODO Provide a username plus first and last names for an existing system user to disable.
            // You will be given the option, before the program terminates, to enable the account again.
            _userId = SystemUserProvider.RetrieveSystemUser("someone@myorg.onmicrosoft.com", "Firstname", 
                "Lastname", domain, service, ref ldappath);

            if (_userId.Equals(Guid.Empty))
                throw new Exception("CreateRequiredRecords(): The specified system user could not be retrieved.");
        }


        /// <summary>
        /// Deletes the custom entity record that was created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user 
        /// to delete the entity created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service, bool prompt)
        {
            bool deleteRecords = true;

            if (prompt)
            {
                Console.WriteLine("\nDo you want the system user record enabled? (y/n)");
                String answer = Console.ReadLine();

                deleteRecords = (answer.StartsWith("y") || answer.StartsWith("Y"));
            }

            if (deleteRecords)
            {
                SetStateRequest request = new SetStateRequest()
                {
                    EntityMoniker = new EntityReference(SystemUser.EntityLogicalName,
                            _userId),
                    //sets the user to enabled
                    State = new OptionSetValue(0),
                    // required by request but always valued at -1 in this context
                    Status = new OptionSetValue(-1)
                };
                service.Execute(request);

                Console.WriteLine("System user has been enabled.");
            }
        }

    }
}
