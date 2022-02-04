using RadarrPusherApi.Pusher.Api.Receivers.Interfaces;

namespace RadarrPusherApi.WorkerService.Windows
{
    public class Worker : BackgroundService
    {
        public Worker(ICloudinaryReceiver cloudinaryReceiver, IWorkerReceiver workerServiceReceiver, IMoviesReceiver movieReceiver)
        {
            workerServiceReceiver.ConnectGetWorkerServiceVersionCommander();
            movieReceiver.ConnectGetMoviesCommander();
            movieReceiver.ConnectGetMovieCommander();
            cloudinaryReceiver.ConnectDeleteCloudinaryFileCommander();
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