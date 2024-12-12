# Retrieve currency exchange rate

This sample shows how to create a new currency, and how to retrieve and display the currency exchange rate relative to the organization’s base currency.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `RetrieveExchangeRateRequest` message is intended to be used in a scenario where it contains data that is needed to retrieve the exchange rate.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
2. The `TransactionCurrency` method creates a new currency for the sample.

### Demonstrate

The `RetrieveExchangeRateRequest` message retrieves the exchange rate against the base currency of the org.

### Clean up

Display an option to delete the sample data created  in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
