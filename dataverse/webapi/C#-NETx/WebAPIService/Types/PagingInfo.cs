namespace PowerApps.Samples.Types
{
    public class PagingInfo
    {
        public int Count { get; set; }
        public int PageNumber { get; set; }
        public string PagingCookie { get; set; }
        public bool ReturnTotalRecordCount { get; set; }
    }
}
