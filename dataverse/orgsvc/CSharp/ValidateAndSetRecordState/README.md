# Sample: Validate record state and set the state of record

This sample shows how to validate a change of state of a table and set a state of a table.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `IsValidStateTransitionRequest` message is intended to be used in a scenario where it contains the data that is needed to validate the state transition.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
2. The `CreateRequiredRecords` method creates any table records that this sample requires.

### Demonstrate

1. The `EntityReference` method creates a EntityReference to represent open case. 
2. The `IsValidStateTransitionRequest`  method sets the transition request to an open case.
3. The `checkState.NewState` property checks if a new state of resolved and a new state of problem solved are valid.
4. The `IsValidStateTransitionResponse` method executes the request.

### Clean up

Display an option to delete the sample data that is created in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.

