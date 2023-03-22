using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TuringCore
{
    [Serializable]
    public class Alphabet
    {
        [JsonInclude]
        public HashSet<string> Characters;
        [JsonInclude]
        public string EmptyCharacter;
        [JsonInclude]
        public string WildcardCharacter;

        public Alphabet()
        {
            Characters = new HashSet<string>();
            EmptyCharacter = "";
            WildcardCharacter = "*";
        }
    }
}
