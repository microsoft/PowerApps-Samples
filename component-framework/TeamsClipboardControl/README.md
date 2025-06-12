---
languages:
  - typescript
products:
  - power-platform
  - power-apps
  - microsoft-teams
page_type: sample
description: "A PowerApps component framework sample that provides reliable clipboard functionality when running within Microsoft Teams environments."
---

# Teams Clipboard Control Power Apps component framework sample

This sample demonstrates how to create a custom control that enables reliable clipboard functionality in PowerApps running within Microsoft Teams. The standard Copy() function often doesn't work due to browser security restrictions and iframe limitations in Teams environments.

![Preview of the sample](assets/teams-clipboard-control-preview.png)

## Features

- **Multiple Clipboard Strategies**: Attempts various methods to copy text to clipboard
- **Teams Integration**: Leverages Microsoft Teams SDK when available
- **Fallback Support**: Provides manual copy options when automated methods fail
- **User Feedback**: Visual notifications for copy success/failure states
- **Datetime & Username Support**: Optimized for common PowerApps data types

## Clipboard Strategies

The control attempts to copy text using the following methods in order:

1. **Modern Clipboard API** - `navigator.clipboard.writeText()` for modern browsers
2. **Microsoft Teams SDK** - Uses Teams SDK clipboard API if available
3. **execCommand with Hidden Input** - Legacy method using temporary input element
4. **Selection-based Copying** - Text selection with execCommand
5. **Manual Copy Fallback** - Displays text for manual copying when all else fails

## Properties

| Property | Type | Description | Required |
|----------|------|-------------|----------|
| textToCopy | SingleLine.Text | The text value to be copied (e.g., formatted datetime, username) | Yes |
| buttonLabel | SingleLine.Text | Custom label for the copy button (defaults to "Copy") | No |

## Use Cases

This control is particularly useful for:

- Copying formatted datetime values (UTC - YYYY-MM-DD HH:MM:SS)
- Copying usernames and user identifiers
- Any scenario where users need to copy text within Teams-hosted PowerApps
- Applications requiring reliable clipboard functionality across different browsers and environments

## Compatibility

This sample works for model-driven and canvas apps, with special optimizations for Microsoft Teams environments.

## Applies to

[Power Apps component framework](https://learn.microsoft.com/power-apps/developer/component-framework/overview)

Get your own free development tenant by subscribing to [Power Apps Developer Plan](https://learn.microsoft.com/power-platform/developer/plan).

## Contributors

This sample was created as part of the Microsoft PowerApps code samples collection.

## Version history

| Version | Date             | Comments       |
| ------- | ---------------- | -------------- |
| 1.0     | January 18, 2024 | Initial release |

## Prerequisites

[Install the Microsoft Power Platform CLI](https://learn.microsoft.com/power-platform/developer/cli/introduction).

## Try this sample

Follow the steps in the [README.md](../README.md) to generate solutions containing the controls so you can import and try the sample components in your model-driven or canvas app.

## Implementation Details

### Microsoft Teams Integration

The control detects if it's running within Microsoft Teams and attempts to use the Teams SDK for clipboard operations:

```typescript
// Check if Teams SDK is available
if (typeof window !== "undefined" && window.microsoftTeams) {
    this._teamsSDKAvailable = true;
}
```

### Multiple Fallback Strategies

When the primary clipboard methods fail, the control provides increasingly robust fallbacks:

1. **Modern API**: Uses the standard `navigator.clipboard.writeText()`
2. **Teams SDK**: Leverages `microsoftTeams.clipboard.writeText()`
3. **Hidden Input**: Creates a temporary input element for `execCommand('copy')`
4. **Text Selection**: Selects visible text and copies via `execCommand('copy')`
5. **Manual Fallback**: Displays text in a special container for manual copying

### User Experience

- **Visual Feedback**: Success/error messages with color-coded styling
- **Button States**: Disabled state during copy operations with "Copying..." text
- **Auto-hide Feedback**: Messages automatically disappear after 3 seconds
- **Responsive Design**: Adapts to different container sizes

## Configuration Example

In your PowerApp, bind the control properties as follows:

```
textToCopy: Text(Now(), "yyyy-mm-dd hh:mm:ss") & " UTC"
buttonLabel: "Copy Timestamp"
```

or

```
textToCopy: User().FullName
buttonLabel: "Copy Username"
```

## Browser Support

The control is designed to work across various browsers and environments:

- **Chrome/Edge**: Full support with modern Clipboard API
- **Firefox**: Support via execCommand fallback
- **Safari**: Support with security limitations
- **Teams Desktop**: Enhanced support via Teams SDK
- **Teams Web**: Standard web browser capabilities

## Security Considerations

The control respects browser security policies:

- **HTTPS Required**: Modern clipboard APIs require secure contexts
- **User Gesture**: Copy operations must be triggered by user interaction
- **Permission Handling**: Gracefully handles clipboard permission denials
- **Cross-origin Safety**: Works within iframe restrictions

## Styling

The control includes comprehensive CSS styling that can be customized:

- **Teams Theme Compatibility**: Colors match Teams design system
- **Responsive Layout**: Flexbox-based layout adapts to container
- **Accessibility**: High contrast support and keyboard navigation
- **Custom Properties**: Easy customization via CSS variables

## Troubleshooting

Common issues and solutions:

1. **"Copy failed" error**: Usually indicates browser security restrictions
2. **No feedback shown**: Check if the control has valid text to copy
3. **Teams SDK not working**: Verify the app is running within Teams context
4. **Manual copy required**: Expected behavior in highly restricted environments

## Related Information

- [Microsoft Teams JavaScript SDK](https://learn.microsoft.com/microsoftteams/platform/tabs/how-to/using-teams-client-sdk)
- [Clipboard API](https://developer.mozilla.org/en-US/docs/Web/API/Clipboard_API)
- [Power Apps Component Framework](https://learn.microsoft.com/power-apps/developer/component-framework/overview)

## Disclaimer

**THIS CODE IS PROVIDED _AS IS_ WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**