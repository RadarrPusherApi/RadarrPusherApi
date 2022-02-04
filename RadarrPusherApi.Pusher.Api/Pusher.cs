using RadarrPusherApi.Cloudinary.Api;
using RadarrPusherApi.Common.Command.Interfaces;
using RadarrPusherApi.Common.Logger.Interfaces;
using RadarrPusherApi.Common.Models;

namespace RadarrPusherApi.Pusher.Api
{
    public class Pusher
    {
        private readonly ILogger _logger;
        private readonly IInvoker _invoker;
        private readonly ICloudinaryClient _cloudinaryClient;
        private readonly IPusherSettings _pusherSettings;

        public Pusher(ILogger logger, IInvoker invoker, ICloudinaryClient cloudinaryClient, IPusherSettings pusherSettings)
        {
            _logger = logger;
            _invoker = invoker;
            _cloudinaryClient = cloudinaryClient;
            _pusherSettings = pusherSettings;

            if (string.IsNullOrWhiteSpace(_pusherSettings.PusherAppId) || string.IsNullOrWhiteSpace(_pusherSettings.PusherKey) || string.IsNullOrWhiteSpace(_pusherSettings.PusherSecret) || string.IsNullOrWhiteSpace(_pusherSettings.PusherCluster))
            {
                throw new Exception("All the Pusher settings not supplied.");
            }
        }

        /// <summary>
        /// Send a message to the Pusher Pub/Sub to a specific channel and event.
        /// </summary>
        /// <param name="channelName">The channel name to connect to</param>
        /// <param name="eventName">The event name to connect to</param>
        /// <param name="message">The message to send</param>
        /// <param name="appId">The Pusher app id</param>
        /// <param name="key">The Pusher key</param>
        /// <param name="secret">The Pusher secret</param>
        /// <param name="cluster">The Pusher cluster</param>
        /// <returns></returns>
        public async Task SendMessage(string channelName, string eventName, bool useCloudinary, string message)
        {
            var pusherSend = new PusherServer.Pusher(_pusherSettings.PusherAppId, _pusherSettings.PusherKey, _pusherSettings.PusherSecret, new PusherServer.PusherOptions { Cluster = _pusherSettings.PusherCluster });

            if (useCloudinary)
            {
                var cloudinaryPublicId = Guid.NewGuid().ToString();
                var url = await _cloudinaryClient.UploadRawFile(message, cloudinaryPublicId);
                await pusherSend.TriggerAsync(channelName, eventName, new { Message = url, CloudinaryPublicId = cloudinaryPublicId });
            }
            else
            {
                await pusherSend.TriggerAsync(channelName, eventName, new { Message = message, CloudinaryPublicId = string.Empty });
            }
        }

        /// <summary>
        /// Command to send a message to the Pusher Pub/Sub to a specific channel and event.
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="channelName">The channel name to connect to</param>
        /// <param name="eventName">The event name to connect to</param>
        /// <param name="appId">The Pusher app id</param>
        /// <param name="key">The Pusher key</param>
        /// <param name="secret">The Pusher secret</param>
        /// <param name="cluster">The Pusher cluster</param>
        /// <returns></returns>
        public async Task ExecuteCommand(ICommand command, string channelName, string eventName)
        {
            try
            {
                var commandObject = await _invoker.Invoke(command);

                if (commandObject.SendMessage)
                {
                    await SendMessage(channelName, eventName, true, commandObject.Message);
                }
            }
            catch (Exception e)
            {
                await _logger.LogErrorAsync(e.Message, e.StackTrace);
            }
        }
    }
}
