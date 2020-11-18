using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Linq;

namespace PowerApps.Samples
{
    public class ExecuteWorkflow
    {
        private static Guid _workflowId;
        private static Guid _leadId;
        private static Guid _asyncOperationId;

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

                    #region Send request

                    // Create an ExecuteWorkflow request.
                    ExecuteWorkflowRequest request = new ExecuteWorkflowRequest()
                    {
                        WorkflowId = _workflowId,
                        EntityId = _leadId
                    };
                    Console.Write("Created an ExecuteWorkflow request, ");

                    // Execute the workflow.
                    ExecuteWorkflowResponse response =
                        (ExecuteWorkflowResponse)service.Execute(request);
                    Console.WriteLine("and sent the request to service.");

                    #endregion Send request

                    #region Check success

                    ColumnSet cols = new ColumnSet("statecode");
                    QueryByAttribute retrieveOpQuery = new QueryByAttribute();
                    retrieveOpQuery.EntityName = AsyncOperation.EntityLogicalName;
                    retrieveOpQuery.ColumnSet = cols;
                    retrieveOpQuery.AddAttributeValue("asyncoperationid", response.Id);

                    // Wait for the asyncoperation to complete.
                    // (wait no longer than 1 minute)
                    for (int i = 0; i < 60; i++)
                    {
                        System.Threading.Thread.Sleep(1000);

                        EntityCollection retrieveOpResults =
                            service.RetrieveMultiple(retrieveOpQuery);

                        if (retrieveOpResults.Entities.Count() > 0)
                        {
                            AsyncOperation op =
                                (AsyncOperation)retrieveOpResults.Entities[0];
                            if (op.StateCode == AsyncOperationState.Completed)
                            {
                                _asyncOperationId = op.AsyncOperationId.Value;
                                Console.WriteLine("AsyncOperation completed successfully.");
                                break;
                            }
                        }

                        if (i == 59)
                        {
                            throw new TimeoutException("AsyncOperation failed to complete in under one minute.");
                        }
                    }

                    // Retrieve the task that was created by the workflow.
                    cols = new ColumnSet("activityid");
                    QueryByAttribute retrieveActivityQuery = new QueryByAttribute();
                    retrieveActivityQuery.EntityName = PhoneCall.EntityLogicalName;
                    retrieveActivityQuery.ColumnSet = cols;
                    retrieveActivityQuery.AddAttributeValue("subject", "First call to Diogo Andrade");

                    EntityCollection results =
                        service.RetrieveMultiple(retrieveActivityQuery);

                    if (results.Entities.Count() == 0)
                    {
                        throw new InvalidOperationException("Phone call activity was not successfully created.");
                    }
                    else
                    {
                        Console.WriteLine("Phone call activity '{0}' successfully created from the workflow.",
                           "First call to Diogo Andrade");
                    }

                    #endregion Check success

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
            // Create a Lead record on which we will execute the Workflow.
            Lead lead = new Lead()
            {
                FirstName = "Diogo",
                LastName = "Andrade"
            };
            _leadId = service.Create(lead);
            Console.WriteLine("Created a lead named '{0} {1}' for the workflow request.", lead.FirstName,
                lead.LastName);

            // Define an anonymous type to define the possible values for
            // process type
            var ProcessType = new
            {
                Definition = 1, 
                Activation = 2,
                Template = 3
            };

            // Define an anonymous type to define the possible values for
            // process category
            var ProcessCategory = new
            {
                Workflow = 0,
                Dialog = 1,
            };

            // Define an anonymous type to define the possible values for
            // process scope
            var ProcessScope = new
            {
                User = 1,
                BusinessUnit = 2,
                Deep = 3,
                Global = 4
            };

            // Create the Workflow that we will execute.
            Workflow workflow = new Workflow()
            {
                Name = "Sample Workflow", // friendly name of the record
                Type = new OptionSetValue(ProcessType.Definition),
                Category = new OptionSetValue(ProcessCategory.Workflow),
                Scope = new OptionSetValue(ProcessScope.User),
                OnDemand = true,
                PrimaryEntity = Lead.EntityLogicalName,
                Xaml =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Activity x:Class=""ExecuteWorkflowSample"" xmlns=""http://schemas.microsoft.com/netfx/2009/xaml/activities"" xmlns:mva=""clr-namespace:Microsoft.VisualBasic.Activities;assembly=System.Activities, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"" xmlns:mxs=""clr-namespace:Microsoft.Xrm.Sdk;assembly=Microsoft.Xrm.Sdk, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"" xmlns:mxswa=""clr-namespace:Microsoft.Xrm.Sdk.Workflow.Activities;assembly=Microsoft.Xrm.Sdk.Workflow, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"" xmlns:s=""clr-namespace:System;assembly=mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"" xmlns:scg=""clr-namespace:System.Collections.Generic;assembly=mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"" xmlns:srs=""clr-namespace:System.Runtime.Serialization;assembly=System.Runtime.Serialization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"" xmlns:this=""clr-namespace:"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <x:Members>
    <x:Property Name=""InputEntities"" Type=""InArgument(scg:IDictionary(x:String, mxs:Entity))"" />
    <x:Property Name=""CreatedEntities"" Type=""InArgument(scg:IDictionary(x:String, mxs:Entity))"" />
  </x:Members>
  <this:ExecuteWorkflowSample.InputEntities>
    <InArgument x:TypeArguments=""scg:IDictionary(x:String, mxs:Entity)"" />
  </this:ExecuteWorkflowSample.InputEntities>
  <this:ExecuteWorkflowSample.CreatedEntities>
    <InArgument x:TypeArguments=""scg:IDictionary(x:String, mxs:Entity)"" />
  </this:ExecuteWorkflowSample.CreatedEntities>
  <mva:VisualBasic.Settings>Assembly references and imported namespaces for internal implementation</mva:VisualBasic.Settings>
  <mxswa:Workflow>
    <Assign x:TypeArguments=""mxs:Entity"" To=""[CreatedEntities(&quot;CreateStep1_localParameter#Temp&quot;)]"" Value=""[New Entity(&quot;phonecall&quot;)]"" />
    <Sequence DisplayName=""CreateStep1: Set first activity for lead."">
      <Sequence.Variables>
        <Variable x:TypeArguments=""x:Object"" Name=""CreateStep1_1"" />
        <Variable x:TypeArguments=""x:Object"" Name=""CreateStep1_2"" />
        <Variable x:TypeArguments=""x:Object"" Name=""CreateStep1_3"" />
        <Variable x:TypeArguments=""x:Object"" Name=""CreateStep1_4"" />
        <Variable x:TypeArguments=""x:Object"" Name=""CreateStep1_5"" />
        <Variable x:TypeArguments=""x:Object"" Name=""CreateStep1_6"" />
        <Variable x:TypeArguments=""x:Object"" Name=""CreateStep1_7"" />
        <Variable x:TypeArguments=""x:Object"" Name=""CreateStep1_8"" />
      </Sequence.Variables>
      <mxswa:ActivityReference AssemblyQualifiedName=""Microsoft.Crm.Workflow.Activities.EvaluateExpression, Microsoft.Crm.Workflow, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"" DisplayName=""EvaluateExpression"">
        <mxswa:ActivityReference.Arguments>
          <InArgument x:TypeArguments=""x:String"" x:Key=""ExpressionOperator"">CreateCrmType</InArgument>
          <InArgument x:TypeArguments=""s:Object[]"" x:Key=""Parameters"">[New Object() { Microsoft.Xrm.Sdk.Workflow.WorkflowPropertyType.Boolean, ""True"" }]</InArgument>
          <InArgument x:TypeArguments=""s:Type"" x:Key=""TargetType"">
            <mxswa:ReferenceLiteral x:TypeArguments=""s:Type"" Value=""x:Boolean"" />
          </InArgument>
          <OutArgument x:TypeArguments=""x:Object"" x:Key=""Result"">[CreateStep1_1]</OutArgument>
        </mxswa:ActivityReference.Arguments>
      </mxswa:ActivityReference>
      <mxswa:SetEntityProperty Attribute=""directioncode"" Entity=""[CreatedEntities(&quot;CreateStep1_localParameter#Temp&quot;)]"" EntityName=""phonecall"" Value=""[CreateStep1_1]"">
        <mxswa:SetEntityProperty.TargetType>
          <InArgument x:TypeArguments=""s:Type"">
            <mxswa:ReferenceLiteral x:TypeArguments=""s:Type"" Value=""x:Boolean"" />
          </InArgument>
        </mxswa:SetEntityProperty.TargetType>
      </mxswa:SetEntityProperty>
      <mxswa:ActivityReference AssemblyQualifiedName=""Microsoft.Crm.Workflow.Activities.EvaluateExpression, Microsoft.Crm.Workflow, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"" DisplayName=""EvaluateExpression"">
        <mxswa:ActivityReference.Arguments>
          <InArgument x:TypeArguments=""x:String"" x:Key=""ExpressionOperator"">CreateCrmType</InArgument>
          <InArgument x:TypeArguments=""s:Object[]"" x:Key=""Parameters"">[New Object() { Microsoft.Xrm.Sdk.Workflow.WorkflowPropertyType.String, ""First call to "", ""String"" }]</InArgument>
          <InArgument x:TypeArguments=""s:Type"" x:Key=""TargetType"">
            <mxswa:ReferenceLiteral x:TypeArguments=""s:Type"" Value=""x:String"" />
          </InArgument>
          <OutArgument x:TypeArguments=""x:Object"" x:Key=""Result"">[CreateStep1_3]</OutArgument>
        </mxswa:ActivityReference.Arguments>
      </mxswa:ActivityReference>
      <mxswa:GetEntityProperty Attribute=""fullname"" Entity=""[InputEntities(&quot;primaryEntity&quot;)]"" EntityName=""lead"" Value=""[CreateStep1_5]"">
        <mxswa:GetEntityProperty.TargetType>
          <InArgument x:TypeArguments=""s:Type"">
            <mxswa:ReferenceLiteral x:TypeArguments=""s:Type"" Value=""x:String"" />
          </InArgument>
        </mxswa:GetEntityProperty.TargetType>
      </mxswa:GetEntityProperty>
      <mxswa:ActivityReference AssemblyQualifiedName=""Microsoft.Crm.Workflow.Activities.EvaluateExpression, Microsoft.Crm.Workflow, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"" DisplayName=""EvaluateExpression"">
        <mxswa:ActivityReference.Arguments>
          <InArgument x:TypeArguments=""x:String"" x:Key=""ExpressionOperator"">SelectFirstNonNull</InArgument>
          <InArgument x:TypeArguments=""s:Object[]"" x:Key=""Parameters"">[New Object() { CreateStep1_5 }]</InArgument>
          <InArgument x:TypeArguments=""s:Type"" x:Key=""TargetType"">
            <mxswa:ReferenceLiteral x:TypeArguments=""s:Type"" Value=""x:String"" />
          </InArgument>
          <OutArgument x:TypeArguments=""x:Object"" x:Key=""Result"">[CreateStep1_4]</OutArgument>
        </mxswa:ActivityReference.Arguments>
      </mxswa:ActivityReference>
      <mxswa:ActivityReference AssemblyQualifiedName=""Microsoft.Crm.Workflow.Activities.EvaluateExpression, Microsoft.Crm.Workflow, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"" DisplayName=""EvaluateExpression"">
        <mxswa:ActivityReference.Arguments>
          <InArgument x:TypeArguments=""x:String"" x:Key=""ExpressionOperator"">Add</InArgument>
          <InArgument x:TypeArguments=""s:Object[]"" x:Key=""Parameters"">[New Object() { CreateStep1_3, CreateStep1_4 }]</InArgument>
          <InArgument x:TypeArguments=""s:Type"" x:Key=""TargetType"">
            <mxswa:ReferenceLiteral x:TypeArguments=""s:Type"" Value=""x:String"" />
          </InArgument>
          <OutArgument x:TypeArguments=""x:Object"" x:Key=""Result"">[CreateStep1_2]</OutArgument>
        </mxswa:ActivityReference.Arguments>
      </mxswa:ActivityReference>
      <mxswa:SetEntityProperty Attribute=""subject"" Entity=""[CreatedEntities(&quot;CreateStep1_localParameter#Temp&quot;)]"" EntityName=""phonecall"" Value=""[CreateStep1_2]"">
        <mxswa:SetEntityProperty.TargetType>
          <InArgument x:TypeArguments=""s:Type"">
            <mxswa:ReferenceLiteral x:TypeArguments=""s:Type"" Value=""x:String"" />
          </InArgument>
        </mxswa:SetEntityProperty.TargetType>
      </mxswa:SetEntityProperty>
      <mxswa:GetEntityProperty Attribute=""leadid"" Entity=""[InputEntities(&quot;primaryEntity&quot;)]"" EntityName=""lead"" Value=""[CreateStep1_7]"">
        <mxswa:GetEntityProperty.TargetType>
          <InArgument x:TypeArguments=""s:Type"">
            <mxswa:ReferenceLiteral x:TypeArguments=""s:Type"" Value=""mxs:EntityReference"" />
          </InArgument>
        </mxswa:GetEntityProperty.TargetType>
      </mxswa:GetEntityProperty>
      <mxswa:ActivityReference AssemblyQualifiedName=""Microsoft.Crm.Workflow.Activities.EvaluateExpression, Microsoft.Crm.Workflow, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"" DisplayName=""EvaluateExpression"">
        <mxswa:ActivityReference.Arguments>
          <InArgument x:TypeArguments=""x:String"" x:Key=""ExpressionOperator"">SelectFirstNonNull</InArgument>
          <InArgument x:TypeArguments=""s:Object[]"" x:Key=""Parameters"">[New Object() { CreateStep1_7 }]</InArgument>
          <InArgument x:TypeArguments=""s:Type"" x:Key=""TargetType"">
            <mxswa:ReferenceLiteral x:TypeArguments=""s:Type"" Value=""mxs:EntityReference"" />
          </InArgument>
          <OutArgument x:TypeArguments=""x:Object"" x:Key=""Result"">[CreateStep1_6]</OutArgument>
        </mxswa:ActivityReference.Arguments>
      </mxswa:ActivityReference>
      <mxswa:SetEntityProperty Attribute=""regardingobjectid"" Entity=""[CreatedEntities(&quot;CreateStep1_localParameter#Temp&quot;)]"" EntityName=""phonecall"" Value=""[CreateStep1_6]"">
        <mxswa:SetEntityProperty.TargetType>
          <InArgument x:TypeArguments=""s:Type"">
            <mxswa:ReferenceLiteral x:TypeArguments=""s:Type"" Value=""mxs:EntityReference"" />
          </InArgument>
        </mxswa:SetEntityProperty.TargetType>
      </mxswa:SetEntityProperty>
      <mxswa:ActivityReference AssemblyQualifiedName=""Microsoft.Crm.Workflow.Activities.EvaluateExpression, Microsoft.Crm.Workflow, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"" DisplayName=""EvaluateExpression"">
        <mxswa:ActivityReference.Arguments>
          <InArgument x:TypeArguments=""x:String"" x:Key=""ExpressionOperator"">CreateCrmType</InArgument>
          <InArgument x:TypeArguments=""s:Object[]"" x:Key=""Parameters"">[New Object() { Microsoft.Xrm.Sdk.Workflow.WorkflowPropertyType.OptionSetValue, ""1"", ""Picklist"" }]</InArgument>
          <InArgument x:TypeArguments=""s:Type"" x:Key=""TargetType"">
            <mxswa:ReferenceLiteral x:TypeArguments=""s:Type"" Value=""mxs:OptionSetValue"" />
          </InArgument>
          <OutArgument x:TypeArguments=""x:Object"" x:Key=""Result"">[CreateStep1_8]</OutArgument>
        </mxswa:ActivityReference.Arguments>
      </mxswa:ActivityReference>
      <mxswa:SetEntityProperty Attribute=""prioritycode"" Entity=""[CreatedEntities(&quot;CreateStep1_localParameter#Temp&quot;)]"" EntityName=""phonecall"" Value=""[CreateStep1_8]"">
        <mxswa:SetEntityProperty.TargetType>
          <InArgument x:TypeArguments=""s:Type"">
            <mxswa:ReferenceLiteral x:TypeArguments=""s:Type"" Value=""mxs:OptionSetValue"" />
          </InArgument>
        </mxswa:SetEntityProperty.TargetType>
      </mxswa:SetEntityProperty>
      <mxswa:CreateEntity EntityId=""{x:Null}"" DisplayName=""CreateStep1: Set first activity for lead."" Entity=""[CreatedEntities(&quot;CreateStep1_localParameter#Temp&quot;)]"" EntityName=""phonecall"" />
      <Assign x:TypeArguments=""mxs:Entity"" To=""[CreatedEntities(&quot;CreateStep1_localParameter&quot;)]"" Value=""[CreatedEntities(&quot;CreateStep1_localParameter#Temp&quot;)]"" />
      <Persist />
    </Sequence>
  </mxswa:Workflow>
</Activity>"
            };
            _workflowId = service.Create(workflow);
            Console.Write("Created '{0}' to call in the ExecuteWorkflow request, ", workflow.Name);

            SetStateRequest setStateRequest = new SetStateRequest()
            {
                EntityMoniker =
                    new EntityReference(Workflow.EntityLogicalName, _workflowId),
                State = new OptionSetValue((int)WorkflowState.Activated),
                Status = new OptionSetValue(2)
            };
            service.Execute(setStateRequest);
            Console.WriteLine("and then activated it.");
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
                SetStateRequest setStateRequest = new SetStateRequest()
                {
                    EntityMoniker =
                       new EntityReference(Workflow.EntityLogicalName, _workflowId),
                    State = new OptionSetValue((int)WorkflowState.Draft),
                    Status = new OptionSetValue(1)
                };
                service.Execute(setStateRequest);

                service.Delete(Workflow.EntityLogicalName, _workflowId);
                service.Delete(Lead.EntityLogicalName, _leadId);
                service.Delete(AsyncOperation.EntityLogicalName, _asyncOperationId);
                Console.WriteLine("Entity records have been deleted.");
            }
        }
        #endregion Common methods
    }
}