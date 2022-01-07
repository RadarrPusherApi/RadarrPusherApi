namespace RadarrPusherApi.Common.Enums
{
    /// <summary>
    /// The different channels that is used by Pusher. Each channel is specific to a different process.
    /// </summary>
    public enum PusherChannel
    {
        ApiChannel,
        WorkerServiceChannel
    }
}
