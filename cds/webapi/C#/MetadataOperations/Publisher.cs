using Newtonsoft.Json;
using System;

namespace PowerApps.Samples
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Publisher
    {
        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.publisher";

        [JsonProperty("address1_addressid")]
        public Guid? Address1_AddressId { get; set; }

        [JsonProperty("address1_addresstypecode")]
        public PublisherAddress1_AddressTypeCode? Address1_AddressTypeCode { get; set; }

        [JsonProperty("address1_city")]
        public string Address1_City { get; set; }

        [JsonProperty("address1_country")]
        public string Address1_Country { get; set; }

        [JsonProperty("address1_county")]
        public string Address1_County { get; set; }

        [JsonProperty("address1_fax")]
        public string Address1_Fax { get; set; }

        [JsonProperty("address1_latitude")]
        public double? Address1_Latitude { get; set; }

        [JsonProperty("address1_line1")]
        public string Address1_Line1 { get; set; }

        [JsonProperty("address1_line2")]
        public string Address1_Line2 { get; set; }

        [JsonProperty("address1_line3")]
        public string Address1_Line3 { get; set; }

        [JsonProperty("address1_longitude")]
        public double? Address1_Longitude { get; set; }

        [JsonProperty("address1_name")]
        public string Address1_Name { get; set; }

        [JsonProperty("address1_postalcode")]
        public string Address1_PostalCode { get; set; }

        [JsonProperty("address1_postofficebox")]
        public string Address1_PostOfficeBox { get; set; }

        [JsonProperty("address1_shippingmethodcode")]
        public PublisherAddress1_ShippingMethodCode? Address1_ShippingMethodCode { get; set; }

        [JsonProperty("address1_stateorprovince")]
        public string Address1_StateOrProvince { get; set; }

        [JsonProperty("address1_telephone1")]
        public string Address1_Telephone1 { get; set; }

        [JsonProperty("address1_telephone2")]
        public string Address1_Telephone2 { get; set; }

        [JsonProperty("address1_telephone3")]
        public string Address1_Telephone3 { get; set; }

        [JsonProperty("address1_upszone")]
        public string Address1_UPSZone { get; set; }

        [JsonProperty("address1_utcoffset")]
        public int? Address1_UTCOffset { get; set; }

        [JsonProperty("address2_addressid")]
        public Guid? Address2_AddressId { get; set; }

        [JsonProperty("address2_addresstypecode")]
        public PublisherAddress2_AddressTypeCode? Address2_AddressTypeCode { get; set; }

        [JsonProperty("address2_city")]
        public string Address2_City { get; set; }

        [JsonProperty("address2_country")]
        public string Address2_Country { get; set; }

        [JsonProperty("address2_county")]
        public string Address2_County { get; set; }

        [JsonProperty("address2_fax")]
        public string Address2_Fax { get; set; }

        [JsonProperty("address2_latitude")]
        public double? Address2_Latitude { get; set; }

        [JsonProperty("address2_line1")]
        public string Address2_Line1 { get; set; }

        [JsonProperty("address2_line2")]
        public string Address2_Line2 { get; set; }

        [JsonProperty("address2_line3")]
        public string Address2_Line3 { get; set; }

        [JsonProperty("address2_longitude")]
        public double? Address2_Longitude { get; set; }

        [JsonProperty("address2_name")]
        public string Address2_Name { get; set; }

        [JsonProperty("address2_postalcode")]
        public string Address2_PostalCode { get; set; }

        [JsonProperty("address2_postofficebox")]
        public string Address2_PostOfficeBox { get; set; }

        [JsonProperty("address2_shippingmethodcode")]
        public PublisherAddress2_ShippingMethodCode? Address2_ShippingMethodCode { get; set; }

        [JsonProperty("address2_stateorprovince")]
        public string Address2_StateOrProvince { get; set; }

        [JsonProperty("address2_telephone1")]
        public string Address2_Telephone1 { get; set; }

        [JsonProperty("address2_telephone2")]
        public string Address2_Telephone2 { get; set; }

        [JsonProperty("address2_telephone3")]
        public string Address2_Telephone3 { get; set; }

        [JsonProperty("address2_upszone")]
        public string Address2_UPSZone { get; set; }

        [JsonProperty("address2_utcoffset")]
        public int? Address2_UTCOffset { get; set; }

        [JsonProperty("customizationoptionvalueprefix")]
        public int? CustomizationOptionValuePrefix { get; set; }

        [JsonProperty("customizationprefix")]
        public string CustomizationPrefix { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("emailaddress")]
        public string EMailAddress { get; set; }

        [JsonProperty("friendlyname")]
        public string FriendlyName { get; set; }

        [JsonProperty("publisherid")]
        public Guid? PublisherId { get; set; }

        [JsonProperty("supportingwebsiteurl")]
        public string SupportingWebsiteUrl { get; set; }

        [JsonProperty("uniquename")]
        public string UniqueName { get; set; }
    }

    public enum PublisherAddress1_AddressTypeCode
    {
        DefaultValue = 1
    }

    public enum PublisherAddress1_ShippingMethodCode
    {
        DefaultValue = 1
    }

    public enum PublisherAddress2_AddressTypeCode
    {
        DefaultValue = 1
    }

    public enum PublisherAddress2_ShippingMethodCode
    {
        DefaultValue = 1
    }
}