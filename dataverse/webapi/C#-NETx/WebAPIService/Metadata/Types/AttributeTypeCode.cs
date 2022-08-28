using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AttributeTypeCode
    {
        Boolean,
        Customer,
        DateTime,
        Decimal,
        Double,
        Integer,
        Lookup,
        Memo,
        Money,
        Owner,
        PartyList,
        Picklist,
        State,
        Status,
        String,
        Uniqueidentifier,
        CalendarRules,
        Virtual,
        BigInt,
        ManagedProperty,
        EntityName
    }
}