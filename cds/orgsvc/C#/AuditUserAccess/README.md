---
title: " AuditUser Access | MicrosoftDocs"
description: "Sample doc which expalins different methods used in this sample."
ms.custom:
  - dyn365-developer
  - dyn365-marketing
ms.date: 07/18/2018
ms.service: dynamics-365-marketing
ms.technology: 
  - "marketing"
ms.topic: "get-started-article"
applies_to: 
  - "Dynamics 365 (online)"
ms.assetid: 49613bcb-ec6e-4f7d-8123-4a805a046b64
author: NavaKiran 
ms.author: nabuthuk
---
# Audit User Access

This sample code shows how to audit user access. 

## How to run this sample

See [How to run samples](../../../How-to-run-samples.md) for information about how to run this sample.

## What this sample does
This sample first enables user access auditing with the logged on user's organization. Next, it creates and modifies an account entity so that audit records are genertated.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup
1. Creates a new account entity and enables auditing on the new account entity.

### Demonstrate
1. Get's the organization's ID from the system user record and retrieves organization record.
2. Enables auditing on the organization, including auditing for user access.
3. Makes an update request ti the account entity to be tracked byauditing.
4. set the organization and account auditing flags back to old values and retrieve them if they were actually changed.

### Clean up

1. Display an option to delete the records created during [Setup](#setup). If you opt **Yes** all the records are deleted.

    The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
