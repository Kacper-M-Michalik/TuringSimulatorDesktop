using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TuringCore.Files
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

        //Construct a new TapeTemplate
        public TapeTemplate()
        {
            Data = new Dictionary<int, string>();
            HighestIndex = 0;
            LowestIndex = 0;
        }
        
        //Construct a new TapeTemplate using data from an existing Tape
        public TapeTemplate(Tape Source)
        {
            Data = new Dictionary<int, string>(Source.Data);
            HighestIndex = Source.HighestIndex;
            LowestIndex = Source.LowestIndex;
        }

        //Set data on the TapeTempalte in bulk
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

        //Get the length of the Tape
        public int Count()
        {
            return Data.Count;
        }

        //Create a new Tape using the TapeTemplates data
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
