﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TuringCore
{
    [Serializable]
    public class ProjectDataMessage : RequestHeader
    {
        [JsonInclude]
        public string ProjectName;
    }
}