---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample shows how to stage a solution and import it asynchronously into a Microsoft Dataverse environment. [SOAP]"
---

# Solution staging and asynchronous import

This sample shows how to perform the following actions with solutions:

- Stage a solution and check the validation results
- Import the staged solution using an asynchronous job and check for job completion

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

Be sure to edit the App.config file and set the Username, Password, and Url values for your test environment.

## What this sample does

This sample shows how to stage (load) a solution in a Microsoft Dataverse environment and check the solution validation results. This enables you to check for a valid solution staging prior to solution import. Next, the sample performs an asynchronous import of the staged solution. An asynchronous job allows for importing large solutions and avoiding a timeout error.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Invokes the `SampleHelpers.Connect` method to authenticate the user and return a web service reference. 

### Demonstrate

1. The `StageSolution` method reads the compressed solution file and stages the solution.
1. The `ImportSolution` method imports the solution using an asynchronous job.
1. The `CheckImportStatus` method waits for the asynchronous job to complete and checks the job for a successful status.

### Clean up

The program does not automatically delete the imported solution. You should manually delete the solution named "Contoso sample" from your test environment.
