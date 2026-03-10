namespace PowerApps.Samples.Metadata.Types
{
    [Flags]
    public enum EntityFilters
    {
        Entity = 1,
        Attributes = 2,
        Privileges = 4,
        Relationships = 8,
        All = 15
    }
}