using RadarrPusherApi.Pusher.Api.IntegrationTests.Common.Helpers;
using RadarrPusherApi.Pusher.Api.Receivers.Implementations;
using RadarrPusherApi.Pusher.Api.Services.Implementations;
using System;
using System.Threading.Tasks;
using Xunit;

namespace RadarrPusherApi.Pusher.Api.IntegrationTests
{
    [Collection(nameof(CommonHelper))]
    public class WorkerTests
    {
        private readonly CommonHelper _commonHelper;
        private readonly CloudinaryReceiver _cloudinaryReceiver;
        private readonly WorkerReceiver _workerReceiver;

        public WorkerTests(CommonHelper commonHelper)
        {
            _commonHelper = commonHelper;
            _cloudinaryReceiver = new CloudinaryReceiver(commonHelper.Logger, commonHelper.Invoker, commonHelper.CloudinaryClient);
            _workerReceiver = new WorkerReceiver(commonHelper.Logger, commonHelper.Invoker, commonHelper.CloudinaryClient);
        }

        [Fact]
        public async Task GetWorkerServiceVersion()
        {
            // Arrange
            await _cloudinaryReceiver.ConnectDeleteCloudinaryFileCommander(_commonHelper.Settings.PusherAppId, _commonHelper.Settings.PusherKey, _commonHelper.Settings.PusherSecret, _commonHelper.Settings.PusherCluster);
            await _workerReceiver.ConnectGetWorkerServiceVersionCommander(_commonHelper.Settings.PusherAppId, _commonHelper.Settings.PusherKey, _commonHelper.Settings.PusherSecret, _commonHelper.Settings.PusherCluster);
            var cloudinaryService = new CloudinaryService(_commonHelper.Logger, _commonHelper.WorkerReceiver);
            var workerService = new WorkerService(_commonHelper.Logger, _commonHelper.WorkerReceiver, _commonHelper.CloudinaryClient, cloudinaryService);

            // Act
            var version = await workerService.GetWorkerServiceVersionServiceAsync(_commonHelper.Settings.PusherAppId, _commonHelper.Settings.PusherKey, _commonHelper.Settings.PusherSecret, _commonHelper.Settings.PusherCluster);

            // Assert
            Assert.NotNull(version);
            Assert.Equal(new Version("1.0.0.0"), version);
        }
    }
}