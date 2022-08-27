# WebAPIService class library (C#)

WebAPIService is a sample .NET 6.0 class library project that demonstrates several important capabilities that you should include when you use the Dataverse Web API.

This library demonstrates:

- Managing Dataverse service protection limits with the .NET resilience and transient fault handling library [Polly](https://github.com/App-vNext/Polly).
- Managing an [HttpClient](https://docs.microsoft.com/dotnet/api/system.net.http.httpclient?view=net-6.0&viewFallbackFrom=dotnet-plat-ext-6.0) in .NET using [IHttpClientFactory](https://docs.microsoft.com/dotnet/api/system.net.http.ihttpclientfactory?view=dotnet-plat-ext-6.0).
- Using configuration data to manage the behavior of the client.
- Managing errors returned by Dataverse Web API.
- A pattern of code re-use by:
   - Creating classes that inherit from [HttpRequestMessage](https://docs.microsoft.com/dotnet/api/system.net.http.httprequestmessage?view=net-6.0) and [HttpResponseMessage](https://docs.microsoft.com/dotnet/api/system.net.http.httpresponsemessage?view=net-6.0).
   - Methods that leverage those classes.
   - A modular pattern for adding new capabilities as needed.

**Note**: This sample library is a helper that is used by all the Dataverse C# Web API samples, but it is not an SDK. It is tested only to confirm that the samples that use it run successfully. This sample code is provided 'as-is' with no warranty for reuse.

This library does not:

- Manage authentication. It depends on a function passed from an application that will provide the access token to use. All samples depend on a shared [App class](../App.cs) that manages authentication using the [Microsoft Authentication Library (MSAL)](https://docs.microsoft.com/azure/active-directory/develop/msal-overview).  MSAL supports several different types of Authentication flows. These samples use the [Username/password (ROPC)](https://docs.microsoft.com/azure/active-directory/develop/msal-authentication-flows#usernamepassword-ropc) flow for simplicity but this is not recommended. For your apps, you should use one of the other flows. More information: [Authentication flow support in the Microsoft Authentication Library](https://docs.microsoft.com/azure/active-directory/develop/msal-authentication-flows).
- Provide for any code generation capabilities. Any classes used in the samples are written by hand. All business entity data uses the well-known [Json.NET JObject Class](https://www.newtonsoft.com/json/help/html/t_newtonsoft_json_linq_jobject.htm) rather than a class representing the entity type.
- Provide an object model for composing OData queries. All queries are represented as strings.

## Class listing

The following are classes included in the WebAPIService.

### Service

The [Service class](Service.cs) constructor accepts a [Config class](#config) instance which contains two required properties: `GetAccessToken` and `Url`. All the other properties represent options that have defaults.

The constructor uses dependency injection to create an [IHttpClientFactory](https://docs.microsoft.com/dotnet/api/system.net.http.ihttpclientfactory?view=dotnet-plat-ext-6.0) that can return a named [HttpClient](https://docs.microsoft.com/dotnet/api/system.net.http.httpclient?view=net-6.0&viewFallbackFrom=dotnet-plat-ext-6.0) with the properties specified in the `ConfigureHttpClient` function. Whether or not this client will use cookies is based on whether the `Config.DisableCookies` parameter is set. In the constructor the policy defined by the static `GetRetryPolicy` method that controls how transient errors and Dataverse service protection limits will be managed.

#### Service Methods

The Service class has the following methods:

##### SendAsync(HttpRequestMessage) Method

This is the single method ultimately responsible for all operations.

This method:

- Returns `Task<HttpResponseMessage>`
- Exposes the same signature as the [HttpClient.SendAsync(HttpRequestMessage)](https://docs.microsoft.com/dotnet/api/system.net.http.httpclient.sendasync?view=net-6.0#system-net-http-httpclient-sendasync(system-net-http-httprequestmessage)) and can be used in the same way.
- Calls the function set in the `Config.GetAccessToken` method to set the `Authorization` header value for the request.
- Uses the [IHttpClientFactory.CreateClient Method](https://docs.microsoft.com/dotnet/api/system.net.http.ihttpclientfactory.createclient?view=dotnet-plat-ext-6.0) to get the named [HttpClient](https://docs.microsoft.com/dotnet/api/system.net.http.httpclient?view=net-6.0&viewFallbackFrom=dotnet-plat-ext-6.0) to send the request.
- Will throw a [ServiceException](#serviceexception) if the [HttpResponseMessage.IsSuccessStatusCode Property](https://docs.microsoft.com/dotnet/api/system.net.http.httpresponsemessage.issuccessstatuscode?view=net-6.0) is false, so you don't need to check this this when using this method.

##### SendAsync&lt;T&gt;(HttpRequestMessage) Method

This method facilitates returning a class that includes properties found in the Complex Types returned by OData actions and functions.

- Returns `Task<T>` where `T` is a class derived from [HttpResponseMessage](https://docs.microsoft.com/dotnet/api/system.net.http.httpresponsemessage?view=net-6.0). See [*Response classes](#response-classes) for more information.
- Calls the [SendAsync(HttpRequestMessage) Method](#sendasynchttprequestmessage-method).
- Uses the [HttpResponseMessage.As&lt;T&gt;](#httpresponsemessage-ast) extension method to return the requested type.

When using this method it is expected, but not required, that the `request` parameter is one of the [*Response classes](#response-classes)  that derive from [HttpRequestMessage](https://docs.microsoft.com/dotnet/api/system.net.http.httprequestmessage?view=net-6.0)

The following examples shows use with the [WhoAmI function](https://docs.microsoft.com/power-apps/developer/data-platform/webapi/reference/whoami):

```csharp
static async Task WhoAmI(Service service)
{
   var response = await service.SendAsync<WhoAmIResponse>(new WhoAmIRequest());

   Console.WriteLine($"Your user ID is {response.UserId}");
}
```

##### ParseError(HttpResponseMessage) Method

This method will parse the content of an `HttpResponseMessage` to return an `ServiceException`. It is used within [SendAsync(HttpRequestMessage)](#sendasynchttprequestmessage-method) when the [HttpResponseMessage.IsSuccessStatusCode Property](https://docs.microsoft.com/dotnet/api/system.net.http.httpresponsemessage.issuccessstatuscode?view=net-6.0) is false. You can also use it to extract error information from `HttpResponseMessage` instances returned by `BatchResponse.HttpResponseMessages` when the `BatchRequest.ContinueOnError` property is set to true. More information: [Batch](#batch)

#### Service Properties

Service has a single property: `BaseAddress`.

##### BaseAddress Property

This property returns the base URL set in the `Config.Url`. This is needed when instantiating the `BatchRequest` class and to append to a relative URL anytime an absolute URL is required.

### Config

The [Config class](Config.cs) contains properties that control the behavior of the application as shown in the table below:


|Property|Type|Description|
|---------|---------|---------|
|`GetAccessToken`|`Func<Task<string>>?`|A function provided by the client application to  return access token.|
|`Url`|`string?`|The Url of the environment. i.e.: `https://org.api.crm.dynamics.com`|
|`CallerObjectId`|`Guid`|The systemuserid value to apply for impersonation. Default is [Guid.Empty](https://docs.microsoft.com/dotnet/api/system.guid.empty?view=net-6.0)|
|`TimeoutInSeconds`|`ushort`|How long to wait for a timeout. Default is 120 seconds.|
|`MaxRetries`|`byte`|Maximum number of times to re-try when service protection limits occur. Default is 3.|
|`Version`     |`string`|The version of the service to use. Default is `v9.2`|
|`DisableCookies`|`bool`|Whether to disable cookies to gain performance in bulk data load scenarios.|

In the samples that use WebAPIService, the data for these properties is set in the [appsettings.json file](../appsettings.json). The `App` will read the data from appsettings.json file and use it to set the properties of a `Config` instance. This data is passed to the `Service` class constructor in the `Program.Main` method.

### EntityReference

An the [EntityReference class](EntityReference.cs) represents a reference to a record in a Dataverse table. In OData resources are identified by a URL. `EntityReference` provides methods to expose specific properties to make managing URLs easier.

#### EntityReference Constructors

Use the following constructors to instantiate an `EntityReference`.

##### EntityReference(string entitySetName, Guid? id)

Creates an entity reference using the EntitySetName and a Guid.

##### EntityReference(string uri)

Parses an absolute or relative url to create an entity reference.

##### EntityReference(string setName, Dictionary&lt;string, string&gt;? keyAttributes)

Use this constructor to instantiate an entity reference using an alternate key.

**Note**: The key values must be string values. This does not convert other types to appropriate strings.

#### EntityReference Properties

EntityReference has the following public properties:

|Property  |Type   |Description  |
|---------|---------|---------|
|`Id`|`Guid?`|The primary key value of the record when not using an alternate key.|
|`KeyAttributes`|`Dictionary<string, string>?`|The string values that represent alternate key values used in a url.|
|`SetName`|`string`|The `EntitySetName` of the entity type.|
|`Path`|`string`|A relative url to the record.|

#### EntityReference methods

`EntityReference` has the following public methods. Neither of them require any parameters.

|Method Name|Return Type|Description|
|---------|---------|---------|
|`AsODataId`|`string`|Returns a string formatted for use as a parameter reference to a record in the URL for an OData Function.|
|`AsJObject`|`JObject`|Returns a JObject that can be used as a parameter reference to a record in an OData Action.|

### Error Classes

`ODataError`, `Error`, and `ODataException` are classes used to deserialize errors returned by the service.

#### ServiceException

[ServiceException](ServiceException.cs) is an [Exception class](https://docs.microsoft.com/dotnet/api/system.exception?view=net-6.0) that contains properties of the error returned by the service.

## Extensions

WebAPIService has one extension method from a .NET type.

### HttpResponseMessage As&lt;T&gt;

This extension instantiates an instance of `T` where `T` is derived from [HttpResponseMessage](https://docs.microsoft.com/dotnet/api/system.net.http.httpresponsemessage?view=net-6.0) and copies the properties of the `HttpResponseMessage` to the derived class. It is used by the `Service` [SendAsync&lt;T&gt; method](#sendasynclttgthttprequestmessage-method) but can also be used separately. For example, when using the `BatchRequest` class, the items in the B`atchResponse.HttpResponseMessages` will be `HttpResponseMessage` types. You can use this extension to convert them to the appropriate derived class to facilitate accessing any properties.

## Messages

The `Messages` folder includes classes that inherit from [HttpRequestMessage](https://docs.microsoft.com/dotnet/api/system.net.http.httprequestmessage?view=net-6.0) or [HttpResponseMessage](https://docs.microsoft.com/dotnet/api/system.net.http.httpresponsemessage?view=net-6.0).

These classes provide re-usable definitions of requests and responses that correspond to OData operations you can use in any Dataverse environment.

Within an application, you may also create custom messages, for example representing a Custom API in your environment, using the same pattern. These are modular classes and are not required to be included in the `Messages` folder.

### *Request classes

These classes will generally have a constructor with parameters that will instantiate a [HttpRequestMessage](https://docs.microsoft.com/dotnet/api/system.net.http.httprequestmessage?view=net-6.0) with the data needed to perform the operation. They may have separate properties as appropriate.

The names of these classes may align with the Dataverse SDK Message classes but are not limited to those operations. Web API provides for performing some operations that cannot be done with the SDK, for example [CreateRetrieveRequest](Messages/CreateRetrieveRequest.cs) is message that will create a record and retrieve it. The SDK doesn't provide this capability in a single request.

### *Response classes

When \*Request classes returns a value there will be a corresponding \*Response class to access the returned properties. If the \*Request returns `204 No Content`, the operation will return an [HttpResponseMessage](https://docs.microsoft.com/dotnet/api/system.net.http.httpresponsemessage?view=net-6.0) but there will be no derived class. Use the [SendAsync(HttpRequestMessage) method](#sendasynchttprequestmessage-method) to send these requests.

\*Response classes provide typed properties that access the `HttpResponseMessage` `Headers` or `Content` properties and parse them to provide access to the Complex Type returned by the operation. An example of this is the [WhoAmIResponse class](Messages/WhoAmIResponse.cs). Within this class you can find all the code needed to extract the properties of the [WhoAmIResponse ComplexType](https://docs.microsoft.com/power-apps/developer/data-platform/webapi/reference/whoamiresponse?view=dataverse-latest). 

These classes can only be properly instantiated when returned by the [SendAsync&lt;T&gt; method](#sendasynclttgthttprequestmessage-method) or by using the [HttpResponseMessage As&lt;T&gt;](#httpresponsemessage-aslttgt) extension on an `HttpResponseMessage` that was returned by a `BatchResponse.HttpResponseMessages` property.



## Batch

## Methods

## Types


