using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;

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
                    // Create and Retrieve Offline Filter
                    // In your Outlook client, this will appear in the System Filters tab
                    // under File | CRM | Synchronize | Outlook Filters.
                    Console.Write("Creating offline filter");
                    String contactName = String.Format("offlineFilteredContact {0}",
                        DateTime.Now.ToLongTimeString());
                    String fetchXml = String.Format("<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\"><entity name=\"contact\"><attribute name=\"contactid\" /><filter type=\"and\">" +
                        "<condition attribute=\"ownerid\" operator=\"eq-userid\" /><condition attribute=\"description\" operator=\"eq\" value=\"{0}\" />" +
                        "<condition attribute=\"statecode\" operator=\"eq\" value=\"0\" /></filter></entity></fetch>", contactName);
                    SavedQuery filter = new SavedQuery();
                    filter.FetchXml = fetchXml;
                    filter.IsQuickFindQuery = false;
                    filter.QueryType = SavedQueryQueryType.OfflineFilters;
                    filter.ReturnedTypeCode = Contact.EntityLogicalName;
                    filter.Name = "ReadOnlyFilter_" + contactName;
                    filter.Description = "Sample offline filter for Contact entity";
                    _offlineFilter = service.Create(filter);

                    Console.WriteLine(" and retrieving offline filter");
                    var result = (SavedQuery)service.Retrieve(
                        SavedQuery.EntityLogicalName,
                        _offlineFilter,
                        new ColumnSet("name", "description"));
                    Console.WriteLine("Name: {0}", result.Name);
                    Console.WriteLine("Description: {0}", result.Description);
                    Console.WriteLine();

                    // Create and Retrieve Offline Template
                    // In your Outlook client, this will appear in the User Filters tab
                    // under File | CRM | Synchronize | Outlook Filters.
                    Console.Write("Creating offline template");
                    String accountName = String.Format("offlineFilteredAccount {0}",
                        DateTime.Now.ToLongTimeString());
                    fetchXml = String.Format("<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\"><entity name=\"account\"><attribute name=\"accountid\" /><filter type=\"and\">" +
                        "<condition attribute=\"ownerid\" operator=\"eq-userid\" /><condition attribute=\"name\" operator=\"eq\" value=\"{0}\" />" +
                        "<condition attribute=\"statecode\" operator=\"eq\" value=\"0\" /></filter></entity></fetch>", accountName);
                    var template = new SavedQuery();
                    template.FetchXml = fetchXml;
                    template.IsQuickFindQuery = false;
                    template.QueryType = SavedQueryQueryType.OfflineTemplate;
                    template.ReturnedTypeCode = Account.EntityLogicalName;
                    template.Name = "ReadOnlyFilter_" + accountName;
                    template.Description = "Sample offline template for Account entity";
                    _offlineTemplate = service.Create(template);

                    Console.WriteLine(" and retrieving offline template");
                    result = (SavedQuery)service.Retrieve(
                        SavedQuery.EntityLogicalName,
                        _offlineTemplate,
                        new ColumnSet("name", "description"));
                    Console.WriteLine("Name: {0}", result.Name);
                    Console.WriteLine("Description: {0}", result.Description);
                    Console.WriteLine();
                  
                 // Call InstantiateFiltersRequest
                    Console.WriteLine("Retrieving user's ID and creating the template collection");
                    var whoAmI = new WhoAmIRequest();
                    Guid id = ((WhoAmIResponse)service.Execute(whoAmI)).UserId;
                    EntityReferenceCollection templates = new EntityReferenceCollection();
                    templates.Add(new EntityReference(
                        SavedQuery.EntityLogicalName,
                        _offlineTemplate));

                    Console.WriteLine("Activating the selected offline templates for this user");
                    var request = new InstantiateFiltersRequest
                    {
                        UserId = id,
                        TemplateCollection = templates
                    };
                    var response = (InstantiateFiltersResponse)service.Execute(request);
                    Console.WriteLine();
                 
                    // Call ResetUserFiltersRequest
                    Console.WriteLine("Resetting the user's offline templates to the defaults");
                    var resetRequest = new ResetUserFiltersRequest
                    {
                        QueryType = SavedQueryQueryType.OfflineFilters
                    };
                    var resetResponse = (ResetUserFiltersResponse)service.Execute(resetRequest);
                    Console.WriteLine();
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
