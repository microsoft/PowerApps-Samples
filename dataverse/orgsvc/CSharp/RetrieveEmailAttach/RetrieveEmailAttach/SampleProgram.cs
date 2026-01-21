using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    ///////////////////////////////////////
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up

                    #region Demonstrate
                    //Create a query to retrieve attachments.
                    var query = new QueryExpression
                    {
                        EntityName = ActivityMimeAttachment.EntityLogicalName,
                        ColumnSet = new ColumnSet("filename"),

                        //Define the conditions for each attachment.
                        Criteria =
                        {
                            FilterOperator = LogicalOperator.And,
                            Conditions =
                        {
                            //The ObjectTypeCode must be specified, or else the query
                            //defaults to "email" instead of "template".
                            new ConditionExpression
                            {
                                AttributeName = "objecttypecode",
                                Operator = ConditionOperator.Equal,
                                Values = {Template.EntityTypeCode}
                            },
                            //Specify which template we need.
                            new ConditionExpression
                            {
                                AttributeName = "objectid",
                                Operator = ConditionOperator.Equal,
                                Values = {_emailTemplateId}
                            }
                        }
                        }
                    };

                    //Write out the filename of each attachment retrieved.
                    foreach (ActivityMimeAttachment attachment in service.RetrieveMultiple(query).Entities)
                    {
                        Console.WriteLine("Retrieved attachment {0}", attachment.FileName);
                    }
                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up
                }
                #endregion Demonstrate
                #endregion Sample Code
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
