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

    public class InputBox : IVisualElement, IClickable, IPollable
    {
        public int Width { get => OutputLabel.Width; set { OutputLabel.Width = value; } }
        public int Height { get => OutputLabel.Height; set { OutputLabel.Height = value; } }
        public bool IsActive { get => OutputLabel.IsActive; set { OutputLabel.IsActive = value; } }

        public bool IsFocused;
        public event OnEditInputBox EditEvent;
        public double TimeSinceLastPoll;

        public Label OutputLabel;
        public UIMesh Background;

        public Vector2 Position { get => OutputLabel.Position; set { OutputLabel.Position = value; Background.MeshTransformations = Matrix.CreateWorld(new Vector3(Position.X, Position.Y, 0), Vector3.Forward, Vector3.Up); } }
        public Vector2 GetBounds { get => new Vector2(Width, Height); }

        public InputBox(int width, int height, Vector2 position, ActionGroup group)
        {
            OutputLabel = new Label(width, height, position);
            Background = UIMesh.CreateRectangle(Vector2.Zero, width, height, GlobalInterfaceData.DebugColor);
            Position = position;

            group.ClickableObjects.Add(this);
            group.PollableObjects.Add(this);
        }

        public void Clicked()
        {
#if DEBUG
            Background.OverlayColor = GlobalInterfaceData.UIOverlayDebugColorHighlight;
#endif
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

        public void PollInput(bool IsInActionFrameGroup)
        {
            TimeSinceLastPoll -= GlobalInterfaceData.Time.ElapsedGameTime.TotalMilliseconds;

            if (TimeSinceLastPoll < -100000000) TimeSinceLastPoll = 0;

            if (IsActive && IsFocused)
            {
                KeyboardState KState = Keyboard.GetState();

                if (KState.GetPressedKeyCount() > 0 && TimeSinceLastPoll <= 0)
                {                    
                    StringBuilder Builder = new StringBuilder(OutputLabel.Text);

                    foreach (Keys PressedKey in KState.GetPressedKeys())
                    {
                        Builder.Append(PressedKey.ToString());
                    }

                    OutputLabel.Text = Builder.ToString();
                    OutputLabel.UpdateTexture();

                    TimeSinceLastPoll = GlobalInterfaceData.TypeWaitTimeMiliseconds;

                    EditEvent?.Invoke(this);
                }
            }
        }

        public void Draw(Viewport BoundPort = default)
        {
            GlobalUIRenderer.Draw(Background);
            OutputLabel.Draw();
        }
    }
}
