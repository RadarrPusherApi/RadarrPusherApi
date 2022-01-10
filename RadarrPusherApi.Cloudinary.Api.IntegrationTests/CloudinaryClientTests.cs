using System;
using System.Threading.Tasks;
using RadarrPusherApi.Cloudinary.Api.IntegrationTests.Common.Helpers;
using Xunit;

namespace RadarrPusherApi.Cloudinary.Api.IntegrationTests
{
    [Collection(nameof(CommonHelper))]
    public class CloudinaryClientTests
    {
        private readonly CommonHelper _commonHelper;
        private readonly ICloudinaryClient _cloudinaryClient;

        public CloudinaryClientTests(CommonHelper commonHelper)
        {
            _commonHelper = commonHelper;
            _cloudinaryClient = new CloudinaryClient(commonHelper.Settings.CloudinaryCloudName, commonHelper.Settings.CloudinaryApiKey, commonHelper.Settings.CloudinaryApiSecret);
        }

        [Fact]
        public async Task CloudinaryClient()
        {
            // Arrange
            var json = "{\"Command\":1,\"SendMessageChanelGuid\":\"99024813-7075-4b03-858b-ecdcd9ade2fc\",\"Values\":null}";
            var cloudinaryPublicId = Guid.NewGuid().ToString();

            // Act: UploadRawFile
            var url = await _cloudinaryClient.UploadRawFile(json, cloudinaryPublicId);

            // Assert: UploadRawFile
            Assert.False(string.IsNullOrWhiteSpace(url));

            // Act: DownloadRawFile
            var responseContent = await _cloudinaryClient.DownloadRawFile(url);

            // Assert: UploadRawFile
            Assert.Equal(json, responseContent);

            // Act: DeleteRawFile
            await _cloudinaryClient.DeleteRawFile(cloudinaryPublicId);
        }
    }
}