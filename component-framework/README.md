# How to run the sample components?

Follow the steps below to import and try the sample components in your model-driven or canvas app:

1. [Download](https://docs.github.com/repositories/working-with-files/using-files/downloading-source-code-archives#downloading-source-code-archives-from-the-repository-view) or [clone](https://docs.github.com/repositories/creating-and-managing-repositories/cloning-a-repository) this repository [github.com/microsoft/PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples).

   If you download the repository:
      
      1. Right click the `PowerApps-Samples-master.zip` file and select **Properties**.
      1. On the **General** tab, in the **Security** area, check the **Unblock** checkbox.
      1. Extract the contents of the `PowerApps-Samples-master.zip` file.

1. Open a [Developer Command Prompt for Visual Studio](https://learn.microsoft.com/visualstudio/ide/reference/command-prompt-powershell) and navigate to the `component-framework` folder.

   On Windows, you can type `developer command prompt` in **Start** to open a developer command prompt.

1. Navigate to the component that you want to try, for example `IncrementControl` and run the `npm install` command to get all the required dependencies.

   For example:
   `[Download or Clone location]\component-framework\IncrementControl>npm install`

1. After the command has completed, run `msbuild /t:restore`.

   For example:
   
   `[Download or Clone location]\component-framework\IncrementControl>msbuild /t:restore`.

1. After the command has completed, create a new folder inside the sample component folder  using the `mkdir` command.

   For example: `[Download or Clone location]\component-framework\IncrementControl>mkdir IncrementControlSolution`

1. Navigate to the folder using the command `cd <folder name>`.

   For example: `[Download or Clone location]\component-framework\IncrementControl>cd IncrementControlSolution`

1. Inside the folder you created, run the [pac solution init](https://learn.microsoft.com/power-platform/developer/cli/reference/solution#pac-solution-init) command, providing the name and customization prefix of the publisher.

   For example: 
  
   `\IncrementControlSolution>pac solution init --publisher-name powerapps_samples --publisher-prefix sample`

   > **Note**: This will create a new file named `IncrementControlSolution.cdsproj` in the folder.

1. Run the [pac solution add-reference](https://learn.microsoft.com/power-platform/developer/cli/reference/solution#pac-solution-add-reference) command with the `path` set to the location of the `IncrementControl.pcfproj` file.

   For example, in this case you can use a relative path like either of the following:

   `\IncrementControlSolution>pac solution add-reference --path ../../IncrementControl`

   `\IncrementControlSolution>pac solution add-reference --path ../../IncrementControl/IncrementControl.pcfproj`

   > **Note**: The important thing is that you are referencing the folder that contains the `.pcfproj` file for the control you want to add.
   > 
   > After you run this command, within the `IncrementControlSolution.cdsproj` you will find this reference.
   > 
   > ```xml
   > <ItemGroup>
   >  <ProjectReference Include="..\IncrementControl.pcfproj" />
   > </ItemGroup>
   > ```

1. To generate a zip file from your solution project, run the following three commands in the location of the solution folder you created:

   `\IncrementControlSolution>msbuild /t:restore`

   `\IncrementControlSolution>msbuild /t:rebuild /restore /p:Configuration=Release`

   `\IncrementControlSolution>msbuild`


   The generated solution zip file will be available at `IncrementControlSolution\bin\debug` folder.

1. Now that you have the zip file you need to import it. You have two options:

   - Manually [import the solution](https://docs.microsoft.com/powerapps/maker/data-platform/import-update-export-solutions) into your environment using [make.powerapps.com](https://make.powerapps.com/).
   - Alternatively, to import the solution using Power Apps CLI commands, see the [Connecting to your environment](https://docs.microsoft.com/powerapps/developer/component-framework/import-custom-controls#connecting-to-your-environment) and [Deployment](https://docs.microsoft.com/powerapps/developer/component-framework/import-custom-controls#deploying-code-components) sections.

1. Finally, to add code components to your model-driven and canvas apps, see [Add components to model-driven apps](https://docs.microsoft.com/powerapps/developer/component-framework/add-custom-controls-to-a-field-or-entity) and [Add components to canvas apps](https://docs.microsoft.com/powerapps/developer/component-framework/component-framework-for-canvas-apps#add-components-to-a-canvas-app).
