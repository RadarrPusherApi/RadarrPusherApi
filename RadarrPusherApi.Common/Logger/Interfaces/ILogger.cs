using RadarrPusherApi.Common.Models;

namespace RadarrPusherApi.Common.Logger.Interfaces
{
    public interface ILogger
    {
        /// <summary>
        /// Get all the logs.
        /// </summary>
        /// <returns>Return a list of Logs</returns>
        Task<List<Log>> GetLogsAsync();

        /// <summary>
        /// Log a warning.
        /// </summary>
        /// <param name="logMessage">The log message</param>
        /// <returns>Returns id</returns>
        Task LogWarnAsync(string logMessage);

        /// <summary>
        /// Log a error.
        /// </summary>
        /// <param name="logMessage">The log message</param>
        /// <param name="stackTrace">The stack trace</param>
        /// <returns>Returns a id</returns>
        Task LogErrorAsync(string logMessage, string stackTrace);
    }
}