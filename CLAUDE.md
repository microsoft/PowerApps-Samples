# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository Overview

This repository contains sample code for Power Apps across multiple technology stacks:
- **component-framework/**: Power Apps Component Framework (PCF) controls (~29 TypeScript/React controls)
- **dataverse/**: Dataverse SDK samples (C#, .NET Core, Web API, Plugins)
- **ai-builder/**: AI Builder sample data and model packages
- **model-driven-apps/**: Model-driven app samples
- **portals/**: Power Apps Portals samples (OAuth, Web API)
- **powershell/** & **build-tools/**: Deployment automation

## Building and Testing

### Power Apps Component Framework (PCF)

Each PCF control is a self-contained project with TypeScript/React frontend and MSBuild packaging.

**Basic Commands:**
```bash
cd component-framework/[ControlName]

# Install dependencies
npm install

# Build (development)
npm run build

# Build and watch for changes
npm start:watch

# Lint
npm run lint
npm run lint:fix

# Clean
npm run clean
```

**Creating a Solution Package:**
```bash
# Restore MSBuild dependencies
msbuild /t:restore

# Create solution folder
mkdir SolutionFolder
cd SolutionFolder

# Initialize solution
pac solution init --publisher-name sample --publisher-prefix sample

# Add PCF reference
pac solution add-reference --path ../

# Build release package (creates .zip)
msbuild /t:rebuild /p:Configuration=Release
```

**Key Files:**
- `ControlManifest.Input.xml` - Control metadata and property definitions
- `package.json` - npm scripts and dependencies (uses `pcf-scripts`)
- `.pcfproj` - MSBuild project that orchestrates build
- `pcfconfig.json` - Output directory configuration
- `index.ts` - Main control implementation (init, updateView, destroy lifecycle)

**Build Output:**
- Webpack bundles to `out/controls/{ControlName}/`
- Solution packages in `bin/Debug/` and `bin/Release/`

### Dataverse C# Samples (.NET Framework 4.7.2)

Located in `dataverse/orgsvc/CSharp/` - legacy samples using traditional .NET Framework.

**Build:**
```bash
cd dataverse/orgsvc/CSharp/[SampleName]

# Restore NuGet packages (if needed)
nuget restore packages.config

# Build
msbuild

# Clean and rebuild
msbuild /t:clean,rebuild
```

**Configuration:**
- Uses `App.config` for connection strings
- Dependencies in `packages.config` (NuGet classic)
- References `Microsoft.CrmSdk.CoreAssemblies` ~9.0.2
- Uses ADAL for authentication

### Dataverse C#-NETCore Samples (.NET 6.0+)

Located in `dataverse/orgsvc/CSharp-NETCore/` - modern .NET samples with better patterns.

**Build and Run:**
```bash
cd dataverse/orgsvc/CSharp-NETCore/[Category]/[SampleName]

# Build (implicit restore)
dotnet build

# Run
dotnet run

# Run specific project
dotnet run --project ./Program.csproj
```

**Configuration:**
- Uses shared `appsettings.json` at category level for connection strings
- Modern `Microsoft.PowerPlatform.Dataverse.Client` package
- Supports `ILogger` and dependency injection patterns
- See `README-code-design.md` for architectural patterns

**appsettings.json structure:**
```json
{
  "ConnectionStrings": {
    "default": "AuthType=OAuth;Url=https://org.crm.dynamics.com;..."
  }
}
```

## Architecture Patterns

### PCF Control Lifecycle

All PCF controls follow this pattern in `index.ts`:

```typescript
export class ControlName implements ComponentFramework.StandardControl<IInputs, IOutputs> {
    init(context, notifyOutputChanged, state, container): void { }
    updateView(context): void { }
    getOutputs(): IOutputs { }
    destroy(): void { }
}
```

- **init**: Called once when control is first loaded
- **updateView**: Called when any value in context changes
- **getOutputs**: Returns values to framework (for bound properties)
- **destroy**: Cleanup before control removal

React-based controls typically render components in `updateView()`.

### Dataverse C#-NETCore Pattern

Modern samples follow this structure:

```
Category/
├── appsettings.json          # Shared configuration
├── Utility.cs                # Shared helper methods
├── Sample1/
│   ├── Sample1.csproj
│   ├── Program.cs
│   └── README.md
└── Sample2/
    ├── Sample2.csproj
    ├── Program.cs
    └── README.md
```

**Connection Pattern:**
```csharp
// From appsettings.json
var connectionString = config.GetConnectionString("default");
var service = new ServiceClient(connectionString);
```

### Dataverse C# Framework Pattern

Legacy samples each have isolated `.sln` files with:
- **SampleProgram.cs** - Entry point
- **SampleMethod.cs** - Core logic
- **Helper-Code/** - Reusable utilities (SystemUserProvider, SampleHelpers)
- **Login-UX/** - WPF login form (ExampleLoginForm.xaml)

## Key Technologies

### PCF Stack
- **TypeScript** - Primary language for control logic
- **React** - For complex UI controls (see `*ReactControl` samples)
- **Fluent UI** - Microsoft's design system (FacepileReactControl, ChoicesPickerReactControl)
- **Webpack** - Bundling (via pcf-scripts)
- **MSBuild** - Solution packaging

### Dataverse SDK Evolution

| Technology | C# Legacy | C#-NETCore |
|------------|-----------|------------|
| Framework | .NET 4.7.2 | .NET 6.0+ |
| Client | `Microsoft.CrmSdk.CoreAssemblies` | `Microsoft.PowerPlatform.Dataverse.Client` |
| Config | App.config | appsettings.json |
| Auth | ADAL | Modern OAuth |
| DI | Manual | Extensions.DependencyInjection |
| Logging | Console | ILogger |

## Sample Categories

### Component Framework Categories
- **Standard Controls**: IncrementControl, LinearInputControl
- **React Controls**: ChoicesPickerReactControl, FacepileReactControl
- **Dataset Controls**: DataSetGrid, ModelDrivenGridControl (for grid/table data)
- **API Demonstrations**: DeviceApiControl, FormattingAPIControl, NavigationAPIControl
- **Advanced**: AngularJSFlipControl, PowerAppsGridCustomizerControl

### Dataverse Sample Categories
- **Activities**: Email, appointments, tasks
- **AttachmentAndAnnotationOperations**: File handling
- **BulkOperations**: Bulk delete, bulk data operations
- **FileOperations**: File column operations
- **ImageOperations**: Image column handling
- **Schema**: Metadata operations, entity creation
- **Search**: Dataverse search, elastic search
- **Security**: Role management, sharing, field security
- **ServiceClient**: Modern client usage patterns
- **Solutions**: Solution management and deployment

## Common Pitfalls

1. **PCF builds fail**: Run `npm install` and `msbuild /t:restore` before building
2. **C# samples won't connect**: Update connection string in `App.config` or `appsettings.json`
3. **TypeScript errors in PCF**: Run `npm run refreshTypes` to update type definitions
4. **Missing dependencies**:
   - PCF: `npm install` + `msbuild /t:restore`
   - C#: `nuget restore` or `dotnet restore`
5. **PCF control not appearing**: Check ControlManifest.Input.xml namespace and version match solution references

## Testing Approach

- **PCF Controls**: Test harness via `npm start` - launches browser with control in test harness
- **Dataverse C# samples**: Standalone console apps - test by running directly against live environment
- **Integration**: Deploy PCF solutions to environment, create app/form to test control

## Shared Resources

- **dataverse/LoginUX/**: Reusable WPF login forms for C# samples
- **dataverse/SharedResources/**: Sample files and images used across multiple samples
- **component-framework/resources/**: PCF control templates

## Documentation Links

- [Power Apps Component Framework docs](https://learn.microsoft.com/power-apps/developer/component-framework/overview)
- [Dataverse developer docs](https://learn.microsoft.com/power-apps/developer/data-platform/)
- [Model-driven apps developer docs](https://learn.microsoft.com/power-apps/developer/model-driven-apps/)
