using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringCore.Actions
{
    public class ChangeStateAction : TuringAction
    {
        public string NewState;

        public ChangeStateAction(string newState)
        {
            NewState = newState;
        }

        public override void Execute(TuringMachine Machine)
        {
            Machine.CurrentState = NewState;
        }
    }
}
