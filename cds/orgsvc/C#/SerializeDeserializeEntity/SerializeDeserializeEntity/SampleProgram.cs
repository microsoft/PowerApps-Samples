using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        [STAThread] // Required to support the interactive login experience
        static void Main(string[] args)
        {
            CrmServiceClient service = null;
            try
            {
                service = SampleHelpers.Connect("Connect");
                if (service.IsReady)
                {
                    // Create any entity records that the demonstration code requires
                    SetUpSample(service);
                    #region Demonstrate
                    // Create the column set object that indicates the fields to be retrieved.
                    var columns = new ColumnSet(
                        "contactid",
                        "firstname",
                        "lastname",
                        "jobtitle");

                    // Retrieve the contact using the ID of the record that was just created.
                    // The EntityLogicalName indicates the EntityType of the object being retrieved.
                    var contact = (Contact)service.Retrieve(
                        Contact.EntityLogicalName, _contactId, columns);

                    Console.WriteLine("The contact for the sample has been retrieved.");

                    // Serialize the contact into XML and write it to the hard drive.
                    var earlyBoundSerializer = new DataContractSerializer(typeof(Contact));

                    // Create a unique file name for the XML.
                    String earlyboundFile = "Contact_early_" + contact.ContactId.Value.ToString("B") + ".xml";

                    // Write the serialized object to a file.  The using statement will
                    // ensure that the FileStream is disposed of correctly.  The FileMode
                    // will ensure that the file is overwritten if it already exists.
                    using (var file = new FileStream(earlyboundFile, FileMode.Create))
                    {
                        // Write the XML to disk.
                        earlyBoundSerializer.WriteObject(file, contact);
                    }

                    Console.WriteLine(
                        "The early-bound contact instance has been serialized to a file, {0}.",
                        earlyboundFile);

                    // Convert the contact to a late-bound entity instance and serialize it to disk.
                    var lateboundContact = contact.ToEntity<Entity>();
                    String lateboundFile = "Contact_late_" + lateboundContact.Id.ToString("B") + ".xml";

                    var lateBoundSerializer = new DataContractSerializer(typeof(Entity));
                    // Write the serialized object to a file.
                    using (var file = new FileStream(lateboundFile, FileMode.Create))
                    {
                        lateBoundSerializer.WriteObject(file, lateboundContact);
                    }

                    Console.WriteLine(
                        "The late-bound contact instance has been serialized to a file, {0}.",
                        lateboundFile);
                    Contact deserializedContact = null;
                    using (var file = new FileStream(earlyboundFile, FileMode.Open))
                    {
                        deserializedContact = (Contact)earlyBoundSerializer.ReadObject(file);
                        Console.WriteLine("The contact has been de-serialized: {0} {1}",
                            deserializedContact.FirstName, deserializedContact.LastName);
                    }

                    // Update the contact to prove that the de-serialization worked.
                    deserializedContact.JobTitle = "Plumber";
                    service.Update(deserializedContact);

                    Console.WriteLine("The contact was updated.");

                    #endregion Demonstrate

                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up
                }
                else
                {
                    const string UNABLE_TO_LOGIN_ERROR = "Unable to Login to Microsoft Dataverse";
                    if (service.LastCrmError.Equals(UNABLE_TO_LOGIN_ERROR))
                    {
                        Console.WriteLine("Check the connection string values in cds/App.config.");
                        throw new Exception(service.LastCrmError);
                    }
                    else
                    {
                        throw service.LastCrmException;
                    }
                }
            }
            catch (Exception ex)
            {
                SampleHelpers.HandleException(ex);
            }

            finally
            {
                if (service != null)
                    service.Dispose();

                Console.WriteLine("Press <Enter> to exit.");
                Console.ReadLine();
            }

        }
    }
}
