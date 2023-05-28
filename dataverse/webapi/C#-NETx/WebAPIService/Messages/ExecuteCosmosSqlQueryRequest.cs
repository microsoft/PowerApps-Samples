using Newtonsoft.Json;
using PowerApps.Samples.Types;
using System.Text;

namespace PowerApps.Samples.Messages
{

    public sealed class ExecuteCosmosSqlQueryRequest : HttpRequestMessage
    {
        private string _queryText;
        private string _entityLogicalName;
        private ParameterCollection? _queryParameters;
        private long? _pageSize;
        private string? _pagingCookie;
        private string? _partitionId;


        public ExecuteCosmosSqlQueryRequest(string queryText, string entityLogicalName)
        {
            _queryText = queryText;
            _entityLogicalName = entityLogicalName;
            Method = HttpMethod.Get;
            SetUri();
        }

        public string QueryText
        {
            get
            {
                return _queryText;
            }

            set
            {
                _queryText = value;
                SetUri();
            }
        }

        public string EntityLogicalName
        {
            get
            {
                return _entityLogicalName;
            }

            set
            {
                _entityLogicalName = value;
                SetUri();
            }
        }

        public ParameterCollection? QueryParameters
        {
            get
            {
                return _queryParameters;
            }

            set
            {
                _queryParameters = value;
                SetUri();
            }

        }

        public long? PageSize
        {
            get
            {
                return _pageSize;
            }

            set
            {
                _pageSize = value;
                SetUri();
            }

        }

        public string? PagingCookie
        {
            get
            {
                return _pagingCookie;
            }

            set
            {
                _pagingCookie = value;
                SetUri();
            }
        }

        public string? PartitionId
        {
            get
            {
                return _partitionId;
            }

            set
            {
                _partitionId = value;
                SetUri();
            }
        }

        private void SetUri()
        {
            int count = 1;

            List<string> parameters = new();
            List<string> parameterValues = new();

            // QueryText
            parameters.Add($"QueryText=@p{count}");
            parameterValues.Add($"@p{count}='{_queryText}'");
            count++;

            // EntityLogicalName
            parameters.Add($"EntityLogicalName=@p{count}");
            parameterValues.Add($"@p{count}='{_entityLogicalName}'");
            count++;

            //QueryParameters
            if (_queryParameters != null) {

                parameters.Add($"QueryParameters=@p{count}");
                parameterValues.Add($"@p{count}={JsonConvert.SerializeObject(_queryParameters)}");
                count++;
            }

            //PageSize
            if(_pageSize.HasValue)
            {
                parameters.Add($"PageSize=@p{count}");
                parameterValues.Add($"@p{count}={_pageSize}");
                count++;
            }

            //PagingCookie
            if (!string.IsNullOrWhiteSpace(_pagingCookie))
            {
                parameters.Add($"PagingCookie=@p{count}");

                parameterValues.Add($"@p{count}='{_pagingCookie}'");
                count++;
            }

            //PartitionId
            if (!string.IsNullOrWhiteSpace(_partitionId))
            {
                parameters.Add($"PartitionId=@p{count}");
                parameterValues.Add($"@p{count}='{_partitionId}'");
            }


            StringBuilder sb = new("ExecuteCosmosSqlQuery");

            if (parameters.Count > 0)
            {
                sb.Append($"({string.Join(",", parameters)})?");
                sb.Append(string.Join("&", parameterValues));

            }

            RequestUri = new Uri(uriString: sb.ToString(), uriKind: UriKind.Relative);

        }
    }
}