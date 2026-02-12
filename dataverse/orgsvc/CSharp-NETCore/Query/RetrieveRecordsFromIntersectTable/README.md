# Retrieve records from an intersect table

This sample demonstrates how to retrieve records from an intersect table in Microsoft Dataverse. Intersect tables are used to represent many-to-many relationships between entities.

## How to run this sample

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.

2. Navigate to the sample directory:
   ```
   cd dataverse/orgsvc/CSharp-NETCore/Query/RetrieveRecordsFromIntersectTable
   ```

3. Update the connection string in `../appsettings.json` with your Dataverse environment details:
   ```json
   {
     "ConnectionStrings": {
       "default": "AuthType=OAuth;Url=https://yourorg.crm.dynamics.com;Username=youruser@yourdomain.com;AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;RedirectUri=http://localhost;LoginPrompt=Auto"
     }
   }
   ```

4. Build and run the sample:
   ```
   dotnet run
   ```

## What this sample does

This sample demonstrates three different approaches for retrieving records from an intersect table:

1. **QueryExpression with LinkEntity** - Uses a QueryExpression to link from the primary entity (role) through the intersect table (systemuserroles) with filter criteria.

2. **FetchXML with intersect link-entity** - Uses FetchXML with the `intersect="true"` attribute to query through the many-to-many relationship.

3. **Direct query of intersect table** - Directly queries the systemuserroles intersect table to retrieve association records.

## How this sample works

### Setup

1. Retrieves the default business unit needed to create the role.
2. Gets the GUID of the current user using `WhoAmIRequest`.
3. Creates a custom role named "ABC Management Role".
4. Associates the current user with the role using the `systemuserroles_association` relationship.

### Demonstrate

The sample demonstrates three different query approaches:

#### 1. QueryExpression with LinkEntity
Creates a QueryExpression that links from the role entity to the systemuserroles intersect table, filtering by the current user's ID. This approach retrieves role records that are associated with the user.

```csharp
var query = new QueryExpression
{
    EntityName = "role",
    ColumnSet = new ColumnSet("name")
};

var linkEntity = new LinkEntity
{
    LinkFromEntityName = "role",
    LinkFromAttributeName = "roleid",
    LinkToEntityName = "systemuserroles",
    LinkToAttributeName = "roleid",
    LinkCriteria = new FilterExpression
    {
        Conditions =
        {
            new ConditionExpression("systemuserid", ConditionOperator.Equal, userId)
        }
    }
};
query.LinkEntities.Add(linkEntity);
```

#### 2. FetchXML with intersect attribute
Uses FetchXML with `intersect="true"` to query through the many-to-many relationship. This is a more declarative approach.

```xml
<fetch version="1.0" output-format="xml-platform" mapping="logical" distinct="true">
  <entity name="role">
    <attribute name="name"/>
    <link-entity name="systemuserroles" from="roleid" to="roleid" visible="false" intersect="true">
      <filter type="and">
        <condition attribute="systemuserid" operator="eq" value="{userId}"/>
      </filter>
    </link-entity>
  </entity>
</fetch>
```

#### 3. Direct query of intersect table
Directly queries the systemuserroles intersect table to retrieve the association records themselves, showing the raw relationship data.

```csharp
var query = new QueryExpression
{
    EntityName = "systemuserroles",
    ColumnSet = new ColumnSet("systemuserid", "roleid"),
    Criteria = new FilterExpression
    {
        Conditions =
        {
            new ConditionExpression("systemuserid", ConditionOperator.Equal, userId),
            new ConditionExpression("roleid", ConditionOperator.Equal, roleId)
        }
    }
};
```

### Clean up

1. Disassociates the user from the role using `DisassociateRequest`.
2. Deletes the custom role that was created.

## Key concepts

### Intersect Tables
- Intersect tables represent many-to-many relationships in Dataverse
- They contain only the IDs of the related entities (e.g., systemuserid and roleid)
- The table name is typically a combination of both entity names (e.g., systemuserroles)

### Querying Intersect Tables
- **LinkEntity approach**: Best when you need data from the related entities
- **FetchXML with intersect**: More declarative, easier to construct dynamically
- **Direct query**: Useful when you need the raw association data or metadata

### Association/Disassociation
- Use `AssociateRequest` to create relationships
- Use `DisassociateRequest` to remove relationships
- The `Relationship` object specifies the schema name (e.g., "systemuserroles_association")

## Related documentation

- [QueryExpression class](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.query.queryexpression)
- [FetchXML reference](https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/overview)
- [Many-to-many relationships](https://learn.microsoft.com/power-apps/developer/data-platform/create-retrieve-entity-relationships#many-to-many-relationships)
- [AssociateRequest class](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.associaterequest)
- [DisassociateRequest class](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.disassociaterequest)
