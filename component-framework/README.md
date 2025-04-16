# How to run the sample components?

Use the following steps to import and try the sample components in your model-driven or canvas app.

> [!NOTE]
> See [Example output](#example-output) for the full console output of these steps.

1. [Download](https://docs.github.com/repositories/working-with-files/using-files/downloading-source-code-archives#downloading-source-code-archives-from-the-repository-view) or [clone](https://docs.github.com/repositories/creating-and-managing-repositories/cloning-a-repository) this repository [github.com/microsoft/PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples).

1. Open a [Developer Command Prompt for Visual Studio](https://learn.microsoft.com/visualstudio/ide/reference/command-prompt-powershell) and navigate to the `component-framework` folder.

   On Windows, you can type `developer command prompt` in **Start** to open a developer command prompt.

1. Navigate to the component you want to try, for example `IncrementControl`, and run the `npm install` command to get all the required dependencies.

   Example: `[Download or Clone location]\component-framework\IncrementControl>npm install`

1. After the command has completed, run `msbuild /t:restore`.

   Example: `[Download or Clone location]\component-framework\IncrementControl>msbuild /t:restore`.

1. After the command completes, create a new folder inside the sample component folder using the `mkdir` command.

   Example: `[Download or Clone location]\component-framework\IncrementControl>mkdir IncrementControlSolution`

1. Navigate to the folder using the command `cd <folder name>`.

   Example: `[Download or Clone location]\component-framework\IncrementControl>cd IncrementControlSolution`

1. Inside the folder you created, run the [pac solution init](https://learn.microsoft.com/power-platform/developer/cli/reference/solution#pac-solution-init) command, providing the name and customization prefix of the publisher.

   Example: `\IncrementControlSolution>pac solution init --publisher-name powerapps_samples --publisher-prefix sample`

   > [!NOTE]
   > This command creates a new file named `IncrementControlSolution.cdsproj` in the folder.

1. Run the [pac solution add-reference](https://learn.microsoft.com/power-platform/developer/cli/reference/solution#pac-solution-add-reference) command with the `path` set to the location of the `IncrementControl.pcfproj` file.

   You can use a relative path like either of the following:

   ```
   \IncrementControlSolution>pac solution add-reference --path ../../IncrementControl
   ```

   ```
   \IncrementControlSolution>pac solution add-reference --path ../../IncrementControl/IncrementControl.pcfproj
   ```

   > [!IMPORTANT]
   > Reference the folder that contains the `.pcfproj` file for the control you want to add.

   After you run this command, within the `IncrementControlSolution.cdsproj` you find the following reference.

   ```xml
   <ItemGroup>
   <ProjectReference Include="..\IncrementControl.pcfproj" />
   </ItemGroup>
   ```

1. To generate a zip file from your solution project, run the following three commands in the location of the solution folder, for example `IncrementControlSolution`, you created:

   `msbuild /t:restore`

   `msbuild /t:rebuild /restore /p:Configuration=Release`

   `msbuild`

   The generated solution zip file becomes in the `IncrementControlSolution\bin\debug` folder.

1. Now that you have the zip file, you need to import it. You have two options:

   - Manually [import the solution](https://learn.microsoft.com/powerapps/maker/data-platform/import-update-export-solutions) into your environment using [make.powerapps.com](https://make.powerapps.com/).
   - Alternatively, to import the solution using Power Apps CLI commands, see the [Connecting to your environment](https://learn.microsoft.com/powerapps/developer/component-framework/import-custom-controls#connecting-to-your-environment) and [Deployment](https://learn.microsoft.com/powerapps/developer/component-framework/import-custom-controls#deploying-code-components) sections.

1. Finally, to add code components to your model-driven and canvas apps, see [Add components to model-driven apps](https://learn.microsoft.com/powerapps/developer/component-framework/add-custom-controls-to-a-field-or-entity) and [Add components to canvas apps](https://learn.microsoft.com/powerapps/developer/component-framework/component-framework-for-canvas-apps#add-components-to-a-canvas-app).

## Example output

The resulting console output is found using a copy of the downloaded repository zip file extracted to the `e:temp` folder:

```
**********************************************************************
** Visual Studio 2022 Developer Command Prompt v17.6.5
** Copyright (c) 2022 Microsoft Corporation
**********************************************************************

C:\Program Files\Microsoft Visual Studio\2022\Professional>e:

E:\>cd temp/powerapps-samples-master/component-framework

E:\temp\PowerApps-Samples-master\component-framework>cd incrementcontrol

E:\temp\PowerApps-Samples-master\component-framework\IncrementControl>npm install

added 706 packages, and audited 707 packages in 1m

123 packages are looking for funding
  run `npm fund` for details

found 0 vulnerabilities

E:\temp\PowerApps-Samples-master\component-framework\IncrementControl>msbuild /t:restore
MSBuild version 17.6.3+07e294721 for .NET Framework
Build started 8/1/2023 9:23:04 AM.

Project "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControl.pcfproj" on node 1 (Res
tore target(s)).
_GetAllRestoreProjectPathItems:
  Determining projects to restore...
Restore:
  X.509 certificate chain validation will use the default trust store selected by .NET for code signing.
  X.509 certificate chain validation will use the default trust store selected by .NET for timestamping.
  Restoring packages for E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControl.pcfproj
  ...
    GET https://api.nuget.org/v3-flatcontainer/microsoft.powerapps.msbuild.pcf/index.json
    OK https://api.nuget.org/v3-flatcontainer/microsoft.powerapps.msbuild.pcf/index.json 65ms
  Generating MSBuild file E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\obj\IncrementControl.pc
  fproj.nuget.g.props.
  Generating MSBuild file E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\obj\IncrementControl.pc
  fproj.nuget.g.targets.
  Writing assets file to disk. Path: E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\obj\project.
  assets.json
  Restored E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControl.pcfproj (in 977 ms).

  NuGet Config files used:
      C:\Users\<you>\AppData\Roaming\NuGet\NuGet.Config
      C:\Program Files (x86)\NuGet\Config\Microsoft.VisualStudio.Offline.config

  Feeds used:
      https://api.nuget.org/v3/index.json
      C:\Program Files (x86)\Microsoft SDKs\NuGetPackages\
Done Building Project "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControl.pcfproj"
(Restore target(s)).


Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:03.77

E:\temp\PowerApps-Samples-master\component-framework\IncrementControl>mkdir IncrementControlSolution

E:\temp\PowerApps-Samples-master\component-framework\IncrementControl>cd IncrementControlSolution

E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSolution>pac solution init --publisher-name powerapps_samples --publisher-prefix sample
Dataverse solution project with name 'IncrementControlSolution' created successfully in: 'E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSolution'
Dataverse solution files were successfully created for this project in the sub-directory Other, using solution name IncrementControlSolution, publisher name powerapps_samples, and customization prefix sample.
Please verify the publisher information and solution name found in the Solution.xml file.

E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSolution>pac solution add-reference --path ../../IncrementControl
Project reference successfully added to Dataverse solution project.

E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSolution>msbuild /t:restore
MSBuild version 17.6.3+07e294721 for .NET Framework
Build started 8/1/2023 9:24:55 AM.

Project "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSolution\IncrementContro
lSolution.cdsproj" on node 1 (Restore target(s)).
_GetAllRestoreProjectPathItems:
  Determining projects to restore...
Restore:
  X.509 certificate chain validation will use the default trust store selected by .NET for code signing.
  X.509 certificate chain validation will use the default trust store selected by .NET for timestamping.
  Restoring packages for E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSolution
  \IncrementControlSolution.cdsproj...
  Assets file has not changed. Skipping assets file writing. Path: E:\temp\PowerApps-Samples-master\component-framework
  \IncrementControl\obj\project.assets.json
  Restored E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControl.pcfproj (in 120 ms).
    GET https://api.nuget.org/v3-flatcontainer/microsoft.powerapps.msbuild.solution/index.json
    CACHE https://api.nuget.org/v3-flatcontainer/microsoft.powerapps.msbuild.pcf/index.json
    OK https://api.nuget.org/v3-flatcontainer/microsoft.powerapps.msbuild.solution/index.json 216ms
  Generating MSBuild file E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSolutio
  n\obj\IncrementControlSolution.cdsproj.nuget.g.props.
  Generating MSBuild file E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSolutio
  n\obj\IncrementControlSolution.cdsproj.nuget.g.targets.
  Writing assets file to disk. Path: E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementCon
  trolSolution\obj\project.assets.json
  Restored E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSolution\IncrementCont
  rolSolution.cdsproj (in 994 ms).

  NuGet Config files used:
      C:\Users\<you>\AppData\Roaming\NuGet\NuGet.Config
      C:\Program Files (x86)\NuGet\Config\Microsoft.VisualStudio.Offline.config

  Feeds used:
      https://api.nuget.org/v3/index.json
      C:\Program Files (x86)\Microsoft SDKs\NuGetPackages\
  1 of 2 projects are up-to-date for restore.
Done Building Project "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSolution\I
ncrementControlSolution.cdsproj" (Restore target(s)).


Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:02.24

E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSolution>msbuild /t:rebuild /restore /p:Configuration=Release
MSBuild version 17.6.3+07e294721 for .NET Framework
Build started 8/1/2023 9:25:16 AM.

Project "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSolution\IncrementContro
lSolution.cdsproj" on node 1 (Restore target(s)).
_GetAllRestoreProjectPathItems:
  Determining projects to restore...
Restore:
  X.509 certificate chain validation will use the default trust store selected by .NET for code signing.
  X.509 certificate chain validation will use the default trust store selected by .NET for timestamping.
  Assets file has not changed. Skipping assets file writing. Path: E:\temp\PowerApps-Samples-master\component-framework
  \IncrementControl\obj\project.assets.json
  Assets file has not changed. Skipping assets file writing. Path: E:\temp\PowerApps-Samples-master\component-framework
  \IncrementControl\IncrementControlSolution\obj\project.assets.json
  Restored E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSolution\IncrementCont
  rolSolution.cdsproj (in 119 ms).
  Restored E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControl.pcfproj (in 119 ms).

  NuGet Config files used:
      C:\Users\<you>\AppData\Roaming\NuGet\NuGet.Config
      C:\Program Files (x86)\NuGet\Config\Microsoft.VisualStudio.Offline.config

  Feeds used:
      https://api.nuget.org/v3/index.json
      C:\Program Files (x86)\Microsoft SDKs\NuGetPackages\
  All projects are up-to-date for restore.
Done Building Project "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSolution\I
ncrementControlSolution.cdsproj" (Restore target(s)).

Project "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSolution\IncrementContro
lSolution.cdsproj" on node 1 (rebuild target(s)).
CoreClean:
  Creating directory "obj\Release\".
Project "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSolution\IncrementContro
lSolution.cdsproj" (1:7) is building "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementCo
ntrol.pcfproj" (2:6) on node 1 (Clean target(s)).
CoreClean:
  Creating directory "obj\Release\".
PcfClean:
  npm run clean -- --noColor --buildMode production --outDir "E:\temp\PowerApps-Samples-master\component-framework\Incr
  ementControl\out\controls" --buildSource MSBuild

  > pcf-project@1.0.0 clean
  > pcf-scripts clean --noColor --buildMode production --outDir E:\temp\PowerApps-Samples-master\component-framework\In
  crementControl\out\controls --buildSource MSBuild

  [9:25:33 AM] [clean]  Initializing...
  [9:25:33 AM] [clean]  Cleaning build outputs...
  [9:25:33 AM] [clean]  Succeeded
Done Building Project "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControl.pcfproj"
(Clean target(s)).

AfterClean:
  Cleaning output directory: bin\Release\, Intermediate directory: obj\Release\ and Solution Packager working directory
  : obj\Release\
  Directory "bin\Release\" doesn't exist. Skipping.
  Removing directory "obj\Release\".
  Directory "obj\Release\" doesn't exist. Skipping.
  Removing log file: obj\Release\SolutionPackager.log and generated solution package: bin\Release\IncrementControlSolut
  ion.zip
PrepareForBuild:
  Creating directory "bin\Release\".
  Creating directory "obj\Release\".
Project "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSolution\IncrementContro
lSolution.cdsproj" (1:7) is building "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementCo
ntrol.pcfproj" (2:7) on node 1 (default targets).
_PcfAutoNpmInstall:
Skipping target "_PcfAutoNpmInstall" because all output files are up-to-date with respect to the input files.
PcfBuild:
  npm run build -- --noColor --buildMode production --outDir "E:\temp\PowerApps-Samples-master\component-framework\Incr
  ementControl\out\controls" --buildSource MSBuild

  > pcf-project@1.0.0 build
  > pcf-scripts build --noColor --buildMode production --outDir E:\temp\PowerApps-Samples-master\component-framework\In
  crementControl\out\controls --buildSource MSBuild

  [9:25:39 AM] [build]  Initializing...
  [9:25:39 AM] [build]  Validating manifest...
  [9:25:39 AM] [build]  Validating control...
  [9:25:42 AM] [build]  Generating manifest types...
  DeprecationWarning: 'createInterfaceDeclaration' has been deprecated since v4.8.0. Decorators are no longer supported
   for this function. Callers should switch to an overload that does not accept a 'decorators' parameter.
  [9:25:42 AM] [build]  Generating design types...
  [9:25:42 AM] [build]  Running ESLint...
  [9:25:51 AM] [build]  Compiling and bundling control...
  [Webpack stats]:
  asset bundle.js 1.53 KiB [emitted] [minimized] (name: main) 1 related asset
  ./IncrementControl/index.ts 5.22 KiB [built] [code generated]
  webpack 5.88.2 compiled successfully in 11437 ms
  [9:26:10 AM] [build]  Generating build outputs...
  [9:26:10 AM] [build]  Succeeded
Done Building Project "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControl.pcfproj"
(default targets).

ResolveAssemblyReferences:
  Primary reference "IncrementControl".
      Could not find dependent files. Expected file "E:\temp\PowerApps-Samples-master\component-framework\IncrementCont
  rol\out\controls\IncrementControl.exe" does not exist.
      Could not find dependent files. Expected file "E:\temp\PowerApps-Samples-master\component-framework\IncrementCont
  rol\out\controls\IncrementControl.exe" does not exist.
      Resolved file path is "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\out\controls\Increme
  ntControl.exe".
      Reference found at search path location "".
      The ImageRuntimeVersion for this reference is "".
CopyCdsSolutionContent:
  Creating directory "obj\Release\Metadata\Other".
  Creating directory "obj\Release\Metadata\Other".
  Creating directory "obj\Release\Metadata\Other".
  Copying file from "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSolution\src
  \Other\Customizations.xml" to "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControl
  Solution\obj\Release\Metadata\Other\Customizations.xml".
  Copying file from "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSolution\src
  \Other\Relationships.xml" to "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlS
  olution\obj\Release\Metadata\Other\Relationships.xml".
  Copying file from "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSolution\src
  \Other\Solution.xml" to "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSoluti
  on\obj\Release\Metadata\Other\Solution.xml".
ProcessCdsProjectReferencesOutputs:
  Processing output of component type: Pcf, source output directory: E:\temp\PowerApps-Samples-master\component-framewo
  rk\IncrementControl\out\controls\, destination directory: E:\temp\PowerApps-Samples-master\component-framework\Increm
  entControl\IncrementControlSolution\obj\Release\Metadata\Controls
  Process: Output copied from source directory: E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\o
  ut\controls\IncrementControl to destination directory: E:\temp\PowerApps-Samples-master\component-framework\Increment
  Control\IncrementControlSolution\obj\Release\Metadata\Controls\sample_SampleNamespace.IncrementControl
  Process: Control data node created in E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\Increment
  ControlSolution\obj\Release\Metadata\Controls\sample_SampleNamespace.IncrementControl
  Process: Root Component of type: 66, schema name: sample_SampleNamespace.IncrementControl added to Solution
PowerAppsPackage:
  Running Solution Packager to build package type: Managed bin\Release\IncrementControlSolution.zip

Packing E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSolution\obj\Release\Metadata to bin\Release\IncrementControlSolution.zip

Processing Component: Entities
Processing Component: Roles
Processing Component: Workflows
Processing Component: FieldSecurityProfiles
Processing Component: Templates
Processing Component: EntityMaps
Processing Component: EntityRelationships
Processing Component: OrganizationSettings
Processing Component: optionsets
Processing Component: CustomControls
 - SampleNamespace.IncrementControl
Processing Component: SolutionPluginAssemblies
Processing Component: EntityDataProviders

Managed Pack complete.

  Solution: bin\Release\IncrementControlSolution.zip generated.
  Solution Package Type: Managed generated.
  Solution Packager log path: obj\Release\SolutionPackager.log.
  Solution Packager error level: Info.
CleanUpIntermediateFiles:
  Removing directory "obj\Release\Metadata".
  Completed intermiediate files clean up.
Done Building Project "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSolution\I
ncrementControlSolution.cdsproj" (rebuild target(s)).


Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:54.59

E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSolution>msbuild
MSBuild version 17.6.3+07e294721 for .NET Framework
Build started 8/1/2023 9:26:21 AM.

Project "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSolution\IncrementContro
lSolution.cdsproj" on node 1 (default targets).
PrepareForBuild:
  Creating directory "bin\Debug\".
  Creating directory "obj\Debug\".
Project "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSolution\IncrementContro
lSolution.cdsproj" (1) is building "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementCont
rol.pcfproj" (2:2) on node 1 (default targets).
_PcfAutoNpmInstall:
Skipping target "_PcfAutoNpmInstall" because all output files are up-to-date with respect to the input files.
PrepareForBuild:
  Creating directory "obj\Debug\".
PcfBuild:
  npm run build -- --noColor --buildMode development --outDir "E:\temp\PowerApps-Samples-master\component-framework\Inc
  rementControl\out\controls" --buildSource MSBuild

  > pcf-project@1.0.0 build
  > pcf-scripts build --noColor --buildMode development --outDir E:\temp\PowerApps-Samples-master\component-framework\I
  ncrementControl\out\controls --buildSource MSBuild

  [9:26:26 AM] [build]  Initializing...
  [9:26:26 AM] [build]  Validating manifest...
  [9:26:26 AM] [build]  Validating control...
  [9:26:27 AM] [build]  Generating manifest types...
  DeprecationWarning: 'createInterfaceDeclaration' has been deprecated since v4.8.0. Decorators are no longer supported
   for this function. Callers should switch to an overload that does not accept a 'decorators' parameter.
  [9:26:27 AM] [build]  Generating design types...
  [9:26:27 AM] [build]  Running ESLint...
  [9:26:29 AM] [build]  Compiling and bundling control...
  [Webpack stats]:
  asset bundle.js 6.87 KiB [emitted] (name: main)
  ./IncrementControl/index.ts 5.22 KiB [built] [code generated]
  webpack 5.88.2 compiled successfully in 2712 ms
  [9:26:32 AM] [build]  Generating build outputs...
  [9:26:32 AM] [build]  Succeeded
Done Building Project "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControl.pcfproj"
(default targets).

ResolveAssemblyReferences:
  Primary reference "IncrementControl".
      Could not find dependent files. Expected file "E:\temp\PowerApps-Samples-master\component-framework\IncrementCont
  rol\out\controls\IncrementControl.exe" does not exist.
      Could not find dependent files. Expected file "E:\temp\PowerApps-Samples-master\component-framework\IncrementCont
  rol\out\controls\IncrementControl.exe" does not exist.
      Resolved file path is "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\out\controls\Increme
  ntControl.exe".
      Reference found at search path location "".
      The ImageRuntimeVersion for this reference is "".
CopyCdsSolutionContent:
  Creating directory "obj\Debug\Metadata\Other".
  Creating directory "obj\Debug\Metadata\Other".
  Creating directory "obj\Debug\Metadata\Other".
  Copying file from "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSolution\src
  \Other\Solution.xml" to "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSoluti
  on\obj\Debug\Metadata\Other\Solution.xml".
  Copying file from "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSolution\src
  \Other\Customizations.xml" to "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControl
  Solution\obj\Debug\Metadata\Other\Customizations.xml".
  Copying file from "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSolution\src
  \Other\Relationships.xml" to "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlS
  olution\obj\Debug\Metadata\Other\Relationships.xml".
ProcessCdsProjectReferencesOutputs:
  Processing output of component type: Pcf, source output directory: E:\temp\PowerApps-Samples-master\component-framewo
  rk\IncrementControl\out\controls\, destination directory: E:\temp\PowerApps-Samples-master\component-framework\Increm
  entControl\IncrementControlSolution\obj\Debug\Metadata\Controls
  Process: Output copied from source directory: E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\o
  ut\controls\IncrementControl to destination directory: E:\temp\PowerApps-Samples-master\component-framework\Increment
  Control\IncrementControlSolution\obj\Debug\Metadata\Controls\sample_SampleNamespace.IncrementControl
  Process: Control data node created in E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\Increment
  ControlSolution\obj\Debug\Metadata\Controls\sample_SampleNamespace.IncrementControl
  Process: Root Component of type: 66, schema name: sample_SampleNamespace.IncrementControl added to Solution
PowerAppsPackage:
  Running Solution Packager to build package type: Unmanaged bin\Debug\IncrementControlSolution.zip

Packing E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSolution\obj\Debug\Metadata to bin\Debug\IncrementControlSolution.zip

Processing Component: Entities
Processing Component: Roles
Processing Component: Workflows
Processing Component: FieldSecurityProfiles
Processing Component: Templates
Processing Component: EntityMaps
Processing Component: EntityRelationships
Processing Component: OrganizationSettings
Processing Component: optionsets
Processing Component: CustomControls
 - SampleNamespace.IncrementControl
Processing Component: SolutionPluginAssemblies
Processing Component: EntityDataProviders

Unmanaged Pack complete.

  Solution: bin\Debug\IncrementControlSolution.zip generated.
  Solution Package Type: Unmanaged generated.
  Solution Packager log path: obj\Debug\SolutionPackager.log.
  Solution Packager error level: Info.
CleanUpIntermediateFiles:
  Removing directory "obj\Debug\Metadata".
  Completed intermiediate files clean up.
Done Building Project "E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSolution\I
ncrementControlSolution.cdsproj" (default targets).


Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:11.47

E:\temp\PowerApps-Samples-master\component-framework\IncrementControl\IncrementControlSolution>
```
