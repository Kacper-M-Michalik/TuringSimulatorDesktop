using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TuringCore
{
    [Serializable]
    public class FolderDataMessage : RequestHeader
    {
        [JsonInclude]
        public int ID;
        [JsonInclude]
        public string Name;
        [JsonInclude]
        public List<FolderDataMessage> ParentFolders;
        [JsonInclude]
        public List<FolderDataMessage> SubFolders;
        [JsonInclude]
        public List<FileDataMessage> Files;
    }
}
