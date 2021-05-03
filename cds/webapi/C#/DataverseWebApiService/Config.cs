using System;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public class Config
    {
        public Func<Task<string>> GetAccessToken { get; set; }
        public string Url { get; set; }
        public Guid CallerObjectId { get; set; }
        public ushort TimeoutInSeconds { get; set; } = 120;
        public byte MaxRetries { get; set; } = 3;
        public string Version { get; set; } = "9.1";

    }
}
