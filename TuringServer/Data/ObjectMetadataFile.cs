using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TuringCore;

namespace TuringServer
{
    [Serializable]
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
