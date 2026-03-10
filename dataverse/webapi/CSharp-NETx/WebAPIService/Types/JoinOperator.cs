using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Types
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum JoinOperator
    {
        Inner,
        Outer,
        Natural,
        MatchFirstRowUsingCrossApply,
        In,
        Exists,
        Any,
        NotAny,
        All,
        NotAll
    }
}
