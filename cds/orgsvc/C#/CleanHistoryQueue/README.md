---
title: "Sample: Clean up history for a queue (early bound) | MicrosoftDocs"
decription: "The sample code demonstrates how to clean up the history for the queue using RemoveFromQueueRequest with inactive items."
ms.custom: ""
ms.date: 07/24/2018
ms.reviewer: ""
ms.service: "crm-online"
ms.suite: ""
ms.tgt_pltfrm: ""
ms.topic: "samples"
applies_to: 
  - "Dynamics 365 (online)"
helpviewer_keywords: 
  - "cleaning up history in queues (early bound), sample"
  - "sample for deleting inactive items from queues (early bound)"
  - "sample for cleaning up history in queues (early bound)"
  - "deleting inactive items from queues (early bound), sample"
ms.assetid: 369a2d36-9eb0-433c-81c1-14856b4d2e45
author: "NavaKiran"
ms.author: "nabuthuk"
---

# Clean up history for a queue (early bound)

 This sample shows how to clean up the history for the queue by using <xref:Microsoft.Crm.Sdk.Messages.RemoveFromQueueRequest> with inactive items. It finds completed phone calls in the queue and removes the associated queue items.

## How to run this sample

See [How to run samples](../../../How-to-run-samples.md) for information about how to run this sample.

## What this sample does

The `RemoveFromQueueRequest` message is intended to be used in a scenario to clean up the queue history with inactive items.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup
1. Creates a queue instance and set its property values. 
2. Creates a phone call activity instance and also queueitems instance and intializes uts properties.
3. Marks the phone call as completed. 


### Demonstrate
1. Retrieves the queueitem with inactive phone calls from a queue using the `RemoveFromQueueRequest` message.


### Clean up

1. Display an option to delete the records created in [Setup](#setup).

    The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
