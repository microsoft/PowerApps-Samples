import { Util } from "../scripts/Util.js";
import { DataverseWebAPI as dv } from "../scripts/DataverseWebAPI.js";
export class TemplateSample {
  /**
   * @type {dv.Client}
   * @private
   */
  #client; // The DataverseWebAPI.Client instance
  #container; // The container element to display messages
  #entityStore = []; // Store for created records to delete at the end of the sample
  #whoIAm; // Store for the current user's information
  #util; // Util instance for utility functions
  #name = "Template sample"; // Name of the sample

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
    this.#util.appendMessage(this.#name + " started");
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
      // Section 1: Do somethings
      this.#util.appendMessage("<h2>1: Do something</h2>");
      await this.#doSomething();

      
    } catch (error) {
      this.#util.showError(error.message);
      // Try to clean up even if an error occurs
      await this.CleanUp();
    }
  }


  //#region Section 1: Do somethings
  // Demonstrates doing something
  async #doSomething() {

    try {
      // Do something with DataverseWebAPIClient here

     const whoAmIResponse =  await this.#client.WhoAmI();
      this.#util.appendMessage(`User ID: ${whoAmIResponse.UserId}`);
      this.#util.appendMessage(`BusinessUnit Id: ${whoAmIResponse.BusinessUnitId}`);
      this.#util.appendMessage(`Organization ID: ${whoAmIResponse.OrganizationId}`);

    } catch (e) {
      this.#util.showError(`Failed to do something: ${e.message}`);
      throw e;
    }
  }
  //#endregion Section 1: Do somethings

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
        message.textContent = `Failed to delete ${item.entityName} ${item.name}`;
        message.className = "error";
        deleteMessageList.append(message);
      }
    }

    // Set the entity store to an empty array
    this.#entityStore = [];
    this.#util.appendMessage(this.#name + " completed.");
    this.#util.appendMessage("<a href='#'>Go to top</a>");
  }
}
