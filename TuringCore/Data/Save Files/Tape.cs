using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TuringCore
{
    [Serializable]
    public class TapeTemplate : SaveFile
    {
        //[JsonInclude]
        //public string ID;
        //[JsonInclude]
        public Guid DefinitionAlphabetID;
        [JsonInclude]
        public Dictionary<int, string> Data = new Dictionary<int, string>();
        [JsonInclude]
        public int HighestIndex;
        [JsonInclude]
        public int LowestIndex;

        public TapeTemplate() 
        {
            //ID = "";
            DefinitionAlphabetID = Guid.Empty;
            Data = new Dictionary<int, string>();
            HighestIndex = 0;
            LowestIndex = 0;
        }

        public void SetData(string[] Input)
        {
            Data.Clear();
            for (int i = 0;  i < Input.Length; i++)
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
            //CloneTape.ID = ID;
            CloneTape.DefinitionAlphabet = Alphabet;
            CloneTape.Data = new Dictionary<int, string>(Data);
            CloneTape.HighestIndex = HighestIndex;
            CloneTape.LowestIndex = LowestIndex;
            return CloneTape;
        }
    }

    public class Tape
    {
        public Alphabet DefinitionAlphabet;
        public Dictionary<int, string> Data = new Dictionary<int, string>();
        public int HighestIndex;
        public int LowestIndex;

        public Tape() { }

        public string this[int Position]
        {
            get
            {
                if (Data.TryGetValue(Position, out string Value))
                {
                    return Value;
                }
                else
                { 
                    Data.Add(Position, DefinitionAlphabet.EmptyCharacter);
                    return Data[Position];
                }
            }
            set
            {
                if (Data.ContainsKey(Position))
                {
                    Data[Position] = value;
                }
                else
                {
                    if (Position < LowestIndex) LowestIndex = Position;
                    if (Position > HighestIndex) HighestIndex = Position;
                    Data.Add(Position, value);
                }
            }
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
    }
}
