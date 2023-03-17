# Sample: Persistent queue listener

This sample shows how to write a Azure Service Bus listener application for a persistent queue endpoint contract.

The listener waits for a message to be posted to the service bus and to be available in the endpoint queue. When a message is available in the queue, the listener reads the message, prints the execution context contained in the message to the console, and deletes the message from the queue.
