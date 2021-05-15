using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Collections.Generic;

/*
 $apply=aggregate(annualincome with average as average,annualincome with sum as total,annualincome with min as minimum,annualincome with max as maximum)

{{webapiurl}}accounts?$apply=groupby((primarycontactid/firstname), aggregate(accountid with countdistinct as count))

{{webapiurl}}accounts?$apply=filter(address1_stateorprovince eq 'WA')/groupby((primarycontactid/fullname))

Aggregation behavior is triggered using the query option $apply. It takes a sequence of set transformations, separated by forward slashes to express that they are consecutively applied,


string collectionUri
filter
string groupby - can include comma seaprated expressions
[
{string field,enum agregatetype,string alias}
{field,agregatetype,alias}
]


aggregateMethod
sum
min
max
average
countdistinct
*/

namespace PowerApps.Samples
{
    public static partial class Extensions
    {
        public static async Task<Dictionary<string, JsonElement>> Apply(this Service service, string collectionUri, string applyExpression, bool formattedValues = false)
        {
            try
            {

                string path = service.BaseAddress + collectionUri + "?$apply=" + applyExpression;

                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(path)
                };
                if (formattedValues)
                {
                    request.Headers.Add("Prefer", "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"");
                }
                var response = await service.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                var value = JsonDocument.Parse(content).RootElement.GetProperty("value").ToString();

                var values = JsonSerializer.Deserialize<List<object>>(value);

                var firstValue = values[0].ToString();

                return JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(firstValue);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in Apply: {ex.Message}", ex);
            }
        }

    }

}
