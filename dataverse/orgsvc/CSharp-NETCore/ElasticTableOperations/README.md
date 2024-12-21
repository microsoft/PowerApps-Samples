---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample demonstrates how to perform common data operations on elastic tables and query JSON columns using the Dataverse SDK for .NET."
---

# SDK for .NET Elastic Table Operations Sample

This .NET 6.0 sample demonstrates how to perform operations with elastic tables and query JSON columns using the Dataverse SDK for .NET.

This sample uses the [Microsoft.PowerPlatform.Dataverse.Client.ServiceClient Class](https://learn.microsoft.com/dotnet/api/microsoft.powerplatform.dataverse.client.serviceclient).

## Prerequisites

- Microsoft Visual Studio 2022
- Access to Dataverse with system administrator or system customizer privileges.

## How to run the sample

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Open the `PowerApps-Samples\dataverse\orgsvc\CSharp-NETCore\ElasticTableOperations\ElasticTableOperations.sln` file using Visual Studio 2022.
1. Edit the `appsettings.json` file. Set the connection string `Url` and `Username` parameters as appropriate for your test environment.

   The environment Url can be found in the Power Platform admin center. It has the form https://\<environment-name>.crm.dynamics.com.

1. Build the solution, and then run the desired project.

When the sample runs, you will be prompted in the default browser to select an environment user account and enter a password. To avoid having to do this every time you run a sample, insert a password parameter into the connection string in the appsettings.json file. For example:

```json
{
"ConnectionStrings": {
    "default": "AuthType=OAuth;Url=https://myorg.crm.dynamics.com;Username=someone@myorg.onmicrosoft.com;Password=mypassword;RedirectUri=http://localhost;AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;LoginPrompt=Auto"
  }
}
```

>**Tip**: You can set a user environment variable named `DATAVERSE_APPSETTINGS` to the file path of the appsettings.json file stored anywhere on your computer. The samples will use that appsettings file if the environment variable exists and is not null. Be sure to log out and back in again after you define the variable for it to take affect. To set an environment variable, go to **Settings > System > About**, select **Advanced system settings**, and then choose **Environment variables**.

## Sample Output

The output of the sample should look something like this:

```
Starting Elastic table operations sample.

=== Start Region 0: Creating contoso_SensorData table ===

Creating contoso_SensorData table...
        contoso_SensorData table created.
Creating contoso_DeviceId column...
        contoso_DeviceId column created.
Creating contoso_Value column...
        contoso_Value column created.
Creating contoso_TimeStamp column...
        contoso_TimeStamp column created.
Creating contoso_EnergyConsumption column...
        contoso_EnergyConsumption column created.

=== Start Region 1: Examples without partitionid ===

Created record with id: 13d827fe-a8fd-ed11-8f6e-000d3a993550
Updated the record with id: 13d827fe-a8fd-ed11-8f6e-000d3a993550
Upserted the record with id: 13d827fe-a8fd-ed11-8f6e-000d3a993550
Record created with upsert?:False
Retrieved the record with id: 13d827fe-a8fd-ed11-8f6e-000d3a993550
Data:
        contoso_sensordataid: 13d827fe-a8fd-ed11-8f6e-000d3a993550
        contoso_deviceid: Device-ABC-1234
        contoso_sensortype: Humidity
        contoso_value: 30
        contoso_timestamp: 5/28/2023 10:43:03 PM
        ttlinseconds: 86400
        contoso_energyconsumption {"power":4,"powerUnit":"Watts","voltage":3,"voltageUnit":"Volts"}

Deleted the record with id: 13d827fe-a8fd-ed11-8f6e-000d3a993550

=== Start Region 2: Examples with partitionid ===

Creating record...
        Record created.
Updating record...
        Record updated.
Updating record with alternate key...
        Record updated with alternate key.
Retrieving record...
        Record retrieved.
        contoso_value: 80
Retrieving record with alternate key...
        Retrieved record with alternate key.
        contoso_value: 80
Upserting  record...
        Upserted record.
Deleting record...
        Record deleted.
Deleting record with alternate key...
        Record deleted with alternate key.

=== Start Region 3: CreateMultiple and UpdateMultiple Examples ===

Creating 1000 records using CreateMultiple
In batches of 100
        Created 1000 records.

Updating 1000 records using UpdateMultiple
In batches of 100
        Updated 1000 records.

=== Start Region 4: ExecuteCosmosSqlQuery Examples ===

Requesting ExecuteCosmosSqlQuery..

ExecuteCosmosSqlQueryResponse.PagingCookie: [Truncated for brevity]

ExecuteCosmosSqlQueryResponse.HasMore: True

Output first page of 100 results:

Returned initial 100 results from ExecuteCosmosSqlQuery:
        Device-ABC-1234 51
     ...
        [Truncated for brevity]

Returned 100 more results from ExecuteCosmosSqlQuery.
        Device-ABC-1234 151
        Device-ABC-1234 152
...
[Truncated for brevity]

Returned total of 951 results using ExecuteCosmosSqlQuery

=== Start Region 5: DeleteMultiple Example ===

Deleteing 1000 records using DeleteMultiple
In batches of 100
Deleted 1000 records.

=== Start Region 6: Delete contoso_SensorData table ===

Deleting contoso_sensordata table...
        contoso_sensordata table deleted.

=== SDK ElasticTableOperations Sample Completed ===

```

## Demonstrates

The code for this sample is in the `Program.cs` file.

This project depends on data in the following classes:

|Class|Description|
|---|---|
|`Utility.cs`|Contains the `CreateSensorDataEntity` and `DeleteSensorDataEntity` static methods to create the elastic table used by this sample. |
|`Settings.cs`|Contains `NumberOfRecords` and `BatchSize` values you can change. Default values are: `NumberOfRecords` = 1000, `BatchSize` = 100.|
|`EnergyConsumption.cs`|Describes the data stored as JSON in the `contoso_EnergyConsumption` column.|
|`ExecuteCosmosSqlQueryResponse.cs`|A class that inherits from [Entity](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.entity) that provides access to the properties of the return value of the `ExecuteCosmosSqlQuery` message. More information: [Custom closed types](https://learn.microsoft.com/power-apps/developer/data-platform/use-open-types?tabs=sdk#custom-closed-types)|

The sample has the following sections:

- [Region 0: Creating contoso_SensorData table](#region-0-creating-contoso_sensordata-table)
- [Region 1: Create, Retrieve, Update, Upsert, and Delete examples without partitionid](#region-1-create-retrieve-update-upsert-and-delete-examples-without-partitionid)
- [Region 2: Create, Retrieve, Update, Upsert, and Delete examples with partitionid](#region-2-create-retrieve-update-upsert-and-delete-examples-with-partitionid)
- [Region 3: CreateMultiple and UpdateMultiple Examples](#region-3-createmultiple-and-updatemultiple-examples)
- [Region 4: ExecuteCosmosSqlQuery Example](#region-4-executecosmossqlquery-example)
- [Region 5: DeleteMultiple Example](#region-5-deletemultiple-example)
- [Region 6: Delete contoso_SensorData table](#region-6-delete-contoso_sensordata-table)

### Region 0: Creating contoso_SensorData table

Uses `Utility.CreateSensorDataEntity` function to create an elastic table named `contoso_SensorData` with the following columns:

|SchemaName|Type|Description|
|---|---|---|
|`contoso_SensorType`|String|The primary name field for the `contoso_SensorData` elastic table.|
|`contoso_DeviceId`|String|Stores a valued for a device. This value is used as the `partitionid`.|
|`contoso_Value`|Integer|Stores a sensor reading value.|
|`contoso_TimeStamp`|DateTime|Stores a timestamp value for the sensor reading.|
|`contoso_EnergyConsumption`|String|This column uses JSON format. It stores JSON data related to energy consumption.|

### Region 1: Create, Retrieve, Update, Upsert, and Delete examples without partitionid

If you do not choose to apply a partitioning strategy for your elastic table, you can perform operations much like you do for standard tables. There are two significant differences: [Managing the session token](#managing-the-session-token) and [Upsert replace behavior](#upsert-replace-behavior).

#### Managing the session token

To have strong consistency on retrieve operations you need to send the current session token. Each write operation returns the `x-ms-session-token`, so this value is refreshed on each request so it can be sent with any requests to retrieve data. More information: [Work with session token](https://learn.microsoft.com/power-apps/developer/data-platform/use-elastic-tables?tabs=sdk#work-with-session-token)

**Note**: Because of the need to capture and send this session token value, you can only use the [IOrganizationService.Execute method](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.execute) to send requests. You cannot use the other [IOrganizationService methods](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/use-messages?tabs=sdk#iorganizationservice-methods) because they don't currently enable managing the session token values.

#### Upsert replace behavior

When performing an upsert operation on an elastic table, you must include all properties for a record that may be updated. If a matching record is found, all the data for that record will be replace by the data you are sending with your Upsert. More information: [Upsert a record in an elastic table](https://learn.microsoft.com/power-apps/developer/data-platform/use-elastic-tables?tabs=sdk#upsert-a-record-in-an-elastic-table)

### Region 2: Create, Retrieve, Update, Upsert, and Delete examples with partitionid

If you choose to apply a partitioning strategy on the elastic table you must always use the `partitionid` value together with the primary key to uniquely identify any record. In these examples using the `contoso_SensorData` table, the `contoso_DeviceId` value is being used for the `partitionid`. More information: [Partitioning and horizontal scaling](https://learn.microsoft.com/power-apps/developer/data-platform/elastic-tables#partitioning-and-horizontal-scaling)

To include this `partitionid`, you have three options:

1. **Use the alternate key style** with the values for the `KeyForNoSqlEntityWithPKPartitionId` alternate key created for all elastic tables. More information: [Using Alternate Key](https://learn.microsoft.com/power-apps/developer/data-platform/use-elastic-tables?tabs=sdk#using-alternate-key)
1. **Use the `partitionId` parameter**. This [optional parameter](https://learn.microsoft.com/power-apps/developer/data-platform/optional-parameters?tabs=sdk) can be used with classes that are derived from [OrganizationRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.organizationrequest).  More information: [Using partitionId parameter](https://learn.microsoft.com/power-apps/developer/data-platform/use-elastic-tables?tabs=sdk#using-partitionid-parameter)
1. **Using `partitionid` column directly**. When you create a record, you must explicitly set the value of the `partitionid` column. For Update and Upsert operations you can't currently use the `partitionId` parameter, so you must either use the alternate key style, or you can set the value of the `partitionid` column. More information [Using partitionid column directly](https://learn.microsoft.com/power-apps/developer/data-platform/use-elastic-tables?tabs=sdk#using-partitionid-column-directly)

**Note**: The examples in this section use static methods such as `CreateRecord`, `UpdateRecord`, and `UpdateRecordWithAlternateKey` to make the different patterns of working with `partitionid` clear for each operation.

### Region 3: CreateMultiple and UpdateMultiple Examples

This section demonstrates the use of the SDK [CreateMultipleRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.createmultiplerequest) and [UpdateMultipleRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.updatemultiplerequest) classes.

With elastic tables, we recommend sending requests in batches of 100. This is different from the recommended practice for standard tables described in [Use CreateMultiple and UpdateMultiple (preview)](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/use-createmultiple-updatemultiple?tabs=sdk).

**Note**: You can change the values in the `Settings.cs` file to adjust the number of records created and the batch size.

### Region 4: ExecuteCosmosSqlQuery Example

This section demonstrates composing a CosmosDB SQL query and sending it using the `ExecuteCosmosSqlQuery` message. There is currently no `ExecuteCosmosSqlQueryRequest` class in the SDK, so this example shows using the [OrganizationRequest Class](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.organizationrequest).

The `ExecuteCosmosSqlQuery` message has the following parameters:

|Name|Type|Description|
|---------|---------|---------|
|`QueryText`|string|(Required) Cosmos sql query.|
|`EntityLogicalName`|string|(Required) The logical name of the table.|
|`QueryParameters`|[ParameterCollection](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.parametercollection)|(Optional) Values for any parameters that are specified in the QueryText parameter.|
|`PageSize`|Long|(Optional) Number of records returned in a single page.|
|`PagingCookie`|string|(Optional) Paging cookie to be used.|
|`PartitionId`|string|(Optional) Partitionid to set the scope of the query.|

The `ExecuteCosmosSqlQuery` message returns an [Entity](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.entity) that is an open type. To access the values returned, this sample uses a `ExecuteCosmosSqlQueryResponse` class that inherits from `Entity`. More information: [Custom closed types](https://learn.microsoft.com/power-apps/developer/data-platform/use-open-types?tabs=sdk#custom-closed-types)

The `ExecuteCosmosSqlQueryResponse` class includes the values that are returned:

|Name|Type|Description|
|---------|---------|---------|
|`PagingCookie`|string| A value to set for subsequent requests when there are more results.|
|`HasMore`|bool|Whether there are more records in the results.|
|`Result`|string|JSON with values with the results.|

This example specifies the query and demonstrates how to manage retrieving paged results using the `HasMore` and `PagingCookie` values returned.

### Region 5: DeleteMultiple Example

This section demonstrates using the `DeleteMultiple` message with the [OrganizationRequest Class](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.organizationrequest). The SDK doesn't currently have `DeleteMultipleRequest` class.

As was done with `CreateMultiple` and `UpdateMultiple`, the records are deleted in batches of 100.

### Region 6: Delete contoso_SensorData table

The `Utility.DeleteSensorDataEntity` deletes the `contoso_SensorData` table using the [DeleteEntityRequest Class](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.deleteentityrequest).

### Clean up

There is no data to clean up because the `contoso_SensorData` table is deleted.
