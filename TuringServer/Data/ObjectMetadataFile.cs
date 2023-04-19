using System;
using System.Text;
using System.Text.Json.Serialization;
using TuringCore;

namespace TuringServer.Data
{
    //Class + JSON Template
    public class ObjectMetadataFile
    {
        [JsonInclude]
        public string FileName;
        [JsonInclude]
        public Guid FileGUID;
        [JsonInclude]
        public CoreFileType FileType;
    }
}
