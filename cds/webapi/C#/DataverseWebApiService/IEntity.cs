using System;

namespace PowerApps.Samples
{
    public interface IEntity
    {
        Guid? Id { get; }
        public static string SetName { get; }
        public static string LogicalName { get; }
        EntityReference ToEntityReference();

    }
}
