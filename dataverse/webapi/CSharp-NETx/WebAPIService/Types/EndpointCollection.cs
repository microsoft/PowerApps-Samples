namespace PowerApps.Samples.Types
{
    public class EndpointCollection
    {
        /// <summary>
        /// The number of elements in the collection.
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// Whether the DataCollection is read-only
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Collection containing the keys in the DataCollection.
        /// </summary>
        public List<EndpointType> Keys { get; set; }

        /// <summary>
        /// Collection containing the values in the DataCollection.
        /// </summary>
        public List<string> Values { get; set; }
    }
}
