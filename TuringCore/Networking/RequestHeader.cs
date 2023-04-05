using System.Text.Json.Serialization;

namespace TuringCore.Networking
{
    public abstract class RequestHeader
    {
        [JsonInclude]
        public int RequestType;
    }
}
