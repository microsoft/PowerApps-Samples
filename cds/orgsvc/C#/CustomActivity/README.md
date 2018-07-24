---
title: "Create a custom activity | MicrosoftDocs"
description: "The following code example demonstrates how to create a custom activity using the CreateEntityRequest and CreateAttributeRequest messages"
ms.custom: ""
ms.date: 07/24/2018
ms.reviewer: ""
ms.service: "crm-online"
ms.suite: ""
ms.tgt_pltfrm: ""
ms.topic: "samples"
applies_to: 
  - "Dynamics 365 (online)"
ms.assetid: eec904da-5af8-43f4-b8f7-32642d8e4916
author: "NavaKiran"
ms.author: "nabuthuk"
---
# Create a custom activity

The following code example demonstrates how to create a custom activity using the <xref:Microsoft.Xrm.Sdk.Messages.CreateEntityRequest> and <xref:Microsoft.Xrm.Sdk.Messages.CreateAttributeRequest> messages.  

## How to run this sample

See [How to run samples](../../../How-to-run-samples.md) for information about how to run this sample.

## What this sample does

The `CreateEntityRequest` message and `CreateAttributeRequest` message is intended to be used in a scenario to create custom activity.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks the version of the current org.

### Demonstrate

1. Creates the custom activity entity using the `CreateEntityRequest` message.
2. Publishes the created custom activity entity.
3. Creates few attributes to the custom activity entity using `CreateAttributeRequest` mesage.

### Clean up

1. Display an option to delete the records in [Setup](#setup).

    The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
