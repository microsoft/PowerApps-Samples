---
languages:
  - typescript
products:
  - power-platform
  - power-apps
page_type: sample
description: "This sample demonstrates how to leverage the multi-select option set type on properties of field components in Microsoft Power Apps."
---

# Choices (multi-select option set) Power Apps component framework sample

This sample demonstrates how to leverage the multi-select option set type on properties of field components. By binding the code component's primary property to this type, users can create all new types of controls with a choices column.

![Preview of the sample](https://learn.microsoft.com/power-apps/developer/component-framework/media/multi-select-option-set-control.png)

## Compatibility

This sample works for model-driven apps.

## Applies to

[Power Apps component framework](https://learn.microsoft.com/power-apps/developer/component-framework/overview)

Get your own free development tenant by subscribing to [Power Apps Developer Plan](https://learn.microsoft.com/power-platform/developer/plan).

## Contributors

This sample was created by the Power Apps component framework team.

## Version history

| Version | Date             | Comments       |
| ------- | ---------------- | -------------- |
| 1.0     | January 18, 2024 | README created |

## Prerequisites

[Install the Microsoft Power Platform CLI](https://learn.microsoft.com/power-platform/developer/cli/introduction)

## Try this sample

Follow the steps in the [README.md](../README.md) to generate solutions containing the controls so you can import and try the sample components in your model-driven or canvas app.

> [!NOTE]
> When you run this sample using the test harness, you get the following error:
> ```
>  Error occured during initilization of control: SampleNamespace.MultiSelectOptionSetControl;Message: Cannot read properties of undefined (reading 'forEach')
>  TypeError: Cannot read properties of undefined (reading 'forEach') at multiSelectOptionSetControl.init
>  ```
> The option set control data can't be simulated or mocked in the test harness. This sample only runs correctly when you configure it for a field in a model-driven app.

## Related information

[Implementing choices (multi select option set) component](https://learn.microsoft.com/power-apps/developer/component-framework/sample-controls/multi-select-option-set-control).

## Disclaimer

**THIS CODE IS PROVIDED _AS IS_ WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**
