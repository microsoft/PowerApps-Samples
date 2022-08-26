using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
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
                    //////////////////////////////////////////////
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate

                    // Create the count metric, setting the Metric Type to 'Count' by
                    // setting IsAmount to false.
                    Metric sampleMetric = new Metric()
                    {
                        Name = "Sample Count Metric",
                        IsAmount = false,
                    };
                    _metricId = service.Create(sampleMetric);
                    sampleMetric.Id = _metricId;

                    Console.Write("Created phone call metric, ");

                    #region Create RollupFields

                    // Create RollupField which targets completed (received) phone calls.
                    RollupField actual = new RollupField()
                    {
                        SourceEntity = PhoneCall.EntityLogicalName,
                        GoalAttribute = "actualinteger",
                        SourceState = 1,
                        SourceStatus = 4,
                        EntityForDateAttribute = PhoneCall.EntityLogicalName,
                        DateAttribute = "actualend",
                        MetricId = sampleMetric.ToEntityReference()
                    };
                    _actualId = service.Create(actual);

                    Console.Write("created actual revenue RollupField, ");

                    // Create RollupField which targets open (in-progress) phone calls.
                    RollupField inprogress = new RollupField()
                    {
                        SourceEntity = PhoneCall.EntityLogicalName,
                        GoalAttribute = "inprogressinteger",
                        SourceState = 0,
                        EntityForDateAttribute = PhoneCall.EntityLogicalName,
                        DateAttribute = "createdon",
                        MetricId = sampleMetric.ToEntityReference()
                    };
                    _inprogressId = service.Create(inprogress);

                    Console.Write("created in-progress revenue RollupField, ");

                    #endregion

                    #region Create the goal rollup queries

                    // Note: Formatting the FetchXml onto multiple lines in the following 
                    // rollup queries causes the length property to be greater than 1,000
                    // chars and will cause an exception.

                    // The following query locates closed incoming phone calls.
                    GoalRollupQuery goalRollupQuery = new GoalRollupQuery()
                    {
                        Name = "Example Goal Rollup Query - Actual",
                        QueryEntityType = PhoneCall.EntityLogicalName,
                        FetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'><entity name='phonecall'><attribute name='subject'/><attribute name='statecode'/><attribute name='prioritycode'/><attribute name='scheduledend'/><attribute name='createdby'/><attribute name='regardingobjectid'/><attribute name='activityid'/><order attribute='subject' descending='false'/><filter type='and'><condition attribute='directioncode' operator='eq' value='0'/><condition attribute='statecode' operator='eq' value='1' /></filter></entity></fetch>"
                    };
                    _rollupQueryIds.Add(service.Create(goalRollupQuery));
                    goalRollupQuery.Id = _rollupQueryIds[0];

                    // The following query locates open incoming phone calls.
                    GoalRollupQuery inProgressGoalRollupQuery = new GoalRollupQuery()
                    {
                        Name = "Example Goal Rollup Query - InProgress",
                        QueryEntityType = PhoneCall.EntityLogicalName,
                        FetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'><entity name='phonecall'><attribute name='subject'/><attribute name='statecode'/><attribute name='prioritycode'/><attribute name='scheduledend'/><attribute name='createdby'/><attribute name='regardingobjectid'/><attribute name='activityid'/><order attribute='subject' descending='false'/><filter type='and'><condition attribute='directioncode' operator='eq' value='0'/><condition attribute='statecode' operator='eq' value='0' /></filter></entity></fetch>"
                    };
                    _rollupQueryIds.Add(service.Create(inProgressGoalRollupQuery));
                    inProgressGoalRollupQuery.Id = _rollupQueryIds[1];

                    Console.Write("created rollup queries for incoming phone calls.\n");
                    Console.WriteLine();

                    #endregion

                    #region Create a goal to track the open incoming phone calls.

                    // Create the goal.
                    Goal goal = new Goal()
                    {
                        Title = "Sample Goal",
                        RollupOnlyFromChildGoals = false,
                        ConsiderOnlyGoalOwnersRecords = false,
                        TargetInteger = 5,
                        RollupQueryActualIntegerId = goalRollupQuery.ToEntityReference(),
                        RollUpQueryInprogressIntegerId =
                            inProgressGoalRollupQuery.ToEntityReference(),
                        IsFiscalPeriodGoal = false,
                        MetricId = sampleMetric.ToEntityReference(),
                        GoalOwnerId = new EntityReference
                        {
                            Id = _salesManagerId,
                            LogicalName = SystemUser.EntityLogicalName
                        },
                        OwnerId = new EntityReference
                        {
                            Id = _salesManagerId,
                            LogicalName = SystemUser.EntityLogicalName
                        },
                        GoalStartDate = DateTime.Today.AddDays(-1),
                        GoalEndDate = DateTime.Today.AddDays(30)
                    };
                    _goalId = service.Create(goal);
                    goal.Id = _goalId;

                    Console.WriteLine("Created goal");
                    Console.WriteLine("-------------------");
                    Console.WriteLine("Target: {0}", goal.TargetInteger.Value);
                    Console.WriteLine("Goal owner: {0}", goal.GoalOwnerId.Id);
                    Console.WriteLine("Goal Start Date: {0}", goal.GoalStartDate);
                    Console.WriteLine("Goal End Date: {0}", goal.GoalEndDate);
                    Console.WriteLine("<End of Listing>");
                    Console.WriteLine();

                    #endregion

                    #region Calculate rollup and display result

                    // Calculate roll-up of the goal.
                    RecalculateRequest recalculateRequest = new RecalculateRequest()
                    {
                        Target = goal.ToEntityReference()
                    };
                    service.Execute(recalculateRequest);

                    Console.WriteLine("Calculated roll-up of goal.");
                    Console.WriteLine();

                    // Retrieve and report 3 different computed values for the goal
                    // - Percentage
                    // - Actual (Integer)
                    // - In-Progress (Integer)
                    QueryExpression retrieveValues = new QueryExpression()
                    {
                        EntityName = Goal.EntityLogicalName,
                        ColumnSet = new ColumnSet(
                            "title",
                            "percentage",
                            "actualinteger",
                            "inprogressinteger")
                    };
                    EntityCollection ec = service.RetrieveMultiple(retrieveValues);

                    // Compute and display the results.
                    for (int i = 0; i < ec.Entities.Count; i++)
                    {
                        Goal temp = (Goal)ec.Entities[i];
                        Console.WriteLine("Roll-up details for goal: {0}", temp.Title);
                        Console.WriteLine("---------------");
                        Console.WriteLine("Percentage Achieved: {0}",
                            temp.Percentage);
                        Console.WriteLine("Actual (Integer): {0}",
                            temp.ActualInteger.Value);
                        Console.WriteLine("In-Progress (Integer): {0}",
                            temp.InProgressInteger.Value);
                        Console.WriteLine("<End of Listing>");
                    }

                    Console.WriteLine();

                    #endregion

                    #region Update goal to override the actual rollup value

                    // Override the actual and in-progress values of the goal.
                    // To prevent rollup values to be overwritten during next Recalculate operation, 
                    // set: goal.IsOverridden = true;

                    goal.IsOverride = true;
                    goal.ActualInteger = 10;
                    goal.InProgressInteger = 5;

                    // Update the goal.
                    UpdateRequest update = new UpdateRequest()
                    {
                        Target = goal
                    };
                    service.Execute(update);

                    Console.WriteLine("Goal actual and in-progress values overridden.");
                    Console.WriteLine();

                    #endregion

                    #region Retrieve result of manual override

                    // Retrieve and report 3 different computed values for the goal
                    // - Percentage
                    // - Actual (Integer)
                    // - In-Progress (Integer)
                    retrieveValues = new QueryExpression()
                    {
                        EntityName = Goal.EntityLogicalName,
                        ColumnSet = new ColumnSet(
                            "title",
                            "percentage",
                            "actualinteger",
                            "inprogressinteger")
                    };
                    ec = service.RetrieveMultiple(retrieveValues);

                    // Compute and display the results.
                    for (int i = 0; i < ec.Entities.Count; i++)
                    {
                        Goal temp = (Goal)ec.Entities[i];
                        Console.WriteLine("Roll-up details for goal: {0}", temp.Title);
                        Console.WriteLine("---------------");
                        Console.WriteLine("Percentage Achieved: {0}",
                            temp.Percentage);
                        Console.WriteLine("Actual (Integer): {0}",
                            temp.ActualInteger.Value);
                        Console.WriteLine("In-Progress (Integer): {0}",
                            temp.InProgressInteger.Value);
                        Console.WriteLine("<End of Listing>");
                    }

                    Console.WriteLine();

                    #endregion

                    #region Close the goal

                    // Close the goal.
                    SetStateRequest closeGoal = new SetStateRequest()
                    {
                        EntityMoniker = goal.ToEntityReference(),
                        State = new OptionSetValue(1),
                        Status = new OptionSetValue(1)
                    };

                    Console.WriteLine("Goal closed.");

                    #endregion
                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up
                }
                #endregion Demonstrate
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
            #endregion Sample Code

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
