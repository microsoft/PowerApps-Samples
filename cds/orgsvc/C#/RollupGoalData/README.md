# Rollup goal data for a custom period against the target avenue

This sample shows how to roll up goal data for a custom period against the target revenue.

This sample requires additional three users that are not in your system. Create the three required users **as is** shown below manually in **Office 365**.

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

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/cds/README.md) for information about how to run this sample.

## What this sample does

This sample shows how to roll up goal data for a custom period against the target revenue.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the version of the org.
2. Retrieves the sales manager and 2 sales representatives created manually in **Office 365**.
3. Creates a sample unit group and retrieve the default unit id.
4. Creates few products and new discount list.
5. The `PriceLevel` creates the price list.
6. The `ProductPriceLevel` creates a price list item for the first product and apply volume discount.
7. Creates account record for the opportunity's potential customer id.
8. Creates a new opportunity with user specified estimated revenue.
9. Creates a catalog products and override the list price.
10. Creates a new write-in opportunity product with a manual discount applied.

### Demonstrate

1. Creates Metric, and setting the Amount Data type to `Money`.
2. Creates a Rollup fields which targets the estimated values and actual values.
3. The `GoalRollupQuery` creates the goal rollup queries, locating the opportunities in the sales representative's area.
4. Creates three goals, one parent goal and two child goals.
5. The `RecalculateRequest` calculates the rollup for goals.

### Clean up

Display an option to delete the sample data created in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
