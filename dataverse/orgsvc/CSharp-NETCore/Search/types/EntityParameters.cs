using Newtonsoft.Json;
using System.Text;

namespace PowerApps.Samples.Search.Types;

/// <summary>
/// This defined common entity parameters required for search apis.
/// </summary>
[Serializable]
public class EntityParameters
{
    /// <summary>
    /// Gets or sets name of the entity.
    /// </summary>
    [JsonRequired]
    [JsonProperty(PropertyName = "entityname")]
    public string EntityName { get; set; }

    /// <summary>
    /// Gets or sets filter criteria.
    /// </summary>
    [JsonProperty(PropertyName = "filtercriteria", NullValueHandling = NullValueHandling.Ignore)]
    public string FilterCriteria { get; set; }

    /// <summary>
    /// Gets or sets Forbidden Secured Fields.
    /// </summary>
    [JsonProperty(PropertyName = "forbiddensecuredfields", NullValueHandling = NullValueHandling.Ignore)]
    public IList<string> ForbiddenSecuredFields { get; set; }

    /// <summary>
    /// Gets or sets owner Ids.
    /// </summary>
    [JsonProperty(PropertyName = "ownerids", NullValueHandling = NullValueHandling.Ignore)]
    public IList<string> OwnerIds { get; set; }

    /// <summary>
    /// Gets or sets owning business units.
    /// </summary>
    [JsonProperty(PropertyName = "owningbusinessunits", NullValueHandling = NullValueHandling.Ignore)]
    public IList<string> OwningBusinessUnits { get; set; }

    /// <summary>
    /// Gets or sets shared to principal Ids.
    /// </summary>
    [JsonProperty(PropertyName = "sharedtoprincipalids", NullValueHandling = NullValueHandling.Ignore)]
    public IList<string> SharedToPrincipalIds { get; set; }

    /// <summary>
    /// Gets or sets primary id attribute.
    /// </summary>
    [JsonProperty(PropertyName = "primaryidattribute", NullValueHandling = NullValueHandling.Ignore)]
    public string PrimaryIdAttribute { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether entity is IsFnO entity.
    /// </summary>
    [JsonProperty(PropertyName = "isfnoentity")]
    public bool IsFnOEntity { get; set; }

    /// <summary>
    /// Gets or sets shared to data area Ids.
    /// </summary>
    [JsonProperty(PropertyName = "dataareaids", NullValueHandling = NullValueHandling.Ignore)]
    public IList<string> DataAreaIds { get; set; }

    /// <summary>
    /// Override method for ToString.
    /// </summary>
    /// <returns>String representation of <see cref="EntityParameters"/>.</returns>
    public override string ToString()
    {
        StringBuilder str = new();
        str.AppendLine($"EntityName: {EntityName}");
        str.AppendLine($"FilterCriteria: [{FilterCriteria}]");
        str.AppendLine($"ForbiddenSecuredFields: [{ForbiddenSecuredFields.ToString()}]");
        str.AppendLine($"OwnerIds: [{OwnerIds.ToString()}]");
        str.AppendLine($"OwningBusinessUnits: [{OwningBusinessUnits.ToString()}]");
        str.AppendLine($"SharedToPrincipalIds: [{SharedToPrincipalIds.ToString()}]");
        str.AppendLine($"PrimaryIdAttribute: [{PrimaryIdAttribute}]");
        return str.ToString();
    }
}