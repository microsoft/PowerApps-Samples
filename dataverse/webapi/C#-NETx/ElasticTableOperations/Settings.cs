namespace PowerPlatform.Dataverse.CodeSamples
{
    public static class Settings
    {
        /// <summary>
        /// The number of records to create 
        /// </summary>
        public const int NumberOfRecords = 1000;

        /// <summary>
        /// The maximum number of records to create with $batch
        /// </summary>
        public const short BatchSize = 100; //Must not exceed 1000
    }
}
