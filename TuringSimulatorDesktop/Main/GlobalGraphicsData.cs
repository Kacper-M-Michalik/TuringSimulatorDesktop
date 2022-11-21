using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.UI;

namespace TuringSimulatorDesktop
{
    public static class GlobalGraphicsData
    {
        public static bool UIRequiresRedraw = true;

        public static MainWindow BaseWindow;
        public static GameTime Time;

        public static GraphicsDevice Device;
        public static SpriteFont Font;

        public static int TabHeight;
        public static Color BackgroundColor = Color.White;
        public static Color AccentColor = Color.DarkSlateGray;
        public static Color FontColor = Color.Black;

        public static Dictionary<TextureLookupKey, Texture2D> TextureLookup = new Dictionary<TextureLookupKey, Texture2D>();
    }

    public enum TextureLookupKey
    {
        StateNodeBackground,
        MenuBar
    }
}
