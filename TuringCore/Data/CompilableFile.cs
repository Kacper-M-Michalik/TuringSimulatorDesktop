using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TuringCore
{
    [Serializable]
    public abstract class CompilableFile
    {
        [JsonInclude]
        public Alphabet DefinitionAlphabet;
        public abstract StateTable Compile();
    }
}
