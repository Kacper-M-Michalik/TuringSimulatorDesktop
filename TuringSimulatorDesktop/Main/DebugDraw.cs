using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.Input;
using TuringSimulatorDesktop.UI;

namespace TuringSimulatorDesktop
{
    public static class DebugManager
    {
        public static Window CurrentWindow;

        public static void Draw(GraphicsDevice Device, SpriteBatch spriteBatch)
        {
            Device.SetRenderTarget(null);

            spriteBatch.DrawString(GlobalGraphicsData.Font, "Cur Mouse Pos: " + InputManager.MouseData.X.ToString() + ", " + InputManager.MouseData.Y.ToString(), Vector2.One, Color.Black);
            if (CurrentWindow != null)  spriteBatch.DrawString(GlobalGraphicsData.Font, "Window Data: Pos - " + CurrentWindow.Position.X.ToString() + ", " + CurrentWindow.Position.Y.ToString(), new Vector2(1f, 20f), Color.Black);
            else spriteBatch.DrawString(GlobalGraphicsData.Font, "Window Data: Null", new Vector2(1f, 20f), Color.Black);
        }
    }
}
