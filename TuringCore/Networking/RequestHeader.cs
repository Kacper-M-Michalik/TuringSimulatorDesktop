using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TuringCore.Networking
{
    [Serializable]
    public abstract class RequestHeader
    {
        [JsonInclude]
        public int RequestType;
    }
}
