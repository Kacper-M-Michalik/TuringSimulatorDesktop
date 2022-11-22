using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringCore.Systems.VisualProgramming
{
    public class StateNode : Node
    {
        public string NewState;

        public StateNode(string SetNewState)
        {
            NewState = SetNewState;
        }
    }
}
