namespace RadarrPusherApi.Cloudinary.Api
{
    public interface ICloudinaryClient
    {
        Task<string> UploadRawFile(string json, string publicId);
        Task<string> DownloadRawFile(string url);
        Task DeleteRawFile(string publicId);
    }
}