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

        //[] Allows tape to be accessed like an array -> Neater code
        public string this[int Position]
        {
            get
            {
                //As the tape is meant to be infinite, it would have to have an infinite amount of empty symbol values on it, which is impossible to store, as such we only store cells that have been read from/written to, then adding empty symbols as required

                //Here we check if we have a symbol written on the tape at this index
                if (Data.TryGetValue(Position, out string Value))
                {
                    return Value;
                }
                else
                { 
                    //If not, write an empty character and return it
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
                    //We store the highest and lowest index of the tape as optimsiation so the UI can know when to stop reading the tape and isntead just print empty symbols on the UI tape

                    //Check if the index exceeds the highest/lowest bound
                    if (Position < LowestIndex) LowestIndex = Position;
                    if (Position > HighestIndex) HighestIndex = Position;
                    Data.Add(Position, value);
                }
            }
        }

        //Set data to an array
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
