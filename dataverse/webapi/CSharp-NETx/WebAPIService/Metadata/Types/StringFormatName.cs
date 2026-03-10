using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata.Types
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
        VersionNumber,
        Json
    }
}