# README SecondaryRecordSource

SecondaryRecordSource is a sample implementation of an IRecordSource to configure as part of TimelineWallControl's Custom Connector feature.

The Custom Connector feature for the timeline is a way for developers to surface information, such as records, within the TimelineWallControl component. It allows for a broader set of scenarios in addition to the existing out-of-box notes, posts, and activities.

## Records from SecondaryRecordSource

Records from SecondaryRecordSource are three records. The sort date is based off the current DateTime when init behavior is called. One record has sort date of current DateTime, one a week earlier, and one a week later. Other information rendered as IRecordFields are inline examples which will reflect the field section they are within, along with responding to Expand/Collapse state as passed within IRecordUXRequest param of getRecordUX behavior.

## Sample IRecordSource solution
This sample includes a managed solution. This solution contains:

- a javascript web resource "new_SecondaryRecordSource" which has an implementation of IRecordSource with full namespace "SampleNamespace.SecondaryRecordSource"
- customization changes for the "Account for Interactive Experiences" form (default for Customer Service Hub single-session) and the "Account for Multisession Experiences" form (default for Customer Service Workspace multi-session)
    - Timeline control parameters includes <UClientRecordSourcesJSON>{"recordSources": [{"name": "new_SecondaryRecordSource", "constructor": "SampleNamespace.SecondaryRecordSource"}]}</UClientRecordSourcesJSON>

After importing solution, emptying cache and hard refresh of the environment within browser should ensure changes are reflected within the forms.