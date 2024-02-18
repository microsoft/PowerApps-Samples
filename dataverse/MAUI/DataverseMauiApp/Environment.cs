using System.Windows.Input;

namespace DataverseMauiApp
{
  public class Environment
  {
    public bool IsUserSysAdmin { get; set; }
    public string Region { get; set; }
    public string Purpose { get; set; }
    public int StatusMessage { get; set; }
    public DateTime TrialExpirationDate { get; set; }
    public OrganizationType OrganizationType { get; set; }
    public string TenantId { get; set; }
    public string EnvironmentId { get; set; }
    public string DatacenterId { get; set; }
    public object DatacenterName { get; set; }
    public string Id { get; set; }
    public string UniqueName { get; set; }
    public string UrlName { get; set; }
    public string FriendlyName { get; set; }
    public int State { get; set; }
    public string Version { get; set; }
    public string Url { get; set; }
    public string ApiUrl { get; set; }
    public DateTime LastUpdated { get; set; }
    public string SchemaType { get; set; }

  }
}
