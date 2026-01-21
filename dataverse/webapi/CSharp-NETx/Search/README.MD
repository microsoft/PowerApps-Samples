---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample demonstrates how to perform search operations using the Dataverse Web API."
---

# Web API search operations sample

This .NET 6.0 sample demonstrates how to perform search operations using the Dataverse Web API.

This sample uses the common helper code in the [WebAPIService](../WebAPIService) class library project.

## Prerequisites

- Microsoft Visual Studio 2022
- Access to Dataverse with privileges to perform data operations

## How to run the sample

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Open the [Search.sln](Search.sln) file using Visual Studio 2022.
1. Edit the [appsettings.json](../appsettings.json) file to set the following property values:

   | Property | Instructions |
   |----------|--------------|
   | `Url` |The URL for your environment. Replace the placeholder `https://yourorg.api.crm.dynamics.com` value with the value for your environment. Learn how to find your URL in [View developer resources](https://learn.microsoft.com/power-apps/developer/data-platform/view-download-developer-resources). |
   | `UserPrincipalName` | Replace the placeholder `you@yourorg.onmicrosoft.com` value with the UPN value you use to access the environment. |
   | `Password` | Replace the placeholder `yourPassword` value with your password. |

1. Save the `appsettings.json` file.
1. Press `F5` to run the sample.

## Demonstrates

This sample has five static methods that demonstrate the search API capabilities.

Before using these methods, the program verifies that search is enabled for the environment using the `CheckSearchStatus` method.

If search isn't provisioned, the program offers the option to enable it using the `EnableSearch` method. You can run the sample again to view the results.

If search is provisioned for the environment, the following example methods are invoked:

### OutputSearchQuery

This example method displays information from the [Dataverse Search query](https://learn.microsoft.com/power-apps/developer/data-platform/search/query) API, using the search term *Contoso*.

The output might look similar to this output:

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

#### OutputSearchQuery dependencies

The `OutputSearchQuery` method depends on the following WebAPIService helper classes:

| Full Name | Description |
|-----------|-------------|
| `PowerApps.Samples.Search.Types.QueryParameters` | Contains parameter data to initialize the `SearchQueryRequest` class. |
| `PowerApps.Samples.Search.Types.SearchEntity` | The entity schema to scope the search request. Used by the the `QueryParameters.Entities` property. |
| `PowerApps.Samples.Search.Messages.SearchQueryRequest` | Contains the data to send a searchquery request. |
| `PowerApps.Samples.Search.Messages.SearchQueryResponse` | Contains the response from the `SearchQueryRequest` class. |
| `PowerApps.Samples.Search.Types.QueryResult` | Contains data about a matching record found from the query API. Used by the `SearchQueryResponse.Value` property. |
| `PowerApps.Samples.Search.Types.ErrorDetail` | The error detail returned as part of response. Used by the `SearchQueryResponse.Error` property. |
| `PowerApps.Samples.Search.Types.FacetResult` | A facet query result that reports the number of documents with a field falling within a particular range or having a particular value or interval. Used by the `SearchQueryResponse.Facets` property. |
| `PowerApps.Samples.Search.Types.QueryContext` | The query context returned as part of response. Used by the `SearchQueryResponse.QueryContext` property. |

### OutputSearchSuggest

This example method displays information from the [Dataverse Search suggest](https://learn.microsoft.com/power-apps/developer/data-platform/search/suggest) API, using the search term *cont*.

The output might look similar to this output:

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

#### OutputSearchSuggest dependencies

The `OutputSearchSuggest` method depends on the following WebAPIService helper classes:

| Full Name | Description |
|-----------|-------------|
| `PowerApps.Samples.Search.Types.SuggestParameters` | Contains parameter data to initialize the `SearchSuggestRequest` class. |
| `PowerApps.Samples.Search.Types.SearchEntity` | The entity schema to scope the search request. Used by the the `SuggestParameters.Entities` property. |
| `PowerApps.Samples.Search.Messages.SearchSuggestRequest` | Contains the data to send a searchsuggest request. |
| `PowerApps.Samples.Search.Messages.SearchSuggestResponse` | Contains the response from the `SearchSuggestRequest` class. |
| `PowerApps.Samples.Search.Types.ErrorDetail` | The error detail returned as part of response. Used by the `SearchSuggestResponse.Error` property. |
| `PowerApps.Samples.Search.Types.SuggestResult` | Result object for suggest results. Used by the `SearchSuggestResponse.Value` property. |
| `PowerApps.Samples.Search.Types.QueryContext` | The query context returned as part of response. Used by the `SearchSuggestResponse.QueryContext` property. |

### OutputAutoComplete

This example method displays information from the [Dataverse Search autocomplete](https://learn.microsoft.com/power-apps/developer/data-platform/search/autocomplete) API using the search term *Con*.

The output might look similar to this output:

```
OutputAutoComplete START

        Search: Con
        Value: {crmhit}contoso{/crmhit}

OutputAutoComplete END
```

#### OutputAutoComplete dependencies

The `OutputAutoComplete` method depends on the following WebAPIService helper classes:

| Full Name | Description |
|-----------|-------------|
| `PowerApps.Samples.Search.Types.AutocompleteParameters` | Contains parameter data to initialize the `SearchAutoCompleteRequest` class. |
| `PowerApps.Samples.Search.Types.SearchEntity` | The entity schema to scope the search request. Used by the the `AutocompleteParameters.Entities` property. |
| `PowerApps.Samples.Search.Messages.SearchAutoCompleteRequest` | Contains the data to send a searchautocomplete request. |
| `PowerApps.Samples.Search.Messages.SearchAutoCompleteResponse` | Contains the response from the `SearchAutoCompleteRequest` class. |
| `PowerApps.Samples.Search.Types.ErrorDetail` | The error detail returned as part of response. Used by the `SearchAutoCompleteResponse.Error` property. |
| `PowerApps.Samples.Search.Types.QueryContext` | The query context returned as part of response. Used by the `SearchAutoCompleteResponse.QueryContext` property. |

### OutputSearchStatus

This example method displays information from the [Dataverse Search status](https://learn.microsoft.com/power-apps/developer/data-platform/search/status) API.

The output might look similar to this output:

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

#### OutputSearchStatus dependencies

The `OutputSearchStatus` method depends on the following WebAPIService helper classes:

| Full Name | Description |
|-----------|-------------|
| `PowerApps.Samples.Search.Messages.SearchStatusRequest` | Contains the data to send a searchstatus request. |
| `PowerApps.Samples.Search.Messages.SearchStatusResponse` | Contains the response from the `SearchStatusRequest` class. |
| `PowerApps.Samples.Search.Types.SearchStatus` | Indicates the status of an entity type or the index as a whole. Used by the `SearchStatusResponse.Status` property. |
| `PowerApps.Samples.Search.Types.LockboxStatus` | Indicates the status of the lockbox. Used by the `SearchStatusResponse.LockboxStatus` property. |
| `PowerApps.Samples.Search.Types.CMKStatus` | Indicates the status of the customer managed key. Used by the `SearchStatusResponse.CMKStatus` property. |
| `PowerApps.Samples.Search.Types.EntityStatusInfo` | Data contract for response parameters for index status api. Used by the `SearchStatusResponse.EntityStatusInfo` property. |
| `PowerApps.Samples.Search.Types.ManyToManyRelationshipSyncStatus` | Data contract for response parameters for many to many relationship sync job status. Used by the `SearchStatusResponse.ManyToManyRelationshipSyncStatus` property. |

### OutputSearchStatistics

This example method displays information from the [Dataverse Search statistics](https://learn.microsoft.com/power-apps/developer/data-platform/search/statistics) API.

The output might look similar to this output:

```
OutputSearchStatistics START

        StorageSizeInBytes: 1429925
        StorageSizeInMb: 1
        DocumentCount: 1223

OutputSearchStatistics END
```

#### OutputSearchStatistics dependencies

The `OutputSearchStatistics` method depends on the following WebAPIService helper classes:

| Full Name | Description |
|-----------|-------------|
| `PowerApps.Samples.Search.Messages.SearchStatisticsRequest` | Contains the data to send a searchstatistics request. |
| `PowerApps.Samples.Search.Messages.SearchStatisticsResponse` | Contains the response from the `SearchStatisticsRequest` class. |

## Clean up

This sample doesn't create any records, but allows you to provision search if it isn't already enabled. However, the sample doesn't provide the option to de-provision search.
