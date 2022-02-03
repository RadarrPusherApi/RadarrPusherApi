namespace RadarrPusherApi.Pusher.Api.Services.Interfaces
{
    public interface ICloudinaryService
    {
        /// <summary>
        /// Delete the Cloudinary file by public id.
        /// </summary>
        /// <returns></returns>
        Task DeleteCloudinaryRawFile(string pusherAppId, string pusherKey, string pusherSecret, string pusherCluster, string publicId);
    }
}
