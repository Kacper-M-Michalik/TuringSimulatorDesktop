using System;
using System.Collections.Generic;

namespace TuringCore.Systems
{
    public class StateTable
    {
        Dictionary<string, InstructionCollection> Instructions = new Dictionary<string, InstructionCollection>();
        HashSet<string> HaltStates = new HashSet<string>();

        //Allows the statetable to be accessed like an array -> Neater Code
        public InstructionCollection this[string State]
        {
            get
            {
                //Check if we have a valid collection of instructions for this state
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
