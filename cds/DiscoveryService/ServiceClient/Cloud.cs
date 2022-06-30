using System.ComponentModel;

namespace PowerApps.Samples
{
    /// <summary>
    /// An enum for the known Clouds
    /// </summary>
    public enum Cloud
    {
        [Description("https://globaldisco.crm.dynamics.com/api/discovery/v2.0/Instances")]
        Commercial,
        [Description("https://globaldisco.crm9.dynamics.com/api/discovery/v2.0/Instances")]
        GCC,
        [Description("https://globaldisco.crm.microsoftdynamics.us/api/discovery/v2.0/Instances")]
        USG,
        [Description("https://globaldisco.crm.appsplatform.us/api/discovery/v2.0/Instances")]
        DOD,
        [Description("https://globaldisco.crm.dynamics.cn/api/discovery/v2.0/Instances")]
        CHINA
    }
}
