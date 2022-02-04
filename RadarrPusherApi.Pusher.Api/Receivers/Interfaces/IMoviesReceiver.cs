namespace RadarrPusherApi.Pusher.Api.Receivers.Interfaces
{
    public interface IMoviesReceiver
    {
        /// <summary>
        /// Connect the get movies command receiver to the Pusher Pub/Sub.
        /// </summary>
        /// <returns></returns>
        Task ConnectGetMoviesCommander();

        /// <summary>
        /// Connect the get movie command receiver to the Pusher Pub/Sub.
        /// </summary>
        /// <returns></returns>
        Task ConnectGetMovieCommander();
    }
}