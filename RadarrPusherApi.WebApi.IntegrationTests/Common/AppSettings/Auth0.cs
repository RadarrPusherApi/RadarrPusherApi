namespace RadarrPusherApi.WebApi.IntegrationTests.Common.AppSettings
{
    public class Auth0
    {
        public string Domain { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Audience { get; set; }
        public string GrantType { get; set; }
    }
}
