---
title: " Book an Appointment | MicrosoftDocs"
description: "Provides inforamtion on how to book an appointmnet using SDK."
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
ms.assetid: d52e2750-7752-431b-8246-016669b5a34d
author: NavaKiran 
ms.author: nabuthuk
---

# Book an Appointment

This sample shows how to book or schedule an appointment by using the <xref:Microsoft.Crm.Sdk.Messages.BookRequest> message.

## How to run this sample

See [How to run samples](../../../How-to-run-samples.md) for information about how to run this sample.

## What this sample does

The `BookRequest` message is intended to be used in a scenario to book or schedule an appointment.


## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Get's the current user inforamtion and creates the ActivityParty instance.


### Demonstrate

1. Creates the appointment instance using the BookRequest message and verifies that the appointment has been scheduled or not. 

### Clean up

1. Display an option to delete the records created in the [Setup](#setup).

    The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
