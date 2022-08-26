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
                // Service implements IOrganizationService interface 
                if (service.IsReady)
                {

                    #region Sample Code
                    //////////////////////////////////////////////
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate

                    // Fetch the average of estimatedvalue for all opportunities.  This is the equivalent of 
                    // SELECT AVG(estimatedvalue) AS estimatedvalue_avg ... in SQL.
                    Console.WriteLine("===============================");
                    string estimatedvalue_avg = @" 
                    <fetch distinct='false' mapping='logical' aggregate='true'> 
                        <entity name='opportunity'> 
                           <attribute name='estimatedvalue' alias='estimatedvalue_avg' aggregate='avg' /> 
                        </entity> 
                    </fetch>";

                    EntityCollection estimatedvalue_avg_result = service.RetrieveMultiple(new FetchExpression(estimatedvalue_avg));

                    foreach (var c in estimatedvalue_avg_result.Entities)
                    {
                        decimal aggregate1 = ((Money)((AliasedValue)c["estimatedvalue_avg"]).Value).Value;
                        Console.WriteLine("Average estimated value: " + aggregate1);

                    }
                    Console.WriteLine("===============================");

                    // *****************************************************************************************************************
                    //                FetchXML      opportunity_count   Aggregate 2
                    // *****************************************************************************************************************
                    // Fetch the count of all opportunities.  This is the equivalent of
                    // SELECT COUNT(*) AS opportunity_count ... in SQL.

                    string opportunity_count = @" 
                    <fetch distinct='false' mapping='logical' aggregate='true'> 
                        <entity name='opportunity'> 
                           <attribute name='name' alias='opportunity_count' aggregate='count'/> 
                        </entity> 
                    </fetch>";

                    EntityCollection opportunity_count_result = service.RetrieveMultiple(new FetchExpression(opportunity_count));

                    foreach (var c in opportunity_count_result.Entities)
                    {
                        Int32 aggregate2 = (Int32)((AliasedValue)c["opportunity_count"]).Value;
                        Console.WriteLine("Count of all opportunities: " + aggregate2);

                    }
                    Console.WriteLine("===============================");

                    // *****************************************************************************************************************
                    //                FetchXML      opportunity_colcount   Aggregate 3
                    // *****************************************************************************************************************
                    // Fetch the count of all opportunities.  This is the equivalent of 
                    // SELECT COUNT(name) AS opportunity_count ... in SQL.
                    string opportunity_colcount = @" 
                    <fetch distinct='false' mapping='logical' aggregate='true'> 
                        <entity name='opportunity'> 
                           <attribute name='name' alias='opportunity_colcount' aggregate='countcolumn'/> 
                        </entity> 
                    </fetch>";

                    EntityCollection opportunity_colcount_result = service.RetrieveMultiple(new FetchExpression(opportunity_colcount));

                    foreach (var c in opportunity_colcount_result.Entities)
                    {
                        Int32 aggregate3 = (Int32)((AliasedValue)c["opportunity_colcount"]).Value;
                        Console.WriteLine("Column count of all opportunities: " + aggregate3);

                    }
                    Console.WriteLine("===============================");

                    // *****************************************************************************************************************
                    //                FetchXML      opportunity_distcount   Aggregate 4
                    // *****************************************************************************************************************
                    // Fetch the count of distinct names for opportunities.  This is the equivalent of 
                    // SELECT COUNT(DISTINCT name) AS opportunity_count ... in SQL.
                    string opportunity_distcount = @" 
                    <fetch distinct='false' mapping='logical' aggregate='true'> 
                        <entity name='opportunity'> 
                           <attribute name='name' alias='opportunity_distcount' aggregate='countcolumn' distinct='true'/> 
                        </entity> 
                    </fetch>";

                    EntityCollection opportunity_distcount_result = service.RetrieveMultiple(new FetchExpression(opportunity_distcount));

                    foreach (var c in opportunity_distcount_result.Entities)
                    {
                        Int32 aggregate4 = (Int32)((AliasedValue)c["opportunity_distcount"]).Value;
                        Console.WriteLine("Distinct name count of all opportunities: " + aggregate4);

                    }
                   Console.WriteLine("===============================");

                    // *****************************************************************************************************************
                    //                FetchXML      estimatedvalue_max   Aggregate 5
                    // *****************************************************************************************************************
                    // Fetch the maximum estimatedvalue of all opportunities.  This is the equivalent of 
                    // SELECT MAX(estimatedvalue) AS estimatedvalue_max ... in SQL.
                    string estimatedvalue_max = @" 
                    <fetch distinct='false' mapping='logical' aggregate='true'> 
                        <entity name='opportunity'> 
                           <attribute name='estimatedvalue' alias='estimatedvalue_max' aggregate='max' /> 
                        </entity> 
                    </fetch>";

                    EntityCollection estimatedvalue_max_result = service.RetrieveMultiple(new FetchExpression(estimatedvalue_max));

                    foreach (var c in estimatedvalue_max_result.Entities)
                    {
                        decimal aggregate5 = ((Money)((AliasedValue)c["estimatedvalue_max"]).Value).Value;
                        Console.WriteLine("Max estimated value of all opportunities: " + aggregate5);

                    }
                    Console.WriteLine("===============================");

                    // *****************************************************************************************************************
                    //                FetchXML      estimatedvalue_min   Aggregate 6
                    // *****************************************************************************************************************
                    // Fetch the minimum estimatedvalue of all opportunities.  This is the equivalent of 
                    // SELECT MIN(estimatedvalue) AS estimatedvalue_min ... in SQL.
                    string estimatedvalue_min = @" 
                    <fetch distinct='false' mapping='logical' aggregate='true'> 
                        <entity name='opportunity'> 
                           <attribute name='estimatedvalue' alias='estimatedvalue_min' aggregate='min' /> 
                        </entity> 
                    </fetch>";

                    EntityCollection estimatedvalue_min_result = service.RetrieveMultiple(new FetchExpression(estimatedvalue_min));

                    foreach (var c in estimatedvalue_min_result.Entities)
                    {
                        decimal aggregate6 = ((Money)((AliasedValue)c["estimatedvalue_min"]).Value).Value;
                        Console.WriteLine("Minimum estimated value of all opportunities: " + aggregate6);

                    }
                    Console.WriteLine("===============================");

                    // *****************************************************************************************************************
                    //                FetchXML      estimatedvalue_sum   Aggregate 7
                    // *****************************************************************************************************************
                    // Fetch the sum of estimatedvalue for all opportunities.  This is the equivalent of 
                    // SELECT SUM(estimatedvalue) AS estimatedvalue_sum ... in SQL.
                    string estimatedvalue_sum = @" 
                    <fetch distinct='false' mapping='logical' aggregate='true'> 
                        <entity name='opportunity'> 
                           <attribute name='estimatedvalue' alias='estimatedvalue_sum' aggregate='sum' /> 
                        </entity> 
                    </fetch>";

                    EntityCollection estimatedvalue_sum_result = service.RetrieveMultiple(new FetchExpression(estimatedvalue_sum));

                    foreach (var c in estimatedvalue_sum_result.Entities)
                    {
                        decimal aggregate7 = ((Money)((AliasedValue)c["estimatedvalue_sum"]).Value).Value;
                        Console.WriteLine("Sum of estimated value of all opportunities: " + aggregate7);

                    }
                    Console.WriteLine("===============================");

                    // *****************************************************************************************************************
                    //                FetchXML      estimatedvalue_avg, estimatedvalue_sum   Aggregate 8
                    // *****************************************************************************************************************
                    // Fetch multiple aggregate values within a single query.
                    string estimatedvalue_avg2 = @" 
                    <fetch distinct='false' mapping='logical' aggregate='true'> 
                        <entity name='opportunity'> 
                           <attribute name='opportunityid' alias='opportunity_count' aggregate='count'/> 
                           <attribute name='estimatedvalue' alias='estimatedvalue_sum' aggregate='sum'/> 
                           <attribute name='estimatedvalue' alias='estimatedvalue_avg' aggregate='avg'/> 
                        </entity> 
                    </fetch>";

                    EntityCollection estimatedvalue_avg2_result = service.RetrieveMultiple(new FetchExpression(estimatedvalue_avg2));

                    foreach (var c in estimatedvalue_avg2_result.Entities)
                    {
                        Int32 aggregate8a = (Int32)((AliasedValue)c["opportunity_count"]).Value;
                        Console.WriteLine("Count of all opportunities: " + aggregate8a);
                        decimal aggregate8b = ((Money)((AliasedValue)c["estimatedvalue_sum"]).Value).Value;
                        Console.WriteLine("Sum of estimated value of all opportunities: " + aggregate8b);
                        decimal aggregate8c = ((Money)((AliasedValue)c["estimatedvalue_avg"]).Value).Value;
                        Console.WriteLine("Average of estimated value of all opportunities: " + aggregate8c);

                    }
                    System.Console.WriteLine("===============================");

                    // *****************************************************************************************************************
                    //                FetchXML      groupby1   Aggregate 9
                    // *****************************************************************************************************************
                    // Fetch a list of users with a count of all the opportunities they own using groupby.
                    string groupby1 = @" 
                    <fetch distinct='false' mapping='logical' aggregate='true'> 
                        <entity name='opportunity'> 
                           <attribute name='name' alias='opportunity_count' aggregate='countcolumn' /> 
                           <attribute name='ownerid' alias='ownerid' groupby='true' /> 
                        </entity> 
                    </fetch>";

                    EntityCollection groupby1_result = service.RetrieveMultiple(new FetchExpression(groupby1));

                    foreach (var c in groupby1_result.Entities)
                    {
                        Int32 aggregate9a = (Int32)((AliasedValue)c["opportunity_count"]).Value;
                        Console.WriteLine("Count of all opportunities: " + aggregate9a + "\n");
                        string aggregate9b = ((EntityReference)((AliasedValue)c["ownerid"]).Value).Name;
                        Console.WriteLine("Owner: " + aggregate9b);
                        string aggregate9c = (string)((AliasedValue)c["ownerid_owneridyominame"]).Value;
                        Console.WriteLine("Owner: " + aggregate9c);
                        string aggregate9d = (string)((AliasedValue)c["ownerid_owneridyominame"]).Value;
                        Console.WriteLine("Owner: " + aggregate9d);
                    }
                    System.Console.WriteLine("===============================");

                    // *****************************************************************************************************************
                    //                FetchXML      groupby2   Aggregate 10
                    // *****************************************************************************************************************
                    // Fetch the number of opportunities each manager's direct reports 
                    // own using a groupby within a link-entity.
                    string groupby2 = @" 
                    <fetch distinct='false' mapping='logical' aggregate='true'> 
                        <entity name='opportunity'> 
                           <attribute name='name' alias='opportunity_count' aggregate='countcolumn' /> 
                           <link-entity name='systemuser' from='systemuserid' to='ownerid'>
                               <attribute name='parentsystemuserid' alias='managerid' groupby='true' />
                           </link-entity> 
                        </entity> 
                    </fetch>";

                    EntityCollection groupby2_result = service.RetrieveMultiple(new FetchExpression(groupby2));

                    foreach (var c in groupby2_result.Entities)
                    {

                        int? aggregate10a = (int?)((AliasedValue)c["opportunity_count"]).Value;
                        Console.WriteLine("Count of all opportunities: " + aggregate10a + "\n");
                    }
                    Console.WriteLine("===============================");

                    // *****************************************************************************************************************
                    //                FetchXML      byyear   Aggregate 11           
                    // *****************************************************************************************************************
                    // Fetch aggregate information about the opportunities that have 
                    // been won by year.
                    string byyear = @" 
                    <fetch distinct='false' mapping='logical' aggregate='true'> 
                        <entity name='opportunity'> 
                           <attribute name='opportunityid' alias='opportunity_count' aggregate='count'/> 
                           <attribute name='estimatedvalue' alias='estimatedvalue_sum' aggregate='sum'/> 
                           <attribute name='estimatedvalue' alias='estimatedvalue_avg' aggregate='avg'/> 
                           <attribute name='actualclosedate' groupby='true' dategrouping='year' alias='year' />
                           <filter type='and'>
                               <condition attribute='statecode' operator='eq' value='Won' />
                           </filter>
                        </entity> 
                    </fetch>";

                    EntityCollection byyear_result = service.RetrieveMultiple(new FetchExpression(byyear));

                    foreach (var c in byyear_result.Entities)
                    {
                        Int32 aggregate11 = (Int32)((AliasedValue)c["year"]).Value;
                        Console.WriteLine("Year: " + aggregate11);
                        Int32 aggregate11a = (Int32)((AliasedValue)c["opportunity_count"]).Value;
                        Console.WriteLine("Count of all opportunities: " + aggregate11a);
                        decimal aggregate11b = ((Money)((AliasedValue)c["estimatedvalue_sum"]).Value).Value;
                        Console.WriteLine("Sum of estimated value of all opportunities: " + aggregate11b);
                        decimal aggregate11c = ((Money)((AliasedValue)c["estimatedvalue_avg"]).Value).Value;
                        Console.WriteLine("Average of estimated value of all opportunities: " + aggregate11c);
                        Console.WriteLine("----------------------------------------------");
                    }
                    System.Console.WriteLine("===============================");
                    // *****************************************************************************************************************
                    //                FetchXML      byquarter   Aggregate 12           
                    // *****************************************************************************************************************
                    // Fetch aggregate information about the opportunities that have 
                    // been won by quarter.(returns 1-4)
                    string byquarter = @" 
                    <fetch distinct='false' mapping='logical' aggregate='true'> 
                        <entity name='opportunity'> 
                           <attribute name='opportunityid' alias='opportunity_count' aggregate='count'/> 
                           <attribute name='estimatedvalue' alias='estimatedvalue_sum' aggregate='sum'/> 
                           <attribute name='estimatedvalue' alias='estimatedvalue_avg' aggregate='avg'/> 
                           <attribute name='actualclosedate' groupby='true' dategrouping='quarter' alias='quarter' />
                           <filter type='and'>
                               <condition attribute='statecode' operator='eq' value='Won' />
                           </filter>
                        </entity> 
                    </fetch>";

                    EntityCollection byquarter_result = service.RetrieveMultiple(new FetchExpression(byquarter));

                    foreach (var c in byquarter_result.Entities)
                    {
                        Int32 aggregate12 = (Int32)((AliasedValue)c["quarter"]).Value;
                        Console.WriteLine("Quarter: " + aggregate12);
                        Int32 aggregate12a = (Int32)((AliasedValue)c["opportunity_count"]).Value;
                        Console.WriteLine("Count of all opportunities: " + aggregate12a);
                        decimal aggregate12b = ((Money)((AliasedValue)c["estimatedvalue_sum"]).Value).Value;
                        Console.WriteLine("Sum of estimated value of all opportunities: " + aggregate12b);
                        decimal aggregate12c = ((Money)((AliasedValue)c["estimatedvalue_avg"]).Value).Value;
                        Console.WriteLine("Average of estimated value of all opportunities: " + aggregate12c);
                        Console.WriteLine("----------------------------------------------");
                    }
                    Console.WriteLine("===============================");

                    // *****************************************************************************************************************
                    //                FetchXML      bymonth   Aggregate 13           
                    // *****************************************************************************************************************
                    // Fetch aggregate information about the opportunities that have 
                    // been won by month. (returns 1-12)
                    string bymonth = @" 
                    <fetch distinct='false' mapping='logical' aggregate='true'> 
                        <entity name='opportunity'> 
                           <attribute name='opportunityid' alias='opportunity_count' aggregate='count'/> 
                           <attribute name='estimatedvalue' alias='estimatedvalue_sum' aggregate='sum'/> 
                           <attribute name='estimatedvalue' alias='estimatedvalue_avg' aggregate='avg'/> 
                           <attribute name='actualclosedate' groupby='true' dategrouping='month' alias='month' />
                           <filter type='and'>
                               <condition attribute='statecode' operator='eq' value='Won' />
                           </filter>
                        </entity> 
                    </fetch>";

                    EntityCollection bymonth_result = service.RetrieveMultiple(new FetchExpression(bymonth));

                    foreach (var c in bymonth_result.Entities)
                    {
                        Int32 aggregate13 = (Int32)((AliasedValue)c["month"]).Value;
                        Console.WriteLine("Month: " + aggregate13);
                        Int32 aggregate13a = (Int32)((AliasedValue)c["opportunity_count"]).Value;
                        Console.WriteLine("Count of all opportunities: " + aggregate13a);
                        decimal aggregate13b = ((Money)((AliasedValue)c["estimatedvalue_sum"]).Value).Value;
                        Console.WriteLine("Sum of estimated value of all opportunities: " + aggregate13b);
                        decimal aggregate13c = ((Money)((AliasedValue)c["estimatedvalue_avg"]).Value).Value;
                        Console.WriteLine("Average of estimated value of all opportunities: " + aggregate13c);
                        Console.WriteLine("----------------------------------------------");
                    }
                    System.Console.WriteLine("===============================");
                    // *****************************************************************************************************************
                    //                FetchXML      byweek   Aggregate 14           
                    // *****************************************************************************************************************
                    // Fetch aggregate information about the opportunities that have 
                    // been won by week. (Returns 1-52)
                    string byweek = @" 
                    <fetch distinct='false' mapping='logical' aggregate='true'> 
                        <entity name='opportunity'> 
                           <attribute name='opportunityid' alias='opportunity_count' aggregate='count'/> 
                           <attribute name='estimatedvalue' alias='estimatedvalue_sum' aggregate='sum'/> 
                           <attribute name='estimatedvalue' alias='estimatedvalue_avg' aggregate='avg'/> 
                           <attribute name='actualclosedate' groupby='true' dategrouping='week' alias='week' />
                           <filter type='and'>
                               <condition attribute='statecode' operator='eq' value='Won' />
                           </filter>
                        </entity> 
                    </fetch>";

                    EntityCollection byweek_result = service.RetrieveMultiple(new FetchExpression(byweek));

                    foreach (var c in byweek_result.Entities)
                    {
                        Int32 aggregate14 = (Int32)((AliasedValue)c["week"]).Value;
                        Console.WriteLine("Week: " + aggregate14);
                        Int32 aggregate14a = (Int32)((AliasedValue)c["opportunity_count"]).Value;
                        Console.WriteLine("Count of all opportunities: " + aggregate14a);
                        decimal aggregate14b = ((Money)((AliasedValue)c["estimatedvalue_sum"]).Value).Value;
                        Console.WriteLine("Sum of estimated value of all opportunities: " + aggregate14b);
                        decimal aggregate14c = ((Money)((AliasedValue)c["estimatedvalue_avg"]).Value).Value;
                        Console.WriteLine("Average of estimated value of all opportunities: " + aggregate14c);
                        Console.WriteLine("----------------------------------------------");
                    }
                    System.Console.WriteLine("===============================");
                    // *****************************************************************************************************************
                    //                FetchXML      byday   Aggregate 15           
                    // *****************************************************************************************************************
                    // Fetch aggregate information about the opportunities that have 
                    // been won by day. (Returns 1-31)
                    string byday = @" 
                    <fetch distinct='false' mapping='logical' aggregate='true'> 
                        <entity name='opportunity'> 
                           <attribute name='opportunityid' alias='opportunity_count' aggregate='count'/> 
                           <attribute name='estimatedvalue' alias='estimatedvalue_sum' aggregate='sum'/> 
                           <attribute name='estimatedvalue' alias='estimatedvalue_avg' aggregate='avg'/> 
                           <attribute name='actualclosedate' groupby='true' dategrouping='day' alias='day' />
                           <filter type='and'>
                               <condition attribute='statecode' operator='eq' value='Won' />
                           </filter>
                        </entity> 
                    </fetch>";

                    EntityCollection byday_result = service.RetrieveMultiple(new FetchExpression(byday));

                    foreach (var c in byday_result.Entities)
                    {
                        Int32 aggregate15 = (Int32)((AliasedValue)c["day"]).Value;
                        Console.WriteLine("Day: " + aggregate15);
                        Int32 aggregate15a = (Int32)((AliasedValue)c["opportunity_count"]).Value;
                        Console.WriteLine("Count of all opportunities: " + aggregate15a);
                        decimal aggregate15b = ((Money)((AliasedValue)c["estimatedvalue_sum"]).Value).Value;
                        Console.WriteLine("Sum of estimated value of all opportunities: " + aggregate15b);
                        decimal aggregate15c = ((Money)((AliasedValue)c["estimatedvalue_avg"]).Value).Value;
                        Console.WriteLine("Average of estimated value of all opportunities: " + aggregate15c);
                        Console.WriteLine("----------------------------------------------");
                    }
                    System.Console.WriteLine("===============================");

                    // *****************************************************************************************************************
                    //                FetchXML      byyrqtr   Aggregate 16           
                    // *****************************************************************************************************************
                    // Fetch aggregate information about the opportunities that have 
                    // been won by year and quarter.
                    string byyrqtr = @" 
                    <fetch distinct='false' mapping='logical' aggregate='true'> 
                        <entity name='opportunity'> 
                           <attribute name='opportunityid' alias='opportunity_count' aggregate='count'/> 
                           <attribute name='estimatedvalue' alias='estimatedvalue_sum' aggregate='sum'/> 
                           <attribute name='estimatedvalue' alias='estimatedvalue_avg' aggregate='avg'/> 
                           <attribute name='actualclosedate' groupby='true' dategrouping='quarter' alias='quarter' />
                           <attribute name='actualclosedate' groupby='true' dategrouping='year' alias='year' />
                           <filter type='and'>
                               <condition attribute='statecode' operator='eq' value='Won' />
                           </filter>
                        </entity> 
                    </fetch>";

                    EntityCollection byyrqtr_result = service.RetrieveMultiple(new FetchExpression(byyrqtr));

                    foreach (var c in byyrqtr_result.Entities)
                    {
                        Int32 aggregate16d = (Int32)((AliasedValue)c["year"]).Value;
                        Console.WriteLine("Year: " + aggregate16d);
                        Int32 aggregate16 = (Int32)((AliasedValue)c["quarter"]).Value;
                        Console.WriteLine("Quarter: " + aggregate16);
                        Int32 aggregate16a = (Int32)((AliasedValue)c["opportunity_count"]).Value;
                        Console.WriteLine("Count of all opportunities: " + aggregate16a);
                        decimal aggregate16b = ((Money)((AliasedValue)c["estimatedvalue_sum"]).Value).Value;
                        Console.WriteLine("Sum of estimated value of all opportunities: " + aggregate16b);
                        decimal aggregate16c = ((Money)((AliasedValue)c["estimatedvalue_avg"]).Value).Value;
                        Console.WriteLine("Average of estimated value of all opportunities: " + aggregate16c);
                        Console.WriteLine("----------------------------------------------");
                    }
                    Console.WriteLine("===============================");

                    // *****************************************************************************************************************
                    //                FetchXML      byyrqtr2   Aggregate 17           
                    // *****************************************************************************************************************
                    // Specify the result order for the previous sample.  Order by year, then quarter.
                    string byyrqtr2 = @" 
                    <fetch distinct='false' mapping='logical' aggregate='true'> 
                        <entity name='opportunity'> 
                           <attribute name='opportunityid' alias='opportunity_count' aggregate='count'/> 
                           <attribute name='estimatedvalue' alias='estimatedvalue_sum' aggregate='sum'/> 
                           <attribute name='estimatedvalue' alias='estimatedvalue_avg' aggregate='avg'/> 
                           <attribute name='actualclosedate' groupby='true' dategrouping='quarter' alias='quarter' />
                           <attribute name='actualclosedate' groupby='true' dategrouping='year' alias='year' />
                           <order alias='year' descending='false' />
                           <order alias='quarter' descending='false' />
                           <filter type='and'>
                               <condition attribute='statecode' operator='eq' value='Won' />
                           </filter>
                        </entity> 
                    </fetch>";

                    EntityCollection byyrqtr2_result = service.RetrieveMultiple(new FetchExpression(byyrqtr2));

                    foreach (var c in byyrqtr2_result.Entities)
                    {
                        Int32 aggregate17 = (Int32)((AliasedValue)c["quarter"]).Value;
                        Console.WriteLine("Quarter: " + aggregate17);
                        Int32 aggregate17d = (Int32)((AliasedValue)c["year"]).Value;
                        Console.WriteLine("Year: " + aggregate17d);
                        Int32 aggregate17a = (Int32)((AliasedValue)c["opportunity_count"]).Value;
                        Console.WriteLine("Count of all opportunities: " + aggregate17a);
                        decimal aggregate17b = ((Money)((AliasedValue)c["estimatedvalue_sum"]).Value).Value;
                        Console.WriteLine("Sum of estimated value of all opportunities: " + aggregate17b);
                        decimal aggregate17c = ((Money)((AliasedValue)c["estimatedvalue_avg"]).Value).Value;
                        Console.WriteLine("Average of estimated value of all opportunities: " + aggregate17c);
                        Console.WriteLine("----------------------------------------------");
                    }
                    Console.WriteLine("===============================");


                    Console.WriteLine("Retrieved aggregate record data.");

                    #region Clean up
                    // Provides option to delete the ChangeTracking solution
                    CleanUpSample(service);
                    #endregion Clean up
                    //////////////////////////////////////////////
                    
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
#endregion Sample Code

