# AI Builder labs

This topic provides information about the sample data files available in the **ai-builder/labs** folder.

AI Builder documentation is available [here](https://docs.microsoft.com/ai-builder).

## AIBuilder_Lab.zip

Click **AIBuilder_Lab.zip** in the file list at the top of this page, and then click the download button to download all the folders and files for the hands-on-labs. The AI Builder labs walk through object detection, prediction, text classification, form processing & business card reader scenarios in AI Builder. It contains the following folders and zip files:

- Lab Data
- Lab Images
- Lab Scripts
- AIBuilderLabSolution_1_0_0_1.zip
- ProcessFeedback_Flow.zip

## AI Builder lab instructions

Please follow the steps below to set up the environment for use with AIBuilder Labs.

For binary classification, text classification, entity extraction and object detection scenarios you will need some sample data in Microsoft Dataverse.

For prediction, text classification, and object detection scenarios you will need some sample data in Microsoft Dataverse.


### Manual data set up

#### Step 1 : Import AIBuilderLabSolution_1_0_0_1 solution to the Dataverse environment

This creates 3 Dataverse entities - Object Detection Product, Health Feedback, and Online Shopping Intent.

- Object Detection Product
- Health Feedback
- Online Shopping Intent
- Travel feedback

#### Step 2 : Upload data to the entities created in step 1

- **Prediction** : Follow the instructions in [Prediction data preparation](https://docs.microsoft.com/ai-builder/binary-classification-data-prep) to upload the **Online Shopping Intent** data.
- **Category classification** : Go to **Lab Data/Text Classification** folder within the lab files and then upload data from **pai_healthcare_feedbacks**. Follow the instructions in [Before you build a category classification model](https://docs.microsoft.com/ai-builder/before-you-build-text-classification-model).
- **Entity extraction** : Go to **Lab Data/EntityClassification** folder with the lab files and then upload data from **aib_travelfeedback**.  Follow the instructions in [Use sample data to do entity extraction](https://docs.microsoft.com/ai-builder/entity-extraction-sample-data).
- **Object detection** : Go to **Lab Data/ObjectDetection** folder and then upload data from **aib_objectdetectionproducts**. Follow the same instructions as above for data upload.
