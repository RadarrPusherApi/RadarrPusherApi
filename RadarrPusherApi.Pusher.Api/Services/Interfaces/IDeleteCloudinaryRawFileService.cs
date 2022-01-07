using RadarrPusherApi.Common.Models;

namespace RadarrPusherApi.Pusher.Api.Services.Interfaces
{
    public interface IDeleteCloudinaryRawFileService
    {
        /// <summary>
        /// Delete the cloudinary raw file by public id.
        /// </summary>
        /// <returns></returns>
        Task DeleteCloudinaryRawFile(Setting setting, string publicId);
    }
}
