using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TuringCore.Networking
{
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
