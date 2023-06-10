namespace PowerApps.Samples.Types
{
    /// <summary>
    /// Provides additional information about the solution components that are related to a missing component.
    /// </summary>
    public class ComponentDetail
    {
        /// <summary>
        /// The display name of the solution component.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The ID of the solution component
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The display name of the parent solution component.
        /// </summary>
        public string ParentDisplayName { get; set; }

        /// <summary>
        /// The ID of the parent solution component.
        /// </summary>
        public Guid ParentId { get; set; }

        /// <summary>
        /// The schema name of the parent solution component.
        /// </summary>
        public string ParentSchemaName { get; set; }

        /// <summary>
        /// The schema name of the solution component.
        /// </summary>
        public string SchemaName { get; set; }

        /// <summary>
        /// The name of the solution.
        /// </summary>
        public string Solution { get; set; }

        /// <summary>
        /// The component type of the solution component.
        /// </summary>
        public int Type { get; set; }
    }
}
