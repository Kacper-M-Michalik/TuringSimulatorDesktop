using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringCore.VisualProgramming;

namespace TuringCore
{
    public class VisualProgrammingFile : CompilableFile
    {
        public Alphabet ProjectAlphabet;
        public List<Node> StoredNodes = new List<Node>();

        public void InsertStateNode(string State)
        {
            StateNode Node = new StateNode(State);
            StoredNodes.Add(Node);
        }

        public override StateTable Compile(Alphabet DefinitionAlphabet)
        {
            throw new NotImplementedException();
        }
    }
}
