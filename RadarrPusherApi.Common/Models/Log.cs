using RadarrPusherApi.Common.Enums;
using SQLite;

namespace RadarrPusherApi.Common.Models
{
    [Table("Log")]
	public class Log
	{
		[PrimaryKey, AutoIncrement]
        [Column("Id")]
		public int Id { get; set; }

        [Column("LogMessage")]
		public string LogMessage { get; set; }

        [Column("LogStackTrace")]
		public string LogStackTrace { get; set; }

        [Column("LogType")]
		public LogType LogType { get; set; }

        [Column("LogDate")]
		public DateTime LogDate { get; set; }
	}
}

