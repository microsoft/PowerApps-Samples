# Use Aggregation in FetchXML

This sample demonstrates how to use aggregate functions in FetchXML queries to perform calculations and grouping operations on Dataverse data.

## What This Sample Does

This sample creates sample opportunity records and then demonstrates 17 different aggregate query patterns using FetchXML, including:

- **Basic Aggregate Functions**: AVG, COUNT, MIN, MAX, SUM
- **Count Variations**: COUNT(*), COUNT(column), COUNT(DISTINCT column)
- **Multiple Aggregates**: Combining multiple aggregate functions in a single query
- **Grouping**: GROUP BY with ownerid and linked entities
- **Date Grouping**: Grouping by year, quarter, month, week, and day
- **Ordering**: Controlling the sort order of aggregate results

## How This Sample Works

### Setup

The sample creates:
1. One account record
2. Three opportunity records with different estimated values ($120,000, $240,000, $360,000)
3. Marks all opportunities as "Won" with a close date of November 1, 2009

### Demonstrate

The sample executes 17 different FetchXML queries demonstrating various aggregation scenarios:

1. **AVG** - Average estimated value of all opportunities
2. **COUNT(*)** - Total count of all opportunities
3. **COUNT(column)** - Count of opportunities with non-null name values
4. **COUNT(DISTINCT column)** - Count of distinct opportunity names
5. **MAX** - Maximum estimated value
6. **MIN** - Minimum estimated value
7. **SUM** - Sum of all estimated values
8. **Multiple Aggregates** - Count, sum, and average in one query
9. **GROUP BY ownerid** - Opportunities grouped by owner
10. **GROUP BY with link-entity** - Opportunities grouped by manager
11. **Date Grouping by Year** - Won opportunities grouped by year
12. **Date Grouping by Quarter** - Won opportunities grouped by quarter (1-4)
13. **Date Grouping by Month** - Won opportunities grouped by month (1-12)
14. **Date Grouping by Week** - Won opportunities grouped by week (1-52)
15. **Date Grouping by Day** - Won opportunities grouped by day (1-31)
16. **Multiple Date Groupings** - Grouped by both year and quarter
17. **Ordered Results** - Same as #16 but with explicit ordering

### Cleanup

The sample deletes all created records (opportunities and account).

## Key Concepts

### Aggregate Attributes

FetchXML supports these aggregate functions via the `aggregate` attribute:
- `count` - Count all records
- `countcolumn` - Count non-null values in a column
- `sum` - Sum numeric values
- `avg` - Average of numeric values
- `min` - Minimum value
- `max` - Maximum value

### Aliased Values

Aggregate results are returned as `AliasedValue` objects. To access the value:

```csharp
var result = (int)((AliasedValue)entity["alias_name"]).Value;
```

### Date Grouping

FetchXML supports date grouping with the `dategrouping` attribute:
- `year` - Group by year
- `quarter` - Group by quarter (1-4)
- `month` - Group by month (1-12)
- `week` - Group by week (1-52)
- `day` - Group by day of month (1-31)

### Distinct Counts

Use `distinct='true'` with `countcolumn` to count unique values:

```xml
<attribute name='name' alias='distinct_count' aggregate='countcolumn' distinct='true'/>
```

## Running the Sample

1. Update the connection string in `appsettings.json` at the Query folder level
2. Build the project: `dotnet build`
3. Run the sample: `dotnet run`

The sample will:
- Create test data (1 account, 3 opportunities)
- Execute all 17 aggregate queries and display results
- Prompt to delete created records
- Clean up all created data

## More Information

- [Use FetchXML aggregation](https://docs.microsoft.com/power-apps/developer/data-platform/use-fetchxml-aggregation)
- [FetchXML reference](https://docs.microsoft.com/power-apps/developer/data-platform/fetchxml-reference)
- [Build queries with FetchXML](https://docs.microsoft.com/power-apps/developer/data-platform/org-service/build-queries-fetchxml)
