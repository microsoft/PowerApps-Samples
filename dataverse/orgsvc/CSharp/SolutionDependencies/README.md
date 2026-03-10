# Detect solution dependencies

This sample shows how to detect dependencies before you delete a solution component.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `RetrieveDependentComponentsRequest`, `RetrieveDependenciesForDeleteRequest` messages are intended to be used in a scenario where it contains data to detect solution dependencies.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
1. The `Publisher` method creates the sample publisher that will `own` the two solutions.
1. The `Solution` method creates the primary solution.
1. The `OptionSetMetadata` creates the global option set and associates it to the solution.
1. The `ExportSolutionRequest` exports the solution as managed so that we can later import it.
1. The `DeleteOptionSetRequest` deletes the option set previously created, so it can be imported under the managed solution.
1. The `ImportSolutionRequest` re-imports the solution as managed.

### Demonstrate

1. The `QueryByAttribute` queries all solution components for a solution.
1. The `RetrieveDependentComponentsRequest` retrieves all the dependencies for the component. If there are no dependencies we can ignore this component. If there are dependencies upon this solution component, and the solution itself is managed, then you will be unable to delete the solution.

### Clean up

Display an option to delete the solutions created in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
