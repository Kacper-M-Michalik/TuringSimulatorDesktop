using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI
{
    public class OldWindow : UIElement
    {
        protected int Width { get { return WindowTexture.Width; } }
        protected int Height { get { return WindowTexture.Height; } }

        protected SpriteBatch LocalSpriteBatch;
        protected RenderTarget2D WindowTexture;
        protected GraphicsDevice Device;

        List<UIElement> SubElements;

        bool Resizable = true;
        bool Draggable = true;
        Vector2 Offset;
        bool StartedResizing;
        bool StartedDragging;

        public OldWindow(GraphicsDevice SetDevice, int SetWidth, int SetHeight)
        {
            LocalSpriteBatch = new SpriteBatch(SetDevice);
            WindowTexture = new RenderTarget2D(SetDevice, SetWidth, SetHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            Device = SetDevice;
            SubElements = new List<UIElement>();
        }

        public OldWindow(GraphicsDevice SetDevice, int SetWidth, int SetHeight, bool SetResizable, bool SetDraggable)
        {
            LocalSpriteBatch = new SpriteBatch(SetDevice);
            WindowTexture = new RenderTarget2D(SetDevice, SetWidth, SetHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            Device = SetDevice;
            SubElements = new List<UIElement>();

            Resizable = SetResizable;
            Draggable = SetDraggable;
        }

        protected bool MouseWithinBounds()
        {
            return InputManager.MouseData.X >= Position.X && InputManager.MouseData.X <= Position.X + WindowTexture.Width && InputManager.MouseData.Y >= Position.Y && InputManager.MouseData.Y <= Position.Y + WindowTexture.Height;
        }

        public override void Draw(SpriteBatch OwnerSpriteBatch, RenderTarget2D PreviousRenderTarget)
        {
            Device.SetRenderTarget(WindowTexture);
            Device.Clear(Color.Yellow);
            LocalSpriteBatch.Begin();
            for (int i = 0; i < SubElements.Count; i++)
            {
                SubElements[i].Draw(LocalSpriteBatch, WindowTexture);
            }
            LocalSpriteBatch.End();

            Device.SetRenderTarget(PreviousRenderTarget);
            OwnerSpriteBatch.Draw(WindowTexture, Position, Color.White);
        }

        /*
        public override void Update(GameTime gameTime)
        {
            if (((StartedDragging || StartedResizing) && InputManager.MouseData.LeftButton == ButtonState.Released && InputManager.IsMouseAvailable(this)))
            {
                InputManager.FreeMouse();
                StartedResizing = false;
                StartedDragging = false;
            }

            if (StartedResizing)
            {
                WindowTexture.Dispose();
                Vector2 Size = InputManager.CurrentMousePosition - Position + Offset;
                WindowTexture = new RenderTarget2D(Device, Math.Clamp((int) Size.X, 40, int.MaxValue), Math.Clamp((int)Size.Y, 40, int.MaxValue));
            }
            else if (!StartedDragging && InputManager.IsMouseAvailable(this) && InputManager.MouseData.Y < ((int)Position.Y + Height) && Math.Abs(InputManager.MouseData.Y - ((int)Position.Y + Height)) < 20 && InputManager.MouseData.X < ((int)Position.X + Width) && Math.Abs(InputManager.MouseData.X - ((int)Position.X + Width)) < 20 && InputManager.LeftMousePressed)
            {
                Offset = new Vector2(Position.X + Width - InputManager.MouseData.X, Position.Y + Height - InputManager.MouseData.Y);
                StartedResizing = true;
                InputManager.ReserveMouse(this);
            }
            else if (StartedDragging || (!StartedResizing && InputManager.IsMouseAvailable(this) && Math.Abs(InputManager.MouseData.Y - (int)Position.Y) < 20 && InputManager.MouseData.X >= Position.X && InputManager.MouseData.X <= Position.X + WindowTexture.Width && InputManager.LeftMousePressed))
            {
                Position += InputManager.MouseDelta;

                StartedDragging = true;
                InputManager.ReserveMouse(this);
            }
        }
        */

    }
}
