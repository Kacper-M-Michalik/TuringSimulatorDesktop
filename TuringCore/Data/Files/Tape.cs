using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TuringCore.Files
{
    public class Tape
    {
        public Alphabet DefinitionAlphabet;
        public Dictionary<int, string> Data = new Dictionary<int, string>();
        public int HighestIndex;
        public int LowestIndex;

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
