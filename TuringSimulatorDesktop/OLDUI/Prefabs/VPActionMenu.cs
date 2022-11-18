using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.UI
{
    public class VPActionMenu : UIElement
    {
        public override void Draw(SpriteBatch OwnerSpriteBatch, RenderTarget2D CurrentRenderTarget)
        {
            if (!Hidden)
            {
                OwnerSpriteBatch.Draw(TextureLookup[TextureLookupKey.MenuBar], Position, Color.White);
            }
        }
    }
}
