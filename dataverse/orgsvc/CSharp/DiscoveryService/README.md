
# Work with Discovery Service

This sample code shows how to use the discovery service with SDK assemblies. 

## How to run this sample

This sample will not open dialog to prompt you for connection information.

You must first set the `username` and `password` variables in the `SampleProgram.Main` method before building this sample.

## What this sample does

This sample uses the SDK Assembly [`CrmServiceClient`](https://learn.microsoft.com/dotnet/api/microsoft.xrm.tooling.connector.crmserviceclient?view=dynamics-xrmtooling-ce-9) to query the global discovery service with a user's credentials to determine which environments they can connect with.

If one or more environments are returned, the sample will prompt the user to choose one, and then use a `WhoAmIRequest` to return the `SystemUser.UserId` for that environment.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

This sample requires no special setup except that there are valid user credential username and password to use.

In SampleMethods.cs there is a `Cloud` enumeration for each of the known discovery region. Each enumeration member is decorated with a `Description` notation. All of these members except `Unknown` have the URL for the global discovery service for that cloud set as the description. 

### Demonstrate

1. Using the user credentials and the `cloud` value, the program uses the `GetAllOrganizations` static method to retrieve all known environments for the user.
1. The `GetAllOrganizations`method detects whether the `cloud` value is set to `Cloud.Unknown`. If it is set to this member, this method will select the commercial region `Cloud` enum and retrieve any environments that are found using the `GetOrganizationsForCloud` static method.

    If a specific data center is set, `GetAllOrganizations` will simply call `GetOrganizationsForCloud` with those values.

1. The `GetOrganizationsForCloud` method extracts the data center global discovery service Url from the member `Description` decoration and uses it together with the user credentials to execute the `CrmServiceClient.DiscoverGlobalOrganizations` discovery service message.

    A `System.ServiceModel.Security.SecurityAccessDeniedException` is expected when the user has no environments in a specific data center.

1. If any environments are returned by the `GetAllOrganizations` method, they will be listed in the console and you will be prompted to choose one by typing a number. If your choice is valid, the selected environment data is used to execute a `WhoAmIRequest` and return the `SystemUser.UserId` for the user in that environment.

### Clean up

This sample creates no records. No cleanup is required.
