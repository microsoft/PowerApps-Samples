using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

//This type created to support the RetrieveAttributeRequest class.
//These are the names used when casting specific types of attributes

namespace PowerApps.Samples.Metadata.Types
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AttributeType
    {
        AttributeMetadata, //When you don't need properties of a specific type
        BigIntAttributeMetadata,
        BooleanAttributeMetadata,
        DateTimeAttributeMetadata,
        DecimalAttributeMetadata,
        DoubleAttributeMetadata,
        EntityNameAttributeMetadata,
        FileAttributeMetadata,
        ImageAttributeMetadata,
        IntegerAttributeMetadata,
        LookupAttributeMetadata,
        ManagedPropertyAttributeMetadata,
        MemoAttributeMetadata,
        MoneyAttributeMetadata,
        MultiSelectPicklistAttributeMetadata,
        PicklistAttributeMetadata,
        StateAttributeMetadata,
        StatusAttributeMetadata,
        StringAttributeMetadata,
        UniqueIdentifierAttributeMetadata
    }
}
