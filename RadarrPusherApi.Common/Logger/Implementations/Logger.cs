using RadarrPusherApi.Common.Enums;
using RadarrPusherApi.Common.Logger.Interfaces;
using RadarrPusherApi.Common.Models;
using SQLite;

namespace RadarrPusherApi.Common.Logger.Implementations
{
    public class Logger : ILogger
    {
        private readonly SQLiteAsyncConnection _database;

        public Logger(string databasePath)
        {
            _database = new SQLiteAsyncConnection(databasePath);
            _database.CreateTableAsync<Log>().Wait();
        }
        
        /// <summary>
        /// Get all the logs.
        /// </summary>
        /// <returns>Return a list of Logs</returns>
        public async Task<List<Log>> GetLogsAsync()
        {
            return await _database.Table<Log>().ToListAsync();
        }

        /// <summary>
        /// Log a warning.
        /// </summary>
        /// <param name="logMessage">The log message</param>
        /// <returns>Returns id</returns>
        public async Task LogWarnAsync(string logMessage)
        {
            var record = new Log
            {
                LogMessage = logMessage,
                LogType = LogType.Warn,
                LogDate = DateTime.Now
            };

            await _database.InsertAsync(record);
        }

        /// <summary>
        /// Log a error.
        /// </summary>
        /// <param name="logMessage">The log message</param>
        /// <param name="stackTrace">The stack trace</param>
        /// <returns>Returns a id</returns>
        public async Task LogErrorAsync(string logMessage, string stackTrace)
        {
            var record = new Log
            {
                LogMessage = logMessage,
                LogStackTrace = stackTrace,
                LogType = LogType.Error,
                LogDate = DateTime.Now
            };

            await _database.InsertAsync(record);
        }
    }
}

