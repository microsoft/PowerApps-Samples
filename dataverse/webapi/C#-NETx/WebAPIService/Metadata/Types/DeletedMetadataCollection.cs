using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
  public class DeletedMetadataCollection
  {
    public int Count { get; set; }
    public bool IsReadOnly { get; set; }

    public List<DeletedMetadataFilters> Keys { get; set; }

    public List<Guid> Values { get; set; }
  }
}
