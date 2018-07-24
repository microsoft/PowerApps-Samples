---
title: "Create a connection role (early bound) | MicrosoftDocs"
description: "The sample code demonstrates how to create a connection role that are used for accouunts and contacts."
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
  - "sample for creating connection roles, accounts and contacts"
  - "creating connection roles, sample"
ms.assetid: 1083ab7b-d4a2-4fed-95a5-2d247179129b
author: "JimDaly"
ms.author: "jdaly"
---
# Create a connection role (early bound)

This sample shows how to create a connection role that can be used for accounts and contacts.

## How to run this sample

See [How to run samples](../../../How-to-run-samples.md) for information about how to run this sample.

## What this sample does

This sample shows how to create a connection role that can be used for accounts and contacts.

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup
1. Checks the version of the org.


### Demonstrate
1. Defines some anonymous types to define the range of possible conection property values.
2. Creates a connection role for account and contact entity.
3. Creates a connection role object type code record for account and contact entity. 

### Clean up

1. Display an option to delete the records in [Setup](#setup).
    The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
