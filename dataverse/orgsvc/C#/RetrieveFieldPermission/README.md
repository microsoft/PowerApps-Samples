# Sample: Retrieve field permissions

This sample shows how to retrieve secured columns for a user according to the steps outlined in [Field security entities](https://learn.microsoft.com/dynamics365/customer-engagement/developer/field-security-entities).

This sample requires additional users that are not in your system. Create the required users manually in **Office 365** in order to run the sample without any errors. For this sample create a user profile **as is** shown below. 

**First Name**: Samantha <br/>
**Last Name**: Smith<br/>
**Security Role**: Marketing Manager<br/>
**UserName**: ssmith@yourorg.onmicrosoft.com<br/>

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `FieldPermission` class is intended to be used in a scenario where it contains the data that defines the possible field permission types.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
1. Gets the user information that you have created manually in **Office 365**.
1. The `QueryExpression` method retrieves the security role needed to assign to the user.
1. The `Team` method instantiate a team table record and set its property values.

### Demonstrate

1. The `FieldSecurityProfile` method creates field security profile.
1. The `AssociateRequest` method adds team and user to the profile.
1. The `CreateEntityRequest` method creates a new custom activity table for the sample.
1. The `RolePrivilege` method adds privileges for the new custom table.
1. The `AddPrivilegeRoleRequest` method creates and execute the `RolePrivilege` method.
1. The `FieldPermission` method creates field permission object for identity.

### Clean up

Display an option to delete the records created in the [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
