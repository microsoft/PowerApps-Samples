namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to perform the FormatAddress function
    /// </summary>
    public sealed class FormatAddressRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes the FormatAddressRequest
        /// </summary>
        /// <param name="line1">The first line of the address.</param>
        /// <param name="city">The city of the address.</param>
        /// <param name="stateOrProvince">The state or province of the address.</param>
        /// <param name="postalCode">The postal code of the address.</param>
        /// <param name="country">The postal code of the address.</param>
        public FormatAddressRequest(
            string line1, 
            string city, 
            string stateOrProvince, 
            string postalCode, 
            string country)
        {
            Method = HttpMethod.Get;
            RequestUri = new Uri(
                uriString: $"FormatAddress(Line1=@p1,City=@p2,StateOrProvince=@p3,PostalCode=@p4,Country=@p5)" +
                $"?@p1='{line1}'" +
                $"&@p2='{city}'" +
                $"&@p3='{stateOrProvince}'" +
                $"&@p4='{postalCode}'" +
                $"&@p5='{country}'",
                uriKind: UriKind.Relative);
        }
    }
}
