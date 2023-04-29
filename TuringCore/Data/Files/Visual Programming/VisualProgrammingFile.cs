using System.Collections.Generic;
using System.Text.Json.Serialization;
using TuringCore.Systems;
using TuringCore.VisualProgramming;

namespace TuringCore.Files
{
    public class VisualProgrammingFile : CompilableFile
    {
        [JsonInclude]
        public HashSet<string> HaltStates;
        [JsonInclude]
        public List<Node> StoredNodes = new List<Node>();

        public void InsertStateNode(string State)
        {
            StateNode Node = new StateNode(State);
            StoredNodes.Add(Node);
        }

        //Did not have time to complete the Extension Objective of a node based TM programming method
        public override StateTable Compile(Alphabet DefinitionAlphabet)
        {
            return null;
        }
    }
}
