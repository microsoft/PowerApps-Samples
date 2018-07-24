---
title: "Create an email using a template | MicrosoftDocs"
description: "This sample shows how to instantiate an email record by using the InstantiateTemplateRequest message"
ms.custom: ""
ms.date: 07/24/2018
ms.reviewer: ""
ms.service: "crm-online"
ms.suite: ""
ms.tgt_pltfrm: ""
ms.topic: "samples"
applies_to: 
  - "Dynamics 365 (online)"
ms.assetid: 262151de-01be-4c90-992b-9e0084938c90
author: "NavaKiran"
ms.author: "nabuthuk"
---
# Create an email using a template

This sample shows how to instantiate an email record by using the <xref:Microsoft.Crm.Sdk.Messages.InstantiateTemplateRequest> message.  

## How to run this sample

See [How to run samples](../../../How-to-run-samples.md) for information about how to run this sample.

## What this sample does

The `InstantiateTemplateRequest` message is intended to be used in a scenario where it instantiates an email record.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Creates an account record. 
2. Defines the body and subject of the email template in **XML** format.
3. Creates an email template.

### Demonstrate

1. The `InstantiateTemplateRequest` message is used to create an email message using a template. 
2. Serialize the email message to **XML** and save to a file.


### Clean up

1. Display an option to delete the record created in [Setup](#setup).
    The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
