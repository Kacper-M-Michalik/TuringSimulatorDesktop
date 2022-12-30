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

    public class InputBox : IVisualElement, IClickable
    {
        public int Width { get => OutputLabel.Width; set { OutputLabel.Width = value; } }
        public int Height { get => OutputLabel.Height; set { OutputLabel.Height = value; } }
        public bool IsActive { get => OutputLabel.IsActive; set { OutputLabel.IsActive = value; } }

        //not in use right now
        KeyboardModifiers Modifiers = new KeyboardModifiers();

        StringBuilder Builder = new StringBuilder();

        public bool IsFocused;
        public event OnEditInputBox EditEvent;

        public string Text { get => OutputLabel.Text; set => OutputLabel.Text = value; }
        public Label OutputLabel;
        public UIMesh Background;

        public Vector2 Position { get => OutputLabel.Position; set { OutputLabel.Position = value; Background.MeshTransformations = Matrix.CreateWorld(new Vector3(Position.X, Position.Y, 0), Vector3.Forward, Vector3.Up); } }
        public Vector2 GetBounds { get => new Vector2(Width, Height); }

        public InputBox(int width, int height, Vector2 position, ActionGroup group)
        {
            OutputLabel = new Label(width, height, position, GlobalInterfaceData.StandardRegularFont);
            Background = UIMesh.CreateRectangle(Vector2.Zero, width, height, GlobalInterfaceData.DebugColor);
            Position = position;

            GlobalInterfaceData.OSWindow.TextInput += TextInput;

            group.ClickableObjects.Add(this);
        }

        public void Clicked()
        {
//#if DEBUG
            //Background.OverlayColor = GlobalInterfaceData.UIOverlayDebugColor2;
//#endif
            IsFocused = true;
        }

        public void ClickedAway()
        {
#if DEBUG
            Background.OverlayColor = GlobalInterfaceData.DebugColor;
#endif
            IsFocused = false;
        }

        public bool IsMouseOver()
        {
            return (IsActive && InputManager.MouseData.X >= Position.X && InputManager.MouseData.X <= Position.X + Width && InputManager.MouseData.Y >= Position.Y && InputManager.MouseData.Y <= Position.Y + Height);
        }

        public void Draw(Viewport BoundPort = default)
        {
            GlobalUIRenderer.Draw(Background);
            OutputLabel.Draw();
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
                        Builder.Append("/n");
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

    }
}
