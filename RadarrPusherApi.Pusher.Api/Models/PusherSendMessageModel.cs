using RadarrPusherApi.Common.Enums;

namespace RadarrPusherApi.Pusher.Api.Models
{
    public class PusherSendMessageModel
    {
        public CommandType Command { get; set; }
        public string SendMessageChanelGuid { get; set; }
        public string Values { get; set; }
    }
}