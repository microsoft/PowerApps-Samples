using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using PowerApps.Samples;
using System;

namespace GoalDataForFiscalYear
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

                    #region Create goal metric

                    // Create the metric, setting the Metric Type to 'Count' and enabling
                    // stretch tracking.
                    Metric metric = new Metric()
                    {
                        Name = "Sample Count Metric",
                        IsAmount = false,
                        IsStretchTracked = true
                    };
                    _metricId = service.Create(metric);
                    metric.Id = _metricId;

                    Console.Write("Created count metric, ");

                    #endregion

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
                        MetricId = metric.ToEntityReference()
                    };
                    _actualId = service.Create(actual);

                    Console.Write("created completed phone call RollupField, ");

                    #endregion

                    #region Create the goal rollup queries

                    // Note: Formatting the FetchXml onto multiple lines in the following 
                    // rollup queries causes the length property to be greater than 1,000
                    // chars and will cause an exception.

                    // The following query locates closed incoming phone calls.
                    GoalRollupQuery goalRollupQuery = new GoalRollupQuery()
                    {
                        Name = "Example Goal Rollup Query",
                        QueryEntityType = PhoneCall.EntityLogicalName,
                        FetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'><entity name='phonecall'><attribute name='subject'/><attribute name='statecode'/><attribute name='prioritycode'/><attribute name='scheduledend'/><attribute name='createdby'/><attribute name='regardingobjectid'/><attribute name='activityid'/><order attribute='subject' descending='false'/><filter type='and'><condition attribute='directioncode' operator='eq' value='0'/><condition attribute='statecode' operator='eq' value='1' /></filter></entity></fetch>"
                    };
                    _rollupQueryIds.Add(service.Create(goalRollupQuery));
                    goalRollupQuery.Id = _rollupQueryIds[0];

                    // The following query locates closed outgoing phone calls.
                    GoalRollupQuery goalRollupQuery2 = new GoalRollupQuery()
                    {
                        Name = "Example Goal Rollup Query",
                        QueryEntityType = PhoneCall.EntityLogicalName,
                        FetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'><entity name='phonecall'><attribute name='subject'/><attribute name='statecode'/><attribute name='prioritycode'/><attribute name='scheduledend'/><attribute name='createdby'/><attribute name='regardingobjectid'/><attribute name='activityid'/><order attribute='subject' descending='false'/><filter type='and'><condition attribute='directioncode' operator='eq' value='1'/><condition attribute='statecode' operator='eq' value='1' /></filter></entity></fetch>"
                    };
                    _rollupQueryIds.Add(service.Create(goalRollupQuery2));
                    goalRollupQuery2.Id = _rollupQueryIds[1];

                    Console.Write("created rollup queries for phone calls.\n");
                    Console.WriteLine();

                    #endregion

                    #region Create goals

                    // Determine current fiscal period and year.
                    // Note: This sample assumes quarterly fiscal periods.
                    DateTime date = DateTime.Now;
                    int quarterNumber = (date.Month - 1) / 3 + 1;
                    int yearNumber = date.Year;

                    // Create three goals: one parent goal and two child goals.
                    Goal parentGoal = new Goal()
                    {
                        Title = "Parent Goal Example",
                        RollupOnlyFromChildGoals = true,
                        ConsiderOnlyGoalOwnersRecords = true,
                        TargetInteger = 8,
                        StretchTargetInteger = 10,
                        IsFiscalPeriodGoal = true,
                        FiscalPeriod = new OptionSetValue(quarterNumber),
                        FiscalYear = new OptionSetValue(yearNumber),
                        MetricId = metric.ToEntityReference(),
                        GoalOwnerId = new EntityReference
                        {
                            Id = _salesManagerId,
                            LogicalName = SystemUser.EntityLogicalName
                        },
                        OwnerId = new EntityReference
                        {
                            Id = _salesManagerId,
                            LogicalName = SystemUser.EntityLogicalName
                        }
                    };
                    _parentGoalId = service.Create(parentGoal);
                    parentGoal.Id = _parentGoalId;

                    Console.WriteLine("Created parent goal");
                    Console.WriteLine("-------------------");
                    Console.WriteLine("Target: {0}", parentGoal.TargetInteger.Value);
                    Console.WriteLine("Stretch Target: {0}",
                        parentGoal.StretchTargetInteger.Value);
                    Console.WriteLine("Goal owner: {0}", parentGoal.GoalOwnerId.Id);
                    Console.WriteLine("Goal Fiscal Period: {0}",
                        parentGoal.FiscalPeriod.Value);
                    Console.WriteLine("Goal Fiscal Year: {0}",
                        parentGoal.FiscalYear.Value);
                    Console.WriteLine("<End of Listing>");
                    Console.WriteLine();

                    Goal firstChildGoal = new Goal()
                    {
                        Title = "First Child Goal Example",
                        ConsiderOnlyGoalOwnersRecords = true,
                        TargetInteger = 5,
                        StretchTargetInteger = 6,
                        IsFiscalPeriodGoal = true,
                        FiscalPeriod = new OptionSetValue(quarterNumber),
                        FiscalYear = new OptionSetValue(yearNumber),
                        MetricId = metric.ToEntityReference(),
                        ParentGoalId = parentGoal.ToEntityReference(),
                        GoalOwnerId = new EntityReference
                        {
                            Id = _salesRepresentativeIds[0],
                            LogicalName = SystemUser.EntityLogicalName
                        },
                        OwnerId = new EntityReference
                        {
                            Id = _salesManagerId,
                            LogicalName = SystemUser.EntityLogicalName
                        },
                        RollupQueryActualIntegerId = goalRollupQuery.ToEntityReference()
                    };
                    _firstChildGoalId = service.Create(firstChildGoal);

                    Console.WriteLine("First child goal");
                    Console.WriteLine("----------------");
                    Console.WriteLine("Target: {0}", firstChildGoal.TargetInteger.Value);
                    Console.WriteLine("Stretch Target: {0}",
                        firstChildGoal.StretchTargetInteger.Value);
                    Console.WriteLine("Goal owner: {0}", firstChildGoal.GoalOwnerId.Id);
                    Console.WriteLine("Goal Fiscal Period: {0}",
                        firstChildGoal.FiscalPeriod.Value);
                    Console.WriteLine("Goal Fiscal Year: {0}",
                        firstChildGoal.FiscalYear.Value);
                    Console.WriteLine();

                    Goal secondChildGoal = new Goal()
                    {
                        Title = "Second Child Goal Example",
                        ConsiderOnlyGoalOwnersRecords = true,
                        TargetInteger = 3,
                        StretchTargetInteger = 4,
                        IsFiscalPeriodGoal = true,
                        FiscalPeriod = new OptionSetValue(quarterNumber),
                        FiscalYear = new OptionSetValue(yearNumber),
                        MetricId = metric.ToEntityReference(),
                        ParentGoalId = parentGoal.ToEntityReference(),
                        GoalOwnerId = new EntityReference
                        {
                            Id = _salesRepresentativeIds[1],
                            LogicalName = SystemUser.EntityLogicalName
                        },
                        OwnerId = new EntityReference
                        {
                            Id = _salesManagerId,
                            LogicalName = SystemUser.EntityLogicalName
                        },
                        RollupQueryActualIntegerId = goalRollupQuery2.ToEntityReference()
                    };
                    _secondChildGoalId = service.Create(secondChildGoal);

                    Console.WriteLine("Second child goal");
                    Console.WriteLine("-----------------");
                    Console.WriteLine("Target: {0}",
                        secondChildGoal.TargetInteger.Value);
                    Console.WriteLine("Stretch Target: {0}",
                        secondChildGoal.StretchTargetInteger.Value);
                    Console.WriteLine("Goal owner: {0}", secondChildGoal.GoalOwnerId.Id);
                    Console.WriteLine("Goal Fiscal Period: {0}",
                        secondChildGoal.FiscalPeriod.Value);
                    Console.WriteLine("Goal Fiscal Year: {0}",
                        secondChildGoal.FiscalYear.Value);
                    Console.WriteLine();

                    #endregion

                    #region Calculate rollup and display result

                    // Calculate roll-up of goals.
                    RecalculateRequest recalculateRequest = new RecalculateRequest()
                    {
                        Target = new EntityReference(Goal.EntityLogicalName, _parentGoalId)
                    };
                    service.Execute(recalculateRequest);

                    Console.WriteLine("Calculated roll-up of goals.");

                    // Retrieve and report 3 different computed values for the goals
                    // - Percentage
                    // - ComputedTargetAsOfTodayPercentageAchieved
                    // - ComputedTargetAsOfTodayInteger
                    QueryExpression retrieveValues = new QueryExpression()
                    {
                        EntityName = Goal.EntityLogicalName,
                        ColumnSet = new ColumnSet(
                            "title",
                            "percentage",
                            "computedtargetasoftodaypercentageachieved",
                            "computedtargetasoftodayinteger")
                    };
                    EntityCollection ec = service.RetrieveMultiple(retrieveValues);

                    // Compute and display the results
                    for (int i = 0; i < ec.Entities.Count; i++)
                    {
                        Goal temp = (Goal)ec.Entities[i];
                        Console.WriteLine("Roll-up details for goal: {0}", temp.Title);
                        Console.WriteLine("---------------");
                        Console.WriteLine("Percentage: {0}", temp.Percentage);
                        Console.WriteLine("ComputedTargetAsOfTodayPercentageAchieved: {0}",
                            temp.ComputedTargetAsOfTodayPercentageAchieved);
                        Console.WriteLine("ComputedTargetAsOfTodayInteger: {0}",
                            temp.ComputedTargetAsOfTodayInteger.Value);
                        Console.WriteLine("<End of Listing>");
                    }

                    #endregion

                    
                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up
                }
                #endregion Demonstrate
                #endregion Sample Code
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
            }
}
