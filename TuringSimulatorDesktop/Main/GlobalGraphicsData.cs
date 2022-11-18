using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop
{
    public static class GlobalGraphicsData
    {
        public static bool UIRequiresRedraw = true;

        public static GraphicsDevice Device;
        public static SpriteFont Font;
    }
}
