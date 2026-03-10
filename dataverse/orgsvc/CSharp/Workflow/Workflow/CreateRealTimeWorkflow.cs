using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace PowerApps.Samples
{
    public class CreateRealTimeWorkflow
    {
        private static Guid _workflowId;

        [STAThread] // Required to support the interactive login experience
        static void Main(string[] args)
        {
            CrmServiceClient service = null;
            try
            {
                service = SampleHelpers.Connect("Connect");

                if (service.IsReady)
                {
                    // Create any entity records that the demonstration code requires
                    SetUpSample(service);

                    #region Demonstrate

                    #region Define the workflow in XAML

                    // Define the workflow using XAML. This is typically done in the Visual Studio
                    // workflow designer.
                    string xamlWF = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Activity x:Class=""SampleWF""
          xmlns=""http://schemas.microsoft.com/netfx/2009/xaml/activities""
          xmlns:mva=""clr-namespace:Microsoft.VisualBasic.Activities;assembly=System.Activities, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35""
          xmlns:mxs=""clr-namespace:Microsoft.Xrm.Sdk;assembly=Microsoft.Xrm.Sdk, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35""
          xmlns:mxswa=""clr-namespace:Microsoft.Xrm.Sdk.Workflow.Activities;assembly=Microsoft.Xrm.Sdk.Workflow, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35""
          xmlns:s=""clr-namespace:System;assembly=mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089""
          xmlns:scg=""clr-namespace:System.Collections.Generic;assembly=mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089""
          xmlns:srs=""clr-namespace:System.Runtime.Serialization;assembly=System.Runtime.Serialization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089""
          xmlns:this=""clr-namespace:""
          xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <x:Members>
    <x:Property Name=""InputEntities""
                Type=""InArgument(scg:IDictionary(x:String, mxs:Entity))"" />
    <x:Property Name=""CreatedEntities""
                Type=""InArgument(scg:IDictionary(x:String, mxs:Entity))"" />
  </x:Members>
  <this:SampleWF.InputEntities>
    <InArgument x:TypeArguments=""scg:IDictionary(x:String, mxs:Entity)"" />
  </this:SampleWF.InputEntities>
  <this:SampleWF.CreatedEntities>
    <InArgument x:TypeArguments=""scg:IDictionary(x:String, mxs:Entity)"" />
  </this:SampleWF.CreatedEntities>
  <mva:VisualBasic.Settings>Assembly references and imported namespaces for internal implementation</mva:VisualBasic.Settings>
  <mxswa:Workflow>
    <Sequence DisplayName=""UpdateStep1"">
      <Sequence.Variables>
        <Variable x:TypeArguments=""x:Object""
                  Name=""UpdateStep1_1"" />
      </Sequence.Variables>
      <Assign x:TypeArguments=""mxs:Entity""
              To=""[CreatedEntities(&quot;primaryEntity#Temp&quot;)]""
              Value=""[New Entity(&quot;opportunity&quot;)]"" />
      <Assign x:TypeArguments=""s:Guid""
              To=""[CreatedEntities(&quot;primaryEntity#Temp&quot;).Id]""
              Value=""[InputEntities(&quot;primaryEntity&quot;).Id]"" />
      <mxswa:ActivityReference AssemblyQualifiedName=""Microsoft.Crm.Workflow.Activities.EvaluateExpression, Microsoft.Crm.Workflow, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35""
                               DisplayName=""EvaluateExpression"">
        <mxswa:ActivityReference.Arguments>
          <InArgument x:TypeArguments=""x:String""
                      x:Key=""ExpressionOperator"">CreateCrmType</InArgument>
          <InArgument x:TypeArguments=""s:Object[]""
                      x:Key=""Parameters"">[New Object() { Microsoft.Xrm.Sdk.Workflow.WorkflowPropertyType.Integer, ""40"" }]</InArgument>
          <InArgument x:TypeArguments=""s:Type""
                      x:Key=""TargetType"">
            <mxswa:ReferenceLiteral x:TypeArguments=""s:Type""
                                    Value=""x:Int32"" />
          </InArgument>
          <OutArgument x:TypeArguments=""x:Object""
                       x:Key=""Result"">[UpdateStep1_1]</OutArgument>
        </mxswa:ActivityReference.Arguments>
      </mxswa:ActivityReference>
      <mxswa:SetEntityProperty Attribute=""closeprobability""
                               Entity=""[CreatedEntities(&quot;primaryEntity#Temp&quot;)]""
                               EntityName=""opportunity""
                               Value=""[UpdateStep1_1]"">
        <mxswa:SetEntityProperty.TargetType>
          <InArgument x:TypeArguments=""s:Type"">
            <mxswa:ReferenceLiteral x:TypeArguments=""s:Type""
                                    Value=""x:Int32"" />
          </InArgument>
        </mxswa:SetEntityProperty.TargetType>
      </mxswa:SetEntityProperty>
      <mxswa:UpdateEntity DisplayName=""UpdateStep1""
                          Entity=""[CreatedEntities(&quot;primaryEntity#Temp&quot;)]""
                          EntityName=""opportunity"" />
      <Assign x:TypeArguments=""mxs:Entity""
              To=""[InputEntities(&quot;primaryEntity&quot;)]""
              Value=""[CreatedEntities(&quot;primaryEntity#Temp&quot;)]"" />
      <Persist />
    </Sequence>
  </mxswa:Workflow>
</Activity>";

                    #endregion Define the workflow in XAML

                    #region Create the workflow

                    // Create a real-time workflow. 
                    // The workflow should execute after a new opportunity is created
                    // and run in the context of the logged on user.
                    Workflow workflow = new Workflow()
                    {
                        // These properties map to the New Process form settings in the web application.
                        Name = "Set closeprobability on opportunity create (real-time)",
                        Type = new OptionSetValue((int)WorkflowType.Definition),
                        Category = new OptionSetValue((int)WorkflowCategory.Workflow),
                        PrimaryEntity = Opportunity.EntityLogicalName,
                        Mode = new OptionSetValue((int)WorkflowMode.Realtime),

                        // Additional settings from the second New Process form.
                        Description = @"When an opportunity is created, this workflow" +
                            " sets the closeprobability field of the opportunity record to 40%.",
                        OnDemand = false,
                        Subprocess = false,
                        Scope = new OptionSetValue((int)WorkflowScope.User),
                        RunAs = new OptionSetValue((int)workflow_runas.CallingUser),
                        SyncWorkflowLogOnFailure = true,
                        TriggerOnCreate = true,
                        CreateStage = new OptionSetValue((int)workflow_stage.Postoperation),
                        Xaml = xamlWF,

                        // Other properties not in the web forms.
                        LanguageCode = 1033,  // U.S. English
                    };
                    _workflowId = service.Create(workflow);

                    Console.WriteLine("Created process '" + workflow.Name + "'");

                    #endregion Create the workflow

                    #region Activate the workflow

                    // Activate the workflow. Initially, the workflow is created in
                    // a Draft state and must be activated before it can run.
                    var activateRequest = new SetStateRequest
                    {
                        EntityMoniker = new EntityReference
                            (Workflow.EntityLogicalName, _workflowId),
                        State = new OptionSetValue((int)WorkflowState.Activated),
                        Status = new OptionSetValue((int)workflow_statuscode.Activated)
                    };
                    service.Execute(activateRequest);
                    Console.WriteLine("Activated process '" + workflow.Name + "'");

                    #endregion Activate the workflow

                    #endregion Demonstrate

                    CleanUpSample(service);
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

        #region Common methods
        /// <summary>
        /// Function to set up the sample.
        /// </summary>
        /// <param name="service">Specifies the service to connect to.</param>
        /// 
        private static void SetUpSample(CrmServiceClient service)
        {
            // Check that the current version is greater than the minimum version
            if (!SampleHelpers.CheckVersion(service, new Version("9.0.0.0")))
            {
                //The environment version is lower than version 9.0.0.0
                return;
            }

            CreateRequiredRecords(service);
        }

        private static void CleanUpSample(CrmServiceClient service)
        {
            DeleteRequiredRecords(service, true);
        }

        /// <summary>
        /// This method creates any entity records that this sample requires.
        /// Creates the email activity.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            // Nothing to create for this sample.
        }

        /// <summary>
        /// Deletes the custom entity record that was created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user 
        /// to delete the entity created in this sample.</param>
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
                // Deactivate, and then delete the workflow.
                SetStateRequest deactivateRequest = new SetStateRequest
                {
                    EntityMoniker = new EntityReference(Workflow.EntityLogicalName, _workflowId),
                    State = new OptionSetValue((int)WorkflowState.Draft),
                    Status = new OptionSetValue((int)workflow_statuscode.Draft)
                };
                service.Execute(deactivateRequest);
                service.Delete(Workflow.EntityLogicalName, _workflowId);
                Console.WriteLine("The workflow has been deactivated, and deleted.");
            }
        }
        #endregion Common methods
    }
}