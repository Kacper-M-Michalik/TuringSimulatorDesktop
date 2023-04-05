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
