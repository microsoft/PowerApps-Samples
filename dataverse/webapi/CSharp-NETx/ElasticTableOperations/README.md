---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample demonstrates how to perform common data operations on elastic tables and query JSON columns using the Dataverse Web API."
---

# Web API Elastic Table Operations sample

This .NET 6.0 sample demonstrates how to perform common data operations on elastic tables query JSON columns using the Dataverse Web API.

This sample uses the common helper code in the [WebAPIService](../WebAPIService) class library project.

## Prerequisites

- Microsoft Visual Studio 2022
- Access to Dataverse with System Administrator privileges to create tables and perform data operations

## How to run the sample

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Open the `PowerApps-Samples\dataverse\webapi\CSharp-NETx\ElasticTableOperations\ElasticTableOperations.sln` file using Visual Studio 2022.
1. Edit the `appsettings.json` file to set the following property values.

   | Property | Instructions |
   |----------|--------------|
   | `Url` | The URL for your environment. Replace the placeholder `https://yourorg.api.crm.dynamics.com` value with the value for your environment. Learn how to find your URL in [View developer resources](https://learn.microsoft.com/power-apps/developer/data-platform/view-download-developer-resources). |
   | `UserPrincipalName` | Replace the placeholder `you@yourorg.onmicrosoft.com` value with the UPN value you use to access the environment. |
   | `Password` | Replace the placeholder `yourPassword` value with the password you use. |

1. Save the `appsettings.json` file.
1. Build the solution.
1. Press `F5` to run the sample.

## Sample Output

The output of the sample should look something like this:

```
Starting Elastic table operations sample.

=== Start Region 0: Creating contoso_SensorData table ===

Creating the contoso_SensorData table...
contoso_SensorData table created.

=== Start Region 1: Create Record ===

Created sensor data record at:contoso_sensordatas(contoso_sensordataid=7fef7745-fbea-ef11-9341-0022482b040d,partitionid='Device-ABC-1234')

=== Start Region 2: Update Record ===

Updated sensor data record using partitionId parameter.
Updated sensor data record using alternate key style.

=== Start Region 3: Retrieve Record ===

Retrieved sensor data record using partitionId:

{
  "@odata.context": "https://crmue.api.crm.dynamics.com/api/data/v9.2/$metadata#contoso_sensordatas(contoso_value)/$entity",
  "@odata.etag": "W/\"e1015c6f-0000-0200-0000-67af80e80000\"",
  "contoso_value": 80,
  "contoso_sensordataid": "7fef7745-fbea-ef11-9341-0022482b040d",
  "versionnumber": 638751518483842799,
  "partitionid": "Device-ABC-1234",
  "_ownerid_value": "4026be43-6b69-e111-8f65-78e7d1620f5e",
  "_owningbusinessunit_value": "38e0dbe4-131b-e111-ba7e-78e7d1620f5e"
}

Retrieved sensor data record using alternate key style:

{
  "@odata.context": "https://crmue.api.crm.dynamics.com/api/data/v9.2/$metadata#contoso_sensordatas(contoso_value)/$entity",
  "@odata.etag": "W/\"e1015c6f-0000-0200-0000-67af80e80000\"",
  "contoso_value": 80,
  "contoso_sensordataid": "7fef7745-fbea-ef11-9341-0022482b040d",
  "versionnumber": 638751518483842799,
  "partitionid": "Device-ABC-1234",
  "_ownerid_value": "4026be43-6b69-e111-8f65-78e7d1620f5e",
  "_owningbusinessunit_value": "38e0dbe4-131b-e111-ba7e-78e7d1620f5e"
}

=== Start Region 4: Upsert Record ===

Upserted sensor data record at contoso_sensordatas(contoso_sensordataid=7fef7745-fbea-ef11-9341-0022482b040d,partitionid='Device-ABC-1234')

=== Start Region 5: Delete Record ===

Deleted sensor data record with partitionId.


=== Start Region 6: Demonstrate CreateMultiple ===

Creating 1000 records to use for query example...
        100 records created using CreateMultiple
        100 records created using CreateMultiple
        100 records created using CreateMultiple
        100 records created using CreateMultiple
        100 records created using CreateMultiple
        100 records created using CreateMultiple
        100 records created using CreateMultiple
        100 records created using CreateMultiple
        100 records created using CreateMultiple
        100 records created using CreateMultiple
1000 records to use for query example created.

=== Start Region 7: Demonstrate UpdateMultiple ===

        100 records updated using UpdateMultiple
        100 records updated using UpdateMultiple
        100 records updated using UpdateMultiple
        100 records updated using UpdateMultiple
        100 records updated using UpdateMultiple
        100 records updated using UpdateMultiple
        100 records updated using UpdateMultiple
        100 records updated using UpdateMultiple
        100 records updated using UpdateMultiple
        100 records updated using UpdateMultiple
1000 records updated using UpdateMultiple.

=== Start Region 8: Demonstrate ExecuteCosmosSqlQuery ===

ExecuteCosmosSqlQueryResponse.PagingCookie:  [removed for brevity]

ExecuteCosmosSqlQueryResponse.HasMore: True

Output first page of 50 results:

        Device-ABC-1234 6
        [removed for brevity]

Output additional page of 50 results:

[removed for brevity]

=== Start Region 9: Demonstrate DeleteMultiple ===

        100 records deleted using DeleteMultiple
        100 records deleted using DeleteMultiple
        100 records deleted using DeleteMultiple
        100 records deleted using DeleteMultiple
        100 records deleted using DeleteMultiple
        100 records deleted using DeleteMultiple
        100 records deleted using DeleteMultiple
        100 records deleted using DeleteMultiple
        100 records deleted using DeleteMultiple
        100 records deleted using DeleteMultiple
1000 records deleted using DeleteMultiple.

=== Start Region 10: Delete Table ===


Deleting the contoso_SensorData table...
contoso_SensorData table deleted.

=== Web API ElasticTableOperations Sample Completed ===
```

## Demonstrates

The code for this sample is in the `Program.cs` file.

In addition to the WebAPIService project classes, this project depends on data in the following classes.

| Class | Description |
|-------|-------------|
| `Settings.cs` | Contains `NumberOfRecords` and `BatchSize` values you can change. Default values are: `NumberOfRecords` = 1000, `BatchSize` = 100. |
| `EnergyConsumption.cs` | Describes the data stored as JSON in the `contoso_EnergyConsumption` column. |
| `WebAPIService\Messages\ExecuteCosmosSqlQueryRequest.cs` | Contains the data to send the `ExecuteCosmosSqlQuery` message |
| `WebAPIService\Messages\ExecuteCosmosSqlQueryResponse.cs` | Contains the data from the ExecuteCosmosSqlQueryRequest |

This sample has 10 regions:

- [Create Elastic table](#create-elastic-table)
- [Create Record](#create-record)
- [Update Record](#update-record)
- [Upsert Record](#upsert-record)
- [Delete Record](#delete-record)
- [Demonstrate CreateMultiple](#demonstrate-createmultiple)
- [Demonstrate UpdateMultiple](#demonstrate-updatemultiple)
- [Demonstrate ExecuteCosmosSqlQuery](#demonstrate-executecosmossqlquery)
- [Demonstrate DeleteMultiple](#demonstrate-deletemultiple)
- [Delete Table](#delete-table)

### Create Elastic table

The code in this region sends this request to create a user-owned elastic table named `contoso_SensorData` with the following columns:

| Schema Name | Type | Description |
|-------------|------|-------------|
| `contoso_SensorType` | String | The primary name column for the `contoso_SensorData` table. |
| `contoso_DeviceId` | String | The ID of the device. Also used as the partitionid value. |
| `contoso_Value` | Integer | The value of the sensor data. |
| `contoso_TimeStamp` | DateTime | The time of the reading. |
| `contoso_EnergyConsumption` | String | A string column using JSON format to demonstrate setting and querying JSON data using `ExecuteCosmosSqlQuery` function. |

The `TableType` property set to 'Elastic' makes this an elastic table.

#### Elastic table - Request

```http
POST [Organization Uri]/api/data/v9.2/EntityDefinitions
Consistency: Strong
OData-MaxVersion: 4.0
OData-Version: 4.0
If-None-Match: null
Accept: application/json
Authorization: Bearer <access token>
Content-Type: application/json; charset=utf-8
Content-Length: 6756

{
  "@odata.type": "Microsoft.Dynamics.CRM.EntityMetadata",
  "CanCreateCharts": {
    "@odata.type": "Microsoft.Dynamics.CRM.BooleanManagedProperty",
    "Value": false,
    "CanBeChanged": false
  },
  "Description": {
    "@odata.type": "Microsoft.Dynamics.CRM.Label",
    "LocalizedLabels": [
      {
        "@odata.type": "Microsoft.Dynamics.CRM.LocalizedLabel",
        "Label": "Stores IoT data emitted from devices",
        "LanguageCode": 1033,
        "IsManaged": false
      }
    ],
    "UserLocalizedLabel": {
      "@odata.type": "Microsoft.Dynamics.CRM.LocalizedLabel",
      "Label": "Stores IoT data emitted from devices",
      "LanguageCode": 1033,
      "IsManaged": false
    }
  },
  "DisplayCollectionName": {
    "@odata.type": "Microsoft.Dynamics.CRM.Label",
    "LocalizedLabels": [
      {
        "@odata.type": "Microsoft.Dynamics.CRM.LocalizedLabel",
        "Label": "Sensor Data",
        "LanguageCode": 1033,
        "IsManaged": false
      }
    ],
    "UserLocalizedLabel": {
      "@odata.type": "Microsoft.Dynamics.CRM.LocalizedLabel",
      "Label": "Sensor Data",
      "LanguageCode": 1033,
      "IsManaged": false
    }
  },
  "DisplayName": {
    "@odata.type": "Microsoft.Dynamics.CRM.Label",
    "LocalizedLabels": [
      {
        "@odata.type": "Microsoft.Dynamics.CRM.LocalizedLabel",
        "Label": "Sensor Data",
        "LanguageCode": 1033,
        "IsManaged": false
      }
    ],
    "UserLocalizedLabel": {
      "@odata.type": "Microsoft.Dynamics.CRM.LocalizedLabel",
      "Label": "Sensor Data",
      "LanguageCode": 1033,
      "IsManaged": false
    }
  },
  "EntitySetName": "contoso_sensordatas",
  "HasActivities": false,
  "HasNotes": false,
  "IsActivity": false,
  "OwnershipType": "UserOwned",
  "SchemaName": "contoso_SensorData",
  "TableType": "Elastic",
  "Attributes": [
    {
      "@odata.type": "Microsoft.Dynamics.CRM.StringAttributeMetadata",
      "AttributeType": "String",
      "AttributeTypeName": {
        "Value": "StringType"
      },
      "MaxLength": 100,
      "DisplayName": {
        "@odata.type": "Microsoft.Dynamics.CRM.Label",
        "LocalizedLabels": [
          {
            "@odata.type": "Microsoft.Dynamics.CRM.LocalizedLabel",
            "Label": "Sensor Type",
            "LanguageCode": 1033,
            "IsManaged": false
          }
        ],
        "UserLocalizedLabel": {
          "@odata.type": "Microsoft.Dynamics.CRM.LocalizedLabel",
          "Label": "Sensor Type",
          "LanguageCode": 1033,
          "IsManaged": false
        }
      },
      "IsPrimaryName": true,
      "SchemaName": "contoso_SensorType"
    },
    {
      "@odata.type": "Microsoft.Dynamics.CRM.StringAttributeMetadata",
      "AttributeType": "String",
      "AttributeTypeName": {
        "Value": "StringType"
      },
      "MaxLength": 1000,
      "DisplayName": {
        "@odata.type": "Microsoft.Dynamics.CRM.Label",
        "LocalizedLabels": [
          {
            "@odata.type": "Microsoft.Dynamics.CRM.LocalizedLabel",
            "Label": "Device Id",
            "LanguageCode": 1033,
            "IsManaged": false
          }
        ],
        "UserLocalizedLabel": {
          "@odata.type": "Microsoft.Dynamics.CRM.LocalizedLabel",
          "Label": "Device Id",
          "LanguageCode": 1033,
          "IsManaged": false
        }
      },
      "SchemaName": "contoso_DeviceId"
    },
    {
      "@odata.type": "Microsoft.Dynamics.CRM.IntegerAttributeMetadata",
      "AttributeType": "Integer",
      "AttributeTypeName": {
        "Value": "IntegerType"
      },
      "MaxValue": 2147483647,
      "MinValue": -2147483648,
      "Format": "None",
      "SourceTypeMask": 0,
      "DisplayName": {
        "@odata.type": "Microsoft.Dynamics.CRM.Label",
        "LocalizedLabels": [
          {
            "@odata.type": "Microsoft.Dynamics.CRM.LocalizedLabel",
            "Label": "Value",
            "LanguageCode": 1033,
            "IsManaged": false
          }
        ],
        "UserLocalizedLabel": {
          "@odata.type": "Microsoft.Dynamics.CRM.LocalizedLabel",
          "Label": "Value",
          "LanguageCode": 1033,
          "IsManaged": false
        }
      },
      "SchemaName": "contoso_Value"
    },
    {
      "@odata.type": "Microsoft.Dynamics.CRM.DateTimeAttributeMetadata",
      "AttributeType": "DateTime",
      "AttributeTypeName": {
        "Value": "DateTimeType"
      },
      "Format": "DateOnly",
      "DisplayName": {
        "@odata.type": "Microsoft.Dynamics.CRM.Label",
        "LocalizedLabels": [
          {
            "@odata.type": "Microsoft.Dynamics.CRM.LocalizedLabel",
            "Label": "Time Stamp",
            "LanguageCode": 1033,
            "IsManaged": false
          }
        ],
        "UserLocalizedLabel": {
          "@odata.type": "Microsoft.Dynamics.CRM.LocalizedLabel",
          "Label": "Time Stamp",
          "LanguageCode": 1033,
          "IsManaged": false
        }
      },
      "SchemaName": "contoso_TimeStamp"
    },
    {
      "@odata.type": "Microsoft.Dynamics.CRM.StringAttributeMetadata",
      "AttributeType": "String",
      "AttributeTypeName": {
        "Value": "StringType"
      },
      "MaxLength": 1000,
      "FormatName": {
        "Value": "Json"
      },
      "Description": {
        "@odata.type": "Microsoft.Dynamics.CRM.Label",
        "LocalizedLabels": [
          {
            "@odata.type": "Microsoft.Dynamics.CRM.LocalizedLabel",
            "Label": "Stores unstructured energy consumption data as reported by device",
            "LanguageCode": 1033,
            "IsManaged": false
          }
        ],
        "UserLocalizedLabel": {
          "@odata.type": "Microsoft.Dynamics.CRM.LocalizedLabel",
          "Label": "Stores unstructured energy consumption data as reported by device",
          "LanguageCode": 1033,
          "IsManaged": false
        }
      },
      "DisplayName": {
        "@odata.type": "Microsoft.Dynamics.CRM.Label",
        "LocalizedLabels": [
          {
            "@odata.type": "Microsoft.Dynamics.CRM.LocalizedLabel",
            "Label": "Energy Consumption",
            "LanguageCode": 1033,
            "IsManaged": false
          }
        ],
        "UserLocalizedLabel": {
          "@odata.type": "Microsoft.Dynamics.CRM.LocalizedLabel",
          "Label": "Energy Consumption",
          "LanguageCode": 1033,
          "IsManaged": false
        }
      },
      "SchemaName": "contoso_EnergyConsumption"
    }
  ]
}
```

#### Elastic table - Response

```http
HTTP/1.1 204 NoContent
OData-Version: 4.0
OData-EntityId: [Organization Uri]/api/data/v9.2/EntityDefinitions(3352268a-02eb-ef11-8eea-6045bdec7ce6)
```

### Create record

The code in this section creates an record setting the `partitionid`.

#### Create record - Request

```http
POST [Organization Uri]/api/data/v9.2/contoso_sensordatas
OData-MaxVersion: 4.0
OData-Version: 4.0
If-None-Match: null
Accept: application/json
Authorization: Bearer <access token>
Content-Type: application/json; charset=utf-8
Content-Length: 225

{
  "contoso_deviceid": "Device-ABC-1234",
  "contoso_sensortype": "Humidity",
  "contoso_value": 40,
  "contoso_timestamp": "2025-02-14T18:36:33.2713234Z",
  "partitionid": "Device-ABC-1234",
  "ttlinseconds": 86400
}
```

#### Create record - Response

```http
HTTP/1.1 204 NoContent
OData-Version: 4.0
OData-EntityId: [Organization Uri]/api/data/v9.2/contoso_sensordatas(contoso_sensordataid=e7387f9c-02eb-ef11-8eea-6045bdec7ce6,partitionid='Device-ABC-1234')
x-ms-session-token: 207:30#172462316#7=154204239
```

### Update record

The code in this section updates the record created.

#### Update record - Request

With `partitionId` parameter.

```http
PATCH [Organization Uri]/api/data/v9.2/contoso_sensordatas(contoso_sensordataid=e7387f9c-02eb-ef11-8eea-6045bdec7ce6,partitionid='Device-ABC-1234')?partitionId=Device-ABC-1234
If-Match: *
OData-MaxVersion: 4.0
OData-Version: 4.0
If-None-Match: null
Accept: application/json
Authorization: Bearer <access token>
Content-Type: application/json; charset=utf-8
Content-Length: 27

{
  "contoso_value": 60
}
```

#### Update record - Response

```http
HTTP/1.1 204 NoContent
OData-Version: 4.0
OData-EntityId: [Organization Uri]/api/data/v9.2/contoso_sensordatas(contoso_sensordataid=e7387f9c-02eb-ef11-8eea-6045bdec7ce6,partitionid='Device-ABC-1234')
x-ms-session-token: 207:30#172462318#7=154204239
```

#### Alternative update record - Request

A request with only an alternate key.

```http
PATCH [Organization Uri]/api/data/v9.2/contoso_sensordatas(contoso_sensordataid=e7387f9c-02eb-ef11-8eea-6045bdec7ce6,partitionid='Device-ABC-1234')
If-Match: *
OData-MaxVersion: 4.0
OData-Version: 4.0
If-None-Match: null
Accept: application/json
Authorization: Bearer <access token>
Content-Type: application/json; charset=utf-8
Content-Length: 27

{
  "contoso_value": 80
}
```

#### Alternative update record - Response

```http
HTTP/1.1 204 NoContent
OData-Version: 4.0
OData-EntityId: [Organization Uri]/api/data/v9.2/contoso_sensordatas(contoso_sensordataid=e7387f9c-02eb-ef11-8eea-6045bdec7ce6,partitionid='Device-ABC-1234')
x-ms-session-token: 207:30#172462320#7=154204239
```

### Retrieve record

The code in this section retrieves the record using two different ways to identify the record.

First, using the `partitionId` parameter:

#### Retrieve record - Request

```http
GET [Organization Uri]/api/data/v9.2/contoso_sensordatas(contoso_sensordataid=e7387f9c-02eb-ef11-8eea-6045bdec7ce6,partitionid='Device-ABC-1234')?partitionId=Device-ABC-1234&$select=contoso_value
MSCRM.SessionToken: 207:30#172462320#7=154204239
OData-MaxVersion: 4.0
OData-Version: 4.0
If-None-Match: null
Accept: application/json
Authorization: Bearer <access token>
```

#### Retrieve record - Response

```http
HTTP/1.1 200 OK
ETag: W/"e2011275-0000-0200-0000-67af8d320000"
OData-Version: 4.0

{
  "@odata.context": "[Organization Uri]/api/data/v9.2/$metadata#contoso_sensordatas(contoso_value)/$entity",
  "@odata.etag": "W/\"e2011275-0000-0200-0000-67af8d320000\"",
  "contoso_value": 80,
  "contoso_sensordataid": "e7387f9c-02eb-ef11-8eea-6045bdec7ce6",
  "versionnumber": 638751549947120004,
  "partitionid": "Device-ABC-1234",
  "_ownerid_value": "4026be43-6b69-e111-8f65-78e7d1620f5e",
  "_owningbusinessunit_value": "38e0dbe4-131b-e111-ba7e-78e7d1620f5e"
}
```

#### Alternative retrieve record - Request

A request using an alternate key.

```http
GET [Organization Uri]/api/data/v9.2/contoso_sensordatas(contoso_sensordataid=e7387f9c-02eb-ef11-8eea-6045bdec7ce6,partitionid='Device-ABC-1234')?$select=contoso_value
MSCRM.SessionToken: 207:30#172462320#7=154204239
OData-MaxVersion: 4.0
OData-Version: 4.0
If-None-Match: null
Accept: application/json
Authorization: Bearer <access token>
```

#### Alternative retrieve record - Response

```http
HTTP/1.1 200 OK
ETag: W/"e2011275-0000-0200-0000-67af8d320000"
OData-Version: 4.0

{
  "@odata.context": "[Organization Uri]/api/data/v9.2/$metadata#contoso_sensordatas(contoso_value)/$entity",
  "@odata.etag": "W/\"e2011275-0000-0200-0000-67af8d320000\"",
  "contoso_value": 80,
  "contoso_sensordataid": "e7387f9c-02eb-ef11-8eea-6045bdec7ce6",
  "versionnumber": 638751549947120004,
  "partitionid": "Device-ABC-1234",
  "_ownerid_value": "4026be43-6b69-e111-8f65-78e7d1620f5e",
  "_owningbusinessunit_value": "38e0dbe4-131b-e111-ba7e-78e7d1620f5e"
}
```

### Upsert record

> [!NOTE]
> When Upserting a record you must update the entire record. The contents get overwritten and any data not included in the upsert payload are lost.

The code in this section performs an upsert operation on the record that already exists and references it using an alternate key that includes the partitionid column value.

#### Upsert record - Request

```http
PATCH [Organization Uri]/api/data/v9.2/contoso_sensordatas(contoso_sensordataid=e7387f9c-02eb-ef11-8eea-6045bdec7ce6,partitionid='Device-ABC-1234')
OData-MaxVersion: 4.0
OData-Version: 4.0
If-None-Match: null
Accept: application/json
Authorization: Bearer <access token>
Content-Type: application/json; charset=utf-8
Content-Length: 225

{
  "contoso_deviceid": "Device-ABC-1234",
  "contoso_sensortype": "Humidity",
  "contoso_value": 60,
  "contoso_timestamp": "2025-02-14T18:36:35.0736637Z",
  "partitionid": "Device-ABC-1234",
  "ttlinseconds": 86400
}
```

#### Upsert record - Response

```http
HTTP/1.1 204 NoContent
OData-Version: 4.0
OData-EntityId: [Organization Uri]/api/data/v9.2/contoso_sensordatas(contoso_sensordataid=e7387f9c-02eb-ef11-8eea-6045bdec7ce6,partitionid='Device-ABC-1234')
x-ms-session-token: 207:30#172462322#7=154204239
```

### Delete record

The code in this section deletes the record.

#### Delete record - Request

```http
DELETE [Organization Uri]/api/data/v9.2/contoso_sensordatas(contoso_sensordataid=e7387f9c-02eb-ef11-8eea-6045bdec7ce6,partitionid='Device-ABC-1234')?partitionId=Device-ABC-1234
OData-MaxVersion: 4.0
OData-Version: 4.0
If-None-Match: null
Accept: application/json
Authorization: Bearer <access token>
```

#### Delete record - Response

```http
HTTP/1.1 204 NoContent
OData-Version: 4.0
```

### CreateMultiple

This code performs 10 asynchronous parallel `CreateMultiple` operations of 100 records each to create 1000 records.

#### CreateMultiple - Request

```http
POST [Organization Uri]/api/data/v9.2/contoso_sensordatas/Microsoft.Dynamics.CRM.CreateMultiple
OData-MaxVersion: 4.0
OData-Version: 4.0
If-None-Match: null
Accept: application/json
Authorization: Bearer <access token>
Content-Type: application/json; charset=utf-8
Content-Length: 23924

{
  "Targets": [
    {
      "@odata.type": "Microsoft.Dynamics.CRM.contoso_sensordata",
      "contoso_deviceid": "Device-ABC-1234",
      "contoso_sensortype": "Humidity",
      "partitionid": "Device-ABC-1234",
      "ttlinseconds": 86400
    },
    {
      "@odata.type": "Microsoft.Dynamics.CRM.contoso_sensordata",
      "contoso_deviceid": "Device-ABC-1234",
      "contoso_sensortype": "Humidity",
      "partitionid": "Device-ABC-1234",
      "ttlinseconds": 86400
    },
    {
      "@odata.type": "Microsoft.Dynamics.CRM.contoso_sensordata",
      "contoso_deviceid": "Device-ABC-1234",
      "contoso_sensortype": "Humidity",
      "partitionid": "Device-ABC-1234",
      "ttlinseconds": 86400
    },
    [97 records truncated for brevity]
  ]
}
```

#### CreateMultiple - Response

```http
HTTP/1.1 200 OK
OData-Version: 4.0

{
  "@odata.context": "[Organization Uri]/api/data/v9.2/$metadata#Microsoft.Dynamics.CRM.CreateMultipleResponse",
  "Ids": [
    "f9387f9c-02eb-ef11-8eea-6045bdec7ce6",
    "fa387f9c-02eb-ef11-8eea-6045bdec7ce6",
    "fb387f9c-02eb-ef11-8eea-6045bdec7ce6",
    [97 Ids truncated for brevity]
  ]
}
```

### UpdateMultiple

This code performs 10 asynchronous parallel `UpdateMultiple` operations of 100 records each to create 1000 records, setting the `contoso_energyconsumption` column to the serialized JSON value of the `EnergyConsumption` class.

#### UpdateMultiple - Request

```http
POST [Organization Uri]/api/data/v9.2/contoso_sensordatas/Microsoft.Dynamics.CRM.UpdateMultiple
OData-MaxVersion: 4.0
OData-Version: 4.0
If-None-Match: null
Accept: application/json
Authorization: Bearer <access token>
Content-Type: application/json; charset=utf-8
Content-Length: 31424

{
  "Targets": [
    {
      "@odata.type": "Microsoft.Dynamics.CRM.contoso_sensordata",
      "contoso_sensordataid": "c7397f9c-02eb-ef11-8eea-6045bdec7ce6",
      "partitionid": "Device-ABC-1234",
      "contoso_energyconsumption": "{\"power\":602,\"powerUnit\":\"Watts\",\"voltage\":301,\"voltageUnit\":\"Volts\"}"
    },
    {
      "@odata.type": "Microsoft.Dynamics.CRM.contoso_sensordata",
      "contoso_sensordataid": "c8397f9c-02eb-ef11-8eea-6045bdec7ce6",
      "partitionid": "Device-ABC-1234",
      "contoso_energyconsumption": "{\"power\":604,\"powerUnit\":\"Watts\",\"voltage\":302,\"voltageUnit\":\"Volts\"}"
    },
    {
      "@odata.type": "Microsoft.Dynamics.CRM.contoso_sensordata",
      "contoso_sensordataid": "c9397f9c-02eb-ef11-8eea-6045bdec7ce6",
      "partitionid": "Device-ABC-1234",
      "contoso_energyconsumption": "{\"power\":606,\"powerUnit\":\"Watts\",\"voltage\":303,\"voltageUnit\":\"Volts\"}"
    },
    [97 records truncated for brevity]
  ]
}
```

#### UpdateMultiple - Response

```http
HTTP/1.1 204 NoContent
OData-Version: 4.0
```

### ExecuteCosmosSqlQuery

This section has two parts:

- [Execute the query to retrieve the first 50 records](#execute-the-query-to-retrieve-the-first-50-records)
- [Retrieve the paged results if `response.HasMore` is true](#retrieve-the-paged-results)

#### Execute the query to retrieve the first 50 records

> [!NOTE]
> The query parameters in this example have been URL decoded for readability. They should be URL encoded when sent.

#### ExecuteCosmosSqlQuery - Request

```http
GET [Organization Uri]/api/data/v9.2/ExecuteCosmosSqlQuery(QueryText=@p1,EntityLogicalName=@p2,QueryParameters=@p3,PageSize=@p4,PartitionId=@p5)?@p1='select c.props.contoso_deviceid as deviceId, c.props.contoso_timestamp as timestamp, c.props.contoso_energyconsumption.power as power from c where c.props.contoso_sensortype=@sensortype and c.props.contoso_energyconsumption.power > @power'
&@p2='contoso_sensordata'
&@p3={"Count":0,"IsReadOnly":false,"Keys":["@sensortype","@power"],"Values":[{"Type":"System.String","Value":"Humidity"},{"Type":"System.Int32","Value":"5"}]}
&@p4=50
&@p5='Device-ABC-1234'
MSCRM.SessionToken: 207:8#142792107#7=-1
OData-MaxVersion: 4.0
OData-Version: 4.0
If-None-Match: null
Accept: application/json
```

#### ExecuteCosmosSqlQuery - Response

```http
HTTP/1.1 200 OK
OData-Version: 4.0

{
  "@odata.context": "[Organization Uri]/api/data/v9.2/$metadata#expando/$entity",
  "@odata.type": "#Microsoft.Dynamics.CRM.expando",
  "PagingCookie": "W3sidG9rZW4iOiIrUklEOn5DVm9OQUpJaWRuTjBJajRBQUFBd0R3PT0jUlQ6MSNUUkM6NTAjSVNWOjIjSUVPOjY1NTUxI1FDRjo4I0ZQQzpBWFFpUGdBQUFEQVBveUkrQUFBQU1BOD0iLCJyYW5nZSI6eyJtaW4iOiIxNDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMCIsIm1heCI6IjE0ODAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwIn19XQ==",
  "HasMore": true,
  "Result@odata.type": "#Collection(Microsoft.Dynamics.CRM.expando)",
  "Result": [
    {
      "@odata.type": "#Microsoft.Dynamics.CRM.expando",
      "deviceId": "Device-ABC-1234",
      "power": 6
    },
    [ 49 records truncated for brevity]
  ]
}
```

#### Retrieve the paged results

You can retrieve the paged results as long as the `response.HasMore` value is true.

This time the `PagingCookie` parameter has the value returned from the previous response.

#### Retrieve the paged results - Request

```http
GET [Organization Uri]/api/data/v9.2/ExecuteCosmosSqlQuery(QueryText=@p1,EntityLogicalName=@p2,QueryParameters=@p3,PageSize=@p4,PagingCookie=@p5,PartitionId=@p6)?@p1='select c.props.contoso_deviceid as deviceId, c.props.contoso_timestamp as timestamp, c.props.contoso_energyconsumption.power as power from c where c.props.contoso_sensortype=@sensortype and c.props.contoso_energyconsumption.power > @power'
&@p2='contoso_sensordata'
&@p3={"Count":0,"IsReadOnly":false,"Keys":["@sensortype","@power"],"Values":[{"Type":"System.String","Value":"Humidity"},{"Type":"System.Int32","Value":"5"}]}
&@p4=50
@p5='W3sidG9rZW4iOiIrUklEOn5DVm9OQUpJaWRuTjBJajRBQUFBd0R3PT0jUlQ6MSNUUkM6NTAjSVNWOjIjSUVPOjY1NTUxI1FDRjo4I0ZQQzpBWFFpUGdBQUFEQVBveUkrQUFBQU1BOD0iLCJyYW5nZSI6eyJtaW4iOiIxNDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMCIsIm1heCI6IjE0ODAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwIn19XQ=='
&@p6='Device-ABC-1234'
MSCRM.SessionToken: 207:8#142792107#7=-1
OData-MaxVersion: 4.0
OData-Version: 4.0
If-None-Match: null
Accept: application/json
```

#### Retrieve the paged results - Response

```http
HTTP/1.1 200 OK
OData-Version: 4.0

{
  "@odata.context": "[Organization Uri]/api/data/v9.2/$metadata#expando/$entity",
  "@odata.type": "#Microsoft.Dynamics.CRM.expando",
  "PagingCookie": "",
  "HasMore": false,
  "Result@odata.type": "#Collection(Microsoft.Dynamics.CRM.expando)",
  "Result": [
    {
      "@odata.type": "#Microsoft.Dynamics.CRM.expando",
      "deviceId": "Device-ABC-1234",
      "power": 106
    },
    [49 records truncated for brevity]
  ]
}
```

### DeleteMultiple

This code performs 10 asynchronous parallel `DeleteMultiple` operations of 100 records each to delete 1000 records.

#### DeleteMultiple - Request

```http
POST [Organization Uri]/api/data/v9.2/contoso_sensordatas/Microsoft.Dynamics.CRM.DeleteMultiple
OData-MaxVersion: 4.0
OData-Version: 4.0
If-None-Match: null
Accept: application/json
Authorization: Bearer <access token>
Content-Type: application/json; charset=utf-8
Content-Length: 19324

{
  "Targets": [
    {
      "@odata.type": "Microsoft.Dynamics.CRM.contoso_sensordata",
      "contoso_sensordataid": "f9387f9c-02eb-ef11-8eea-6045bdec7ce6",
      "partitionid": "Device-ABC-1234"
    },
    {
      "@odata.type": "Microsoft.Dynamics.CRM.contoso_sensordata",
      "contoso_sensordataid": "fa387f9c-02eb-ef11-8eea-6045bdec7ce6",
      "partitionid": "Device-ABC-1234"
    },
    {
      "@odata.type": "Microsoft.Dynamics.CRM.contoso_sensordata",
      "contoso_sensordataid": "fb387f9c-02eb-ef11-8eea-6045bdec7ce6",
      "partitionid": "Device-ABC-1234"
    },
    [97 records truncated for brevity]
  ]
}
```

#### DeleteMultiple - Response

```http
HTTP/1.1 204 NoContent
OData-Version: 4.0
```

### Delete table

The code in this section sends this request to delete the `contoso_SensorData` table using the `LogicalName` alternate key.

#### Delete table - Request

```http
DELETE [Organization Uri]/api/data/v9.2/EntityDefinitions(LogicalName='contoso_sensordata')
OData-MaxVersion: 4.0
OData-Version: 4.0
If-None-Match: null
Accept: application/json
Authorization: Bearer <access token>
```

#### Delete table - Response

```http
HTTP/1.1 204 NoContent
OData-Version: 4.0
```

## Clean up

By default this sample deletes the `contoso_SensorData` table created at the beginning. The clean up should leave no data in the system from this sample.
