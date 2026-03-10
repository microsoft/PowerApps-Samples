using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.Msagl.Drawing;

namespace MetaViz
{
    //
    // ERInformation captures Entity information (EREntityAttribute) and Relationship information (ERRelation) for rendering and serialization purposes.
    // ERInformation.ConvertToGraph() creates a Graph object for MSAGL for selected entities for visualization.
    //

    [JsonObject]
    internal class EREntityAttribute
    {
        [JsonProperty]
        internal string EntityName;

        [JsonProperty]
        internal string AttributeName;

        [JsonProperty]
        internal string DataType;

        [JsonProperty]
        internal string Description;

        internal EREntityAttribute()
        {
        }

        internal EREntityAttribute(string entityName, string attributeName, string dataType, string description)
        {
            EntityName = entityName;
            AttributeName = attributeName;
            DataType = dataType;
            Description = description;
        }
    }

    [JsonObject]
    internal class ERRelation
    {
        [JsonProperty]
        internal string EntityOne;

        [JsonProperty]
        internal string EntityMany;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        internal string LookupField;        // Lookup field name from the many entity side.

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        internal string IntersectEntity;    // Intersect entity. If name is set, EntityOne and EntityMany are "Many-To-Many".


        internal ERRelation()
        {
        }

        internal ERRelation(string entityOne, string entityMany, string lookupField)
        {
            EntityOne = entityOne;
            EntityMany = entityMany;
            LookupField = lookupField;
        }

        internal ERRelation(string entityMany1, string entityMany2, string intersectEntity, bool isIntersect)
        {
            if (!isIntersect) throw new InvalidOperationException("This constructor is only for intersect relationship/many-to-many.");
            EntityOne = entityMany1;
            EntityMany = entityMany2;
            IntersectEntity = intersectEntity;
        }

        internal bool IsManyToMany()
        {
            return (IntersectEntity != null);
        }
    }

    [JsonObject]
    internal class ERInformation
    {
        // list of common entities that are usually 'too much' to render in the graph
        private const string SKIP_ENTITIES_FOR_GRAPH = @",businessunit,organization,systemuser,team,transactioncurrency,workflow,webresource,webwizard,uom,transformationparametermapping,transformationmapping,traceregarding,tracelog,topicmodelconfiguration,topicmodel,timezonedefinition,template,teamtemplate,syncattributemappingprofile,syncattributemapping,subscription,solution,dependencynode,solution,solutioncomponent,sdkmessage,sdkmessagefilter,sdkmessagepair,sdkmessageprocessingstep,sdkmessageprocessingstepsecureconfig,sdkmessagerequest,sdkmessageresponse,role,roletemplate,rollupproperties,routingrule,ribboncustomization,ribbondiff,report,relationshiprole,recurringappointmentmaster,recurrencerule,recommendationmodel,recommendationmodelmapping,recommendationmodelversion,ratingmodel,ratingvalue,principalsyncattributemap,privilege,pluginassembly,plugintype,Empty,asyncoperation,processtrigger,systemform,attributemap,entitymap,knowledgesearchmodel,azureserviceconnection,advancedsimilarityrule,teamprofiles,fieldsecurityprofile,systemuserprofiles,fieldpermission,mobileofflineprofileitemassociation,mobileofflineprofileitem,mobileofflineprofile,savedquery,publisher,publisheraddress,appmodule,appmoduleroles,appmodulecomponent,importlog,importdata,importfile,import,ownermapping,importentitymapping,importmap,columnmapping,lookupmapping,picklistmapping,";

        [JsonProperty]
        internal string OrganizationUrl;

        [JsonProperty]
        internal DateTime CreatedOn;

        [JsonProperty]
        internal List<ERRelation> ERRelations;

        [JsonProperty]
        internal List<EREntityAttribute> EREntitieAttributes;

        internal ERInformation()
        {
            EREntitieAttributes = new List<EREntityAttribute>();
            ERRelations = new List<ERRelation>();
        }

        internal void AddMetadata(ERInformation erInformationToBeAdded)
        {
            EREntitieAttributes.AddRange(erInformationToBeAdded.EREntitieAttributes);
            ERRelations.AddRange(erInformationToBeAdded.ERRelations);
        }

        internal IEnumerable<string> ListEntities()
        {
            return ERRelations.Select(rel => rel.EntityOne).Union(ERRelations.Select(rel => rel.EntityMany)).Distinct().OrderBy(val => val);
        }

        internal Graph ConvertToGraph(List<string> targetEntities, int depth)
        {
            // Populate MSAGL Graph object for the selected entities.
            // Find related entities as well if depth is set larger than 0 (i.e. 1 means related entities. 2+ depth is too large to use practically)
            ExpandTargetEntities(targetEntities, depth);

            Graph graph = new Graph("graph");
            foreach (ERRelation relation in ERRelations.Distinct().OrderBy(obj => obj.EntityOne))
            {
                if (!targetEntities.Contains(relation.EntityMany) || !targetEntities.Contains(relation.EntityOne)) continue;
                AddNewRelation(graph, relation);
            }
            return graph;
        }

        private static void AddNewRelation(Graph graph, ERRelation relation)
        {
            string attrId = (relation.IsManyToMany() ? $"{relation.EntityMany} n-{relation.IntersectEntity}-n {relation.EntityOne}" : $"{relation.EntityMany}.{relation.LookupField} n-1 {relation.EntityOne}");
            System.Diagnostics.Debug.WriteLine(attrId);
            if (graph.EdgeById(attrId) != null) return;
            Edge edge = graph.AddEdge(relation.EntityMany, relation.EntityOne);
            edge.Attr.ArrowheadAtSource = ArrowStyle.Diamond;
            edge.Attr.ArrowheadAtTarget = (relation.IsManyToMany() ? ArrowStyle.Diamond : ArrowStyle.None);
            edge.Attr.Id = attrId;
        }

        internal List<string> FindRelatedEntities(string targetEntity)
        {
            List<string> relatedEntities = new List<string>();
            relatedEntities.Add(targetEntity);
            foreach (ERRelation relation in ERRelations.Distinct())
            {
                if (relation.EntityOne.Equals(targetEntity) && !IsEntityInSkipList(relation.EntityMany) && !relatedEntities.Contains(relation.EntityMany)) relatedEntities.Add(relation.EntityMany);
                if (relation.EntityMany.Equals(targetEntity) && !IsEntityInSkipList(relation.EntityOne) && !relatedEntities.Contains(relation.EntityOne)) relatedEntities.Add(relation.EntityOne);
            }
            return relatedEntities;
        }

        // Keep adding entity names if entities are used for relationship with the given entities
        private void ExpandTargetEntities(List<string> targetEntities, int depth)
        {
            if (depth == 0) return;
            List<string> addedEntities = new List<string>();
            foreach (string entity in targetEntities)
            {
                foreach (ERRelation relation in ERRelations.Where(rel => entity.Equals(rel.EntityOne) || entity.Equals(rel.EntityMany)))
                {
                    if (!targetEntities.Contains(relation.EntityOne) && !addedEntities.Contains(relation.EntityOne) && !IsEntityInSkipList(relation.EntityOne)) addedEntities.Add(relation.EntityOne);
                    if (!targetEntities.Contains(relation.EntityMany) && !addedEntities.Contains(relation.EntityMany) && !IsEntityInSkipList(relation.EntityMany)) addedEntities.Add(relation.EntityMany);
                }
            }
            targetEntities.AddRange(addedEntities);
            ExpandTargetEntities(targetEntities, depth - 1);
        }

        internal IEnumerable<ERRelation> GetERRelationsForSpecificEntity(string targetEntity)
        {
            return ERRelations.Where(rel => rel.EntityOne.Equals(targetEntity) || rel.EntityMany.Equals(targetEntity));
        }

        internal IEnumerable<EREntityAttribute> GetEREntitieAttributesByEntityName(string targetEntity)
        {
            return EREntitieAttributes.Where(def => def.EntityName.Equals(targetEntity));
        }

        internal static bool IsEntityInSkipList(string entityName)
        {
            // Some entities (such as SystemUser entity) are not useful for ER diagram. Skip those noisy not useful entities for visualization.
            return (SKIP_ENTITIES_FOR_GRAPH.Contains($",{entityName},"));
        }
    }

    internal static class ERInformationUtil
    {
        internal static void AggregateERInformationForAllEntities(string organizationUrl, List<EntityMetadata> allEntitiesMetadata, StreamWriter jsonWriter, StreamWriter specWriter)
        {
            // allEntitiesMetadata has all metadata now. 
            // Aggregate all entities' ERInformation and create a final ERInformation object, then persist the data in json.
            ERInformation erInformationForAllEntities = new ERInformation();
            erInformationForAllEntities.OrganizationUrl = organizationUrl;
            erInformationForAllEntities.CreatedOn = DateTime.UtcNow;
            foreach (EntityMetadata entityMetadata in allEntitiesMetadata)
            {
                erInformationForAllEntities.AddMetadata(entityMetadata.ERInformationForThisEntity);
            }
            string relationsJson = JsonConvert.SerializeObject(erInformationForAllEntities, Formatting.Indented);
            jsonWriter.WriteLine(relationsJson);
            DumpERRelationsInSpecFile(specWriter, erInformationForAllEntities);
        }

        private static void DumpERRelationsInSpecFile(StreamWriter specWriter, ERInformation entityRelations)
        {
            // Add all relationship information in the scheme (spec file) at the end
            foreach (ERRelation relation in entityRelations.ERRelations.Distinct().OrderBy(obj => obj.EntityOne))
            {
                specWriter.WriteLine("{0,50} 1-n {1}.{2}", relation.EntityOne, relation.EntityMany, relation.LookupField);
            }
        }

        internal static ERInformation LoadRelationsFromDataFile(string fileNamePath)
        {
            string relationJsonText;
            using (StreamReader jsonTextReader = new StreamReader(fileNamePath))
            {
                relationJsonText = jsonTextReader.ReadToEnd();
            }
            ERInformation entityRelations = JsonConvert.DeserializeObject<ERInformation>(relationJsonText);
            return entityRelations;
        }
    }

}
