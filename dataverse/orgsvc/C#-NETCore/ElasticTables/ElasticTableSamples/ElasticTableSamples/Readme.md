# Elastic table samples

This sample provides the basic operations that can be performed on Elastic tables.

## Prerequisites

- Access to Dataverse with CreateEntity, CreateAttribute and DeleteEntity privileges.

## What this sample does

- Creates an Elastic entity contoso_SensorData.
- Creates the attributes contoso_DeviceId, contoso_Value, contoso_TimeStamp and contoso_EnergyConsumption.
- Creates a record of contoso_SensorData entity.
- Updates the above record with partitionid in the payload.
- Updates the above record with the alternate key.
- Retrieves the above record with partitionId as parameter.
- Retrieves the above record with alternate key.
- Upserts the record.
- Deletes the above record with partitionid in the parameter.
- Creates another record.
- Deletes the above record with alternate key.
- Creates 100 records using CreateMultiple.
- Uses ExecuteCosmosSqlQuery to retrieve some records using a condition.
- Updates the above created records using UpdateMultiple.
- Deletes the records using DeleteMultiple.
- Deletes the contoso_SensorData entity.
