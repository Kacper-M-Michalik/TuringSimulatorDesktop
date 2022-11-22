using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringCore.Systems.VisualProgramming
{
    public class VisualProgrammingProject
    {
        public Alphabet ProjectAlphabet;
        public List<Node> StoredNodes = new List<Node>();

        public void InsertStateNode(string State)
        {
            StateNode Node = new StateNode(State);
            StoredNodes.Add(Node);
        }

        public StateTable Compile()
        {
            return null;
        }
    }
}
