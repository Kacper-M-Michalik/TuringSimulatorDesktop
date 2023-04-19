using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TuringSimulatorDesktop.Files
{
    [Serializable]
    public class FileInfoWrapper
    {
        [JsonInclude]
        public string FileName;
        [JsonInclude]
        public string FullPath;
        [JsonInclude]
        public DateTime LastAccessed;

        public FileInfoWrapper(string fileName, string fullPath, DateTime lastAccessed)
        {
            FileName = fileName;
            FullPath = fullPath;
            LastAccessed = lastAccessed;
        }
    }
}
