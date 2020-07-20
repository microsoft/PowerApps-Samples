---
languages:
- csharp
products:
- dotnet
- powerapps
page_type: sample
description: "This sample shows how to create, retrieve, update, delete and associative operations using the Web API in Common Data Service. [REST]"
---

# Web API Basic Operations Sample

This Sample demonstrates how to perform basic CRUD (Create, Retrieve, Update, and Delete) and associative operations using the Common Data Service Web API.

## How to run the sample

see [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/cds/README.md) for information about how to run this sample.

## Demonstrate

1. The `createrequest1` creates a contact record with the `firstname` and `lastname`. 
2. The `createResponse1` gets the response and diaplays the result. 
3. The `updaterequest1` updates the existing contact record that is created with new values. 
4. The `queryOption` filters the contact record by fullname, annulaincome, jobtitle, description.
5. The `retrieveresponse1` gets the response and displays the result.
6. The `updateRequest2` updates the specific properties of the contact record. 
7. The `createRequest2` creates a new account and associate with existing contact in one operation.
8. The `queryOption1` queries the account name and primary contact info. 
9. The `createRequest3` creates an account, its primary contact info and open tasks for that contatct. 
10. The `queryOption2` retrieves account, primary contact info, and assigned tasks for contact. 
11. The `assRequest1` demonstrates the association of existing entity instances.
12. The `queryOption4` retrieves and output all the contacts for account `Fourth Coffee`.
13. The `createRequest4` associates an opportunity to a competitor.
14. The `assocRequest2` associates opportunity to competitor via opportunitycompetitors_association.
15. The `queryOption5` retrieves all opportunities for competitor `Adventure Works`.

## Clean up

1. Displays an option to delete the records.

    The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
