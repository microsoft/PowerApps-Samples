import { Util } from "../scripts/Util.js";
import { DataverseWebAPI as dv } from "../scripts/DataverseWebAPI.js";
export class BasicOperationsSample {
  /**
   * @type {dv.Client}
   * @private
   */
  #client; // The DataverseWebAPIClient.Client instance
  #container; // The container element to display messages
  #entityStore = []; // Store for created records to delete at the end of the sample
  #whoIAm; // Store for the current user's information
  #util; // Util instance for utility functions
  #name = "Basic Operations";

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
      // Section 1: Basic create and update operations
      this.#util.appendMessage(
        "<h2>1: Basic create and update operations</h2>"
      );
      const rafelShilloId = await this.#createContact();
      await this.#updateContact(rafelShilloId);
      await this.#retrieveContact(rafelShilloId);
      await this.#updateContactAgain(rafelShilloId);
      await this.#retrieveContact(rafelShilloId);
      await this.#setPhoneNumber(rafelShilloId);
      await this.#getPhoneNumber(rafelShilloId);
      // Section 2: Create with association
      this.#util.appendMessage("<h2>2: Create with association</h2>");
      const contosoAccountId = await this.#createWithAssociation(rafelShilloId);
      await this.#retrievePrimaryContactFromAccount(contosoAccountId);
      // Section 3: Create related table rows (deep insert)
      this.#util.appendMessage(
        "<h2>3: Create related table rows (deep insert)</h2>"
      );
      const fourthCoffeeId = await this.#createRelatedTableRows();
      const susieCurtisId = await this.#retrieveFourthCoffeePrimaryContact(
        fourthCoffeeId
      );
      await this.#retrieveContactRelatedTasks(susieCurtisId);
      // Section 4: Associate and disassociate existing entities
      this.#util.appendMessage(
        "<h2>4: Associate and disassociate existing entities</h2>"
      );
      await this.#associateContactToAccount(fourthCoffeeId, rafelShilloId);
      await this.#showRelatedContacts(fourthCoffeeId);
      const roleId = await this.#createSecurityRole();
      await this.#associateRoleToUser(roleId);
      await this.#retrieveRelatedRole(roleId);
      await this.#disassociateRoleFromUser(roleId);
    } catch (error) {
      this.#util.showError(error.message);
      // Try to clean up even if an error occurs
      await this.CleanUp();
    }
  }

  //#region Section 1: Basic create and update operations
  // Demonstrates creating a record
  async #createContact() {
    const contact = {
      firstname: "Rafel",
      lastname: "Shillo",
    };
    try {
      const contactId = await this.#client.Create("contacts", contact);
      this.#util.appendMessage(
        `Created contact: ${contact.firstname} ${contact.lastname}`
      );
      // To delete later
      this.#entityStore.push({
        name: `${contact.firstname} ${contact.lastname}`,
        entityName: "contact",
        entitySetName: "contacts",
        id: contactId,
      });
      return contactId;
    } catch (e) {
      this.#util.showError(`Failed to create contact: ${e.message}`);
      throw e;
    }
  }

  // Demonstrates updating a record
  async #updateContact(contactId) {
    const contact = {
      annualincome: 80000,
      jobtitle: "Junior Developer",
    };
    try {
      await this.#client.Update("contacts", contactId, contact);
      this.#util.appendMessage(
        `Updated contact setting new job title and annual income.`
      );
    } catch (e) {
      this.#util.showError("Failed to update contact.");
      throw e;
    }
  }

  // Demonstrates retrieving a record
  async #retrieveContact(contactId) {
    try {
      const contact = await this.#client.Retrieve(
        "contacts",
        contactId,
        "$select=fullname,annualincome,telephone1,jobtitle,description"
      );

      this.#util.appendMessage(`Retrieved contact with id: ${contactId}`);
      const table = this.#util.createTable(contact);
      this.#container.appendChild(table);
    } catch (e) {
      this.#util.showError("Failed to retrieve contact.");
      throw e;
    }
  }

  // Demonstrates updating the record again.
  async #updateContactAgain(contactId) {
    const contact = {
      annualincome: 95000,
      jobtitle: "Senior Developer",
      description: "Assignment to-be-determined",
    };
    try {
      await this.#client.Update("contacts", contactId, contact);
      this.#util.appendMessage(
        `Updated contact setting new job title, annual income, and description.`
      );
    } catch (e) {
      this.#util.showError("Failed to update contact.");
      throw e;
    }
  }

  // Demonstrates setting a single value on a record
  async #setPhoneNumber(contactId) {
    const phoneNumber = "555-0105";
    try {
      await this.#client.SetValue(
        "contacts",
        contactId,
        "telephone1",
        phoneNumber
      );
      this.#util.appendMessage(`Set contact phone number to: ${phoneNumber}`);
    } catch (e) {
      this.#util.showError("Failed to update contact.");
      throw e;
    }
  }

  // Demonstrates retrieving a single value from a record
  async #getPhoneNumber(contactId) {
    try {
      const phoneNumber = await this.#client.GetValue(
        "contacts",
        contactId,
        "telephone1"
      );
      this.#util.appendMessage(
        `Contact's telephone number is: ${phoneNumber}.`
      );
    } catch (e) {
      this.#util.showError("Failed to retrieve contact phone number.");
      throw e;
    }
  }
  //#endregion Section 1: Basic create and update operations

  //#region Section 2: Create with association

  // Demonstrates creating a record with an association to another record
  async #createWithAssociation(contactId) {
    // Use the @odata.bind notation to create an account associated with an existing primary contact
    const contosoAccount = {
      name: "Contoso Ltd",
      telephone1: "555-5555",
      "primarycontactid@odata.bind": "contacts(" + contactId + ")",
    };

    try {
      const accountId = await this.#client.Create("accounts", contosoAccount);

      this.#util.appendMessage(`Created ${contosoAccount.name} account`);
      this.#entityStore.push({
        name: contosoAccount.name,
        entityName: "account",
        entitySetName: "accounts",
        id: accountId,
      });
      return accountId;
    } catch (e) {
      this.#util.showError(`Failed to create account: ${e.message}`);
      throw e;
    }
  }

  // Demonstrates retrieving a record with an associated record
  async #retrievePrimaryContactFromAccount(accountId) {
    try {
      const account = await this.#client.Retrieve(
        "accounts",
        accountId,
        "$select=name&$expand=primarycontactid($select=fullname,jobtitle,annualincome)"
      );

      this.#util.appendMessage(
        `Account '${account.name}' has primary contact '${account.primarycontactid.fullname}':`
      );
      const table = this.#util.createTable(account.primarycontactid);
      this.#container.appendChild(table);
    } catch (e) {
      this.#util.showError("Failed to retrieve account.");
      throw e;
    }
  }

  //#endregion Section 2: Create with association

  //#region Section 3: Create related table rows (deep insert)

  // Demonstrates creating a record with related table rows (deep insert)
  async #createRelatedTableRows() {
    const fourthCoffee = {
      name: "Fourth Coffee",
      primarycontactid: {
        firstname: "Susie",
        lastname: "Curtis",
        jobtitle: "Coffee Master",
        annualincome: 48000,
        Contact_Tasks: [
          {
            subject: "Sign invoice",
            description: "Invoice #12321",
            scheduledstart: new Date(Date.UTC(2025, 3, 18, 20)),
            scheduledend: new Date(Date.UTC(2025, 3, 18, 21)),
            scheduleddurationminutes: 60,
          },
          {
            subject: "Setup new display",
            description: "Theme is - Spring is in the air",
            scheduledstart: new Date(Date.UTC(2025, 4, 18, 20)),
            scheduledend: new Date(Date.UTC(2025, 4, 18, 21)),
            scheduleddurationminutes: 60,
          },
          {
            subject: "Conduct training",
            description: "Train team on making our new blended coffee",
            scheduledstart: new Date(Date.UTC(2025, 4, 21, 20)),
            scheduledend: new Date(Date.UTC(2025, 4, 21, 21)),
            scheduleddurationminutes: 60,
          },
        ],
      },
    };
    try {
      const accountId = await this.#client.Create("accounts", fourthCoffee);
      this.#util.appendMessage(`Account '${fourthCoffee.name}' created.`);
      this.#entityStore.push({
        name: fourthCoffee.name,
        entityName: "account",
        entitySetName: "accounts",
        id: accountId,
      });
      return accountId;
    } catch (e) {
      this.#util.showError(
        `Failed to create account '${fourthCoffee.name}' : ${e.message}.`
      );
      throw e;
    }
  }

  // Demonstrates retrieving a record with related table rows
  async #retrieveFourthCoffeePrimaryContact(fourthCoffeeId) {
    try {
      const account = await this.#client.Retrieve(
        "accounts",
        fourthCoffeeId,
        "$select=name&$expand=primarycontactid($select=fullname,jobtitle,annualincome)"
      );
      this.#util.appendMessage(
        `Account '${account.name}' has primary contact '${account.primarycontactid.fullname}':`
      );
      const table = this.#util.createTable(account.primarycontactid);
      this.#container.appendChild(table);
      return account.primarycontactid.contactid;
    } catch (e) {
      this.#util.showError(`Failed to retrieve account: ${e.message}`);
      throw e;
    }
  }

  // Demonstrates retrieving a record with related table rows
  async #retrieveContactRelatedTasks(contactId) {
    try {
      const contact = await this.#client.Retrieve(
        "contacts",
        contactId,
        "$select=fullname&$expand=Contact_Tasks($select=subject,description,scheduledstart,scheduledend)"
      );
      this.#util.appendMessage(
        `Contact has ${contact.Contact_Tasks.length} related tasks:`
      );

      let taskList = document.createElement("ul");
      for (const task of contact.Contact_Tasks) {
        const taskItem = this.#util.appendMessage(
          `Task: ${task.subject}`,
          taskList,
          "li"
        );
        const table = this.#util.createTable(task);
        taskItem.appendChild(table);
      }
      this.#container.appendChild(taskList);
    } catch (e) {
      this.#util.showError(
        `Failed to retrieve contact with related tasks: ${e.message}`
      );
      throw e;
    }
  }
  //#endregion Section 3: Create related table rows (deep insert)

  //#region Section 4: Associate and disassociate existing entities

  async #associateContactToAccount(accountId, contactId) {
    try {
      await this.#client.Associate(
        "accounts",
        accountId,
        "contact_customer_accounts",
        "contacts",
        contactId
      );
      this.#util.appendMessage(
        "Associated contact Rafel Shillo to account Contoso Ltd."
      );
    } catch (e) {
      this.#util.showError(
        `Failed to associate contact with id: ${contactId} to account with id: ${accountId}: ${e.message}`
      );
      throw e;
    }
  }

  // Demonstrates retrieving related records
  async #showRelatedContacts(accountId) {
    try {
      const contacts = await this.#client.RetrieveMultiple(
        "accounts(" + accountId + ")/contact_customer_accounts",
        "$select=fullname,jobtitle"
      );
      this.#util.appendMessage(
        `Account has ${contacts.value.length} related contacts:`
      );
      let contactList = document.createElement("ul");
      for (const contact of contacts.value) {
        const contactItem = this.#util.appendMessage(
          `Name: ${contact.fullname}`,
          contactList,
          "li"
        );
        const table = this.#util.createTable(contact);
        contactItem.appendChild(table);
      }
      this.#container.appendChild(contactList);
    } catch (e) {
      this.#util.showError(`Failed to retrieve related contacts: ${e.message}`);
      throw e;
    }
  }

  async #createSecurityRole() {
    const role = {
      name: "Example Security Role",
      "businessunitid@odata.bind": `businessunits(${
        this.#whoIAm.BusinessUnitId
      })`,
    };

    try {
      const roleId = await this.#client.Create("roles", role);
      this.#util.appendMessage(
        `Created the ${role.name} security role with id: ${roleId}`
      );
      // To delete later
      this.#entityStore.push({
        name: role.name,
        entityName: "role",
        entitySetName: "roles",
        id: roleId,
      });
      return roleId;
    } catch (e) {
      this.#util.showError(`Failed to create a security role: ${e.message}`);
      throw e;
    }
  }

  async #associateRoleToUser(roleId) {
    try {
      await this.#client.Associate(
        "systemusers",
        this.#whoIAm.UserId,
        "systemuserroles_association",
        "roles",
        roleId
      );
      this.#util.appendMessage(
        "Security Role 'Example Security Role' associated with to your user account."
      );
    } catch (e) {
      this.#util.showError(
        `Failed to associate role with id: ${roleId} to user with id: ${
          this.#whoIAm.UserId
        }: ${e.message}`
      );
      throw e;
    }
  }

  // Verifies that the role is associated with the user
  async #retrieveRelatedRole(roleId) {
    try {
      const roles = await this.#client.RetrieveMultiple(
        `systemusers(${this.#whoIAm.UserId})/systemuserroles_association`,
        `$select=name&$filter=roleid eq ${roleId}&$top=1`
      );
      if (roles.value.length > 0) {
        this.#util.appendMessage(`Role is associated with user.`);
      }
    } catch (e) {
      this.#util.showError(`Failed to retrieve related role: ${e.message}`);
      throw e;
    }
  }

  async #disassociateRoleFromUser(roleId) {
    try {
      await this.#client.Disassociate(
        "systemusers",
        this.#whoIAm.UserId,
        "systemuserroles_association",
        roleId
      );
      this.#util.appendMessage(
        "Security Role 'Example Security Role' disassociated from your user account."
      );
    } catch (e) {
      this.#util.showError(
        `Failed to disassociate role with id: ${roleId} from user with id: ${
          this.#whoIAm.UserId
        }: ${e.message}`
      );
      throw e;
    }
  }

  //#endregion Section 4: Associate and disassociate existing entities

  // Clean up the created records
  async CleanUp() {
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
