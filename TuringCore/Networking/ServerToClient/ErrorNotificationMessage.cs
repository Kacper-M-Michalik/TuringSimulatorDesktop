using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TuringCore.Networking
{
    [Serializable]
    public class ErrorNotificationMessage : RequestHeader
    {
        [JsonInclude]
        public string ErrorMessgae;
    }
}
