using System;

namespace PowerApps.Samples.Metadata
{
    //Modified to reflect changes to EntityMetadata for Web API
    [Flags]
    public enum EntityFilters
    {
        EntityOnly = 0,
        Keys = 2,
        Attributes = 4,
        ManyToManyRelationships = 8,
        ManyToOneRelationships = 16,
        OneToManyRelationships = 32,
        AllRelationships = 64,
        All = 128
    }
}