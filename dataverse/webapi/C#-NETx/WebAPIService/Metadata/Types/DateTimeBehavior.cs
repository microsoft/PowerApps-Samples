namespace PowerApps.Samples.Metadata.Types
{
    public class DateTimeBehavior
    {
        private string _value;
        public string Value { 
            get { return _value; }
            set {
                _value = value switch
                {
                    "UserLocal" or "DateOnly" or "TimeZoneIndependent" => value,
                    _ => throw new ArgumentOutOfRangeException($"'{value}' is not a valid DateTimeBehavior value."),
                };
            }
        }
    }
}