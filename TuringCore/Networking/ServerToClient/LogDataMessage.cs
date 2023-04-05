using System.Text.Json.Serialization;

namespace TuringCore.Networking
{
    public class LogDataMessage : RequestHeader
    {
        [JsonInclude]
        public string LogMessage;
    }
}
