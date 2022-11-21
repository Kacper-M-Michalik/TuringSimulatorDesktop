using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI
{
    /*
    public abstract class ParentWindow : UIElement
    {
        protected SpriteBatch LocalSpriteBatch;
        protected RenderTarget2D ViewRenderTarget;
        protected GraphicsDevice RendererGraphicsDevice;

        public ParentWindow(GraphicsDevice graphicsDevice, int SetWidth, int SetHeight)
        {
            LocalSpriteBatch = new SpriteBatch(graphicsDevice);
            ViewRenderTarget = new RenderTarget2D(graphicsDevice, SetWidth, SetHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            RendererGraphicsDevice = graphicsDevice;
        }

        public abstract void GetView();

        protected bool MouseWithinBounds()
        {
            return InputManager.MouseData.X >= Position.X && InputManager.MouseData.X <= Position.X + ViewRenderTarget.Width && InputManager.MouseData.Y >= Position.Y && InputManager.MouseData.Y <= Position.Y + ViewRenderTarget.Height;
        }

        public override void Draw(SpriteBatch OwnerSpriteBatch, RenderTarget2D CurrentRenderTarget)
        {
            GetView();
            RendererGraphicsDevice.SetRenderTarget(CurrentRenderTarget);
            OwnerSpriteBatch.Draw(ViewRenderTarget, Position, Color.White);
        }
    }
    */
}
