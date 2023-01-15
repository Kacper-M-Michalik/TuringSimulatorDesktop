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
using TuringSimulatorDesktop.UI.Prefabs;

namespace TuringSimulatorDesktop
{
    public static class DebugManager
    {
        public static Window CurrentWindow;
        public static Window LastCreatedWindow;
        static ActionGroup Group;
        //public static CheckBox DrawCheck = new CheckBox(20, 20, new Vector2(0, 100), Group);
        static DebugMenu Menu;

        static DebugManager()
        {
            //Group = InputManager.CreateActionGroup(0, 0, GlobalInterfaceData.MinimumApplicationWindowWidth, GlobalInterfaceData.MinimumApplicationWindowHeight);
            //Group.IsPersistant = true;

            //Menu = new DebugMenu(Group);
        }

        public static void GenerateRecentFiles()
        {
            if (GlobalProjectAndUserData.UserData == null) return; 

            Random Rand = new Random((int)DateTime.UtcNow.Ticks);

            for (int i = 0; i < 10; i++)
            {
                GlobalProjectAndUserData.UserData.RecentlyAccessedFiles.Add(new FileInfoWrapper("File: " + i.ToString(), Rand.NextInt64().ToString(), DateTime.Now));
            }
            GlobalProjectAndUserData.SaveUserData();
        }

        public static void Draw(GraphicsDevice Device, SpriteBatch spriteBatch, GameTime Time)
        {
            Menu.Draw();

           /* DrawCheck.Draw();

            // if (DrawCheck.Checked)
            // {
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
           // }
           */
        }
    }

    public class DebugMenu : IVisualElement, IPollable
    {
        public bool IsActive { get; set; } = true;
        int Width = 200;
        int Height = 100;

        Icon Background;
        Label MouseLabel;
        Label ViewportLabel;
        CheckBox DrawCheck;

        ActionGroup Group;
        public bool IsMarkedForDeletion { get; set; }

        public DebugMenu(ActionGroup group)
        {
            Group = group;

            Background = new Icon(Width, Height, Vector2.Zero, GlobalInterfaceData.Scheme.UIOverlayDebugColor1);
            MouseLabel = new Label(Vector2.Zero, GlobalInterfaceData.MediumRegularFont);
            MouseLabel.FontSize = 14;
            ViewportLabel = new Label(Vector2.Zero, GlobalInterfaceData.MediumRegularFont);
            ViewportLabel.FontSize = 14;
        }

        public Vector2 position;
        public Vector2 Position { get => position; set => position = value; }

        Point bounds;
        public Point Bounds { get => bounds; set => bounds = value; }

        public void Draw(Viewport? BoundPort = null)
        {
            InputManager.SendActionGroupToFront(Group);
            DrawCheck.Draw();

            if (IsActive)
            {   
                MouseLabel.Text = "Cur Mouse Pos: " + InputManager.MouseData.X.ToString() + ", " + InputManager.MouseData.Y.ToString();
                ViewportLabel.Text = "ViewPort: Width: " + GlobalInterfaceData.Device.Viewport.Width.ToString() + ", Y: " + GlobalInterfaceData.Device.Viewport.Height.ToString();
                Background.Draw();
                MouseLabel.Draw();
                ViewportLabel.Draw();
            }
        }
        
        public bool IsMouseOver()
        {
            return (IsActive && InputManager.MouseData.X >= Position.X && InputManager.MouseData.X <= Position.X + Width && InputManager.MouseData.Y >= Position.Y && InputManager.MouseData.Y <= Position.Y + Height);
        }

        public void PollInput(bool IsInActionGroupFrame)
        {
            if (IsInActionGroupFrame && IsActive && IsMouseOver())
            {

            }
        }
    }

}
