using Autofac;
using RadarrPusherApi.Cloudinary.Api;
using RadarrPusherApi.Common.Command.Implementations;
using RadarrPusherApi.Common.Command.Interfaces;
using RadarrPusherApi.Common.Logger.Implementations;
using RadarrPusherApi.Common.Models;
using RadarrPusherApi.Pusher.Api.Receivers.Implementations;
using RadarrPusherApi.Pusher.Api.Receivers.Interfaces;
using RadarrPusherApi.Pusher.Api.Services.Implementations;
using RadarrPusherApi.Pusher.Api.Services.Interfaces;

namespace RadarrPusherApi.WebApi
{
    public class AutofacConfig
    {
        public static void Configure(IConfiguration configuration, ContainerBuilder builder)
        {
            var cloudinaryCloudName = configuration.GetValue<string>("Cloudinary:CloudName");
            var cloudinaryApiKey = configuration.GetValue<string>("Cloudinary:ApiKey");
            var cloudinaryApiSecret = configuration.GetValue<string>("Cloudinary:ApiSecret");
            var pusherAppId = configuration.GetValue<string>("Pusher:AppId");
            var pusherKey = configuration.GetValue<string>("Pusher:Key");
            var pusherSecret = configuration.GetValue<string>("Pusher:Secret");
            var pusherCluster = configuration.GetValue<string>("Pusher:Cluster");

            builder.Register(c => new CloudinaryClient(cloudinaryCloudName, cloudinaryApiKey, cloudinaryApiSecret)).As<ICloudinaryClient>().SingleInstance();
            builder.Register(c => new PusherSettings(pusherAppId, pusherKey, pusherSecret, pusherCluster)).As<IPusherSettings>().SingleInstance();

            builder.Register(c => new Logger(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RadarrPusherApi.WebApi.SQLite.db3"))).As<Common.Logger.Interfaces.ILogger>().SingleInstance();
            builder.RegisterType<Invoker>().As<IInvoker>().SingleInstance();

            builder.RegisterType<WorkerConnector>().As<IWorkerConnector>().InstancePerDependency();

            builder.RegisterType<CloudinaryService>().As<ICloudinaryService>().InstancePerDependency();
            builder.RegisterType<WorkerService>().As<IWorkerService>().InstancePerDependency();
            builder.RegisterType<MoviesService>().As<IMoviesService>().InstancePerDependency();

        }
    }
}
