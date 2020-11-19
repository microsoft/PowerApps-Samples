# Web API CDSWebApiService class

This is a .NET Framework Class library project that creates an assembly to define a service when using the CDS Web API.

This assembly helps to:

- Make your code 'DRY'er by wrapping common operations by Http methods.
- Demonstrates how to manage an HttpClient
- Demonstrates how to manage Service Protection Limit API 429 errors that a client application should expect

## Properties

This class exposes only the **BaseAddress** property. This is the configured base address used by the HttpClient. It can be useful to build complete URIs when needed since most cases will expect relative URIs.

## Methods

This class provides the following public methods:

### PostCreate

Creates an entity synchronously and returns the URI

#### Parameters

|Name  |Type  |Description  |
|---------|---------|---------|
|entitySetName|string|The entity set name of the type of entity to create. |
|body|JObject|Contains the data for the entity to create|

#### Return Value

The Uri of the created entity

#### Remarks

This method is provided because creating entities is a common operation and the URI is returned in the `OData-EntityId` header. Having this specialized method allows for less code than having only the Post method, which returns only a JObject.


### PostCreateAsync

The asynchronous version of PostCreate.

### Post

Sends a POST request synchronously and returns the response as a JObject.

#### Parameters

|Name  |Type  |Description  |
|---------|---------|---------|
|path|string|The relative path to send the request. Frequently the name of an action|
|body|JObject|The payload to POST|
|headers|Dictionary<string, List<string>>|(Optional) Any headers needed to apply special behaviors|

#### Return Value

A JObject containing the response.

#### Remarks

This method can be used for any operation using the POST http method, but it only includes the response content. Use PostCreate to create entities and return only the URI of the created entity.


### PostAsync

The asynchronous version of Post

### Patch

Sends a PATCH request synchronously.

#### Parameters

|Name  |Type  |Description  |
|---------|---------|---------|
|uri|Uri|The relative path to send the request. Frequently the Uri for a specific entity|
|body|JObject|The payload to send|
|headers|Dictionary<string, List<string>>|(Optional) Any headers needed to apply special behaviors|

#### Remarks

Patch is frequently used to Update or Upsert records.

### PatchAsync

The asynchronous version of Patch.

### Get

Sends a GET request synchronously and returns data

#### Parameters

|Name  |Type  |Description  |
|---------|---------|---------|
|path|string|The relative path of the resource to return |
|headers|Dictionary<string, List<string>>|(Optional) Any headers needed to apply special behaviors|

#### Return Value

A JToken representing the requested data.

### GetAsync

The asynchronous version of Get.

### Delete

Sends a DELETE request synchronously.

#### Parameters

|Name  |Type  |Description  |
|---------|---------|---------|
|uri|Uri|The relative path of the resource to delete |
|headers|Dictionary<string, List<string>>|(Optional) Any headers needed to apply special behaviors|

### DeleteAsync

The asynchronous version of Delete.

### Put

Sends a PUT request synchronously.

#### Parameters

|Name  |Type  |Description  |
|---------|---------|---------|
|uri|Uri|The relative path to the resource to update.|
|property|string|The name of the property to update|
|value|string|The value to set|

#### Remarks

Put is used to update specific entity properties.

**Note**: The Http PUT method is also used to update Metadata. This method cannot be used for that purpose. It is specifically for business data.


### PutAsync

The asynchronous version of Put.

## Private SendAsync method

All of the methods above route their requests through the private SendAsync method. This is where the low level common logic occurs.

This method contains the logic to manage any Service Protection API 429 errors and re-try them for a number of times configurable in the service.

In order to do this, it sends a copy of the request rather than the actual request because the request will be disposed and cannot be sent again if an error is returned.

The copy of the request is available because of the custom HttpRequestMessage Clone method defined in the Extensions.cs file.

## OAuthMessageHandler

When the internal HttpClient is initialized in the CDSWebApiService constructor, an instance of this class is set as an HttpMessageHandler. This class works with the ADAL libraries to ensure that the accessToken will be refreshed each time a request is sent. If the accessToken expires, the ADAL library methods will automatically refresh it.

## ServiceConfig

The CDSWebApiService class should be initialized with a connection string via the ServiceConfig class.

The ServiceConfig constructor accepts a connection string, typically from the App.config configuration, and the data defined there is parsed into a ServiceConfig instance which the CDSWebApiService constructor requires.

### Properties

The following are the properties of the ServiceConfig class.

|Name|Type|Description|
|---------|---------|---------|
|Authority|string|The authority to use to authorize user. Default is 'https://login.microsoftonline.com/common'|
|CallerObjectId|Guid|The Azure AD ObjectId for the user to impersonate other users.|
|ClientId|string|The id of the application registered with Azure AD|
|MaxRetries|byte|The maximum number of attempts to retry a request blocked by service protection limits. Default is 3.|
|Password|SecureString|The password for the user principal|
|RedirectUrl|string|The Redirect Url of the application registered with Azure AD|
|TimeoutInSeconds|ushort|The amount of time to try completing a request before it will be cancelled. Default is 120 (2 minutes)|
|Url|string|The Url to the CDS environment, i.e "https://yourorg.api.crm.dynamics.com"|
|UserPrincipalName|string|The user principal name of the user. i.e. you@yourorg.onmicrosoft.com|
|Version|string|The version of the Web API to use. Default is '9.1'|

### Example connection string

Each of the samples that use CDSWebApiService include a reference to a common App.config and code to read a connection string value named 'Connect'. The following is an example of that App.config:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <startup>
    <supportedRuntime version="v4.0"
                      sku=".NETFramework,Version=v4.7.2" />
  </startup>
  <connectionStrings>
    <add name="Connect"
         connectionString="Url=https://yourorg.api.crm.dynamics.com;
         Authority=null;
         ClientId=51f81489-12ee-4a9e-aaae-a2591f45987d;
         RedirectUrl=app://58145B91-0C36-4500-8554-080854F2AC97;
         UserPrincipalName=you@yourorg.onmicrosoft.com;
         Password=y0urp455w0rd;
         CallerObjectId=null;
         Version=9.1;
         MaxRetries=3;
         TimeoutInSeconds=180;
         "/>
  </connectionStrings>
</configuration>
```

The **ClientId** and **RedirectUrl** values are for sample applications. You can use these to run the samples, but you should register your own applications and enter the corresponding values for these properties.

## ServiceException

This class simply extends Exception and provides additional properties from an error response.

### Properties


|Name  |Type  |Description  |
|---------|---------|---------|
|Message|string|The message returned by the platform|
|ErrorCode|int|The error code returned by the platform|
|StatusCode|int|The HttpResponseMessage.StatusCode|
|ReasonPhrase|string|The HttpResponseMessage.ReasonPhrase|
