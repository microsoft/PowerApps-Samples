using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Types
{
    /// <summary>
    /// Contains the possible access rights for a user.
    /// </summary>
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    [Flags]
    public enum AccessRights
    {
        /// <summary>
        /// No access.
        /// </summary>
        None = 0,
        /// <summary>
        /// The right to read the specified type of record.
        /// </summary>
        ReadAccess = 1,
        /// <summary>
        /// The right to update the specified record.
        /// </summary>
        WriteAccess = 2,
        /// <summary>
        /// The right to append the specified record to another object.
        /// </summary>
        AppendAccess = 4,
        /// <summary>
        /// The right to append another record to the specified object.
        /// </summary>
        AppendToAccess = 16,
        /// <summary>
        /// The right to create a record.
        /// </summary>
        CreateAccess = 32,
        /// <summary>
        /// The right to delete the specified record.
        /// </summary>
        DeleteAccess = 65536,
        /// <summary>
        /// The right to share the specified record.
        /// </summary>
        ShareAccess = 262144,
        /// <summary>
        /// The right to assign the specified record to another user or team.
        /// </summary>
        AssignAccess = 524288
    }
}
