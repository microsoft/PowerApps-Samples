# Sample: Retrieve multiple with the QueryByAttribute class

This sample shows how to retrieve multiple entities using the [IOrganizationService.RetrieveMultiple(QueryBase)](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.retrievemultiple#Microsoft_Xrm_Sdk_IOrganizationService_RetrieveMultiple_Microsoft_Xrm_Sdk_Query_QueryBase_) method with [QueryExpression](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.query.queryexpression) along with their related table columns.

## How to run this sample

[!include[cc-how-to-run-samples](../../includes/cc-how-to-run-samples.md)]

## What this sample does

The `QueryExpression` class is intended to be used in a scenario where it contains a complex query expressed in a hierarchy of expressions.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Checks for the current version of the org.

### Demonstrate

1. Creates multiple accounts with primary contacts.
1. The `QueryExpression` class creates a query expression specifying the link table alias and the columns of the link table that you want to return.

### Clean up

No clean up is required.
