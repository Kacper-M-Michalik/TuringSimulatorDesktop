using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TuringCore.Files
{
    //Class + JSON Template
    [Serializable]
    public class Alphabet
    {
        [JsonInclude]
        public HashSet<string> Characters;
        [JsonInclude]
        public string EmptyCharacter;
        [JsonInclude]
        public string WildcardCharacter;

        //Construct a new alphabet
        public Alphabet()
        {
            Characters = new HashSet<string>();
            EmptyCharacter = "";
            WildcardCharacter = "*";
        }
    }
}
