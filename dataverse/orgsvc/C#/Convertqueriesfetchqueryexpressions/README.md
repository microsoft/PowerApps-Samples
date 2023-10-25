# Sample: Convert queries between FetchXML and QueryExpression

This sample shows how to convert queries between FetchXML and QueryExpression.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `QueryExpression` and `fetchExpression`messages are intended to be used in a scenario that contains queries in a hierarchy of expressions and FetchXML respectively.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org. 
1. The `CreateRequireRecords` method creates an account and two contact records that are used by the sample.
1. The `QueryExpression` builds a query expression that we will convert into FetchXML.
1. The `DoFetchXmlToQueryExpressionConversion` class creates a Fetch query that we will convert into a query expression.
1. The `conversionRequest` method converts the generated query expression into FetchXML and vice versa.
1. Use the converted query to make retrieve multiple request. 

### Clean up

Display an option to delete the records created in the [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
