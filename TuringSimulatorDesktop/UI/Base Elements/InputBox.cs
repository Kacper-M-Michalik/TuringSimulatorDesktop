using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI
{
    public delegate void OnEditInputBox(InputBox Sender);
    public delegate void OnClickedInputBox(InputBox Sender);
    public delegate void OnClickedAwayInputBox(InputBox Sender);

    public class InputBox : IVisualElement, IClickable
    {
        Vector2 position;
        public Vector2 Position
        {
            get => position;
            set
            {
                position = value;
                Background.Position = position;
                OutputLabel.Position = position;
            }
        }

        Point bounds;
        public Point Bounds
        {
            get => bounds;
            set
            {
                bounds = value;
                Background.Bounds = bounds;
                OutputLabel.Bounds = bounds;
            }
        }

        public bool IsActive = true;
        ActionGroup Group;
        public bool IsMarkedForDeletion { get; set; }

        //TODO - NOT FULLY IMPLEMENTED
        public KeyboardModifiers Modifiers = new KeyboardModifiers();
        StringBuilder Builder = new StringBuilder();
        public bool IsFocused;
        public event OnEditInputBox EditEvent;
        public event OnClickedAwayInputBox ClickEvent;
        public event OnClickedAwayInputBox ClickAwayEvent;
        public string Text 
        {
            get => OutputLabel.Text;
            set
            {
                OutputLabel.Text = value;
                Builder.Clear();
                Builder.Append(Text);
            }
        }
        public Color BackgroundColor
        {
            get => Background.DrawColor;
            set => Background.DrawColor = value;
        }

        Icon Background;
        Label OutputLabel;

        public InputBox(int width, int height, ActionGroup group)
        {
            Background = new Icon(GlobalRenderingData.DebugColor);
            OutputLabel = new Label(0, 0);
            Bounds = new Point(width, height);
            Position = Vector2.Zero;

            GlobalRenderingData.OSWindow.TextInput += TextInput;

            group.ClickableObjects.Add(this);
            Group = group;
        }
        public InputBox(int width, int height, Vector2 position, ActionGroup group)
        {
            Background = new Icon();
            OutputLabel = new Label(0,0);
            Bounds = new Point(width, height);
            Position = position;

            GlobalRenderingData.OSWindow.TextInput += TextInput;

            group.ClickableObjects.Add(this);
        }

        public void Clicked()
        {
            IsFocused = true;
            ClickEvent?.Invoke(this);
        }

        public void ClickedAway()
        {                        
            IsFocused = false;
            ClickAwayEvent?.Invoke(this);
        }

        public bool IsMouseOver()
        {
            return (IsActive && InputManager.MouseData.X >= Position.X && InputManager.MouseData.X <= Position.X + bounds.X && InputManager.MouseData.Y >= Position.Y && InputManager.MouseData.Y <= Position.Y + bounds.Y);
        }

        public void TextInput(object Sender, TextInputEventArgs Args)
        {
            if (IsActive && IsFocused)
            {
                switch (Args.Key)
                {
                    case Keys.Tab:
                        Builder.Append("    ");
                        break;
                    case Keys.Enter:
                        if (Modifiers.AllowsNewLine) Builder.Append("/n");
                        break;
                    case Keys.Back:
                        if (Builder.Length > 0)
                        {
                            if (Builder[Builder.Length - 1] == 'n' && Builder[Builder.Length - 2] == '/') Builder.Remove(Builder.Length - 2, 2);
                            else Builder.Remove(Builder.Length - 1, 1);
                        }
                        break;
                    default:
                        Builder.Append(Args.Character);
                        break;
                }

                OutputLabel.Text = Builder.ToString();

                EditEvent?.Invoke(this);            
            }
           
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                Background.Draw(BoundPort);
                OutputLabel.Draw(BoundPort);
            }
        }

        public void Close()
        {
            Group.IsDirtyClickable = true;
            IsMarkedForDeletion = true;
        }
    }
}
