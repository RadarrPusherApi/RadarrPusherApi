using RadarrPusherApi.Pusher.Api.IntegrationTests.Common.Helpers;
using RadarrPusherApi.Pusher.Api.Receivers.Implementations;
using RadarrPusherApi.Pusher.Api.Services.Implementations;
using System;
using System.Threading.Tasks;
using Xunit;

namespace RadarrPusherApi.Pusher.Api.IntegrationTests
{
    [Collection(nameof(CommonHelper))]
    public class GetWorkerServiceVersionTests
    {
        private readonly CommonHelper _commonHelper;
        private readonly DeleteCloudinaryRawFileCommandReceiver _deleteCloudinaryRawFileCommandReceiver;
        private readonly GetWorkerServiceVersionCommandReceiver _getWorkerServiceVersionCommandReceiver;

        public GetWorkerServiceVersionTests(CommonHelper commonHelper)
        {
            _commonHelper = commonHelper;
            _deleteCloudinaryRawFileCommandReceiver = new DeleteCloudinaryRawFileCommandReceiver(commonHelper.Logger, commonHelper.Invoker, commonHelper.CloudinaryClient);
            _getWorkerServiceVersionCommandReceiver = new GetWorkerServiceVersionCommandReceiver(commonHelper.Logger, commonHelper.Invoker, commonHelper.CloudinaryClient);
        }

        [Fact]
        public async Task GetWorkerServiceVersion()
        {
            // Arrange
            await _deleteCloudinaryRawFileCommandReceiver.Connect(_commonHelper.Settings.PusherAppId, _commonHelper.Settings.PusherKey, _commonHelper.Settings.PusherSecret, _commonHelper.Settings.PusherCluster);
            await _getWorkerServiceVersionCommandReceiver.Connect(_commonHelper.Settings.PusherAppId, _commonHelper.Settings.PusherKey, _commonHelper.Settings.PusherSecret, _commonHelper.Settings.PusherCluster);
            var deleteCloudinaryRawFileService = new DeleteCloudinaryRawFileService(_commonHelper.Logger, _commonHelper.WorkerServiceReceiver);
            var getWorkerServiceVersionService = new GetWorkerServiceVersionService(_commonHelper.Logger, _commonHelper.WorkerServiceReceiver, _commonHelper.CloudinaryClient, deleteCloudinaryRawFileService);

            // Act
            var version = await getWorkerServiceVersionService.GetWorkerServiceVersionServiceAsync(_commonHelper.Settings.PusherAppId, _commonHelper.Settings.PusherKey, _commonHelper.Settings.PusherSecret, _commonHelper.Settings.PusherCluster);

            // Assert
            Assert.NotNull(version);
            Assert.Equal(new Version("1.0.0.0"), version.Version);
        }
    }
}