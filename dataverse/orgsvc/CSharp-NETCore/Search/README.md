---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample demonstrates how to perform search operations using the Dataverse SDK for .NET."
---
# SDK for .NET search operations sample

This .NET 6.0 sample demonstrates how to perform search operations using the Dataverse SDK for .NET.

This sample uses:

- Classes generated using the [pac modelbuilder build command](https://learn.microsoft.com/power-platform/developer/cli/reference/modelbuilder#pac-modelbuilder-build). [Learn more about using this command](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/generate-early-bound-classes).

   These classes are in the `model/Messages` folder.

- Helper classes in the types folder.

  These classes enable deserializing the string values returned by the search apis.

## Prerequisites

- Microsoft Visual Studio 2022
- Access to Dataverse with privileges to perform data operations.

## How to run the sample

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Open the [PowerApps-Samples/dataverse/orgsvc/CSharp-NETCore/Search/Search.sln](Search.sln) file using Visual Studio 2022.
1. Edit the *appsettings.json* file. Set the connection string `Url` and `Username` parameters as appropriate for your test environment.

   The environment Url can be found in the Power Platform admin center. It has the form `https://\<environment-name>.crm.dynamics.com`.

1. Build the solution, and then run the **Search** project.

When the sample runs, you will be prompted in the default browser to select an environment user account and enter a password. To avoid having to do this every time you run a sample, insert a password parameter into the connection string in the appsettings.json file. For example:

```json
{
"ConnectionStrings": {
    "default": "AuthType=OAuth;Url=https://myorg.crm.dynamics.com;Username=someone@myorg.onmicrosoft.com;Password=mypassword;RedirectUri=http://localhost;AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;LoginPrompt=Auto"
  }
}
```

>**Tip**: You can set a user environment variable named DATAVERSE_APPSETTINGS to the file path of the appsettings.json file stored anywhere on your computer. The samples will use that appsettings file if the environment variable exists and is not null. Be sure to log out and back in again after you define the variable for it to take affect. To set an environment variable, go to **Settings > System > About**, select **Advanced system settings**, and then choose **Environment variables**.

## Demonstrates

This sample has 5 static methods that demonstrate the search API capabilities.

Before using these methods, the program verifies that search is enabled for the environment using the `CheckSearchStatus` method.

If search is not provisioned, the program offers the option to enable it using the `EnableSearch` method.
You can run the sample again to view the results.

If search is provisioned for the environment, the following example methods are invoked:

### OutputSearchQuery

This example method displays information from the `searchquery` message
using the search term 'Contoso'. The output may look something like this:

```
OutputSearchQuery START

        Count:1
        Value:
                Id:197b4431-db4c-ee11-be6f-00224809d350
                EntityName:account
                ObjectTypeCode:0
                Attributes:
                        @search.objecttypecode:1
                        name:Contoso Pharmaceuticals (sample)
                        createdon:9/6/2023 5:31:46 PM
                        createdon@OData.Community.Display.V1.FormattedValue:9/6/2023 5:31 PM
                Highlights:
                        name:
                                {crmhit}Contoso{/crmhit} Pharmaceuticals (sample):
                Score:4.8729024

OutputSearchQuery END
```

#### Dependencies

The `OutputSearchQuery` method depends on the following helper classes:

|Full Name|Description|
|---|---|
|`model/Messages/searchquery.cs`|Contains data to send the `searchquery` message and process the response.|
|`types/SearchEntity.cs`|The entity schema to scope the search request. Used by the the `searchquery.Entities` property.|
|`types/SearchQueryResults.cs`|Contains the response from the search query request.|
|`types/ErrorDetail.cs`|Contains information about errors that may be returned with the request.|
|`types/QueryResult.cs`|Contains data about a matching record found from the search query request.|
|`types/FacetResult.cs`|A facet query result that reports the number of documents with a field falling within a particular range or having a particular value or interval.|
|`types/FacetType.cs`|Specifies the type of a facet query result.|
|`types/QueryContext.cs`|The query context returned as part of response.|

### OutputSearchSuggest

This example method displays information from the `searchsuggest` message
using the search term 'cont'. The output may look something like this:

```
OutputSearchSuggest START

        Text:{crmhit}cont{/crmhit}act
        Document:
                @search.objectid: 217b4431-db4c-ee11-be6f-00224809d350
                @search.entityname: contact
                @search.objecttypecode: 2
                fullname: Yvonne McKay (sample)

        Text:{crmhit}cont{/crmhit}act
        Document:
                @search.objectid: 237b4431-db4c-ee11-be6f-00224809d350
                @search.entityname: contact
                @search.objecttypecode: 2
                fullname: Susanna Stubberod (sample)

        Text:{crmhit}cont{/crmhit}act
        Document:
                @search.objectid: 257b4431-db4c-ee11-be6f-00224809d350
                @search.entityname: contact
                @search.objecttypecode: 2
                fullname: Nancy Anderson (sample)

OutputSearchSuggest END
```

#### Dependencies

The `OutputSearchSuggest` method depends on the following helper classes:

|Full Name|Description|
|---|---|
|`model/Messages/searchsuggest.cs`|Contains data to send the `searchsuggest` message and process the response.|
|`types/SearchSuggestResults.cs`|Contains the data from the searchsuggestResponse response property|
|`types/SuggestResult.cs`|Result object for suggest results.|
|`types/ErrorDetail.cs`|Contains information about errors that may be returned with the request.|
|`types/QueryContext.cs`|The query context returned as part of response.|

### OutputAutoComplete

This example method displays information from the `searchautocomplete message` using the search term 'Con'.
The output may look something like this:

```
OutputAutoComplete START

        Search: Con
        Value: {crmhit}contoso{/crmhit}

OutputAutoComplete END
```

#### Dependencies

The `OutputAutoComplete` method depends on the following helper classes:

|Full Name|Description|
|---|---|
|`model/Messages/searchautocomplete.cs`|Contains data to send the `searchautocomplete` message and process the response.|
|`types/SearchSuggestResults.cs`|Contains the data from the searchsuggestResponse response property|
|`types/SearchEntity.cs`|The entity schema to scope the search request. Used by the the `searchautocompleteRequest.entities` property.|

### OutputSearchStatus

This example method displays information from the `searchstatus` message.
The output may look something like this:

```
OutputSearchStatus START

        Status: Provisioned
        LockboxStatus: Disabled
        CMKStatus: Disabled
        Entity Status Results:

                entitylogicalname: account
                objecttypecode: 1
                primarynamefield: name
                lastdatasynctimestamp: 1758985!09/06/2023 18:18:51
                lastprincipalobjectaccesssynctimestamp: 0!09/06/2023 18:18:51
                entitystatus: EntitySyncComplete
                searchableindexedfieldinfomap:
                        accountid        indexfieldname:a_0
                        accountnumber    indexfieldname:a0f
                        address1_city    indexfieldname:a0g
                        createdon        indexfieldname:i_0
                        emailaddress1    indexfieldname:a0h
                        entityimage_url  indexfieldname:h_0
                        modifiedon       indexfieldname:j_0
                        name     indexfieldname:d_0
                        ownerid  indexfieldname:b_0
                        owningbusinessunit       indexfieldname:c_0
                        primarycontactid         indexfieldname:a0i
                        statecode        indexfieldname:f_0
                        statuscode       indexfieldname:g_0
                        telephone1       indexfieldname:a0l
                        telephone2       indexfieldname:a0m
                        versionnumber    indexfieldname:e_0


                entitylogicalname: activitymimeattachment
                objecttypecode: 1001
                primarynamefield: filename
                lastdatasynctimestamp: 1758984!09/06/2023 18:14:59
                lastprincipalobjectaccesssynctimestamp:
                entitystatus: EntitySyncComplete
                searchableindexedfieldinfomap:
                        activitymimeattachmentid         indexfieldname:a_0
                        activitysubject  indexfieldname:a00
                        body     indexfieldname:l_0
                        filename         indexfieldname:d_0
                        mimetype         indexfieldname:a01
                        objectid         indexfieldname:a02
                        objecttypecode   indexfieldname:a05
                        ownerid  indexfieldname:b_0
                        owningbusinessunit       indexfieldname:c_0
                        versionnumber    indexfieldname:e_0


                entitylogicalname: annotation
                objecttypecode: 5
                primarynamefield: subject
                lastdatasynctimestamp: 1758985!09/06/2023 18:18:51
                lastprincipalobjectaccesssynctimestamp: 0!09/06/2023 18:18:52
                entitystatus: EntitySyncComplete
                searchableindexedfieldinfomap:
                        annotationid     indexfieldname:a_0
                        createdon        indexfieldname:i_0
                        documentbody     indexfieldname:k_0
                        filename         indexfieldname:a06
                        isdocument       indexfieldname:a07
                        mimetype         indexfieldname:a09
                        modifiedon       indexfieldname:j_0
                        notetext         indexfieldname:a0a
                        objectid         indexfieldname:a0b
                        objecttypecode   indexfieldname:a0e
                        ownerid  indexfieldname:b_0
                        owningbusinessunit       indexfieldname:c_0
                        subject  indexfieldname:d_0
                        versionnumber    indexfieldname:e_0


                entitylogicalname: appointment
                objecttypecode: 4201
                primarynamefield: subject
                lastdatasynctimestamp: 1758985!09/06/2023 18:18:51
                lastprincipalobjectaccesssynctimestamp: 0!09/06/2023 18:18:51
                entitystatus: EntitySyncComplete
                searchableindexedfieldinfomap:
                        activityid       indexfieldname:a_0
                        createdon        indexfieldname:i_0
                        formattedscheduledend    indexfieldname:a13
                        formattedscheduledstart  indexfieldname:a14
                        instancetypecode         indexfieldname:a15
                        location         indexfieldname:a17
                        modifiedon       indexfieldname:j_0
                        ownerid  indexfieldname:b_0
                        owningbusinessunit       indexfieldname:c_0
                        regardingobjecttypecode  indexfieldname:a18
                        scheduledend     indexfieldname:a19
                        scheduledstart   indexfieldname:a1a
                        statecode        indexfieldname:f_0
                        statuscode       indexfieldname:g_0
                        subject  indexfieldname:d_0
                        versionnumber    indexfieldname:e_0


                entitylogicalname: contact
                objecttypecode: 2
                primarynamefield: fullname
                lastdatasynctimestamp: 1758985!09/06/2023 18:18:51
                lastprincipalobjectaccesssynctimestamp: 0!09/06/2023 18:18:51
                entitystatus: EntitySyncComplete
                searchableindexedfieldinfomap:
                        address1_city    indexfieldname:a0r
                        address1_telephone1      indexfieldname:a0s
                        contactid        indexfieldname:a_0
                        createdon        indexfieldname:i_0
                        emailaddress1    indexfieldname:a0t
                        entityimage_url  indexfieldname:h_0
                        firstname        indexfieldname:a0u
                        fullname         indexfieldname:d_0
                        lastname         indexfieldname:a0v
                        middlename       indexfieldname:a0w
                        mobilephone      indexfieldname:a0x
                        modifiedon       indexfieldname:j_0
                        ownerid  indexfieldname:b_0
                        owningbusinessunit       indexfieldname:c_0
                        parentcustomerid         indexfieldname:a0y
                        statecode        indexfieldname:f_0
                        statuscode       indexfieldname:g_0
                        telephone1       indexfieldname:a11
                        versionnumber    indexfieldname:e_0


                entitylogicalname: email
                objecttypecode: 4202
                primarynamefield: subject
                lastdatasynctimestamp: 1758985!09/06/2023 18:18:51
                lastprincipalobjectaccesssynctimestamp: 0!09/06/2023 18:18:51
                entitystatus: EntitySyncComplete
                searchableindexedfieldinfomap:
                        acceptingentityid        indexfieldname:a4h
                        activityid       indexfieldname:a_0
                        attachmentopencount      indexfieldname:a4k
                        createdon        indexfieldname:i_0
                        linksclickedcount        indexfieldname:a4l
                        modifiedon       indexfieldname:j_0
                        opencount        indexfieldname:a4m
                        ownerid  indexfieldname:b_0
                        owningbusinessunit       indexfieldname:c_0
                        regardingobjectid        indexfieldname:a4n
                        regardingobjecttypecode  indexfieldname:a4q
                        replycount       indexfieldname:a4r
                        statecode        indexfieldname:f_0
                        statuscode       indexfieldname:g_0
                        subject  indexfieldname:d_0
                        versionnumber    indexfieldname:e_0


                entitylogicalname: fax
                objecttypecode: 4204
                primarynamefield: subject
                lastdatasynctimestamp: 1758985!09/06/2023 18:18:51
                lastprincipalobjectaccesssynctimestamp: 0!09/06/2023 18:18:51
                entitystatus: EntitySyncComplete
                searchableindexedfieldinfomap:
                        activityid       indexfieldname:a_0
                        createdby        indexfieldname:a52
                        createdon        indexfieldname:i_0
                        modifiedon       indexfieldname:j_0
                        ownerid  indexfieldname:b_0
                        owningbusinessunit       indexfieldname:c_0
                        prioritycode     indexfieldname:a55
                        regardingobjectid        indexfieldname:a57
                        regardingobjecttypecode  indexfieldname:a5a
                        scheduledend     indexfieldname:a5b
                        statecode        indexfieldname:f_0
                        statuscode       indexfieldname:g_0
                        subject  indexfieldname:d_0
                        versionnumber    indexfieldname:e_0


                entitylogicalname: goal
                objecttypecode: 9600
                primarynamefield: title
                lastdatasynctimestamp: 1758985!09/06/2023 18:18:52
                lastprincipalobjectaccesssynctimestamp: 0!09/06/2023 18:18:52
                entitystatus: EntitySyncComplete
                searchableindexedfieldinfomap:
                        actualstring     indexfieldname:a1b
                        createdon        indexfieldname:i_0
                        entityimage_url  indexfieldname:h_0
                        fiscalperiod     indexfieldname:a1c
                        fiscalyear       indexfieldname:a1e
                        goalenddate      indexfieldname:a1g
                        goalid   indexfieldname:a_0
                        goalownerid      indexfieldname:a1h
                        goalstartdate    indexfieldname:a1k
                        inprogressstring         indexfieldname:a1l
                        metricid         indexfieldname:a1m
                        modifiedon       indexfieldname:j_0
                        ownerid  indexfieldname:b_0
                        owningbusinessunit       indexfieldname:c_0
                        parentgoalid     indexfieldname:a1p
                        percentage       indexfieldname:a1s
                        statecode        indexfieldname:f_0
                        statuscode       indexfieldname:g_0
                        targetstring     indexfieldname:a1t
                        title    indexfieldname:d_0
                        versionnumber    indexfieldname:e_0


                entitylogicalname: knowledgearticle
                objecttypecode: 9953
                primarynamefield: title
                lastdatasynctimestamp: 1759042!09/06/2023 18:19:21
                lastprincipalobjectaccesssynctimestamp: 0!09/06/2023 18:19:23
                entitystatus: EntitySyncComplete
                searchableindexedfieldinfomap:
                        articlepublicnumber      indexfieldname:a2g
                        content  indexfieldname:a2h
                        createdby        indexfieldname:a2i
                        createdon        indexfieldname:i_0
                        createdonbehalfby        indexfieldname:a2l
                        description      indexfieldname:a2o
                        isinternal       indexfieldname:a2p
                        islatestversion  indexfieldname:a2r
                        isprimary        indexfieldname:a2t
                        isrootarticle    indexfieldname:a2v
                        keywords         indexfieldname:a2x
                        knowledgearticleid       indexfieldname:a_0
                        knowledgearticleviews    indexfieldname:a2y
                        languagelocaleid         indexfieldname:a2z
                        majorversionnumber       indexfieldname:a32
                        minorversionnumber       indexfieldname:a33
                        modifiedby       indexfieldname:a34
                        modifiedon       indexfieldname:j_0
                        msdyn_contentstore       indexfieldname:a37
                        msdyn_externalreferenceid        indexfieldname:a38
                        msdyn_ingestedarticleurl         indexfieldname:a39
                        msdyn_integratedsearchproviderid         indexfieldname:a3a
                        msdyn_isingestedarticle  indexfieldname:a3d
                        ownerid  indexfieldname:b_0
                        owningbusinessunit       indexfieldname:c_0
                        owningteam       indexfieldname:a3f
                        owninguser       indexfieldname:a3i
                        parentarticlecontentid   indexfieldname:a3l
                        previousarticlecontentid         indexfieldname:a3o
                        rating   indexfieldname:a3r
                        statecode        indexfieldname:f_0
                        statuscode       indexfieldname:g_0
                        subjectid        indexfieldname:a3s
                        title    indexfieldname:d_0
                        transactioncurrencyid    indexfieldname:a3v
                        versionnumber    indexfieldname:e_0


                entitylogicalname: letter
                objecttypecode: 4207
                primarynamefield: subject
                lastdatasynctimestamp: 1758985!09/06/2023 18:18:52
                lastprincipalobjectaccesssynctimestamp: 0!09/06/2023 18:18:52
                entitystatus: EntitySyncComplete
                searchableindexedfieldinfomap:
                        activityid       indexfieldname:a_0
                        createdby        indexfieldname:a26
                        createdon        indexfieldname:i_0
                        modifiedon       indexfieldname:j_0
                        ownerid  indexfieldname:b_0
                        owningbusinessunit       indexfieldname:c_0
                        prioritycode     indexfieldname:a29
                        regardingobjectid        indexfieldname:a2b
                        regardingobjecttypecode  indexfieldname:a2e
                        scheduledend     indexfieldname:a2f
                        statecode        indexfieldname:f_0
                        statuscode       indexfieldname:g_0
                        subject  indexfieldname:d_0
                        versionnumber    indexfieldname:e_0


                entitylogicalname: metric
                objecttypecode: 9603
                primarynamefield: name
                lastdatasynctimestamp: 1758985!09/06/2023 18:18:52
                lastprincipalobjectaccesssynctimestamp: 0!09/06/2023 18:18:52
                entitystatus: EntitySyncComplete
                searchableindexedfieldinfomap:
                        amountdatatype   indexfieldname:a0n
                        createdon        indexfieldname:i_0
                        isamount         indexfieldname:a0p
                        metricid         indexfieldname:a_0
                        modifiedon       indexfieldname:j_0
                        name     indexfieldname:d_0
                        statecode        indexfieldname:f_0
                        statuscode       indexfieldname:g_0
                        versionnumber    indexfieldname:e_0


                entitylogicalname: msdyn_kbattachment
                objecttypecode: 10108
                primarynamefield: msdyn_filename
                lastdatasynctimestamp: 1758985!09/06/2023 18:18:52
                lastprincipalobjectaccesssynctimestamp: 0!09/06/2023 18:18:52
                entitystatus: EntitySyncComplete
                searchableindexedfieldinfomap:
                        createdon        indexfieldname:i_0
                        modifiedon       indexfieldname:j_0
                        msdyn_fileattachment     indexfieldname:a3y
                        msdyn_filename   indexfieldname:d_0
                        msdyn_kbattachmentid     indexfieldname:a_0
                        ownerid  indexfieldname:b_0
                        owningbusinessunit       indexfieldname:c_0
                        statecode        indexfieldname:f_0
                        statuscode       indexfieldname:g_0
                        versionnumber    indexfieldname:e_0


                entitylogicalname: mspcat_catalogsubmissionfiles
                objecttypecode: 10217
                primarynamefield: mspcat_name
                lastdatasynctimestamp: 1758985!09/06/2023 18:18:52
                lastprincipalobjectaccesssynctimestamp: 0!09/06/2023 18:18:52
                entitystatus: EntitySyncComplete
                searchableindexedfieldinfomap:
                        createdon        indexfieldname:i_0
                        modifiedon       indexfieldname:j_0
                        mspcat_catalogsubmissionfilesid  indexfieldname:a_0
                        mspcat_name      indexfieldname:d_0
                        ownerid  indexfieldname:b_0
                        owningbusinessunit       indexfieldname:c_0
                        statecode        indexfieldname:f_0
                        statuscode       indexfieldname:g_0
                        versionnumber    indexfieldname:e_0


                entitylogicalname: phonecall
                objecttypecode: 4210
                primarynamefield: subject
                lastdatasynctimestamp: 1758985!09/06/2023 18:18:52
                lastprincipalobjectaccesssynctimestamp: 0!09/06/2023 18:18:52
                entitystatus: EntitySyncComplete
                searchableindexedfieldinfomap:
                        activityid       indexfieldname:a_0
                        createdby        indexfieldname:a47
                        createdon        indexfieldname:i_0
                        modifiedon       indexfieldname:j_0
                        ownerid  indexfieldname:b_0
                        owningbusinessunit       indexfieldname:c_0
                        prioritycode     indexfieldname:a4a
                        regardingobjectid        indexfieldname:a4c
                        regardingobjecttypecode  indexfieldname:a4f
                        scheduledend     indexfieldname:a4g
                        statecode        indexfieldname:f_0
                        statuscode       indexfieldname:g_0
                        subject  indexfieldname:d_0
                        versionnumber    indexfieldname:e_0


                entitylogicalname: recurringappointmentmaster
                objecttypecode: 4251
                primarynamefield: subject
                lastdatasynctimestamp: 1758985!09/06/2023 18:18:52
                lastprincipalobjectaccesssynctimestamp: 0!09/06/2023 18:18:52
                entitystatus: EntitySyncComplete
                searchableindexedfieldinfomap:
                        activityid       indexfieldname:a_0
                        createdon        indexfieldname:i_0
                        modifiedon       indexfieldname:j_0
                        ownerid  indexfieldname:b_0
                        owningbusinessunit       indexfieldname:c_0
                        regardingobjecttypecode  indexfieldname:a12
                        statecode        indexfieldname:f_0
                        statuscode       indexfieldname:g_0
                        subject  indexfieldname:d_0
                        versionnumber    indexfieldname:e_0


                entitylogicalname: socialactivity
                objecttypecode: 4216
                primarynamefield: subject
                lastdatasynctimestamp: 1758985!09/06/2023 18:18:52
                lastprincipalobjectaccesssynctimestamp: 0!09/06/2023 18:18:52
                entitystatus: EntitySyncComplete
                searchableindexedfieldinfomap:
                        activityid       indexfieldname:a_0
                        community        indexfieldname:a1u
                        createdon        indexfieldname:i_0
                        modifiedon       indexfieldname:j_0
                        ownerid  indexfieldname:b_0
                        owningbusinessunit       indexfieldname:c_0
                        postfromprofileid        indexfieldname:a1w
                        prioritycode     indexfieldname:a1z
                        regardingobjectid        indexfieldname:a21
                        regardingobjecttypecode  indexfieldname:a24
                        sentimentvalue   indexfieldname:a25
                        statecode        indexfieldname:f_0
                        statuscode       indexfieldname:g_0
                        subject  indexfieldname:d_0
                        versionnumber    indexfieldname:e_0


                entitylogicalname: socialprofile
                objecttypecode: 99
                primarynamefield: profilename
                lastdatasynctimestamp: 1758985!09/06/2023 18:18:52
                lastprincipalobjectaccesssynctimestamp: 0!09/06/2023 18:18:52
                entitystatus: EntitySyncComplete
                searchableindexedfieldinfomap:
                        blocked  indexfieldname:a3z
                        community        indexfieldname:a41
                        createdon        indexfieldname:i_0
                        customerid       indexfieldname:a43
                        modifiedon       indexfieldname:j_0
                        ownerid  indexfieldname:b_0
                        owningbusinessunit       indexfieldname:c_0
                        profilefullname  indexfieldname:a46
                        profilename      indexfieldname:d_0
                        socialprofileid  indexfieldname:a_0
                        statecode        indexfieldname:f_0
                        statuscode       indexfieldname:g_0
                        versionnumber    indexfieldname:e_0


                entitylogicalname: task
                objecttypecode: 4212
                primarynamefield: subject
                lastdatasynctimestamp: 1758985!09/06/2023 18:18:52
                lastprincipalobjectaccesssynctimestamp: 0!09/06/2023 18:18:52
                entitystatus: EntitySyncComplete
                searchableindexedfieldinfomap:
                        activityid       indexfieldname:a_0
                        createdby        indexfieldname:a4s
                        createdon        indexfieldname:i_0
                        modifiedon       indexfieldname:j_0
                        ownerid  indexfieldname:b_0
                        owningbusinessunit       indexfieldname:c_0
                        prioritycode     indexfieldname:a4v
                        regardingobjectid        indexfieldname:a4x
                        regardingobjecttypecode  indexfieldname:a50
                        scheduledend     indexfieldname:a51
                        statecode        indexfieldname:f_0
                        statuscode       indexfieldname:g_0
                        subject  indexfieldname:d_0
                        versionnumber    indexfieldname:e_0


OutputSearchStatus END
```

#### Dependencies

The `OutputSearchStatus` method depends on the following helper classes:

|Full Name|Description|
|---|---|
|`model/Messages/searchstatus.cs`|Contains data to send the `searchstatus` message and process the response.|
|`types/SearchStatusResult.cs`|Contains the data from the searchstatusResponse response property|
|`types/SearchStatus.cs`|Contains the data from the SearchStatusResult.Status property|
|`types/LockboxStatus.cs`|Contains the data from the SearchStatusResult.LockboxStatus property|
|`types/CMKStatus.cs`|Contains the data from the SearchStatusResult.CMKStatus property|
|`types/EntityStatusInfo.cs`|Contains the data from the SearchStatusResult.EntityStatusInfo property|
|`types/ManyToManyRelationshipSyncStatus.cs`|Contains the data from the SearchStatusResult.ManyToManyRelationshipSyncStatus property|
|`types/FieldStatusInfo.cs`|Contains the data from the EntityStatusInfo.SearchableIndexedFieldInfoMap property|

## OutputSearchStatistics

This example method displays information from the `searchstatistics` message.
The output may look something like this:

```
OutputSearchStatistics START

        StorageSizeInBytes: 1429925
        StorageSizeInMb: 1
        DocumentCount: 1223

OutputSearchStatistics END
```

#### Dependencies

The `OutputSearchStatistics` method depends on the following helper classes:

|Full Name|Description|
|---|---|
|`model/Messages/searchstatistics.cs`|Contains data to send the `searchstatistics` message and process the response.|
|`types/SearchStatisticsResult.cs`|Contains the data from the searchstatisticsResponse response property|

## Clean up

This sample doesn't create any records, but it does allow you to provision search if it isn't already enabled.
It doesn't provide the option to de-provision search.
