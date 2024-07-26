# How to run Microsoft Dataverse samples?

1. Download or clone the repo to get a local copy.
1. (Optional) Edit the `dataverse/App.config` file to define a connection string specifying the Dataverse instance you want to connect to.
1. Open the sample solution in Visual Studio and press `F5` to run the sample.
    - If you specified a connection string in `dataverse/App.config`, any sample you run uses that connection information.
    - If you didn't specify a connection string in the `dataverse/App.config` file, a dialog [XRM tooling common login control](https://learn.microsoft.com/powerapps/developer/common-data-service/xrm-tooling/use-xrm-tooling-common-login-control-client-applications) opens each time you run the sample. Enter information to choose a  Dataverse instance with which to connect and the credentials you want to use. This dialog caches previous connections to make them available for future use.

The samples in this repo require a connection to a Dataverse instance to run and include a linked reference to the `dataverse/App.config` file.
