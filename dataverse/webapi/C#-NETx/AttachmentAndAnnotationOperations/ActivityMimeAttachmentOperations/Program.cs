using Newtonsoft.Json.Linq;
using PowerApps.Samples.Methods;
using System.IO;
using System.Net.Mail;

namespace PowerApps.Samples
{
    internal class Program
    {
        static async Task Main()
        {
            // Files used in this sample
            FileInfo wordDoc = new("Files/WordDoc.docx");
            FileInfo excelDoc = new("Files/ExcelDoc.xlsx");
            FileInfo pdfDoc = new("Files/25mb.pdf");

            Config config = App.InitializeApp();

            var service = new Service(config);

            // Get current MaxUploadFileSize
            int originalMaxUploadFileSize = await Utility.GetMaxUploadFileSize(service);
            Console.WriteLine($"Current MaxUploadFileSize: {originalMaxUploadFileSize}");

            // Create email activity
            JObject email = new() {
                {"subject", "This is an example email." }
            };

            EntityReference emailRef = await service.Create(
                entitySetName: "emails", 
                record: email);

            Console.WriteLine("Created an email activity.");

            List<FileInfo> smallFiles = new() { wordDoc, excelDoc };

            smallFiles.ForEach(async smallFile => {
                JObject attachment = new() {
                        { "objectid", emailRef.Id},
                        { "objecttypecode", "email" },
                        { "subject", $"Sample attached {smallFile.Name}" },
                        { "body", Convert.ToBase64String(File.ReadAllBytes(smallFile.FullName)) },
                        { "filename", smallFile.Name},
                        { "mimetype", Utility.GetMimeType(smallFile)}
                };

                await service.Create(
                    entitySetName: "activitymimeattachments", 
                    record: attachment);
            });

            Console.WriteLine("Created two e-mail attachments for the e-mail activity.");

            // Set MaxUploadFileSize to the maximum value
            await Utility.SetMaxUploadFileSize(service, 131072000);

            Console.WriteLine($"Updated MaxUploadFileSize to: {await Utility.GetMaxUploadFileSize(service)}");

            JObject largeAttachment = new() {
                        { "objectid", emailRef.Id},
                        { "objecttypecode", "email" },
                        { "subject", $"Sample attached {pdfDoc.Name}" },
                        { "filename", pdfDoc.Name},
                        { "mimetype", Utility.GetMimeType(pdfDoc)}
            };

            // Creates the activitymimeattachment record with a file, but doesn't return id.
            int fileSizeInBytes = await UploadAttachment(
                service: service,
                attachment: largeAttachment,
                fileInfo: pdfDoc);

            Console.WriteLine($"Uploaded {pdfDoc.Name} as attachment.");
        }

        static async Task<int> UploadAttachment(
        Service service,
        JObject attachment,
        FileInfo fileInfo)
        {
            return 10;
        }
    }
}