
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
/// <reference path="Sdk.WebAPIModelSample.js" />
/// <reference path="Sdk.EntityClasses.js" />



var mns = Sdk.WebAPIModelSample;
var createdEntityUri;
document.onreadystatechange = function () {
 if (document.readyState == "complete") {
  startSample();
 }
}

//Simple error handler used by all samples just writes message to console
function sampleErrorHandler(error) {
 console.log(error.message);
}



//,
//Incident_Tasks: [
//    {
//        subject: "Sample Incident Task 1",
//        actualdurationminutes: 10
//    },


function startSample() {
 //createCustomerRelationship("new_bankaccount");

 retrivedExpandedAccounts();

 //createAppointment();
 //createBooleanAttribute();
 // showAddToQueue();
 // showWinOpportunity();
 // showCustomAction();
 // queryEntities();
//  createEntity();

 // createManyToMany();

 // var myNameSpace = Sdk.WebAPISample;

 //myNameSpace.create("accounts",
 //    { name: "Sample account" },
 //    function () { console.log("Created account with URI: " + accountUri) },
 //    function (error) { console.log(error.message); });

 //Sdk.WebAPISample.queryEntitySet("accounts/$count", "", false, null, function () {
 // console.log("success");
 //}, sampleErrorHandler);
 //var properties = ["title","_accountid_value", "_contactid_value","_customerid_value"];
 //var navigationProperties = ["customerid_account($select=name)","customerid_contact($select=fullname)"];
 //Sdk.WebAPISample.retrieve(Sdk.WebAPISample.getWebAPIPath() + "incidents(39dd0b31-ed8b-e511-80d2-00155d2a68d4)",
 // properties,
 // navigationProperties,
 // function (opp) { },
 // sampleErrorHandler,
 // true);

 //Sdk.WebAPISample.queryEntitySet("accounts", "$select=name&$filter=primarycontactid/firstname eq 'Renee'", false, null, function (results) {
 // console.log(JSON.stringify(results));
 //}, sampleErrorHandler)

 //Sdk.WebAPISample.retrieve(Sdk.WebAPISample.getWebAPIPath()+"accounts("+"25DD0B31-ED8B-E511-80D2-00155D2A68D4)",
 // ["name"], ["primarycontactid($select=fullname)"], function (results) {
 //  console.log(JSON.stringify(results));
 // },sampleErrorHandler)

 //var query = "<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" distinct=\"true\"><entity name=\"account\"><attribute name=\"name\" /><link-entity name=\"activitypointer\" from=\"regardingobjectid\" to=\"accountid\" link-type=\"inner\"><attribute name=\"subject\" /></link-entity></entity></fetch>"

 //Sdk.WebAPISample.queryEntitySet("accounts", "fetchXml=" + query, false, null, function (results) {
 // console.log("done");
 //}, sampleErrorHandler);

 // Sdk.WebAPISample.retrieve(Sdk.WebAPISample.getWebAPIPath()+"accounts("+"25DD0B31-ED8B-E511-80D2-00155D2A68D4)",
 //  ["name"], ["contact_customer_accounts($select=emailaddress1,lastname,firstname)"], function (results) {


 //   Sdk.WebAPISample.retrieve(Sdk.WebAPISample.getWebAPIPath() + "accounts(" + "25DD0B31-ED8B-E511-80D2-00155D2A68D4)/contact_customer_accounts?$select=emailaddress1,lastname,firstname",
 //null, null, function (results) {
 // }, sampleErrorHandler)


 //  },sampleErrorHandler)


};

//Expanded request
// - GET request with expanded collections.
// - Expanding the Tasks for accountid=a8daed6f-8efb-e511-80d1-00155da84802
//   accountid=236b12cc-8efb-e511-80d1-00155da84802
//   b24797ba-3efc-e511-80d1-00155da84802
//   5fc988f3-40fc-e511-80d1-00155da84802

//b0d08bc5-9bf6-e511-80d0-00155d2a68db

function retrivedExpandedAccounts() {

 var accountid = "7460c03f-cffa-e511-80d2-00155d2a68db"
 var req = new XMLHttpRequest();

 var select = "$select=accountid,name";
 var expand = "$expand=primarycontactid($select=fullname),Account_Tasks($top=100;$select=subject,description)";

 req.open("GET", "http://jdalynaos3/Contoso/api/data/v8.1/accounts("+accountid+")" + "?" + select + "&" + expand, true);
 req.setRequestHeader("OData-MaxVersion", "4.0");
 req.setRequestHeader("OData-Version", "4.0");
 req.setRequestHeader("Accept", "application/json");
 req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
 req.setRequestHeader("Prefer", "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"");
 req.setRequestHeader("Prefer", "odata.maxpagesize=40"); //Record limit control
 req.onreadystatechange = function () {
  if (this.readyState === 4) {
   req.onreadystatechange = null;
   if (this.status === 200) {
    var results = JSON.parse(this.response);
    var recordCount = results["@odata.count"];

    console.log("Account ID: " + results["accountid"]);
    console.log("Full Name: " + results["name"]);
    console.log("Primary contact ID: " + results.primarycontactid["contactid"]);
    console.log("Primary contact Name: " + results.primarycontactid["fullname"]);

    for (var i = 0; i < results.Account_Tasks.length; i++) {
     console.log("Task Subject: " + results.Account_Tasks[i]["subject"]);
     console.log("Task Description: " + results.Account_Tasks[i]["description"]);
     console.log("Task ActivityID: " + results.Account_Tasks[i]["activityid"]);
    }

   }
   else {
    console.log("Retrieve failed due to: " + this.statusText);
   }
  }
 };
 req.send();
}



function createAppointment() {

 var organizerUri = "/systemusers(dc74deca-57a4-e511-80cf-00155d0a2875)";
 var requiredContactUri = "/contacts(086df768-fab3-e511-80d2-00155d2a68d9)"; //Tom
 var optionalContactUri = "/contacts(e9e0846e-b4b8-e511-80d4-00155d2a68d9)"; //Vivek

 var start = new Date('2016-03-17T03:00:00');
 var end = new Date('2016-03-17T04:00:00');

 var appointment = {
  subject: "Test Appointment",
  scheduledstart: start,
  scheduledend: end,
   activitypointer_activity_parties: [
   {
    "partyid_systemuser@odata.bind": organizerUri,
    participationtypemask: 7
   },
 {
 "partyid_contact@odata.bind": requiredContactUri,
 participationtypemask: 5
 },
  {
 "partyid_contact@odata.bind": optionalContactUri,
  participationtypemask: 6
 }
  ]

 };
 Sdk.WebAPISample.create("appointments", appointment, function () {
  console.log("success");
 }, sampleErrorHandler);






};

function createCustomerRelationship(entityLogicalName)
{
 var msn = Sdk.WebAPIModelSample;
 var lookup = new msn.LookupAttributeMetadata();
 lookup.Description = new msn.Label(new msn.LocalizedLabel("Customer Lookup Attribute Description", 1033));
 lookup.DisplayName = new msn.Label(new msn.LocalizedLabel("Customer", 1033));
 lookup.SchemaName = "new_customerId";
 
 var accountRelationship = new msn.OneToManyRelationshipMetadata();
 accountRelationship.ReferencedEntity = "account";
 accountRelationship.ReferencingEntity = entityLogicalName;
 accountRelationship.SchemaName =  entityLogicalName + "_customer_account";

 var contactRelationship = new msn.OneToManyRelationshipMetadata();
 contactRelationship.ReferencedEntity = "contact";
 contactRelationship.ReferencingEntity = entityLogicalName;
 contactRelationship.SchemaName =  entityLogicalName + "_customer_contact";

 var obj = {
  "@odata.type": "Microsoft.Dynamics.CRM.CustomerRelationshipMetadata",
  OneToManyRelationships: [accountRelationship, contactRelationship],
  Lookup: lookup
 
 }

 Sdk.WebAPISample.create("RelationshipDefinitions", obj, function () {
  console.log("success creating customer lookup")
 }, sampleErrorHandler)

}

function createManyToMany() {
 var msn = Sdk.WebAPIModelSample;


 var mmre1amc = new msn.AssociatedMenuConfiguration();
 mmre1amc.Behavior = msn.AssociatedMenuBehavior.UseLabel;
 mmre1amc.Group = msn.AssociatedMenuGroup.Details;
 mmre1amc.Label = new msn.Label(new msn.LocalizedLabel("Account", 1033));
 mmre1amc.Order = 10000;

 var mmre2amc = new msn.AssociatedMenuConfiguration();
 mmre2amc.Behavior = msn.AssociatedMenuBehavior.UseLabel;
 mmre2amc.Group = msn.AssociatedMenuGroup.Details;
 mmre2amc.Label = new msn.Label(new msn.LocalizedLabel("Campaign", 1033));
 mmre2amc.Order = 10000;



 var mmr = new msn.ManyToManyRelationshipMetadata();
 mmr.IntersectEntityName = "new_accounts_campaigns";
 mmr.SchemaName = "new_accounts_campaigns";
 mmr.Entity1LogicalName = "account";
 mmr.Entity1AssociatedMenuConfiguration = mmre1amc;
 mmr.Entity2LogicalName = "campaign";
 mmr.Entity2AssociatedMenuConfiguration = mmre2amc;

 Sdk.WebAPISample.create("RelationshipDefinitions", mmr, function (uri) {
  console.log(uri)
  Sdk.WebAPISample.del(uri, function () {
   console.log("Many-to-many relationship deleted.")
  }, sampleErrorHandler);
 }, sampleErrorHandler);


}

function createEntity() {

 var msn = Sdk.WebAPIModelSample;
 var pa = new msn.StringAttributeMetadata();
 pa.SchemaName = "new_AccountName";
 pa.RequiredLevel = new msn.AttributeRequiredLevelManagedProperty(msn.AttributeRequiredLevel.None, true);
 pa.MaxLength = 100;
 pa.FormatName = msn.StringFormatName.Text;
 pa.DisplayName = new msn.Label(new msn.LocalizedLabel("Account Name", 1033));
 pa.Description = new msn.Label(new msn.LocalizedLabel("Type the name of the bank account", 1033));
 pa.IsPrimaryName = true;


 var entity = new msn.EntityMetadata();
 entity.SchemaName = "new_BankAccount";
 entity.DisplayName = new msn.Label(new msn.LocalizedLabel("Bank Account", 1033));
 entity.DisplayCollectionName = new msn.Label(new msn.LocalizedLabel("Bank Accounts", 1033));
 entity.Description = new msn.Label(new msn.LocalizedLabel("An entity to store information about customer bank accounts", 1033));
 entity.OwnershipType = msn.OwnershipTypes.UserOwned;
 entity.IsActivity = false;
 entity.Attributes = [pa];
 entity.HasActivities = false;
 entity.HasNotes = false;

 var sa = new msn.StringAttributeMetadata();
 sa.SchemaName = "new_BankName";
 sa.DisplayName = new msn.Label(new msn.LocalizedLabel("Bank Name", 1033));
 sa.Description = new msn.Label(new msn.LocalizedLabel("Type the name of the bank", 1033));
 sa.RequiredLevel = new msn.AttributeRequiredLevelManagedProperty(msn.AttributeRequiredLevel.None, true);
 sa.MaxLength = 100;
 sa.FormatName = msn.StringFormatName.Text;

 var ma = new msn.MoneyAttributeMetadata();
 ma.SchemaName = "new_Balance";
 ma.DisplayName = new msn.Label(new msn.LocalizedLabel("Balance", 1033));
 ma.Description = new msn.Label(new msn.LocalizedLabel("Enter the balance amount.", 1033));
 ma.RequiredLevel = new msn.AttributeRequiredLevelManagedProperty(msn.AttributeRequiredLevel.None, true);
 ma.PrecisionSource = 2;

 var da = new msn.DateTimeAttributeMetadata();
 da.SchemaName = "new_Checkeddate";
 da.DisplayName = new msn.Label(new msn.LocalizedLabel("Date", 1033));
 da.Description = new msn.Label(new msn.LocalizedLabel("The date the account balance was last confirmed", 1033));
 da.RequiredLevel = new msn.AttributeRequiredLevelManagedProperty(msn.AttributeRequiredLevel.None, true);
 da.Format = msn.DateTimeFormat.DateOnly;

 var la = new msn.LookupAttributeMetadata();
 la.SchemaName = "new_AccountOwner";
 la.DisplayName = new msn.Label(new msn.LocalizedLabel("Account Owner", 1033));
 la.Description = new msn.Label(new msn.LocalizedLabel("The owner of the account", 1033));
 la.RequiredLevel = new msn.AttributeRequiredLevelManagedProperty(msn.AttributeRequiredLevel.ApplicationRequired, true);

 var omramc = new msn.AssociatedMenuConfiguration();
 omramc.Behavior = msn.AssociatedMenuBehavior.UseCollectionName;
 omramc.Group = msn.AssociatedMenuGroup.Details;
 omramc.Label = new msn.Label(new msn.LocalizedLabel("Bank Accounts", 1033));
 omramc.Order = 10000;

 var omrcc = new msn.CascadeConfiguration();
 omrcc.Assign = msn.CascadeType.Cascade;
 omrcc.Delete = msn.CascadeType.Cascade;
 omrcc.Merge = msn.CascadeType.Cascade;
 omrcc.Reparent = msn.CascadeType.Cascade;
 omrcc.Share = msn.CascadeType.Cascade;
 omrcc.Unshare = msn.CascadeType.Cascade;

 var omr = new msn.OneToManyRelationshipMetadata();
 omr.AssociatedMenuConfiguration = omramc;
 omr.CascadeConfiguration = omrcc;
 omr.ReferencedEntity = "contact";
 omr.ReferencedAttribute = "contactid";
 omr.ReferencingEntity = "new_bankaccount"
 omr.SchemaName = "new_contact_new_bankaccount"
 omr.Lookup = la;

 //This works, just need to capture it
 Sdk.WebAPISample.create("EntityDefinitions", entity, function (uri) {
  console.log(uri);
  //Sdk.WebAPISample.create("RelationshipDefinitions", omr, function () {
  // console.log("success?")
  //}, sampleErrorHandler);
  createCustomerRelationship("new_bankaccount");
 }, sampleErrorHandler);


 //Sdk.WebAPISample.create("EntityDefinitions", entity, function (uri) {
 //    console.log(uri);
 //    Sdk.WebAPISample.retrieve(uri, null, null, function (entity) {
 //        entity.DisplayName = new msn.Label(new msn.LocalizedLabel("Bank Business Name", 1033));
 //        Sdk.WebAPISample.updateMetadata(uri, entity, function () {
 //            console.log("Entity Updated");
 //            Sdk.WebAPISample.del(uri, function () {
 //                console.log("Entity Deleted");
 //            }, sampleErrorHandler);
 //        }, sampleErrorHandler);
 //    }, sampleErrorHandler);
 //}, sampleErrorHandler);


 //Sdk.WebAPISample.create("EntityDefinitions", entity, function (uri) {
 //    console.log(uri);
 //    Sdk.WebAPISample.createAttribute(uri, sa, function () {
 //        console.log("String attribute created");
 //        Sdk.WebAPISample.createAttribute(uri, ma, function () {
 //            console.log("Money attribute created");
 //            Sdk.WebAPISample.createAttribute(uri, da, function () {
 //                console.log("Date attribute created");
 //                Sdk.WebAPISample.del(uri, function () {
 //                    console.log("Entity Deleted");
 //                }, sampleErrorHandler);
 //            }, sampleErrorHandler)
 //        }, sampleErrorHandler)
 //    }, sampleErrorHandler)
 //}, sampleErrorHandler);


}

function queryEntities() {

 //Sdk.WebAPISample.queryEntitySet("EntityDefinitions", "$select=LogicalName,DisplayName", true, null, function (results) {
 //    console.log("success");
 //}, sampleErrorHandler);

 Sdk.WebAPISample.retrieve(Sdk.WebAPISample.getWebAPIPath() + "EntityDefinitions(70816501-edb9-4740-a16c-6a5efbc05d84)/Attributes(5967e7cc-afbb-4c10-bf7e-e7ef430c52be)/Microsoft.Dynamics.CRM.PicklistAttributeMetadata/OptionSet", ["Options"], null, function () {
  console.log("success");
 }, sampleErrorHandler);

 //Sdk.WebAPISample.retrieve(Sdk.WebAPISample.getWebAPIPath() + "EntityDefinitions(70816501-edb9-4740-a16c-6a5efbc05d84)/Attributes/Microsoft.Dynamics.CRM.MoneyAttributeMetadata", ["LogicalName,Precision" ],null, function () {
 //    console.log("success");
 //}, sampleErrorHandler);

}


function showCustomAction() {

 var contacturi;
 var contact = { firstname: "Joe", lastname: "Smith" }
 var new_AddNoteToContact = {
  NoteTitle: "New Note Title",
  NoteText: "This is the text of the note"
 }
 Sdk.WebAPISample.create("contacts", contact, function (uri) {
  contacturi = uri;
  Sdk.WebAPISample.invokeInstanceBoundAction(contacturi, "new_AddNoteToContact", new_AddNoteToContact, function (note) {
   console.log(note.annotationid);
  }, sampleErrorHandler);
 }, sampleErrorHandler);


}

function showWinOpportunity() {
 var accountUri, opportunityUri;

 var account = { name: "Example Account" };
 var opportunity = { name: "Example Opportunity" }
 var opportunityClose = {
  subject: "Won Opportunity"
 };
 var winOpportunity = { Status: 3 }


 Sdk.WebAPISample.create("accounts", account, function (uri) {
  accountUri = uri;
  opportunity["customerid_account@odata.bind"] = accountUri;
  Sdk.WebAPISample.create("opportunities", opportunity, function (oppUri) {
   opportunityUri = oppUri;
   opportunityClose["opportunityid@odata.bind"] = opportunityUri;
   winOpportunity.OpportunityClose = opportunityClose;
   Sdk.WebAPISample.invokeUnboundAction("WinOpportunity", winOpportunity, function () {
    console.log("success");
   }, sampleErrorHandler);
  }, sampleErrorHandler)
 }, sampleErrorHandler);

}

function showAddToQueue() {
 //create a queue
 var destinationQueueUri, letterUri, letter;

 var destinationqueue = {

  name: "Destination Queue",
  description: "This is an example queue",
  queueviewtype: 1
 }
 var letter = {
  subject: "Example Letter",
  description: "Example letter description"
 }

 var AddToQueueParameters = {
  Target: ""

 };

 Sdk.WebAPISample.create("queues", destinationqueue, function (dquri) {
  destinationQueueUri = dquri;
  Sdk.WebAPISample.create("letters", letter, function (luri) {
   letterUri = luri;
   Sdk.WebAPISample.retrieve(letterUri, ["activityid"], null, function (l) {
    letter = l;
    // These properties are included in the retrieved entity.
    delete letter["@odata.etag"];
    delete letter["@odata.context"];
    //The type has to be set.
    letter["@odata.type"] = "Microsoft.Dynamics.CRM.letter";
    AddToQueueParameters.Target = letter;
    Sdk.WebAPISample.invokeInstanceBoundAction(destinationQueueUri, "AddToQueue", AddToQueueParameters, function (response) {
     console.log("success?")
     //Delete start
     Sdk.WebAPISample.del(letterUri, function () {
      Sdk.WebAPISample.del(destinationQueueUri, function () {
       console.log("records deleted");
      }, sampleErrorHandler);
     }, sampleErrorHandler);
     //Delete end
    }, sampleErrorHandler);
   }, sampleErrorHandler);
  }, sampleErrorHandler);
 }, sampleErrorHandler);

 //Didn't work
 //Sdk.WebAPISample.create("queues", destinationqueue, function (dquri) {
 //    destinationQueueUri = dquri;
 //    Sdk.WebAPISample.create("letters", letter, function (luri) {
 //        letterUri = luri;
 //        AddToQueueParameters.Target = { "@odata.id": letterUri };
 //        Sdk.WebAPISample.invokeInstanceBoundAction(destinationQueueUri, "AddToQueue", AddToQueueParameters, function (response) {
 //            console.log("success?")
 //            //Delete start
 //            Sdk.WebAPISample.del(letterUri, function () {
 //                Sdk.WebAPISample.del(destinationQueueUri, function () {
 //                    console.log("records deleted");
 //                }, sampleErrorHandler);
 //            }, sampleErrorHandler);
 //            //Delete end
 //        }, sampleErrorHandler);
 //    }, sampleErrorHandler);
 //}, sampleErrorHandler);



}

function showCalculateTotalTimeIncident() {

 var taskTemplate = {
  subject: "",
  actualdurationminutes: 0,
  "regardingobjectid_incident_task@odata.bind": ""
 };

 var incident = {
  title: "Sample Incident",
  "customerid_account@odata.bind": "http://jdaly803/Contoso/api/data/v8.0/accounts(89390c24-9c72-e511-80d4-00155d2a68d1)"
 };
 var incidentUri;

 Sdk.WebAPISample.create("incidents", incident, function (incidentUri) {
  incidentUri = incidentUri;
  console.log("incident created");

  taskTemplate.subject = "Sample Incident Task 1";
  taskTemplate.actualdurationminutes = 10;
  taskTemplate["regardingobjectid_incident_task@odata.bind"] = incidentUri;

  Sdk.WebAPISample.create("tasks", taskTemplate, function (task1Uri) {
   console.log("task1 created");
   Sdk.WebAPISample.update(task1Uri, { statecode: 1, statuscode: 5 }, function () {
    console.log("task1 closed");

    taskTemplate.subject = "Sample Incident Task 2";
    taskTemplate.actualdurationminutes = 20;
    taskTemplate["regardingobjectid_incident_task@odata.bind"] = incidentUri;

    Sdk.WebAPISample.create("tasks", taskTemplate, function (task2Uri) {
     console.log("task2 created");
     Sdk.WebAPISample.update(task2Uri, { statecode: 1, statuscode: 5 }, function () {
      console.log("task2 closed");

      Sdk.WebAPISample.invokeInstanceBoundFunction(incidentUri, "CalculateTotalTimeIncident", null, function (response) {
       console.log("TotalTime: " + response.TotalTime);

      }, sampleErrorHandler);
     }, sampleErrorHandler);
    }, sampleErrorHandler);
   }, sampleErrorHandler);
  }, sampleErrorHandler);
 }, sampleErrorHandler);
}

//var user = "{ \"systemuserid\":\"568a0c00-b976-e511-80d4-00155d2a68d1\" }";

//var parameters = ["EntityLogicalName=account",
//    "FormType=2",
//"User=" + user];

//Sdk.WebAPISample.invokeBoundFunction("systemforms", "RetrieveFilteredForms", parameters, function (results) {
//    console.log("successs");
//}, sampleErrorHandler);

//var tomtuserid = "568a0c00-b976-e511-80d4-00155d2a68d1";
//var tomsAccount = "http://jdaly803/Contoso/api/data/v8.0/accounts(3e9711ef-b976-e511-80d4-00155d2a68d1)";

//var navProps = ["createdby($select=fullname)", "createdonbehalfby($select=fullname)", "owninguser($select=fullname)"]

//Sdk.WebAPISample.retrieve(tomsAccount, ["name"], navProps, function (results) {
//    console.log("success");

//}, sampleErrorHandler);
//Sdk.WebAPISample.create("accounts", { name: "Sample Account created using impersonation" }, function () {
//    console.log("success");
//}, sampleErrorHandler, tomtuserid);

//Sdk.WebAPISample.queryEntitySet("systemusers", "$select=fullname", false, 3, function (results) {
//    console.log("success");

//}, sampleErrorHandler);

//Sdk.WebAPISample.queryEntitySet("accounts", "$filter=name eq 'A. Datum Corporation (sample)'&$select=name,accountid", false, 1, function (result) {

//    var accountid = result[0].accountid;
//    console.log(accountid);
//    var accountUri = Sdk.WebAPISample.getWebAPIPath() + "accounts(" + accountid + ")/Account_Tasks/$count";
//    var properties = null;
//    var navigationProperties = null;
//    Sdk.WebAPISample.retrieve(accountUri, properties, navigationProperties, function (results) {
//        console.log("success");
//    }, sampleErrorHandler)

//}, sampleErrorHandler);

//var contact = { firstname:"Jim", lastname:"Daly"};

//Sdk.WebAPISample.create("contacts", contact, function (contactURi) {

//    var account = {
//        "name": "Sample Account",
//        "primarycontactid@odata.bind": contactURi
//    };
//    Sdk.WebAPISample.create("accounts", account, function (id) {
//        console.log(id);
//    }, sampleErrorHandler);

//}, sampleErrorHandler);




//var query = "<fetch mapping='logical'><entity name='account'><attribute name='accountid'/><attribute name='name'/></entity></fetch>";

//Sdk.WebAPISample.queryEntitySet("accounts", "fetchXml=" + query, false, 2, function (results) {
//    console.log("success");
//}, sampleErrorHandler);

//var mysavedquery = "121c6fd8-1975-e511-80d4-00155d2a68d1";
//var openOpportunitiesView = "00000000-0000-0000-00aa-000010003001";
//var fabrikamId = "8f390c24-9c72-e511-80d4-00155d2a68d1";

//var fabrikamOpenOpps = ["opportunity_parent_account"]

//Sdk.WebAPISample.retrieve(Sdk.WebAPISample.getWebAPIPath() + "accounts(" + fabrikamId + ")/opportunity_parent_account/?savedQuery=" + openOpportunitiesView, null, null, function (results) {
//    console.log(results.length)
//}, sampleErrorHandler);

//Sdk.WebAPISample.queryEntitySet("accounts","$filter=startswith(name,'Fab')&$select=accountid", false, null, function (results) {
//    console.log(results.length);
//}, sampleErrorHandler);

//Sdk.WebAPISample.queryEntitySet("accounts", "userQuery=" + mysavedquery, false, null, function (results) {
//    console.log(results.length);
//}, sampleErrorHandler);

//Sdk.WebAPISample.queryEntitySet("userqueries", null, false, null, function (results) {
//    console.log("success");

//}, sampleErrorHandler);

//Sdk.WebAPISample.queryEntitySet("savedqueries", "$select=name,savedqueryid&$filter=name eq 'Open Opportunities'", false, null, function (results) {
//    console.log(results[0].savedqueryid)

//}, sampleErrorHandler);

//Sdk.WebAPISample.queryEntitySet("accounts", "$select=name,industrycode", true, 3, function (results) {
//    console.log("Success");
//}, sampleErrorHandler);

//var activeAccountsSavedQueryId = "00000000-0000-0000-00aa-000010001002"

//Sdk.WebAPISample.getSavedQueryResults("accounts", activeAccountsSavedQueryId, false, null, function (results) {
//    console.log("success");
//}, sampleErrorHandler);

//createCustomEntity();
//var relationshipUri = Sdk.WebAPISample.getWebAPIPath() + "RelationshipDefinitions(bbe3ec07-0175-e511-80d4-00155d2a68d1)";
//Sdk.WebAPISample.retrieve(relationshipUri, null, null, function (relationship) {

//    relationship.ReferencedEntityNavigationPropertyName = "new_relatedTest"
//    relationship.ReferencingEntityNavigationPropertyName = "new_ParentAccount";
//    Sdk.WebAPISample.updateMetadata(relationshipUri, relationship, function (result) {
//        console.log("success");
//    }, sampleErrorHandler);

//}, sampleErrorHandler)

//Sdk.WebAPISample.queryEntitySet("EntityDefinitions", "$filter=LogicalName eq 'new_test'", false, 1, function (results) {

//    var entity = results.value[0];
//    entity.EntitySetName = "new_exams";
//    var id = entity.MetadataId;

//    Sdk.WebAPISample.updateMetadata(Sdk.WebAPISample.getWebAPIPath() + "/EntityDefinitions(" + id + ")", entity, function () {
//        console.log("entity updated");
//    }, sampleErrorHandler);

//    console.log("success");
//}, sampleErrorHandler);

//}

var stringValue = {
 "@odata.type": "#Microsoft.Dynamics.CRM.Label",
 "LocalizedLabels": [
     {
      "@odata.type": "#Microsoft.Dynamics.CRM.LocalizedLabel",
      "Label": "Bank Account Name",
      "LanguageCode": 1033
     }
 ],
 "UserLocalizedLabel": {
  "@odata.type": "#Microsoft.Dynamics.CRM.LocalizedLabel",
  "Label": "Bank Account Name",
  "LanguageCode": 1033
 }
};

function reviver(k, v) {

 if (this["@odata.type"]) {
  var type;
  switch (this["@odata.type"]) {
   case "#Microsoft.Dynamics.CRM.Label":
    type = new Sdk.WebAPIModelSample.Label();
    break;
   case "#Microsoft.Dynamics.CRM.LocalizedLabel":
    type = new Sdk.WebAPIModelSample.LocalizedLabel();
    break;
   default:
    break;

  }
  for (var i in this) {
   type[i] = JSON.parse(JSON.stringify(this[i]), reviver);
  }
  return type;
 }
 //switch (k) {
 //    case "@odata.type":
 //        console.log(v);

 //        break;
 //    default:
 //        break;

 //}


 return v;
}

function reviverStudy() {

 var mns = Sdk.WebAPIModelSample; //Model namespace


 var string = JSON.stringify(new mns.Label(new mns.LocalizedLabel("Bank Account Name", 1033)));
 //console.log(string);
 var obj = JSON.parse(string, reviver);

}

function createCustomEntity() {
 var mns = Sdk.WebAPIModelSample; //Model namespace
 //Define Primary Attribute
 var pa = new mns.StringAttributeMetadata();
 pa.DisplayName = new mns.Label(new mns.LocalizedLabel("Bank Account Name", 1033));
 pa.Description = new mns.Label(new mns.LocalizedLabel("Type the name of the bank account.", 1033));
 pa.SchemaName = "sdk_BankAccountName";
 pa.RequiredLevel = new mns.AttributeRequiredLevelManagedProperty(mns.AttributeRequiredLevel.ApplicationRequired, true);
 pa.MaxLength = 100;
 pa.FormatName = mns.StringFormatName.Text;
 pa.IsPrimaryName = true; //Required

 //Define the custom entity
 var ce = new mns.EntityMetadata();
 ce.SchemaName = "sdk_BankAccount";
 ce.DisplayName = new mns.Label(new mns.LocalizedLabel("Bank Account", 1033));
 ce.DisplayCollectionName = new mns.Label(new mns.LocalizedLabel("Bank Accounts", 1033));
 ce.Description = new mns.Label(new mns.LocalizedLabel("An entity to store information about customer bank accounts.", 1033));
 ce.OwnershipType = mns.OwnershipTypes.UserOwned;
 ce.IsActivity = false;
 ce.HasNotes = false;
 ce.HasActivities = false;
 ce.PrimaryNameAttribute = pa.SchemaName;
 ce.Attributes = [pa];


 Sdk.WebAPISample.create("EntityDefinitions", ce, function (entityUri) {
  console.log("entity created");
  createdEntityUri = entityUri;
  //NEXT EXAMPLE
  // attributeOperations();
 }, sampleErrorHandler);

}

function attributeOperations() {

 //Model namespace alias
 //Define custom string Attribute
 var ca = new mns.StringAttributeMetadata();
 ca.DisplayName = new mns.Label(new mns.LocalizedLabel("Custom String Attribute", 1033));
 ca.Description = new mns.Label(new mns.LocalizedLabel("The description of the custom string attribute.", 1033));
 ca.SchemaName = "sdk_CustomAttribute";
 ca.RequiredLevel = new mns.AttributeRequiredLevelManagedProperty(mns.AttributeRequiredLevel.None, true);
 ca.MaxLength = 200;
 ca.FormatName = mns.StringFormatName.Text;

 Sdk.WebAPISample.createAttribute(createdEntityUri, ca, function (customAttributeUri) {
  console.log("Custom Attribute created.");
  console.log(customAttributeUri);
  Sdk.WebAPISample.retrieve(customAttributeUri, null, null, function (attribute) {

   attribute.DisplayName = new mns.Label(new mns.LocalizedLabel("Updated Custom String Attribute", 1033));
   attribute.Description = new mns.Label(new mns.LocalizedLabel("The updated description of the custom string attribute.", 1033));

   Sdk.WebAPISample.updateMetadata(customAttributeUri, attribute, function () {
    console.log("Attribute Updated");
    Sdk.WebAPISample.del(customAttributeUri, function () {
     console.log("Custom atribute deleted.")
     //NEXT EXAMPLE
     updateEntity();
    }, sampleErrorHandler);
   }, sampleErrorHandler)
  }, sampleErrorHandler)
 }, sampleErrorHandler);

}

function createBooleanAttribute() {
 var accountMetadataId = "70816501-edb9-4740-a16c-6a5efbc05d84";

 var ba = new mns.BooleanAttributeMetadata();

 ba.DisplayName = new mns.Label(new mns.LocalizedLabel("Custom Boolean Attribute", 1033));
 ba.Description = new mns.Label(new mns.LocalizedLabel("The description of the custom boolean attribute.", 1033));
 ba.SchemaName = "sdk_CustomBooleanAttribute";
 ba.RequiredLevel = new mns.AttributeRequiredLevelManagedProperty(mns.AttributeRequiredLevel.None, true);
 ba.AttributeTypeName = mns.AttributeTypeDisplayName.BooleanType;

 var options = new mns.BooleanOptionSetMetadata();
 options.OptionSetType = mns.OptionSetType.Boolean;
 var fo = new mns.OptionMetadata()
 fo.Label = new mns.Label(new mns.LocalizedLabel("False", 1033))
 fo.Value = 0;
 options.FalseOption = fo;

 var to = new mns.OptionMetadata();
 to.Label = new mns.Label(new mns.LocalizedLabel("True", 1033))
 to.Value = 1;
 options.TrueOption = to;

 ba.OptionSet = options;

 Sdk.WebAPISample.createAttribute("http://jdalyv8ga/Contoso/api/data/v8.0/EntityDefinitions(70816501-edb9-4740-a16c-6a5efbc05d84)", ba, function () {
  console.log("success");
 }, sampleErrorHandler);

 //Sdk.WebAPISample.create("EntityDefinitions(" + accountMetadataId + ")/Attributes", ba, function () {
 // console.log("Success");
 //}, sampleErrorHandler);


}

function updateEntity() {
 //Retrieve the current entity definition
 Sdk.WebAPISample.retrieve(createdEntityUri, null, null, function (customEntity) {
  //update properties
  customEntity.DisplayName = new mns.Label(new mns.LocalizedLabel("Custom Entity", 1033));;
  customEntity.EntityColor = "#43B89F";

  //Has to be done using PUT with the entire entity definition.
  Sdk.WebAPISample.updateMetadata(createdEntityUri, customEntity, function () {
   console.log("entity updated");
   //NEXT EXAMPLE
   deleteEntity();
  }, sampleErrorHandler);
 }, sampleErrorHandler)
}

function deleteEntity() {
 Sdk.WebAPISample.del(createdEntityUri, function () {
  console.log("entity deleted");
 }, sampleErrorHandler);
}