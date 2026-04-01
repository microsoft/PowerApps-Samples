---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "Demonstrates exporting FetchXML query results to CSV format and storing them as annotation (note) attachments"
---

# ExportDataUsingFetchXmlToAnnotation

Demonstrates exporting FetchXML query results to CSV format and storing them as annotation (note) attachments in Dataverse

## What this sample does

This sample shows how to:
- Execute FetchXML queries to retrieve entity records
- Handle FetchXML paging automatically to retrieve all records
- Convert entity data to CSV format
- Handle various Dataverse data types (Money, EntityReference, OptionSetValue, DateTime, etc.)
- Create annotation (note) records with CSV file attachments
- Store base64-encoded file data in the documentbody attribute

This pattern is useful for:
- Creating data exports that can be viewed or downloaded from Dataverse
- Generating reports as CSV files attached to records
- Creating audit trails or data snapshots
- Building custom export functionality

## How this sample works

### Setup

The setup process:
1. Creates 5 sample account records with name, email, phone, and city data
2. Stores entity references in entityStore for cleanup

### Run

The main demonstration:
1. **Execute FetchXML Query**: Creates a FetchXML query to retrieve accounts matching "Sample Export Account"
2. **Fetch All Data with Paging**: Calls `FetchAllDataFromFetchXml()` which:
   - Executes the initial FetchXML query
   - Checks for MoreRecords flag
   - Automatically handles paging by modifying FetchXML with paging-cookie and page attributes
   - Continues retrieving pages until all records are fetched
3. **Convert to CSV**: Calls `ConvertEntitiesToCsv()` which:
   - Builds a DataTable from entity attributes
   - Dynamically adds columns for all attributes found
   - Serializes Dataverse-specific types (Money, EntityReference, OptionSetValue, DateTime, AliasedValue)
   - Converts DataTable to CSV format with proper quoting and escaping
4. **Create Annotation**: Calls `CreateAnnotationWithCsvData()` which:
   - Creates an annotation entity
   - Sets subject and filename attributes
   - Encodes CSV string to base64 and stores in documentbody
   - Sets mimetype to "text/csv"
   - Returns the annotation ID

### Cleanup

The cleanup process deletes all created records (accounts and annotation) in reverse order.

## Demonstrates

This sample demonstrates:
- **FetchXML**: Building and executing XML-based queries
- **Automatic Paging**: Handling large result sets with paging-cookie
- **Data Type Serialization**: Converting Dataverse types to string format
- **CSV Generation**: Creating properly formatted CSV with headers and quoted fields
- **Annotations**: Creating note records with file attachments
- **Base64 Encoding**: Encoding file content for storage in documentbody
- **Dynamic Schema**: Handling entities with varying attributes
- **Late-Bound Entities**: Using Entity class without early-bound types
- **EntityStore Pattern**: Tracking created entities for cleanup

## Key Methods

### FetchAllDataFromFetchXml
Retrieves all records from a FetchXML query by automatically handling paging:
- Uses RetrieveMultiple with FetchExpression
- Checks MoreRecords flag
- Modifies FetchXML XML to add paging-cookie and page attributes
- Loops until all records are retrieved

### ConvertEntitiesToCsv
Converts entity collection to CSV format:
- Builds DataTable dynamically based on entity attributes
- Handles all entities in collection, even with different attribute sets
- Calls SerializeAttributeValue for each attribute
- Converts DataTable to CSV with proper formatting

### SerializeAttributeValue
Converts Dataverse attribute values to string format:
- **Money**: Extracts decimal value
- **OptionSetValue**: Extracts integer value
- **EntityReference**: Extracts GUID
- **OptionSetValueCollection**: Joins multiple values with comma
- **DateTime**: Formats as ISO 8601 (yyyy-MM-ddTHH:mm:ssZ)
- **AliasedValue**: Recursively serializes inner value
- **Other types**: Uses ToString()

### ConvertDataTableToCsv
Converts DataTable to CSV string:
- Creates header row with column names
- Wraps all fields in quotes for CSV compliance
- Escapes internal quotes by doubling them (" → "")
- Joins fields with commas

### CreateAnnotationWithCsvData
Creates annotation record with CSV attachment:
- Sets subject for identification
- Sets filename with .csv extension
- Converts CSV string to UTF8 bytes
- Encodes bytes to base64 string
- Stores in documentbody attribute
- Sets mimetype to text/csv

## Sample Output

```
Connected to Dataverse.

Setup: Creating sample account records for export...
Created 5 sample accounts.

Demonstrating export of query results to annotation (note)...

Step 1: Executing FetchXML query to retrieve account records...
Retrieved 5 records.

Step 2: Converting entity data to CSV format...
CSV conversion complete.

CSV Preview (first 500 characters):
name,emailaddress1,telephone1,address1_city
"Sample Export Account 1","export1@contoso.com","555-0101","City 1"
"Sample Export Account 2","export2@contoso.com","555-0102","City 2"
"Sample Export Account 3","export3@contoso.com","555-0103","City 3"
"Sample Export Account 4","export4@contoso.com","555-0104","City 4"
"Sample Export Account 5","export5@contoso.com","555-0105","City 5"

Step 3: Creating annotation (note) record with CSV data...
Created annotation with ID: 12345678-abcd-1234-abcd-123456789012

Export complete! The CSV data has been saved as an annotation.
You can view this annotation in Dataverse under the Notes section.

Cleaning up...
Deleting 6 created record(s)...
Records deleted.

Press any key to exit.
```

## Real-World Use Cases

This pattern can be applied to:
1. **Custom Export Reports**: Create scheduled exports of business data
2. **Audit Trails**: Generate snapshots of data at specific points in time
3. **Integration**: Prepare data for external systems that consume CSV
4. **User Downloads**: Allow users to export filtered data from custom pages
5. **Backup Annotations**: Store data backups as attachments to configuration records

## Extension Ideas

To extend this sample:
- Add filtering parameters to customize the FetchXML query
- Support exporting to other formats (Excel, JSON, XML)
- Associate annotations with specific records (set objectid and objecttypecode)
- Compress large CSV files before storing
- Add metadata headers to CSV (export date, user, filter criteria)
- Implement incremental exports using modification dates
- Add support for linked entities in FetchXML
- Handle binary attributes (images, files)

## See also

[Use FetchXML to construct a query](https://learn.microsoft.com/power-apps/developer/data-platform/use-fetchxml-construct-query)
[Page large result sets with FetchXML](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/page-large-result-sets-with-fetchxml)
[Work with annotations (notes)](https://learn.microsoft.com/power-apps/developer/data-platform/annotation-note-entity)
[Query data using the SDK for .NET](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/entity-operations-query-data)
