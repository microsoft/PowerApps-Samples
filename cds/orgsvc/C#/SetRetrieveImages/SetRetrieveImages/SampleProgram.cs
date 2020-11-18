using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PowerApps.Samples
{
  public partial  class SampleProgram
    {
        
       
        private static bool prompt = true; 
        [STAThread] // Added to support UX
        static void Main(string[] args)
        {

            CrmServiceClient service = null;

          
            try
            {
                service = SampleHelpers.Connect("Connect");
                if (service.IsReady)
                {
                    #region Sample Code
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate

                    /*This sample uses late-binding because the entity was just created and is
      not included in the 'MyOrganizationsCrmSdkTypes.cs' file created by the
      code generation tool(CrmSvcUtil.exe)

     */
                    //Use a 144x144 pixel image

                    Entity imageEntity1 = new Entity(_customEntityName.ToLower());
                    imageEntity1["sample_name"] = "144x144.png";
                    imageEntity1["entityimage"] = File.ReadAllBytes("Images\\144x144.png");
                    Guid imageEntity1Id = service.Create(imageEntity1);
                    

                    //Use a 144x400 pixel image
                    Entity imageEntity2 = new Entity(_customEntityName.ToLower());
                    imageEntity2["sample_name"] = "144x400.png";
                    imageEntity2["entityimage"] = File.ReadAllBytes("Images\\144x400.png");
                    Guid imageEntity2Id = service.Create(imageEntity2);
                    

                    //Use a 400x144 pixel image
                    Entity imageEntity3 = new Entity(_customEntityName.ToLower());
                    imageEntity3["sample_name"] = "400x144.png";
                    imageEntity3["entityimage"] = File.ReadAllBytes("Images\\400x144.png");
                    Guid imageEntity3Id = service.Create(imageEntity3);
                    

                    //Use a 400x500 pixel image
                    Entity imageEntity4 = new Entity(_customEntityName.ToLower());
                    imageEntity4["sample_name"] = "400x500.png";
                    imageEntity4["entityimage"] = File.ReadAllBytes("Images\\400x500.png");
                    Guid imageEntity4Id = service.Create(imageEntity4);
                    

                    //Use a 60x80 pixel image
                    Entity imageEntity5 = new Entity(_customEntityName.ToLower());
                    imageEntity5["sample_name"] = "60x80.png";
                    imageEntity5["entityimage"] = File.ReadAllBytes("Images\\60x80.png");
                    Guid imageEntity5Id = service.Create(imageEntity5);
                   
                    Console.WriteLine();
                    //Retrieve and download the binary images
                    string binaryImageQuery =
               String.Format(@"<fetch mapping='logical'>
  <entity name='{0}'>
    <attribute name='sample_name' />
    <attribute name='entityimage' />
  </entity>
</fetch>", _customEntityName.ToLower());

                    EntityCollection binaryImageResults = service.RetrieveMultiple(new FetchExpression(binaryImageQuery));


                    Console.WriteLine("Records retrieved and image files saved to: {0}", Directory.GetCurrentDirectory());
                    foreach (Entity record in binaryImageResults.Entities)
                    {
                        String recordName = record["sample_name"] as String;
                        String downloadedFileName = String.Format("Downloaded_{0}", recordName);
                        byte[] imageBytes = record["entityimage"] as byte[];
                        var fs = new BinaryWriter(new FileStream(downloadedFileName, FileMode.Append, FileAccess.Write));
                        fs.Write(imageBytes);
                        fs.Close();
                        Console.WriteLine(downloadedFileName);
                    }
                    Console.WriteLine();
                    //Retrieve and the records with just the url
                    string imageUrlQuery =
               String.Format(@"<fetch mapping='logical'>
  <entity name='{0}'>
    <attribute name='sample_name' />
    <attribute name='entityimage_url' />
  </entity>
</fetch>", _customEntityName.ToLower());

                    EntityCollection imageUrlResults = service.RetrieveMultiple(new FetchExpression(imageUrlQuery));


                    Console.WriteLine("These are the relative URLs for the images retrieved:");
                    foreach (Entity record in imageUrlResults.Entities)
                    {
                        String imageUrl = record["entityimage_url"] as String;
                        Console.WriteLine(imageUrl);
                    }


                    DeleteImageAttributeDemoEntity(service,prompt);
                }

                else
                {
                    const string UNABLE_TO_LOGIN_ERROR = "Unable to Login to Microsoft Dataverse";
                    if (service.LastCrmError.Equals(UNABLE_TO_LOGIN_ERROR))
                    {
                        Console.WriteLine("Check the connection string values in cds/App.config.");
                        throw new Exception(service.LastCrmError);
                    }
                    else
                    {
                        throw service.LastCrmException;
                    }
                }
                #endregion Demonstrate
                #endregion Sample Code
            }
            catch (Exception ex)
            {
                SampleHelpers.HandleException(ex);
            }

            finally
            {
                if (service != null)
                    service.Dispose();

                Console.WriteLine("Press <Enter> to exit.");
                Console.ReadLine();
            }
        }
    }
}
