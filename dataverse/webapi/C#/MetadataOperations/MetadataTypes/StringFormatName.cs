using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata
{
    public class StringFormatName
    {
        public StringFormatName(StringFormatNameValues value)
        {
            Value = value;
        }

        public StringFormatNameValues Value { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum StringFormatNameValues
    {
        Email,
        Phone,
        PhoneticGuide,
        Text,
        TextArea,
        TickerSymbol,
        Url,
        VersionNumber
    }
}