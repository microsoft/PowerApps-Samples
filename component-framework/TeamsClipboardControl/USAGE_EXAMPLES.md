# Teams Clipboard Control Usage Examples

This document provides practical examples of how to use the Teams Clipboard Control in your PowerApps.

## Basic Setup

1. **Import the Solution**: Follow the [main README](README.md) to build and import the TeamsClipboardControl solution
2. **Add to App**: Insert the custom control into your PowerApp
3. **Configure Properties**: Set the required `textToCopy` property

## Common Use Cases

### 1. Copy Formatted DateTime (UTC)

Perfect for copying timestamps that users need to share or document:

```
// In PowerApps expression for textToCopy property:
Text(Now(), "yyyy-mm-dd hh:mm:ss") & " UTC"

// Or for a specific date field:
Text(YourDateTimeField, "yyyy-mm-dd hh:mm:ss") & " UTC"

// Button label:
"Copy Timestamp"
```

**Example Output**: `2024-01-18 14:30:25 UTC`

### 2. Copy Username/User Information

Useful for support scenarios or user identification:

```
// Copy current user's full name:
User().FullName

// Copy user's email:
User().Email

// Copy user display name with department:
User().FullName & " (" & Office365Users.MyProfile().Department & ")"

// Button label:
"Copy User Info"
```

**Example Output**: `John Smith (Engineering)`

### 3. Copy Record IDs or Reference Numbers

Essential for support tickets, case numbers, or database references:

```
// Copy a case number:
"Case #" & YourRecordIDField

// Copy formatted record reference:
"REF: " & Text(YourRecord.ID) & " - " & YourRecord.Title

// Button label:
"Copy Reference"
```

**Example Output**: `Case #12345` or `REF: 67890 - Customer Support Request`

### 4. Copy URLs or Deep Links

For sharing links to specific records or pages:

```
// Copy a formatted URL:
"https://yourorg.sharepoint.com/lists/Items/DispForm.aspx?ID=" & YourRecord.ID

// Copy Teams meeting link:
YourMeeting.JoinWebUrl

// Button label:
"Copy Link"
```

### 5. Copy JSON or Structured Data

For developers or advanced scenarios:

```
// Copy record as JSON:
JSON(YourRecord, JSONFormat.Compact)

// Copy formatted data structure:
"{""name"":""" & YourRecord.Name & """,""id"":" & YourRecord.ID & "}"

// Button label:
"Copy JSON"
```

## Advanced Configuration Examples

### Dynamic Button Labels

Make the button text context-aware:

```
// Change button text based on content:
If(
    IsBlank(YourTextField),
    "Nothing to Copy",
    If(
        Len(YourTextField) > 50,
        "Copy Long Text",
        "Copy Text"
    )
)
```

### Conditional Visibility

Show/hide the control based on context:

```
// Control visibility:
!IsBlank(YourDataField) && YourRecord.Status = "Active"
```

### Multiple Copy Controls

Use multiple controls for different data types:

```
// Control 1 - DateTime
textToCopy: Text(YourRecord.CreatedOn, "yyyy-mm-dd hh:mm:ss") & " UTC"
buttonLabel: "Copy Date"

// Control 2 - User Info  
textToCopy: YourRecord.AssignedTo.FullName
buttonLabel: "Copy User"

// Control 3 - Reference
textToCopy: "REF-" & YourRecord.ID
buttonLabel: "Copy ID"
```

## Teams-Specific Scenarios

### Meeting Information

Perfect for Teams meeting scenarios:

```
// Copy meeting details:
YourMeeting.Subject & " - " & Text(YourMeeting.StartTime, "mmm dd, yyyy h:mm AM/PM")

// Copy participant list:
Concat(YourMeeting.Participants, DisplayName, ", ")

// Copy Teams channel link:
"https://teams.microsoft.com/l/channel/" & YourChannel.ID
```

### Channel and Team References

```
// Copy team information:
"Team: " & YourTeam.DisplayName & " | Channel: " & YourChannel.DisplayName

// Copy @mention format:
"<at>" & User().FullName & "</at>"
```

## Responsive Design Tips

The control adapts to different container sizes:

```
// For narrow containers, use shorter button labels:
If(Parent.Width < 300, "Copy", "Copy to Clipboard")

// For mobile, consider vertical layout:
If(Parent.Width < 500, "📋 Copy", "📋 Copy to Clipboard")
```

## Error Handling and User Experience

### Validation Before Copying

```
// Only show control when data is valid:
!IsBlank(YourTextField) && Len(YourTextField) > 0

// Provide user feedback for invalid data:
If(
    IsBlank(YourTextField),
    "No data available",
    YourTextField
)
```

### Accessibility Considerations

- Use descriptive button labels: `"Copy timestamp for " & YourRecord.Title`
- Ensure color contrast meets accessibility standards
- Test with screen readers in Teams environment

## Testing Scenarios

### Browser Compatibility Testing

Test the control in different environments:

1. **Teams Desktop App**: Best performance with Teams SDK
2. **Teams Web (Chrome/Edge)**: Modern clipboard API support
3. **Teams Web (Firefox)**: Falls back to execCommand
4. **Teams Mobile**: Manual copy fallback

### Permission Testing

Test in different security contexts:

1. **Normal permissions**: All methods should work
2. **Restricted permissions**: Should fall back gracefully
3. **Iframe restrictions**: Should show manual copy option

## Troubleshooting Common Issues

### "Copy failed" Error

**Cause**: Browser security restrictions or lack of user gesture
**Solution**: Ensure copy is triggered by direct user interaction

### Manual Copy Always Shows

**Cause**: All clipboard methods are blocked
**Solution**: Expected in highly secure environments - manual copy is the intended fallback

### Teams SDK Not Working

**Cause**: App not running in Teams context or old Teams client
**Solution**: Control automatically falls back to standard clipboard methods

### Button Not Responding

**Cause**: Empty or invalid text to copy
**Solution**: Check that `textToCopy` property has valid, non-empty content

## Performance Optimization

### Minimize Re-renders

```
// Use stable expressions to avoid unnecessary updates:
Set(varTimestamp, Text(Now(), "yyyy-mm-dd hh:mm:ss") & " UTC")

// Then reference the variable:
textToCopy: varTimestamp
```

### Efficient String Building

```
// Instead of multiple concatenations in the property:
Set(varUserInfo, 
    User().FullName & " - " & 
    User().Email & " - " & 
    Text(Now(), "mm/dd/yyyy")
)

// Use the variable:
textToCopy: varUserInfo
```

This approach ensures optimal performance and user experience in Microsoft Teams environments.