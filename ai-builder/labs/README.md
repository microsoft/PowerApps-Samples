# AI Builder labs

This topic provides information about the sample data files available in the **ai-builder/labs** folder.

AI Builder documentation is available in the [AI Builder documentation](https://learn.microsoft.com/ai-builder).

## AIBuilder_Lab.zip

Select **AIBuilder_Lab.zip** in the file list at the top of this page, and then select the **Download** button to download all the folders and files for the hands-on labs. The AI Builder labs walk through object detection, prediction, text classification, document processing, and business card reader scenarios in AI Builder. It contains the following folders and zip files:

- Lab Data
- Lab Images
- Lab Scripts
- AIBuilderLabSolution_1_0_0_1.zip
- ProcessFeedback_Flow.zip

## AI Builder lab instructions

Follow the steps below to set up the environment for use with AI Builder labs.

For binary classification, text classification, entity extraction, and object detection scenarios, you'll need some sample data in Microsoft Dataverse.

For prediction, text classification, and object detection scenarios, you'll need some sample data in Microsoft Dataverse.


### Manual data set up

#### Step 1: Import AIBuilderLabSolution_1_0_0_1 solution to the Dataverse environment

This will create five (5) Dataverse tables - **Object Detection Product**, **Health Feedback**, **Online Shopping Intent**, **Travel Feedback**, and **Expenses**.

#### Step 2: Upload data to the entities created in step 1

- **Prediction**: Follow the instructions in [Prediction data preparation](https://learn.microsoft.com/ai-builder/binary-classification-data-prep) to upload the **Online Shopping Intent** data.
- **Category classification**: Go to the **Lab Data/CategoryClassification** folder within the lab files, and then upload the data from **pai_healthcare_feedbacks**. Follow the instructions in [Before you build a category classification model](https://learn.microsoft.com/ai-builder/before-you-build-text-classification-model).
- **Entity extraction**: Go to the **Lab Data/EntityExtraction** folder with the lab files, and then upload the data from **aib_travelfeedback**. Follow the instructions in [Use sample data to do entity extraction](https://learn.microsoft.com/ai-builder/entity-extraction-sample-data).

For business card reader, document processing, identity document reader, object detection, receipt processing, and text recognition labs, you'll need images/pdfs that are available in the **Lab Images** folder.
