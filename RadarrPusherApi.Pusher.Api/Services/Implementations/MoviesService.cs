using Newtonsoft.Json;
using NzbDrone.Core.Movies;
using RadarrPusherApi.Cloudinary.Api;
using RadarrPusherApi.Common.Enums;
using RadarrPusherApi.Common.Logger.Interfaces;
using RadarrPusherApi.Pusher.Api.Models;
using RadarrPusherApi.Pusher.Api.Receivers.Interfaces;
using RadarrPusherApi.Pusher.Api.Services.Interfaces;
using System.Diagnostics;

namespace RadarrPusherApi.Pusher.Api.Services.Implementations
{
    public class MoviesService : IMoviesService
    {
        private readonly ILogger _logger;
        private readonly IWorkerReceiver _workerReceiver;
        private readonly ICloudinaryClient _cloudinaryClient;
        private readonly ICloudinaryService _cloudinaryService;

        public MoviesService(ILogger logger, IWorkerReceiver workerReceiver, ICloudinaryClient cloudinaryClient, ICloudinaryService cloudinaryService)
        {
            _logger = logger;
            _workerReceiver = workerReceiver;
            _cloudinaryClient = cloudinaryClient;
            _cloudinaryService = cloudinaryService;
        }

        /// <summary>
        /// Returns the movies from Radarr.
        /// </summary>
        /// <returns>Returns a list of Movies</returns>
        public async Task<IList<Movie>> GetMoviesServiceAsync(string pusherAppId, string pusherKey, string pusherSecret, string pusherCluster)
        {
            IList<Movie> movies = null;

            var chanelGuid = Guid.NewGuid();
            var channelNameReceive = $"{ CommandType.GetMoviesCommand }{ PusherChannel.ApiChannel}_{chanelGuid}";
            var eventNameReceive = $"{ CommandType.GetMoviesCommand }{ PusherEvent.ApiEvent}";
            var channelNameSend = $"{ CommandType.GetMoviesCommand }{ PusherChannel.WorkerServiceChannel}";
            var eventNameSend = $"{ CommandType.GetMoviesCommand }{ PusherEvent.WorkerServiceEvent}";

            try
            {
                await _workerReceiver.ConnectWorker(channelNameReceive, eventNameReceive, pusherAppId, pusherKey, pusherSecret, pusherCluster);
                
                var pusherSendMessage = new PusherSendMessageModel { Command = CommandType.GetMoviesCommand, SendMessageChanelGuid = chanelGuid.ToString() };
                await _workerReceiver.SendMessage(channelNameSend, eventNameSend, false, JsonConvert.SerializeObject(pusherSendMessage), pusherAppId, pusherKey, pusherSecret, pusherCluster);

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                while (!_workerReceiver.CommandCompleted || stopwatch.ElapsedMilliseconds > _workerReceiver.TimeLimit.TotalMilliseconds)
                {
                    if (stopwatch.ElapsedMilliseconds > _workerReceiver.TimeLimit.TotalMilliseconds)
                    {
                        stopwatch.Stop();
                        throw new Exception("Get movies took too long!");
                    }
                }

                if (_workerReceiver.CommandCompleted)
                {
                    if (string.IsNullOrWhiteSpace(_workerReceiver.ReturnData))
                    {
                        throw new Exception("Get movies has no return data!");
                    }

                    var responseContent = await _cloudinaryClient.DownloadRawFile(_workerReceiver.ReturnData);

                    await _cloudinaryService.DeleteCloudinaryRawFile(pusherAppId, pusherKey, pusherSecret, pusherCluster, _workerReceiver.CloudinaryPublicId);

                    if (string.IsNullOrWhiteSpace(responseContent))
                    {
                        throw new Exception("Get movies cloudinary response has no return data!");
                    }

                    movies = JsonConvert.DeserializeObject<IList<Movie>>(responseContent);
                }
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync(ex.Message, ex.StackTrace);
            }
            finally
            {
                await _workerReceiver.DisconnectWorker();
            }

            return movies;
        }
    }
}