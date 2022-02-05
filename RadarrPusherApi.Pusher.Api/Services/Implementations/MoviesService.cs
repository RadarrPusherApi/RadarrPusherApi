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
        private readonly IWorkerConnector _workerConnector;
        private readonly ICloudinaryClient _cloudinaryClient;
        private readonly ICloudinaryService _cloudinaryService;

        private readonly string _channelNameReceive;
        private readonly string _eventNameReceive;
        private readonly string _channelNameSend;
        private readonly string _eventNameSend;

        public MoviesService(ILogger logger, IWorkerConnector workerConnector, ICloudinaryClient cloudinaryClient, ICloudinaryService cloudinaryService)
        {
            _logger = logger;
            _workerConnector = workerConnector;
            _cloudinaryClient = cloudinaryClient;
            _cloudinaryService = cloudinaryService;

            _channelNameReceive =PusherChannel.ApiChannel.ToString();
            _eventNameReceive = PusherEvent.ApiEvent.ToString();
            _channelNameSend = PusherChannel.WorkerServiceChannel.ToString();
            _eventNameSend = PusherEvent.WorkerServiceEvent.ToString();
        }

        /// <summary>
        /// Returns the movies from Radarr.
        /// </summary>
        /// <returns>Returns a list of Movies</returns>
        public async Task<IList<Movie>> GetMoviesServiceAsync()
        {
            IList<Movie> movies = null;

            var chanelGuid = Guid.NewGuid();

            try
            {
                await _workerConnector.ConnectWorker(_channelNameReceive, $"{_eventNameReceive}_{chanelGuid}");

                var pusherSendMessage = new PusherSendMessageModel { Command = CommandType.GetMoviesCommand, SendMessageChanelGuid = chanelGuid.ToString() };
                await _workerConnector.SendMessage(_channelNameSend, $"{_eventNameSend}_{CommandType.GetMoviesCommand}", false, JsonConvert.SerializeObject(pusherSendMessage));

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                while (!_workerConnector.CommandCompleted || stopwatch.ElapsedMilliseconds > _workerConnector.TimeLimit.TotalMilliseconds)
                {
                    if (stopwatch.ElapsedMilliseconds > _workerConnector.TimeLimit.TotalMilliseconds)
                    {
                        stopwatch.Stop();
                        throw new Exception("Get movies took too long!");
                    }
                }

                if (_workerConnector.CommandCompleted)
                {
                    if (string.IsNullOrWhiteSpace(_workerConnector.ReturnData))
                    {
                        throw new Exception("Get movies has no return data!");
                    }

                    var responseContent = await _cloudinaryClient.DownloadRawFile(_workerConnector.ReturnData);

                    await _cloudinaryService.DeleteCloudinaryRawFile(_workerConnector.CloudinaryPublicId);

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
                await _workerConnector.DisconnectWorker();
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

            try
            {
                await _workerConnector.ConnectWorker(_channelNameReceive, $"{_eventNameReceive}_{chanelGuid}");

                var pusherSendMessage = new PusherSendMessageModel { Command = CommandType.GetMovieCommand, SendMessageChanelGuid = chanelGuid.ToString(), Values = JsonConvert.SerializeObject(id)};
                await _workerConnector.SendMessage(_channelNameSend, $"{_eventNameSend}_{CommandType.GetMovieCommand}", false, JsonConvert.SerializeObject(pusherSendMessage));

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                while (!_workerConnector.CommandCompleted || stopwatch.ElapsedMilliseconds > _workerConnector.TimeLimit.TotalMilliseconds)
                {
                    if (stopwatch.ElapsedMilliseconds > _workerConnector.TimeLimit.TotalMilliseconds)
                    {
                        stopwatch.Stop();
                        throw new Exception("Get movies took too long!");
                    }
                }

                if (_workerConnector.CommandCompleted)
                {
                    if (string.IsNullOrWhiteSpace(_workerConnector.ReturnData))
                    {
                        throw new Exception("Get movies has no return data!");
                    }

                    var responseContent = await _cloudinaryClient.DownloadRawFile(_workerConnector.ReturnData);

                    await _cloudinaryService.DeleteCloudinaryRawFile(_workerConnector.CloudinaryPublicId);

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
                await _workerConnector.DisconnectWorker();
            }

            return movie;
        }
    }
}