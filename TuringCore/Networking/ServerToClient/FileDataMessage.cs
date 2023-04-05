using System;
using System.Text.Json.Serialization;

namespace TuringCore.Networking
{
    public class FileDataMessage : RequestHeader
    {
        [JsonInclude]
        public Guid GUID;
        [JsonInclude]
        public CoreFileType FileType;
        [JsonInclude]
        public string Name;
        [JsonInclude]
        public int Version;
        [JsonInclude]
        public byte[] Data;
    }
}
