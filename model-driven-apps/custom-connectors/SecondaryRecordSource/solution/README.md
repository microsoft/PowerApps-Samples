# README SecondaryRecordSource

SecondaryRecordSource is a sample implementation of an IRecordSource to configure as part of TimelineWallControl's custom connector feature.

Custom connectors is a feature that provides a way for developers to surface information such as Dataverse table rows, external data sources, and so forth, as records entries within the TimelineWallControl component. It allows for a broader set of scenarios in addition to the existing out-of-box notes, posts, and activities.

For more information, see [Use custom connectors with the timeline control](https://learn.microsoft.com/powerapps/maker/model-driven-apps/custom-connectors-timeline-control).

## What this sample does

This sample uses the custom connector feature of TimelineWallControl to configure an IRecordSource to surface information as custom records.

## How this sample works

This sample includes a managed solution. This solution contains:

- a JavaScript web resource "new_SecondaryRecordSource" which has an implementation of IRecordSource with full namespace "SampleNamespace.SecondaryRecordSource"
- customization changes for the "Account for Interactive Experiences" form (default for Customer Service Hub single-session) and the "Account for Multisession Experiences" form (default for Customer Service Workspace multi-session)
    - Timeline control parameters includes 
    ```xml
    <UClientRecordSourcesJSON>
        {"recordSources": [{
            "name": "new_SecondaryRecordSource", 
            "constructor": "SampleNamespace.SecondaryRecordSource"}]
        }
    </UClientRecordSourcesJSON>
    ```

### Setup

1. Ensure the table forms used in sample have TimelineWallControl configured.
- Configured table forms are "Account for Interactive Experiences" form (default for Customer Service Hub single-session) and the "Account for Multisession Experiences" form (default for Customer Service Workspace multi-session)
- TimelineWallControl control is within first tab of these forms by default

2. Download the managed solution "SecondaryRecordSource_SampleSolution_1_23091_1_managed.zip"

3. Install managed solution within D365 test environment

4. Visit either of the configured table forms ("Account for Interactive Experiences" or "Account for Multisession Experiences")
- Empty the cache and do a hard refresh of the environment within the browser. This should ensure changes are reflected within the forms. 

### Demonstrate

TimelineWallControl will now display the three records implemented within the SecondaryRecordSource implementation. The sort date is based off the current DateTime when init behavior is called. One record has sort date of current DateTime, one a week earlier, and one a week later. Other information rendered as IRecordFields are inline examples which will reflect the field section they are within, along with responding to Expand/Collapse state as passed within IRecordUXRequest param of getRecordUX behavior.

### Cleanup

1. Uninstall the managed solution "SecondaryRecordSource_SampleSolution_1_23091_1_managed.zip"
