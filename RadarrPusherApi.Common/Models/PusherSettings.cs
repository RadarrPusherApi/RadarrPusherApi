namespace RadarrPusherApi.Common.Models
{
    public class PusherSettings : IPusherSettings
    {
        public string PusherAppId { get; }
        public string PusherKey { get; }
        public string PusherSecret { get; }
        public string PusherCluster { get; }

        public PusherSettings(string pusherAppId, string pusherKey, string pusherSecret, string pusherCluster)
        {
            PusherAppId = pusherAppId;
            PusherKey = pusherKey;
            PusherSecret = pusherSecret;
            PusherCluster = pusherCluster;

            if (string.IsNullOrWhiteSpace(PusherAppId) || string.IsNullOrWhiteSpace(PusherKey) || string.IsNullOrWhiteSpace(PusherSecret) || string.IsNullOrWhiteSpace(PusherCluster))
            {
                throw new Exception("All the Pusher settings not supplied.");
            }
        }
    }
}
