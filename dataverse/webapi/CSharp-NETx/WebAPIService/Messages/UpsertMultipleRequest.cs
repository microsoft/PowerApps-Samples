using Newtonsoft.Json.Linq;
using System.Text;

namespace PowerApps.Samples.Messages
{
    public class UpsertMultipleRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes the UpsertMultipleRequest
        /// </summary>
        /// <param name="entitySetName">The entity set name for the table.</param>
        /// <param name="targets">JObject containing the data to update.</param>
        /// <param name="entityLogicalName">The logical name for the table. Required when using setType.</param>
        /// <param name="setType">Option to set the @odata.type if the property not set on the targets.</param>
        public UpsertMultipleRequest(
            string entitySetName, 
            List<JObject> targets, 
            string? entityLogicalName = null,
            bool setType = false)
        {
            // Provides an option to set the required @odata.type property
            // if the targets do not already have it.
            if (setType && !string.IsNullOrWhiteSpace(entityLogicalName))
            {
                targets.ForEach(t =>
                {
                    if (!t.ContainsKey("@odata.type"))
                    {
                        t.Add(propertyName: "@odata.type", $"Microsoft.Dynamics.CRM.{entityLogicalName}");
                    }
                });
            }


            JObject targetsObj = new() {
                {"Targets", JToken.FromObject(targets.ToArray()) }
            };


            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: $"{entitySetName}/Microsoft.Dynamics.CRM.UpsertMultiple",
                uriKind: UriKind.Relative);

            Content = new StringContent(
                    content: targetsObj.ToString(),
                    encoding: Encoding.UTF8,
                    mediaType: "application/json");
        }
    }
}
