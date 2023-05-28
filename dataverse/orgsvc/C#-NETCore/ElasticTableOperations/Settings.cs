namespace PowerPlatform.Dataverse.CodeSamples
{
    public static class Settings
    {
        /// <summary>
        /// The number of records to create for all samples in this solution.
        /// </summary>
        public const int NumberOfRecords = 1000;

        /// <summary>
        /// The maximum number of records operations to send with 
        /// CreateMultiple, UpdateMultiple and DeleteMultiple.
        /// </summary>
        public const short BatchSize = 100;
    }
}
