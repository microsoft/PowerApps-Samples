using Microsoft.Xrm.Sdk;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Provides access to the attributes of the entity returned by ExecuteCosmosSqlQueryRequest
    /// </summary>
    public class ExecuteCosmosSqlQueryResponse : Entity
    {
        [AttributeLogicalName("PagingCookie")]
        public string? PagingCookie
        {
            get => GetAttributeValue<string?>("PagingCookie");
            set => SetAttributeValue("PagingCookie", value);
        }

        [AttributeLogicalName("HasMore")]
        public bool HasMore
        {
            get => GetAttributeValue<bool>("HasMore");
            set => SetAttributeValue("HasMore", value);
        }

        [AttributeLogicalName("Result")]
        public string? Result
        {
            get => GetAttributeValue<string?>("Result");
            set => SetAttributeValue("Result", value);
        }
    }
}
