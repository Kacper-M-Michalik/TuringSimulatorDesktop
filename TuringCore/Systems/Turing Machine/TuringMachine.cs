using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TuringCore.Files;

namespace TuringCore.Systems
{
    public class TuringMachine
    {
        //Arbitrarily decided error state -> Shown when there is a problem while xecuting TM program
        const string HaltError = "HALT-ERROR";

        public bool IsActive { get; private set; } = false;
        public bool ReachedHaltState { get; private set; } = false;
        public string CurrentState { get; set; } = "Null";
        public int HeadPosition { get; set; } = 0;
        public InstructionVariant NextInstruction { get; private set; } = null;

        public TapeTemplate OriginalTape { get; private set; }

        public Alphabet ActiveAlphabet { get; private set; }
        public StateTable ActiveStateTable { get; private set; }
        public Tape ActiveTape { get; private set; }

        //Starts the TM
        public int Start(string StartState, int StartHeadPosition)
        {
            //Error codes returned depending  on error type
            //if (IsActive) return 1;//throw new Exception("Machine already on");
            if (ActiveStateTable == null) return 2;//throw new Exception("No state table loaded");
            if (ActiveAlphabet == null) return 3;//throw new Exception("No alphabet loaded");
            if (OriginalTape == null) return 4;//throw new Exception("No tape loaded");

            //Clears the machine in case it was active before
            ShallowClear();

            IsActive = true;
            CurrentState = StartState;
            HeadPosition = StartHeadPosition;

            //Clone sourece tape to one that will be edited by the TM
            ActiveTape = OriginalTape.Clone(ActiveAlphabet);

            //If the current state is a halt state as programmed by the user, there will no longer be a next instruction
            if (ActiveStateTable.IsHaltState(CurrentState))
            {
                NextInstruction = null;
            }
            else if (ActiveStateTable.ContainsInstructionForState(CurrentState))
            {
                //Check if the state table has a valid instruction set for the current state/tape value combination, if not then we check if there is a valid wildcard instruction
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
                    //If no valid instructions found, enter error state
                    IsActive = false;
                    ReachedHaltState = true;
                    CurrentState = HaltError;
                }
            }
            else
            {
                //If no valid instructions found, enter error state
                IsActive = false;
                ReachedHaltState = true;
                CurrentState = HaltError;
            }

            //Successful return code
            return 0;
        }

        public void StepProgram()
        {
            if (!IsActive) return;

            //Finish program once halt state or error state are reached
            if (CurrentState == HaltError || ActiveStateTable.IsHaltState(CurrentState))
            {
                ReachedHaltState = true;
                IsActive = false;
                return;
            }

            //Execute all instructions
            for (int i = 0; i < NextInstruction.Actions.Count; i++)
            {
                NextInstruction.Actions[i].Execute(this);
            }

            //Similiar code as in the Start function, checks for the next valdi isntructions or sers the TM into a error state if none found
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
                    ReachedHaltState = true;
                    CurrentState = HaltError;
                }
            }
            else
            {
                ReachedHaltState = true;
                CurrentState = HaltError;
            }
        }

        public void SetActiveTape(TapeTemplate NewTape)
        {
            //if (ActiveStateTable != null && NewTape.DefenitionAlphabetID != ActiveStateTable.DefenitionAlphabetID) throw new Exception("Incompatible alphabets");
            OriginalTape = NewTape;
            //if (ActiveAlphabet != null) ActiveTape = NewTape.Clone(ActiveAlphabet);
        }

        public void SetActiveStateTable(StateTable Table, Alphabet Alphabet)
        {
            //if (ActiveStateTable.DefinitionAlphabetID != Alphabet.ID) throw new Exception("Wrong alphabet given");
            //if (OriginalTape != null && Table.DefenitionAlphabetID != OriginalTape.DefenitionAlphabetID) throw new Exception("Incompatible alphabets");
            ActiveAlphabet = Alphabet;
            ActiveStateTable = Table;
        }

        public void ShallowClear()
        {
            //Resets all variables on the TM
            ReachedHaltState = false;
            IsActive = false;
            CurrentState = "Null";
            HeadPosition = 0;
            NextInstruction  = null;
            ActiveTape = null;
        }
    }
}
