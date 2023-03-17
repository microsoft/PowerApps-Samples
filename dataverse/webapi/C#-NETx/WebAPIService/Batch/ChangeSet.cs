namespace PowerApps.Samples.Batch
{
    public class ChangeSet
    {
        private List<HttpRequestMessage> requests = new();

        public ChangeSet(List<HttpRequestMessage> requests)
        {
            Requests = requests;
        }

        /// <summary>
        /// Sets Requests to send with the change set
        /// </summary>
        public List<HttpRequestMessage> Requests
        {
            set {
                requests.Clear();
                value.ForEach(x => {
                    if (x.Method == HttpMethod.Get)
                    {
                        throw new ArgumentException("ChangeSets cannot contain GET requests.");
                    }
                    requests.Add(x);
                });                 
            }
            get { return requests; }
        }

    }
}
