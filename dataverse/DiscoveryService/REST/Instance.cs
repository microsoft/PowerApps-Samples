namespace PowerApps.Samples
{
    /// <summary>
    /// Environment instance returned from the Discovery service.
    /// </summary>
    class Instance
    {
        public string? ApiUrl { get; set; }
        public Guid? DatacenterId { get; set; }
        public string? DatacenterName { get; set; }
        public string? EnvironmentId { get; set; }
        public string? FriendlyName { get; set; }
        public string? Id { get; set; }
        public bool IsUserSysAdmin { get; set; }
        public DateTime? LastUpdated { get; set; }
        public int OrganizationType { get; set; }
        public string? Purpose { get; set; }
        public string? Region { get; set; }
        public string? SchemaType { get; set; }
        public int? State { get; set; }
        public int? StatusMessage { get; set; }
        public string? TenantId { get; set; }
        public string? TrialExpirationDate { get; set; }
        public string? UniqueName { get; set; }
        public string? UrlName { get; set; }       
        public string? Version { get; set; }
        public string? Url { get; set; }              
    }
}
