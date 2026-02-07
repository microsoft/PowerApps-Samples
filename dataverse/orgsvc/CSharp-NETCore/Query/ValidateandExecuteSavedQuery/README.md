# Sample: Validate and execute a saved query

This sample shows how to validate FetchXML queries and execute both saved queries (system views) and user queries (personal views) using the Dataverse SDK.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `ValidateSavedQueryRequest` message is used to validate that a saved query's FetchXML is well-formed and valid.

The `ExecuteByIdSavedQueryRequest` message is used to execute a saved query (system view) by its ID and return the results as XML.

The `ExecuteByIdUserQueryRequest` message is used to execute a user query (personal view) by its ID and return the results as XML.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Creates 3 sample account records:
   - "Coho Vineyard"
   - "Coho Winery"
   - "Coho Vineyard & Winery"

2. Creates a saved query (system view) that retrieves all account names using FetchXML:
   ```xml
   <fetch mapping='logical'>
       <entity name='account'>
           <attribute name='name' />
       </entity>
   </fetch>
   ```

3. Creates a user query (personal view) that retrieves only the "Coho Winery" account using filtered FetchXML:
   ```xml
   <fetch mapping='logical'>
       <entity name='account'>
           <attribute name='name' />
           <filter>
               <condition attribute='name' operator='eq' value='Coho Winery' />
           </filter>
       </entity>
   </fetch>
   ```

### Demonstrate

1. **Validate the saved query:**
   - Uses `ValidateSavedQueryRequest` to validate the FetchXML syntax
   - The validation will throw an exception if the FetchXML is malformed or invalid
   - Confirms successful validation

2. **Execute the saved query:**
   - Uses `ExecuteByIdSavedQueryRequest` to execute the saved query by ID
   - Returns results as XML string containing all account names
   - Formats and displays the XML results

3. **Execute the user query:**
   - Uses `ExecuteByIdUserQueryRequest` to execute the user query by ID
   - Returns results as XML string containing only "Coho Winery" account
   - Formats and displays the XML results

### Clean up

Displays an option to delete all the data created in the sample. The deletion is optional in case you want to examine the data created by the sample. You can manually delete the data to achieve the same results.

## Key concepts

### Saved Query vs User Query

- **Saved Query (savedquery)**: System-wide views visible to all users (requires appropriate privileges to create)
- **User Query (userquery)**: Personal views created by and visible only to the user who created them

### FetchXML Validation

The `ValidateSavedQueryRequest` message validates:
- XML syntax correctness
- Entity and attribute names exist
- Operators are appropriate for attribute types
- Overall query structure is valid

This is useful when building query editors or when constructing FetchXML programmatically to catch errors before execution.

### Query Execution

Both `ExecuteByIdSavedQueryRequest` and `ExecuteByIdUserQueryRequest`:
- Execute queries by their unique ID
- Return results as XML string (not EntityCollection)
- Are useful when you need the raw XML response format

For most scenarios, using `RetrieveMultiple` with `FetchExpression` is more common and returns structured `EntityCollection` objects.

## Related samples

- [Use QueryExpression with paging](../UseQueryExpressionwithPaging/)
- [Retrieve multiple by QueryExpression](../RetrieveMultipleByQueryExpression/)
- [Retrieve multiple by QueryByAttribute](../RetrieveMultipleQueryByAttribute/)

## Learn more

- [Use FetchXML to construct a query](https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/overview)
- [ValidateSavedQueryRequest Class](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.validatesavedqueryrequest)
- [ExecuteByIdSavedQueryRequest Class](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.executebyidsavedqueryrequest)
- [ExecuteByIdUserQueryRequest Class](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.executebyiduserqueryrequest)
