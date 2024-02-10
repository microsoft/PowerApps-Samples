namespace PowerPlatform.Dataverse.CodeSamples
{
    public static class Settings
    {

        /// <summary>
        /// Whether you want to use elastic tables. Otherwise, standard table will be used.
        /// </summary>
        public const bool UseElastic = false;

        /// <summary>
        /// The number of records to create for all samples in this solution.
        /// </summary>
        public const int NumberOfRecords = 100;

        /// <summary>
        /// The maximum number of records operations to send with 
        /// ExecuteMultiple, CreateMultiple, and UpdateMultiple.
        /// ExecuteMultiple cannot exceed 1000.
        /// </summary>
        public const short StandardBatchSize = 1000;

        /// <summary>
        /// The recommended number of records operations to send with 
        /// CreateMultiple, UpdateMultiple and DeleteMultiple
        /// for Elastic tables is 100. 
        /// You can use a higher or lower number, but a higher batch size isn't
        /// necessarily going to provide higher throughput because there is no
        /// transaction with elastic tables.
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

        /// <summary>
        /// Whether to create alternate key for the table at the end of each sample.
        /// </summary>
        public const bool CreateAlternateKey = false;

        /// <summary>
        /// Column name for PartitionId in Elastic tables
        /// </summary>
        public const string ElasticTablePartitionId = "partitionid";

    }
}

