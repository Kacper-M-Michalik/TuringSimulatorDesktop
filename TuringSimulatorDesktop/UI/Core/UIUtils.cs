using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.UI
{
    public static class UIUtils
    {
        public static int ConvertFloatToMinInt(float Value)
        {
            return Convert.ToInt32(Math.Clamp(MathF.Round(Value /*, MidpointRounding.AwayFromZero*/), 1f, float.PositiveInfinity));
        }

        public static int ConvertFloatToInt(float Value)
        {
            return Convert.ToInt32(Value);//MathF.Round(Value, MidpointRounding.AwayFromZero));
        }

        public static Viewport CalculateOverlapPort(Viewport Left, Viewport Right)
        {
            throw new Exception();
        }

        public static bool IsDefaultViewport(Viewport Port)
        {
            return (Port.X == 0 && Port.Y == 0 && Port.Width == 0 && Port.Height == 0);
        }

    }

}
