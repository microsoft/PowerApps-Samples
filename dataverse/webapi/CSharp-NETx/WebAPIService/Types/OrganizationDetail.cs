namespace PowerApps.Samples.Types
{
    public class OrganizationDetail
    {
        /// <summary>
        /// The global unique identifier of the organization.
        /// </summary>
        public Guid OrganizationId { get; set; }
        /// <summary>
        /// The friendly name of the organization.
        /// </summary>
        public string FriendlyName { get; set; }
        /// <summary>
        /// The version of the organization.
        /// </summary>
        public string OrganizationVersion { get; set; }

        // In Default environment the value has 'Default-' appended to to a guid value.
        public string EnvironmentId { get; set; }

        public Guid DatacenterId { get; set; }

        public string Geo { get; set; }

        public string TenantId { get; set; }
        /// <summary>
        /// The organization name used in the URL for the organization web service.
        /// </summary>
        public string UrlName { get; set; }
        /// <summary>
        /// The unique name of the organization.
        /// </summary>
        public string UniqueName { get; set; }
        /// <summary>
        /// Collection that identifies the service type and address for each endpoint of the organization.
        /// </summary>
        public EndpointCollection Endpoints { get; set; }
        /// <summary>
        /// The state of the organization.
        /// </summary>
        public OrganizationState State { get; set; }
    }
}
