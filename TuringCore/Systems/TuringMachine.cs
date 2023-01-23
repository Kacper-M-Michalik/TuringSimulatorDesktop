using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TuringCore
{
    public class TuringMachine
    {
        const string HaltError = "HALT-ERROR";

        public bool IsActive { get; private set; } = false;
        public string CurrentState { get; set; } = "Null";
        public int HeadPosition { get; set; } = 0;
        public InstructionVariant NextInstruction { get; private set; } = null;

        public TapeTemplate OriginalTape { get; private set; }

        public Alphabet ActiveAlphabet { get; private set; }
        public StateTable ActiveStateTable { get; private set; }
        public Tape ActiveTape { get; private set; }

        public int Start(string StartState, int StartHeadPosition)
        {
            if (IsActive) return 1;//throw new Exception("Machine already on");
            if (ActiveStateTable == null) return 2;//throw new Exception("No state table loaded");
            if (ActiveAlphabet == null) return 3;//throw new Exception("No alphabet loaded");
            if (OriginalTape == null) return 4;//throw new Exception("No tape loaded");

            ShallowClear();

            IsActive = true;
            CurrentState = StartState;
            HeadPosition = StartHeadPosition;

            ActiveTape = OriginalTape.Clone(ActiveAlphabet);

            if (ActiveStateTable.ContainsInstructionForState(CurrentState))
            {
                NextInstruction = ActiveStateTable[CurrentState][ActiveTape[HeadPosition]];
            }
            else
            {
                CurrentState = HaltError;
                NextInstruction = null;
            }
            /*
            if (ActiveStateTable.DefinitionAlphabetID != OriginalTape.DefinitionAlphabetID)
            {
                CurrentState = HaltError;
                NextInstruction = null;
            }
            else
            {
                NextInstruction = ActiveStateTable[CurrentState][ActiveTape[HeadPosition]];
            }
            */
            return 0;
        }

        public void StepProgram()
        {
            if (!IsActive) return;

            if (CurrentState == HaltError || ActiveStateTable.IsHaltState(CurrentState))
            {
                IsActive = false;
                return;
            }

            //change so executes only one at time
            for (int i = 0; i < NextInstruction.Actions.Count; i++)
            {
                NextInstruction.Actions[i].Execute(this);
            }

            if (ActiveStateTable.IsHaltState(CurrentState))
            {
                NextInstruction = null;
            }
            else if (ActiveStateTable.ContainsInstructionForState(CurrentState))
            {
                if (ActiveStateTable[CurrentState].ContainsVariant(ActiveTape[HeadPosition]))
                {
                    NextInstruction = ActiveStateTable[CurrentState][ActiveTape[HeadPosition]];
                }
                else if (ActiveStateTable[CurrentState].ContainsVariant(ActiveAlphabet.WildcardCharacter))
                {
                    NextInstruction = ActiveStateTable[CurrentState][ActiveAlphabet.WildcardCharacter];
                }
                else
                {
                    CurrentState = HaltError;
                }
            }
            else
            {
                CurrentState = HaltError;
            }
        }

        public void SetActiveTape(TapeTemplate NewTape)
        {
            //if (ActiveStateTable != null && NewTape.DefenitionAlphabetID != ActiveStateTable.DefenitionAlphabetID) throw new Exception("Incompatible alphabets");
            OriginalTape = NewTape;
        }

        public void SetActiveStateTable(StateTable Table)
        {
            //if (ActiveStateTable.DefinitionAlphabetID != Alphabet.ID) throw new Exception("Wrong alphabet given");
            //if (OriginalTape != null && Table.DefenitionAlphabetID != OriginalTape.DefenitionAlphabetID) throw new Exception("Incompatible alphabets");
            ActiveAlphabet = Table.DefinitionAlphabet;
            ActiveStateTable = Table;
        }

        public void ShallowClear()
        {
            IsActive = false;
            CurrentState = "Null";
            HeadPosition = 0;
            NextInstruction  = null;
            ActiveTape = null;
        }
    }
}
