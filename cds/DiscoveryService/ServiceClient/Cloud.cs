using System.ComponentModel;

namespace PowerApps.Samples
{
    /// <summary>
    /// An enum for the known Clouds
    /// </summary>
    public enum Cloud
    {
        [Description("https://globaldisco.crm.dynamics.com")]
        Commercial,
        [Description("https://globaldisco.crm9.dynamics.com")]
        GCC,
        [Description("https://globaldisco.crm.microsoftdynamics.us")]
        USG,
        [Description("https://globaldisco.crm.appsplatform.us")]
        DOD,
        [Description("https://globaldisco.crm.dynamics.cn")]
        CHINA
    }
}
