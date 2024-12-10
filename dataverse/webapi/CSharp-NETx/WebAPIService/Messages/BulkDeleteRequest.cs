using Newtonsoft.Json.Linq;
using PowerApps.Samples.Types;

namespace PowerApps.Samples.Messages
{
    public class BulkDeleteRequest : HttpRequestMessage
    {
        public BulkDeleteRequest()
        {
            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: "BulkDelete",
                uriKind: UriKind.Relative);
        }

        private List<QueryExpression>? _querySet;
        private string? _jobName;
        private bool? _sendEmailNotification;
        private List<JObject>? _toRecipients;
        private List<JObject>? _cCRecipients;
        private string _recurrencePattern;
        private DateTimeOffset? _startDateTime;
        private Guid? _sourceImportId;
        private bool? _runNow;


        public List<QueryExpression>? QuerySet
        {
            get { return _querySet; }
            set
            {
                _querySet = value;
                SetContent();
            }
        }
        public string? JobName
        {
            get { return _jobName; }
            set
            {
                _jobName = value;
                SetContent();
            }
        }
        public bool? SendEmailNotification
        {
            get { return _sendEmailNotification; }
            set
            {
                _sendEmailNotification = value;
                SetContent();
            }
        }
        public List<JObject>? ToRecipients
        {
            get { return _toRecipients; }
            set
            {
                _toRecipients = value;
                SetContent();
            }
        }
        public List<JObject>? CCRecipients
        {
            get { return _cCRecipients; }
            set
            {
                _cCRecipients = value;
                SetContent();
            }
        }
        public string? RecurrencePattern
        {
            get { return _recurrencePattern; }
            set
            {
                _recurrencePattern = value;
                SetContent();
            }
        }
        public DateTimeOffset? StartDateTime
        {
            get { return _startDateTime; }
            set
            {
                _startDateTime = value;
                SetContent();
            }
        }
        public Guid? SourceImportId
        {
            get { return _sourceImportId; }
            set
            {
                _sourceImportId = value;
                SetContent();
            }
        }
        public bool? RunNow
        {
            get { return _runNow; }
            set
            {
                _runNow = value;
                SetContent();
            }
        }

        private void SetContent()
        {
            JObject _content = new();

            if (_querySet != null)
            {
                _content.Add(nameof(QuerySet), JToken.FromObject(_querySet));
            }

            if (!string.IsNullOrWhiteSpace(_jobName))
            {
                _content.Add(nameof(JobName), _jobName);
            }

            if (_sendEmailNotification != null)
            {
                _content.Add(nameof(SendEmailNotification), _sendEmailNotification);
            }

            if (_toRecipients != null)
            {
                _content.Add(nameof(ToRecipients), JToken.FromObject(_toRecipients));
            }

            if (_cCRecipients != null)
            {
                _content.Add(nameof(CCRecipients), JToken.FromObject(_cCRecipients));
            }

            // Always required
            _content.Add(nameof(RecurrencePattern), _recurrencePattern);

            if (_startDateTime != null)
            {
                _content.Add(nameof(StartDateTime), JToken.FromObject(_startDateTime));
            }

            if (_sourceImportId != null)
            {
                _content.Add(nameof(SourceImportId), JToken.FromObject(_sourceImportId));
            }

            if (_runNow != null)
            {
                _content.Add(nameof(RunNow), JToken.FromObject(_runNow));
            }


            Content = new StringContent(
                content: _content.ToString(),
                encoding: System.Text.Encoding.UTF8,
                mediaType: "application/json");

        }
    }
}
