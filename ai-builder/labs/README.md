# AI Builder labs

This topic provides information about the sample data files available in the **ai-builder/labs** folder.

AI Builder documentation is available [here](https://docs.microsoft.com/ai-builder).

## AIBuilder_Lab.zip

Click **AIBuilder_Lab.zip** in the file list at the top of this page, and then click the download button to download all the folders and files for the hands-on-labs. The AI Builder labs walk through object detection, binary classification, text classification, form processing & business card reader scenarios in AI Builder. It contains the following folders and zip files:

- Lab Data
- Lab Images
- Lab Scripts
- AIBuilderLabSolution_1_0_0_0.zip
- ProcessFeedback_Flow.zip

## AI Builder lab instructions

Please follow the steps below to set up the environment for use with AIBuilder Labs. 

For binary classification, text classification, and object detection scenarios you will need some sample data in Common Data Service.

### Manual data set up

#### Part 1 : Import AIBuilderLabSolution_1_0_0_0 solution to the CDS environment

This creates 3 Common Data Service entities - Object Detection Product, Health Feedback, and Online Shopping Intent.

#### Part 2 : Upload data to the entities created in Part 1

- Binary classification : Follow the instructions [here](https://docs.microsoft.com/en-us/ai-builder/binary-classification-data-prep) to upload the **Online Shopping Intent** data.
- Text classification : Go to **Lab Data/Text Classification** folder within the lab files and then upload data from **pai_healthcare_feedbacks**. Follow the instructions [here](https://docs.microsoft.com/en-us/ai-builder/before-you-build-text-classification-model).
- Object detection : Go to **Lab Data/ObjectDetection** folder and then upload data from **aib_objectdetectionproducts**. Follow the same instructions as above for data upload.
