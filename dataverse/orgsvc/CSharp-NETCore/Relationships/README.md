# Relationships

Samples demonstrating how to work with relationships between entities in Dataverse including associations, connections, and connection roles.

These samples show how to associate and disassociate records, create and manage connections between records, work with connection roles, and handle reciprocal connections.

More information: [Entity relationships](https://learn.microsoft.com/power-apps/developer/data-platform/entity-relationship-metadata)

## Samples

|Sample folder|Description|Build target|
|---|---|---|
|AssociateRecords|Associate and disassociate records using relationships|.NET 6|
|ConnectionEarlyBound|Create connections between records using early-bound entities|.NET 6|
|ConnectionRole|Create and manage connection roles|.NET 6|
|ReciprocalConnection|Create reciprocal connections between records|.NET 6|
|UpdateConnectionRole|Update connection role definitions|.NET 6|

## Prerequisites

- Visual Studio 2022 or later
- .NET 6.0 SDK or later
- Access to a Dataverse environment

## How to run samples

1. Clone the PowerApps-Samples repository
2. Navigate to `dataverse/orgsvc/CSharp-NETCore/Relationships/`
3. Open the desired sample folder
4. Edit the `appsettings.json` file (located in the Relationships folder) with your environment connection details:
   ```json
   {
     "ConnectionStrings": {
       "default": "AuthType=OAuth;Url=https://yourorg.crm.dynamics.com;Username=youruser@yourdomain.com;AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;RedirectUri=http://localhost;LoginPrompt=Auto"
     }
   }
   ```
5. Build and run the sample:
   ```bash
   cd SampleFolder
   dotnet run
   ```

## See also

[Entity relationships](https://learn.microsoft.com/power-apps/developer/data-platform/entity-relationship-metadata)
[Connection entities](https://learn.microsoft.com/power-apps/developer/data-platform/connection-entities)
[Associate and disassociate records](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/entity-operations-associate-disassociate)
