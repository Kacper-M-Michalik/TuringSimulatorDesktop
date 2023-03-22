using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TuringCore
{
    [Serializable]
    public class TapeTemplate
    {
        [JsonInclude]
        public Dictionary<int, string> Data = new Dictionary<int, string>();
        [JsonInclude]
        public int HighestIndex;
        [JsonInclude]
        public int LowestIndex;

        public TapeTemplate()
        {
            //ID = "";
            // DefinitionAlphabetID = Guid.Empty;
            Data = new Dictionary<int, string>();
            HighestIndex = 0;
            LowestIndex = 0;
        }

        public TapeTemplate(Tape Source)
        {
            Data = new Dictionary<int, string>(Source.Data);
            HighestIndex = Source.HighestIndex;
            LowestIndex = Source.LowestIndex;
        }

        public void SetData(string[] Input)
        {
            Data.Clear();
            for (int i = 0; i < Input.Length; i++)
            {
                Data.Add(i, Input[i]);
            }
            LowestIndex = 0;
            HighestIndex = Input.Length - 1;
        }

        public int Count()
        {
            return Data.Count;
        }

        public Tape Clone(Alphabet Alphabet)
        {
            Tape CloneTape = new Tape();
            CloneTape.DefinitionAlphabet = Alphabet;
            CloneTape.Data = new Dictionary<int, string>(Data);
            CloneTape.HighestIndex = HighestIndex;
            CloneTape.LowestIndex = LowestIndex;
            return CloneTape;
        }
    }
}
