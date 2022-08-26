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

/// <reference path="Sdk.WebAPISample.js" />


document.onreadystatechange = function () {
 if (document.readyState == "complete") {
  startSample();
 }
}

//Simple error handler used by all samples just writes message to console
function sampleErrorHandler(error) {
 console.log(error.message);
}

function startSample() {
     getEntityList();
 
}

var accountToDeleteUri, parentAccountToDeleteUri, UserId;

//Demonstrates how to retrieve a list of entities
function getEntityList() {
 console.log("getEntityList function starting.")

 

 Sdk.WebAPISample.getEntityList(function (result) {
  //sort the entities by name  
  result.sort(function (a, b) {
   if (a.name.toLowerCase() < b.name.toLowerCase())
    return -1
   if (a.name.toLowerCase() > b.name.toLowerCase())
    return 1
   return 0
  })

  result.forEach(function (entity) {
   console.log("  " + entity.name);
  })
  console.log("getEntityList function completed.")
  //Next sample:
  addAndRemoveReference();
 },
 function (error) { console.log(error.message) })
}

//Demonstrates how to associate and disassociate using the single-valued navigation property.
function addAndRemoveReference() {
 console.log("addAndRemoveReference function starting.")
 var contactAUri, accountAUri;

 var contactA = {
  firstname: "Tom",
  lastname: "Test"
 };
 var accountA = {
  name: "Tom's Company"
 };

 Sdk.WebAPISample.create("contacts", contactA, function (uri) {
  contactAUri = uri;
  Sdk.WebAPISample.create("accounts", accountA, function (uri) {
   accountAUri = uri;
   //Set the contact as the primary contact for the account
      Sdk.WebAPISample.addReference(accountAUri, "primarycontactid", contactAUri, function () {
    //Retrieve the account
    Sdk.WebAPISample.retrieve(accountAUri,
     ["name"],
     ["primarycontactid($select=fullname)"],
     function (account) {
      console.log("   "+account.primarycontactid.fullname + " is the primary contact for " + accountA.name);
         //Remove the reference        
      Sdk.WebAPISample.removeReference(accountAUri, "primarycontactid", function () {
       //Retrieve the account again
       Sdk.WebAPISample.retrieve(accountAUri, ["name"],
        ["primarycontactid($select=fullname)"],
        function (account) {
         var primaryContactFullName = (account.primarycontactid == null) ? "null" : account.primarycontactid.fullname;
         //The value is null
         console.log("   The primary contact for " + accountA.name + " is " + primaryContactFullName);
         //Clean up by deleting records:
         Sdk.WebAPISample.del(contactAUri, function () {
          Sdk.WebAPISample.del(accountAUri, function () {
           console.log("addAndRemoveReference function completed.");
           //Next sample:
           upsertContact();
          }, sampleErrorHandler);
         }, sampleErrorHandler);
        }, sampleErrorHandler, true);
      }, sampleErrorHandler);
     }, sampleErrorHandler, true);
   }, sampleErrorHandler);
  }, sampleErrorHandler);
 }, sampleErrorHandler);
}

//Demonstrates the use of Upsert with options to prevent create or update
function upsertContact() {
 console.log("upsertContact function starting.")
 var joeJones = { firstname: "Joe", lastname: "Jones" };
    // var joeJonesUri = Xrm.Page.context.getClientUrl() + "/api/data/contacts(aa3293fa-c7a2-4ad3-9c1b-975325b19a17)";
    //TODO: remove following restore previous
 var joeJonesUri = Sdk.WebAPISample.getWebAPIPath() + "contacts(aa3293fa-c7a2-4ad3-9c1b-975325b19a17)";
 Sdk.WebAPISample.upsert(
     joeJonesUri,
     joeJones,
     false, //preventCreate
     false, //preventUpdate
     function (uri) {
      //Retrieve the full name of the contact
      Sdk.WebAPISample.retrievePropertyValue(uri, "fullname", function (value) {
       var fullName = value;
       console.log("   New Contact fullname returned: " + fullName);
       //Do not update the contact if it already exists
       Sdk.WebAPISample.upsert(uri, { firstname: "Joseph", lastname: "Jones" }, false, true, function () {
        //Retrieve the full name of the contact again
        Sdk.WebAPISample.retrievePropertyValue(uri, "fullname", function (value) {
         if (value == fullName) {
          console.log("   Expected result: The fullname property value did not change");
         }
         else {
          console.log("   Unexpected result: The fullname property value did change");
         }
         //Delete Joe Jones
         Sdk.WebAPISample.del(uri, function () {
          //Do not create the contact if it doesn't already exist
          var bobBurns = { firstname: "Bob", lastname: "Burns" };
             // var bobBurnsUri = Xrm.Page.context.getClientUrl() + "/api/data/contacts(b4d2b1c4-3577-496d-aa5e-d53dcf2d1513)";
             //TODO: remove following restore previous
          var bobBurnsUri = Sdk.WebAPISample.getWebAPIPath() + "contacts(b4d2b1c4-3577-496d-aa5e-d53dcf2d1513)";

          Sdk.WebAPISample.upsert(bobBurnsUri, bobBurns, true, false, function () {
           //Verify contact not created
              Sdk.WebAPISample.retrieve(bobBurnsUri,
                  ["fullname"],
                  null,
                  function () {
            console.log("   Unexpected result: The contact was created.")
                  },
           function (error) {
            console.log("   Expected result: The contact was not created.");
            console.log("   Expected error: " + error.message);
            //Next sample:
            createAccount();
           });
          }, sampleErrorHandler);
         }, sampleErrorHandler);
        }, sampleErrorHandler);
       }, sampleErrorHandler);
      }, sampleErrorHandler);
     }, sampleErrorHandler);
}

//Demonstrates creating new entity
function createAccount() {
 console.log("createAccount function starting.")
 var account = {}
 account.name = "Sample Account"; //String value
 account.creditonhold = false; //Boolean value
 account.address1_latitude = 47.6395830; //Double value
 account.description = "This is the description of the sample account"; //Memo value
 account.revenue = 5000000; //Money value
 account.accountcategorycode = 1; //OptionSet value: Preferred Customer

 Sdk.WebAPISample.create(
  "accounts", //entitySetName
  account, //entity
  function (uri) {
   console.log("   New account created with Uri = " + uri);

   //Cache in global variable to delete later
   accountToDeleteUri = uri;
   //Next sample:
   retrieveIndividualProperties(uri);
  }, //successCallback
  sampleErrorHandler); //errorCallback
}

//Demonstrates how to retrieve individual properties
function retrieveIndividualProperties(accountUri) {
 console.log("retrieveIndividualProperties function starting.")
 Sdk.WebAPISample.retrievePropertyValue(accountUri, "createdon", function (value) {
  console.log("   Account created on " + value.toLocaleDateString());
  Sdk.WebAPISample.retrievePropertyValue(accountUri, "_createdby_value", function (value) {
   console.log("   Account created by user with systemuserid = " + value);
   Sdk.WebAPISample.retrievePropertyValue(accountUri, "statuscode", function (value) {
    console.log("   Account status code = " + value);
    //CreditLimit is null
    Sdk.WebAPISample.retrievePropertyValue(accountUri, "creditlimit", function (value) {
     console.log("   Account credit limit is expected to be null and " + ((value == null) ? "it is." : "it is not."));
     //Next Sample
     updateIndividualProperties(accountUri);
    }, sampleErrorHandler);
   }, sampleErrorHandler);
  }, sampleErrorHandler);
 }, sampleErrorHandler);

}

//Demonstrates how to update individual properties
function updateIndividualProperties(accountUri) {
 console.log("updateIndividualProperties function starting.")
 var newName = "New Improved Account Name";
 Sdk.WebAPISample.updatePropertyValue(accountUri, "name", newName, function () {
  //Retrieve to verify it was set
  Sdk.WebAPISample.retrievePropertyValue(accountUri, "name", function (value) {
   console.log("   Account name property value should be '" + newName + "' and " + ((value == newName) ? "it is." : " it is not."));
   //Next Sample:
   createParentAccount(accountUri);
  }, sampleErrorHandler);
 }, sampleErrorHandler);
}

//Demonstrates how to associate entities using a collection-valued navigation property
function createParentAccount(accountUri) {
 console.log("createParentAccount function starting.");
 //Create an account to the parent
 var parentAccountName = "Parent Account";
 var parentAccount = { name: parentAccountName };
 Sdk.WebAPISample.create("accounts", parentAccount, function (uri) {
  var parentAccountUri = uri;
  //Associate parent to existing account
  Sdk.WebAPISample.associate(parentAccountUri, "account_parent_account", accountUri, function () {
   console.log("  accounts associated");
   //Retrieve from child to verify
   // Note that the navigation property names are different because this is a self-referential relationship
      Sdk.WebAPISample.retrieve(accountUri, ["name"], ["parentaccountid($select=name)"], function (value) {

          var retrievedParentAccountName = (value.parentaccountid == null) ? "null" : value.parentaccountid.name;
    console.log("   The name of the parent account should be " + parentAccountName + " and " + ((retrievedParentAccountName == parentAccountName) ? " it is." : " it is not."));

    //Delete later
    parentAccountToDeleteUri = parentAccountUri;

    //Next Sample:
    createThreeAssociatedTasks(accountUri);

   }, sampleErrorHandler);
  }, sampleErrorHandler);
 }, sampleErrorHandler);
}

//Demonstrates associating records on create using @odata.bind
function createThreeAssociatedTasks(accountUri) {
 console.log("createThreeAssociatedTasks function starting.");

 //Get date to see for scheduled start
 var now = new Date();
 var tomorrow = new Date();
 tomorrow.setDate(now.getDate() + 1);

 //Define a task object to re-use with each create
 var task = {}
 task.subject = "Task " + 1;
 task.scheduledstart = tomorrow;
// task["Account_Tasks@odata.bind"] = accountUri;
 task["regardingobjectid_account_task@odata.bind"] = accountUri;
 


 Sdk.WebAPISample.create("tasks",
  task,
  function () {  //successcallback 1
   task.subject = "Task " + 2; //Update the task.subject for next new task
   Sdk.WebAPISample.create("tasks",
    task,
    function () { //successcallback 2
     task.subject = "Task " + 3; //Update the task.subject for next new task
     Sdk.WebAPISample.create("tasks",
      task,
      function () { //successcallback 3
       console.log("    3 tasks associated with the account created");

       //Next Sample
       createTwoAssociatedTasksInBatch(accountUri);

      }, sampleErrorHandler)
    }, sampleErrorHandler)
  }, sampleErrorHandler);
}

//Demonstrates the use of batch operations
function createTwoAssociatedTasksInBatch(accountUri) {
 console.log("createTwoAssociatedTasksInBatch function starting.");

 //Generate a random set of 10 characters to serve as the batchId value
 var batchId = Sdk.WebAPISample.getRandomId();
 //Generate a random set of 10 characters to serve as the changeSetId value
 var changeSetId = Sdk.WebAPISample.getRandomId();

 //Define tasks to be created:
 var firstTask = {
  subject: "Task 1 in batch",
  "regardingobjectid_account_task@odata.bind": accountUri
 };
 var secondTask = {
  subject: "Task 2 in batch",
  "regardingobjectid_account_task@odata.bind": accountUri
 };

 //Prepare the payload:

 //Start of ChangeSet
 payload = ["--batch_" + batchId]
 payload.push("Content-Type: multipart/mixed;boundary=changeset_" + changeSetId);
 payload.push("");
 //First item in ChangeSet
 payload.push("--changeset_" + changeSetId);
 payload.push("Content-Type: application/http");
 payload.push("Content-Transfer-Encoding:binary");
 payload.push("Content-ID: 1");
 payload.push("");
    // payload.push("POST " + Xrm.Page.context.getClientUrl() + "/api/data/tasks HTTP/1.1");
    //TODO: remove following restore previous
 payload.push("POST " + Sdk.WebAPISample.getWebAPIPath() + "tasks HTTP/1.1");
 payload.push("Content-Type: application/json;type=entry");
 payload.push("");
 payload.push(JSON.stringify(firstTask));
 //Second item in ChangeSet
 payload.push("--changeset_" + changeSetId);
 payload.push("Content-Type: application/http");
 payload.push("Content-Transfer-Encoding:binary");
 payload.push("Content-ID: 2");
 payload.push("");
    // payload.push("POST " + Xrm.Page.context.getClientUrl() + "/api/data/tasks HTTP/1.1");
    //TODO: remove following restore previous
 payload.push("POST " + Sdk.WebAPISample.getWebAPIPath() + "tasks HTTP/1.1");
 payload.push("Content-Type: application/json;type=entry");
 payload.push("");
 payload.push(JSON.stringify(secondTask));
 //End of ChangeSet
 payload.push("--changeset_" + changeSetId + "--");
 payload.push("");
 //Adding a GET request outside of the ChangeSet
 payload.push("--batch_" + batchId);
 payload.push("Content-Type: application/http");
 payload.push("Content-Transfer-Encoding:binary");
 payload.push("");
 //Retrieve all the tasks related to the account
 payload.push("GET " + accountUri + "/Account_Tasks?$select=subject HTTP/1.1");
 payload.push("Accept: application/json");
 payload.push("");
 payload.push("--batch_" + batchId + "--");


 Sdk.WebAPISample.executeBatch(payload.join("\r\n"), //payload
    batchId, //batchId
  function (response) {  //successCallback
   console.log("   --Response from createTwoAssociatedTasksInBatch START--")
   //Internet Explorer truncates text sent to console.log after 1024 characters
   // Breaking up the response into chunks of 1000 characters and sending
   // the output to the console.
   var chunkedResponse = response.match(/(.|[\r\n]){1,1000}/g);
   chunkedResponse.forEach(function (chunk) {
    console.log(chunk)
   }); 
   console.log("   --Response from createTwoAssociatedTasksInBatch END--")



   //Next Sample
   updateAccount(accountUri);

  }, sampleErrorHandler);




}

//Demonstrates a simple update
function updateAccount(accountUri) {
 console.log("updateAccount function starting.");
 var updatedAccount = {};
 updatedAccount.name = "Updated Account Name";
 updatedAccount.description = "Sample Account Description Updated.";
 Sdk.WebAPISample.update(accountUri, //uri for record to update
  updatedAccount, //updatedEntity
  function () {
   console.log("   Account Name Updated");

   //Next Sample
   retrieveAccount(accountUri);
  }, //successCallback
  sampleErrorHandler); //errorCallback
}

//Demonstrates retrieve with related entities
function retrieveAccount(accountUri) {
 console.log("retrieveAccount function starting.");

 // set entity properties to retrieve
 var properties = [
     "accountcategorycode",
     "accountclassificationcode",
     "accountnumber",
     "accountratingcode",
     "businesstypecode",
     "creditonhold",
     "createdon",
     "lastusedincampaign",
     "address1_latitude",
     "address1_longitude",
     "numberofemployees",
     "parentaccountid",
     "description",
     "name",
     "revenue"
 ];
 //Set navigation properties to retrieve
 var navigationProperties = [
     "parentaccountid($select=createdon,name)",//Data from parent account   
     "Account_Tasks($select=subject,scheduledstart)" //Data from related tasks 
 ];

    
 Sdk.WebAPISample.retrieve(accountUri,//uri for record to update
  properties, //properties
  navigationProperties, //navigationproperties
  function(retrievedAccount){
   retrieveAccountSuccess(retrievedAccount,accountUri)
  },  //successCallback 
  sampleErrorHandler,//errorCallback
  true, //includeFormattedValues
  null, //eTag
  null, //unmodifiedCallback
  null); //callerId
}
function retrieveAccountSuccess(retrievedAccount, accountUri) {
 console.log("retrieveAccountSuccess function starting.");
 console.log("    Account Retrieved");
 console.log("        Account accountcategorycode value: " + retrievedAccount.accountcategorycode);
 //Account accountcategorycode value: 1
 console.log("        Account accountcategorycode formatted value: " + retrievedAccount["accountcategorycode@mscrm.formattedvalue"]);
 //Account accountcategorycode formatted value: Preferred Customer
 console.log("        Parent Account name: " + retrievedAccount.parentaccountid.name);
 //Parent Account name: Parent Account
 console.log("        Related tasks subject values: ");

 //Loop through the array of related tasks returned
 retrievedAccount.Account_Tasks.forEach(function (task) {
  console.log("           " + task.subject);
 })
 // console output:
 //Task 1 
 //Task 2
 //Task 3
 //Task 1 in batch
 //Task 2 in batch

 //Next Sample
 queryEntitySet(retrievedAccount.accountid);
}

//Demonstrates querying an entity set and retrieving additional pages

//These variables are used in the following functions related to querying entity sets
var includeFormattedValues = true;
var maxPageSize = 2;
function queryEntitySet(accountid) {
 console.log("queryEntitySet function starting.");
 var query = "$filter=_regardingobjectid_value eq " + accountid + "&$select=subject";
 Sdk.WebAPISample.queryEntitySet("activitypointers",
  query,// query
  includeFormattedValues,// includeFormattedValues
  maxPageSize, // maxPageSize 
  queryEntitySetSuccess,//successCallback
  sampleErrorHandler)//errorCallback

}
function queryEntitySetSuccess(response) {
 console.log("queryEntitySetSuccess function starting.");
 var activities = response.value;
 var nextLink = response["@odata.nextLink"];
 console.log("   First page of activities retrieved using QueryEntitySet:");
 activities.forEach(function (activity) {
  console.log("   Activity Subject:" + activity.subject);
 });

 //Get the next page of results
 Sdk.WebAPISample.getNextPage(nextLink,
  includeFormattedValues, // includeFormattedValues same value used with queryEntitySet
  maxPageSize,// maxPageSize same value used with queryEntitySet
  getNextPageSuccess, //successCallback
  sampleErrorHandler)//errorCallback
}
function getNextPageSuccess(response) {
 console.log("getNextPageSuccess function starting.");
 console.log("   Second page of activities retrieved using QueryEntitySet:");
 response.value.forEach(function (activity) {
  console.log("   Activity Subject:" + activity.subject);
 });

 //Next Sample
 deleteAccounts();
}

//Demonstrates deleteing entities
function deleteAccounts() {
 console.log("deleteAccounts function starting.");
 Sdk.WebAPISample.del(accountToDeleteUri, //uri of account to delete
  function () {  //successcallback
   console.log("    Account Deleted");
   Sdk.WebAPISample.del(parentAccountToDeleteUri,  //uri of account to delete
    function () {  //successcallback
     console.log("    Parent Account Deleted");

     //Next Sample
     WhoAmIFunction();

    }, sampleErrorHandler)
  }, sampleErrorHandler)
}

//Demonstrates using an unbound function
function WhoAmIFunction() {
 console.log("WhoAmIFunction function starting.");
 Sdk.WebAPISample.invokeUnboundFunction("WhoAmI", //functionName
  null, //parameters
  WhoAmIFunctionSuccess,  //successCallback
  sampleErrorHandler); //errorCallback
}
function WhoAmIFunctionSuccess(WhoAmIResponse) {
 console.log("WhoAmIFunctionSuccess function starting.");
 console.log("    Results from WhoAmI function:")
 console.log("       UserId: " + WhoAmIResponse.UserId);
 //caching this for use in a later sample
 UserId = WhoAmIResponse.UserId;
 console.log("       BusinessUnitId: " + WhoAmIResponse.BusinessUnitId);
 console.log("       OrganizationId: " + WhoAmIResponse.OrganizationId);

 //Next Sample
 GetAllTimeZonesWithDisplayName();
}

//Demonstrates using an bound function : GetAllTimeZonesWithDisplayName
function GetAllTimeZonesWithDisplayName() {
 console.log("GetAllTimeZonesWithDisplayName function starting.");
 var lcidParam = ["LocaleId=1033"];
 Sdk.WebAPISample.invokeBoundFunction("timezonedefinitions", "GetAllTimeZonesWithDisplayName",//functionName
  lcidParam, //parameters
  GetAllTimeZonesWithDisplayNameSuccess, //successCallback
  sampleErrorHandler);//errorCallback
}
function GetAllTimeZonesWithDisplayNameSuccess(GATZWDNResponse) {
 console.log("GetAllTimeZonesWithDisplayNameSuccess function starting.");
 console.log("    Results from GetAllTimeZonesWithDisplayName function:")
 //Loop through the results:
 GATZWDNResponse.forEach(function (item) {
  console.log("       " + item.userinterfacename)
 })

 //Next Sample
 //retrieveUserQueues(); //NOT WORKING, SKIPPING
 WinOpportunityAction();
}

//Demonstrates using an unbound function : RetrieveUserQueues
//NOTE: RetrieveUserQueues is a bound function post preview;
function retrieveUserQueues() {
 console.log("retrieveUserQueues function starting.");

 var parameters = [];
 parameters.push("UserId=" + UserId);
 parameters.push("IncludePublic=true");

 Sdk.WebAPISample.invokeUnboundFunction("RetrieveUserQueues",
  parameters,
  function (response) {
   console.log("   Returned " + response.value.length + " user queues.")

   //Next Sample
   WinOpportunityAction();
  }, sampleErrorHandler);

}



//Demonstrates using an unbound action : WinOpportunity
function WinOpportunityAction() {
 console.log("WinOpportunityAction function starting.");
 var winOppAccountUri, winOppUri;

 //Create an account to own the opportunity
 var winOppAccount = { name: "Win Opp Account" };
 Sdk.WebAPISample.create("accounts", winOppAccount,
  function (uri) { //successcallback
   winOppAccountUri = uri;

   //Create an opportunity to Win
   var winOpp = {
    name: "Opportunity to Win",
    "customerid_account@odata.bind": winOppAccountUri,
    "parentaccountid@odata.bind": winOppAccountUri
   };
   Sdk.WebAPISample.create("opportunities", winOpp,
    function (uri) { //successcallback
     winOppUri = uri;

     //Prepare parameters to close the opportunity as Won
     WinOppParams = {
      Status: 3,
      OpportunityClose: {
          "@odata.type": "#Microsoft.Dynamics.CRM.opportunityclose",
       subject: "Won Opp Activity",
       description: "We won this opportunity.",       
       "opportunityid@odata.bind": winOppUri
      }
     }

     //Call the WinOpportunity action
     Sdk.WebAPISample.invokeUnboundAction("WinOpportunity", WinOppParams,
      function () { //successcallback
       console.log("    Opportunity closed as won");

       //Cleanup records created by this sample
       Sdk.WebAPISample.del(winOppAccountUri,
        function () {  //successcallback
         console.log("    Account deleted and all related records with it.")
        }, sampleErrorHandler);
      }, sampleErrorHandler);
    }, sampleErrorHandler);
  }, sampleErrorHandler)

}




