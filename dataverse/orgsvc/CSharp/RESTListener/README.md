# Sample: REST listener

This sample shows how to write a `Azure Service Bus` Listener for a `REST` endpoint contract.

This sample registers a remote service plug-in that executes whenever a message is posted to a `REST` endpoint on the service bus. When the plug-in executes, it prints to the console the contents of the execution context contained in the message.

