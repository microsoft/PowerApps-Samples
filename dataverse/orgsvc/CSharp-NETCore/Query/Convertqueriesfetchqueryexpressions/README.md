# Convert queries between FetchXML and QueryExpression

This sample demonstrates how to convert queries between FetchXML and QueryExpression formats using the Dataverse SDK.

## Demonstrates

This sample shows how to:

1. Build a complex `QueryExpression` with link entities and multiple filters
2. Convert a `QueryExpression` to FetchXML using `QueryExpressionToFetchXmlRequest`
3. Execute queries in both QueryExpression and FetchXML formats
4. Convert FetchXML to `QueryExpression` using `FetchXmlToQueryExpressionRequest`
5. Work with aliased values from linked entities in query results

## Key Concepts

### QueryExpression to FetchXML Conversion

The sample creates a `QueryExpression` that queries contacts with:
- Link to parent account entity
- Multiple filter conditions (state, city, creation date, email)
- OR conditions for phone numbers
- Retrieves full name and phone number

It then converts this to FetchXML using `QueryExpressionToFetchXmlRequest` and executes both versions to show they produce identical results.

### FetchXML to QueryExpression Conversion

The sample uses FetchXML that queries opportunities with:
- Filter on estimated close date (next 3 fiscal years)
- Nested link entities (opportunity -> account -> contact)
- Conditions on the linked contact entity

It converts this to a `QueryExpression` using `FetchXmlToQueryExpressionRequest` and demonstrates that both formats retrieve the same data.

### Working with Aliased Values

When querying linked entities, the results contain `AliasedValue` objects. The sample shows how to:
- Access aliased values using the entity alias prefix (e.g., "contact2.fullname")
- Extract the actual value from the `AliasedValue` object

## Sample Output

```
Connected to Dataverse.

Creating sample data...
Setup complete.

=== QueryExpression to FetchXML Conversion ===

Output for query as QueryExpression:
List all contacts matching specified parameters
===============================================
Contact ID: {guid}
Contact Name: Ben Andrews
Contact Phone: (206)555-5555

Contact ID: {guid}
Contact Name: Colin Wilcox
Contact Phone: (425)555-5555

<End of Listing>

Converted FetchXML:
<fetch mapping='logical' distinct='false'>
  <entity name='contact'>
    <attribute name='fullname' />
    <attribute name='address1_telephone1' />
    ...
  </entity>
</fetch>

Output for query after conversion to FetchXML:
List all contacts matching specified parameters
===============================================
Contact ID: {guid}
Contact Name: Ben Andrews
Contact Phone: (206)555-5555

Contact ID: {guid}
Contact Name: Colin Wilcox
Contact Phone: (425)555-5555

<End of Listing>

=== FetchXML to QueryExpression Conversion ===

Original FetchXML:
<fetch mapping='logical' version='1.0'>
  <entity name='opportunity'>
    <attribute name='name' />
    ...
  </entity>
</fetch>

Output for query as FetchXML:
List all opportunities matching specified parameters.
===========================================================================
Opportunity ID: {guid}
Opportunity: Litware, Inc. Opportunity 2
Associated contact: Colin Wilcox

<End of Listing>

Output for query after conversion to QueryExpression:
List all opportunities matching specified parameters.
===========================================================================
Opportunity ID: {guid}
Opportunity: Litware, Inc. Opportunity 2
Associated contact: Colin Wilcox

<End of Listing>

Cleaning up...
Deleting 5 created record(s)...
Records deleted.

Press any key to exit.
```

## How to run the sample

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
2. Open the `Convertqueriesfetchqueryexpressions.csproj` file in Visual Studio 2022 or later.
3. Edit the `appsettings.json` file in the parent `Query` folder. Set the connection string values appropriate for your test environment.
4. Build and run the project.

The sample will:
- Create test data (1 account, 2 contacts, 2 opportunities)
- Demonstrate QueryExpression to FetchXML conversion
- Demonstrate FetchXML to QueryExpression conversion
- Display query results from both formats
- Clean up the test data

## Clean up

By default, this sample deletes all the data it creates. If you want to view the created data, change the `deleteCreatedRecords` variable to `false` in the `Main` method before the `finally` block.

## What this sample does

The sample uses the following message requests:

- [QueryExpressionToFetchXmlRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.queryexpressiontofetchxmlrequest): Converts a QueryExpression to FetchXML format
- [FetchXmlToQueryExpressionRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.fetchxmltoqueryrequestexpression): Converts FetchXML to QueryExpression format

## Supporting information

- [Query data using FetchXML](https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/overview)
- [Build queries with QueryExpression](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/build-queries-with-queryexpression)
- [Use FetchXML to construct a query](https://learn.microsoft.com/power-apps/developer/data-platform/use-fetchxml-construct-query)
