using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringCore.Actions;
using System.Text.Json.Serialization;
using TuringCore.TextProgramming;

namespace TuringCore
{
    [Serializable]
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

        public override StateTable Compile(Alphabet DefinitionAlphabet)
        {
            StateTable Table = new StateTable();
            //Table.ID = "NULL NAME";
            //Table.DefinitionAlphabet = DefinitionAlphabet;

            foreach (string HaltState in HaltStates)
            {
                Table.AddHaltState(HaltState);
            }

            for (int i = 0; i < Transitions.Count; i++)
            {
                if (!Table.ContainsInstructionForState(Transitions[i].CurrentState))
                {
                    Table.AddInstruction(Transitions[i].CurrentState, new InstructionCollection());
                }
            }

            for (int i = 0; i < Transitions.Count; i++)
            {
                if (!DefinitionAlphabet.Characters.Contains(Transitions[i].TapeValue) || Table[Transitions[i].CurrentState].ContainsVariant(Transitions[i].TapeValue))
                {
                    //return error codes?
                    return null;
                }

                InstructionVariant Variant = new InstructionVariant();
                if (Transitions[i].NewState != DefinitionAlphabet.WildcardCharacter) Variant.Actions.Add(new ChangeStateAction(Transitions[i].NewState));
                if (Transitions[i].NewTapeValue != DefinitionAlphabet.WildcardCharacter) Variant.Actions.Add(new WriteAction(Transitions[i].NewTapeValue));

                if (Transitions[i].MoveDirection == MoveHeadDirection.Left)
                {
                    Variant.Actions.Add(new MoveHeadAction(-1));
                }
                else
                {
                    Variant.Actions.Add(new MoveHeadAction(1));
                }

                Table[Transitions[i].CurrentState].AddVariant(Transitions[i].TapeValue, Variant);
            }

            return Table;
        }
    }

    [Serializable]
    public enum MoveHeadDirection
    {
        Left,
        Right,
        Empty
    }
}
