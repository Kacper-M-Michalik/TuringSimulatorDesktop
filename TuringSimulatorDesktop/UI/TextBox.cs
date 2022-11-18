using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.UI
{
    public class TextBox : UIElement, IPoll, IClickable
    {
        public string Text;

        public void Clicked()
        {
            throw new NotImplementedException();
        }

        public bool IsMouseOver()
        {
            throw new NotImplementedException();
        }

        public void Poll()
        {
            throw new NotImplementedException();
        }

        public override void Draw(SpriteBatch OwnerSpriteBatch, RenderTarget2D PreviousRenderTarget)
        {
            throw new NotImplementedException();
        }

    }
}
