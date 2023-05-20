namespace PowerPlatform.Dataverse.CodeSamples
{
    public static class Settings {

        /// <summary>
        /// The number of records to create for all samples in this solution.
        /// </summary>
        public const int NumberOfRecords = 100;

        /// <summary>
        /// The maximum number of records operations to send with 
        /// ExecuteMultiple, CreateMultiple, and UpdateMultiple.
        /// ExecuteMultiple cannot exceed 1000.
        /// </summary>
        public const short BatchSize = 1000;

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

