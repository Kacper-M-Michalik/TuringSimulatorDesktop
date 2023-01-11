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
        public string DefenitionAlphabetID;
        [JsonInclude]
        public List<string> HaltStates;
        [JsonInclude]
        public List<Transition> Transitions;

        public TransitionFile()
        {
            DefenitionAlphabetID = "";
            HaltStates = new List<string>();
            Transitions = new List<Transition>();            
        }

        public override StateTable Compile()
        {
            StateTable Table = new StateTable();
            Table.ID = "NULL NAME";
            Table.DefenitionAlphabetID = DefenitionAlphabetID;

            for (int i = 0; i < HaltStates.Count; i++)
            {
                Table.AddHaltState(HaltStates[i]);
            }
            /*
                Dictionary<string, InstructionCollection> Collections = new Dictionary<string, InstructionCollection>();

            for (int i = 0; i < Transitions.Count; i++)
            {
                if (!Collections.ContainsKey(Transitions[i].CurrentState))
                {
                    Collections.Add(Transitions[i].CurrentState, new InstructionCollection());
                }
            }

            InstructionVariant Variant = new InstructionVariant();
            Variant.Actions.Add(new ChangeStateAction());
            Variant.Actions.Add(new WriteAction());
            Variant.Actions.Add(new MoveHeadAction());
            */
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
        Right
    }
}
