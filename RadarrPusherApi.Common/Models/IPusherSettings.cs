namespace RadarrPusherApi.Common.Models
{
    public interface IPusherSettings
    {
        string PusherAppId { get; }
        string PusherKey { get; }
        string PusherSecret { get; }
        string PusherCluster { get; }
    }
}
