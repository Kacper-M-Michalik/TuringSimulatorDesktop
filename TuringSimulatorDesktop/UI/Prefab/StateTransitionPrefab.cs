using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.UI.Prefab
{
    public class StateTransitionPrefab
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
        }
    }
}
