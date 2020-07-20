using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Collections.Generic;
using System.Linq;


namespace MetaViz
{
    //
    // MetadataDownloader is responsible for downloading all necessary metadata from the organization, as well as populating all reportings and metadata data structure.
    // Its nextStageToExecute keeps what metadata needs to be downloaded next. Upon dowload the response will be parsed then next download initiates.
    //

    internal class MetadataDownloader
    {
        private StreamWriter htmlWriter;    // trigger dump report in html
        private StreamWriter logWriter;     // log file
        private StreamWriter specWriter;    // scheme dump report in txt
        private StreamWriter jsonWriter;    // ER json file

        private DateTime reportStartDateTime;

        private enum DumpStage
        {
            Initialize,
            FetchSolutions,
            FetchSolutionComponents,
            FetchEntities,
            FetchWorkflows,
            FetchPluginTypes,
            FetchSdkSteps,
            FetchFullSpecReports,
            Completed
        }
        private DumpStage nextStageToExecute = DumpStage.Initialize;

        #region Fetch XMLs
        // https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/entities/solution
        private const string URL_SOLUTIONS = "/solutions?fetchXml=";
        private const string URL_SOLUTIONS_FETCHXML = @"<fetch no-lock='true'>
<entity name='solution'>
<attribute name='solutionid' />
<attribute name='uniquename' />
<attribute name='friendlyname' />
<attribute name='installedon' />
<attribute name='modifiedon' />
<attribute name='ismanaged' />
<attribute name='description' />
<attribute name='version' />
<link-entity name='publisher' from='publisherid' to='publisherid' alias='publisher'>
<attribute name='uniquename' alias='publishername' />
</link-entity>
</entity>
</fetch>";

        // https://docs.microsoft.com/en-us/dynamics365/customer-engagement/web-api/solutioncomponent?view=dynamics-ce-odata-9
        private const string URL_SOLUTIONCOMPONENTS = "/solutioncomponents?$select=objectid,_solutionid_value";

        // https://docs.microsoft.com/en-us/powerapps/developer/common-data-service/webapi/retrieve-metadata-name-metadataid
        private const string URL_ENTITIES = "/EntityDefinitions?$select=MetadataId,DisplayName,LogicalName,PrimaryIdAttribute,PrimaryNameAttribute,EntitySetName,ObjectTypeCode,Description,IsIntersect";

        // https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/entities/workflow
        // workflow type 1 Definition 2 Activation 3 Template
        // showing 1 Definition here, accuracy wise 2 Activation more makes sense (if we think workflow published different time 'different') but duplications make folks more confused
        private const string URL_WORKFLOWS = "/workflows?$filter=type%20eq%201&$select=workflowid,name,uniquename,primaryentity,mode,triggeroncreate,triggerondelete,triggeronupdateattributelist,createstage,deletestage,updatestage,description,solutionid,ismanaged,asyncautodelete,statecode,statuscode,componentstate,subprocess";

        // https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/entities/plugintype
        // Excludes InternalOperationPlugin -> "plugintypeid": "29efe3a5-047c-40a3-b89c-2787eff38eee", "typename": "Microsoft.Crm.Extensibility.InternalOperationPlugin", 
        private const string URL_PLUGINTYPES = "/plugintypes?fetchXml=";
        private const string URL_PLUGINTYPES_FETCHXML = @"<fetch no-lock='true'>
<entity name='plugintype'>
<all-attributes />
<link-entity name='pluginassembly' from='pluginassemblyid' to='pluginassemblyid' alias='assembly'>
<attribute name='name' alias='pluginassemblyname' />
<attribute name='description' alias='pluginassemblyescription' />
<attribute name='isolationmode' alias='assemblyisolationmode' />
</link-entity>
<filter>
{0}
</filter>
</entity>
</fetch>";

        // https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/entities/sdkmessageprocessingstep
        private const string URL_SDKSTEPS = "/sdkmessageprocessingsteps?fetchXml=";
        private const string URL_SDKSTEPS_FETCHXML = @"<fetch no-lock='true'>
<entity name='sdkmessageprocessingstep'>
<attribute name='sdkmessageprocessingstepid' />
<attribute name='createdon' />
<attribute name='createdby' />
<attribute name='statecode' />
<attribute name='statuscode' />
<attribute name='mode' />
<attribute name='name' />
<attribute name='description' />
<attribute name='stage' />
<attribute name='rank' />
<attribute name='plugintypeid' />
<attribute name='eventhandler' />
<attribute name='asyncautodelete' />
<attribute name='modifiedon' />
<attribute name='filteringattributes' />
<attribute name='ishidden' />
<link-entity name='sdkmessage' from='sdkmessageid' to='sdkmessageid' alias='messsage'>
<attribute name='name' alias='MessageName' />
</link-entity>
<link-entity name='sdkmessagefilter' from='sdkmessagefilterid' to='sdkmessagefilterid' alias='step'>
<attribute name='primaryobjecttypecode' alias='primaryobjecttype'/>
</link-entity>
<filter>
<condition attribute='plugintypeid' operator='ne' value='29efe3a5-047c-40a3-b89c-2787eff38eee' />
</filter>
</entity>
</fetch>";
        #endregion

        #region Html Header
        private string HtmlHeader()
        {
            return @"<!DOCTYPE html>
<html>
<head>
<meta charset='utf-8' />
<title>Metadata Visualizer - " + OrganizationUrl + @"</title>
<style>
body {
font-family: Segoe UI,Arial,sans-serif;
}
table {
border-collapse: collapse;
}
th {
background: #817865;
color: #fff;
font-weight: bold;
}
td, th {
padding: 4px;
border: 1px solid #ddd;
font-size: 0.9em;
}
td.smallfontcell
{
    font-size: 0.7em;
    width: 100px;
}
</style>
</head>
<body>";
        }
        #endregion

        private JToken solutions = null;
        private JToken solutionComponents = null;
        private JToken entities = null;
        private List<string> entitiesWithChildren = new List<string>(); // logical names for entities that have registered workflows or plugins
        private Dictionary<string, List<string>> pluginTypeIdToEntityLogicalName = new Dictionary<string, List<string>>();  // plugin type id -> entities plugins are registered
        private JToken workflows = null;
        private JToken pluginTypes = null;
        private JToken sdkSteps = null;

        private readonly bool fetchAllEntities = false;
        private List<EntityMetadata> allEntitiesMetadata;
        private int fetchAllEntitiesTargetIndex = 0;

        internal string FetchAllEntitiesProgressText()
        {
            if ((allEntitiesMetadata == null) || (nextStageToExecute != DumpStage.FetchFullSpecReports)) return string.Empty;
            return $"Downloading entities {fetchAllEntitiesTargetIndex + 1} / {allEntitiesMetadata.Count}";
        }

        private readonly string OrganizationUrl;
        internal readonly string OutputFolderPath;
        private readonly string OutputFileNamePrefix;
        internal readonly string OutputFileFullPath;

        private bool cancelled;
        internal bool Cancelled { get => (cancelled || exceptionCaught != null); set => cancelled = value; }

        private Exception exceptionCaught;
        internal Exception ExceptionCaught { get => exceptionCaught; set => exceptionCaught = value; }

        private string endpointVersion = "9.0";
        internal string EndpointVersion { get => endpointVersion; set => endpointVersion = value; }

        internal string WEBAPIURL { get => $"/api/data/v{EndpointVersion}"; }

        internal string WHOAMIURL { get => $"{WEBAPIURL}/WhoAmI"; }

        internal MetadataDownloader(string organizationUrl, string outputFolder, bool fetchAllEntities)
        {
            this.OrganizationUrl = organizationUrl;
            this.fetchAllEntities = fetchAllEntities;
            this.OutputFolderPath = outputFolder;
            this.OutputFileNamePrefix = string.Format("MetaViz-{0}_{1:yyyyMMddHHmm}", organizationUrl.Substring(organizationUrl.IndexOf("://") + 3).Replace('.', '_').Replace('/', '_'), DateTime.UtcNow);
            this.OutputFileFullPath = Path.Combine(OutputFolderPath, OutputFileNamePrefix);
        }

        internal string GetNextWebApiUrl()
        {
            if (nextStageToExecute == DumpStage.Initialize)
            {
                // this is the first time this API is called, initialize things then move the stage to first stage
                StartReport();
                nextStageToExecute = DumpStage.FetchSolutions;
            }

            string webApiUrlToExecute = null;
            switch (nextStageToExecute)
            {
                case DumpStage.FetchSolutions:
                    webApiUrlToExecute = WEBAPIURL + URL_SOLUTIONS + System.Web.HttpUtility.UrlEncode(URL_SOLUTIONS_FETCHXML);
                    break;
                case DumpStage.FetchSolutionComponents:
                    webApiUrlToExecute = WEBAPIURL + URL_SOLUTIONCOMPONENTS;
                    break;
                case DumpStage.FetchEntities:
                    webApiUrlToExecute = WEBAPIURL + URL_ENTITIES;
                    break;
                case DumpStage.FetchWorkflows:
                    webApiUrlToExecute = WEBAPIURL + URL_WORKFLOWS;
                    break;
                case DumpStage.FetchPluginTypes:
                    string criteria = @"<condition attribute = 'plugintypeid' operator='ne' value='29efe3a5-047c-40a3-b89c-2787eff38eee' />";
                    webApiUrlToExecute = WEBAPIURL + URL_PLUGINTYPES + System.Web.HttpUtility.UrlEncode(string.Format(URL_PLUGINTYPES_FETCHXML, criteria));
                    break;
                case DumpStage.FetchSdkSteps:
                    webApiUrlToExecute = WEBAPIURL + URL_SDKSTEPS + System.Web.HttpUtility.UrlEncode(URL_SDKSTEPS_FETCHXML);
                    break;
                case DumpStage.FetchFullSpecReports:
                    webApiUrlToExecute = allEntitiesMetadata[fetchAllEntitiesTargetIndex].MetadataUrl;
                    break;
                case DumpStage.Completed:
                    // return null as next url, which ends the interactions
                    CloseStream(logWriter);
                    CloseStream(htmlWriter);
                    CloseStream(specWriter);
                    CloseStream(jsonWriter);
                    return null;
                default:
                    throw new NotImplementedException(nextStageToExecute.ToString());
            }
            webApiUrlToExecute = string.Format("{0}{1}", OrganizationUrl, webApiUrlToExecute);
            logWriter.WriteLine(webApiUrlToExecute);
            logWriter.WriteLine(new string('-', 10));
            return webApiUrlToExecute;
        }

        private void CloseStream(StreamWriter writer)
        {
            if (writer == null) return;
            writer.Flush();
            writer.Close();
            writer = null;
        }

        internal void HandleResponse(string response)
        {
            if (nextStageToExecute != DumpStage.FetchFullSpecReports)
            {
                logWriter.WriteLine(response);
                logWriter.WriteLine(new string('=', 10));
                logWriter.Flush();
            }
            switch (nextStageToExecute)
            {
                case DumpStage.FetchSolutions:
                    solutions = HandleResponseJson(response);
                    nextStageToExecute = DumpStage.FetchSolutionComponents;
                    break;
                case DumpStage.FetchSolutionComponents:
                    solutionComponents = HandleResponseJson(response);
                    nextStageToExecute = DumpStage.FetchEntities;
                    break;
                case DumpStage.FetchEntities:
                    entities = HandleResponseJson(response);
                    nextStageToExecute = DumpStage.FetchWorkflows;
                    break;
                case DumpStage.FetchWorkflows:
                    workflows = HandleResponseJson(response);
                    nextStageToExecute = DumpStage.FetchPluginTypes;
                    break;
                case DumpStage.FetchPluginTypes:
                    pluginTypes = HandleResponseJson(response);
                    nextStageToExecute = DumpStage.FetchSdkSteps;
                    break;
                case DumpStage.FetchSdkSteps:
                    sdkSteps = HandleResponseJson(response);
                    DumpHtmlReport();   // all objects for html report are avaialble.
                    // If fullspec report is requested we got work on execution of those, otherwise job is done
                    nextStageToExecute = (fetchAllEntities ? DumpStage.FetchFullSpecReports : DumpStage.Completed);
                    break;
                case DumpStage.FetchFullSpecReports:
                    // iterating through the target entity object one by one and dump the full report
                    allEntitiesMetadata[fetchAllEntitiesTargetIndex].DumpReport(response, specWriter);
                    fetchAllEntitiesTargetIndex++;
                    //if (fetchAllEntitiesTargetIndex < 20) // for debuging just dump 20 instead all 700+ entities
                    if (fetchAllEntitiesTargetIndex < allEntitiesMetadata.Count)
                    {
                        // until all objects are completed keep repeating
                        nextStageToExecute = DumpStage.FetchFullSpecReports;
                    }
                    else
                    {
                        // now all entities are dumped, dump all the relations
                        specWriter.WriteLine("===|" + new string('=', EntityMetadata.SEPARATOR_LENGTH));
                        ERInformationUtil.AggregateERInformationForAllEntities(OrganizationUrl, allEntitiesMetadata, jsonWriter, specWriter);
                        nextStageToExecute = DumpStage.Completed;
                    }
                    break;
                case DumpStage.Completed:
                    throw new InvalidOperationException("No response handling for Completed state");
                default:
                    throw new NotImplementedException(nextStageToExecute.ToString());
            }
        }

        private JToken HandleResponseJson(string response)
        {
            JToken root = JsonConvert.DeserializeObject<JObject>(response);
            if (root["error"] != null)
            {
                throw new WebApiResponseException(root["error"].ToString());
            }

            return root["value"];
        }

        private void StartReport()
        {
            reportStartDateTime = DateTime.UtcNow;
            string header = $"Metadata Visualizer - " + OrganizationUrl + $" at {reportStartDateTime.ToString("yyyy-MM-dd HH:mm:ss")}";

            htmlWriter = new StreamWriter(new FileStream(OutputFileFullPath + ".htm", FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
            logWriter = new StreamWriter(new FileStream(OutputFileFullPath + ".log", FileMode.Append, FileAccess.Write, FileShare.ReadWrite));

            htmlWriter.WriteLine(HtmlHeader());

            htmlWriter.WriteLine($"<h1>{header}</h1>");
            htmlWriter.WriteLine($"<h4><a href='./{OutputFileNamePrefix}.log' target='_blank'>Log file</a></h4>");

            if (!fetchAllEntities) return;

            allEntitiesMetadata = new List<EntityMetadata>();

            htmlWriter.WriteLine($"<h4><a href='./{OutputFileNamePrefix}.txt' target='_blank'>Entities and Attributes</a></h4>");
            specWriter = new StreamWriter(new FileStream(OutputFileFullPath + ".txt", FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
            specWriter.WriteLine(header);

            jsonWriter = new StreamWriter(new FileStream(OutputFileFullPath + ".json", FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
        }

        private void DumpHtmlReport()
        {
            htmlWriter.WriteLine("<h2>Entities</h2>");
            DumpEntities();

            htmlWriter.WriteLine("<h2>Entities and Plugins/Workflows</h2>");
            foreach (JObject entity in entities.OrderBy(obj => (string)obj["LogicalName"]))
            {
                DumpEntityAndChildren(entity);
            }

            htmlWriter.WriteLine("<h2>Solutions and Plugins/Workflows</h2>");
            DumpSolutionAndChildren();

            htmlWriter.WriteLine("<hr /><p>MetaViz completed at UTC {1:yyyy-MM-dd HH:mm:ss}. This report generation took {0} seconds. Please send your feedback and ideas to hianzai@microsoft.com for this report.</p>", (DateTime.Now - reportStartDateTime).TotalSeconds, DateTime.Now);
            htmlWriter.WriteLine("</body></html>");
            logWriter.Flush();
            htmlWriter.Flush();
        }

        private void DumpEntities()
        {
            htmlWriter.WriteLine("<table>");

            htmlWriter.WriteLine("<tr>");
            htmlWriter.Write("<th>Logical(EntitySet)Name</th><th>Description</th><th>OTC</th><th>Primary</th><th>DisplayNames</th><th>Sync Plugin</th><th>Async Plugin</th><th>Realtime WF</th><th>Workflow</th>");
            htmlWriter.WriteLine("</tr>");

            foreach (JObject entity in entities.OrderBy(obj => (string)obj["LogicalName"]))
            {
                EntityMetadata entityMetadata = DumpEntity(entity);
                if (fetchAllEntities) allEntitiesMetadata.Add(entityMetadata);
            }

            htmlWriter.WriteLine("</table>");
        }

        private EntityMetadata DumpEntity(JObject entity)
        {
            // find all "Label" under DisplayName to show display names used by the customer
            string displayNamesText = FindLabelsUtil.FindLabels(entity, "DisplayName");

            // Likewise for Description
            string descriptiontext = FindLabelsUtil.FindLabels(entity, "Description");

            int objectTypeCode = entity["ObjectTypeCode"].Value<int>();
            string metadataId = entity["MetadataId"].Value<string>();
            string entityLogicalName = entity["LogicalName"].Value<string>();
            string entityLogicalNameHtml = entityLogicalName;
            string entitySetName = entity["EntitySetName"].Value<string>();
            string primaryIdAttribute = entity["PrimaryIdAttribute"].Value<string>();
            string primaryNameAttribute = entity["PrimaryNameAttribute"].Value<string>();
            bool isIntersect = entity["IsIntersect"].Value<bool>();

            // Capture the information per entity for the full spec dump
            EntityMetadata entityMetadata = new EntityMetadata(OrganizationUrl, WEBAPIURL, metadataId, entityLogicalName, entitySetName, descriptiontext, objectTypeCode, primaryIdAttribute, primaryNameAttribute, displayNamesText, isIntersect);

            //0 Synchronous
            //1 Asynchronous
            int syncPlugin = sdkSteps.Where(obj => ((string)obj["primaryobjecttype"] == entityLogicalName && (int)obj["mode"] == 0)).Count<JToken>();
            int asyncPlugin = sdkSteps.Where(obj => ((string)obj["primaryobjecttype"] == entityLogicalName && (int)obj["mode"] == 1)).Count<JToken>();
            //0 Background
            //1 Real - time
            int syncWorkflow = workflows.Where(obj => (string)obj["primaryentity"] == entityLogicalName && (int)obj["mode"] == 1).Count<JToken>();
            int asyncWorkflow = workflows.Where(obj => (string)obj["primaryentity"] == entityLogicalName && (int)obj["mode"] == 0).Count<JToken>();
            int total = syncPlugin + asyncPlugin + syncWorkflow + asyncWorkflow;
            if (total > 0)
            {
                // if there is any registered plugin or workflow, dump a link to the details
                entitiesWithChildren.Add(entityLogicalName);
                entityLogicalNameHtml = string.Format("<a href='#entity_{0}'>{0}</a>", entityLogicalName);
            }

            // interesect (m to m) relationship entity is not useful for this level of reporting. dump report only if it is not intersect.
            if (!isIntersect)
            {
                string linkHtml = $"<a href='{entityMetadata.AttributesUrl}' target='_blank'>attributes</a> <a href='{entityMetadata.EntityListUrl}' target='_blank'>entity list</a>";
                htmlWriter.WriteLine("<tr>");
                htmlWriter.Write($"<td>{entityLogicalNameHtml} ({entitySetName})</td><td>{descriptiontext} ({linkHtml})</td><td>{objectTypeCode}</td><td>{primaryIdAttribute} {primaryNameAttribute}</td><td>{displayNamesText}</td><td>{syncPlugin}</td><td>{asyncPlugin}</td><td>{syncWorkflow}</td><td>{asyncWorkflow}</td>");
                htmlWriter.WriteLine("</tr>");
            }
            return entityMetadata;
        }

        private void DumpEntityAndChildren(JObject entity)
        {
            string entityLogicalName = entity["LogicalName"].Value<string>();
            if (!entitiesWithChildren.Contains(entityLogicalName)) return;  // if no plugin/workflow no need to show the entity name again

            htmlWriter.WriteLine("<a name='entity_{1}' /><h3>{0} ({1})</h3>", entityLogicalName, entityLogicalName);

            DumpSDKSteps(entityLogicalName, 1);
            DumpSDKSteps(entityLogicalName, 0);

            DumpWorkflows(entityLogicalName, 1);
            DumpWorkflows(entityLogicalName, 0);
        }

        private void DumpSDKSteps(string entityLogicalName, int mode)
        {
            IEnumerable<JToken> targets = sdkSteps.Where(obj => ((string)obj["primaryobjecttype"] == entityLogicalName && (int)obj["mode"] == mode)).OrderBy(obj => (string)obj["MessageName"]).ThenBy(obj => (int)obj["stage"]).ThenBy(obj => (int)obj["rank"]);

            if (targets.Count<JToken>() == 0) return;

            //0 Synchronous
            //1 Asynchronous
            htmlWriter.WriteLine("<p>{0} plugins</p>", (mode == 0 ? "Synchronous" : "Asynchronous"));
            htmlWriter.WriteLine("<table>");

            htmlWriter.WriteLine("<tr>");
            htmlWriter.Write("<th>sdkmessageprocessingstepid</th><th>Message</th><th>Stage</th><th>Rank</th><th>Name</th><th>Hidden</th><th>State</th>");
            htmlWriter.WriteLine("<th>Plugin</th><th>IsolationMode</th><th>Filtering Attributes</th><th>Solution</th>");
            htmlWriter.WriteLine("</tr>");

            foreach (JToken row in targets)
            {
                DumpSDKStepsRow(entityLogicalName, row);
            }

            htmlWriter.WriteLine("</table>");
        }

        private void DumpSDKStepsRow(string entityLogicalName, JToken row)
        {
            htmlWriter.WriteLine("<tr>");

            string sdkmessageprocessingstepid = DumpCellWebApiLink("sdkmessageprocessingstepid", "sdkmessageprocessingsteps", row);

            DumpCell(row, "MessageName");
            DumpCellStage(row);
            DumpCell(row, "rank");
            DumpCell(row, "name");
            DumpCell(row, "ishidden");

            DumpCellSdkStepStateStatusCode(row, "statecode");

            // Plugin
            string pluginTypeId = row["_plugintypeid_value"].Value<string>();
            JToken pluginRow = DumpCellPluginTypeIsolationMode(entityLogicalName, pluginTypeId);
            if (pluginRow == null)
            {
                // Plugin is not found.. dump plugintype, isolation mode, filtering attribute cells empty then move next
                htmlWriter.Write("<td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr>");
                return;
            }

            // Filtering attributes
            string filteringattributes = (row["filteringattributes"] == null ? "(not set)" : row["filteringattributes"].Value<string>());
            htmlWriter.Write("<td class='smallfontcell'>{0}</td>", filteringattributes.Replace(",", ", "));

            // Find solutionid. First look for it from the assemblyid with the SolutionComponent, if not found then directly use the solutionid.
            string solutionId = FindSolutionIdFromSolutionComponent(pluginRow, pluginRow["_pluginassemblyid_value"].Value<string>());

            if ((solutionId == null) && (pluginRow != null)) solutionId = pluginRow["solutionid"].Value<string>();
            DumpCellSolutionName(solutionId);
            htmlWriter.WriteLine("</tr>");
        }

        private string FindSolutionIdFromSolutionComponent(JToken row, string objectId)
        {
            IEnumerable<JToken> component = solutionComponents.Where(obj => (string)obj["objectid"] == objectId);
            if (component.Count<JToken>() == 0)
            {
                // solution id is not found from the SolutionComponent. Use the value from the object row just as-is.
                return row["solutionid"].Value<string>();
            }
            string solutionIdFromSolutionComponent = component.FirstOrDefault<JToken>()["_solutionid_value"].Value<string>();
            // solution id is found from the SolutionComponent. Use that value instead of SolutionId in the row.
            row["solutionid"] = solutionIdFromSolutionComponent;
            return solutionIdFromSolutionComponent;
        }

        private void DumpWorkflows(string entityLogicalName, int mode)
        {
            IEnumerable<JToken> targets = workflows.Where(obj => (string)obj["primaryentity"] == entityLogicalName && (int)obj["mode"] == mode).OrderBy(obj => (string)obj["name"]);

            if (targets.Count<JToken>() == 0) return;

            string[] fields = new string[] { "name", "uniquename", "primaryentity", "description", "ondemand", "triggeroncreate", "triggerondelete", "triggeronupdateattributelist", "createstage", "deletestage", "updatestage" };
            string[] captions = new string[] { "Name", "UniqueName", "primaryentity", "description", "ondemand", "triggeroncreate", "triggerondelete", "triggeronupdateattributelist", "createstage", "deletestage", "updatestage" };

            //0 Background
            //1 Real-time
            htmlWriter.WriteLine("<p>{0} workflows</p>", (mode == 1 ? "Real-time" : "Background"));
            htmlWriter.WriteLine("<table>");

            htmlWriter.WriteLine("<tr>");
            htmlWriter.WriteLine("<th>WorkflowId</th>");
            foreach (string caption in captions) htmlWriter.Write("<th>{0}</th>", caption);
            htmlWriter.WriteLine("<th>Solution</th>");
            htmlWriter.WriteLine("</tr>");

            foreach (JToken row in targets)
            {
                htmlWriter.WriteLine("<tr>");
                string workflowId = DumpCellWebApiLink("workflowid", "workflows", row);
                foreach (string field in fields)
                {
                    DumpCell(row, field);
                }

                // Solution
                string solutionId = FindSolutionIdFromSolutionComponent(row, workflowId);
                DumpCellSolutionName(solutionId);

                htmlWriter.WriteLine("</tr>");
            }

            htmlWriter.WriteLine("</table>");
        }

        private void DumpCell(JToken row, string field)
        {
            string fieldValue = null;
            switch (field)
            {
                case "createstage":
                case "deletestage":
                case "updatestage":
                    fieldValue = row[field].Value<string>();
                    if ("20".Equals(fieldValue)) { fieldValue = "Pre"; } else if ("40".Equals(fieldValue)) { fieldValue = "Post"; };
                    break;
                case "ishidden":
                    fieldValue = row[field]["Value"].Value<string>();
                    break;
                case "triggeronupdateattributelist":
                    fieldValue = row[field].Value<string>();
                    fieldValue = (string.IsNullOrEmpty(fieldValue) ? "&nbsp;" : fieldValue.Replace(",", ", "));
                    break;
                default:
                    fieldValue = (row[field] == null ? "&nbsp;" : row[field].Value<string>());
                    break;
            }

            htmlWriter.WriteLine("<td>{0}</td>", fieldValue);
        }

        private void DumpCellSdkStepStateStatusCode(JToken row, string field)
        {
            int state = row[field].Value<int>();
            string stateText = null;
            switch (state)
            {
                case 0: stateText = "Enabled"; break;
                case 1: stateText = "Disabled"; break;
                default: stateText = "Undefined-" + state; break;
            }
            htmlWriter.WriteLine("<td>{0}</td>", stateText);
        }

        private void DumpCellStage(JToken row)
        {
            int stage = row["stage"].Value<int>();
            string stageName = null;
            switch (stage)
            {
                case 5: stageName = "Initial Pre-operation"; break;
                case 10: stageName = "Pre-validation"; break;
                case 15: stageName = "Internal Pre-operation Before External Plugins"; break;
                case 20: stageName = "Pre-operation"; break;
                case 25: stageName = "Internal Pre-operation After External Plugins"; break;
                case 30: stageName = "Main Operation"; break;
                case 35: stageName = "Internal Post-operation Before External Plugins"; break;
                case 40: stageName = "Post-operation"; break;
                case 45: stageName = "Internal Post-operation After External Plugins"; break;
                case 50: stageName = "Post-operation (Deprecated)"; break;
                case 55: stageName = "Final Post-operation"; break;
                default: stageName = "Undefined-" + stage; break;
            }
            htmlWriter.WriteLine("<td>{0}-{1}</td>", stage, stageName);
        }

        private void DumpCellAssemblyIsolationMode(JToken row)
        {
            string isolationModeText = null;
            int isolationMode = row["assemblyisolationmode"].Value<int>();
            switch (isolationMode)
            { 
                case 1:
                    isolationModeText = "None";
                    break;
                case 2:
                    isolationModeText = "Sandbox";
                    break;
                case 3:
                    isolationModeText = "External";
                    break;
                default:
                    isolationModeText = isolationMode.ToString();
                    break;
            }
            htmlWriter.WriteLine("<td>{0}</td>", isolationModeText);
        }

        private JToken DumpCellPluginTypeIsolationMode(string entityLogicalName, string plugintypeid)
        {
            IEnumerable<JToken> pluginsById = pluginTypes.Where(obj => (string)obj["plugintypeid"] == plugintypeid);
            if (pluginsById.Count<JToken>() == 0)
            {
                return null;
            }
            JToken plugin = pluginsById.FirstOrDefault<JToken>();
            string typeName = plugin["typename"].Value<string>();
            string criteria = $"<condition attribute = 'plugintypeid' operator='eq' value='{plugintypeid}' />";
            string webapiurl = string.Format(WEBAPIURL + "/plugintypes?fetchXml=" + System.Web.HttpUtility.UrlEncode(string.Format(URL_PLUGINTYPES_FETCHXML, criteria)));

            htmlWriter.WriteLine($"<td><a href='{OrganizationUrl}{webapiurl}' target='_blank'>{typeName}</a></td>");
            DumpCellAssemblyIsolationMode(plugin);
            // create a back reference what entites are using this plugin type
            // dictionary is used at the end of list of plugins to show entities using the plugin
            if (pluginTypeIdToEntityLogicalName.Keys.Contains<string>(plugintypeid))
            {
                if (!pluginTypeIdToEntityLogicalName[plugintypeid].Contains(entityLogicalName)) pluginTypeIdToEntityLogicalName[plugintypeid].Add(entityLogicalName);
            }
            else
            {
                List<string> entitiesPluginIsUsed = new List<string>();
                entitiesPluginIsUsed.Add(entityLogicalName);
                pluginTypeIdToEntityLogicalName.Add(plugintypeid, entitiesPluginIsUsed);
            }
            return plugin;
        }

        private void DumpCellSolutionName(string solutionId)
        {
            IEnumerable<JToken> solution = solutions.Where(obj => (string)obj["solutionid"] == solutionId);
            if (solution.Count<JToken>() == 0)
            {
                htmlWriter.Write("<td>&nbsp;</td>");
                return;
            }
            htmlWriter.Write("<td><a href='#sln_{1}'>{0}</a> ({2})</td>", solution.FirstOrDefault<JToken>()["friendlyname"].Value<string>(), solution.FirstOrDefault<JToken>()["solutionid"].Value<string>(), solution.FirstOrDefault<JToken>()["publishername"].Value<string>());
        }

        private void DumpSolutionAndChildren()
        {
            foreach (JToken solution in solutions.OrderBy(obj => (string)obj["uniquename"]))
            {
                string solutionid = solution["solutionid"].Value<string>();
                int childrenCount = pluginTypes.Where(obj => (string)obj["solutionid"] == solutionid).Count<JToken>() + workflows.Where(obj => (string)obj["solutionid"] == solutionid).Count<JToken>();

                htmlWriter.WriteLine("<h3><a name='sln_{2}' />{0} ({1}) InstalledOn:{3}</h3>", solution["friendlyname"].Value<string>(), solution["uniquename"].Value<string>(), solutionid, solution["installedon"].Value<DateTime>());

                if (childrenCount == 0)
                {
                    htmlWriter.WriteLine("<p>no child</p>");
                    continue;
                }

                IEnumerable<JToken> pluginsForSolution = pluginTypes.Where(obj => (string)obj["solutionid"] == solutionid).OrderBy(obj => (string)obj["assemblyname"] + (string)obj["typename"]);
                string[] fields = new string[] { "assemblyname", "assemblyisolationmode", "typename", "friendlyname", "description", "modifiedon" };
                DumpAsTableWithWebApiLink(pluginsForSolution, "Plugins", fields, fields, "plugintypeid", "plugintypes");

                IEnumerable <JToken> workflowsForThisSolution = workflows.Where(obj => (string)obj["solutionid"] == solutionid).OrderBy(obj => (string)obj["name"]);
                string[] wfFields = new string[] { "name", "uniquename", "primaryentity", "mode"};
                DumpAsTableWithWebApiLink(workflowsForThisSolution, "Workflows", wfFields, wfFields, "workflowid", "workflows");
            }
        }

        private void DumpAsTableWithWebApiLink(IEnumerable<JToken> targets, string tableCaption, string[] fields, string[] captions, string webApiLinkField, string webapiName)
        {
            if (targets.Count<JToken>() == 0) return;

            htmlWriter.WriteLine("<p>{0}</p>", tableCaption);
            htmlWriter.WriteLine("<table>");

            htmlWriter.WriteLine("<tr>");
            htmlWriter.WriteLine("<th>{0}</th>", webApiLinkField);
            foreach (string caption in captions) htmlWriter.Write("<th>{0}</th>", caption);
            if ("plugintypes".Equals(webapiName)) htmlWriter.Write("<th>Registered Entities</th>");
            htmlWriter.WriteLine("</tr>");

            foreach (JToken row in targets)
            {
                htmlWriter.WriteLine("<tr>");
                DumpCellWebApiLink(webApiLinkField, webapiName, row);
                foreach (string field in fields)
                {
                    string fieldValue = (row[field] == null ? "&nbsp;" : row[field].Value<string>());
                    if ("mode".Equals(field)) fieldValue = (row[field].Value<int>() == 1 ? "Realtime" : "Background");  // workflow.mode display value
                    else if ("primaryentity".Equals(field) && !"none".Equals(fieldValue)) fieldValue = string.Format("<a href='#entity_{0}'>{0}</a>", fieldValue);   // workflow.primaryentity -> entity link
                    else if ("assemblyisolationmode".Equals(field)) { DumpCellAssemblyIsolationMode(row); continue; };
                    htmlWriter.WriteLine("<td>{0}</td>", fieldValue);
                }
                if ("plugintypes".Equals(webapiName)) DumpPluginRegisteredEntities(row["plugintypeid"].Value<string>());
                htmlWriter.WriteLine("</tr>");
            }

            htmlWriter.WriteLine("</table>");
        }

        private string DumpCellWebApiLink(string webApiLinkField, string webapiName, JToken row)
        {
            string linkValue = row[webApiLinkField].Value<string>();
            string webapiurl = string.Format(WEBAPIURL + "/{0}({1})", webapiName, linkValue);
            htmlWriter.WriteLine("<td class='smallfontcell'><a href='{1}{2}' target='_blank'>{0}</a></td>", linkValue, OrganizationUrl, webapiurl);
            return linkValue;
        }

        private void DumpPluginRegisteredEntities(string plugintypeid)
        {
            if (! pluginTypeIdToEntityLogicalName.Keys.Contains<string>(plugintypeid))
            {
                htmlWriter.WriteLine("<td>none</td>");
                return;
            }
            htmlWriter.Write("<td>");
            foreach (string entityname in pluginTypeIdToEntityLogicalName[plugintypeid])
            {
                htmlWriter.Write("<a href='#entity_{0}'>{0}</a> ", entityname);
            }
            htmlWriter.Write("</td>");
        }
    }
   
    internal static class FindLabelsUtil
    {
        internal static string FindLabels(JToken entity, string fieldName)
        {
            List<string> displayNames = new List<string>();
            List<JToken> labelMatches = new List<JToken>();
            JToken containerToken = entity[fieldName];
            if (containerToken == null) return string.Empty;
            FindTokens(containerToken, "Label", labelMatches);
            foreach (JToken localizedLabel in labelMatches)
            {
                string displayName = localizedLabel.Value<string>();
                if (!displayNames.Contains<string>(displayName)) displayNames.Add(string.Format("{0}", @displayName));
            }
            string displayNamesText = string.Join(", ", displayNames.ToArray());
            return displayNamesText;
        }

        private static void FindTokens(JToken containerToken, string name, List<JToken> matches)
        {
            if (containerToken.Type == JTokenType.Object)
            {
                foreach (JProperty child in containerToken.Children<JProperty>())
                {
                    if (child.Name == name)
                    {
                        matches.Add(child.Value);
                    }
                    FindTokens(child.Value, name, matches);
                }
            }
            else if (containerToken.Type == JTokenType.Array)
            {
                foreach (JToken child in containerToken.Children())
                {
                    FindTokens(child, name, matches);
                }
            }
        }
    }

    internal class WebApiResponseException : Exception
    {
        internal WebApiResponseException(string errorMessage) : base(errorMessage)
        {
            // do nothing
        }
    }
}
