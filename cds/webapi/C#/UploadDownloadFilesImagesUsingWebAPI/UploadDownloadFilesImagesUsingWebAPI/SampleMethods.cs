using Microsoft.Xrm.Tooling.Connector;
using System;
using System.IO;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        /// <summary>
        /// Function to setup the sample
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entityLogicalName">Logical Name of the entity</param>
        /// <param name="columnLogicalName">Image column name</param>
        private static void SetUpSample(CrmServiceClient service, string solutionName)
        {
            // Check that the current version is greater than the minimum version
            if (!SampleHelpers.CheckVersion(service, new Version("7.1.0.0")))
            {
                return;
            }
            Console.WriteLine($"\nConnected to CRM\n", ConsoleColor.Green);
            var importSolutionPath = Path.GetFullPath("Solutions\\PreparedSolution.zip");
            SampleHelpers.ImportSolution(_service, solutionName, importSolutionPath);
        }
    }
}
