using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
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

                    // Create the revenue metric, setting the Amount Data Type to 'Money'
                    // and the Metric Type to 'Amount'.
                    Metric sampleMetric = new Metric()
                    {
                        Name = "Sample Revenue Metric",
                        AmountDataType = new OptionSetValue(0),
                        IsAmount = true,
                    };
                    _metricId = service.Create(sampleMetric);
                    sampleMetric.Id = _metricId;

                    Console.Write("Created revenue metric, ");

                    #region Create RollupFields

                    // Create RollupField which targets the actual totals.
                    RollupField actual = new RollupField()
                    {
                        SourceEntity = SalesOrder.EntityLogicalName,
                        SourceAttribute = "totalamount",
                        GoalAttribute = "actualmoney",
                        SourceState = 1,
                        EntityForDateAttribute = SalesOrder.EntityLogicalName,
                        DateAttribute = "datefulfilled",
                        MetricId = sampleMetric.ToEntityReference()
                    };
                    _actualId = service.Create(actual);

                    Console.Write("created actual revenue RollupField, ");

                    #endregion

                    #region Create the goal rollup query

                    // The query locates sales orders in the first sales 
                    // representative's area (zip code: 60661) and with a value
                    // greater than $1,000.
                    GoalRollupQuery goalRollupQuery = new GoalRollupQuery()
                    {
                        Name = "First Example Goal Rollup Query",
                        QueryEntityType = SalesOrder.EntityLogicalName,
                        FetchXml = @"<fetch mapping=""logical"" version=""1.0""><entity name=""salesorder""><attribute name=""customerid"" /><attribute name=""name"" /><attribute name=""salesorderid"" /><attribute name=""statuscode"" /><attribute name=""totalamount"" /><order attribute=""name"" /><filter><condition attribute=""totalamount"" operator=""gt"" value=""1000"" /><condition attribute=""billto_postalcode"" operator=""eq"" value=""60661"" /></filter></entity></fetch>"
                    };
                    _rollupQueryId = service.Create(goalRollupQuery);
                    goalRollupQuery.Id = _rollupQueryId;

                    Console.Write("created rollup query.");
                    Console.WriteLine();

                    #endregion

                    #region Create two goals: one parent and one child goal

                    // Create the parent goal.
                    Goal parentGoal = new Goal()
                    {
                        Title = "Parent Goal Example",
                        RollupOnlyFromChildGoals = true,
                        TargetMoney = new Money(1000.0M),
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
                    _parentGoalId = service.Create(parentGoal);
                    parentGoal.Id = _parentGoalId;

                    Console.WriteLine("Created parent goal");
                    Console.WriteLine("-------------------");
                    Console.WriteLine("Target: {0}", parentGoal.TargetMoney.Value);
                    Console.WriteLine("Goal owner: {0}", parentGoal.GoalOwnerId.Id);
                    Console.WriteLine("Goal Start Date: {0}", parentGoal.GoalStartDate);
                    Console.WriteLine("Goal End Date: {0}", parentGoal.GoalEndDate);
                    Console.WriteLine("<End of Listing>");
                    Console.WriteLine();

                    // Create the child goal.
                    Goal firstChildGoal = new Goal()
                    {
                        Title = "First Child Goal Example",
                        ConsiderOnlyGoalOwnersRecords = true,
                        TargetMoney = new Money(1000.0M),
                        IsFiscalPeriodGoal = false,
                        MetricId = sampleMetric.ToEntityReference(),
                        ParentGoalId = parentGoal.ToEntityReference(),
                        GoalOwnerId = new EntityReference
                        {
                            Id = _salesRepresentativeId,
                            LogicalName = SystemUser.EntityLogicalName
                        },
                        OwnerId = new EntityReference
                        {
                            Id = _salesManagerId,
                            LogicalName = SystemUser.EntityLogicalName
                        },
                        RollUpQueryActualMoneyId = goalRollupQuery.ToEntityReference(),
                        GoalStartDate = DateTime.Today.AddDays(-1),
                        GoalEndDate = DateTime.Today.AddDays(30)
                    };
                    _firstChildGoalId = service.Create(firstChildGoal);

                    Console.WriteLine("First child goal");
                    Console.WriteLine("----------------");
                    Console.WriteLine("Target: {0}", firstChildGoal.TargetMoney.Value);
                    Console.WriteLine("Goal owner: {0}", firstChildGoal.GoalOwnerId.Id);
                    Console.WriteLine("Goal Start Date: {0}", firstChildGoal.GoalStartDate);
                    Console.WriteLine("Goal End Date: {0}", firstChildGoal.GoalEndDate);
                    Console.WriteLine("<End of Listing>");
                    Console.WriteLine();

                    #endregion

                    // Calculate roll-up of goals.
                    // Note: Recalculate can be run against any goal in the tree to cause
                    // a rollup of the whole tree.
                    RecalculateRequest recalculateRequest = new RecalculateRequest()
                    {
                        Target = parentGoal.ToEntityReference()
                    };
                    service.Execute(recalculateRequest);

                    Console.WriteLine("Calculated roll-up of goals.");
                    Console.WriteLine();

                    // Retrieve and report 3 different computed values for the goals
                    // - Percentage
                    // - ComputedTargetAsOfTodayPercentageAchieved
                    // - ComputedTargetAsOfTodayMoney
                    QueryExpression retrieveValues = new QueryExpression()
                    {
                        EntityName = Goal.EntityLogicalName,
                        ColumnSet = new ColumnSet(
                            "title",
                            "computedtargetasoftodaypercentageachieved",
                            "computedtargetasoftodaymoney")
                    };
                    EntityCollection ec = service.RetrieveMultiple(retrieveValues);

                    // Compute and display the results
                    for (int i = 0; i < ec.Entities.Count; i++)
                    {
                        Goal temp = (Goal)ec.Entities[i];
                        Console.WriteLine("Roll-up details for goal: {0}", temp.Title);
                        Console.WriteLine("---------------");
                        Console.WriteLine("ComputedTargetAsOfTodayPercentageAchieved: {0}",
                            temp.ComputedTargetAsOfTodayPercentageAchieved);
                        Console.WriteLine("ComputedTargetAsOfTodayMoney: {0}",
                            temp.ComputedTargetAsOfTodayMoney.Value);
                        Console.WriteLine("<End of Listing>");
                    }

                   
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
