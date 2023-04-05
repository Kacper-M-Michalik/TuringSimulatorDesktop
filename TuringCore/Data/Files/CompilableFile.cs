using System;
using System.Text.Json.Serialization;
using TuringCore.Systems;

namespace TuringCore.Files
{
    //Class + JSON Template
    public abstract class CompilableFile
    {
        //The JsonIncludeAttribute Marks that this variable can be serialized by the JSON serializer
        [JsonInclude]
        public Guid DefinitionAlphabetFileID = Guid.Empty;
        [JsonInclude]
        public bool IsClassical = true;

        //Abstract function means every child will have to implement this function in some way
        public abstract StateTable Compile(Alphabet DefinitionAlphabet);
    }
}
