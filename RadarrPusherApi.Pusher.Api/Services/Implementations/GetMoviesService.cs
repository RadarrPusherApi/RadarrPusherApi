using Newtonsoft.Json;
using NzbDrone.Core.Movies;
using RadarrPusherApi.Cloudinary.Api;
using RadarrPusherApi.Common.Enums;
using RadarrPusherApi.Common.Models;
using RadarrPusherApi.Pusher.Api.Models;
using RadarrPusherApi.Pusher.Api.Receivers.Interfaces;
using RadarrPusherApi.Pusher.Api.Services.Interfaces;
using System.Diagnostics;

namespace RadarrPusherApi.Pusher.Api.Services.Implementations
{
    public class GetMoviesService : IGetMoviesService
    {
        private readonly IWorkerServiceReceiver _workerServiceReceiver;
        private readonly ICloudinaryClient _cloudinaryClient;
        private readonly IDeleteCloudinaryRawFileService _deleteCloudinaryRawFileService;

        public GetMoviesService(IWorkerServiceReceiver workerServiceReceiver, ICloudinaryClient cloudinaryClient, IDeleteCloudinaryRawFileService deleteCloudinaryRawFileService)
        {
            _workerServiceReceiver = workerServiceReceiver;
            _cloudinaryClient = cloudinaryClient;
            _deleteCloudinaryRawFileService = deleteCloudinaryRawFileService;
        }

        /// <summary>
        /// Returns the movies from Radarr.
        /// </summary>
        /// <returns>Returns a list of Movies</returns>
        public async Task<IList<Movie>> GetMoviesServiceAsync(Setting setting)
        {
            IList<Movie>? movies = null;

            var chanelGuid = Guid.NewGuid();
            var channelNameReceive = $"{ CommandType.GetMoviesCommand }{ PusherChannel.ApiChannel}_{chanelGuid}";
            var eventNameReceive = $"{ CommandType.GetMoviesCommand }{ PusherEvent.ApiEvent}";
            var channelNameSend = $"{ CommandType.GetMoviesCommand }{ PusherChannel.WorkerServiceChannel}";
            var eventNameSend = $"{ CommandType.GetMoviesCommand }{ PusherEvent.WorkerServiceEvent}";

            try
            {
                await _workerServiceReceiver.Connect(channelNameReceive, eventNameReceive, setting.PusherAppId, setting.PusherKey, setting.PusherSecret, setting.PusherCluster);
                
                var pusherSendMessage = new PusherSendMessageModel { Command = CommandType.GetMoviesCommand, SendMessageChanelGuid = chanelGuid.ToString() };
                await _workerServiceReceiver.SendMessage(channelNameSend, eventNameSend, JsonConvert.SerializeObject(pusherSendMessage), setting.PusherAppId, setting.PusherKey, setting.PusherSecret, setting.PusherCluster);

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                while (!_workerServiceReceiver.CommandCompleted || stopwatch.ElapsedMilliseconds > _workerServiceReceiver.TimeLimit.TotalMilliseconds)
                {
                    if (stopwatch.ElapsedMilliseconds > _workerServiceReceiver.TimeLimit.TotalMilliseconds)
                    {
                        stopwatch.Stop();
                        throw new Exception("Get movies took too long!");
                    }
                }

                if (_workerServiceReceiver.CommandCompleted)
                {
                    if (string.IsNullOrWhiteSpace(_workerServiceReceiver.ReturnData))
                    {
                        throw new Exception("Get movies has no return data!");
                    }

                    var responseContent = await _cloudinaryClient.DownloadRawFile(_workerServiceReceiver.ReturnData);

                    await _deleteCloudinaryRawFileService.DeleteCloudinaryRawFile(setting, _workerServiceReceiver.CloudinaryPublicId);

                    if (string.IsNullOrWhiteSpace(responseContent))
                    {
                        throw new Exception("Get movies cloudinary response has no return data!");
                    }

                    movies = JsonConvert.DeserializeObject<IList<Movie>>(responseContent);
                }
            }
            finally
            {
                await _workerServiceReceiver.Disconnect();
            }

            return movies;
        }
    }
}