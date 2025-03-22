import { Util } from "../scripts/Util.js";
import { DataverseWebAPI as dv } from "../scripts/DataverseWebAPI.js";
export class ConditionalOperationsSample {
  /**
   * @type {dv.Client}
   * @private
   */
  #client; // The DataverseWebAPIClient instance
  #container; // The container element to display messages
  #entityStore = []; // Store for created records to delete at the end of the sample
  #whoIAm; // Store for the current user's information
  #util; //Common functions for samples
  #name = "Conditional Operations";

  // Constructor to initialize the client, container, and utility helper functions
  constructor(client, container) {
    this.#client = client;
    this.#container = container;
    this.#util = new Util(container);
  }

  // Public functions to set up, run, and clean up data created by the sample
  async SetUp() {
    // Clear the container
    this.#container.replaceChildren();
    this.#util.appendMessage(this.#name + " sample started");
    // Get the current user's information
    try {
      this.#whoIAm = await this.#client.WhoAmI();
    } catch (error) {
      this.#util.showError(error.message);
    }
  }
  // Run the sample
  async Run() {
    try {
      //Section 0: Create sample record
      this.#util.appendMessage("<h2>0: Create sample record</h2>");
      let accountRecord = await this.#createRetrieveAccount();

      // Store the initial etag value for later use
      const initialETagValue = accountRecord["@odata.etag"];

      //Section 1: Conditional GET
      this.#util.appendMessage("<h2>1: Conditional GET</h2>");

      // Uses the client.Refresh method to refresh the account record with data from the server if changed
      accountRecord = await this.#conditionalGetUnChanged(
        accountRecord,
        "accountid"
      );

      // Update the account record's telephone number
      await this.#updatePhoneNumber(accountRecord.accountid);

      // Uses the client.Refresh method to refresh the account record with changed data from the server
      accountRecord = await this.#conditionalGetChanged(
        accountRecord,
        "accountid"
      );

      // Store the updated etag value for later use
      const updatedETagValue = accountRecord["@odata.etag"];

      this.#util.appendMessage(
        `Original etag value:  <code> ${initialETagValue}</code> Updated etag value: <code>${updatedETagValue}</code>`
      );

      // Section 2: Optimistic concurrency on delete and update
      this.#util.appendMessage(
        "<h2>2: Optimistic concurrency on delete and update</h2>"
      );
      // This should fail because the record has been changed
      this.#util.appendMessage(
        "Attempting to delete the account record with the <strong>original</strong> etag value: " +
          initialETagValue
      );
      await this.#tryDelete(accountRecord.accountid, initialETagValue);
      // This should fail because the record has been changed
      this.#util.appendMessage(
        "Attempting to update the account record with the <strong>original</strong> etag value: " +
          initialETagValue
      );
      await this.#tryUpdate(accountRecord.accountid, initialETagValue);
      // This should succeed because the etag value is current.
      this.#util.appendMessage(
        "Updating the account record with the <strong>updated</strong> etag value: " +
          updatedETagValue
      );
      await this.#tryUpdate(accountRecord.accountid, updatedETagValue);
      // Show the record with updated values
      await this.#getRecord(accountRecord.accountid);
    } catch (error) {
      this.#util.showError(error.message);
      // Try to clean up even if an error occurs
      await this.CleanUp();
    }
  }

  //#region Section 0: Create sample records

  // Create and retrieve an account record using the CreateRetrieve method
  async #createRetrieveAccount() {
    const contosoAccount = {
      name: "Contoso Ltd",
      telephone1: "555-0000",
      revenue: 5000000,
      description: "Parent company of Contoso Pharmaceuticals, etc.",
    };

    try {
      const createdRetrievedAccount = await this.#client.CreateRetrieve(
        "accounts",
        contosoAccount,
        "$select=name,revenue,telephone1,description",
        false
      );
      // To delete later
      this.#entityStore.push({
        entitySetName: "accounts",
        id: createdRetrievedAccount.accountid,
        name: contosoAccount.name,
        entityName: "account",
      });

      this.#util.appendMessage(
        "Created and retrieved an account record with this data:"
      );
      const table = this.#util.createTable(createdRetrievedAccount, false);
      this.#container.appendChild(table);

      return createdRetrievedAccount;
    } catch (e) {
      this.#util.showError(
        "Failed to create and retrieve account." + e.message
      );
      throw e;
    }
  }

  //#endregion Section 0: Create sample records
  //#region Section 1: Conditional GET

  // Uses the client.Refresh method to refresh the account record with data from the server if changed
  async #conditionalGetUnChanged(accountRecord, primaryKeyName) {
    try {
      // Attempt to refresh the account record with data from the server if changed
      const record = await this.#client.Refresh(accountRecord, primaryKeyName);
      this.#util.appendMessage(
        "Attempted to refresh the account record, but the data is not changed."
      );
      const table = this.#util.createTable(record, false);
      this.#container.appendChild(table);
      return record;
    } catch (e) {
      this.#util.showError("Failed to refresh account record." + e.message);
      throw e;
    }
  }

  // Update the account record's telephone number
  async #updatePhoneNumber(accountId) {
    const newPhoneNumber = "555-0001";
    try {
      await this.#client.SetValue(
        "accounts",
        accountId,
        "telephone1",
        newPhoneNumber
      );
      this.#util.appendMessage(
        `Updated the account record's telephone number to ${newPhoneNumber}`
      );
    } catch (e) {
      this.#util.showError("Failed to update account record." + e.message);
      throw e;
    }
  }

  // Uses the client.Refresh method to refresh the account record with changed data from the server
  async #conditionalGetChanged(accountRecord, primaryKeyName) {
    try {
      // Refresh the account record with changed data from the server
      const record = await this.#client.Refresh(accountRecord, primaryKeyName);
      this.#util.appendMessage(
        "The account record now has a new phone number and etag value."
      );
      const table = this.#util.createTable(record, false);
      this.#container.appendChild(table);
      return record;
    } catch (e) {
      this.#util.showError("Failed to refresh account record." + e.message);
      throw e;
    }
  }
  //#endregion Section 1: Conditional GET

  //#region Section 2: Optimistic concurrency on delete and update

  async #tryDelete(accountId, etagValue) {
    try {
      // Attempt to delete the account record with a specific etag value
      await this.#client.Delete("accounts", accountId, etagValue);
      this.#util.appendMessage(
        "Deleted the account record with etag value: " + etagValue
      );
    } catch (e) {
      this.#util.showExpectedError(
        "As expected, failed to delete account record." + e.message
      );
      // Don't throw e;
    }
  }

  async #tryUpdate(accountId, etagValue) {
    const newData = {
      telephone1: "555-0002",
      revenue: 6000000,
    };
    try {
      // Attempt to update the account record with a specific etag value
      await this.#client.Update("accounts", accountId, newData, etagValue);
      this.#util.appendMessage(
        "Updated the account record with etag value: " + etagValue
      );
    } catch (e) {
      this.#util.showExpectedError(
        "As expected, failed to update account record." + e.message
      );
      // Don't throw e;
    }
  }

  async #getRecord(accountId) {
    try {
      // Retrieve the account record with updated data
      const record = await this.#client.Retrieve(
        "accounts",
        accountId,
        "$select=name,revenue,telephone1,description"
      );
      this.#util.appendMessage(
        "Retrieved the account record with updated values:"
      );
      const table = this.#util.createTable(record, false);
      this.#container.appendChild(table);
    } catch (e) {
      this.#util.showError("Failed to retrieve account record." + e.message);
      throw e;
    }
  }

  //#endregion Section 2: Optimistic concurrency on delete and update

  // Clean up the created records
  async CleanUp() {
    // Section 3: Delete sample records
    this.#util.appendMessage("Deleting the records created by this sample");

    let deleteMessageList = document.createElement("ul");
    this.#container.append(deleteMessageList);

    for (const item of this.#entityStore) {
      try {
        await this.#client.Delete(item.entitySetName, item.id);
        const message = document.createElement("li");
        message.textContent = `Deleted ${item.entityName} ${item.name}`;
        deleteMessageList.append(message);
      } catch (e) {
        const message = document.createElement("li");
        message.textContent = `Failed to delete ${item.entityName} ${item.name}`;
        message.className = "error";
        deleteMessageList.append(message);
      }
    }

    // Set the entity store to an empty array
    this.#entityStore = [];
    this.#util.appendMessage(this.#name + " sample completed.");
    this.#util.appendMessage("<a href='#'>Go to top</a>");
  }
}
