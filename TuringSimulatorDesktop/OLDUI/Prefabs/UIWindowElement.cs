using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TuringSimulatorDesktop.UI;

namespace TuringSimulatorDesktop
{
    /*
    public class RenderWindow : ParentWindow
    {
        public List<UIElement> UIElements = new List<UIElement>();
        public Color BackgroundColor = Color.White;

        public RenderWindow(GraphicsDevice graphicsDevice, int SetWidth, int SetHeight) : base(graphicsDevice, SetWidth, SetHeight)
        {            
        }

        public override Vector2 GetBounds()
        {
            throw new NotImplementedException();
        }

        public override void GetView()
        {
            RendererGraphicsDevice.SetRenderTarget(ViewRenderTarget);
            RendererGraphicsDevice.Clear(BackgroundColor);

            LocalSpriteBatch.Begin();
            for (int i = 0; i < UIElements.Count; i++)
            {
                UIElements[i].Draw(LocalSpriteBatch, ViewRenderTarget);
            }
            LocalSpriteBatch.End();
        }
    }
    */
}
