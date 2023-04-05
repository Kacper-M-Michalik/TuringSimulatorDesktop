using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using TuringCore.Systems;
using TuringCore.TextProgramming;

namespace TuringCore.Files
{
    public class TransitionFile : CompilableFile
    {
        [JsonInclude]
        public HashSet<string> HaltStates;
        [JsonInclude]
        public List<Transition> Transitions;

        public TransitionFile()
        {
            HaltStates = new HashSet<string>();
            Transitions = new List<Transition>();            
        }

        //Implementation of the Compile parent function
        public override StateTable Compile(Alphabet DefinitionAlphabet)
        {
            StateTable Table = new StateTable();

            //Copy all halt states to the statetable
            foreach (string HaltState in HaltStates)
            {
                Table.AddHaltState(HaltState);
            }

            //Generate a instruction collection for every state in transitions
            for (int i = 0; i < Transitions.Count; i++)
            {
                if (!Table.ContainsInstructionForState(Transitions[i].CurrentState))
                {
                    Table.AddInstruction(Transitions[i].CurrentState, new InstructionCollection());
                }
            }

            //Generate Instructions
            for (int i = 0; i < Transitions.Count; i++)
            {
                //If a transition has a read tape value that is not part of the definition alphabet, an empty statetable is returned, signifying an error
                if (!DefinitionAlphabet.Characters.Contains(Transitions[i].TapeValue) || Table[Transitions[i].CurrentState].ContainsVariant(Transitions[i].TapeValue))
                {
                    return null;
                }

                //Every classical transition has these three instructions: Chang State, Write To Tape, Move Left/Right
                InstructionVariant Variant = new InstructionVariant();
                //In a non classical TM we can use a Wildcard Character to indicate no change on function
                if (IsClassical || Transitions[i].NewState != DefinitionAlphabet.WildcardCharacter) Variant.Actions.Add(new ChangeStateAction(Transitions[i].NewState));
                if (IsClassical || Transitions[i].NewTapeValue != DefinitionAlphabet.WildcardCharacter) Variant.Actions.Add(new WriteAction(Transitions[i].NewTapeValue));

                //Directions are defined as negative being to the left of the tape and positive as being to the right of the tape
                if (Transitions[i].MoveDirection == MoveHeadDirection.Left)
                {
                    Variant.Actions.Add(new MoveHeadAction(-1));
                }
                else
                {
                    Variant.Actions.Add(new MoveHeadAction(1));
                }

                //Add the variant to the Instruction Collection
                Table[Transitions[i].CurrentState].AddVariant(Transitions[i].TapeValue, Variant);
            }

            //Return successfully compiled StateTable
            return Table;
        }
    }
}
