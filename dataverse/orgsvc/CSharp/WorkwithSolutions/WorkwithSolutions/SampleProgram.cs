using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.IO;
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
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate 

                    //Define a new publisher
                    Publisher _powerappsSdkPublisher = new Publisher
                    {
                        UniqueName = "powerappssamples",
                        FriendlyName = "PowerApps SDK Samples",
                        SupportingWebsiteUrl = "https://learn.microsoft.com/power-apps/developer/data-platform/overview",
                        CustomizationPrefix = "sample",
                        EMailAddress = "someone@microsoft.com",
                        Description = "This publisher was created with samples from the PowerApps SDK"
                    };

                    //Does publisher already exist?
                    QueryExpression querySDKSamplePublisher = new QueryExpression
                    {
                        EntityName = Publisher.EntityLogicalName,
                        ColumnSet = new ColumnSet("publisherid", "customizationprefix"),
                        Criteria = new FilterExpression()
                    };

                    querySDKSamplePublisher.Criteria.AddCondition("uniquename", ConditionOperator.Equal, _powerappsSdkPublisher.UniqueName);
                    EntityCollection querySDKSamplePublisherResults = service.RetrieveMultiple(querySDKSamplePublisher);
                    Publisher SDKSamplePublisherResults = null;

                    //If it already exists, use it
                    if (querySDKSamplePublisherResults.Entities.Count > 0)
                    {
                        SDKSamplePublisherResults = (Publisher)querySDKSamplePublisherResults.Entities[0];
                        _powerappsSdkPublisherId = (Guid)SDKSamplePublisherResults.PublisherId;
                        _customizationPrefix = SDKSamplePublisherResults.CustomizationPrefix;
                    }
                    //If it doesn't exist, create it
                    if (SDKSamplePublisherResults == null)
                    {
                        _powerappsSdkPublisherId = service.Create(_powerappsSdkPublisher);
                        Console.WriteLine(String.Format("Created publisher: {0}.", _powerappsSdkPublisher.FriendlyName));
                        _customizationPrefix = _powerappsSdkPublisher.CustomizationPrefix;
                    }

                    // Retrieve the Default Publisher

                    //The default publisher has a constant GUID value;
                    Guid DefaultPublisherId = new Guid("{d21aab71-79e7-11dd-8874-00188b01e34f}");

                    Publisher DefaultPublisher = (Publisher)service.Retrieve(Publisher.EntityLogicalName, DefaultPublisherId, new ColumnSet(new string[] { "friendlyname" }));

                    EntityReference DefaultPublisherReference = new EntityReference
                    {
                        Id = DefaultPublisher.Id,
                        LogicalName = Publisher.EntityLogicalName,
                        Name = DefaultPublisher.FriendlyName
                    };
                    Console.WriteLine("Retrieved the {0}.", DefaultPublisherReference.Name);

                    // Create a Solution
                    //Define a solution
                    Solution solution = new Solution
                    {
                        UniqueName = "samplesolution",
                        FriendlyName = "Sample Solution",
                        PublisherId = new EntityReference(Publisher.EntityLogicalName, _powerappsSdkPublisherId),
                        Description = "This solution was created by the WorkWithSolutions sample code in the Microsoft Dynamics CRM SDK samples.",
                        Version = "1.0"
                    };

                    //Check whether it already exists
                    QueryExpression queryCheckForSampleSolution = new QueryExpression
                    {
                        EntityName = Solution.EntityLogicalName,
                        ColumnSet = new ColumnSet(),
                        Criteria = new FilterExpression()
                    };
                    queryCheckForSampleSolution.Criteria.AddCondition("uniquename", ConditionOperator.Equal, solution.UniqueName);

                    //Create the solution if it does not already exist.
                    EntityCollection querySampleSolutionResults = service.RetrieveMultiple(queryCheckForSampleSolution);
                    Solution SampleSolutionResults = null;
                    if (querySampleSolutionResults.Entities.Count > 0)
                    {
                        SampleSolutionResults = (Solution)querySampleSolutionResults.Entities[0];
                        _solutionsSampleSolutionId = (Guid)SampleSolutionResults.SolutionId;
                    }
                    if (SampleSolutionResults == null)
                    {
                        _solutionsSampleSolutionId = service.Create(solution);
                    }

                    // Retrieve a solution
                    String solutionUniqueName = "samplesolution";
                    QueryExpression querySampleSolution = new QueryExpression
                    {
                        EntityName = Solution.EntityLogicalName,
                        ColumnSet = new ColumnSet(new string[] { "publisherid", "installedon", "version", "versionnumber", "friendlyname" }),
                        Criteria = new FilterExpression()
                    };

                    querySampleSolution.Criteria.AddCondition("uniquename", ConditionOperator.Equal, solutionUniqueName);
                    Solution SampleSolution = (Solution)service.RetrieveMultiple(querySampleSolution).Entities[0];

                    // Add an existing Solution Component
                    //Add the Account entity to the solution
                    RetrieveEntityRequest retrieveForAddAccountRequest = new RetrieveEntityRequest()
                    {
                        LogicalName = Account.EntityLogicalName
                    };
                    RetrieveEntityResponse retrieveForAddAccountResponse = (RetrieveEntityResponse)service.Execute(retrieveForAddAccountRequest);
                    AddSolutionComponentRequest addReq = new AddSolutionComponentRequest()
                    {
                        ComponentType = (int)componenttype.Entity,
                        ComponentId = (Guid)retrieveForAddAccountResponse.EntityMetadata.MetadataId,
                        SolutionUniqueName = solution.UniqueName
                    };
                    service.Execute(addReq);

                    // Remove a Solution Component
                    //Remove the Account entity from the solution
                    RetrieveEntityRequest retrieveForRemoveAccountRequest = new RetrieveEntityRequest()
                    {
                        LogicalName = Account.EntityLogicalName
                    };
                    RetrieveEntityResponse retrieveForRemoveAccountResponse = (RetrieveEntityResponse)service.Execute(retrieveForRemoveAccountRequest);

                    RemoveSolutionComponentRequest removeReq = new RemoveSolutionComponentRequest()
                    {
                        ComponentId = (Guid)retrieveForRemoveAccountResponse.EntityMetadata.MetadataId,
                        ComponentType = (int)componenttype.Entity,
                        SolutionUniqueName = solution.UniqueName
                    };
                    service.Execute(removeReq);

                    // Export or package a solution
                    //Export an a solution

                    ExportSolutionRequest exportSolutionRequest = new ExportSolutionRequest();
                    exportSolutionRequest.Managed = false;
                    exportSolutionRequest.SolutionName = solution.UniqueName;

                    ExportSolutionResponse exportSolutionResponse = (ExportSolutionResponse)service.Execute(exportSolutionRequest);

                    byte[] exportXml = exportSolutionResponse.ExportSolutionFile;
                    string filename = solution.UniqueName + ".zip";
                    File.WriteAllBytes(outputDir + filename, exportXml);

                    Console.WriteLine("Solution exported to {0}.", outputDir + filename);

                    // Install or Upgrade a Solution                  

                    byte[] fileBytes = File.ReadAllBytes(ManagedSolutionLocation);

                    ImportSolutionRequest impSolReq = new ImportSolutionRequest()
                    {
                        CustomizationFile = fileBytes
                    };

                    service.Execute(impSolReq);

                    Console.WriteLine("Imported Solution from {0}", ManagedSolutionLocation);


                    // Monitor import success
                    byte[] fileBytesWithMonitoring = File.ReadAllBytes(ManagedSolutionLocation);

                    ImportSolutionRequest impSolReqWithMonitoring = new ImportSolutionRequest()
                    {
                        CustomizationFile = fileBytes,
                        ImportJobId = Guid.NewGuid()
                    };

                    service.Execute(impSolReqWithMonitoring);
                    Console.WriteLine("Imported Solution with Monitoring from {0}", ManagedSolutionLocation);

                    ImportJob job = (ImportJob)service.Retrieve(ImportJob.EntityLogicalName, impSolReqWithMonitoring.ImportJobId, new ColumnSet(new System.String[] { "data", "solutionname" }));


                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                    doc.LoadXml(job.Data);

                    String ImportedSolutionName = doc.SelectSingleNode("//solutionManifest/UniqueName").InnerText;
                    String SolutionImportResult = doc.SelectSingleNode("//solutionManifest/result/@result").Value;

                    Console.WriteLine("Report from the ImportJob data");
                    Console.WriteLine("Solution Unique name: {0}", ImportedSolutionName);
                    Console.WriteLine("Solution Import Result: {0}", SolutionImportResult);
                    Console.WriteLine("");

                    // This code displays the results for Global Option sets installed as part of a solution.

                    System.Xml.XmlNodeList optionSets = doc.SelectNodes("//optionSets/optionSet");
                    foreach (System.Xml.XmlNode node in optionSets)
                    {
                        string OptionSetName = node.Attributes["LocalizedName"].Value;
                        string result = node.FirstChild.Attributes["result"].Value;

                        if (result == "success")
                        {
                            Console.WriteLine("{0} result: {1}", OptionSetName, result);
                        }
                        else
                        {
                            string errorCode = node.FirstChild.Attributes["errorcode"].Value;
                            string errorText = node.FirstChild.Attributes["errortext"].Value;

                            Console.WriteLine("{0} result: {1} Code: {2} Description: {3}", OptionSetName, result, errorCode, errorText);
                        }
                    }
                    #endregion Demonstrate
                    #endregion Sample Code

                    
                    DeleteRequiredRecords(service, prompt);
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