using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringCore.Actions;
using System.Text.Json.Serialization;

namespace TuringCore
{
    [Serializable]
    public class TransitionFile : CompilableFile
    {
        [JsonInclude]
        public List<string> HaltStates;
        [JsonInclude]
        public List<Transition> Transitions;

        public TransitionFile()
        {
            HaltStates = new List<string>();
            Transitions = new List<Transition>();            
        }

        public override StateTable Compile(Alphabet DefinitionAlphabet)
        {
            StateTable Table = new StateTable();
            //Table.ID = "NULL NAME";
            //Table.DefinitionAlphabet = DefinitionAlphabet;

            for (int i = 0; i < HaltStates.Count; i++)
            {
                Table.AddHaltState(HaltStates[i]);
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
                Variant.Actions.Add(new ChangeStateAction(Transitions[i].NewState));
                Variant.Actions.Add(new WriteAction(Transitions[i].NewTapeValue));

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
    public class Transition
    {
        [JsonInclude]
        public string CurrentState;
        [JsonInclude]
        public string TapeValue;
        [JsonInclude]
        public string NewState;
        [JsonInclude]
        public string NewTapeValue;
        [JsonInclude]
        public MoveHeadDirection MoveDirection;

        public Transition()
        {
        }

        public Transition(string currentState, string tapeValue, string newState, string newTapeValue, MoveHeadDirection moveDirection)
        {
            CurrentState = currentState;
            TapeValue = tapeValue;
            NewState = newState;
            NewTapeValue = newTapeValue;
            MoveDirection = moveDirection;
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
