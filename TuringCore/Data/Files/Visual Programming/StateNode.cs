using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringCore.VisualProgramming
{
    public class StateNode : Node
    {
        public string CurrentState;

        public StateNode(string SetCurrentState)
        {
            CurrentState = SetCurrentState;
        }
    }
}
