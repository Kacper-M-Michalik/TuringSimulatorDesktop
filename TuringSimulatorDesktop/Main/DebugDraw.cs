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
        public static WindowView CurrentWindow;
        public static WindowView LastCreatedWindow;

        public static void Draw(GraphicsDevice Device, SpriteBatch spriteBatch)
        {
            Device.SetRenderTarget(null);

            spriteBatch.DrawString(GlobalGraphicsData.Font, "Cur Mouse Pos: " + InputManager.MouseData.X.ToString() + ", " + InputManager.MouseData.Y.ToString(), Vector2.One, GlobalGraphicsData.FontColor);
            spriteBatch.DrawString(GlobalGraphicsData.Font, "ViewPort: Width: " + GlobalGraphicsData.Device.Viewport.Width.ToString() + ", Y: " + GlobalGraphicsData.Device.Viewport.Height.ToString(), new Vector2(1f, 20f), GlobalGraphicsData.FontColor);
            if (CurrentWindow != null)  spriteBatch.DrawString(GlobalGraphicsData.Font, "Window Data: Pos : " + CurrentWindow.X.ToString() + ", " + CurrentWindow.Y.ToString(), new Vector2(1f, 40f), GlobalGraphicsData.FontColor);
            else spriteBatch.DrawString(GlobalGraphicsData.Font, "Window Data: Null", new Vector2(1f, 40f), GlobalGraphicsData.FontColor);
            if (CurrentWindow != null) spriteBatch.DrawString(GlobalGraphicsData.Font, "Window Data: Port X : " + CurrentWindow.Port.X.ToString() + ", Y: " + CurrentWindow.Port.Y.ToString() + "    Width: " + CurrentWindow.Port.Width.ToString() + ", Height: " + CurrentWindow.Port.Height.ToString(), new Vector2(1f, 60f), GlobalGraphicsData.FontColor);
            else spriteBatch.DrawString(GlobalGraphicsData.Font, "Window Data: Null", new Vector2(1f, 60f), GlobalGraphicsData.FontColor);
            if (CurrentWindow != null) spriteBatch.DrawString(GlobalGraphicsData.Font, "Window Data: Projection X : " + CurrentWindow.LastProjectionX.ToString() + ", Y: " + CurrentWindow.LastProjectionY.ToString() + "    Width: " + CurrentWindow.LastProjectionWidth.ToString() + ", Height: " + CurrentWindow.LastProjectionHeight.ToString(), new Vector2(1f, 80f), GlobalGraphicsData.FontColor);
            else spriteBatch.DrawString(GlobalGraphicsData.Font, "Window Data: Null", new Vector2(1f, 80f), GlobalGraphicsData.FontColor);
            if (LastCreatedWindow != null) spriteBatch.DrawString(GlobalGraphicsData.Font, "Last Window Data: Port X : " + LastCreatedWindow.Port.X.ToString() + ", Y: " + LastCreatedWindow.Port.Y.ToString() + "    Width: " + LastCreatedWindow.Port.Width.ToString() + ", Height: " + LastCreatedWindow.Port.Height.ToString(), new Vector2(1f, 100f), GlobalGraphicsData.FontColor);
            if (LastCreatedWindow != null) spriteBatch.DrawString(GlobalGraphicsData.Font, "Last Window Data: Projection X : " + LastCreatedWindow.LastProjectionX.ToString() + ", Y: " + LastCreatedWindow.LastProjectionY.ToString() + "    Width: " + LastCreatedWindow.LastProjectionWidth.ToString() + ", Height: " + LastCreatedWindow.LastProjectionHeight.ToString(), new Vector2(1f, 120f), GlobalGraphicsData.FontColor);
        }
    }
}
