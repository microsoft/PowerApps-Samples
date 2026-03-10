
# Sample: Custom workflow activities

These samples show how to write custom workflow activities to perform a number of different operations.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for general information about how to run this sample.

All of the custom activities are compiled into a single assembly which you may register with your environment. After registering the assembly, the custom activities will be available for use when adding steps in a custom workflow.

## What this sample does

The activities provided here as samples are:

- **RetrieveCreditScore** - Calculates the credit score based on a Social Security Number (SSN) and name.
- **SimpleSdkActivity**, **SdkWithLooselyTypesActivity**, **SimpleSdkWithRelatedEntitiesActivity** - Create an account and a task for the account.
- **UpdateNextBirthday** - Returns the next birthday. Use this in a workflow to send a birthday greeting to a customer.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

You must customize your environment as stated below for the indicated custom activities to function properly.

**UpdateNextBirthday**  
A custom column named *Contact*.new_nextbirthday must exist for this custom workflow activity to work.

**RetrieveCreditScore**  
The following customizations must exist for this custom workflow activity to work:

Custom table schema name: *new_loanapplication*  
Column: new_loanapplicationid as the primary key  
Column: new_creditscore of type int with min of 0 and max of 1000 (if it is to be updated)  
Column: new_loanamount of type money with default min/max  
Customize the form to include the column new_loanapplicantid  

The *contact* table must have the following customizations:  
Column: new_ssn as Single Line of Text with max length of 15  
One-To-Many Relationship with these properties:  
Relationship Definition Schema Name: new_loanapplicant  
Relationship Definition Related table Display Name: Loan Application  
Relationship column Schema Name: new_loanapplicantid  
Relationship Behavior Type: Referential

### See Also

[Workflow extensions](https://learn.microsoft.com/powerapps/developer/common-data-service/workflow/workflow-extensions)  
[Tutorial: Create workflow extension](https://learn.microsoft.com/powerapps/developer/common-data-service/workflow/tutorial-create-workflow-extension)  
[Create a custom table](https://learn.microsoft.com/powerapps/maker/common-data-service/data-platform-create-entity)
