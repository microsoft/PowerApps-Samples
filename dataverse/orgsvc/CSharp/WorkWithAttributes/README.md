# Work with attribute metadata

This sample shows how to perform various actions on attributes.

## How to run this sample

See [How to run this sample](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

This sample shows how to create different types of attributes in Microsoft Dataverse.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Checks for the current version of the environment.

### Demonstrates

1. Create columns

   This sample creates the following types of columns using the CreateAttributeRequest class:
  
   - Yes/No (BooleanAttributeMetadata)
   - Date Time (DateTimeAttributeMetadata)
   - Decimal (DecimalAttributeMetadata)
   - Whole Number (IntegerAttributeMetadata)
   - Memo (MemoAttributeMetadata)
   - Money (MoneyAttributeMetadata)
   - Choice (PicklistAttributeMetadata)
   - String (StringAttributeMetadata)
   - Multi-Select Choice (MultiSelectPicklistAttributeMetadata)
   - Big Integer (BigIntAttributeMetadata)

1. Add a status value (InsertStatusValueRequest)
1. Retrieve a column (RetrieveAttributeRequest)
1. Update a column (UpdateAttributeRequest)
1. Update the label for a state column option (UpdateStateValueRequest)
1. Add a new option to a choice column (InsertOptionValueRequest)
1. Change the order of options in a choice column (OrderOptionRequest)
1. Delete columns (DeleteAttributeRequest)



### Clean up

Displays an option to delete all the data created in the sample. The deletion is optional in case you want to examine the data created by the sample. You can manually delete the data to achieve same results.