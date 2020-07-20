using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
   public partial class SampleProgram
    {
        // Define the IDs needed for this sample.
        public static Guid _emailTemplateId;
        public static List<Guid> _templateAttachmentIds = new List<Guid>();
        private static bool prompt = true;
        /// <summary>
        /// Function to set up the sample.
        /// </summary>
        /// <param name="service">Specifies the service to connect to.</param>
        /// 
        private static void SetUpSample(CrmServiceClient service)
        {
            // Check that the current version is greater than the minimum version
            if (!SampleHelpers.CheckVersion(service, new Version("7.1.0.0")))
            {
                //The environment version is lower than version 7.1.0.0
                return;
            }

            CreateRequiredRecords(service);
        }
        private static void CleanUpSample(CrmServiceClient service)
        {
            DeleteRequiredRecords(service, prompt);
        }
        /// <summary>
        /// This method creates any entity records that this sample requires.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            //Define the email template to create.
            Template emailTemplate = new Template
            {
                Title = "An example email template",
                Subject = "This is an example email.",
                IsPersonal = false,
                TemplateTypeCode = "lead",

                //1033 is the code for US English - you may need to change this value
                //depending on your locale.
                LanguageCode = 1033

            };

            _emailTemplateId = service.Create(emailTemplate);

            for (int i = 0; i < 3; i++)
            {
                ActivityMimeAttachment attachment = new ActivityMimeAttachment
                {
                    Subject = String.Format("Attachment {0}", i),
                    FileName = String.Format("ExampleAttachment{0}.txt", i),
                    Body = "Some Text",
                    ObjectId = new EntityReference(Template.EntityLogicalName, _emailTemplateId),
                    ObjectTypeCode = Template.EntityLogicalName
                };

                _templateAttachmentIds.Add(service.Create(attachment));
            }

            Console.WriteLine("An email template and {0} attachments were created.", _templateAttachmentIds.Count);

            return;
        }

        /// <summary>
        /// Deletes any entity records that were created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user to delete the records created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service, bool prompt)
        {
            bool deleteRecords = true;

            if (prompt)
            {
                Console.WriteLine("\nDo you want these entity records deleted? (y/n)");
                String answer = Console.ReadLine();

                deleteRecords = (answer.StartsWith("y") || answer.StartsWith("Y"));
            }

            if (deleteRecords)
            {
                foreach (Guid attachmentId in _templateAttachmentIds)
                {
                    service.Delete(ActivityMimeAttachment.EntityLogicalName, attachmentId);
                }

               service.Delete(Template.EntityLogicalName, _emailTemplateId);

                Console.WriteLine("Entity records have been deleted.");
            }
        }

    }
}
