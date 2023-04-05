namespace TuringCore.Systems
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
