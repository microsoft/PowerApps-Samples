using System.Collections.Generic;

namespace PowerApps.Samples
{
    public class EntityRecordCountCollection
    {
        //Number of elements in the collection.
        public int Count { get; set; }
        // 	Value indicating whether the DataCollection is read-only.
        public bool IsReadOnly { get; set; }
        // Collection containing the keys in the DataCollection.
        public List<string> Keys { get; set; }
        // Collection containing the values in the DataCollection.
        public List<long> Values { get; set; }

    }
}
