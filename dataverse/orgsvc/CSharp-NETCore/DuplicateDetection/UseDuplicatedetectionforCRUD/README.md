---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "Demonstrates using duplicate detection with Create and Update operations"
---

# UseDuplicatedetectionforCRUD

Demonstrates using duplicate detection with Create and Update operations

## What this sample does

This sample shows how to:
- Create and publish a duplicate detection rule programmatically
- Control duplicate detection behavior during Create operations using the SuppressDuplicateDetection parameter
- Control duplicate detection behavior during Update operations using the SuppressDuplicateDetection parameter
- Bypass duplicate detection when needed
- Allow duplicate detection to run when needed

The SuppressDuplicateDetection parameter provides fine-grained control over when duplicate detection fires during CRUD operations, allowing developers to choose when to enforce or bypass duplicate detection rules.

## How this sample works

### Setup

The setup process:
1. Creates an initial account record named "Fourth Coffee" with account number "ACC005"
2. Creates a duplicate detection rule for accounts with matching account numbers
3. Adds a rule condition for exact match on the "accountnumber" attribute
4. Publishes the duplicate detection rule using PublishDuplicateRuleRequest
5. Polls the rule's statuscode until it reaches "Published" (statuscode = 2)

### Run

The main demonstration:
1. **Create with SuppressDuplicateDetection = true**:
   - Creates a duplicate account "Proseware, Inc." with the same account number "ACC005"
   - Uses CreateRequest with SuppressDuplicateDetection parameter set to true
   - The duplicate is created successfully because detection was suppressed

2. **Retrieve the duplicate account**:
   - Uses standard Retrieve to get the account record
   - Retrieves name and accountnumber attributes

3. **Update with SuppressDuplicateDetection = false**:
   - Updates the account with a new account number "ACC006"
   - Uses UpdateRequest with SuppressDuplicateDetection parameter set to false
   - Duplicate detection is active, but no duplicates exist with "ACC006", so update succeeds

### Cleanup

The cleanup process:
1. Unpublishes the duplicate detection rule using UnpublishDuplicateRuleRequest
2. Deletes all created records in reverse order:
   - Duplicate rule conditions
   - Duplicate detection rule
   - Account records
3. Handles errors gracefully if unpublish or delete operations fail

## Demonstrates

This sample demonstrates:
- **CreateRequest with SuppressDuplicateDetection**: Bypassing duplicate detection during create
- **UpdateRequest with SuppressDuplicateDetection**: Controlling duplicate detection during update
- **PublishDuplicateRuleRequest/Response**: Publishing duplicate detection rules
- **UnpublishDuplicateRuleRequest**: Unpublishing rules before deletion
- **Status code polling**: Waiting for rule publishing to complete
- **Programmatic rule creation**: Creating duplicate rules and conditions via SDK
- **Request.Parameters collection**: Adding optional parameters to SDK requests

## Sample Output

```
Connected to Dataverse.

Creating initial account record...
  Created account: Fourth Coffee (ACC005)

Creating duplicate detection rule...
  Rule created: a1234567-89ab-cdef-0123-456789abcdef
  Rule condition created
Publishing duplicate detection rule...
  Waiting for rule to publish...
  Rule published successfully

Demonstrating duplicate detection control with CRUD operations...

Creating duplicate account with SuppressDuplicateDetection = true...
  Created: Proseware, Inc. (ACC005)
  Duplicate detection was suppressed, so the duplicate was created

Retrieving the account...
  Retrieved: Proseware, Inc.

Updating account with SuppressDuplicateDetection = false...
  Updated account number to: ACC006
  Duplicate detection was active, update succeeded (no duplicates found)

Duplicate detection CRUD operations complete.
Cleaning up...
Unpublishing duplicate detection rule...
  Rule unpublished
Deleting 4 created record(s)...
Records deleted.

Press any key to exit.
```
