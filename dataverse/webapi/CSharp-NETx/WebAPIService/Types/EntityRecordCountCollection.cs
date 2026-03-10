namespace PowerApps.Samples.Types
{
    public class EntityRecordCountCollection
    {
        /// <summary>
        /// Number of elements in the collection.
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// Value indicating whether the DataCollection is read-only.
        /// </summary>
        public bool IsReadOnly { get; set; }
        /// <summary>
        /// Collection containing the keys in the DataCollection.
        /// </summary>
        public List<string> Keys { get; set; }
        /// <summary>
        /// Collection containing the values in the DataCollection.
        /// </summary>
        public List<Int64> Values { get; set; } 
    }
}
