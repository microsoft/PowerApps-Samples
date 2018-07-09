# How to run samples

1. Download or clone the repo so that you have a local copy.
1. (Optional) Edit the cds/App.config file to define a connection string specifying the Common Data service for Apps instance you want to connect to.
1. Open the sample solution in Visual Studio and press F5 to run the sample.
    - After you specify a connection string in cds/App.config, any sample you run will use that connection information.

If you do not specify a connection string in cds/App.config file, a dialog will open each time you run the sample and you will need to enter information about which CDS for Apps instance you want to connect to and which credentials you want to use. This dialog will cache previous connections so that you can choose a previously used connection.

Those samples in this repo that require a connection to a Common Data Service for Apps instance to run will include a linked reference to the cds/App.config file.

