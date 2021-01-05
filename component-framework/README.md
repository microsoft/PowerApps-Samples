# How to run the sample components?

Follow the steps below to import and try the sample components in your model-driven or canvas app:

1. Navigate to the folder on your computer where you have downloaded the sample components, and extract the .zip file.  
1. Open Developer Command Prompt for Visual Studio 2017 and navigate to the sample component folder in the extracted folder that you want to see it in runtime. For example, navigate to the \<extracted_folder>/TS_IncrementComponent folder.
1. Run the following command to get all the required dependencies:
    ```
    npm install
    ```
1. Create a new folder using the command `mkdir <folder name>` inside the sample component folder and navigate into the folder using the command `cd <folder name>`. 
1. Create a new solution project inside the folder using the following command:
    ```
    pac solution init --publisher-name <Name of the publisher> --publisher-prefix <Publisher prefix>
    ```
1. After the new solution project is created, refer to the location where the sample component is located. You can add the reference using the following command:
    ```
    pac solution add-reference --path <Path to the root of the sample component>
    ```
1. To generate a zip file from your solution project, you need to `cd` into your solution project directory and build the project using the following command:

    ```
    msbuild /t:restore
    Generate release
    msbuild /t:rebuild /restore /p:Configuration=Release
    ```
1. Again, run the command `msbuild`.
1. The generated solution zip file will be available at `Solution\bin\debug` folder. Manually [import the solution](/powerapps/maker/common-data-service/import-update-export-solutions) into your Microsoft Dataverse environment using the web portal once the zip file is ready. Alternatively, to import the solution using Power Apps CLI commands, see the [Connecting to your environment](https://docs.microsoft.com/powerapps/developer/component-framework/import-custom-controls#connecting-to-your-environment) and [Deployment](https://docs.microsoft.com/powerapps/developer/component-framework/import-custom-controls#deploying-code-components) sections.
1. Finally, to add code components to your model-driven and canvas apps, see [Add components to model-driven apps](https://docs.microsoft.com/powerapps/developer/component-framework/add-custom-controls-to-a-field-or-entity) and [Add components to canvas apps](https://docs.microsoft.com/powerapps/developer/component-framework/component-framework-for-canvas-apps#add-components-to-a-canvas-app).
