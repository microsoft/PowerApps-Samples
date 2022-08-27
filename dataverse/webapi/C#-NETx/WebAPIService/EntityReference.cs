using Newtonsoft.Json.Linq;

namespace PowerApps.Samples
{
    public class EntityReference
    {
        /// <summary>
        /// Creates an EntityReference with EntitySetName and Guid Id
        /// </summary>
        /// <param name="entitySetName">The entity set name</param>
        /// <param name="id">The Guid Id value.</param>
        /// <exception cref="Exception"></exception>
        public EntityReference(string entitySetName, Guid? id)
        {
            if (!id.HasValue)
            {
                throw new Exception("EntityReference constructor id parameter must have a value.");
            }

            Id = id.Value;
            SetName = entitySetName;
        }

        /// <summary>
        /// Creates an entity reference from a URI
        /// </summary>
        /// <param name="uri">An absolute or relative URI to a record.</param>
        /// <exception cref="ArgumentException"></exception>
        public EntityReference(string uri)
        {
            int firstParen = uri.LastIndexOf('(');
            int lastParen = uri.LastIndexOf(')');
            int lastBackSlash = uri.LastIndexOf('/') + 1;
            if (lastBackSlash >= 0 && firstParen > lastBackSlash && lastParen > firstParen)
            {
                SetName = uri[lastBackSlash..firstParen];

                firstParen++;

                if (Guid.TryParse(uri[firstParen..lastParen], out Guid id))
                {
                    Id = id;
                }
                else
                {
                    //It may be an alternate key.
                    try
                    {
                        KeyAttributes = uri[firstParen++..lastParen].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                       .Select(part => part.Split('='))
                       .ToDictionary(split => split[0], split => split[1]);
                    }
                    catch (Exception)
                    {
                        throw new ArgumentException("Invalid Uri");
                    }
                }
            }
            else
            {
                throw new ArgumentException("Invalid Uri");
            }
        }

        /// <summary>
        /// Creates an EntityReference with alternate keys
        /// </summary>
        /// <param name="setName">The entity set name</param>
        /// <param name="keyAttributes">The key attributes to use.</param>
        public EntityReference(string setName, Dictionary<string, string>? keyAttributes)
        {
            KeyAttributes = keyAttributes;
            SetName = setName;
        }

        /// <summary>
        /// The primary key value
        /// </summary>
        public Guid? Id { get; set; }
        /// <summary>
        /// Alternate key values
        /// </summary>
        public Dictionary<string, string>? KeyAttributes { get; set; }
        /// <summary>
        /// The EntitySet name
        /// </summary>
        public string SetName { get; set; }
        /// <summary>
        /// The calculated relative Uri to the record.
        /// </summary>
        public string Path
        {
            get
            {
                if (Id.HasValue)
                {
                    return $"{SetName}({Id})";
                }
                else if (KeyAttributes?.Count > 0)
                {

                    return $"{SetName}({string.Join(",", KeyAttributes.Select(kvp => $"{kvp.Key}={kvp.Value}"))})";

                }
                else
                {
                    throw new Exception("Unable to generate path.");
                }
            }
        }

        public string AsODataId()
        {

            return $"{{'@odata.id':'{Path}'}}";
        }

        public JObject AsJObject(string entityLogicalName, string primaryKeyLogicalName)
        {

            if (Id.HasValue)
            {
                return new JObject(){

                    { primaryKeyLogicalName,Id },
                    { "@odata.type",$"Microsoft.Dynamics.CRM.{entityLogicalName}"}
                };

            }
            else
            {
                throw new NotImplementedException("EntityReference.AsJObject can only be used if the primary key value is known.");
            }
        }
    }
}
