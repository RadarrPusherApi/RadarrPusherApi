using RadarrPusherApi.Pusher.Api.Receivers.Interfaces;

namespace RadarrPusherApi.WorkerService.Windows
{
    public class Worker : BackgroundService
    {
        public Worker(IConfiguration configuration,
            ICloudinaryReceiver cloudinaryReceiver,
            IWorkerReceiver workerServiceReceiver,
            IMovieReceiver movieReceiver)
        {
            var pusherAppId = configuration.GetSection("PusherAppId").Value;
            var pusherKey = configuration.GetSection("PusherKey").Value;
            var pusherSecret = configuration.GetSection("PusherSecret").Value;
            var pusherCluster = configuration.GetSection("PusherCluster").Value;

            workerServiceReceiver.ConnectGetWorkerServiceVersionCommander(pusherAppId, pusherKey, pusherSecret, pusherCluster);
            movieReceiver.ConnectGetMoviesCommander(pusherAppId, pusherKey, pusherSecret, pusherCluster);
            cloudinaryReceiver.ConnectDeleteCloudinaryFileCommander(pusherAppId, pusherKey, pusherSecret, pusherCluster);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}