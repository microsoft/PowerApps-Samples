using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;


namespace MetaViz
{
    //
    // EntityMetadata class represents one entity's metadata information. Class is mainly used for the trigger html reporting.
    // Its member variable ERInformationForThisEntity keeps ERInformation specific to this entity. 
    // ERInformation for all entities will be aggregated at the end of process and final ERInformation will be populated.
    //

    internal class EntityMetadata
    {
        internal string MetadataUrl
        {
            get
            {
                // If intersect entity ManyToManyRelationships has more information
                // otherwise use attributes metadata Url
                return (isIntersect ? manyToManyRelationshipsPath : $"{entityDefinitionsPath}/Attributes");
            }
        }

        private ERInformation erInformationForThisEntity = new ERInformation();
        internal ERInformation ERInformationForThisEntity { get => erInformationForThisEntity; }

        internal const int SEPARATOR_LENGTH = 150;

        readonly string organizationUrl;
        readonly string entityDefinitionsPath;
        readonly string manyToManyRelationshipsPath;
        internal readonly string AttributesUrl;
        internal readonly string EntityListUrl;

        readonly string metadataId;
        readonly string entityLogicalName;
        readonly string entitySetName;
        readonly string descriptiontext;
        readonly int objectTypeCode;
        readonly string primaryIdAttribute;
        readonly string primaryNameAttribute;
        readonly string displayNamesText;
        readonly bool isIntersect;

        internal EntityMetadata(string organizationUrl, string APIURL, string metadataId, string entityLogicalName, string entitySetName, string descriptiontext, int objectTypeCode, string primaryIdAttribute, string primaryNameAttribute, string displayNamesText, bool isIntersect)
        {
            this.organizationUrl = organizationUrl;
            this.entityDefinitionsPath = $"{APIURL}/EntityDefinitions({metadataId})";
            this.manyToManyRelationshipsPath = $"{entityDefinitionsPath}/ManyToManyRelationships?$select=Entity1LogicalName,Entity2LogicalName,Entity1IntersectAttribute,Entity2IntersectAttribute,IntersectEntityName,SchemaName";
            this.AttributesUrl = $"{this.organizationUrl}{entityDefinitionsPath}/Attributes?$select=MetadataId,SchemaName,AttributeTypeName,DisplayName";
            this.EntityListUrl = $"{this.organizationUrl}/main.aspx?etn={entityLogicalName}&pagetype=entitylist";
            this.metadataId = metadataId;
            this.entityLogicalName = entityLogicalName;
            this.entitySetName = entitySetName;
            this.descriptiontext = descriptiontext;
            this.objectTypeCode = objectTypeCode;
            this.primaryIdAttribute = primaryIdAttribute;
            this.primaryNameAttribute = primaryNameAttribute;
            this.displayNamesText = displayNamesText;
            this.isIntersect = isIntersect;
        }

        internal void DumpReport(string responseJson, StreamWriter specWriter)
        {
            string entityName = $"[{entityLogicalName}] ({entitySetName}) Primary:{primaryIdAttribute} {primaryNameAttribute}";
            string hashCodeHeader = Hashcord(entityLogicalName) + "|";
            specWriter.WriteLine(hashCodeHeader + new string('=', SEPARATOR_LENGTH));
            specWriter.WriteLine($"{hashCodeHeader}{entityName}");

            JToken root = null;
            try
            {
                root = JsonConvert.DeserializeObject<JObject>(responseJson);
            }
            catch (JsonReaderException ex)
            {
                specWriter.WriteLine(hashCodeHeader + $"Fetch failed: {ex.ToString()}");
                return;
            }

            if (root["error"] != null)
            {
                specWriter.WriteLine(hashCodeHeader + $"Fetch failed: {root["error"].ToString()}");
                return;
            }

            JToken rootAttributes = root["value"];
            if (isIntersect)
            {
                // intersect entity, capture the relationship information and then dump
                DumpIntersectInformation(entityLogicalName, hashCodeHeader, rootAttributes, specWriter);
            }
            else
            {
                // normal entity, dump the entity information
                DumpEntity(hashCodeHeader, rootAttributes, specWriter);
            }
        }

        private void DumpIntersectInformation(string entityLogicalName, string hashCodeHeader, JToken rootAttributes, StreamWriter specWriter)
        {
            // some reason it comes as a first element of the array
            JToken[] targets = rootAttributes.ToArray<JToken>();
            if (targets.Length == 0)
            {
                specWriter.WriteLine(hashCodeHeader + $"IntersectEntity (no details available)");
                return;
            }
            rootAttributes = targets[0];
            string Entity1LogicalName = rootAttributes["Entity1LogicalName"].Value<string>();
            string Entity2LogicalName = rootAttributes["Entity2LogicalName"].Value<string>();
            string Entity1IntersectAttribute = rootAttributes["Entity1IntersectAttribute"].Value<string>();
            string Entity2IntersectAttribute = rootAttributes["Entity2IntersectAttribute"].Value<string>();
            specWriter.WriteLine(hashCodeHeader + $"IntersectEntity {Entity1LogicalName}.{Entity1IntersectAttribute} <=> {Entity2LogicalName}.{Entity2IntersectAttribute}");
            //Many to Many
            //Instead of creating 2 lookup 1-n among entity 1, 2 and intersect entities create an n-n relations
            erInformationForThisEntity.ERRelations.Add(new ERRelation(Entity1LogicalName, Entity2LogicalName, entityLogicalName, true));
        }

        private void DumpEntity(string hashCodeHeader, JToken rootAttributes, StreamWriter specWriter)
        {
            // normal entity, dump the list of attributes
            string completeDesc = string.Empty;
            if (!string.IsNullOrWhiteSpace(displayNamesText)) completeDesc = displayNamesText + ". ";
            if (!string.IsNullOrWhiteSpace(descriptiontext)) completeDesc += descriptiontext;
            specWriter.WriteLine(hashCodeHeader + completeDesc);
            specWriter.WriteLine(hashCodeHeader + new string('-', SEPARATOR_LENGTH));
            erInformationForThisEntity.EREntitieAttributes.Add(new EREntityAttribute(entityLogicalName, string.Empty, "Entity", $"({entitySetName}) " + completeDesc));
            DumpEntityAttributes(entityLogicalName, hashCodeHeader, rootAttributes, specWriter);
        }

        private void DumpEntityAttributes(string entityLogicalName, string hashCodeHeader, JToken rootAttributes, StreamWriter specWriter)
        {
            foreach (JObject attribute in rootAttributes.OrderBy(obj => (string)obj["SchemaName"]))
            {
                string schemaName = attribute["SchemaName"].Value<string>();
                string dataType = GetDataType(schemaName, attribute, out string lookupTarget);
                string description = DisplayNameAndDescription(attribute, lookupTarget);
                specWriter.WriteLine("{3}{0, -50}|{1, -20}|{2}", schemaName, dataType, description, hashCodeHeader);
                erInformationForThisEntity.EREntitieAttributes.Add(new EREntityAttribute(entityLogicalName, schemaName, dataType, description));
            }
        }

        // add the hashcode from the entity name so that comparison between two output files can work better
        private string Hashcord(string entityName)
        {
            string hash = string.Format("{0:X8}", entityName.ToLower().GetHashCode());
            return hash.Substring(hash.Length - 3, 3);
        }

        private string DisplayNameAndDescription(JToken attribute, string lookupTarget)
        {
            string completeDesc = string.Empty;
            string displayName = null, description = null;
            if (attribute["DisplayName"] != null) displayName += FindLabelsUtil.FindLabels(attribute["DisplayName"], "UserLocalizedLabel");
            if (attribute["Description"] != null) description += FindLabelsUtil.FindLabels(attribute["Description"], "UserLocalizedLabel");
            if (!string.IsNullOrWhiteSpace(lookupTarget)) completeDesc = $"=>{lookupTarget}. ";
            if (!string.IsNullOrWhiteSpace(displayName)) completeDesc += $"{displayName}. ";
            if (!string.IsNullOrWhiteSpace(displayName)) completeDesc += description;
            completeDesc = completeDesc.Replace('\r', ' ').Replace('\n', ' ');
            if (completeDesc.Length > 250) completeDesc = completeDesc.Substring(0, 250) + "...";
            return completeDesc.TrimEnd();
        }

        private string GetDataType(string schemaName, JToken attribute, out string lookupTarget)
        {
            lookupTarget = string.Empty;
            string attType = null;
            if (attribute["AttributeType"] != null) attType = attribute["AttributeType"].Value<string>();
            else
            {
                string[] dataTypeTokens = attribute["@odata.type"].Value<string>().Split(new char[] { '.' });
                attType = dataTypeTokens[dataTypeTokens.Length - 1];
            }
            switch (attType)
            {
                case "Lookup":
                    // e.g. [invoicedetail] (invoicedetails) InvoiceId lookup [invoice]
                    //      [invoice] (invoices) AccountId lookup [account]
                    lookupTarget = GetLookupTarget(attribute);
                    erInformationForThisEntity.ERRelations.Add(new ERRelation(lookupTarget, entityLogicalName, schemaName));
                    break;
                case "String":
                    attType += "(" + attribute["MaxLength"].Value<string>() + ")";
                    break;
                default:
                    break;
            }
            return attType;
        }

        private string GetLookupTarget(JToken attribute)
        {
            if (attribute["Targets"] == null) return "Null";
            JToken[] targets = attribute["Targets"].ToArray<JToken>();
            if (targets.Length == 0) return "Empty";
            return targets.First<JToken>().Value<string>();
        }
    }
}
