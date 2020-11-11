using System;
using System.Linq;
using System.Security;

namespace PowerApps.Samples
{
   public class ServiceConfig
    {
        private static string connectionString;
        private string authority = "https://login.microsoftonline.com/common";
        private string url = null;
        private string clientId = null;
        private string redirectUrl = null;

        /// <summary>
        /// Constructor that parses a connection string
        /// </summary>
        /// <param name="connectionStringParam">The connection string to instantiate the configuration</param>
        public ServiceConfig(string connectionStringParam)
        {
            connectionString = connectionStringParam;

            string authorityValue = GetParameterValue("Authority");
            if (!string.IsNullOrEmpty(authorityValue))
            {
                Authority = authorityValue;
            }

            Url = GetParameterValue("Url");
            ClientId = GetParameterValue("ClientId");
            RedirectUrl = GetParameterValue("RedirectUrl");

            string userPrincipalNameValue = GetParameterValue("UserPrincipalName");
            if (!string.IsNullOrEmpty(userPrincipalNameValue))
            {
                UserPrincipalName = userPrincipalNameValue;
            }

            if (Guid.TryParse(GetParameterValue("CallerObjectId"), out Guid callerObjectId))
            {
                CallerObjectId = callerObjectId;
            }

            string versionValue = GetParameterValue("Version");
            if (!string.IsNullOrEmpty(versionValue))
            {
                Version = versionValue;
            }

            if (byte.TryParse(GetParameterValue("MaxRetries"), out byte maxRetries))
            {
                MaxRetries = maxRetries;
            }

            if (ushort.TryParse(GetParameterValue("TimeoutInSeconds"), out ushort timeoutInSeconds))
            {
                TimeoutInSeconds = timeoutInSeconds;
            }


            string pwd = GetParameterValue("Password");
            if (!string.IsNullOrEmpty(pwd))
            {
                var ss = new SecureString();

                pwd.ToCharArray().ToList().ForEach(ss.AppendChar);
                ss.MakeReadOnly();

                Password = ss;
            }


        }
        /// <summary>
        /// The authority to use to authorize user. 
        /// Default is 'https://login.microsoftonline.com/common'
        /// </summary>
        public string Authority
        {
            get => authority; set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    authority = value;
                }
                else
                {
                    throw new Exception("Service.Authority value cannot be null.");
                }
            }
        }
        /// <summary>
        /// The Url to the CDS environment, i.e "https://yourorg.api.crm.dynamics.com"
        /// </summary>
        public string Url
        {
            get => url; set

            {
                if (!string.IsNullOrEmpty(value))
                {
                    url = value;
                }
                else
                {
                    throw new Exception("Service.Url value cannot be null.");
                }
            }
        }
        /// <summary>
        /// The id of the application registered with Azure AD
        /// </summary>
        public string ClientId
        {
            get => clientId; set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    clientId = value;
                }
                else
                {
                    throw new Exception("Service.ClientId value cannot be null.");
                }
            }
        }

        /// <summary>
        /// The Redirect Url of the application registered with Azure AD
        /// </summary>
        public string RedirectUrl
        {
            get => redirectUrl; set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    redirectUrl = value;
                }
                else
                {
                    throw new Exception("Service.RedirectUrl value cannot be null.");
                }
            }
        }

        /// <summary>
        /// The user principal name of the user. i.e. you@yourorg.onmicrosoft.com
        /// </summary>
        public string UserPrincipalName { get; set; } = null;

        /// <summary>
        /// The password for the user principal
        /// </summary>
        public SecureString Password { get; set; } = null;

        /// <summary>
        /// The Azure AD ObjectId for the user to impersonate other users.
        /// </summary>
        public Guid CallerObjectId { get; set; }
        /// <summary>
        /// The version of the Web API to use
        /// Default is '9.1'
        /// </summary>
        public string Version { get; set; } = "9.1";
        /// <summary>
        /// The maximum number of attempts to retry a request blocked by service protection limits.
        /// Default is 3.
        /// </summary>
        public byte MaxRetries { get; set; } = 3;
        /// <summary>
        /// The amount of time to try completing a request before it will be cancelled.
        /// Default is 120 (2 minutes)
        /// </summary>
        public ushort TimeoutInSeconds { get; set; } = 120;

        /// <summary>
        /// Extracts a parameter value from a connection string
        /// </summary>
        /// <param name="parameter">The name of the parameter value</param>
        /// <returns></returns>
        private static string GetParameterValue(string parameter)
        {
            try
            {
                string value = connectionString
                    .Split(';')
                    .Where(s => s.Trim()
                    .StartsWith(parameter))
                    .FirstOrDefault()
                    .Split('=')[1];
                if (value.ToLower() == "null")
                {
                    return string.Empty;
                }
                return value;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
