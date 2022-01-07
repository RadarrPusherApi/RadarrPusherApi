using SQLite;

namespace RadarrPusherApi.Common.Models
{
    [Table("Setting")]
	public class Setting
	{
        public string PusherAppId { get; set; }
        
        public string PusherKey { get; set; }
        
        public string PusherSecret { get; set; }
        
        public string PusherCluster { get; set; }
    }
}

