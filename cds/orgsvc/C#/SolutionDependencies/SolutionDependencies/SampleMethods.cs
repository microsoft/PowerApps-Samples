using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
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
        private static Guid _publisherId;
        private const string _primarySolutionName = "PrimarySolution";
        private static Guid _primarySolutionId;
        private static Guid _secondarySolutionId;
        private  const String _prefix = "test";
        private  const String _globalOptionSetName = _prefix + "_exampleoptionset";
        private static Guid? _globalOptionSetId;
        private  const String _picklistName = _prefix + "_examplepicklist";
        private const int _languageCode = 1033;
        private static bool prompt = true;
        /// <summary>
        /// Function to set up the sample.
        /// </summary>
        /// <param name="service">Specifies the service to connect to.</param>
        /// 
        private static void SetUpSample(CrmServiceClient service)
        {
            // Check that the current version is greater than the minimum version
            if (!SampleHelpers.CheckVersion(service, new Version("7.1.0.0")))
            {
                //The environment version is lower than version 7.1.0.0
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
        /// Create a publisher
        /// Create a new solution, "Primary"
        /// Create a Global Option Set in solution "Primary"
        /// Export the "Primary" solution, setting it to Protected
        /// Delete the option set and solution
        /// Import the "Primary" solution, creating a managed solution in CRM.
        /// Create a new solution, "Secondary"
        /// Create an attribute in "Secondary" that references the Global Option Set
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            //Create the publisher that will "own" the two solutions
            Publisher publisher = new Publisher
            {
                UniqueName = "samplepublisher",
                FriendlyName = "An Example Publisher",
                Description = "This is an example publisher",
                CustomizationPrefix = _prefix
            };

            _publisherId = service.Create(publisher);
            //Create the primary solution - note that we are not creating it 
            //as a managed solution as that can only be done when exporting the solution.
            Solution primarySolution = new Solution
            {
                Version = "1.0",
                FriendlyName = "Primary Solution",
                PublisherId = new EntityReference(Publisher.EntityLogicalName, _publisherId),
                UniqueName = _primarySolutionName
            };
             _primarySolutionId = service.Create(primarySolution);
            //Now, create the Global Option Set and associate it to the solution.
            OptionSetMetadata optionSetMetadata = new OptionSetMetadata()
            {
                Name = _globalOptionSetName,
                DisplayName = new Label("Example Option Set", _languageCode),
                IsGlobal = true,
                OptionSetType = OptionSetType.Picklist,
                Options =
            {
                new OptionMetadata(new Label("Option 1", _languageCode), 1),
                new OptionMetadata(new Label("Option 2", _languageCode), 2)
            }
            };
            CreateOptionSetRequest createOptionSetRequest = new CreateOptionSetRequest
            {
                OptionSet = optionSetMetadata
            };

            createOptionSetRequest.SolutionUniqueName = "PrimarySolution";
            service.Execute(createOptionSetRequest);
            //Export the solution as managed so that we can later import it.
            ExportSolutionRequest exportRequest = new ExportSolutionRequest
            {
                Managed = true,
                SolutionName = "PrimarySolution"
            };
            ExportSolutionResponse exportResponse =
                (ExportSolutionResponse)service.Execute(exportRequest);
            // Delete the option set previous created, so it can be imported under the
            // managed solution.
            DeleteOptionSetRequest deleteOptionSetRequest = new DeleteOptionSetRequest
            {
                Name = _globalOptionSetName
            };
            service.Execute(deleteOptionSetRequest);
            // Delete the previous primary solution, so it can be imported as managed.
            service.Delete(Solution.EntityLogicalName, _primarySolutionId);
            _primarySolutionId = Guid.Empty;

            // Re-import the solution as managed.
            ImportSolutionRequest importRequest = new ImportSolutionRequest
            {
                CustomizationFile = exportResponse.ExportSolutionFile
            };
            service.Execute(importRequest);
        
            // Retrieve the solution from CRM in order to get the new id.
            QueryByAttribute primarySolutionQuery = new QueryByAttribute
            {
                EntityName = Solution.EntityLogicalName,
                ColumnSet = new ColumnSet("solutionid"),
                Attributes = { "uniquename" },
                Values = { "PrimarySolution" }
            };
            _primarySolutionId = service.RetrieveMultiple(primarySolutionQuery).Entities
                .Cast<Solution>().FirstOrDefault().SolutionId.GetValueOrDefault();


            // Create a secondary solution.
            Solution secondarySolution = new Solution
            {
                Version = "1.0",
                FriendlyName = "Secondary Solution",
                PublisherId = new EntityReference(Publisher.EntityLogicalName, _publisherId),
                UniqueName = "SecondarySolution"
            };
             _secondarySolutionId = service.Create(secondarySolution);

            // Create a Picklist attribute in the secondary solution linked to the option set in the
            // primary - see WorkWithOptionSets.cs for more on option sets.
            PicklistAttributeMetadata picklistMetadata = new PicklistAttributeMetadata
            {
                SchemaName = _picklistName,
                LogicalName = _picklistName,
                DisplayName = new Label("Example Picklist", _languageCode),
                RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                OptionSet = new OptionSetMetadata
                {
                    IsGlobal = true,
                    Name = _globalOptionSetName
                }

            };

            CreateAttributeRequest createAttributeRequest = new CreateAttributeRequest
            {
                EntityName = Contact.EntityLogicalName,
                Attribute = picklistMetadata
            };
            createAttributeRequest["SolutionUniqueName"] = secondarySolution.UniqueName;
            service.Execute(createAttributeRequest);
        }

        /// <summary>
        /// Deletes any entity records that were created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user to delete the records created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service,bool prompt)
        {
            bool deleteRecords = true;

            if (prompt)
            {
                Console.WriteLine("\nDo you want these entity records deleted? (y/n)");
                String answer = Console.ReadLine();

                deleteRecords = (answer.StartsWith("y") || answer.StartsWith("Y"));
            }

            if (deleteRecords)
            {

                DeleteAttributeRequest deleteAttributeRequest = new DeleteAttributeRequest
                {
                    EntityLogicalName = Contact.EntityLogicalName,
                    LogicalName = "sample" + "_examplepicklist"
                };
                service.Execute(deleteAttributeRequest);
                service.Delete(Solution.EntityLogicalName, _primarySolutionId);
                service.Delete(Solution.EntityLogicalName, _secondarySolutionId);
                service.Delete(Publisher.EntityLogicalName, _publisherId);


                Console.WriteLine("Entity records have been deleted.");
            }
        }

        /// <summary>
        /// Shows how to get a more friendly message based on information within the dependency
        /// <param name="dependency">A Dependency returned from the RetrieveDependentComponents message</param>
        /// </summary> 
        public static void DependencyReport(CrmServiceClient service, Dependency dependency)
        {
            //These strings represent parameters for the message.
            String dependentComponentName = "";
            String dependentComponentTypeName = "";
            String dependentComponentSolutionName = "";
            String requiredComponentName = "";
            String requiredComponentTypeName = "";
            String requiredComponentSolutionName = "";

            //The ComponentType global Option Set contains options for each possible component.
            RetrieveOptionSetRequest componentTypeRequest = new RetrieveOptionSetRequest
            {
                Name = "componenttype"
            };

            RetrieveOptionSetResponse componentTypeResponse = (RetrieveOptionSetResponse)service.Execute(componentTypeRequest);
            OptionSetMetadata componentTypeOptionSet = (OptionSetMetadata)componentTypeResponse.OptionSetMetadata;
            // Match the Component type with the option value and get the label value of the option.
            foreach (OptionMetadata opt in componentTypeOptionSet.Options)
            {
                if (dependency.DependentComponentType.Value == opt.Value)
                {
                    dependentComponentTypeName = opt.Label.UserLocalizedLabel.Label;
                }
                if (dependency.RequiredComponentType.Value == opt.Value)
                {
                    requiredComponentTypeName = opt.Label.UserLocalizedLabel.Label;
                }
            }
            //The name or display name of the compoent is retrieved in different ways depending on the component type
            dependentComponentName = getComponentName(service, dependency.DependentComponentType.Value, (Guid)dependency.DependentComponentObjectId);
            requiredComponentName = getComponentName(service, dependency.RequiredComponentType.Value, (Guid)dependency.RequiredComponentObjectId);

            // Retrieve the friendly name for the dependent solution.
            Solution dependentSolution = (Solution)service.Retrieve
             (
              Solution.EntityLogicalName,
              (Guid)dependency.DependentComponentBaseSolutionId,
              new ColumnSet("friendlyname")
             );
            dependentComponentSolutionName = dependentSolution.FriendlyName;

            // Retrieve the friendly name for the required solution.
            Solution requiredSolution = (Solution)service.Retrieve
              (
               Solution.EntityLogicalName,
               (Guid)dependency.RequiredComponentBaseSolutionId,
               new ColumnSet("friendlyname")
              );
            requiredComponentSolutionName = requiredSolution.FriendlyName;

            //Display the message
            Console.WriteLine("The {0} {1} in the {2} depends on the {3} {4} in the {5} solution.",
            dependentComponentName,
            dependentComponentTypeName,
            dependentComponentSolutionName,
            requiredComponentName,
            requiredComponentTypeName,
            requiredComponentSolutionName);
        }

        // The name or display name of the component depends on the type of component.
        public static String getComponentName(CrmServiceClient service, int ComponentType, Guid ComponentId)
        {
            String name = "";

            switch (ComponentType)
            {
                // A separate method is required for each type of component
                case (int)componenttype.Attribute:
                    name = getAttributeInformation(service,ComponentId);
                    break;
                case (int)componenttype.OptionSet:
                    name = getGlobalOptionSetName(service, ComponentId);
                    break;
                default:
                    name = "not implemented";
                    break;
            }

            return name;

        }

        // Retrieve the display name and parent entity information about an attribute solution component.
        public static string getAttributeInformation(CrmServiceClient service,Guid id)
        {
            String attributeInformation = "";
            RetrieveAttributeRequest req = new RetrieveAttributeRequest
            {
                MetadataId = id
            };
            RetrieveAttributeResponse resp = (RetrieveAttributeResponse)service.Execute(req);

            AttributeMetadata attmet = resp.AttributeMetadata;

            attributeInformation = attmet.EntityLogicalName + " : " + attmet.DisplayName.UserLocalizedLabel.Label;


            return attributeInformation;
        }
        //Retrieve the name of a global Option set
        public static String getGlobalOptionSetName(CrmServiceClient service, Guid id)
        {
            String name = "";
            RetrieveOptionSetRequest req = new RetrieveOptionSetRequest
            {
                MetadataId = id
            };
            RetrieveOptionSetResponse resp = (RetrieveOptionSetResponse)service.Execute(req);
            OptionSetMetadataBase os = (OptionSetMetadataBase)resp.OptionSetMetadata;
            name = os.DisplayName.UserLocalizedLabel.Label;
            return name;
        }

    }
}
