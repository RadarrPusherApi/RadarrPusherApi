using RadarrPusherApi.Cloudinary.Api;
using RadarrPusherApi.Common.Command.Interfaces;
using RadarrPusherApi.Common.Logger.Interfaces;

namespace RadarrPusherApi.Pusher.Api
{
    public class Pusher
    {
        private readonly ILogger _logger;
        private readonly IInvoker _invoker;
        private readonly ICloudinaryClient _cloudinaryClient;

        public Pusher(ILogger logger, IInvoker invoker, ICloudinaryClient cloudinaryClient)
        {
            _logger = logger;
            _invoker = invoker;
            _cloudinaryClient = cloudinaryClient;
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
        public async Task SendMessage(string channelName, string eventName, bool useCloudinary, string message, string appId, string key, string secret, string cluster)
        {
            if (!string.IsNullOrWhiteSpace(appId) && !string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(secret) && !string.IsNullOrWhiteSpace(cluster))
            {
                var pusherSend = new PusherServer.Pusher(appId, key, secret, new PusherServer.PusherOptions { Cluster = cluster });

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
            else
            {
                throw new Exception("All the Pusher settings not supplied.");
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
        public async Task ExecuteCommand(ICommand command, string channelName, string eventName, string appId, string key, string secret, string cluster)
        {
            try
            {
                var commandObject = await _invoker.Invoke(command);

                if (commandObject.SendMessage)
                {
                    await SendMessage(channelName, eventName, true, commandObject.Message, appId, key, secret, cluster);
                }
            }
            catch (Exception e)
            {
                await _logger.LogErrorAsync(e.Message, e.StackTrace);
            }
        }
    }
}
