using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace PowerApps.Samples.Types
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum XrmDateTimeGrouping
    {
        None,
        Day,
        Week,
        Month,
        Quarter,
        Year,
        FiscalPeriod,
        FiscalYear
    }
}
