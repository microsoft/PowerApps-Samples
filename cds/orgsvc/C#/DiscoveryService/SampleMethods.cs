using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Discovery;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
  public partial class SampleProgram
  {
    /// <summary>
    /// Gets organization data for user in all regions when data center is unknown
    /// </summary>
    /// <param name="username">The user's username</param>
    /// <param name="password">The user's password</param>
    /// <param name="dataCenter">The DataCenter enum balue that corresponds to the data center if known. Otherwise DataCenter.Unknown</param>
    /// <returns>A List of OrganizationDetail records</returns>
    public static List<OrganizationDetail> GetAllOrganizations(string username, string password, DataCenter dataCenter)
    {
      var organizations = new List<OrganizationDetail>();

      //Create ClientCredentials from user's name and password
      ClientCredentials creds = new ClientCredentials();
      creds.UserName.UserName = username;
      creds.UserName.Password = password;

      //If DataCenter.Unknown is used, loop through all known DataCenters
      if (dataCenter == DataCenter.Unknown)
      {
        foreach (DataCenter dc in (DataCenter[])Enum.GetValues(typeof(DataCenter)))
        {

          if (dc != DataCenter.Unknown)
          {
            //Get the organization detail information for a specific region
            List<OrganizationDetail> orgs = GetOrganizationsForDataCenter(creds, dc);
            organizations = organizations.Concat(orgs).ToList();
          }
        }
      }
      else
      {
        //When the data center is not unknown, get data from the specified region
        organizations = GetOrganizationsForDataCenter(creds, dataCenter);
      }

      return organizations;
    }

    /// <summary>
    /// Get organization data for a specific known region only
    /// </summary>
    /// <param name="creds"></param>
    /// <param name="dataCenter"></param>
    /// <returns></returns>
    public static List<OrganizationDetail> GetOrganizationsForDataCenter(ClientCredentials creds, DataCenter dataCenter)
    {
      if (dataCenter == DataCenter.Unknown)
      {
        throw new ArgumentOutOfRangeException("DataCenter.Unknown cannot be used as a parameter for this method.");
      }

      //Get the DataCenter URL from the Description Attribute applied for the DataCenter member
      var type = typeof(DataCenter);
      var memInfo = type.GetMember(dataCenter.ToString());
      var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
      string url = ((DescriptionAttribute)attributes[0]).Description;

      var organizations = new List<OrganizationDetail>();

      using (var dsvc = new DiscoveryServiceProxy(new Uri(url), null, creds, null))
      {
        RetrieveOrganizationsRequest orgsRequest = new RetrieveOrganizationsRequest()
        {
          AccessType = EndpointAccessType.Default,
          Release = OrganizationRelease.Current
        };

        try
        {
          RetrieveOrganizationsResponse response = (RetrieveOrganizationsResponse)dsvc.Execute(orgsRequest);

          organizations = response.Details.ToList();
        }
        catch (System.ServiceModel.Security.SecurityAccessDeniedException)
        {
          //This error is expected for regions where the user has no organizations
        }

      };
      return organizations;
    }

    /// <summary>
    /// Gets Dictionary of connection string values
    /// </summary>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    public static Dictionary<string, string> GetConnectionStringKeysAndValues(string connectionString)
    {
      char splitAttribute = ';';
      char splitKeyValue = '=';

      Dictionary<string, string> setting = new Dictionary<string, string>();

      string[] attributes = connectionString.Split(splitAttribute);
      string[] keyAndValue = null;
      string key = string.Empty;
      string value = string.Empty;

      foreach (string attribute in attributes)
      {
        keyAndValue = attribute.Split(splitKeyValue);
        key = keyAndValue[0].Trim();
        value = keyAndValue[1].Trim();

        setting.Add(key, value);
      }

      return setting;
    }
  }

  /// <summary>
  /// An enum for the known data centers
  /// </summary>
  public enum DataCenter
  {
    [Description("Unknown")]
    Unknown,
    [Description("https://disco.crm.dynamics.com/XRMServices/2011/Discovery.svc")]
    NorthAmerica,
    [Description("https://disco.crm2.dynamics.com/XRMServices/2011/Discovery.svc")]
    SouthAmerica,
    [Description("https://disco.crm3.dynamics.com/XRMServices/2011/Discovery.svc")]
    Canada,
    [Description("https://disco.crm4.dynamics.com/XRMServices/2011/Discovery.svc")]
    EMEA,
    [Description("https://disco.crm5.dynamics.com/XRMServices/2011/Discovery.svc")]
    APAC,
    [Description("https://disco.crm6.dynamics.com/XRMServices/2011/Discovery.svc")]
    Oceania,
    [Description("https://disco.crm7.dynamics.com/XRMServices/2011/Discovery.svc")]
    Japan,
    [Description("https://disco.crm8.dynamics.com/XRMServices/2011/Discovery.svc")]
    India,
    [Description("https://disco.crm9.dynamics.com/XRMServices/2011/Discovery.svc")]
    NorthAmerica2,
    [Description("https://disco.crm11.dynamics.com/XRMServices/2011/Discovery.svc")]
    UK
  }
}
