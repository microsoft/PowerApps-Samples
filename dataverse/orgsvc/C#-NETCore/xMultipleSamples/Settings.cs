namespace PowerPlatform.Dataverse.CodeSamples
{
    public static class Settings
    {

        /// <summary>
        /// The number of records to create for all samples in this solution.
        /// </summary>
        public const int NumberOfRecords = 100000;

        /// <summary>
        /// The maximum number of records operations to send with 
        /// ExecuteMultiple, CreateMultiple, and UpdateMultiple for Standard entities.
        /// ExecuteMultiple cannot exceed 1000.
        /// </summary>
        public const short StandardBatchSize = 1000;

        /// <summary>
        /// The maximum number of records operations to send with 
        /// CreateMultiple, UpdateMultiple and DeleteMultiple
        /// for Elastic entities cannot exceed 100.
        /// </summary>
        public const short ElasticBatchSize = 100;

        /// <summary>
        /// Whether to bypass custom plugins for all samples in this solution.
        /// </summary>
        public const bool BypassCustomPluginExecution = false;

        /// <summary>
        /// Whether to delete the table at the end of each sample.
        /// You may want to test plug-ins registered for the table.
        /// </summary>
        public const bool DeleteTable = true;


    }
}

