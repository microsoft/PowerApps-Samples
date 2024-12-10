# Export and import a data map

This sample shows how to create an import map (data map) in Dynamics 365, export it as an XML formatted data, import modified mappings, and create a new import map in Dynamics 365 based on the imported mappings.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

See the text description in the [Setup](#setup) section as the program's primary functionality is performed during setup.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org. 
2. The `CreateImportMapping` method creates the import mapping record.
3. The `RetrieveMappingXML` method exports the mapping (Xml) that is created.
4. The `ChangeMappingName` method parses the Xml to change the name column.
5. The `ImportMappingsByXml` method creates a mapping from the (previously exported) Xml.

### Clean up

Display an option to delete the sample data that is created in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
