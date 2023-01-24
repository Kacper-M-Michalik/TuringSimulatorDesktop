using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TuringCore
{
    [Serializable]
    public class SaveFile
    {
        [JsonInclude]
        public Guid FileID = Guid.Empty;
    }
}
