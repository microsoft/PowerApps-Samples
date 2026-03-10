namespace PowerApps.Samples
{
    public class ODataError
    {
        public Error? Error { get; set; }
    }

    public class Error
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
    }
}
