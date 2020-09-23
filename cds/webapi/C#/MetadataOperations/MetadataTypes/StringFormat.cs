using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StringFormat
    {
        Email, Text, TextArea, Url, TickerSymbol, PhoneticGuide, VersionNumber, Phone
    }
}