import { DataverseWebAPI as dv } from "../scripts/DataverseWebAPI.js";
import { Util } from "../scripts/Util.js";
export class BatchSample {
   /**
    * @type {dv.Client}
    * @private
    */
   #client; // The DataverseWebAPIClient.Client instance
   #container; // The container element to display messages
   #entityStore = []; // Store for created records to delete at the end of the sample
   #whoIAm; // Store for the current user's information
   #util; // Util instance for utility functions
   #name = "Batch Operations";

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
         // Section 1: Create three accounts with $batch
         this.#util.appendMessage("<h2>Create three accounts with $batch</h2>");
         await this.#createAccountsInChangeSet();
      } catch (error) {
         this.#util.showError(error.message);
         // Try to clean up even if an error occurs
         await this.CleanUp();
      }
   }

   //#region Section 1: Create three accounts with $batch

   async #createAccountsInChangeSet() {

      const account1 = new Request(
         new URL(
            "accounts", 
            this.#client.apiEndpoint), {
         method: "POST",
         body: JSON.stringify({ name: "Account 1 of 3" }),
         headers: {
            "Content-Type": "application/json",
         },
      }
      );

      const account2 = new Request(
         new URL(
            "accounts", 
            this.#client.apiEndpoint), {
         method: "POST",
         body: JSON.stringify({ name: "Account 2 of 3" }),
         headers: {
            "Content-Type": "application/json",
         },
      }
      );

      const account3 = new Request(
         new URL(
            "accounts", 
            this.#client.apiEndpoint), {
         method: "POST",
         body: JSON.stringify({ name: "Account 3 of 3" }),
         headers: {
            "Content-Type": "application/json",
            Prefer: "return=representation"
         },
      }
      );

      const whoAmI = new Request(
         new URL(
            "WhoAmI", 
            this.#client.apiEndpoint), {
         method: "GET"
      }
      );

      const changeSet = new dv.ChangeSet(this.#client, [account1, account2]);

      const batch = [changeSet, account3, whoAmI];

      try {
         const batchResponse = await this.#client.Batch(batch, true);

         for (const response of batchResponse) {
            let accountId = null;
            switch (response.status) {
               case 201:
                  const record = await response.json();
                  accountId = record.accountid;
                  break;
               case 204:
                  const url = response.headers.get("OData-EntityId");
                  accountId = url.substring(
                     url.lastIndexOf("(") + 1,
                     url.lastIndexOf(")")
                  );
                  break;
            }
            if (accountId) {
               this.#entityStore.push({
                  name: "created in batch",
                  entityName: "account",
                  entitySetName: "accounts",
                  id: accountId,
               });
            }
         }
      } catch (e) {
         this.#util.showError(
            `Failed to create Accounts In ChangeSet: ${e.message}`
         );
         throw e;
      }
   }
   //#endregion Section 1: Create three accounts with $batch
   // Clean up the created records
   async CleanUp() {
      if (this.#entityStore.length === 0) {
         this.#util.appendMessage("No records to delete");
         return;
      }

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
            message.textContent = `Failed to delete ${item.entityName} ${item.name}: ${e.message}`;
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
