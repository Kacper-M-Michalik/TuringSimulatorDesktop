using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TuringSimulatorDesktop.Files
{
    [Serializable]
    public class LocalUserData
    {
        [JsonInclude]
        public List<FileInfoWrapper> RecentlyAccessedFiles = new List<FileInfoWrapper>();        
    }
}
