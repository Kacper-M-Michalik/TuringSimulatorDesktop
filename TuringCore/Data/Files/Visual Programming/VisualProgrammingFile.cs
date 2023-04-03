using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringCore.Systems;
using TuringCore.VisualProgramming;

namespace TuringCore.Files
{
    public class VisualProgrammingFile : CompilableFile
    {
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
