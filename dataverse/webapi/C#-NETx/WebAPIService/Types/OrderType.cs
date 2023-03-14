using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples.Types
{
    /// <summary>
    /// The possible values for the order type in an OrderExpression.
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// The values of the specified attribute should be sorted in ascending order, from lowest to highest.
        /// </summary>
        Ascending,
        /// <summary>
        /// The values of the specified attribute should be sorted in descending order, from highest to lowest.
        /// </summary>
        Descending
    }
}
