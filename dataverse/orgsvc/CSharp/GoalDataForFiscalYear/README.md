# Rollup goal data for a fiscal period against the stretch target count

This sample shows how to roll up goal data for a fiscal period against stretch target count representing a number of completed phone calls.

This sample requires additional licensed users that may not exist in your system. Create the required users manually in **Office 365** or Azure AD (as appropriate), then adding them to your test environment, in order to run the sample without any errors. For this sample, create licensed user profiles as shown below.

**First Name**: Nancy<br/>
**Last Name**: Anderson<br/>
**Security Role**: Salesperson<br/>
**UserName**: nanderson@yourorg.onmicrosoft.com<br/>

**First Name**: David<br/>
**Last Name**: Bristol<br/>
**Security Role**: Salesperson<br/>
**UserName**: dbristol@yourorg.onmicrosoft.com<br/>

**First Name**: Kevin<br/>
**Last Name**: Cook<br/>
**Security Role**: SalesManager<br/>
**UserName**: kcook@yourorg.onmicrosoft.com<br/>

Alternately, you can substitute any licensed user profiles in your test environment as long as they are assigned the correct role indicated above. In the SystemUserProvider.cs file shared by several code samples, change the `Retrieve*()` method code that uses the above profiles to use your replacement user profiles.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

This sample shows how to roll up goal data for a fiscal period against stretch target count representing a number of completed telephone calls.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the version of the org.
2. Retrieves the sales manager and 2 sales representatives created manually in **Office 365**.
3. Creates a PhoneCall record and supporting account record for the sample.
4. Creates ActivityPartys for the phone calla "From" column.
5. Creates an open phone call.
6. Closes the first phone call and creates a second one.
7. Closes the second phone call.

### Demonstrate

1. Creates Metric, and setting the Metric type to "count" and enabling stretch tracking.
2. Creates a Rollup column which targets completed (received) phone calls.
3. The `GoalRollupQuery` creates the goal rollup queries, locating the incoming and outgoing closed phone calls.
4. Creates three goals, one parent goal and two child goals.
5. The `RecalculateRequest` calculates the rollup for goals. 

### Clean up

Display an option to delete the sample data created in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
