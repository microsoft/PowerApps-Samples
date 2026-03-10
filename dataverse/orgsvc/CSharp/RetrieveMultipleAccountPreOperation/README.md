# Sample: Modify query in PreOperation stage

This sample shows how to write a plug-in that modifies a query defined within the `PreOperation` stage of a `RetrieveMultiple` request.

Data filtering in a plug-in is commonly done in the `PostOperation` stage. The [EntityCollection.Entities](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.entitycollection.entities) data can be examined and entities that should not be returned are removed from the collection. But this pattern introduces issues where the number of records returned within a page may not match the expected paging sizes.

The approach described by this sample is different. Rather than filter entities after they have been retrieved, this plug-in will apply changes to the query in the `PreOperation` stage before it is executed.

A key point demonstrated by this sample is that the [RetrieveMultipleRequest.Query](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.retrievemultiplerequest.query) can be one of three different types that are derived from the [QueryBase Class](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.query.querybase). To accommodate queries of any type, the plug-in code
must detect the type of query and implement the appropriate type of filter.

You can download the sample from [here](https://github.com/Microsoft/PowerApps-Samples/tree/master/dataverse/orgsvc/CSharp/RetrieveMultipleAccountPreOperation).

## How to run this sample

1. Download or clone the [Samples](https://github.com/Microsoft/PowerApps-Samples) repo so that you have a local copy. This sample is located under PowerApps-Samples-master\cds\orgsvc\CSharp\RetrieveMultipleAccountPreOperation.
1. Open the sample solution in Visual Studio, navigate to the project's properties, and verify the assembly will be signed during the build. Press F6 to build the sample's assembly (RetrieveMultipleAccountPreOperation.dll).
1. Run the Plug-in Registration tool and register the assembly in Microsoft Dataverse server's sandbox and database for the `PreOperation` stage of the `RetrieveMultiple` message for the `Account` table.
1. Using an app or write code to retrieve accounts to trigger the plug-in. See [Code to test this sample](#code-to-test-this-sample) below for an example.
1. When you are done testing, unregister the assembly and step.

## What this sample does

When executed, the plug-in will ensure that inactive account records will not be returned for the most common types of queries: [QueryExpression](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.query.queryexpression) and [FetchExpression](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.query.fetchexpression).

[QueryByAttribute](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.query.querybyattribute) is a third type of query that may also be used. It doesn't support complex queries and therefore complex filtering cannot be applied using this method. Fortunately, this type of query is not frequently used. You may want to reject queries of this type by throwing an [InvalidPluginExecutionException](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.invalidpluginexecutionexception) in the `PreValidation` stage.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

1. Verify that the input parameters includes a parameter named `Query`
1. Test the type of the query by attempting to cast it as one of the three expected types.
1. Based on the type of the query, the query is altered in the following manner:

### FetchExpression

1. Parse the [FetchExpression.Query](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.query.fetchexpression.query) value containing the FetchXml into an [XDocument](https://learn.microsoft.com/dotnet/api/system.xml.linq.xdocument)
1. Verify that the `entity` element `attribute` column specifies the `account` table.
1. Examine all the `filter` elements in the query for conditions that test the `statecode` column.
1. Remove any existing conditions based on that column.
1. Add a new `filter` to the Query that requires that only accounts where the `statecode` is not equal to 1 (Inactive) will be returned.
1. Set the modified query to the `FetchExpression.Query` value

```csharp
if (fetchExpressionQuery != null)
{
    tracingService.Trace("Found FetchExpression Query");

    XDocument fetchXmlDoc = XDocument.Parse(fetchExpressionQuery.Query);
    //The required element
    var entityElement = fetchXmlDoc.Descendants("entity").FirstOrDefault();
    var entityName = entityElement.Attributes("name").FirstOrDefault().Value;

    //Only applying to the account table
    if (entityName == "account")
    {
        tracingService.Trace("Query on Account confirmed");

        //Get all filter elements
        var filterElements = entityElement.Descendants("filter");

        //Find any existing statecode conditions
        var stateCodeConditions = from c in filterElements.Descendants("condition")
                                    where c.Attribute("attribute").Value.Equals("statecode")
                                    select c;

        if (stateCodeConditions.Count() > 0)
        {
            tracingService.Trace("Removing existing statecode filter conditions.");
        }
        //Remove statecode conditions
        stateCodeConditions.ToList().ForEach(x => x.Remove());


        //Add the condition you want in a new filter
        entityElement.Add(
            new XElement("filter",
                new XElement("condition",
                    new XAttribute("attribute", "statecode"),
                    new XAttribute("operator", "neq"), //not equal
                    new XAttribute("value", "1") //Inactive
                    )
                )
            );
    }


    fetchExpressionQuery.Query = fetchXmlDoc.ToString();

}
```

### QueryExpression

1. Verify that the [QueryExpression.EntityName](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.query.queryexpression.entityname) is the `account` table.
1. Loop through the [QueryExpression.Criteria.Filters](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.query.filterexpression.filters) collection
1. Use the recursive `RemoveAttributeConditions` method to look for any [ConditionExpression](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.query.conditionexpression) instances that test the statecode column and remove them.
1. Add a new [FilterExpression](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.query.filterexpression) to the `QueryExpression.Criteria.Filters` collection that requires that only accounts where the `statecode` is not equal to 1 (Inactive) will be returned.

```csharp
if (queryExpressionQuery != null)
{
    tracingService.Trace("Found Query Expression Query");
    if (queryExpressionQuery.EntityName.Equals("account"))
    {
        tracingService.Trace("Query on Account confirmed");

        //Recursively remove any conditions referring to the statecode column
        foreach (FilterExpression fe in queryExpressionQuery.Criteria.Filters)
        {
            //Remove any existing criteria based on statecode column
            RemoveAttributeConditions(fe, "statecode", tracingService);
        }

        //Define the filter
        var stateCodeFilter = new FilterExpression();
        stateCodeFilter.AddCondition("statecode", ConditionOperator.NotEqual, 1);
        //Add it to the Criteria
        queryExpressionQuery.Criteria.AddFilter(stateCodeFilter);
    }

}
```

#### RemoveAttributeConditions method

A recursive method that removes any conditions for a specific named column

```csharp
/// <summary>
/// Removes any conditions using a specific named column
/// </summary>
/// <param name="filter">The filter that may have a condition using the column</param>
/// <param name="attributeName">The name of the column that should not be used in a condition</param>
/// <param name="tracingService">The tracing service to use</param>
private void RemoveAttributeConditions(FilterExpression filter, string attributeName, ITracingService tracingService)
{

    List<ConditionExpression> conditionsToRemove = new List<ConditionExpression>();

    foreach (ConditionExpression ce in filter.Conditions)
    {
        if (ce.AttributeName.Equals(attributeName))
        {
            conditionsToRemove.Add(ce);
        }
    }

    conditionsToRemove.ForEach(x =>
    {
        filter.Conditions.Remove(x);
        tracingService.Trace("Removed existing statecode filter conditions.");
    });

    foreach (FilterExpression fe in filter.Filters)
    {
        RemoveAttributeConditions(fe, attributeName, tracingService);
    }
}
```

### QueryByAttribute

Because QueryByAttribute doesn't support complex filters, only write a message to the plug-in trace log.

If you don't want this type of query to be used at all, you could throw an InvalidPluginExecutionException to prevent the operation, but this would be better applied during the `PreValidation` stage.

```csharp
if (queryByAttributeQuery != null)
{
    tracingService.Trace("Found Query By Attribute Query");
    //Query by attribute doesn't provide a complex query model that 
    // can be manipulated
}
```

## Code to test this sample

The following code demonstrates 5 different ways to perform the same query that will trigger the plug-in.

By specifying a specific criteria, in this case the `address1_city` colum value, which only one active record will match, these queries will return just that record.

Then, deactivate that record and run this code a second time. No records will be returned.

```csharp
try
{
    string account_city_value = "Redmond";

    //QueryByAttribute
    var queryByAttribute = new QueryByAttribute("account")
    {
        TopCount = 1,
        ColumnSet = new ColumnSet("accountid", "name")
    };
    queryByAttribute.AddAttributeValue("address1_city", account_city_value);
    queryByAttribute.AddOrder("name", OrderType.Descending);

    //QueryExpression
    var queryExpression = new QueryExpression("account")
    { ColumnSet = new ColumnSet("accountid", "name"), TopCount = 1 };
    queryExpression.Orders.Add(new OrderExpression("name", OrderType.Descending));
    var qeFilter = new FilterExpression(LogicalOperator.And);
    qeFilter.AddCondition(new ConditionExpression("address1_city", ConditionOperator.Equal, account_city_value));
    queryExpression.Criteria = qeFilter;

    //Fetch
    var fetchXml = $@"<fetch mapping='logical' count='1'>   
                <entity name='account'>  
                    <attribute name='accountid'/>   
                    <attribute name='name'/>   
                    <order attribute='name' descending='true' />
                    <filter>
                    <condition attribute='address1_city' operator='eq' value='{account_city_value}' />
                    </filter>
                </entity>  
            </fetch>";

    var fetchExpression = new FetchExpression(fetchXml);

    //Get results:
    var queryByAttributeResults = service.RetrieveMultiple(queryByAttribute);
    var queryExpressionResults = service.RetrieveMultiple(queryExpression);
    var fetchExpressionResults = service.RetrieveMultiple(fetchExpression);

    //WebAPI
    string WebAPIAccountName = string.Empty;

    Dictionary<string, List<string>> ODataHeaders = new Dictionary<string, List<string>>() {
    {"Accept", new List<string>(){"application/json" } },
    {"OData-MaxVersion", new List<string>(){ "4.0" } },
    {"OData-Version", new List<string>(){ "4.0" } }};


    HttpResponseMessage response = service.ExecuteCrmWebRequest(HttpMethod.Get,
        $"accounts?$select=accountid,name&$top=1&$orderby=name desc&$filter=address1_city eq '{account_city_value}'",
        string.Empty,
        ODataHeaders);
    if (response.IsSuccessStatusCode)
    {
        var results = response.Content.ReadAsStringAsync().Result;
        var jsonResults = JObject.Parse(results);
        var accounts = (JArray)jsonResults.GetValue("value");
        if (accounts.Count > 0)
        {
            var account = accounts.First();
            WebAPIAccountName = account.Value<string>("name");
        }

    }

    else
    {
        Console.WriteLine(response.ReasonPhrase);
    }

    //Using Fetch with Web API
    string FetchWebAPIAccountName = string.Empty;
    HttpResponseMessage fetchResponse = service.ExecuteCrmWebRequest(HttpMethod.Get,
    $"accounts?fetchXml=" + Uri.EscapeDataString(fetchXml),
    string.Empty,
    ODataHeaders);
    if (fetchResponse.IsSuccessStatusCode)
    {

        var results = fetchResponse.Content.ReadAsStringAsync().Result;
        var jsonResults = JObject.Parse(results);
        var accounts = (JArray)jsonResults.GetValue("value");
        if (accounts.Count > 0)
        {
            var account = accounts.First();
            FetchWebAPIAccountName = account.Value<string>("name");
        }
    }

    else
    {
        Console.WriteLine(fetchResponse.ReasonPhrase);
    }

    string no_records_message = "No records returned";

    Console.WriteLine("QueryByAttribute Account Returned: {0}", queryByAttributeResults.Entities.Count > 0 ?
        queryByAttributeResults.Entities[0]["name"] : no_records_message);
    Console.WriteLine("QueryExpression Account Returned: {0}", queryExpressionResults.Entities.Count > 0 ?
        queryExpressionResults.Entities[0]["name"] : no_records_message);
    Console.WriteLine("Fetch Account Returned: {0}", fetchExpressionResults.Entities.Count > 0 ?
        fetchExpressionResults.Entities[0]["name"] : no_records_message);
    Console.WriteLine("WebAPI Account Returned: {0}", WebAPIAccountName != string.Empty ?
        WebAPIAccountName : no_records_message);
    Console.WriteLine("WebAPI Fetch Account Returned: {0}", FetchWebAPIAccountName != string.Empty ?
        FetchWebAPIAccountName : no_records_message);

}
catch (Exception ex)
{

    throw ex;
}
```

### See also

[Use plug-ins to extend business processes](https://learn.microsoft.com/powerapps/developer/common-data-service/plug-ins)<br/>
[Register a plug-in](https://learn.microsoft.com/powerapps/developer/common-data-service/register-plug-in)
