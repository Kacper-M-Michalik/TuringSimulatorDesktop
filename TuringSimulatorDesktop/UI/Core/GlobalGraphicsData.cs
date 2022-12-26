using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.UI;
using FontStashSharp;

namespace TuringSimulatorDesktop
{
    public static class GlobalInterfaceData
    {
        public static bool UIRequiresRedraw = true;

        public static MainWindow BaseWindow;
        public static Viewport FullscreenViewport;
        public static GameTime Time;

        public static GraphicsDevice Device;
        public static Effect UIEffect;
        public static FontSystem Fonts;

        public static double TypeWaitTimeMiliseconds = 100;

        public static int ToolbarHeight = 35;
        public static int WindowTabHeight = 25;

        public static Color BackgroundColor = Color.SlateGray;
        public static Color AccentColor = Color.Black;
        public static Color FontColor = Color.Black;

        public static Color UIOverlayDebugColor = new Color(124, 0, 124, 255);
        public static Color UIOverlayDebugColorHighlight = new Color(255, 124, 255, 255);
        public static Color DebugColor = Color.Yellow;



        public static Dictionary<UILookupKey, Texture2D> TextureLookup = new Dictionary<UILookupKey, Texture2D>();

        public static void BakeTextures()
        {
            //manually implement per ui
            //paint background, paint on icons, text
        }

    }

    public enum UILookupKey
    {
        StateNodeBackground,
        MenuBar
    }
}
