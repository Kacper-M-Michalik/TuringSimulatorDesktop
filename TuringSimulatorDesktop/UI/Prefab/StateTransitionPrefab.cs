using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.UI
{
    public class StateTransitionPrefab : ITileable
    {
        //( CurrentState , TapeValue ) => ( NewState , New Value, Move Dir  )

        public TextLabel BackgroundLabel;

        public TextBox CurrentStateTextBox;
        public TextBox TapeValueTextBox;

        public TextBox NewStateTextBox;
        public TextBox NewTapeValueTextBox;
        public TextBox MoveDirectionTextBox;

        public StateTransitionPrefab()
        {
            BackgroundLabel = new TextLabel();

            CurrentStateTextBox = new TextBox();
            TapeValueTextBox = new TextBox();
            NewStateTextBox = new TextBox();
            NewTapeValueTextBox = new TextBox();
            MoveDirectionTextBox = new TextBox();

            CurrentStateTextBox.EditEvent += ResizeElements;
            TapeValueTextBox.EditEvent += ResizeElements;
            NewStateTextBox.EditEvent += ResizeElements;
            NewTapeValueTextBox.EditEvent += ResizeElements;
            MoveDirectionTextBox.EditEvent += ResizeElements;
        }

        public int X { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Y { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int GetBoundX => throw new NotImplementedException();

        public int GetBoundY => throw new NotImplementedException();

        public void ResizeElements(TextBox Sender)
        {

        }
    }
}
