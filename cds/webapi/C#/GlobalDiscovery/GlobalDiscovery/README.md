# Use the Web API Global Discovery Service

This sample shows how to use the Web API Global discovery Service

## How to run this sample

See [How to run samples](../../../../How-to-run-samples.md) for information about how to run this sample.

## What this sample does

This sample returns the available Common Data Service for Apps instances for a given user credential.


## How this sample works

This sample will use credential information in the App.config file, but will not use the URL configured in the connection string.
Instead, it will just use the user credentials and the clientid.

### Setup

This sample doesn't require any setup.


### Demonstrate

Uses a HttpClient to authenticate using ADAL (v2.29) and call the global discovery service to return information about available instances the user can connect to.

### Clean up

This sample doesn't require any clean up since it doesn't create any records.