namespace PowerApps.Samples.Types
{
    public class ColumnSet
    {
        public ColumnSet(params string[] columns)
        {
            Columns = columns.ToList();
        }
        public bool AllColumns { get; set; }

        public List<string> Columns { get; set; }

        //TODO: Add AttributeExpressions when needed
    }
}
