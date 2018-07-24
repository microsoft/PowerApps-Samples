# Enable field security for an entity
This sample shows how to enable field security for an entity.

[!IMPORTANT]
> Before running the sample, create the required users manually. For this sample create user profile as is as shown below. 
> First Name: Samantha
> Last Name: Smith
> Security Role: Marketing Manager
> UserName: ssmith@yourorg.com

## How to run this sample

See [How to run samples](../../../How-to-run-samples.md) for information about how to run this sample.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Get the user that you have created manually in office 365.
2. Retrieve the security role needed to assign to the user. 
3. Retrieve the default business unit needed to create the team.
4. Instantiate a team entity record and set its property values. 


### Demonstrate

1. Creates firld security profile and create the request object and set the monikers with the teamprofiles_assocation relationship.
2. Creates custom activity entity and attributes using the `CreateEntityRequest` and `CreateAttributeRequest` mesaage.
3. Create the field permission for the identity attribute.

### Clean up

1. Display an option to delete the records in [Setup](#setup).

    The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
