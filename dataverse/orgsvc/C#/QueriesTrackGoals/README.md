# Use rollup queries to track goals

This sample shows how to use rollup queries to track goals.

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

This sample shows how to yse rollup queries to track goals.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the version of the org.
2. Retrieves the sales manager and 2 sales representatives created manually in **Office 365**.
3. Creates records to support `SalesOrder` records.
4. Creates a new unit group for the sample.
5. Retrieves the default unit id that is automatically created when we created a new unit group.
6. The `Product` creates few products that are required for the sample.
7. The `PriceLevel` creates anew price list.
8. The `ProductPriceLevel` creates a price list item for the first product and applies volume discount.
9. Creates an account record for the sales order's potential customer id.
10. The `SalesOrderDetails` adds the product to the order with the price overridden with a negative value.

### Demonstrate

1. Creates Metric, and setting the Metric type to `Amount` and setting amount data type to `Money`.
2. The `RollupField` creates a Rollup field which targets the actual totals.
3. The `GoalRollupQuery` creates the goal rollup queries, locating the sales orders in the first sales representative's area (zip code: 60661) and with a value greater than $1000. 
4. Creates two goals, one parent goal and one child goals.
5. The `RecalculateRequest` calculates the rollup for goals.

### Clean up

Display an option to delete the sample data created in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
