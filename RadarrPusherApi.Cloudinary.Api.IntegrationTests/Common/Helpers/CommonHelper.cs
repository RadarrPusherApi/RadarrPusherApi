using Microsoft.Extensions.Configuration;
using RadarrPusherApi.Cloudinary.Api.IntegrationTests.Common.AppSettings;

namespace RadarrPusherApi.Cloudinary.Api.IntegrationTests.Common.Helpers
{
    public class CommonHelper
    {
        public Settings Settings { get; }

        public CommonHelper()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            Settings = new Settings();
            configuration.Bind(Settings);
        }
    }
}
