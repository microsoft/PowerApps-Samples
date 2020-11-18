# Work with attribute metadata

This sample shows how to perform various actions on attributes.

## How to run this sample

See [How to run this sample](https://github.com/microsoft/PowerApps-Samples/blob/master/cds/README.md) for information about how to run this sample.

## What this sample does

This sample shows how to create different types of attributes in Microsoft Dataverse.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Checks for the current version of the org.

### Demonstrate

1. The `BooleanAttributeMetadata` method creates an attribute of type boolean.
2. The `DateTimeAttributeMetadata` message creates an attribute of type date time.
3. The `DecimalAttributeMetadata` message creates an attribute of type decimal.
4. The `IntegerAttributeMetadata` message creates an attribute of type integer.
5. The `MemoAttributeMetadata` message creates an attribute of type memo.
6. The `MoneyAttributeMetadata` message creates an attribute of type money.
7. The `PicklistAttributeMetadata` message creates an attribute of type picklist.

### Clean up

Displays an option to delete all the data created in the sample. The deletion is optional in case you want to examine the data created by the sample. You can manually delete the data to achieve same results.