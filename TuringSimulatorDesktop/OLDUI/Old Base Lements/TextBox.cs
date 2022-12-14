using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.Input;
using TuringSimulatorDesktop;
using Microsoft.Xna.Framework;

namespace TuringSimulatorDesktop.UI
{
   // public delegate void OnEditTextBox(TextBox Sender);

    public class OldTextBox : TextLabel, IPollable, IClickable
    {
        public bool Pollable;
       // public event OnEditTextBox EditEvent;

        public OldTextBox(Vector2 SetPosition, ActionGroup Group, int SetFontSize = 12, string SetString = "") : base(SetPosition, SetFontSize, SetString)
        {
            Group.ClickableObjects.Add(this);
            Group.PollableObjects.Add(this);
        }

        void IClickable.Clicked()
        {
            Pollable = true;
        }

        void IClickable.ClickedAway()
        {
            Pollable = false;
        }

        public bool IsMouseOver()
        {
            return (InputManager.LeftMousePressed && InputManager.MouseData.X >= Position.X - 0 && InputManager.MouseData.X <= Position.X + 30 && InputManager.MouseData.Y >= Position.Y - 0 && InputManager.MouseData.Y <= Position.Y + 20);
        }

        public void PollInput(bool IsInActionFrameGroup)
        {
            if (Pollable)
            {
                KeyboardState KState = Keyboard.GetState();

                if (KState.GetPressedKeyCount() > 0)
                {
                    /*
                    StringBuilder Builder = new StringBuilder(Text);

                    foreach (Keys PressedKey in KState.GetPressedKeys())
                    {
                        Builder.Append(PressedKey.ToString());
                    }

                    Text = Builder.ToString();
                    */
                  //  EditEvent?.Invoke(this);
                }
            }
        }

    }
}
