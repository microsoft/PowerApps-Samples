namespace PowerApps.Samples.Types
{
    public class Object
    {
        public Object() { }

        public Object(ObjectType type, string value) {
            Type = type;
            Value = value; 
        }

        /// <summary>
        /// The name of the.NET type
        /// </summary>
        public ObjectType Type { get; set; }

        /// <summary>
        /// The value to use for comparison.
        /// </summary>
        public string Value { get; set; }
    }
}
