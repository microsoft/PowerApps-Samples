# Activities

Samples demonstrating how to work with activity entities in Dataverse including emails, appointments, tasks, faxes, and custom activities.

These samples show how to create, retrieve, update, and delete various types of activities, work with recurring appointments, manage email templates and attachments, and collaborate using activity feeds.

More information: [Activity entities](https://learn.microsoft.com/power-apps/developer/data-platform/activity-entities)

## Samples

|Sample folder|Description|Build target|
|---|---|---|
|BookAppointment|Book or schedule an appointment|.NET 6|
|BulkEmail|Send bulk email messages|.NET 6|
|CollaborateWithActivityFeeds|Use activity feeds for collaboration|.NET 6|
|ConvertToRecurring|Convert an appointment to a recurring appointment|.NET 6|
|CreateEmailUsingTemplate|Create email from a template|.NET 6|
|CRUDEmailAttachments|Create, retrieve, update, delete email attachments|.NET 6|
|CRUDRecurringAppointment|CRUD operations on recurring appointments|.NET 6|
|CustomActivity|Work with custom activity entities|.NET 6|
|EmailTemplate|Create and use email templates|.NET 6|
|EndRecurringAppointment|End a recurring appointment series|.NET 6|
|PromoteEmail|Promote an email to create related records|.NET 6|
|RecurringAppointment|Create and manage recurring appointments|.NET 6|
|RetrieveEmailAttach|Retrieve email attachments|.NET 6|
|SendEmail|Send an email message|.NET 6|
|SendEmailUsingTemp|Send email using a template|.NET 6|
|ValidateAppointment|Validate appointment scheduling|.NET 6|

## Prerequisites

- Visual Studio 2022 or later
- .NET 6.0 SDK or later
- Access to a Dataverse environment

## How to run samples

1. Clone the PowerApps-Samples repository
2. Navigate to `dataverse/orgsvc/CSharp-NETCore/Activities/`
3. Open the desired sample folder
4. Edit the `appsettings.json` file (located in the Activities folder) with your environment connection details:

   ```json
   {
     "ConnectionStrings": {
       "default": "AuthType=OAuth;Url=https://yourorg.crm.dynamics.com;Username=youruser@yourdomain.com;AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;RedirectUri=http://localhost;LoginPrompt=Auto"
     }
   }
   ```

5. Build and run the sample:

   ```bash
   cd SampleFolder
   dotnet run
   ```

## See also

[Activity entities](https://learn.microsoft.com/power-apps/developer/data-platform/activity-entities)
[Email activity entities](https://learn.microsoft.com/power-apps/developer/data-platform/email-activity-entities)
[Task, fax, phone call, and letter activity entities](https://learn.microsoft.com/power-apps/developer/data-platform/task-fax-phone-call-letter-activity-entities)
[Appointment entities](https://learn.microsoft.com/power-apps/developer/data-platform/appointment-entities)
[Recurring appointment entities](https://learn.microsoft.com/power-apps/developer/data-platform/recurring-appointment-entities)
