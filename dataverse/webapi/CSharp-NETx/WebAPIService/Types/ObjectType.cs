using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace PowerApps.Samples.Types
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ObjectType
    {

        [EnumMember(Value = "System.String")]
        String,
        [EnumMember(Value = "System.String[]")]
        StringArray,
        [EnumMember(Value = "Microsoft.Xrm.Sdk.Metadata.AttributeTypeDisplayName")]
        AttributeTypeDisplayName,
        [EnumMember(Value = "System.Boolean")]
        // Bool also works for BooleanManagedProperties
        Bool,
        [EnumMember(Value = "System.Int32")]
        Int,
        [EnumMember(Value = "System.DateTime")]
        DateTime,
        [EnumMember(Value = "System.Guid")]
        Guid,
        [EnumMember(Value = "Microsoft.Xrm.Sdk.Metadata.OwnershipTypes")]
        EntityOwnerShipType,
        [EnumMember(Value = "Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode")]
        AttributeTypeCode,
        [EnumMember(Value = "Microsoft.Xrm.Sdk.Metadata.AttributeRequiredLevel")]
        AttributeRequiredLevel,
        [EnumMember(Value = "Microsoft.Xrm.Sdk.Metadata.RelationshipType")]
        RelationshipType



        //TODO Add more types
    }
}
