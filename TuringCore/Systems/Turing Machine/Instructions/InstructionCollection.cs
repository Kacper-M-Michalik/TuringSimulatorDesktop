using System;
using System.Collections.Generic;

namespace TuringCore.Systems
{
    public class InstructionCollection
    {
        Dictionary<string, InstructionVariant> InstructionVariants = new Dictionary<string, InstructionVariant>();

        //Allows the InstructionCollection to be accessed like an array -> Neater Code
        public InstructionVariant this[string ReadAlphabetCharacter]
        {
            get
            {
                //Check if we have valid instructions for this read tape value
                if (InstructionVariants.TryGetValue(ReadAlphabetCharacter, out InstructionVariant Variant))
                {
                    return Variant;
                }
                else
                {
                    throw new Exception("Exception! No variant implentation for current character! Index: " + ReadAlphabetCharacter);
                }
            }
        }

        public void AddVariant(string TriggerState, InstructionVariant NewVariant)
        {
            InstructionVariants.Add(TriggerState, NewVariant);
        }

        public bool ContainsVariant(string ReadAlphabetCharacter)
        {
            return InstructionVariants.ContainsKey(ReadAlphabetCharacter);
        }
    }
}
