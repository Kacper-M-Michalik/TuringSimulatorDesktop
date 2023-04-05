namespace TuringCore.Systems
{
    public class WriteAction : TuringAction
    {
        public string WriteValue;

        public WriteAction(string writeValue)
        {
            WriteValue = writeValue;
        }

        public override void Execute(TuringMachine Machine)
        {
            Machine.ActiveTape[Machine.HeadPosition] = WriteValue;
        }
    }
}
