using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace PowerApps.Samples.Types
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum JoinOperator
    {
        /// <summary>
        /// The values in the attributes being joined are compared using a comparison operator.
        /// </summary>
        Inner,
        /// <summary>
        /// All instances of the entity in the FROM clause are returned if they meet WHERE or HAVING search conditions.
        /// </summary>
        LeftOuter,
        /// <summary>
        /// Only one value of the two joined attributes is returned if an equal-join operation is performed and the two values are identical.
        /// </summary>
        Natural,
        /// <summary>
        /// Link-entity is generated as Correlated Subquery. The outer entity uses the 'cross apply' operator on the Correlated Subquery. Pick the top 1 row.
        /// </summary>
        MatchFirstRowUsingCrossApply,
        In,
        /// <summary>
        /// Link-entity is generated as a Correlated Subquery. The outer entity uses 'exists' operator on the Correlated Subquery.
        /// </summary>
        Exists,
        /// <summary>
        /// Link-entity is generated as Subquery. The outer entity uses the 'in' operator on the Subquery.
        /// </summary>
        Any,
        NotAny,
        All,
        NotAll

    }
}
