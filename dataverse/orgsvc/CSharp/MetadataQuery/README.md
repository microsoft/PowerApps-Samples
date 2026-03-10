# Sample: Metadata query changes

This sample shows how to retrieve and detect metadata changes using [RetrieveMetadataChangeRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.retrievemetadatachangesrequest) method.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `RetrieveMetadataChangeRequest` message is intended to be used in a scenario where it contains the data  that is needed to to retrieve a collection of metadata records that satisfy the specified criteria. The [RetrieveMetadataChangesResponse](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.retrievemetadatachangesresponse) returns a timestamp value that can be used with this request at a later time to return information about how metadata has changed since the last request.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Checks for the current version of the org.

### Demonstrate

1. The `MetadataFilterExpression` method creates the filter expression to limit entities returned to non-intersect, user-owned entities not found in the list of excluded entities. 
2. The `MetadataConditionExpression` method returns the optionset attributes.
3. The `MetadataPropertiesExpression` method limits the properties to be included with the attributes.
4. The `LabelQueryExpression` method limits the labels returned to only those for user's preferred language.
5. The `RetrieveMetadataChangeRequest` method retrieves the metadata for the query.


### Clean up

Display an option to delete the sample data that is created in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
