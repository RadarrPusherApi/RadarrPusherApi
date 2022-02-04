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
        public async Task<IList<Movie>> GetMoviesServiceAsync()
        {
            IList<Movie> movies = null;

            var chanelGuid = Guid.NewGuid();
            var channelNameReceive = $"{ CommandType.MoviesCommand }{ PusherChannel.ApiChannel}_{chanelGuid}";
            var eventNameReceive = $"{ CommandType.MoviesCommand }{ PusherEvent.ApiEvent}";
            var channelNameSend = $"{ CommandType.MoviesCommand }{ PusherChannel.WorkerServiceChannel}";
            var eventNameSend = $"{ CommandType.MoviesCommand }{ PusherEvent.WorkerServiceEvent}";

            try
            {
                await _workerReceiver.ConnectWorker(channelNameReceive, eventNameReceive);
                
                var pusherSendMessage = new PusherSendMessageModel { Command = CommandType.MoviesCommand, SendMessageChanelGuid = chanelGuid.ToString() };
                await _workerReceiver.SendMessage(channelNameSend, eventNameSend, false, JsonConvert.SerializeObject(pusherSendMessage));

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

                    await _cloudinaryService.DeleteCloudinaryRawFile(_workerReceiver.CloudinaryPublicId);

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

        /// <summary>
        /// Returns a movie from Radarr.
        /// </summary>
        /// <returns>Returns a Movie</returns>
        public async Task<Movie> GetMovieServiceAsync(int id)
        {
            Movie movie = null;

            var chanelGuid = Guid.NewGuid();
            var channelNameReceive = $"{ CommandType.MoviesCommand }{ PusherChannel.ApiChannel}_{chanelGuid}";
            var eventNameReceive = $"{ CommandType.MoviesCommand }{ PusherEvent.ApiEvent}";
            var channelNameSend = $"{ CommandType.MoviesCommand }{ PusherChannel.WorkerServiceChannel}";
            var eventNameSend = $"{ CommandType.MoviesCommand }{ PusherEvent.WorkerServiceEvent}";

            try
            {
                await _workerReceiver.ConnectWorker(channelNameReceive, eventNameReceive);

                var pusherSendMessage = new PusherSendMessageModel { Command = CommandType.MoviesCommand, SendMessageChanelGuid = chanelGuid.ToString(), Values = JsonConvert.SerializeObject(id)};
                await _workerReceiver.SendMessage(channelNameSend, eventNameSend, false, JsonConvert.SerializeObject(pusherSendMessage));

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

                    await _cloudinaryService.DeleteCloudinaryRawFile(_workerReceiver.CloudinaryPublicId);

                    if (string.IsNullOrWhiteSpace(responseContent))
                    {
                        throw new Exception("Get movies cloudinary response has no return data!");
                    }

                    movie = JsonConvert.DeserializeObject<Movie>(responseContent);
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

            return movie;
        }
    }
}