using Newtonsoft.Json;

namespace PowerApps.Samples.Search.Types;

/// <summary>
/// Data contract for response parameters for many to many relationship sync job status.
/// </summary>
public sealed class ManyToManyRelationshipSyncStatus
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ManyToManyRelationshipSyncStatus"/> class.
    /// </summary>
    /// <param name="relationshipName">the relationship name.</param>
    /// <param name="relationshipMetadataId">the relationship metadata id.</param>
    /// <param name="searchEntity">the search entity name.</param>
    /// <param name="relatedEntity">the second entity name.</param>
    /// <param name="searchEntityIdAttribute">the search entity Id attribute.</param>
    /// <param name="relatedEntityIdAttribute">the related entity Id attribute.</param>
    /// <param name="intersectEntity">the intersect entity name.</param>
    /// <param name="searchEntityObjectTypeCode">the search entity object type code.</param>
    /// <param name="lastSyncedDataVersion">the synced version.</param>
    public ManyToManyRelationshipSyncStatus(
        string relationshipName,
        Guid relationshipMetadataId,
        string searchEntity,
        string relatedEntity,
        string searchEntityIdAttribute,
        string relatedEntityIdAttribute,
        string intersectEntity,
        int searchEntityObjectTypeCode,
        string lastSyncedDataVersion)
    {
        this.RelationshipName = relationshipName;
        this.RelationshipMetadataId = relationshipMetadataId;
        this.SearchEntity = searchEntity;
        this.RelatedEntity = relatedEntity;
        this.SearchEntityIdAttribute = searchEntityIdAttribute;
        this.RelatedEntityIdAttribute = relatedEntityIdAttribute;
        this.IntersectEntity = intersectEntity;
        this.SearchEntityObjectTypeCode = searchEntityObjectTypeCode;
        this.LastSyncedDataVersion = lastSyncedDataVersion;
    }

    /// <summary>
    /// Gets the relationship name.
    /// </summary>
    [JsonProperty("relationshipName")]
    public string RelationshipName { get; }

    /// <summary>
    /// Gets the relationship metadata id.
    /// </summary>
    [JsonProperty("relationshipMetadataId")]
    public Guid RelationshipMetadataId { get; }

    /// <summary>
    /// Gets the search entity name.
    /// </summary>
    [JsonProperty("searchEntity")]
    public string SearchEntity { get; }

    /// <summary>
    /// Gets the second entity name.
    /// </summary>
    [JsonProperty("relatedEntity")]
    public string RelatedEntity { get; }

    /// <summary>
    /// Gets the search entity Id attribute.
    /// </summary>
    [JsonProperty("searchEntityIdAttribute")]
    public string SearchEntityIdAttribute { get; }

    /// <summary>
    /// Gets the related entity Id attribute.
    /// </summary>
    [JsonProperty("relatedEntityIdAttribute")]
    public string RelatedEntityIdAttribute { get; }

    /// <summary>
    /// Gets the intersect entity name.
    /// </summary>
    [JsonProperty("intersectEntity")]
    public string IntersectEntity { get; }

    /// <summary>
    /// Gets the search entity object type code.
    /// </summary>
    [JsonProperty("searchEntityObjectTypeCode")]
    public int SearchEntityObjectTypeCode { get; }

    /// <summary>
    /// Gets the synced version.
    /// </summary>
    [JsonProperty("lastSyncedVersion")]
    public string LastSyncedDataVersion { get; }
}