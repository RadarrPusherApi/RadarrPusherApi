using RadarrPusherApi.Pusher.Api.Receivers.Interfaces;

namespace RadarrPusherApi.WorkerService.Windows
{
    public class Worker : BackgroundService
    {
        public Worker(IConfiguration configuration,
            IDeleteCloudinaryRawFileCommandReceiver deleteCloudinaryRawFileCommandReceiver,
            IGetWorkerServiceVersionCommandReceiver getWorkerServiceVersionCommandReceiver,
            IGetMoviesCommandReceiver getMoviesCommandReceiver)
        {
            var pusherAppId = configuration.GetSection("PusherAppId").Value;
            var pusherKey = configuration.GetSection("PusherKey").Value;
            var pusherSecret = configuration.GetSection("PusherSecret").Value;
            var pusherCluster = configuration.GetSection("PusherCluster").Value;

            getWorkerServiceVersionCommandReceiver.Connect(pusherAppId, pusherKey, pusherSecret, pusherCluster);
            getMoviesCommandReceiver.Connect(pusherAppId, pusherKey, pusherSecret, pusherCluster);
            deleteCloudinaryRawFileCommandReceiver.Connect(pusherAppId, pusherKey, pusherSecret, pusherCluster);
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