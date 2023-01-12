using System;
using System.Collections.Generic;

namespace TuringCore
{
    public class StateTable
    {
        public string ID;
        public string DefinitionAlphabetID;
        Dictionary<string, InstructionCollection> Instructions = new Dictionary<string, InstructionCollection>();
        HashSet<string> HaltStates = new HashSet<string>();

        public InstructionCollection this[string State]
        {
            get
            {
                if (Instructions.TryGetValue(State, out InstructionCollection InstructionToReturn))
                {
                    return InstructionToReturn;
                }
                else
                {
                    throw new Exception("Exception! Statetable doesn't contain a instruction for this state: " + State.ToString());
                }
            }
        }

        public void AddInstruction(string TriggerState, InstructionCollection NewInstruction)
        {
            Instructions.Add(TriggerState, NewInstruction);
        }

        public void AddHaltState(string State)
        {
            HaltStates.Add(State);
        }

        public bool ContainsInstructionForState(string State)
        {
            return Instructions.ContainsKey(State);
        }

        public bool IsHaltState(string State)
        {
            return HaltStates.Contains(State);
        }
    }
}
