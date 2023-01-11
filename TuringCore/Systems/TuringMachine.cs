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

        public void Start(string StartState, int StartHeadPosition)
        {
            if (IsActive) throw new Exception("Machine already on");

            ShallowClear();

            IsActive = true;
            CurrentState = StartState;
            HeadPosition = StartHeadPosition;

            ActiveTape = OriginalTape.Clone(ActiveAlphabet);

            if (ActiveStateTable.DefenitionAlphabetID != OriginalTape.DefenitionAlphabetID)
            {
                CurrentState = HaltError;
                NextInstruction = null;
            }
            else
            {
                NextInstruction = ActiveStateTable[CurrentState][ActiveTape[HeadPosition]];
            }

        }

        public void StepProgram()
        {
            if (!IsActive) return;

            if (CurrentState == HaltError || ActiveStateTable.IsHaltState(CurrentState))
            {
                IsActive = false;
                return;
            }

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

        public void SetActiveTape(TapeTemplate NewTape, Alphabet Alphabet)
        {
            OriginalTape = NewTape;
            ActiveTape = NewTape.Clone(Alphabet);
        }

        public void SetActiveStateTable(StateTable Table, Alphabet Alphabet)
        {
            if (ActiveStateTable.DefenitionAlphabetID != Alphabet.ID) throw new Exception("Wrong alphabet given");
            ActiveAlphabet = Alphabet;
            ActiveStateTable = Table;
        }

        public void ShallowClear()
        {
            IsActive = false;
            CurrentState = "Null";
            HeadPosition = 0;
            NextInstruction  = null;
            ActiveStateTable = null;
            ActiveTape = null;
        }
    }
}
