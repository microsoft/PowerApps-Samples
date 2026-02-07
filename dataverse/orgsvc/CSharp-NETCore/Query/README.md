# Query

Samples demonstrating various query methods in Dataverse including QueryExpression, FetchXML, and LINQ.

These samples show how to retrieve data using different query APIs, implement paging, use aggregation functions, work with saved queries, and query specialized data like intersect tables and working hours.

More information: [Query data using the SDK for .NET](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/entity-operations-query-data)

## Samples

|Sample folder|Description|Build target|
|---|---|---|
|Convertqueriesfetchqueryexpressions|Convert queries between FetchXML and QueryExpression|.NET 6|
|QueriesUsingLINQ|Query data using LINQ|.NET 6|
|RetrieveMultipleByQueryExpression|Retrieve multiple records using QueryExpression|.NET 6|
|RetrieveMultipleQueryByAttribute|Retrieve multiple records using QueryByAttribute|.NET 6|
|RetrieveRecordsFromIntersectTable|Query many-to-many relationship intersect tables|.NET 6|
|UseAggregationInFetchXML|Use aggregate functions in FetchXML queries|.NET 6|
|UseFetchXMLWithPaging|Implement paging with FetchXML|.NET 6|
|UseQueryExpressionwithPaging|Implement paging with QueryExpression|.NET 6|
|ValidateandExecuteSavedQuery|Work with saved queries (views)|.NET 6|
|QueryByReciprocalRole|Query connection records by reciprocal role|.NET 6|
|QueryByRecord|Query connections for specific records|.NET 6|
|QueryHoursMultipleUsers|Query working hours for multiple users|.NET 6|
|QueryWorkingHours|Query working hours schedules|.NET 6|
|ExportDataUsingFetchXmlToAnnotation|Export query results to annotations|.NET 6|

## Prerequisites

- Visual Studio 2022 or later
- .NET 6.0 SDK or later
- Access to a Dataverse environment

## How to run samples

1. Clone the PowerApps-Samples repository
2. Navigate to `dataverse/orgsvc/CSharp-NETCore/Query/`
3. Open the desired sample folder
4. Edit the `appsettings.json` file (located in the Query folder) with your environment connection details:
   ```json
   {
     "ConnectionStrings": {
       "default": "AuthType=OAuth;Url=https://yourorg.crm.dynamics.com;Username=youruser@yourdomain.com;AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;RedirectUri=http://localhost;LoginPrompt=Auto"
     }
   }
   ```
5. Build and run the sample:
   ```bash
   cd SampleFolder
   dotnet run
   ```

## See also

[Query data using the SDK for .NET](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/entity-operations-query-data)
[Use FetchXML to construct a query](https://learn.microsoft.com/power-apps/developer/data-platform/use-fetchxml-construct-query)
[Build queries with QueryExpression](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/build-queries-with-queryexpression)
[Query data using LINQ](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/query-data-using-linq)
