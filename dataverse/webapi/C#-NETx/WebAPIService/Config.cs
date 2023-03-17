namespace PowerApps.Samples
{
    public class Config
    {
        /// <summary>
        /// A function provided by the client application to  return access token.
        /// </summary>
        public Func<Task<string>>? GetAccessToken { get; set; }
        /// <summary>
        /// The Url of the environment: https://org.api.crm.dynamics.com
        /// </summary>
        public string? Url { get; set; }
        /// <summary>
        /// The systemuserid value to apply for impersonation;
        /// </summary>
        public Guid CallerObjectId { get; set; }
        /// <summary>
        /// How long to wait for a timeout
        /// </summary>
        public ushort TimeoutInSeconds { get; set; } = 120;
        /// <summary>
        /// Maximum number of times to re-try when service protection limits hit
        /// </summary>
        public byte MaxRetries { get; set; } = 3;
        /// <summary>
        /// The version of the service to use
        /// </summary>
        public string Version { get; set; } = "9.2";
        /// <summary>
        /// Whether to disable Affinity cookies to gain performance
        /// </summary>
        public bool DisableCookies { get; set; } = false;


    }
}
