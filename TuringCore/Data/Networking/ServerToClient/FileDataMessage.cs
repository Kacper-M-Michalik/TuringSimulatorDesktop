using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TuringCore
{
    [Serializable]
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

        /*
        public FileDataMessage(Guid SetGUID, CoreFileType SetType, string SetName, int SetVersion, byte[] SetData)
        {
            Name = SetName;
            GUID = SetGUID;
            Type = SetType;
            Version = SetVersion;
            Data = SetData;
        }
        */
    }
}
