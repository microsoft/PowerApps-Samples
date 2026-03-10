# Sample: Use aggregation in FetchXML

This sample shows how to retrieve aggregate record data using FetchXML.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `FetchXML` query is intended to be used in a scenario where it creates queries to get the data.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
1. The `CreateRequiredRecords` class creates 3 opportunity records and account record.

### Demonstrate

1. The `estimatedvalue_avg` fetches the average of estimatedvalue for all the opportunities. The `EntityCollection` method returns the results of the `RetrieveMultiple` request.
1. The `opportunity_count` fetches the count of all opportunities.
1. The `estimatedvalue_max` fetches the maximum estimatedvalue of all opportunities.
1. The `estimatedvalue_min` fetches the minimum estimatedvalue of all opportunities.
1. The `estimatedvalue_sum` fetches the sum of estimatedvalue for all opportunities.
1. The `estimatedvalue_avg2` fetches the multiple aggregate values within a single query.
1. The `groupby1` fetches a list of users with a count of all the opportunities they own using the groupby.
1. The `byyear` fetches the aggregate information about all the opportunities that have been won by year.
1. The `byquarter` fetches the aggregate information about the opportunities that have been won by quarter.
1. The `bymonth` fetches the aggregate information about the opportunities that have been won by month.
1. The `byweek` fetches the aggregate information about the opportunities that have been won by week.
1. The `byday` fetches the aggregate information about the opportunities that have been won by day.
1. The `byyrqtr` fetches the aggregate information about the opportunities that have been won by year and quarter.
1. The `byyrqtr2` specifies the result order. 

### Clean up

Displays an option to delete all the data created in the sample. The deletion is optional in case you want to examine the data created by the sample. You can manually delete the data to achieve same results.
