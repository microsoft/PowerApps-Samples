using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
  [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
  public class DeletedMetadataCollection
  {
        public List<GuidCollection> this[DeletedMetadataFilters index]
        {
            get { 

            List<GuidCollection> result = new();
                int count = 0;
                foreach (DeletedMetadataFilters filter in Keys) {
                    if (filter == index)
                    {
                        result.Add(Values[count]);                    
                    }
                    count++;
                }
                return result;           
            }
        }

    public int Count { get; set; }
    public bool IsReadOnly { get; set; }

    public List<DeletedMetadataFilters> Keys { get; set; }

    public List<GuidCollection> Values { get; set; }
  }
}
