using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace PowerApps.Samples
{
    /// <summary>
    /// Stage and asynchronously import a solution. Environment variables and
    /// connection references in the solution are supported.
    /// </summary>
    /// <see cref="https://docs.microsoft.com/power-platform/alm/solution-api"/>
    class Program
    {
        static void Main(string[] args)
        {
            string solutionFile = @"Contoso_sample_1_0_0_1_managed.zip";

            // Authenticate the user in Microsoft Dataverse
            var service = SampleHelpers.Connect("Connect");
            if (!service.IsReady)
                throw service.LastCrmException;

            // Stage the solution and check the solution validation results
            var stagingResults = StageSolution(service, solutionFile);

            if (stagingResults.StageSolutionStatus == StageSolutionStatus.Failed)
            {
                Console.WriteLine("The solution staging failed.");

                var validationResults = stagingResults.SolutionValidationResults;
                // TODO Check or log solution validation results
            }

            else // Staging was a success
            {
                Console.WriteLine("The solution staging completed successfully.");

                // Import the solution and check the import status
                var response = ImportSolution(service, stagingResults);
                CheckImportStatus(service, response.AsyncOperationId, Guid.Parse(response.ImportJobKey));
            }

            // Pause program execution by waiting for a key press.
            Console.ReadKey();
        }

        /// <summary>
        /// Stage and validate a solution.
        /// </summary>
        /// <param name="service">The Organization service on the 2011 SOAP endpoint.</param>
        /// <param name="solutionFilePath">The path to the compressed solution file.</param>
        /// <returns>The results from staging the solution.</returns>
        //<snippet_stage-solution>
        public static StageSolutionResults StageSolution(IOrganizationService service, string solutionFilePath)
        {
            // Stage the solution
            var req = new StageSolutionRequest();

            byte[] fileBytes = File.ReadAllBytes(solutionFilePath);
            req["CustomizationFile"] = fileBytes;
            var res = service.Execute(req);

            return (res["StageSolutionResults"] as StageSolutionResults);
        }
        //</snippet_stage-solution>

        /// <summary>
        /// Import a staged solution.
        /// </summary>
        /// <param name="service">The Organization service on the 2011 SOAP endpoint.</param>
        /// <param name="stagingResults">The results from staging the solution.</param>
        /// <returns>The response returned from the service after processing the import request.</returns>
        //<snippet_import-solution-async>
        public static ImportSolutionAsyncResponse ImportSolution(IOrganizationService service, StageSolutionResults stagingResults)
        {
            // Import the staged solution
            var componentDetails = stagingResults.SolutionComponentsDetails;

            // TODO These are not referenced in the code but are usefull to explore
            var missingDependencies = stagingResults.MissingDependencies;   // Contains missing dependencies
            var solutionDetails = stagingResults.SolutionDetails;           // Contains solution details

            var connectionReferences = componentDetails.Where(x => string.Equals(x.ComponentTypeName, "connectionreference"));
            var envVarDef = componentDetails.Where(x => string.Equals(x.ComponentTypeName, "environmentvariabledefinition"));
            var envVarValue = componentDetails.Where(x => string.Equals(x.ComponentTypeName, "environmentvariablevalue"));

            var componentParams = new EntityCollection();

            // Add each connection reference to the component parmameters entity collection.
            foreach (var conn in connectionReferences)
            {
                var e = new Entity("connectionreference")
                {
                    ["connectionreferencelogicalname"] = conn.Attributes["connectionreferencelogicalname"].ToString(),
                    ["connectionreferencedisplayname"] = conn.Attributes["connectionreferencedisplayname"].ToString(),
                    ["connectorid"] = conn.Attributes["connectorid"].ToString(),
                    ["connectionid"] = "custom input"
                };
                componentParams.Entities.Add(e);
            }
            
            // Add each environment variable to the component parmameters entity collection.
            foreach (var value in envVarValue)
            {
                var e = new Entity("environmentvariablevalue")
                {
                    ["schemaname"] = value.Attributes["schemaname"].ToString(),
                    ["value"] = "custom input"
                };

                if (value.Attributes.ContainsKey("environmentvariablevalueid"))
                {
                    e["environmentvariablevalueid"] = value.Attributes["environmentvariablevalueid"].ToString();
                }
                componentParams.Entities.Add(e);
            }

            // Import the solution
            var importSolutionReq = new ImportSolutionAsyncRequest();
            importSolutionReq.ComponentParameters = componentParams;
            importSolutionReq.SolutionParameters = new SolutionParameters { StageSolutionUploadId = stagingResults.StageSolutionUploadId };
            var response = service.Execute(importSolutionReq) as ImportSolutionAsyncResponse;

            return (response);
        }
        //</snippet_import-solution-async>

        /// <summary>
        /// Check the solution import status.
        /// </summary>
        /// <param name="service">The Organization service on the 2011 SOAP endpoint.</param>
        /// <param name="asyncOperationId">The identifier of the asynchronous job performing the solution import.</param>
        /// <param name="importJobKey">The key that identifies the import job.</param>
        //<snippet_check-import-status>
        public static void CheckImportStatus(IOrganizationService service, Guid asyncOperationId, Guid importJobKey)
        {
            // Get solution import status
            var finished = false;
            Entity asyncOperation = null;

            // Wait until the async job is finished
            while (!finished)
            {
                asyncOperation = service.Retrieve("asyncoperation", asyncOperationId, new ColumnSet("statecode", "statuscode"));
                OptionSetValue statecode = (OptionSetValue)asyncOperation["statecode"];

                if (statecode.Value == 3)
                {
                    finished = true;
                }
                else
                {
                    Thread.Sleep(10000);
                }
            }

            // Solution import completed successfully
            OptionSetValue statuscode = (OptionSetValue)asyncOperation["statuscode"];
            if (statuscode.Value == 30)
            {
                Console.WriteLine("The solution import completed successfully.");
            }

            else if (asyncOperation["statuscode"].ToString() == "31")  // Solution import failed
            {
                Console.WriteLine("The solution import failed.");

                var getLogReq = new RetrieveFormattedImportJobResultsRequest { ImportJobId = importJobKey };
                var importJob = service.Execute(getLogReq) as RetrieveFormattedImportJobResultsResponse;
                // TODO Do something with the import job results
            }
        }
        //</snippet_check-import-status>
    }
}
