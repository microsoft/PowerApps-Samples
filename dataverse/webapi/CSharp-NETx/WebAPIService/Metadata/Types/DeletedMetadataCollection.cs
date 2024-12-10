using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class DeletedMetadataCollection
    {
        public GuidCollection this[DeletedMetadataFilters index]
        {
            get
            {
                if (Keys.Contains(index))
                {
                    return Values[Keys.IndexOf(index)];
                }
                else
                {
                    return new GuidCollection();
                }
            }
        }

        public int Count { get; set; }
        public bool IsReadOnly { get; set; }

        public List<DeletedMetadataFilters> Keys { get; set; }

        public List<GuidCollection> Values { get; set; }
    }
}
