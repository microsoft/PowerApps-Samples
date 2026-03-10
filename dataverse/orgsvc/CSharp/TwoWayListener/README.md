# Sample: Two-way listener

This sample shows how to write a `Azure Service Bus` Listener for a two-way endpoint contract.

This sample registers a remote service plug-in that executes whenever a message is posted to a two-way endpoint on the `Azure Service Bus`. When the plug-in executes, it prints to the console the contents of the execution context contained in the message.
