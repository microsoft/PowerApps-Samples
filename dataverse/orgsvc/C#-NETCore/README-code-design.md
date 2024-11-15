---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "Sample code design"
---

# Sample code design

Our .NET Core based code samples follow a certain design. This table highlights common design elements of a program's class members and how they are used.

|Member name|Purpose|
|--|--|
|Program()|Class constructor that reads the connection string value from the app settings JSON file. The code supports a global app settings file referred to by an environment variable. Otherwise, the app settings file included with the project is used.|
|Main()|Executes the Setup(), Run(), and Cleanup() methods (in that order).|
|Setup()|Pre-creates any entity or other data required by the Run() method. While this code may be interesting, the code is not the primary focus of the sample.|
|Run()|Performs (or invokes) the primary operations being demonstrated by the code sample.|
|Cleanup()|Deletes all created entities and frees any other resources allocated during the execution of the program. Additonal overloaded Cleanup() methods may be added to the program to free additional non-entity related resources.|
|entityStore| Dictionary that stores information about all entities created in Dataverse by Setup() and Run(). Used by Cleanup() to delete any created entities.|

Beyond these standard elements, other class methods may be added for larger samples when adding code to just the Run() or Setup() methods would make the sample difficult to understand.