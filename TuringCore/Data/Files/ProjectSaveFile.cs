using System.Text.Json.Serialization;

namespace TuringCore.Files
{
    //Class + JSON Template
    public class ProjectSaveFile
    {
        [JsonInclude]
        public string ProjectName;
        [JsonInclude]
        public string BaseFolder;
        [JsonInclude]
        public TuringProjectType TuringTypeRule;

        //Marks that the JSON serializer should use this contructor
        [JsonConstructor]
        public ProjectSaveFile(string ProjectName, string BaseFolder, TuringProjectType TuringTypeRule)
        {
            this.ProjectName = ProjectName;
            this.BaseFolder = BaseFolder;
            this.TuringTypeRule = TuringTypeRule;
        }
    }
}
