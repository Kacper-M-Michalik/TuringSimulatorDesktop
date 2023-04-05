namespace TuringCore.Systems
{
    public class MoveHeadAction : TuringAction
    {
        public int MoveValue;

        public MoveHeadAction(int moveValue)
        {
            MoveValue = moveValue;
        }

        public override void Execute(TuringMachine Machine)
        {
            Machine.HeadPosition += MoveValue;
        }
    }
}
