namespace PowerApps.Samples.Types
{
    public class ParameterCollection
    {
        public int Count { get; }
        public bool IsReadOnly { get;}

        public List<string> Keys { get; set; }

        public List<Object> Values { get; set; }
    }
}
