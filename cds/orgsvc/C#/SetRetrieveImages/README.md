# Set and retrieve entity images

This sample shows how to set and retrieve data for entity images.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/cds/README.md) for information about how to run this sample.

## What this sample does

This sample shows how to create a connection role that can be used for accounts and contacts.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Checks the current version of the org.

### Demonstrate

1. Uses the CreateImageAttributeDemoEntity method to create a custom entity with the schema name `sample_ImageAttributeDemo` and a primary attribute with the schema name `sample_Name`.
2. Create an image attribute with the schema name `EntityImage`. All image attributes use this name.

3. Retrieve and update the main form for the `sample_ImageAttributeDemo` entity to set the `showImage` attribute to true so that the image is displayed in the form.

4. Publish the `sample_ImageAttributeDemo` entity.

5. Creates five new records for the `sample_ImageAttributeDemo` entity using five different sized images located in the Images folder as shown here.After each record is created you have the opportunity to view the record in the web browser application using the `ShowEntityFormInBrowser` method so that you can see how the source images are resized to fit the space available in the form.
6. Retrieves the records with the `entityimage` attribute and saves the resized files. After you run the sample you can find the files saved in the `\bin\Debug` folder.
7. Retrieves the records with the `entityimage_url` attribute and displays the relative URL values you can include in your application to access the images. This query should be more responsive because the amount of data transferred is smaller.

### Clean up

Display an option to delete the records in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
