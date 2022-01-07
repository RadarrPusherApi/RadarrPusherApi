using RadarrPusherApi.Cloudinary.Api;
using RadarrPusherApi.Common.Command.Interfaces;
using RadarrPusherApi.Common.Models;

namespace RadarrPusherApi.Common.Command.Implementations.Commands
{
    public class DeleteCloudinaryRawFileCommand : ICommand
    {
        private readonly ICloudinaryClient _cloudinaryClient;
        private readonly string _publicId;

        public DeleteCloudinaryRawFileCommand(ICloudinaryClient cloudinaryClient, string publicId)
        {
            _cloudinaryClient = cloudinaryClient;
            _publicId = publicId;
        }

        /// <summary>
        /// Delete the uploaded Cloudinary file by public id.
        /// </summary>
        /// <returns>CommandObject</returns>
        public async Task<CommandObject> Execute()
        {
            await _cloudinaryClient.DeleteRawFile(_publicId);

            return new CommandObject();
        }
    }
}