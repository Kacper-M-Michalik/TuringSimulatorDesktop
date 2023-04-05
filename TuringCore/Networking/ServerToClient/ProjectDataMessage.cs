using System.Text.Json.Serialization;

namespace TuringCore.Networking
{
    public class ProjectDataMessage : RequestHeader
    {
        [JsonInclude]
        public string ProjectName;
        [JsonInclude]
        public TuringProjectType ProjectType;
    }
}
