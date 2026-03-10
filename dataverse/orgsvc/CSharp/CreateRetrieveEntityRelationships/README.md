
# Create and retrieve table relationships

This sample shows how to create and retrieve table relationships. The following methods are used to create and retrieve the relationships:

- [CreateOneToManyRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.createonetomanyrequest)
- [CreateManyToManyRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.createmanytomanyrequest)
- [CanBeReferencedRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.canbereferencedrequest)
- [CanBeReferencingRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.canbereferencingrequest)
- [CanManyToManyRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.canmanytomanyrequest)
- [RetrieveRelationshipRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.retrieverelationshiprequest)

## How to run this sample

See [How to run this sample](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `CreateOneToManyRequest`, `CreateManyToManyRequest`, `CanManyToManyRequest`, `CreateOneToManyRequest`, `CanBeReferencedRequest`, `CanBeReferencingRequest`, and `RetrieveRelationshipRequest` messages are intended to be used in a scenario where it contains the data that is needed to create and retrieve table relationships.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Checks for the current version of the org.

### Demonstrate

1. The `CreateOneToManyRequest` method creates a new One-to-Many (1:N) relationship. 
2. The `CreateManyToManyRequest` method creates a new Many-To-Many (N:N) relationship.
3. The `EligibleCreateManyToManyRelationship` method verifies whether entities can participate in N:N relationship.
4. The `RetrieveRelationshipRequest` method retrieves the two table relationships previously created.


### Clean up

Display an option to delete the records created in the [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
