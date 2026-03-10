import { Util } from "../scripts/Util.js";
import { DataverseWebAPI as dv } from "../scripts/DataverseWebAPI.js";
import { customizationFile } from "../solutions/IsSystemAdminFunction_1_0_0_0_managed.js";
export class FunctionsAndActions {
   /**
    * @type {dv.Client}
    * @private
    */
   #client; // The DataverseWebAPIClient.Client instance
   #container; // The container element to display messages
   #entityStore = []; // Store for created records to delete at the end of the sample
   #util; // Util instance for utility functions
   #whoIAm; // The current user's information
   #isSystemAdminFunctionSolutionId = null; // ID of the SystemAdminFunction solution
   #name = "Functions and actions"; // Name of the sample

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

         const contosoConsulting = {
            accountcategorycode: 1,
            address1_addresstypecode: 3,
            address1_city: "Redmond",
            address1_country: "USA",
            address1_line1: "123 Maple St.",
            address1_name: "Corporate Headquarters",
            address1_postalcode: "98000",
            address1_shippingmethodcode: 4,
            address1_stateorprovince: "WA",
            address1_telephone1: "555-1234",
            customertypecode: 3,
            description: "Contoso is a business consulting company.",
            emailaddress1: "info@contoso.com",
            industrycode: 7,
            name: "Contoso Consulting",
            numberofemployees: 150,
            ownershipcode: 2,
            preferredcontactmethodcode: 2,
            telephone1: "(425) 555-1234",
         };

         const contosoConsultingId = await this.#client.Create(
            "accounts",
            contosoConsulting
         );
         this.#entityStore.push({
            entitySetName: "accounts",
            id: contosoConsultingId,
            entityName: "account",
            name: contosoConsulting.name,
         });
      } catch (error) {
         this.#util.showError(error.message);
      }

      this.#isSystemAdminFunctionSolutionId =
         await this.#getIsSystemAdminFunctionSolutionId();
      if (!this.#isSystemAdminFunctionSolutionId) {
         this.#util.appendMessage(
            "IsSystemAdmin Function solution is not installed. Installing it now... "
         );
         // Install the IsSystemAdmin Function solution
         await this.#installIsSystemAdminFunctionSolution();

         // Try to retrieve the ID after installing the solution
         this.#isSystemAdminFunctionSolutionId =
            await this.#getIsSystemAdminFunctionSolutionId();
         if (this.#isSystemAdminFunctionSolutionId) {
            this.#entityStore.push({
               entitySetName: "solutions",
               id: this.#isSystemAdminFunctionSolutionId,
               entityName: "solution",
               name: "IsSystemAdmin Function",
            });

            // Pause for 30 seconds to give time for the API to be available
            await new Promise(resolve => setTimeout(resolve, 30000));

            this.#util.appendMessage(
               "Installed IsSystemAdminFunction solution and added it to the entity store:"
            );

         } else {
            this.#util.showError(
               "Failed to install retrieve the ID of the IsSystemAdminFunction solution."
            );
         }
      } else {
         this.#util.appendMessage(
            "IsSystemAdmin Function solution is already installed."
         );
      }

      // Create account to share
      const accountToShare = {
         name: "Account to Share",
      };

      try {
         const accountToShareId = await this.#client.Create(
            "accounts",
            accountToShare
         );
         this.#entityStore.push({
            entitySetName: "accounts",
            id: accountToShareId,
            entityName: "account",
            name: accountToShare.name,
         });
      } catch (error) {
         this.#util.showError(
            "Couldn't create the account record for sharing:" + error.message
         );
      }
   }

   //#region Section 0: Install Solution in Setup

   async #getIsSystemAdminFunctionSolutionId() {
      try {
         const records = await this.#client.RetrieveMultiple(
            "solutions",
            "$select=solutionid&$filter=uniquename eq 'IsSystemAdminFunction'"
         );

         if (records.value.length === 0) {
            return null;
         }
         return records.value[0].solutionid;
      } catch (error) {
         this.#util.showError(
            `Failed to retrieve IsSystemAdminFunction solution ID: ${error.message}`
         );
      }
   }

   async #installIsSystemAdminFunctionSolution() {

      // The customizationFile is a JavaScript object that contains the solution package.
      // It is imported from the IsSystemAdminFunction_1_0_0_0_managed.js file.
      // The solution package is a managed solution that contains the IsSystemAdminFunction custom API.

      const request = new Request(
         new URL(
            "ImportSolution",
            this.#client.apiEndpoint),
         {
            method: "POST",
            headers: {
               "Content-Type": "application/json",
               "Consistency": "Strong"
            },
            body: JSON.stringify({
               OverwriteUnmanagedCustomizations: false,
               PublishWorkflows: false,
               CustomizationFile: customizationFile,
               ImportJobId: "00000000-0000-0000-0000-000000000000",
            }),
         }
      );

      try {
         await this.#client.Send(request);
      } catch (error) {
         this.#util.showError(
            `Failed to install the IsSystemAdminFunction solution: ${error.message}`
         );
      }
   }

   //#endregion Section 0: Install Solution in Setup


   // Run the sample
   async Run() {
      try {
         this.#util.appendMessage("<h2>1: Unbound Function WhoAmI</h2>");
         await this.#whoAmIExample();
         this.#util.appendMessage("<h2>2: Unbound Function FormatAddress</h2>");
         await this.#formatAddressExample();
         this.#util.appendMessage("<h2>3: Unbound Function InitializeFrom</h2>");
         await this.#initializeFromExample();
         this.#util.appendMessage(
            "<h2>4: Unbound Function RetrieveCurrentOrganization</h2>"
         );
         await this.#retrieveCurrentOrganizationExample();
         this.#util.appendMessage(
            "<h2>5: Unbound Function RetrieveTotalRecordCount</h2>"
         );
         await this.#retrieveTotalRecordCountExample();
         this.#util.appendMessage(
            "<h2>6: Bound Function IsSystemAdmin custom API</h2>"
         );
         await this.#isSystemAdminExample();
         this.#util.appendMessage("<h2>7: Unbound Action GrantAccess</h2>");
         await this.#grantAccessExample();
         this.#util.appendMessage("<h2>8: Bound Action AddPrivilegesRole</h2>");
         await this.#addPrivilegesRoleExample();
      } catch (error) {
         this.#util.showError(error.message);
         // Try to clean up even if an error occurs
         await this.CleanUp();
      }
   }




   //#region Section 1: Unbound Function WhoAmI
   // Demonstrates calling the WhoAmI function
   async #whoAmIExample() {
      try {
         // Invoke the WhoAmI function

         const request = new Request(
            new URL(
               "WhoAmI",
               this.#client.apiEndpoint),
            {
               method: "GET"
            });

         const response = await this.#client.Send(request);
         let message = [
            "<a target='_blank' href='https://learn.",
            "microsoft.com/power-apps/developer/data-platform/webapi/reference/",
            "whoami'>WhoAmI function</a> returns the current user's information ",
            "with the properties of the <a target='_blank' href='https://learn.",
            "microsoft.com/power-apps/developer/data-platform/webapi/reference/",
            "whoamiresponse'>WhoAmIResponse complex type</a>:",
         ];

         this.#util.appendMessage(message.join(""));

         const data = await response.json();
         const table = this.#util.createTable(data);
         this.#container.appendChild(table);
      } catch (e) {
         this.#util.showError(`Failed to call WhoAmI: ${e.message}`);
         throw e;
      }
   }
   //#endregion Section 1: Unbound Function WhoAmI

   //#region Section 2: Unbound Function FormatAddress

   async #formatAddressExample() {
      // Function to generate aliases and parameter assignment
      // for a function where all the parameter types are strings.
      function generateStringAliasesAndAssignments(object) {
         const keys = Object.keys(object);
         const values = Object.values(object);

         // Validate that all the values are strings
         for (const value of values) {
            if (typeof value !== "string") {
               throw new Error(
                  "This function requires that all values are strings."
               );
            }
         }

         // Generate aliases
         const aliases = keys
            .map((key, index) => `${key}=@p${index + 1}`)
            .join(",");

         // Generate assigned values
         const assignedValues = keys
            .map((key, index) => `@p${index + 1}='${values[index]}'`)
            .join("&");

         return { aliases, assignedValues };
      }

      // Define the address object
      const address1 = {
         Line1: "123 Maple St.",
         City: "Seattle",
         StateOrProvince: "WA",
         PostalCode: "98007",
         Country: "USA",
      };

      // Call the FormatAddress function
      try {
         const { aliases, assignedValues } =
            generateStringAliasesAndAssignments(address1);

         const request = new Request(
            new URL(
               `FormatAddress(${aliases})?${assignedValues}`.replace(/'/g, "%27"),
               this.#client.apiEndpoint),
            {
               method: "GET"
            });
         const response = await this.#client.Send(request);
         const data = await response.json();

         const message = [
            "<a target='_blank' href='https://learn.",
            "microsoft.com/power-apps/developer/data-platform/webapi/reference/",
            "formataddress'>FormatAddress function</a> returns a formatted address ",
            "with the properties of the <a target='_blank' href='https://learn.",
            "microsoft.com/power-apps/developer/data-platform/webapi/reference/",
            "formataddressresponse'>FormatAddressResponse complex type</a>:",
         ];

         this.#util.appendMessage(message.join(""));

         const addressMessage = [
            "<strong>Formatted US Address:</strong>",
            `<div style="white-space: pre-line;">${data.Address}</div>`
         ];

         this.#util.appendMessage(
            addressMessage.join("")
         );
      } catch (error) {
         this.#util.showError(`Failed to format address1: ${error.message}`);
      }

      // Define a new  address object
      const address2 = {
         Line1: "1-2-3 Sakura",
         City: "Nagoya",
         StateOrProvince: "Aichi",
         PostalCode: "455-2345",
         Country: "JAPAN",
      };

      try {
         const { aliases, assignedValues } =
            generateStringAliasesAndAssignments(address2);
         const request = new Request(
            new URL(
               `FormatAddress(${aliases})?${assignedValues}`.replace(/'/g, "%27"),
               this.#client.apiEndpoint),
            {
               method: "GET"
            });
         const response = await this.#client.Send(request);
         const data = await response.json();

         const addressMessage = [
            "<strong>Formatted US Address:</strong>",
            `<div style="white-space: pre-line;">${data.Address}</div>`
         ];

         this.#util.appendMessage(
            addressMessage.join("")
         );
      } catch (error) {
         this.#util.showError(`Failed to format address2: ${error.message}`);
      }
   }

   //#endregion Section 2: Unbound Function FormatAddress

   //#region Section 3: Unbound Function InitializeFrom

   async #initializeFromExample() {
      // Get the account ID for Contoso Consulting created in SetUp
      const contosoAccountId = this.#entityStore.find(
         (item) => item.name === "Contoso Consulting"
      ).id;

      const aliases = [
         "EntityMoniker=@p1",
         "TargetEntityName=@p2",
         "TargetFieldType=@p3",
      ].join(",");

      const assignedValues = [
         `@p1={'@odata.id':'accounts(${contosoAccountId})'}`,
         "@p2='account'",
         "@p3=Microsoft.Dynamics.CRM.TargetFieldType'ValidForCreate'",
      ].join("&");

      const request = new Request(
         new URL(
            `InitializeFrom(${aliases})?${assignedValues}`.replace(/'/g, "%27"),
            this.#client.apiEndpoint),
         {
            method: "GET"
         });

      // Will set the data for the new record.
      let newaccount = null;
      try {
         // const response = await this.#client.SendRequest(intializeFromRequest);
         const response = await this.#client.Send(request);
         // Check if the response is successful

         let message = [
            "The <a target='_blank' href='https://learn.",
            "microsoft.com/power-apps/developer/data-platform/webapi/reference/",
            "initializefrom'>InitializeFrom function</a> returns a ",
            "<a target='_blank' href='https://learn.microsoft.com/power-apps/",
            "developer/data-platform/webapi/reference/crmbaseentity'>crmbaseentity</a>",
            " object with the following properties that ",
            "provide default values copied from the source record:",
         ];

         this.#util.appendMessage(message.join(""));

         const data = await response.json();
         newaccount = data;
         const table = this.#util.createTable(newaccount, false);
         this.#container.appendChild(table);
      } catch (error) {
         this.#util.showError(`Failed to initialize from: ${error.message}`);
      }

      if (newaccount) {
         // Create a new account using the initialized values
         newaccount.name = "Contoso Consulting Chicago Branch";
         newaccount.address1_city = "Chicago";
         newaccount.address1_line1 = "456 Elm St.";
         newaccount.address1_name = "Chicago Branch Office";
         newaccount.address1_postalcode = "60007";
         newaccount.address1_stateorprovince = "IL";
         newaccount.address1_telephone1 = "(312) 555-3456";
         newaccount.numberofemployees = 12;

         // Remove the ownerid@odata.bind property from the new account object
         // if it exists. This will only occur if this column is mapped.
         // This column should not be mapped.
         // The calling user will be set as the owner of the new record.
         if (newaccount["ownerid@odata.bind"]) {
            delete newaccount["ownerid@odata.bind"];
         }

         // Converts object properties to an array of strings
         // that can be used with $select.
         function getSelectablePropertyNames(obj) {
            const result = {};
            for (const key in obj) {
               if (obj.hasOwnProperty(key)) {
                  if (!key.startsWith("@")) {
                     if (key.endsWith("@odata.bind")) {
                        const newKey = "_" + key.replace(/@odata\.bind$/, "_value");
                        result[newKey] = obj[key];
                     } else {
                        result[key] = obj[key];
                     }
                  }
               }
            }
            return Object.keys(result);
         }

         // Transform the object to remove @odata.bind properties
         const columns = getSelectablePropertyNames(newaccount);

         try {
            const contosoChicago = await this.#client.CreateRetrieve(
               "accounts",
               newaccount,
               `$select=${columns.join(",")}`
            );
            this.#entityStore.push({
               entitySetName: "accounts",
               id: contosoChicago.accountid,
               entityName: "account",
               name: newaccount.name,
            });

            this.#util.appendMessage(
               `New ${contosoChicago.name} account record created.`
            );
            const table = this.#util.createTable(contosoChicago, true);
            this.#container.appendChild(table);
         } catch (error) {
            this.#util.showError(
               `Failed to create new record using payload from initializeFrom message: ${error.message}`
            );
         }
      }
   }

   //#endregion Section 3: Unbound Function InitializeFrom

   //#region Section 4: Unbound Function RetrieveCurrentOrganization

   async #retrieveCurrentOrganizationExample() {
      try {

         const request = new Request(
            new URL(
               "RetrieveCurrentOrganization(AccessType=@p1)?" +
               "@p1=Microsoft.Dynamics.CRM.EndpointAccessType'Default'",
               this.#client.apiEndpoint),
            {
               method: "GET"
            });
         const response = await this.#client.Send(request);
         const data = await response.json();

         let message = [
            "<a target='_blank' ",
            "href='https://learn.microsoft.com/power-apps/developer/",
            "data-platform/webapi/reference/retrievecurrentorganization'>",
            "RetrieveCurrentOrganization function</a> returns the current ",
            "organization information with the properties of the ",
            "<a target='_blank' href='https://learn.microsoft.com/power-apps",
            "/developer/data-platform/webapi/reference/retrievecurrentorganizationresponse'>",
            "RetrieveCurrentOrganizationResponse complex type</a>. ",
            "The <strong>Details</strong> property contains the following information:",
         ];

         this.#util.appendMessage(message.join(""));
         const table = this.#util.createTable(data.Detail);
         this.#container.appendChild(table);
      } catch (error) {
         this.#util.showError(
            `Failed to retrieve current organization: ${error.message}`
         );
      }
   }
   //#endregion Section 4: Unbound Function RetrieveCurrentOrganization

   //#region Section 5: Unbound Function RetrieveTotalRecordCount

   async #retrieveTotalRecordCountExample() {
      let message = [
         "<a target='_blank' ",
         "href='https://learn.microsoft.com/power-apps/developer/data-platform",
         "/webapi/reference/retrievetotalrecordcount'>",
         "RetrieveTotalRecordCount function</a> returns the current ",
         "organization information with the properties of the ",
         "<a target='_blank' href='https://learn.microsoft.com/power-apps/",
         "developer/data-platform/webapi/reference/retrievetotalrecordcountresponse'>",
         "RetrieveTotalRecordCountResponse  complex type</a>. ",
         "The <strong>EntityRecordCountCollection</strong> property contains the following information:",
      ];
      this.#util.appendMessage(message.join(""));

      const request = new Request(
         new URL(
            "RetrieveTotalRecordCount(EntityNames=@p1)?@p1=['account','contact']",
            this.#client.apiEndpoint),
         {
            method: "GET"
         });

      try {
         const response = await this.#client.Send(request);
         const data = await response.json();

         this.#util.appendMessage(
            "The number of records for each table according to RetrieveTotalRecordCount:"
         );

         const keys = data.EntityRecordCountCollection.Keys;
         const values = data.EntityRecordCountCollection.Values;

         const tableRecordCounts = {};
         for (let i = 0; i < keys.length; i++) {
            tableRecordCounts[keys[i]] = values[i];
         }

         const table = this.#util.createTable(tableRecordCounts, false);
         this.#container.appendChild(table);
      } catch (error) {
         this.#util.showError(
            `Failed to retrieve total record count: ${error.message}`
         );
      }
   }

   //#endregion Section 5: Unbound Function RetrieveTotalRecordCount

   //#region Section 6: Bound Function IsSystemAdmin custom API

   async #isSystemAdminExample() {
      let startMessage = [
         "The <strong>sample_IsSystemAdmin</strong> function is a ",
         "custom API that checks if the user has the system administrator role. ",
         "It is bound to the system user table and returns a boolean value ",
         "indicating whether the user is a system administrator.",
         "<br />",
         "This function is contained within a solution called ",
         "<strong>IsSystemAdminFunction</strong> that is installed if it isn't found ",
         "when the samples starts, and is deleted at the end of the sample if it was installed.",
         "The sample calls the <strong>sample_IsSystemAdmin</strong> function for the ",
         "first 10 enabled interactive users in the system who do not have a # ",
         "character in their name and are enabled.",
         "<a target='_blank' href='https://learn.microsoft.com/power-apps/developer/",
         "data-platform/org-service/samples/issystemadmin-customapi-sample-plugin'> ",
         "Learn more about how this custom API was created</a>.",
      ];

      this.#util.appendMessage(startMessage.join(""));

      // Check if the IsSystemAdminFunction solution is installed
      if (!this.#isSystemAdminFunctionSolutionId) {
         this.#util.showError("IsSystemAdminFunction solution is not installed.");
         return;
      }

      // Get top 10 user records that don't start with # character
      const records = await this.#client.RetrieveMultiple(
         "systemusers",
         [
            "$select=systemuserid,fullname",
            "$filter=not contains(fullname,'%23') and accessmode eq 0",
            "$top=10",
         ].join("&")
      );

      // Check if each user is a system admin
      const checkPromises = records.value.map((record) =>
         this.#checkIsSystemAdmin(record)
      );
      const results = await Promise.all(checkPromises);

      let message = [];

      results.forEach(({ record, isSystemAdmin }) => {
         let item = [
            "<li>",
            record.fullname,
            isSystemAdmin ? " <strong>HAS</strong> " : " does not have ",
            "the system administrator role.",
            "</li>",
         ];
         message.push(item.join(""));
      });

      this.#util.appendMessage(message.join(""), null, "ul");
   }

   async #checkIsSystemAdmin(record) {
      if (!record || !record.systemuserid) {
         this.#util.showError("Invalid record or systemuserid.");
         return { record, isSystemAdmin: false };
      }

      const request = new Request(
         new URL(
            `systemusers(${record.systemuserid})/Microsoft.Dynamics.CRM.sample_IsSystemAdmin`,
            this.#client.apiEndpoint),
         {
            method: "GET"
         });

      try {
         const response = await this.#client.Send(request);
         const data = await response.json();
         return { record, isSystemAdmin: data.HasRole };
      } catch (error) {
         this.#util.showError(
            `Failed to check IsSystemAdmin for user ${record.systemuserid}: ${error.message}`
         );
         return { record, isSystemAdmin: false };
      }
   }
   //#endregion Section 6: Bound Function IsSystemAdmin custom API

   //#region Section 7: Unbound Action GrantAccess

   async #grantAccessExample() {
      const startMessage = [
         "Use the <a target='_blank' ",
         "href='https://learn.microsoft.com/power-apps/developer/data-platform/",
         "webapi/reference/grantaccess'>GrantAccess action</a> ",
         "to grant access rights to a record for ",
         "a principal, which means a user or team. ",
         "This unbound action requires a reference to ",
         "the record using the <strong>Target</strong> parameter. ",
         "The <strong>PrincipalAccess</strong> parameter contains ",
         "data about the principal and the access rights to be granted ",
         "using a <a target='_blank' ",
         "href='https://learn.microsoft.com/power-apps/developer/data-platform",
         "/webapi/reference/principalaccess'>PrincipalAccess complex type</a> ",
         "instance.",
      ].join("");

      this.#util.appendMessage(startMessage);

      // Get the ID for "Account to Share" account record created in SetUp
      const accountToShareId = this.#entityStore.find(
         (item) => item.name === "Account to Share"
      ).id;

      // Get an enabled, interactive user other than current user
      let otherUser = null;
      try {
         const records = await this.#client.RetrieveMultiple(
            "systemusers",
            [
               "$select=systemuserid,fullname",
               [
                  "$filter=systemuserid ne ",
                  this.#whoIAm.UserId,
                  " and isdisabled eq false",
                  " and accessmode eq 0",
                  " and not startswith(fullname,'%23')",
               ].join(""),
               "$top=1",
            ].join("&")
         );

         if (records.value.length > 0) {
            otherUser = records.value[0];
         } else {
            this.#util.showError(
               "No other enabled interactive users found in the system. Can't demonstrate the GrantAccess action."
            );
            return;
         }
      } catch (error) {
         this.#util.showError(`Failed to retrieve other user: ${error.message}`);
      }

      if (otherUser && accountToShareId) {
         const accessRights = await this.#retrievePrincipalAccessRequest(
            otherUser.systemuserid,
            accountToShareId
         );

         // Display the access rights
         this.#util.appendMessage(
            [
               otherUser.fullname,
               " has the following access rights to the account record: ",
               accessRights,
            ].join("")
         );

         // Show if the user has DeleteAccess rights
         this.#util.appendMessage(
            [
               otherUser.fullname,
               accessRights.includes("DeleteAccess") ? " has " : " does not have ",
               "DeleteAccess rights to the account record.",
            ].join("")
         );

         // Give them DeleteAccess rights if they don't have it
         if (!accessRights.includes("DeleteAccess")) {
            // Prepare the body for the GrantAccess request
            const grantAccessBody = {
               Target: {
                  accountid: accountToShareId,
                  "@odata.type": "Microsoft.Dynamics.CRM.account",
               },
               PrincipalAccess: {
                  Principal: {
                     systemuserid: otherUser.systemuserid,
                     "@odata.type": "Microsoft.Dynamics.CRM.systemuser",
                  },
                  AccessMask: "DeleteAccess",
               },
            };

            const request = new Request(
               new URL("GrantAccess", this.#client.apiEndpoint),
               {
                  method: "POST",
                  headers: {
                     "Content-Type": "application/json",
                  },
                  body: JSON.stringify(grantAccessBody),
               });

            try {
               // Send the GrantAccess request
               await this.#client.Send(request);
               this.#util.appendMessage(
                  `Granted DeleteAccess rights to ${otherUser.fullname} for the account record.`
               );
            } catch (error) {
               this.#util.showError(`Failed to grant access: ${error.message}`);
            }

            // Retrieve the updated access rights
            const updatedAccessRights = await this.#retrievePrincipalAccessRequest(
               otherUser.systemuserid,
               accountToShareId
            );

            if (updatedAccessRights.includes("DeleteAccess")) {
               this.#util.appendMessage(
                  `${otherUser.fullname} DeleteAccess rights to the account record is confirmed.`
               );
            } else {
               this.#util.appendMessage(
                  `${otherUser.fullname} still does not have DeleteAccess rights to the account record.`
               );
            }
         } else {
            this.#util.appendMessage(
               `${otherUser.fullname} already has DeleteAccess rights to the account record.`
            );
         }
      }
   }

   /**
    * Retrieves the principal access rights for a given system user and account.
    *
    * @param {string} systemUserId - The ID of the system user.
    * @param {string} accountid - The ID of the account.
    * @returns {Promise<string>} A promise that resolves to the access rights of the principal.
    * @throws Will throw an error if the request fails.
    * @private
    */
   async #retrievePrincipalAccessRequest(systemUserId, accountid) {

      const request = new Request(
         new URL(
            `systemusers(${systemUserId})/Microsoft.Dynamics.CRM.RetrievePrincipalAccess` +
            `(Target=@p1)?@p1={ '@odata.id':'accounts(${accountid})'}`,
            this.#client.apiEndpoint),
         {
            method: "GET"
         });

      try {
         const response = await this.#client.Send(request);
         const data = await response.json();
         return data.AccessRights;
      } catch (error) {
         this.#util.showError(
            `Failed to retrieve principal access for user ${systemUserId} and account ${accountid}: ${error.message}`
         );
      }
   }

   //#endregion Section 7: Unbound Action GrantAccess

   //#region Section 8: Bound Action AddPrivilegesRole

   async #addPrivilegesRoleExample() {
      const startMessage = [
         "Use the <a target='_blank' href='https://learn.microsoft.",
         "com/power-apps/developer/data-platform/webapi/reference/",
         "addprivilegesrole'>AddPrivilegesRole action</a> to add privileges to a security role. ",
         "This action is bound to the <a target='_blank' href='htt",
         "ps://learn.microsoft.com/power-apps/developer/data-platfo",
         "rm/webapi/reference/role'>role entity type</a>. ",
         "The <strong>Target</strong> parameter contains a reference to the record.",
         "The <strong>Privileges</strong> parameter contains data ",
         "about the privileges to be added.",
         "Use a collection of <a target='_blank' href='https://lea",
         "rn.microsoft.com/power-apps/developer/data-platform/webap",
         "i/reference/roleprivilege'>RolePrivilege complex type</a> ",
         "instances. to set the privileges.",
         "RolePrivilege are added to the role in the context of a business unit,",
         " in this case the user's business unit. ",
         "Each RolePrivilege must have a <strong>Depth</strong> assigned using <a targ",
         "et='_blank' href='https://learn.microsoft.com/power-apps/",
         "developer/data-platform/webapi/reference/privilegedepth'>",
         "PrivilegeDepth enum type</a> values.",
         "The Depth value is set to <strong>Basic</strong> in this ",
         "sample.",
      ].join("");
      this.#util.appendMessage(startMessage);

      // Create a security role to add privileges to
      const role = {
         "businessunitid@odata.bind": `businessunits(${this.#whoIAm.BusinessUnitId
            })`,
         name: "Test Role",
      };

      let roleId = null;
      try {
         roleId = await this.#client.Create("roles", role);
         // To delete later
         this.#entityStore.push({
            entitySetName: "roles",
            id: roleId,
            entityName: "role",
            name: role.name,
         });

         this.#util.appendMessage(`Created a security role named ${role.name}.`);
      } catch (error) {
         this.#util.showError(`Failed to create security role: ${error.message}`);
      }

      if (roleId) {
         try {
            // Show the current privileges for the role
            await this.#showRolePrivileges(roleId, role.name);
         } catch (error) {
            this.#util.showError(error.message);
         }

         // Retrieve the prvCreateAccount and prvReadAccount privileges

         try {
            const privileges = await this.#client.RetrieveMultiple(
               "privileges",
               ["$select=name",
                  "$filter=name eq 'prvCreateAccount' or name eq 'prvReadAccount'",
               ].join("&")
            );

            let rolePrivileges = [];

            privileges.value.forEach((privilege) => {
               rolePrivileges.push({
                  BusinessUnitId: this.#whoIAm.BusinessUnitId,
                  Depth: "Basic",
                  PrivilegeId: privilege.privilegeid,
                  PrivilegeName: privilege.name,
               });
            });

            const request = new Request(
               new URL(
                  `roles(${roleId})/Microsoft.Dynamics.CRM.AddPrivilegesRole`,
                  this.#client.apiEndpoint),
               {
                  method: "POST",
                  headers: {
                     "Content-Type": "application/json",
                  },
                  body: JSON.stringify({ Privileges: rolePrivileges }),
               });

            // Send the request to add the privileges
            try {
               await this.#client.Send(request);

               this.#util.appendMessage(
                  `Added the 'prvCreateAccount' and 'prvReadAccount' privileges to the ${role.name} security role:`
               );

               try {
                  // Show the updated privileges for the role
                  await this.#showRolePrivileges(roleId, role.name);
               } catch (error) {
                  this.#util.showError(error.message);
               }
            } catch (error) {
               this.#util.showError(
                  `Failed to add privileges to the security role: ${error.message}`
               );
            }
         } catch (error) {
            this.#util.showError(
               `Failed to retrieve 'prvCreateAccount' and 'prvReadAccount' privileges: ${error.message}`
            );
         }
      } else {
         this.#util.showError(
            "Failed to create security role. Cannot add privileges."
         );
         return;
      }
   }

   async #showRolePrivileges(roleId, name) {
      // Get the roles currently associated with the role

      try {
         const rolePrivileges = await this.#client.RetrieveMultiple(
            `roles(${roleId})/roleprivileges_association`,
            "$select=name"
         );

         this.#util.appendMessage(
            `The ${name} security role has the following ${rolePrivileges.value.length} privileges:`
         );
         let list = [];
         rolePrivileges.value.forEach((privilege) => {
            list.push(`<li>${privilege.name}</li>`);
         });
         this.#util.appendMessage(list.join(""), null, "ul");
      } catch (error) {
         throw new Error(
            `Failed to retrieve privileges for role ${roleId}: ${error.message}`
         );
      }
   }

   //#endregion Section 8: Bound Action AddPrivilegesRole

   // Clean up the created records
   async CleanUp() {
      if (this.#entityStore.length === 0) {
         this.#util.appendMessage("No records to delete");
         return;
      }

      this.#util.appendMessage("<h2>9: Delete sample records</h2>");
      this.#util.appendMessage("Deleting the records created by this sample:");

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
