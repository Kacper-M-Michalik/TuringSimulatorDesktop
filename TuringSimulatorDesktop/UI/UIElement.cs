using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TuringSimulatorDesktop.UI
{
    public abstract class UIElement
    {
        public Vector2 Position;
        public bool Hidden;

        public UIElement()
        {
            AllUIElements.Add(this);
        }

        ~UIElement()
        {
            AllUIElements.Remove(this);
        }

        //public abstract void Update(GameTime gameTime);

        public abstract void Draw(SpriteBatch OwnerSpriteBatch, RenderTarget2D PreviousRenderTarget);

        public static List<UIElement> AllUIElements = new List<UIElement>();
        public static Dictionary<TextureLookupKey, Texture2D> TextureLookup = new Dictionary<TextureLookupKey, Texture2D>();
    }

    public enum TextureLookupKey 
    { 
        StateNodeBackground,
        MenuBar
    }
}
