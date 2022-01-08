using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        [STAThread] // Added to support UX
        static void Main(string[] args)
        {
            CrmServiceClient service = null;
            try
            {
                service = SampleHelpers.Connect("Connect");
                if (service.IsReady)
                {
                    #region Sample Code
                    //////////////////////////////////////////////
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate

                    // Create a custom string attribute for the appointment instance
                    StringAttributeMetadata customAppointmentInstanceAttribute = new StringAttributeMetadata
                    {
                        LogicalName = "new_customAppInstanceAttribute",
                        DisplayName = new Microsoft.Xrm.Sdk.Label("CustomAppInstanceAttribute", 1033),
                        Description = new Microsoft.Xrm.Sdk.Label("Sample Custom Appointment Instance Attribute", 1033),
                        MaxLength = 500,
                        RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                        SchemaName = "new_customAppInstanceAttribute"
                    };

                    CreateAttributeRequest instanceAttributeRequest = new CreateAttributeRequest
                    {
                        Attribute = customAppointmentInstanceAttribute,
                        EntityName = "appointment"
                    };

                    CreateAttributeResponse instanceAttributeResponse = (CreateAttributeResponse)service.Execute(instanceAttributeRequest);
                    _instanceAttributeID = instanceAttributeResponse.AttributeId;

                    // Create a custom string attribute for the recurring appointment master (series)
                    StringAttributeMetadata customAppointmentSeriesAttribute = new StringAttributeMetadata
                    {
                        LogicalName = "new_customAppSeriesAttribute",
                        DisplayName = new Microsoft.Xrm.Sdk.Label("CustomAppSeriesAttribute", 1033),
                        Description = new Microsoft.Xrm.Sdk.Label("Sample Custom Appointment Series Attribute", 1033),
                        MaxLength = 500,
                        RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                        SchemaName = "new_customAppSeriesAttribute",
                        LinkedAttributeId = _instanceAttributeID // Link the custom attribute to the appointment’s custom attribute.
                    };

                    CreateAttributeRequest seriesAttributeRequest = new CreateAttributeRequest
                    {
                        Attribute = customAppointmentSeriesAttribute,
                        EntityName = "recurringappointmentmaster"
                    };

                    CreateAttributeResponse seriesAttributeResponse = (CreateAttributeResponse)service.Execute(seriesAttributeRequest);
                    _seriesAttributeID = seriesAttributeResponse.AttributeId;

                    // Publish all the changes to the solution.
                    PublishAllXmlRequest createRequest = new PublishAllXmlRequest();
                    service.Execute(createRequest);

                    Console.WriteLine("Created a custom string attribute, {0}, for the appointment.", customAppointmentInstanceAttribute.LogicalName);
                    Console.WriteLine("Created a custom string attribute, {0}, for the recurring appointment, and linked it with {1}.", customAppointmentSeriesAttribute.LogicalName, customAppointmentInstanceAttribute.LogicalName);


                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up
                }
                #endregion Demonstrate
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
            #endregion Sample Code
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
