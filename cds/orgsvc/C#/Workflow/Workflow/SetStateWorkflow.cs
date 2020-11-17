using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace PowerApps.Samples
{
    public class SetStateWorkflow
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

                    // Activate the workflow.
                    Console.WriteLine("\nActivating the 'CreateTask' workflow...");
                    var activateRequest = new SetStateRequest
                    {
                        EntityMoniker = new EntityReference
                            (Workflow.EntityLogicalName, _workflowId),
                        State = new OptionSetValue((int)WorkflowState.Activated),
                        Status = new OptionSetValue((int)workflow_statuscode.Activated)
                    };
                    service.Execute(activateRequest);

                    // Verify that the workflow is activated.
                    Workflow retrievedWorkflow =
                        (Workflow)service.Retrieve("workflow", _workflowId, new ColumnSet("statecode", "name"));

                    Console.WriteLine("The state of workflow {0} is: {1}.", retrievedWorkflow.Name, retrievedWorkflow.StateCode);

                    // Deactivate the workflow.
                    if (retrievedWorkflow.StateCode == WorkflowState.Activated)
                    {
                        Console.WriteLine("\nDeactivating the 'CreateTask' workflow...");
                        SetStateRequest deactivateRequest = new SetStateRequest
                        {
                            EntityMoniker =
                                new EntityReference(Workflow.EntityLogicalName, _workflowId),
                            State = new OptionSetValue((int)WorkflowState.Draft),
                            Status = new OptionSetValue((int)workflow_statuscode.Draft)
                        };
                        service.Execute(deactivateRequest);
                    }

                    // Verify that the workflow is deactivated (in a draft state).
                    retrievedWorkflow =
                        (Workflow)service.Retrieve("workflow", _workflowId, new ColumnSet("statecode", "name"));

                    Console.WriteLine("The state of workflow {0} is: {1}.", retrievedWorkflow.Name, retrievedWorkflow.StateCode);

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
            #region Define workflow XAML

            string xamlWF;

            // This xml defines a workflow that creates a new task when executed
            xamlWF = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Activity x:Class=""CreateTask""
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
  <this:CreateTask.InputEntities>
    <InArgument x:TypeArguments=""scg:IDictionary(x:String, mxs:Entity)"" />
  </this:CreateTask.InputEntities>
  <this:CreateTask.CreatedEntities>
    <InArgument x:TypeArguments=""scg:IDictionary(x:String, mxs:Entity)"" />
  </this:CreateTask.CreatedEntities>
  <mva:VisualBasic.Settings>Assembly references and imported namespaces for internal implementation</mva:VisualBasic.Settings>
  <mxswa:Workflow>
    <Sequence DisplayName=""CreateStep1: Create a task"">
      <Sequence.Variables>
        <Variable x:TypeArguments=""x:Object""
                  Name=""CreateStep1_1"" />
        <Variable x:TypeArguments=""x:Object""
                  Name=""CreateStep1_2"" />
        <Variable x:TypeArguments=""x:Object""
                  Name=""CreateStep1_3"" />
        <Variable x:TypeArguments=""x:Object""
                  Name=""CreateStep1_4"" />
      </Sequence.Variables>
      <Assign x:TypeArguments=""mxs:Entity""
              To=""[CreatedEntities(&quot;CreateStep1_localParameter#Temp&quot;)]""
              Value=""[New Entity(&quot;task&quot;)]"" />
      <mxswa:ActivityReference AssemblyQualifiedName=""Microsoft.Crm.Workflow.Activities.EvaluateExpression, Microsoft.Crm.Workflow, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35""
                               DisplayName=""EvaluateExpression"">
        <mxswa:ActivityReference.Arguments>
          <InArgument x:TypeArguments=""x:String""
                      x:Key=""ExpressionOperator"">CreateCrmType</InArgument>
          <InArgument x:TypeArguments=""s:Object[]""
                      x:Key=""Parameters"">[New Object() { Microsoft.Xrm.Sdk.Workflow.WorkflowPropertyType.String, ""New Task"", ""String"" }]</InArgument>
          <InArgument x:TypeArguments=""s:Type""
                      x:Key=""TargetType"">
            <mxswa:ReferenceLiteral x:TypeArguments=""s:Type""
                                    Value=""x:String"" />
          </InArgument>
          <OutArgument x:TypeArguments=""x:Object""
                       x:Key=""Result"">[CreateStep1_1]</OutArgument>
        </mxswa:ActivityReference.Arguments>
      </mxswa:ActivityReference>
      <mxswa:SetEntityProperty Attribute=""subject""
                               Entity=""[CreatedEntities(&quot;CreateStep1_localParameter#Temp&quot;)]""
                               EntityName=""task""
                               Value=""[CreateStep1_1]"">
        <mxswa:SetEntityProperty.TargetType>
          <InArgument x:TypeArguments=""s:Type"">
            <mxswa:ReferenceLiteral x:TypeArguments=""s:Type""
                                    Value=""x:String"" />
          </InArgument>
        </mxswa:SetEntityProperty.TargetType>
      </mxswa:SetEntityProperty>
      <mxswa:GetEntityProperty Attribute=""leadid""
                               Entity=""[InputEntities(&quot;primaryEntity&quot;)]""
                               EntityName=""lead""
                               Value=""[CreateStep1_3]"">
        <mxswa:GetEntityProperty.TargetType>
          <InArgument x:TypeArguments=""s:Type"">
            <mxswa:ReferenceLiteral x:TypeArguments=""s:Type""
                                    Value=""mxs:EntityReference"" />
          </InArgument>
        </mxswa:GetEntityProperty.TargetType>
      </mxswa:GetEntityProperty>
      <mxswa:ActivityReference AssemblyQualifiedName=""Microsoft.Crm.Workflow.Activities.EvaluateExpression, Microsoft.Crm.Workflow, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35""
                               DisplayName=""EvaluateExpression"">
        <mxswa:ActivityReference.Arguments>
          <InArgument x:TypeArguments=""x:String""
                      x:Key=""ExpressionOperator"">SelectFirstNonNull</InArgument>
          <InArgument x:TypeArguments=""s:Object[]""
                      x:Key=""Parameters"">[New Object() { CreateStep1_3 }]</InArgument>
          <InArgument x:TypeArguments=""s:Type""
                      x:Key=""TargetType"">
            <mxswa:ReferenceLiteral x:TypeArguments=""s:Type""
                                    Value=""mxs:EntityReference"" />
          </InArgument>
          <OutArgument x:TypeArguments=""x:Object""
                       x:Key=""Result"">[CreateStep1_2]</OutArgument>
        </mxswa:ActivityReference.Arguments>
      </mxswa:ActivityReference>
      <mxswa:SetEntityProperty Attribute=""regardingobjectid""
                               Entity=""[CreatedEntities(&quot;CreateStep1_localParameter#Temp&quot;)]""
                               EntityName=""task""
                               Value=""[CreateStep1_2]"">
        <mxswa:SetEntityProperty.TargetType>
          <InArgument x:TypeArguments=""s:Type"">
            <mxswa:ReferenceLiteral x:TypeArguments=""s:Type""
                                    Value=""mxs:EntityReference"" />
          </InArgument>
        </mxswa:SetEntityProperty.TargetType>
      </mxswa:SetEntityProperty>
      <mxswa:ActivityReference AssemblyQualifiedName=""Microsoft.Crm.Workflow.Activities.EvaluateExpression, Microsoft.Crm.Workflow, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35""
                               DisplayName=""EvaluateExpression"">
        <mxswa:ActivityReference.Arguments>
          <InArgument x:TypeArguments=""x:String""
                      x:Key=""ExpressionOperator"">CreateCrmType</InArgument>
          <InArgument x:TypeArguments=""s:Object[]""
                      x:Key=""Parameters"">[New Object() { Microsoft.Xrm.Sdk.Workflow.WorkflowPropertyType.OptionSetValue, ""1"", ""Picklist"" }]</InArgument>
          <InArgument x:TypeArguments=""s:Type""
                      x:Key=""TargetType"">
            <mxswa:ReferenceLiteral x:TypeArguments=""s:Type""
                                    Value=""mxs:OptionSetValue"" />
          </InArgument>
          <OutArgument x:TypeArguments=""x:Object""
                       x:Key=""Result"">[CreateStep1_4]</OutArgument>
        </mxswa:ActivityReference.Arguments>
      </mxswa:ActivityReference>
      <mxswa:SetEntityProperty Attribute=""prioritycode""
                               Entity=""[CreatedEntities(&quot;CreateStep1_localParameter#Temp&quot;)]""
                               EntityName=""task""
                               Value=""[CreateStep1_4]"">
        <mxswa:SetEntityProperty.TargetType>
          <InArgument x:TypeArguments=""s:Type"">
            <mxswa:ReferenceLiteral x:TypeArguments=""s:Type""
                                    Value=""mxs:OptionSetValue"" />
          </InArgument>
        </mxswa:SetEntityProperty.TargetType>
      </mxswa:SetEntityProperty>
      <mxswa:CreateEntity EntityId=""{x:Null}""
                          DisplayName=""CreateStep1: Create a task""
                          Entity=""[CreatedEntities(&quot;CreateStep1_localParameter#Temp&quot;)]""
                          EntityName=""task"" />
      <Assign x:TypeArguments=""mxs:Entity""
              To=""[CreatedEntities(&quot;CreateStep1_localParameter&quot;)]""
              Value=""[CreatedEntities(&quot;CreateStep1_localParameter#Temp&quot;)]"" />
      <Persist />
    </Sequence>
  </mxswa:Workflow>
</Activity>";

            #endregion Define workflow XAML

            // Create the workflow
            Workflow workflow = new Workflow()
            {
                Name = "CreateTask",
                Type = new OptionSetValue((int)WorkflowType.Definition),
                Category = new OptionSetValue((int)WorkflowCategory.Workflow),
                Scope = new OptionSetValue((int)WorkflowScope.User),
                LanguageCode = 1033,                // U.S. English
                TriggerOnCreate = true,
                OnDemand = true,
                PrimaryEntity = Lead.EntityLogicalName,
                Description =
                    "Test workflow for the SetStateWorkflow SDK sample",
                Xaml = xamlWF
            };
            _workflowId = service.Create(workflow);

            Console.WriteLine("Created workflow {0}.", workflow.Name);
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
                // Delete the workflow.
                service.Delete(Workflow.EntityLogicalName, _workflowId);
                Console.WriteLine("The workflow has been deleted.");
            }
        }
        #endregion Common methods
    }
}