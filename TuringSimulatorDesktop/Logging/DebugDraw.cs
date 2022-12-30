using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.Input;
using TuringSimulatorDesktop.UI;
using FontStashSharp;

namespace TuringSimulatorDesktop
{
    public static class DebugManager
    {
        public static WindowView CurrentWindow;
        public static WindowView LastCreatedWindow;
        public static ActionGroup Group = InputManager.CreateActionGroup(0, 0, GlobalInterfaceData.MinimumApplicationWindowWidth, GlobalInterfaceData.MinimumApplicationWindowHeight);
        public static CheckBox DrawCheck = new CheckBox(20, 20, new Vector2(0, 100), Group);

        public static void Draw(GraphicsDevice Device, SpriteBatch spriteBatch, GameTime Time)
        {
            Group.IsPersistant = true;

            InputManager.SendActionGroupToFront(Group);
            DrawCheck.Draw();

            if (DrawCheck.Checked)
            {
                Device.SetRenderTarget(null);

                spriteBatch.DrawString(GlobalInterfaceData.StandardRegularFont.GetFont(12), "Cur Mouse Pos: " + InputManager.MouseData.X.ToString() + ", " + InputManager.MouseData.Y.ToString(), Vector2.Zero, GlobalInterfaceData.FontColor);
                spriteBatch.DrawString(GlobalInterfaceData.StandardRegularFont.GetFont(12), "ViewPort: Width: " + GlobalInterfaceData.Device.Viewport.Width.ToString() + ", Y: " + GlobalInterfaceData.Device.Viewport.Height.ToString(), new Vector2(1f, 20f), GlobalInterfaceData.FontColor);
                if (CurrentWindow != null) spriteBatch.DrawString(GlobalInterfaceData.StandardRegularFont.GetFont(12), "Window Data: Pos : " + CurrentWindow.X.ToString() + ", " + CurrentWindow.Y.ToString(), new Vector2(1f, 40f), GlobalInterfaceData.FontColor);
                else spriteBatch.DrawString(GlobalInterfaceData.StandardRegularFont.GetFont(12), "Window Data: Null", new Vector2(1f, 40f), GlobalInterfaceData.FontColor);
                if (CurrentWindow != null) spriteBatch.DrawString(GlobalInterfaceData.StandardRegularFont.GetFont(12), "Window Data: Port X : " + CurrentWindow.Port.X.ToString() + ", Y: " + CurrentWindow.Port.Y.ToString() + "    Width: " + CurrentWindow.Port.Width.ToString() + ", Height: " + CurrentWindow.Port.Height.ToString(), new Vector2(1f, 60f), GlobalInterfaceData.FontColor);
                else spriteBatch.DrawString(GlobalInterfaceData.StandardRegularFont.GetFont(12), "Window Data: Null", new Vector2(1f, 60f), GlobalInterfaceData.FontColor);
                if (CurrentWindow != null) spriteBatch.DrawString(GlobalInterfaceData.StandardRegularFont.GetFont(12), "Window Data: Projection X : " + CurrentWindow.LastProjectionX.ToString() + ", Y: " + CurrentWindow.LastProjectionY.ToString() + "    Width: " + CurrentWindow.LastProjectionWidth.ToString() + ", Height: " + CurrentWindow.LastProjectionHeight.ToString(), new Vector2(1f, 80f), GlobalInterfaceData.FontColor);
                else spriteBatch.DrawString(GlobalInterfaceData.StandardRegularFont.GetFont(12), "Window Data: Null", new Vector2(1f, 80f), GlobalInterfaceData.FontColor);
                if (LastCreatedWindow != null) spriteBatch.DrawString(GlobalInterfaceData.StandardRegularFont.GetFont(12), "Last Window Data: Port X : " + LastCreatedWindow.Port.X.ToString() + ", Y: " + LastCreatedWindow.Port.Y.ToString() + "    Width: " + LastCreatedWindow.Port.Width.ToString() + ", Height: " + LastCreatedWindow.Port.Height.ToString(), new Vector2(1f, 100f), GlobalInterfaceData.FontColor);
                if (LastCreatedWindow != null) spriteBatch.DrawString(GlobalInterfaceData.StandardRegularFont.GetFont(12), "Last Window Data: Projection X : " + LastCreatedWindow.LastProjectionX.ToString() + ", Y: " + LastCreatedWindow.LastProjectionY.ToString() + "    Width: " + LastCreatedWindow.LastProjectionWidth.ToString() + ", Height: " + LastCreatedWindow.LastProjectionHeight.ToString(), new Vector2(1f, 120f), GlobalInterfaceData.FontColor);
            }
        }
    }
}
