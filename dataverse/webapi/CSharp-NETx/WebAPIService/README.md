# WebAPIService class library (C#)

WebAPIService is a sample .NET 6.0 class library project that demonstrates several important capabilities that you should include when you use the Dataverse Web API.

This library demonstrates:

- Managing Dataverse service protection limits with the .NET resilience and transient fault handling library [Polly](https://github.com/App-vNext/Polly).
- Managing an [HttpClient](https://learn.microsoft.com/dotnet/api/system.net.http.httpclient?view=net-6.0&viewFallbackFrom=dotnet-plat-ext-6.0) in .NET using [IHttpClientFactory](https://learn.microsoft.com/dotnet/api/system.net.http.ihttpclientfactory?view=dotnet-plat-ext-6.0).
- Using configuration data to manage the behavior of the client.
- Managing errors returned by Dataverse Web API.
- A pattern of code re-use by:

  - Creating classes that inherit from [HttpRequestMessage](https://learn.microsoft.com/dotnet/api/system.net.http.httprequestmessage?view=net-6.0) and [HttpResponseMessage](https://learn.microsoft.com/dotnet/api/system.net.http.httpresponsemessage?view=net-6.0).
  - Methods that leverage those classes.
  - A modular pattern for adding new capabilities as needed.

> [!NOTE]
> This sample library is a helper used by all the Dataverse C# Web API samples, but it's not an SDK. This sample is tested only to confirm that the samples that use it run successfully. The sample code is provided 'as-is' with no warranty for reuse.

## Limitations

This library doesn't:

### Manage authentication

- Authorization depends on a function passed from an application that provides the access token.
- All Web API samples depend on a shared [App class](../App.cs) that manages authentication using the [Microsoft Authentication Library (MSAL)](https://learn.microsoft.com/azure/active-directory/develop/msal-overview).
- MSAL supports several different types of authentication flows.
- These samples use the [Username/password (ROPC)](https://learn.microsoft.com/azure/active-directory/develop/msal-authentication-flows#usernamepassword-ropc) flow for simplicity, but this method isn't recommended. For your apps, you should use one of the other flows. Learn more in [Authentication flow support in the Microsoft Authentication Library](https://learn.microsoft.com/azure/active-directory/develop/msal-authentication-flows).

### Provide for any code generation capabilities

- All classes used in the samples are written by hand. 
- All business entity data uses the well-known [Json.NET JObject Class](https://www.newtonsoft.com/json/help/html/t_newtonsoft_json_linq_jobject.htm) rather than a class representing the entity type.

### Provide an object model for composing OData queries

All queries show the OData query syntax as query parameters.

## Class listing

The following classes are included in the WebAPIService.

### Service

The [Service class](Service.cs) provides methods to send requests to Dataverse through an [HttpClient](https://learn.microsoft.com/dotnet/api/system.net.http.httpclient?view=net-6.0&viewFallbackFrom=dotnet-plat-ext-6.0) managed using [IHttpClientFactory](https://learn.microsoft.com/dotnet/api/system.net.http.ihttpclientfactory?view=dotnet-plat-ext-6.0).

The Service is the core component for all samples and can complete any operations demonstrated with sample code. Everything else included in the WebAPIService, or any of the samples using it, provides for re-use of code and allows for the capabilities of the Dataverse Web API to be demonstrated at a higher level.

The Service constructor accepts a [Config class](#config) instance which contains two required properties: `GetAccessToken` and `Url`. All other properties represent options with defaults.

The constructor uses dependency injection to create an [IHttpClientFactory](https://learn.microsoft.com/dotnet/api/system.net.http.ihttpclientfactory?view=dotnet-plat-ext-6.0) that can return a named [HttpClient](https://learn.microsoft.com/dotnet/api/system.net.http.httpclient?view=net-6.0&viewFallbackFrom=dotnet-plat-ext-6.0) with the properties specified in the `ConfigureHttpClient` function. Whether or not this client uses cookies is based on whether the `Config.DisableCookies` parameter is set. In the constructor, the policy defined by the static `GetRetryPolicy` method controls how transient errors and Dataverse service protection limits are managed.

#### Service methods

The Service class has the following methods.

##### SendAsync method

This method is responsible for all operations.

This method:

- Has an [HttpRequestMessage](https://learn.microsoft.com/dotnet/api/system.net.http.httprequestmessage?view=net-6.0) parameter.
- Returns `Task<HttpResponseMessage>`.
- Exposes the same signature as the [HttpClient.SendAsync(HttpRequestMessage)](https://learn.microsoft.com/dotnet/api/system.net.http.httpclient.sendasync?view=net-6.0#system-net-http-httpclient-sendasync(system-net-http-httprequestmessage)) and can be used in the same way.
- Calls the function set in the `Config.GetAccessToken` method to set the `Authorization` header value for the request.
- Uses the [IHttpClientFactory.CreateClient Method](https://learn.microsoft.com/dotnet/api/system.net.http.ihttpclientfactory.createclient?view=dotnet-plat-ext-6.0) to get the named [HttpClient](https://learn.microsoft.com/dotnet/api/system.net.http.httpclient?view=net-6.0&viewFallbackFrom=dotnet-plat-ext-6.0) to send the request.
- Throws a [ServiceException](#serviceexception) if the [HttpResponseMessage.IsSuccessStatusCode Property](https://learn.microsoft.com/dotnet/api/system.net.http.httpresponsemessage.issuccessstatuscode?view=net-6.0) is false, so you don't need to check for this error when using this method.

##### SendAsync&lt;T&gt; method

This method facilitates returning a class that includes properties found in the [ComplexTypes](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/complextypes?view=dataverse-latest) returned by OData [Actions](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/actions?view=dataverse-latest) and [Functions](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/functions?view=dataverse-latest) in Dataverse Web API.

- Has an [HttpRequestMessage](https://learn.microsoft.com/dotnet/api/system.net.http.httprequestmessage?view=net-6.0) parameter. When using this method, it's expected, but not required, that the `request` parameter is one of the [*Response classes](#response-classes) that derive from `HttpRequestMessage`.
- Returns `Task<T>` where `T` is a class derived from [HttpResponseMessage](https://learn.microsoft.com/dotnet/api/system.net.http.httpresponsemessage?view=net-6.0). Learn more in [*Response classes](#response-classes).
- Calls the [SendAsync Method](#sendasync-method).
- Uses the [HttpResponseMessage.As&lt;T&gt;](#httpresponsemessage-ast) extension method to return the requested type.

An example using the [WhoAmI function](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/whoami):

```csharp
static async Task WhoAmI(Service service)
{
   var response = await service.SendAsync<WhoAmIResponse>(new WhoAmIRequest());

   Console.WriteLine($"Your user ID is {response.UserId}");
}
```

##### ParseError method

This method parses the content of an `HttpResponseMessage` to return a `ServiceException`. `ParseError` is used within the [SendAsync method](#sendasync-method) when the [HttpResponseMessage.IsSuccessStatusCode](https://learn.microsoft.com/dotnet/api/system.net.http.httpresponsemessage.issuccessstatuscode?view=net-6.0) property is false. You can also use this method to extract error information from `HttpResponseMessage` instances returned by `BatchResponse.HttpResponseMessages` when the `BatchRequest.ContinueOnError` property is set to true. Learn more in [Batch](#batch).

#### Service properties

Service has five properties:

- BaseAddress
- RecommendedDegreeOfParallelism
- UserId
- OrganizationId
- BusinessUnitId

##### BaseAddress property

This property returns the base URL set in the `Config.Url`. This base URL is needed when instantiating the `BatchRequest` class and to append to a relative URL anytime an absolute URL is required.

##### RecommendedDegreeOfParallelism property

This property returns the value of the `x-ms-dop-hint` response header from a `WhoAmI` function request sent when the service is initialized. This value provides a recommended degree of parallelism for the environment when sending requests in parallel. Learn more in [Optimum degree of parallelism (DOP)](https://learn.microsoft.com/power-apps/developer/data-platform/send-parallel-requests?tabs=sdk#optimum-degree-of-parallelism-dop).

##### UserId property

This property returns the value of the `WhoAmIResponse.UserId` from a `WhoAmI` function request sent when the service is initialized.

##### OrganizationId property

This property returns the value of the `WhoAmIResponse.OrganizationId` from a `WhoAmI` function request sent when the service is initialized.

##### BusinessUnitId property

This property returns the value of the `WhoAmIResponse.BusinessUnitId` from a `WhoAmI` function request sent when the service is initialized.

### Config

The [Config class](Config.cs) contains properties that control the behavior of the application.

| Property | Type | Description |
|----------|------|-------------|
| `GetAccessToken` | `Func<Task<string>>` | A function provided by the client application to return an access token. |
| `Url` | `string?` | The base URL of the environment, for example `https://org.api.crm.dynamics.com`.|
| `CallerObjectId` | `Guid` | The [SystemUser.ActiveDirectoryGuid](https://learn.microsoft.com/power-apps/developer/data-platform/reference/entities/systemuser#BKMK_ActiveDirectoryGuid) value to apply for impersonation. Default is [Guid.Empty](https://learn.microsoft.com/dotnet/api/system.guid.empty?view=net-6.0).<br /> Learn more in [Impersonate another user using the Web API](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/impersonate-another-user-web-api?view=dataverse-latest). |
| `TimeoutInSeconds` | `ushort` | How long to wait for a timeout. Default is 120 seconds. |
| `MaxRetries` | `byte` | Maximum number of times to re-try when service protection limits occur. Default is 3. |
| `Version` | `string` | The version of the service to use. Default is `v9.2`. |
| `DisableCookies` | `bool` | Determines if cookies are disabled to gain performance in bulk data load scenarios. Learn more in [Server affinity](https://learn.microsoft.com/power-apps/developer/data-platform/send-parallel-requests?tabs=sdk#server-affinity). |

In the samples that use WebAPIService, the data for these properties is set in the [appsettings.json file](../appsettings.json). The `App` reads the data from `appsettings.json` and uses it to set the properties of a `Config` instance. This data is passed to the `Service` class constructor in the `Program.Main` method.

### EntityReference

The [EntityReference class](EntityReference.cs) represents a reference to a record in a Dataverse table. In OData, resources are identified by a URL. `EntityReference` provides methods to make it easier to create and access properties of Urls.

#### EntityReference Constructors

Use the following constructors to instantiate an `EntityReference`.

##### EntityReference(string entitySetName, Guid? id)

Creates an entity reference using the `EntitySetName` and a `Guid`.

##### EntityReference(string uri)

Parses an absolute or relative url to create an entity reference, including URLs that use alternate keys.

##### EntityReference(string setName, Dictionary&lt;string, string&gt;? keyAttributes)

Use this constructor to instantiate an entity reference using an alternate key.

> [!NOTE]
> The key values must be string values. This does not convert other types to appropriate strings.

#### EntityReference Properties

`EntityReference` has the following public properties.

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid?` | The primary key value of the record when not using an alternate key. |
| `KeyAttributes` | `Dictionary<string, string>` | The string values that represent alternate key values used in a URL. |
| `SetName` | `string` | The `EntitySetName` of the entity type. |
| `Path` | `string` | A relative url to the record. |

#### EntityReference methods

`EntityReference` has the following public methods that don't require any parameters.

| Method Name | Return Type | Description |
|-------------|-------------|-------------|
| `AsODataId` | `string` | Returns a string formatted for use as a parameter reference to a record in the URL for an OData Function. |
| `AsJObject` | `JObject` | Returns a JObject that can be used as a parameter reference to a record in an OData Action. |

### Error Classes

`ODataError`, `Error`, and `ODataException` are classes used to deserialize errors returned by the service.

#### ServiceException

[ServiceException](ServiceException.cs) is an [Exception class](https://learn.microsoft.com/dotnet/api/system.exception?view=net-6.0) that contains properties of the error returned by the service. Use the [ParseError](#parseerror-method) to get an instance of this exception.

## Extensions

WebAPIService has one extension method from a .NET type.

### HttpResponseMessage As&lt;T&gt;

This extension instantiates an instance of `T` derived from [HttpResponseMessage](https://learn.microsoft.com/dotnet/api/system.net.http.httpresponsemessage?view=net-6.0) and copies the properties of the `HttpResponseMessage` to the derived class. `HttpResponseMessage As<T>` is used by the `Service` [SendAsync&lt;T&gt; method](#sendasync-method) but can also be used separately.

For example, when using the `BatchRequest` class, the items in `BatchResponse.HttpResponseMessages` are `HttpResponseMessage` types. You can use this extension to convert them to the appropriate derived class to facilitate accessing any properties.

## Messages

The `Messages` folder includes classes that inherit from [HttpRequestMessage](https://learn.microsoft.com/dotnet/api/system.net.http.httprequestmessage?view=net-6.0) or [HttpResponseMessage](https://learn.microsoft.com/dotnet/api/system.net.http.httpresponsemessage?view=net-6.0).

These classes provide reusable definitions of requests and responses that correspond to OData operations you can use in any Dataverse environment.

These classes also provide examples of specific operations that can be applied using `HttpRequestMessage` and `HttpResponseMessage` without deriving from those types.

Within an application, you might create custom messages, for example representing a Custom API in your environment, using the same pattern. These modular classes and aren't required to be included in the `WebAPIService.Messages` folder.

You can see an example in the [FunctionsAndActions](../FunctionsAndActions) sample:

- [Messages/IsSystemAdminRequest.cs](../FunctionsAndActions/Messages/IsSystemAdminRequest.cs)
- [Messages/IsSystemAdminResponse.cs](../FunctionsAndActions/Messages/IsSystemAdminResponse.cs)

### *Request classes

These classes have a constructor with parameters that instantiate a [HttpRequestMessage](https://learn.microsoft.com/dotnet/api/system.net.http.httprequestmessage?view=net-6.0) with the data needed to perform the operation. The classes might have separate properties as appropriate.

A simple example of this pattern is the [WhoAmIRequest class](Messages/WhoAmIRequest.cs).

```csharp
namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to perform the WhoAmI function
    /// </summary>
    public sealed class WhoAmIRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes the WhoAmIRequest
        /// </summary>
        public WhoAmIRequest()
        {
            Method = HttpMethod.Get;
            RequestUri = new Uri(
                uriString: "WhoAmI", 
                uriKind: UriKind.Relative);
        }
    }
}
```

The names of these classes might align with the classes in the Dataverse SDK [Microsoft.Xrm.Sdk.Messages Namespace](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages) but aren't limited to those operations. Web API provides for performing some operations that can't be done with the SDK, for example [CreateRetrieveRequest](Messages/CreateRetrieveRequest.cs) is a message that creates a record and retrieves it. The Dataverse SDK doesn't provide this capability in a single request.

### *Response classes

When \*Request classes return a value, there's a corresponding \*Response class to access the returned properties. If the \*Request returns `204 No Content`, the operation returns an [HttpResponseMessage](https://learn.microsoft.com/dotnet/api/system.net.http.httpresponsemessage?view=net-6.0), but there's no derived class. Use the [SendAsync method](#sendasync-method) to send these requests.

\*Response classes provide typed properties that access the `HttpResponseMessage` `Headers` or `Content` properties and parse them to provide access to the `ComplexType` returned by the operation.

For example, in the [WhoAmIResponse class](Messages/WhoAmIResponse.cs), you can find all the code needed to extract the properties of the [WhoAmIResponse ComplexType](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/whoamiresponse?view=dataverse-latest).

```csharp
using Newtonsoft.Json.Linq;

namespace PowerApps.Samples.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the response from the WhoAmIRequest
    /// </summary>
    public sealed class WhoAmIResponse : HttpResponseMessage
    {

        // Cache the async content
        private string? _content;

        //Provides JObject for property getters
        private JObject _jObject
        {
            get
            {
                _content ??= Content.ReadAsStringAsync().GetAwaiter().GetResult();

                return JObject.Parse(_content);
            }
        }

        /// <summary>
        /// Gets the ID of the business to which the logged on user belongs.
        /// </summary>
        public Guid BusinessUnitId => (Guid)_jObject.GetValue(nameof(BusinessUnitId));

        /// <summary>
        /// Gets ID of the user who is logged on.
        /// </summary>
        public Guid UserId => (Guid)_jObject.GetValue(nameof(UserId));

        /// <summary>
        /// Gets ID of the organization that the user belongs to.
        /// </summary>
        public Guid OrganizationId => (Guid)_jObject.GetValue(nameof(OrganizationId));
    }
}

```

These classes can only be properly instantiated when returned by the [SendAsync&lt;T&gt; method](#sendasynclttgt-method) or by using the [HttpResponseMessage As&lt;T&gt;](#httpresponsemessage-aslttgt) extension on an `HttpResponseMessage` that was returned by a `BatchResponse.HttpResponseMessages` property.

## Batch

The [Batch folder](Batch) contains three classes to manage sending OData $`batch` requests. More information: [Execute batch operations using the Web API](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/execute-batch-operations-using-web-api).

### BatchRequest

The [BatchRequest](Batch/BatchRequest.cs) constructor initializes an `HttpRequestMessage` that can be used with [SendAsync&lt;T&gt; Method](#sendasync-method) to send requests in batches. The constructor requires the `Service.BaseAddress` value to be passed as a parameter.

`BatchRequest` has the following properties.

| Property | Type | Description |
|----------|------|-------------|
| `ContinueOnError` | `Bool` | Controls whether the batch operation should continue when an error occurs. |
| `ChangeSets` | `List<ChangeSet>` | One or more change sets to be included in the batch. |
| `Requests` | `List<HttpRequestMessage>` | One or more `HttpMessageRequest` to be sent outside of any `ChangeSet`. |

When `ChangeSets` or `Requests` are set they are encapsulated into [HttpMessageContent](https://learn.microsoft.com/previous-versions/aspnet/hh834416(v=vs.118)) and added to the `Content` of the request. The private `ToMessageContent` method applies the required changes to headers and returns the `HttpMessageContent` for both `ChangeSets` and `Requests` properties.

### ChangeSet

A change set represents a group of requests that must complete within a transaction.

`ChangeSet` has a single property:

| Property | Type | Description |
|----------|------|-------------|
| `Requests` | `List<HttpRequestMessage>` | One or more `HttpMessageRequest` to be performed within the transaction. |

### BatchResponse

`BatchResponse` has a single property:

| Property | Type | Description |
|----------|------|-------------|
| `HttpResponseMessages` | `List<HttpResponseMessage>` | The responses from the $batch operation. |

`BatchResponse` has a private `ParseMultipartContent` method used by the `HttpResponseMessages` property getter to parse the `MultipartContent` returned into individual `HttpResponseMessage`.

To access type properties of the `HttpResponseMessage` instances returned, you can use the [HttpResponseMessage As&lt;T&gt; extension method](#httpresponsemessage-ast).

## Methods

For operations that are frequently performed, the [Methods folder](Methods) contains extensions of the `Service` class. These allow for using the corresponding *Request classes in a single line.

The following methods are included:

| Method | Return Type | Description |
|--------|-------------|-------------|
| `Create` | `Task<EntityReference>` | Creates a new record. |
| `CreateRetrieve` | `Task<JObject>` | Creates a new record and retrieves it. |
| `Delete` | `Task` | Deletes a record. |
| `FetchXml` | `Task<FetchXmlResponse>` | Retrieves the results of a FetchXml query. Request is sent with `POST` using `$batch` to mitigate issues where long URLs sent with `GET` can exceed limits. |
| `GetColumnValue<T>` | `Task<T>` | Retrieves a single column value from a table row. |
| `Retrieve` | `Task<JObject>` | Retrieves a record. |
| `RetrieveMultiple` | `Task<RetrieveMultipleResponse>` | Retrieves multiple records. |
| `SetColumnValue<T>` | `Task` | Sets the value of a column for a table row. |
| `Update` | `Task | Updates a record. |
| `Upsert` | `Task<UpsertResponse>` | Performs an Upsert on a record. |

Within a sample application using WebAPIService, when the operation doesn't represent an API found in Dataverse by default, the method is defined in the application rather than in the WebAPIService. The [FunctionsAndActions](../FunctionsAndActions) sample has a custom method included: [/Methods/IsSystemAdmin.cs](../FunctionsAndActions/Methods/IsSystemAdmin.cs).

## Types

The [Types folder](Types) contains any classes or enums that correspond to [ComplexTypes](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/complextypes?view=dataverse-latest) or [EnumTypes](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/enumtypes?view=dataverse-latest) needed as parameters or response properties for messages.

## Metadata

The [Metadata folder](Metadata) contains [Messages](Metadata/Messages) and [Types](Metadata/Types) specific to operations that work with Dataverse schema definitions. These metadata are  classes with many properties that return complex types.

These metadata are used by the [MetadataOperations sample](../MetadataOperations).
