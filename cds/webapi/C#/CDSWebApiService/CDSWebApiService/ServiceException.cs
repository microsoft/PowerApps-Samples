using System;

namespace PowerApps.Samples
{
    /// <summary>
    /// An exception that captures data returned by the Web API
    /// </summary>
    public class ServiceException : Exception
    {
        public int ErrorCode { get; private set; }
        public int StatusCode { get; private set; }
        public string ReasonPhrase { get; private set; }

        public ServiceException(int errorcode, int statuscode, string reasonphrase, string message) : base(message)
        {
            ErrorCode = errorcode;
            StatusCode = statuscode;
            ReasonPhrase = reasonphrase;
        }
    }
}
