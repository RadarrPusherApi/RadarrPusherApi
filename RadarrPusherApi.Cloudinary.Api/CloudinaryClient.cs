using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using RestSharp;
using System.Text;

namespace RadarrPusherApi.Cloudinary.Api
{
    public class CloudinaryClient : ICloudinaryClient
    {
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;
        
        public CloudinaryClient(string cloudName, string apiKey, string apiSecret)
        {
            _cloudinary = new CloudinaryDotNet.Cloudinary(new Account(cloudName, apiKey, apiSecret));
        }

        public async Task<string> UploadRawFile(string json, string publicId)
        {
            var byteArray = Encoding.ASCII.GetBytes(json);
            var stream = new MemoryStream(byteArray);

            var rawUploadParams = new RawUploadParams
            {
                File = new FileDescription(publicId, stream),
                PublicId = publicId,
                Overwrite = true
            };

            var rawUploadResult = await _cloudinary.UploadAsync(rawUploadParams);

            if (rawUploadResult.Error != null)
            {
                throw new Exception(rawUploadResult.Error.Message);
            }

            return rawUploadResult.Url.AbsoluteUri;
        }

        public async Task<string> DownloadRawFile(string url)
        {
            var restClient = new RestClient();
            var request = new RestRequest(url);
            var response = await restClient.ExecuteGetAsync(request);
            return response.Content;
        }

        public async Task DeleteRawFile(string publicId)
        {
            var delResResult = await _cloudinary.DeleteResourcesAsync(ResourceType.Raw, publicId);
            if (delResResult.Error != null)
            {
                throw new Exception(delResResult.Error.Message);
            }
        }
    }
}
