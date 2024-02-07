// =====================================================================
//  This file is part of the Microsoft Dynamics CRM SDK code samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  This source code is intended only as a supplement to Microsoft
//  Development Tools and/or on-line documentation.  See these other
//  materials for detailed information regarding Microsoft code samples.
//
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//  PARTICULAR PURPOSE.
// =====================================================================

"use strict";
var Sdk = window.Sdk || {};

/**
 * @function getClientUrl
 * @description Get the client URL.
 * @returns {string} The client URL.
 */
Sdk.getClientUrl = function () {
    var context;
    // GetGlobalContext defined by including reference to 
    // ClientGlobalContext.js.aspx in the HTML page.
    if (typeof GetGlobalContext != "undefined") {
        context = GetGlobalContext();
    } else {
        if (typeof Xrm != "undefined") {
            // Xrm.Page.context defined within the Xrm.Page object model for form scripts.
            context = Xrm.Page.context;
        } else {
            throw new Error("Context is not available.");
        }
    }
    return context.getClientUrl();
};

// Global variables
var entitiesToDelete = [];              // Entity URIs to be deleted later (if user so chooses)
var deleteData = true;                  // Controls whether sample data are deleted at the end of sample run
var clientUrl = Sdk.getClientUrl();     // e.g.: https://org.crm.dynamics.com
var webAPIPath = "/api/data/v8.2";      // Path to the web API
var contact1Uri;                        // e.g.: Peter Cambel
var contactAltUri;                      // e.g.: Peter_Alt Cambel
var account1Uri;                        // e.g.: Contoso, Ltd
var account2Uri;                        // e.g.: Fourth Coffee
var contact2Uri;                        // e.g.: Susie Curtis
var opportunity1Uri, competitor1Uri;    // e.g.: Adventure Works
                                                                                        
/**
 * @function request
 * @description Generic helper function to handle basic XMLHttpRequest calls.
 * @param {string} action - The request action. String is case-sensitive.
 * @param {string} uri - An absolute or relative URI. Relative URI starts with a "/".
 * @param {object} data - An object representing an entity. Required for create and update actions.
 * @param {object} addHeader - An object with header and value properties to add to the request
 * @returns {Promise} - A Promise that returns either the request object or an error object.
 */
Sdk.request = function (action, uri, data, addHeader) {
    if (!RegExp(action, "g").test("POST PATCH PUT GET DELETE")) { // Expected action verbs.
        throw new Error("Sdk.request: action parameter must be one of the following: " +
            "POST, PATCH, PUT, GET, or DELETE.");
    }
    if (!typeof uri === "string") {
        throw new Error("Sdk.request: uri parameter must be a string.");
    }
    if ((RegExp(action, "g").test("POST PATCH PUT")) && (!data)) {
        throw new Error("Sdk.request: data parameter must not be null for operations that create or modify data.");
    }
    if (addHeader) {
        if (typeof addHeader.header != "string" || typeof addHeader.value != "string") {
            throw new Error("Sdk.request: addHeader parameter must have header and value properties that are strings.");
        }
    }

    // Construct a fully qualified URI if a relative URI is passed in.
    if (uri.charAt(0) === "/") {
        uri = clientUrl + webAPIPath + uri;
    }

    return new Promise(function (resolve, reject) {
        var request = new XMLHttpRequest();
        request.open(action, encodeURI(uri), true);
        request.setRequestHeader("OData-MaxVersion", "4.0");
        request.setRequestHeader("OData-Version", "4.0");
        request.setRequestHeader("Accept", "application/json");
        request.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        if (addHeader) {
            request.setRequestHeader(addHeader.header, addHeader.value);
        }
        request.onreadystatechange = function () {
            if (this.readyState === 4) {
                request.onreadystatechange = null;
                switch (this.status) {
                    case 200: // Operation success with content returned in response body.
                    case 201: // Create success. 
                    case 204: // Operation success with no content returned in response body.
                        resolve(this);
                        break;
                    default: // All other statuses are unexpected so are treated like errors.
                        var error;
                        try {
                            error = JSON.parse(request.response).error;
                        } catch (e) {
                            error = new Error("Unexpected Error");
                        }
                        reject(error);
                        break;
                }
            }
        };
        request.send(JSON.stringify(data));
    });
};

/**
 * @function startSample
 * @description Runs the sample. 
 * This sample demonstrates basic CRUD+ operations. 
 * Results are sent to the debugger's console window.
 */
Sdk.startSample = function () {
    // Initializing.
    deleteData = document.getElementsByName("removesampledata")[0].checked;
    entitiesToDelete = []; // Reset the array.
    contact1Uri = "";
    account1Uri = "";
    account2Uri = "";
    contact2Uri = "";
    opportunity1Uri = "";
    competitor1Uri = "";

    // Section 1.
    //
    // Create the contact using POST request. 
    // A new entry will be added regardless if a contact with this info already exists in the system or not.
    console.log("--Section 1 started--");
    var contact = {};
    contact.firstname = "Peter";
    contact.lastname = "Cambel";

    var entitySetName = "/contacts";
    
    // Starts chain of operations.
    // Create contact.
    Sdk.request("POST", entitySetName, contact)
    .then(function (request) {
        // Process response from previous request.
        contact1Uri = request.getResponseHeader("OData-EntityId");
        entitiesToDelete.push(contact1Uri); // To delete later
        console.log("Contact 'Peter Cambel' created with URI: %s", contact1Uri);

        // Setup for next request.
        //
        // Update contact.
        // Add property values to a specific contact using PATCH request.
        var contact = {};
        contact.annualincome = 80000.00;
        contact.jobtitle = "Junior Developer";
        return Sdk.request("PATCH", contact1Uri, contact)
    })
    .then(function () {
        // Process response from previous request.
        console.log("Contact 'Peter Cambel' updated with job title and annual income.");

        // Setup for next request.
        //
        // Retrieve selected properties of a Contact entity using GET request. 
        // NOTE: It is performance best practice to select only the properties you need.

        // Retrieved contact properties.
        var properties = [
         "fullname",
         "annualincome",
         "jobtitle",
         "description"].join();

        // NOTE: For performance best practices, use $select to limit the properties you want to return
        // See also: https://learn.microsoft.com/power-apps/developer/data-platform/webapi/query-data-web-api#select-columns
        var query = "?$select=" + properties;
        return Sdk.request("GET", contact1Uri + query, null);
    })
    .then(function (request) {
        // Process response from previous request.
        var contact1 = JSON.parse(request.response);
        var successMsg = "Contact '%s' retrieved:\n"
            + "\tAnnual income: %s \n"
            + "\tJob title: %s \n"
            + "\tDescription: %s";
        console.log(successMsg,
            contact1.fullname,      // This property is read-only. Calculated from firstname and lastname.
            contact1.annualincome,
            contact1.jobtitle,
            contact1.description); // Description will be "null" because it has not been set yet.

        // Setup for next request.
        //
        // Update properties.
        // Set new values for some of the properties and apply the values to the server via PATCH request.
        // Notice that we are updating the jobtitle and annualincome properties and adding value to the 
        // description property in the same request.
        var contact = {};
        contact.jobtitle = "Senior Developer";
        contact.annualincome = 95000.00;
        contact.description = "Assignment to-be-determined. ";
        return Sdk.request("PATCH", contact1Uri, contact);
    })
    .then(function () {
        // Process response from previous request.
        console.log("Contact 'Peter Cambel' updated:\n"
                            + "\tJob title: Senior Developer, \n"
                            + "\tAnnual income: 95000, \n"
                            + "\tDescription: Assignment to-be-determined.");

        // Setup for next request.
        //
        // Set value for a single property using PUT request.
        // In this case, we are setting the telephone1 property to "555-0105".
        var value = { value: "555-0105" };
        return Sdk.request("PUT", contact1Uri + "/telephone1", value);
    })
    .then(function () {
        // Process response from previous request.
        console.log("Contact 'Peter Cambel' phone number updated.");

        // Setup for next request.
        //
        // Retrieve single value property.
        // Get a value of a single property using GET request.
        // In this case, telephone1 is retrieved. We should get back "555-0105".
        return Sdk.request("GET", contact1Uri + "/telephone1", null);
    })
    .then(function (request) {
        // Process response from previous request.
        var phoneNumber = JSON.parse(request.response);
        console.log("Contact's phone number is: %s", phoneNumber.value);
    })
    .then(function () {
        // Setup for next request.
        // Starting with December 2016 update (v8.2), a contact instance can be 
        // created and its properties returned in one operation by using a 
        //'Prefer: return=representation' header.
        var contactAlt = {};
        contactAlt.firstname = "Peter_Alt";
        contactAlt.lastname = "Cambel";
        contactAlt.jobtitle = "Junior Developer";
        contactAlt.annualincome = 80000;
        contactAlt.telephone1 = "555-0110";
        var properties = [
         "fullname",
         "annualincome",
         "jobtitle"].join();
        var query = "?$select=" + properties;
        // Create contact and return its state (in the body).
        var retRepHeader = { header: "Prefer", value: "return=representation" };
        return Sdk.request("POST", entitySetName + query, contactAlt, retRepHeader);
    })
    .then(function (request) {
        var contactA = JSON.parse(request.response);
        //Because 'OData-EntityId' header not returned in a 201 response, you must instead 
        // construct the URI.
        contactAltUri = clientUrl + webAPIPath + "/contacts(" + contactA.contactid + ")"; 
        entitiesToDelete.push(contactAltUri); 
        var successMsg = "Contact '%s' created:\n"
            + "\tAnnual income: %s \n"
            + "\tJob title: %s \n";
        console.log(successMsg,
            contactA.fullname,      
            contactA.annualincome,
            contactA.jobtitle); 
        console.log("Contact URI: %s", contactAltUri);
    })
    .then(function () {    
        // Setup for next request.
        //Similarly, the December 2016 update (v8.2) also enables returning selected properties   
        //after an update operation (PATCH), with the 'Prefer: return=representation' header.
         var contactAlt = {};
         contactAlt.jobtitle = "Senior Developer";
         contactAlt.annualincome = 95000;
         contactAlt.description = "MS Azure and Dynamics 365 Specialist";
         var properties = [
            "fullname",
            "annualincome",
            "jobtitle",
            "description"].join();
         var query = "?$select=" + properties;
         // Update contact and return its state (in the body).
         var retRepHeader = { header: "Prefer", value: "return=representation" };
         return Sdk.request("PATCH", contactAltUri + query, contactAlt, retRepHeader);
    })
    .then(function (request) {
        // Process response from previous request.
        var contactA = JSON.parse(request.response);
        var successMsg = "Contact '%s' updated:\n"
            + "\tAnnual income: %s \n"
            + "\tJob title: %s \n";
        console.log(successMsg,
            contactA.fullname,      
            contactA.annualincome,
            contactA.jobtitle); 
    })
    .then(function () {
        // Setup for next request.
        //
        // Section 2.
        //
        // Create a new account entity and associate it with an existing contact using POST request.
        console.log("\n--Section 2 started--");
        var account = {};
        account.name = "Contoso, Ltd.";
        account.telephone1 = "555-5555";
        account["primarycontactid@odata.bind"] = contact1Uri; //relative URI ok. E.g.: "/contacts(###)".

        var entitySetName = "/accounts";
        return Sdk.request("POST", entitySetName, account);
    })
    .then(function (request) {
        // Process response from previous request.
        account1Uri = request.getResponseHeader("OData-EntityId");
        entitiesToDelete.push(account1Uri);
        console.log("Account 'Contoso, Ltd.' created.");

        // Setup for next request.
        //
        // Retrieve account's primary contact with selected properties using GET request and 'expand' query.
        var contactProperties = [
            "fullname",
            "jobtitle",
            "annualincome"
        ].join();
        var query = "?$select=name,telephone1&$expand=primarycontactid($select=" + contactProperties + ")";
        return Sdk.request("GET", account1Uri + query, null);
    })
    .then(function (request) {
        // Process response from previous request.
        var account1 = JSON.parse(request.response);
        var successMsg = "Account '%s' has primary contact '%s':  \n"
                        + "\tJob title:  %s \n"
                        + "\tAnnual income:  %s ";
        console.log(successMsg,
            account1.name,
            account1.primarycontactid.fullname,
            account1.primarycontactid.jobtitle,
            account1.primarycontactid.annualincome);

        // Setup for next request.
        //
        // Section 3.
        //
        // Create related entities (deep insert).
        // Create the following entities in one operation using deep insert technique:
        //   account
        //   |--- contact
        //        |--- tasks
        // Then retrieve properties of these entities
        //
        // Constructing the entity relationship.
        console.log("\n--Section 3 started--");
        var account = {};
        account.name = "Fourth Coffee";
        account.primarycontactid = {
            firstname: "Susie",
            lastname: "Curtis",
            jobtitle: "Coffee Master",
            annualincome: 48000.00,
            Contact_Tasks: [
                {
                    subject: "Sign invoice",
                    description: "Invoice #12321",
                    scheduledend: new Date("April 19th, 2016")
                },
                {
                    subject: "Setup new display",
                    description: "Theme is - Spring is in the air",
                    scheduledstart: new Date("4/20/2016")
                },
                {
                    subject: "Conduct training",
                    description: "Train team on making our new blended coffee",
                    scheduledstart: new Date("6/1/2016")
                }
            ]
        };

        var entitySetName = "/accounts";
        return Sdk.request("POST", entitySetName, account);
    })
    .then(function (request) {
        // Process response from previous request.
        account2Uri = request.getResponseHeader("OData-EntityId");
        entitiesToDelete.push(account2Uri);
        console.log("Account 'Fourth Coffee' created.");

        // Setup for next request.
        //
        // Retrieve account entity info using GET request and 'expand' query.
        var contactProperties = [
         "fullname",
         "jobtitle",
         "annualincome"].join();

        // Expand on primarycontactid to select some of contact's properties.
        // NOTE: With $expand, the CRM server will return values for the selected properties. 
        // The CRM Web API only supports expansions one level deep.
        // See also: https://learn.microsoft.com/power-apps/developer/data-platform/webapi/query-data-web-api#join-tables
        var query = "?$select=name&$expand=primarycontactid($select=" + contactProperties + ")";
        return Sdk.request("GET", account2Uri + query, null);
    })
    .then(function (request) {
        // Process response from previous request.
        var account2 = JSON.parse(request.response);
        var successMsg = "Account '%s' has primary contact '%s':\n"
            + "\tJob title:  %s \n"
            + "\tAnnual income:  %s";
        console.log(successMsg,
            account2.name,
            account2.primarycontactid.fullname,
            account2.primarycontactid.jobtitle,
            account2.primarycontactid.annualincome);

        // Setup for next request.
        //
        // Retrieve contact entity and expanding on its tasks using GET request.
        contact2Uri = clientUrl + webAPIPath + "/contacts(" + account2.primarycontactid.contactid + ")"; //Full URI.
        entitiesToDelete.push(contact2Uri); // For Susie Curtis
        var contactProperties = ["fullname", "jobtitle"].join();
        var contactTaskProperties = ["subject", "description", "scheduledstart", "scheduledend"].join();

        // Expand on contact_tasks to select some of its properties for each task.
        var query = "?$select=" + contactProperties +
            "&$expand=Contact_Tasks($select=" + contactTaskProperties + ")";
        return Sdk.request("GET", contact2Uri + query, null);
    })
    .then(function (request) {
        // Process response from previous request.
        var contact2 = JSON.parse(request.response);
        console.log("Contact '%s' has the following assigned tasks:", contact2.fullname);

        // construct the output string.
        var successMsg = "Subject: %s \n"
        + "\tDescription: %s \n"
        + "\tStart: %s \n"
        + "\tEnd: %s \n";

        for (var i = 0; i < contact2.Contact_Tasks.length; i++) {
            console.log(successMsg,
                contact2.Contact_Tasks[i].subject,
                contact2.Contact_Tasks[i].description,
                contact2.Contact_Tasks[i].scheduledstart,
                contact2.Contact_Tasks[i].scheduledend
            );
        }
        
        // Setup for next request.
        //
        // Section 4
        //
        // Entity associations:
        // Associate to existing entities via the different relationship types:
        // 1) 1:N relationship - Associate an existing contact to an existing account 
        //      (e.g.: contact - Peter Cambel to account - Fourth Coffee).
        // 2) N:N relationship - Associate an competitor to opportunity.

        console.log("\n--Section 4 started--");
        var contact = {};
        contact["@odata.id"] = contact1Uri;

        return Sdk.request("POST", account2Uri + "/contact_customer_accounts/$ref", contact)
    })
    .then(function () {
        // Process response from previous request.
        console.log("Contact 'Peter Cambel' associated to account 'Fourth Coffee'.");

        // Setup for next request.
        //
        // Verify that the reference was made as expected.
        var contactProperties = ["fullname", "jobtitle"].join();

        // This returns a collection of all associated contacts...in a "value" array.
        var query = "/contact_customer_accounts?$select=" + contactProperties;
        return Sdk.request("GET", account2Uri + query, null);
    })
    .then(function (request) {
        // Process response from previous request.
        var relatedContacts = JSON.parse(request.response).value; //collection is in the "value" array.
        var successMsg = "\tName: %s, "
                        + "Job title: %s ";

        console.log("Contact list for account 'Fourth Coffee': ");

        for (var i = 0; i < relatedContacts.length; i++) {
            console.log(successMsg,
                relatedContacts[i].fullname,
                relatedContacts[i].jobtitle
            );
        }

        // Setup for next request.
        //
        // Disassociate a contact from an account.
        return Sdk.request("DELETE", account2Uri + "/contact_customer_accounts/$ref?$id=" + contact1Uri, null);
    })
    .then(function () {
        // Process response from previous request.
        console.log("Contact 'Peter Cambel' disassociated from account 'Fourth Coffee'.");

        // Setup for next request.
        //
        // N:N relationship:
        // Associate a competitor to an opportunity.
        var competitor = {};
        competitor.name = "Adventure Works";
        competitor.strengths = "Strong promoter of private tours for multi-day outdoor adventures.";

        var entitySetName = "/competitors";
        return Sdk.request("POST", entitySetName, competitor);
    })
    .then(function (request) {
        // Process response from previous request.
        competitor1Uri = request.getResponseHeader("OData-EntityId");
        entitiesToDelete.push(competitor1Uri);
        console.log("Competitor 'Adventure Works' created.");

        // Setup for next request.
        // 
        // Create a new opportunity...
        var opportunity = {};
        opportunity.name = "River rafting adventure";
        opportunity.description = "Sales team on a river-rafting offsite and team building.";
        var entitySetName = "/opportunities";
        return Sdk.request("POST", entitySetName, opportunity);
    })
    .then(function (request) {
        // Process response from previous request.
        opportunity1Uri = request.getResponseHeader("OData-EntityId");
        entitiesToDelete.push(opportunity1Uri);
        console.log("Opportunity 'River rafting adventure' created.");

        // Setup for next request.
        //
        // Associate competitor to opportunity.
        var competitor = {};
        competitor["@odata.id"] = competitor1Uri;
        return Sdk.request("POST", opportunity1Uri + "/opportunitycompetitors_association/$ref", competitor);
    })
    .then(function () {
        // Process response from previous request.
        console.log("Opportunity 'River rafting adventure' associated with competitor 'Adventure Works'.");

        // Setup for next request.
        //
        // Retrieve competitor entity and expanding on its opportunitycompetitors_association
        // for all opportunities, using GET request.
        var opportunityProperties = ["name", "description"].join();
        var competitorProperties = ["name"].join();
        var query = "?$select=" + competitorProperties +
            "&$expand=opportunitycompetitors_association($select=" + opportunityProperties + ")";
        return Sdk.request("GET", competitor1Uri + query, null);
    })
    .then(function (request) {
        // Process response from previous request.
        var competitor1 = JSON.parse(request.response);
        console.log("Competitor '%s' has the following opportunities:", competitor1.name);
        var successMsg = "\tName: %s, \n"
                       + "\tDescription: %s";
        for (var i = 0; i < competitor1.opportunitycompetitors_association.length; i++) {
            console.log(successMsg,
                competitor1.opportunitycompetitors_association[i].name,
                competitor1.opportunitycompetitors_association[i].description
            );
        }

        // Setup for next request.
        //
        // Disassociate competitor from opportunity.
        return Sdk.request("DELETE", opportunity1Uri +
                "/opportunitycompetitors_association/$ref?$id=" + competitor1Uri, null);
    })
    .then(function () {
        // Process response from previous request.
        console.log("Opportunity 'River rafting adventure' disassociated with competitor 'Adventure Works'");

        // Setup for next request.
        //
        // House cleaning - deleting sample data
        // NOTE: If instances have a parent-child relationship, then deleting the parent will, 
        // by default, automatically cascade delete child instances. In this program, 
        // tasks related using the Contact_Tasks relationship have contact as their parent. 
        // Other relationships may behave differently.
        // See also: https://learn.microsoft.com/power-apps/developer/data-platform/configure-entity-relationship-cascading-behavior
        console.log("\n--Section 5 started--");
        if (deleteData) {
            for (var i = 0; i < entitiesToDelete.length; i++) {
                console.log("Deleting entity: " + entitiesToDelete[i]);
                Sdk.request("DELETE", entitiesToDelete[i], null)
                .catch(function (err) {
                    console.log("ERROR: Delete failed --Reason: \n\t" + err.message);
                });
            }
        } else {
            console.log("Sample data not deleted.");
        }
    })
    .catch(function (err) {
        console.log("ERROR: " + err.message);
    });
};