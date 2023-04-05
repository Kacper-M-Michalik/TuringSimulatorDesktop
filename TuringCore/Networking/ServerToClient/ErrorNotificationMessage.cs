using System.Text.Json.Serialization;

namespace TuringCore.Networking
{
    public class ErrorNotificationMessage : RequestHeader
    {
        [JsonInclude]
        public string ErrorMessage;
    }
}
