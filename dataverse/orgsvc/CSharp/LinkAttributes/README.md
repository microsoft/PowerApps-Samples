# Link custom attributes between series and instances

This sample shows how to link a custom attribute that is created for a recurring appointment series (`RecurringAppointmentMaster`) with a custom attribute that is created for the appointment instances (`Appointment`).

## What this sample does

The `CreateAttributeRequest` message is intended to be used in a scenario where it contains data that is needed to create custom attributes.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Checks the current version of the org.

### Demonstrate

1. The `StringAttributeMetadata` message creates custom string attributes for recurring appointment instance and appointment instance.
2. The `LinkedAttributeId` links the custom attribute to the appointment's custom attribute.

### Clean up

Display an option to delete the records that are created in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
