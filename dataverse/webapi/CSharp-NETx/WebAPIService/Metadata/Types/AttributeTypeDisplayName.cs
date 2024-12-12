using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata.Types
{
    public class AttributeTypeDisplayName
    {
        public AttributeTypeDisplayName(AttributeTypeDisplayNameValues value)
        {
            Value = value;
        }

        public AttributeTypeDisplayNameValues Value { get; set; }
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AttributeTypeDisplayNameValues
    {
        BigIntType,
        BooleanType,
        CalendarRulesType,
        CustomerType,
        DateTimeType,
        DecimalType,
        DoubleType,
        EntityNameType,
        FileType,
        ImageType,
        IntegerType,
        LookupType,
        ManagedPropertyType,
        MemoType,
        MoneyType,
        MultiSelectPicklistType,
        OwnerType,
        PartyListType,
        PicklistType,
        StateType,
        StatusType,
        StringType,
        UniqueidentifierType,
        VirtualType
    }
}