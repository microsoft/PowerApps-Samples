import { Util } from "../scripts/Util.js";
import { DataverseWebAPI as dv } from "../scripts/DataverseWebAPI.js";
export class QueryDataSample {
  /**
   * @type {dv.Client}
   * @private
   */
  #client; // The DataverseWebAPIClient instance
  #container; // The container element to display messages
  #entityStore = []; // Store for created records to delete at the end of the sample
  #util; //Common functions for samples
  #name = "Query data";
  #contosoAccountId; // Store for the account ID
  #contactYvonneId; // Store for the primary contact ID

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
    this.#util.appendMessage(this.#name + " sample started...");

    // Section 0: Create records to query
    const contosoAccount = {
      name: "Contoso, Ltd. (sample)",
      Account_Tasks: [
        {
          subject: "Task 1 for Contoso, Ltd.",
          description: "Task 1 for Contoso, Ltd. description",
          actualdurationminutes: 10,
        },
        {
          subject: "Task 2 for Contoso, Ltd.",
          description: "Task 2 for Contoso, Ltd. description",
          actualdurationminutes: 10,
        },
        {
          subject: "Task 3 for Contoso, Ltd.",
          description: "Task 3 for Contoso, Ltd. description",
          actualdurationminutes: 10,
        },
      ],
      primarycontactid: {
        firstname: "Yvonne",
        lastname: "McKay (sample)",
        jobtitle: "Coffee Master",
        annualincome: 45000,
        Contact_Tasks: [
          {
            subject: "Task 1 for Yvonne McKay",
            description: "Task 1 for Yvonne McKay description",
            actualdurationminutes: 5,
          },
          {
            subject: "Task 2 for Yvonne McKay",
            description: "Task 2 for Yvonne McKay description",
            actualdurationminutes: 5,
          },
          {
            subject: "Task 3 for Yvonne McKay",
            description: "Task 3 for Yvonne McKay description",
            actualdurationminutes: 5,
          },
        ],
      },
      contact_customer_accounts: [
        {
          firstname: "Susanna",
          lastname: "Stubberod (sample)",
          jobtitle: "Senior Purchaser",
          annualincome: 52000,
          Contact_Tasks: [
            {
              subject: "Task 1 for Susanna Stubberod",
              description: "Task 1 for Susanna Stubberod description",
              actualdurationminutes: 3,
            },
            {
              subject: "Task 2 for Susanna Stubberod",
              description: "Task 2 for Susanna Stubberod description",
              actualdurationminutes: 3,
            },
            {
              subject: "Task 3 for Susanna Stubberod",
              description: "Task 3 for Susanna Stubberod description",
              actualdurationminutes: 3,
            },
          ],
        },
        {
          firstname: "Nancy",
          lastname: "Anderson (sample)",
          jobtitle: "Activities Manager",
          annualincome: 55500,
          Contact_Tasks: [
            {
              subject: "Task 1 for Nancy Anderson",
              description: "Task 1 for Nancy Anderson description",
              actualdurationminutes: 4,
            },
            {
              subject: "Task 2 for Nancy Anderson",
              description: "Task 2 for Nancy Anderson description",
              actualdurationminutes: 4,
            },
            {
              subject: "Task 3 for Nancy Anderson",
              description: "Task 3 for Nancy Anderson description",
              actualdurationminutes: 4,
            },
          ],
        },
        {
          firstname: "Maria",
          lastname: "Cambell (sample)",
          jobtitle: "Accounts Manager",
          annualincome: 31000,
          Contact_Tasks: [
            {
              subject: "Task 1 for Maria Cambell",
              description: "Task 1 for Maria Cambell description",
              actualdurationminutes: 5,
            },
            {
              subject: "Task 2 for Maria Cambell",
              description: "Task 2 for Maria Cambell description",
              actualdurationminutes: 5,
            },
            {
              subject: "Task 3 for Maria Cambell",
              description: "Task 3 for Maria Cambell description",
              actualdurationminutes: 5,
            },
          ],
        },
        {
          firstname: "Scott",
          lastname: "Konersmann (sample)",
          jobtitle: "Accounts Manager",
          annualincome: 38000,
          Contact_Tasks: [
            {
              subject: "Task 1 for Scott Konersmann",
              description: "Task 1 for Scott Konersmann description",
              actualdurationminutes: 6,
            },
            {
              subject: "Task 2 for Scott Konersmann",
              description: "Task 2 for Scott Konersmann description",
              actualdurationminutes: 6,
            },
            {
              subject: "Task 3 for Scott Konersmann",
              description: "Task 3 for Scott Konersmann description",
              actualdurationminutes: 6,
            },
          ],
        },
        {
          firstname: "Robert",
          lastname: "Lyon (sample)",
          jobtitle: "Senior Technician",
          annualincome: 78000,
          Contact_Tasks: [
            {
              subject: "Task 1 for Robert Lyon",
              description: "Task 1 for Robert Lyon description",
              actualdurationminutes: 7,
            },
            {
              subject: "Task 2 for Robert Lyon",
              description: "Task 2 for Robert Lyon description",
              actualdurationminutes: 7,
            },
            {
              subject: "Task 3 for Robert Lyon",
              description: "Task 3 for Robert Lyon description",
              actualdurationminutes: 7,
            },
          ],
        },
        {
          firstname: "Paul",
          lastname: "Cannon (sample)",
          jobtitle: "Ski Instructor",
          annualincome: 68500,
          Contact_Tasks: [
            {
              subject: "Task 1 for Paul Cannon",
              description: "Task 1 for Paul Cannon description",
              actualdurationminutes: 8,
            },
            {
              subject: "Task 2 for Paul Cannon",
              description: "Task 2 for Paul Cannon description",
              actualdurationminutes: 8,
            },
            {
              subject: "Task 3 for Paul Cannon",
              description: "Task 3 for Paul Cannon description",
              actualdurationminutes: 8,
            },
          ],
        },
        {
          firstname: "Rene",
          lastname: "Valdes (sample)",
          jobtitle: "Data Analyst III",
          annualincome: 86000,
          Contact_Tasks: [
            {
              subject: "Task 1 for Rene Valdes",
              description: "Task 1 for Rene Valdes description",
              actualdurationminutes: 9,
            },
            {
              subject: "Task 2 for Rene Valdes",
              description: "Task 2 for Rene Valdes description",
              actualdurationminutes: 9,
            },
            {
              subject: "Task 3 for Rene Valdes",
              description: "Task 3 for Rene Valdes description",
              actualdurationminutes: 9,
            },
          ],
        },
        {
          firstname: "Jim",
          lastname: "Glynn (sample)",
          jobtitle: "Senior International Sales Manager",
          annualincome: 81400,
          Contact_Tasks: [
            {
              subject: "Task 1 for Jim Glynn",
              description: "Task 1 for Jim Glynn description",
              actualdurationminutes: 10,
            },
            {
              subject: "Task 2 for Jim Glynn",
              description: "Task 2 for Jim Glynn description",
              actualdurationminutes: 10,
            },
            {
              subject: "Task 3 for Jim Glynn",
              description: "Task 3 for Jim Glynn description",
              actualdurationminutes: 10,
            },
          ],
        },
      ],
    };
    // Create the records that are all related to the account
    this.#contosoAccountId = await this.#createRecords(contosoAccount);
    // Add the primary contact to the entity store
    this.#contactYvonneId = await this.#addPrimaryContactToEntityStore(
      this.#contosoAccountId
    );
  }

  //#region Section 0: Create records to query

  // Create records to query
  async #createRecords(contosoAccount) {
    try {
      const contosoAccountId = await this.#client.Create(
        "accounts",
        contosoAccount
      );
      this.#util.appendMessage("Created records for this sample");
      // To delete later
      this.#entityStore.push({
        name: `${contosoAccount.name}`,
        entityName: "account",
        entitySetName: "accounts",
        id: contosoAccountId,
      });
      return contosoAccountId;
    } catch (e) {
      this.#util.showError("Failed to create sample records:" + e.message);
      throw e;
    }
  }

  // Add the primary contact to the entity store
  async #addPrimaryContactToEntityStore(contosoAccountId) {
    try {
      const contoso = await this.#client.Retrieve(
        "accounts",
        contosoAccountId,
        "$select=accountid&$expand=primarycontactid($select=contactid,fullname)"
      );
      // To delete later
      this.#entityStore.push({
        name: `${contoso.primarycontactid.fullname}`,
        entityName: "contact",
        entitySetName: "contacts",
        id: contoso.primarycontactid.contactid,
      });

      return contoso.primarycontactid.contactid;
    } catch (e) {
      this.#util.showError(
        "Failed to add primary contact to entity store:" + e.message
      );
      throw e;
    }
  }
  //#endregion Section 0: Create records to query

  // Run the sample
  async Run() {
    try {
      // Section 1: Select specific properties
      this.#util.appendMessage(
        "<h2>Section 1: Select specific properties</h2>"
      );

      await this.#selectSpecificProperties();

      // Section 2: Use query functions
      this.#util.appendMessage("<h2>Section 2: Use query functions</h2>");
      // standard query functions
      await this.#retrieveContactsWhereFullNameContainsSample();
      // Dataverse query functions
      await this.#retrieveContactsCreatedInLastHour();
      // Use operators
      await this.#retrieveHighIncomeContacts();
      // Set Set precedence
      await this.#retrieveHighIncomeSeniorOrManagerContacts();
      // Section 3: Ordering and aliases
      this.#util.appendMessage("<h2>Section 3: Ordering and aliases</h2>");
      // Order results
      await this.#retrieveContosoContactsOrderedByAnnualIncomeAndTitle();
      // Parameter alias
      await this.#demonstrateParameterAliases();
      // Section 4: Limit and count results
      this.#util.appendMessage("<h2>Section 4: Limit and count results</h2>");
      // Top results
      await this.#getTop5Contacts();
      // Collection count
      await this.#getContactCount();
      // Result count
      await this.#getCountOfFilteredCollection();
      // Section 5: Pagination
      this.#util.appendMessage("<h2>Section 5: Pagination</h2>");
      const results = await this.#retrievePageOfFourContacts();
      await this.#showNextPageOfContacts(results);
      // Section 6: Expand results
      this.#util.appendMessage("<h2>Section 6: Expand results</h2>");
      // Expand on single-valued navigation property
      await this.#retrieveAccountWithPrimaryContact();
      // Expand on partner property
      await this.#retrieveContactWithAccounts();
      // Expand on collection-valued navigation property
      await this.#retrieveAccountContacts();
      // Expand on multiple navigation properties
      await this.#retrieveAccountContactsAndTasks();
      // Multi-level expands
      await this.#retrieveMultiLevelExpands();
      // Section 7: Aggregate results
      this.#util.appendMessage("<h2>Section 7: Aggregate results</h2>");
      await this.#retrieveAnnualIncomeAggregates();
      // Section 8: FetchXML queries
      this.#util.appendMessage("<h2>Section 8: FetchXML queries</h2>");
      await this.#retrieveContactsWithFetchXml();
      await this.#retrieveFirstPageOfContactsWithFetchXml();
      // Section 9: Use predefined queries
      this.#util.appendMessage("<h2>Section 9: Use predefined queries</h2>");
      await this.#getSavedQueryResults();
      const userQueryId = await this.#createUserQuery();
      await this.#showUserQueryResults(userQueryId);
    } catch (error) {
      this.#util.showError(error.message);
      // Try to clean up even if an error occurs
      await this.CleanUp();
    }
  }

  //#region Section 1: Select specific properties

  async #selectSpecificProperties() {
    const columns = ["fullname", "jobtitle", "annualincome"];

    const query = "$select=" + columns.join(",");

    try {
      const contact = await this.#client.Retrieve(
        "contacts",
        this.#contactYvonneId,
        query
      );
      this.#util.appendMessage("<strong>Selected specific properties</strong>");
      this.#util.appendMessage(
        "<pre>contacts(" + this.#contactYvonneId + ")?" + query + "</pre>"
      );
      const table = this.#util.createTable(contact);
      this.#container.appendChild(table);
    } catch (e) {
      this.#util.showError("Failed to retrieve the data:" + e.message);
      throw e;
    }
  }

  //#endregion Section 1: Select specific properties

  //#region Section 2: Use query functions

  // Standard query functions
  async #retrieveContactsWhereFullNameContainsSample() {
    const columns = ["fullname", "jobtitle", "annualincome"];

    const filters = [
      "contains(fullname,'(sample)')",
      "_parentcustomerid_value eq " + this.#contosoAccountId,
    ];

    const query =
      "$select=" + columns.join(",") + "&$filter=" + filters.join(" and ");

    try {
      const contacts = await this.#client.RetrieveMultiple("contacts", query);
      this.#util.appendMessage(
        "<strong>Contoso contacts where full name contains (sample):</strong>"
      );
      this.#util.appendMessage("<pre>contacts?" + query + "</pre>");
      const table = this.#util.createListTable(contacts, columns);
      this.#container.appendChild(table);
    } catch (e) {
      this.#util.showError("Failed to retrieve contacts.");
      throw e;
    }
  }

  //Dataverse query functions
  async #retrieveContactsCreatedInLastHour() {
    const columns = ["fullname", "jobtitle", "annualincome", "createdon"];

    const filters = [
      "Microsoft.Dynamics.CRM.LastXHours(PropertyName=@p1,PropertyValue=@p2)",
      "_parentcustomerid_value eq " + this.#contosoAccountId,
    ];

    const parameters = [
      "$select=" + columns.join(","),
      "$filter=" + filters.join(" and "),
      "@p1='createdon'",
      "@p2='1'",
    ];

    const query = parameters.join("&");

    try {
      const contacts = await this.#client.RetrieveMultiple("contacts", query);
      this.#util.appendMessage(
        "<strong>Contoso contacts created in the last hour:<strong>"
      );
      this.#util.appendMessage("<pre>contacts?" + query + "</pre>");
      const table = this.#util.createListTable(contacts, columns);
      this.#container.appendChild(table);
    } catch (e) {
      this.#util.showError("Failed to retrieve contacts.");
      throw e;
    }
  }

  // Use operators
  async #retrieveHighIncomeContacts() {
    const columns = ["fullname", "jobtitle", "annualincome"];

    const filters = [
      "contains(fullname,'(sample)')",
      "annualincome gt 50000",
      "_parentcustomerid_value eq " + this.#contosoAccountId,
    ];

    const parameters = [
      "$select=" + columns.join(","),
      "$filter=" + filters.join(" and "),
    ];

    const query = parameters.join("&");

    try {
      const contacts = await this.#client.RetrieveMultiple("contacts", query);
      this.#util.appendMessage(
        "<strong>Contoso contacts with income greater than $50,000 :<strong>"
      );
      this.#util.appendMessage("<pre>contacts?" + query + "</pre>");
      const table = this.#util.createListTable(contacts, columns);
      this.#container.appendChild(table);
    } catch (e) {
      this.#util.showError("Failed to retrieve contacts.");
      throw e;
    }
  }

  // Set precedence

  async #retrieveHighIncomeSeniorOrManagerContacts() {
    const columns = ["fullname", "jobtitle", "annualincome"];

    const OrFilters = [
      "contains(jobtitle, 'senior')",
      "contains(jobtitle, 'manager')",
    ];

    const filters = [
      "contains(fullname,'(sample)')",
      "(" + OrFilters.join(" or ") + ")",
      "annualincome gt 50000",
      "_parentcustomerid_value eq " + this.#contosoAccountId,
    ];

    const parameters = [
      "$select=" + columns.join(","),
      "$filter=" + filters.join(" and "),
    ];

    const query = parameters.join("&");

    try {
      const contacts = await this.#client.RetrieveMultiple("contacts", query);
      this.#util.appendMessage(
        "<strong>Contoso contacts with Senior or Manager titles and income greater than $50,000 :</strong>"
      );
      this.#util.appendMessage("<pre>contacts?" + query + "</pre>");
      const table = this.#util.createListTable(contacts, columns);
      this.#container.appendChild(table);
    } catch (e) {
      this.#util.showError("Failed to retrieve contacts.");
      throw e;
    }
  }

  //#endregion Section 2: Use query functions

  //#region Section 3: Ordering and aliases

  // Order results
  async #retrieveContosoContactsOrderedByAnnualIncomeAndTitle() {
    const columns = ["fullname", "jobtitle", "annualincome"];

    const filters = [
      "contains(fullname,'(sample)')",
      "_parentcustomerid_value eq " + this.#contosoAccountId,
    ];

    const orders = ["annualincome desc", "jobtitle asc"];

    const parameters = [
      "$select=" + columns.join(","),
      "$filter=" + filters.join(" and "),
      "$orderby=" + orders.join(","),
    ];

    const query = parameters.join("&");

    try {
      const contacts = await this.#client.RetrieveMultiple("contacts", query);
      this.#util.appendMessage(
        "<strong>Contoso contacts ordered by annual income and title:</strong>"
      );
      this.#util.appendMessage("<pre>contacts?" + query + "</pre>");
      const table = this.#util.createListTable(contacts, columns);
      this.#container.appendChild(table);
    } catch (e) {
      this.#util.showError("Failed to retrieve contacts.");
      throw e;
    }
  }

  // Parameter alias
  async #demonstrateParameterAliases() {
    const columns = ["fullname", "jobtitle", "annualincome"];

    const filters = ["contains(@p1,'(sample)')", "@p2 eq @p3"];

    const orders = ["@p4 asc", "@p5 desc"];

    const aliases = [
      "@p1=fullname",
      "@p2=_parentcustomerid_value",
      "@p3=" + this.#contosoAccountId,
      "@p4=jobtitle",
      "@p5=annualincome",
    ];

    const parameters = [
      "$select=" + columns.join(","),
      "$filter=" + filters.join(" and "),
      "$orderby=" + orders.join(","),
      aliases.join("&"),
    ];

    const query = parameters.join("&");

    try {
      const contacts = await this.#client.RetrieveMultiple("contacts", query);
      this.#util.appendMessage(
        "<strong>Contoso contacts ordered by annual income and title using parameter aliases:</strong>"
      );
      this.#util.appendMessage("<pre>contacts?" + query + "</pre>");
      const table = this.#util.createListTable(contacts, columns);
      this.#container.appendChild(table);
    } catch (e) {
      this.#util.showError("Failed to retrieve contacts.");
      throw e;
    }
  }

  //#endregion Section 3: Ordering and aliases

  //#region Section 4: Limit and count results

  // Top results
  async #getTop5Contacts() {
    const columns = ["fullname", "jobtitle", "annualincome"];

    const filters = [
      "contains(fullname,'(sample)')",
      "_parentcustomerid_value eq " + this.#contosoAccountId,
    ];

    const parameters = [
      "$select=" + columns.join(","),
      "$filter=" + filters.join(" and "),
      "$top=5",
    ];

    const query = parameters.join("&");

    try {
      const contacts = await this.#client.RetrieveMultiple("contacts", query);
      this.#util.appendMessage("<strong>Top 5 contacts:</strong>");
      this.#util.appendMessage("<pre>contacts?" + query + "</pre>");
      const table = this.#util.createListTable(contacts, columns);
      this.#container.appendChild(table);
    } catch (e) {
      this.#util.showError("Failed to retrieve top 5 contacts.");
      throw e;
    }
  }

  // Collection count
  async #getContactCount() {
    try {
      const number = await this.#client.GetCollectionCount(
        "accounts(" + this.#contosoAccountId + ")/contact_customer_accounts"
      );
      this.#util.appendMessage(
        `<strong>Contoso contact count: ${number}</strong>`
      );
    } catch (e) {
      this.#util.showError("Failed to retrieve contact count.");
      throw e;
    }
  }

  // Result count

  async #getCountOfFilteredCollection() {
    const columns = ["fullname", "jobtitle", "annualincome"];

    const OrFilters = [
      "contains(jobtitle, 'senior')",
      "contains(jobtitle, 'manager')",
    ];

    const filters = [
      "contains(fullname,'(sample)')",
      "(" + OrFilters.join(" or ") + ")",
      "annualincome gt 50000",
      "_parentcustomerid_value eq " + this.#contosoAccountId,
    ];

    const parameters = [
      "$select=" + columns.join(","),
      "$filter=" + filters.join(" and "),
      "$count=true",
    ];

    const query = parameters.join("&");

    try {
      const contacts = await this.#client.RetrieveMultiple("contacts", query);
      this.#util.appendMessage(
        `<strong>Contact result count: ${contacts["@odata.count"]}</strong>`
      );
      this.#util.appendMessage("<pre>contacts?" + query + "</pre>");
    } catch (e) {
      this.#util.showError("Failed to retrieve contact count.");
      throw e;
    }
  }

  //#endregion Section 4: Limit and count results

  //#region Section 5: Pagination

  async #retrievePageOfFourContacts() {
    const columns = ["fullname", "jobtitle", "annualincome"];

    const filters = [
      "contains(fullname,'(sample)')",
      "_parentcustomerid_value eq " + this.#contosoAccountId,
    ];

    const parameters = [
      "$select=" + columns.join(","),
      "$filter=" + filters.join(" and "),
    ];

    const query = parameters.join("&");

    try {
      const contacts = await this.#client.RetrieveMultiple(
        "contacts",
        query,
        4
      );
      this.#util.appendMessage("<strong>Page of 4 contacts:</strong>");
      this.#util.appendMessage("<pre>contacts?" + query + "</pre>");
      const table = this.#util.createListTable(contacts, columns);
      this.#container.appendChild(table);
      return contacts;
    } catch (e) {
      this.#util.showError("Failed to retrieve first page of 4 contacts.");
      throw e;
    }
  }

  async #showNextPageOfContacts(results) {
    try {
      const nextLink = results["@odata.nextLink"];
      if (!nextLink) {
        this.#util.appendMessage("<strong>No more pages of contacts.</strong>");
        return;
      }

      // The GetNextLink function
      const nextPageResults = await this.#client.GetNextLink(nextLink, 4);

      this.#util.appendMessage("<strong>Next page of 4 contacts:</strong>");
      const table = this.#util.createListTable(nextPageResults, [
        "fullname",
        "jobtitle",
        "annualincome",
      ]);
      this.#container.appendChild(table);
    } catch (error) {
      this.#util.showError("Failed to retrieve next page of contacts.");
      throw error;
    }
  }

  //#endregion Section 5: Pagination

  //#region Section 6: Expand results

  // Expand on single-valued navigation property

  async #retrieveAccountWithPrimaryContact() {
    const columns = ["name"];

    const expands = [
      "primarycontactid($select=fullname,jobtitle,annualincome)",
    ];

    const parameters = [
      "$select=" + columns.join(","),
      "$expand=" + expands.join(","),
    ];
    const query = parameters.join("&");

    try {
      const account = await this.#client.Retrieve(
        "accounts",
        this.#contosoAccountId,
        query
      );
      this.#util.appendMessage(
        `<strong>Account ${account.name} has the following primary contact person:</strong>`
      );
      this.#util.appendMessage(
        "<pre>accounts(" + this.#contosoAccountId + ")?" + query + "</pre>"
      );
      const table = this.#util.createTable(account.primarycontactid);
      this.#container.appendChild(table);
    } catch (e) {
      this.#util.showError("Failed to retrieve account with primary contact.");
      throw e;
    }
  }

  async #retrieveContactWithAccounts() {
    const columns = ["fullname", "jobtitle", "annualincome"];

    const expands = ["account_primary_contact($select=name)"];

    const parameters = [
      "$select=" + columns.join(","),
      "$expand=" + expands.join(","),
    ];
    const query = parameters.join("&");

    try {
      const contact = await this.#client.Retrieve(
        "contacts",
        this.#contactYvonneId,
        query
      );
      this.#util.appendMessage(
        `<strong>Contact ${contact.fullname} is associated with the following accounts:</strong>`
      );
      this.#util.appendMessage(
        "<pre>contacts(" + this.#contactYvonneId + ")?" + query + "</pre>"
      );
      for (const account of contact.account_primary_contact) {
        const table = this.#util.createTable(account);
        this.#container.appendChild(table);
      }
    } catch (e) {
      this.#util.showError("Failed to retrieve contact with accounts.");
      throw e;
    }
  }

  // Expand on collection-valued navigation property

  async #retrieveAccountContacts() {
    const accountColumns = ["name"];

    const contactColumns = ["fullname", "jobtitle", "annualincome"];

    const expands = [
      "contact_customer_accounts($select=" + contactColumns.join(",") + ")",
    ];

    const parameters = [
      "$select=" + accountColumns.join(","),
      "$expand=" + expands.join(","),
    ];
    const query = parameters.join("&");

    try {
      const account = await this.#client.Retrieve(
        "accounts",
        this.#contosoAccountId,
        query
      );
      this.#util.appendMessage(
        `<strong>Account ${account.name} has the following related contacts:</strong>`
      );
      this.#util.appendMessage(
        "<pre>accounts(" + this.#contosoAccountId + ")?" + query + "</pre>"
      );
      // createListTable expects a collection with a value property.
      let collection = {
        value: [],
      };
      for (const contact of account.contact_customer_accounts) {
        collection.value.push(contact);
      }

      const table = this.#util.createListTable(collection, contactColumns);
      this.#container.appendChild(table);
    } catch (e) {
      this.#util.showError("Failed to retrieve account contacts.");
      throw e;
    }
  }

  // Expand on multiple navigation properties

  async #retrieveAccountContactsAndTasks() {
    const accountColumns = ["name"];

    const contactColumns = ["fullname", "jobtitle", "annualincome"];

    const taskColumns = ["subject", "description"];

    const expands = [
      `primarycontactid($select=${contactColumns.join(",")})`,
      `contact_customer_accounts($select=${contactColumns.join(",")})`,
      `Account_Tasks($select=${taskColumns.join(",")})`,
    ];

    const parameters = [
      "$select=" + accountColumns.join(","),
      "$expand=" + expands.join(","),
    ];
    const query = parameters.join("&");

    try {
      const account = await this.#client.Retrieve(
        "accounts",
        this.#contosoAccountId,
        query
      );
      this.#util.appendMessage(
        `<strong>Account ${account.name} has the following related contacts and tasks:</strong>`
      );
      this.#util.appendMessage(
        "<pre>accounts(" + this.#contosoAccountId + ")?" + query + "</pre>"
      );

      this.#util.appendMessage("<strong>Primary contact:</strong>");
      const primaryContactTable = this.#util.createTable(
        account.primarycontactid
      );
      this.#container.appendChild(primaryContactTable);
      this.#util.appendMessage("<strong>Related contacts:</strong>");

      // createListTable expects a collection with a value property.
      let contactsCollection = {
        value: [],
      };
      for (const contact of account.contact_customer_accounts) {
        contactsCollection.value.push(contact);
      }

      const contactsTable = this.#util.createListTable(
        contactsCollection,
        contactColumns
      );
      this.#container.appendChild(contactsTable);

      this.#util.appendMessage("<strong>Related Tasks:</strong>");

      // createListTable expects a collection with a value property.
      let taskCollection = {
        value: [],
      };
      for (const task of account.Account_Tasks) {
        taskCollection.value.push(task);
      }
      const taskTable = this.#util.createListTable(taskCollection, taskColumns);
      this.#container.appendChild(taskTable);
    } catch (e) {
      this.#util.showError("Failed to retrieve account contacts and tasks.");
      throw e;
    }
  }

  // Multi-level expands
  async #retrieveMultiLevelExpands() {
    const taskColumns = ["subject"];
    const contactColumns = ["fullname"];

    const accountColumns = ["name"];

    const userColumns = ["fullname"];

    const nextedExpands = [
      `regardingobjectid_contact_task($select=${contactColumns.join(",")};`,
      `$expand=parentcustomerid_account($select=${accountColumns.join(",")};`,
      `$expand=createdby($select=${userColumns.join(",")})))`,
    ];

    const filters = [
      `regardingobjectid_contact_task/_accountid_value eq ${
        this.#contosoAccountId
      }`,
    ];

    const parameters = [
      "$select=" + taskColumns.join(","),
      "$expand=" + nextedExpands.join(""),
      "$filter=" + filters.join(" and "),
    ];
    const query = parameters.join("&");
    try {
      const tasks = await this.#client.RetrieveMultiple("tasks", query);

      this.#util.appendMessage(
        "<strong>Tasks related to account Contoso, Ltd. (sample):</strong>"
      );
      this.#util.appendMessage("<pre>tasks?" + query + "</pre>");

      let collection = {
        value: [],
      };
      for (const task of tasks.value) {
        let row = {
          subject: task?.subject,
          contact: task?.regardingobjectid_contact_task?.fullname,
          account:
            task?.regardingobjectid_contact_task?.parentcustomerid_account
              ?.name,
          createdby:
            task?.regardingobjectid_contact_task?.parentcustomerid_account
              ?.createdby?.fullname,
        };
        collection.value.push(row);
      }

      const table = this.#util.createListTable(collection, [
        "subject",
        "contact",
        "account",
        "createdby",
      ]);
      this.#container.appendChild(table);
    } catch (e) {
      this.#util.showError("Failed to retrieve multi-level expands.");
      throw e;
    }
  }

  //#endregion Section 6: Expand results

  //#region Section 7: Aggregate results

  async #retrieveAnnualIncomeAggregates() {
    const aggregates = [
      "annualincome with average as average",
      "annualincome with sum as total",
      "annualincome with min as minimum",
      "annualincome with max as maximum",
    ];

    const columns = ["average", "total", "minimum", "maximum"];

    const parameters = ["$apply=aggregate(" + aggregates.join(",") + ")"];

    // Doesn't need to be an actual entity set name, but the a string that
    // represents a collection of records.
    const entitySetName = `accounts(${
      this.#contosoAccountId
    })/contact_customer_accounts`;
    const query = parameters.join("&");

    try {
      const results = await this.#client.RetrieveMultiple(entitySetName, query);
      this.#util.appendMessage("<strong>Annual income aggregates:</strong>");
      this.#util.appendMessage(
        "<pre>" + entitySetName + "?" + query + "</pre>"
      );
      const table = this.#util.createListTable(results, columns);
      this.#container.appendChild(table);
    } catch (e) {
      this.#util.showError("Failed to retrieve aggregate data.");
      throw e;
    }
  }

  //#endregion Section 7: Aggregate results

  //#region Section 8: FetchXML queries

  async #retrieveContactsWithFetchXml() {
    const fetchXml = `<fetch>
      <entity name='contact'>  
        <attribute name='fullname' />  
        <attribute name='jobtitle' />  
        <attribute name='annualincome' />  
        <order descending='true'  
               attribute='fullname' />  
        <filter type='and'>  
          <condition value='%(sample)%'  
                     attribute='fullname'  
                     operator='like' />
          <condition value='${this.#contosoAccountId}'
                     attribute='parentcustomerid'
                     operator='eq' /> 
        </filter>  
      </entity>  
    </fetch>`;

    try {
      const contacts = await this.#client.FetchXml("contacts", fetchXml);
      this.#util.appendMessage(
        "<strong>Contoso contacts with FetchXML:</strong>"
      );
      this.#util.appendMessage(
        "<pre><code>" + this.#util.escapeXml(fetchXml) + "</code></pre>"
      );
      const table = this.#util.createListTable(contacts, [
        "fullname",
        "jobtitle",
        "annualincome",
      ]);
      this.#container.appendChild(table);
    } catch (e) {
      this.#util.showError("Failed to retrieve contacts with FetchXML.");
      throw e;
    }
  }

  // FetchXML pagination

  async #retrieveFirstPageOfContactsWithFetchXml() {
    // Query to retrieve the first page of three contacts
    const fetchXmlPage1 = `<fetch count='3' page='1'>
      <entity name='contact'>  
        <attribute name='fullname' />  
        <attribute name='jobtitle' />  
        <attribute name='annualincome' />  
        <order descending='true'  
               attribute='fullname' />  
        <filter type='and'>  
          <condition value='%(sample)%'  
                     attribute='fullname'  
                     operator='like' />
          <condition value='${this.#contosoAccountId}'
                     attribute='parentcustomerid'
                     operator='eq' /> 
        </filter>  
      </entity>  
    </fetch>`;

    let contactsPage1 = null;
    try {
      contactsPage1 = await this.#client.FetchXml("contacts", fetchXmlPage1);
      this.#util.appendMessage(
        "<strong>First three contacts with FetchXML:</strong>"
      );
      this.#util.appendMessage(
        "<pre><code>" + this.#util.escapeXml(fetchXmlPage1) + "</code></pre>"
      );
      const table = this.#util.createListTable(contactsPage1, [
        "fullname",
        "jobtitle",
        "annualincome",
      ]);
      this.#container.appendChild(table);
    } catch (e) {
      this.#util.showError(
        "Failed to retrieve first three contacts with FetchXML."
      );
      throw e;
    }

    // Check if there are more pages of results
    if (contactsPage1["@Microsoft.Dynamics.CRM.morerecords"]) {
      const pagingCookie =
        contactsPage1["@Microsoft.Dynamics.CRM.fetchxmlpagingcookie"];

      try {
        await this.#retrieveSecondPageOfContactsWithFetchXml(
          fetchXmlPage1,
          pagingCookie
        );
      } catch (e) {
        this.#util.showError(
          "Failed to retrieve second three contacts with FetchXML." + e.message
        );
        throw e;
      }
    }
  }

  // This example demonstrates simple pagination using the page attribute in FetchXML.
  // https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/page-results?tabs=webapi#simple-paging

  async #retrieveSecondPageOfContactsWithFetchXml(fetchXmlPage1, pagingCookie) {
    // To programmatically change the page attribute in the FetchXML
    // Parse the XML string into a DOM Document

    const xmlDoc = new DOMParser().parseFromString(fetchXmlPage1, "text/xml");

    // Select the root fetch element
    const fetchElement = xmlDoc.getElementsByTagName("fetch")[0];

    // Change the page attribute value
    fetchElement.setAttribute("page", "2");

    // Serialize the updated XML back to a string
    const serializer = new XMLSerializer();

    const fetchXmlPage2 = serializer.serializeToString(xmlDoc);

    try {
      const contactsPage2 = await this.#client.FetchXml(
        "contacts",
        fetchXmlPage2
      );
      this.#util.appendMessage(
        "<strong>Second three contacts with FetchXML:</strong>"
      );
      this.#util.appendMessage(
        "<pre><code>" + this.#util.escapeXml(fetchXmlPage2) + "</code></pre>"
      );
      const table = this.#util.createListTable(contactsPage2, [
        "fullname",
        "jobtitle",
        "annualincome",
      ]);
      this.#container.appendChild(table);
    } catch (e) {
      this.#util.showError(
        "Failed to retrieve second three contacts with FetchXML." + e.message
      );
      throw e;
    }
  }

  //#endregion Section 8: FetchXML queries

  //#region Section 9: Use predefined queries

  async #getSavedQueryResults() {
    let savedqueries = null;
    let activeAccountsSavedQueryId = null;

    try {
      // Get the ID of the Active Accounts query
      savedqueries = await this.#client.RetrieveMultiple(
        "savedqueries",
        "$select=savedqueryid,columnsetxml&$filter=name eq 'Active Accounts'"
      );
    } catch (e) {
      this.#util.showError(
        "Failed to retrieve Active Accounts saved query ID." + e.message
      );
      throw e;
    }

    if (savedqueries.value.length > 0) {
      activeAccountsSavedQueryId = savedqueries.value[0].savedqueryid;
    } else {
      throw new Error("Active Accounts saved query details not found.");
    }

    try {
      // Retrieve first three records using the Active Accounts query
      const results = await this.#client.RetrieveMultiple(
        "accounts",
        `savedQuery=${activeAccountsSavedQueryId}`,
        3
      );

      this.#util.appendMessage(
        "<strong>Active Accounts saved query results:</strong>"
      );

      this.#util.appendMessage(
        "<pre>" + JSON.stringify(results, null, 3) + "</pre>"
      );
    } catch (e) {
      this.#util.showError(
        "Failed to retrieve the results of the Active Accounts saved query." +
          e.message
      );
      throw e;
    }
  }

  async #createUserQuery() {
    const userQuery = {
      name: "My User Query",
      description: "User query to display contact info.",
      querytype: 0,
      returnedtypecode: "contact",
      fetchxml: `<fetch>
          <entity name ='contact'>
              <attribute name ='fullname' />
              <attribute name ='contactid' />
              <attribute name ='jobtitle' />
              <attribute name ='annualincome' />
              <order descending ='false' attribute='fullname' />
              <filter type ='and'>
                  <condition value ='%(sample)%' attribute='fullname' operator='like' />
                  <condition value ='%Manager%' attribute='jobtitle' operator='like' />
                  <condition value ='55000' attribute='annualincome' operator='gt' />
              </filter>
          </entity>
       </fetch>`,
    };

    try {
      const userQueryId = await this.#client.Create("userqueries", userQuery);
      this.#util.appendMessage(
        `<strong>User query created with ID: ${userQueryId}</strong>`
      );
      // To delete later
      this.#entityStore.push({
        name: `${userQuery.name}`,
        entityName: "userquery",
        entitySetName: "userqueries",
        id: userQueryId,
      });

      return userQueryId;
    } catch (e) {
      this.#util.showError("Failed to create user query." + e.message);
      throw e;
    }
  }

  async #showUserQueryResults(userQueryId) {
    try {
      // Retrieve first three records using the user query
      const results = await this.#client.RetrieveMultiple(
        "contacts",
        `userQuery=${userQueryId}`,
        3
      );

      this.#util.appendMessage("<strong>User query results:</strong>");

      this.#util.appendMessage(
        "<pre>" + JSON.stringify(results, null, 3) + "</pre>"
      );
    } catch (e) {
      this.#util.showError(
        "Failed to retrieve the results of the user query." + e.message
      );
      throw e;
    }
  }

  //#endregion Section 9: Use predefined queries

  //#region Section 10: Delete sample records

  // Clean up the created records
  async CleanUp() {
    if (this.#entityStore.length === 0) {
      // No records to delete
      return;
    }
    // Section 10: Delete sample records
    this.#util.appendMessage("<h2>Section 10: Delete sample records</h2>");

    let deleteMessageList = document.createElement("ul");
    this.#container.append(deleteMessageList);

    const requests = [];
    for (const item of this.#entityStore) {
      const request = new Request(
        new URL(`${item.entitySetName}(${item.id})`, this.#client.apiEndpoint),
        {
          method: "DELETE",
        }
      );
      requests.push(request);
    }

    const changeSet = new dv.ChangeSet(this.#client, requests);
    const responses = await this.#client.Batch([changeSet]);

    responses.forEach((response, index) => {
      const message = document.createElement("li");
      const entity = this.#entityStore[index];

      if (response.status === 204) {
        message.textContent = `Deleted ${entity.entityName} ${entity.name}`;
      } else {
        message.textContent = `Failed to delete ${entity.entityName} ${entity.name}`;
        message.className = "error";
      }

      deleteMessageList.append(message);
    });

    this.#util.appendMessage(
      "Related contact records deleted due to cascade delete."
    );

    // Set the entity store to an empty array
    this.#entityStore = [];
    this.#util.appendMessage(this.#name + " sample completed.");
    this.#util.appendMessage("<a href='#'>Go to top</a>");
  }

  //#endregion Section 10: Delete sample records
}
