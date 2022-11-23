using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Taskbar;

namespace TuringSimulatorDesktop.UI
{
    public class ProjectScreenView : View, IPoll
    {
        List<IRenderable> RenderElements;
        Viewport Port;

        Mesh ScreenBackground;
        Mesh ToolbarBackground;

        //Button ExitProjectButton;
        //Button WindowAddDropDown;
        Button AddWindowButton;

        List<WindowView> Windows;
        WindowView CurrentlyFocusedWindow;

        bool IsDragging;

        // DEBUG
        WindowView DebugWindow;

        public ProjectScreenView()
        {
            Port = new Viewport(0, 0, GlobalGraphicsData.Device.PresentationParameters.BackBufferWidth, GlobalGraphicsData.Device.PresentationParameters.BackBufferHeight);
            RenderElements = new List<IRenderable>();

            ToolbarBackground = Mesh.CreateRectangle(Vector2.Zero, GlobalGraphicsData.Device.PresentationParameters.BackBufferWidth, GlobalGraphicsData.ToolbarHeight, GlobalGraphicsData.AccentColor);
            ScreenBackground = Mesh.CreateRectangle(Vector2.Zero, GlobalGraphicsData.Device.PresentationParameters.BackBufferWidth, GlobalGraphicsData.Device.PresentationParameters.BackBufferHeight, GlobalGraphicsData.BackgroundColor);

            AddWindowButton = new Button(new Vector2(20f, 20f), Mesh.CreateRectangle(Vector2.Zero, 40, 20, Color.Yellow), ElementCreateType.Persistent);
            AddWindowButton.ClickEvent += AddNewWindow;

            RenderElements.Add(AddWindowButton);

            Windows = new List<WindowView>();

            WindowView Win1 = new WindowView(200, 200);
            Win1.SetWindowPosition(300, 0);
            Windows.Add(Win1);
            DebugWindow = Win1;

            WindowView Win2 = new WindowView(100, 400);
            Win2.MoveWindowPosition(0, 25);
            Windows.Add(Win2);

            InputManager.RegisterPollableObjectOnQueuePersistent(this);
        }

        public void PollInput()
        {
            if (DebugWindow != null) DebugWindow.ViewResize(Convert.ToInt32(Math.Clamp(350 * Math.Cos(GlobalGraphicsData.Time.TotalGameTime.TotalSeconds), DebugWindow.TabHeight, 350f)), Convert.ToInt32(Math.Clamp(300 * Math.Sin(GlobalGraphicsData.Time.TotalGameTime.TotalSeconds), DebugWindow.TabHeight, 300f)));

            int i = Windows.Count - 1;
            bool AssignedFocus = false;
            if (!IsDragging)
            {
                while (!AssignedFocus && i > -1)
                {
                    if (Windows[i].IsMouseOverWindow())
                    {
                        AssignedFocus = true;
                        CurrentlyFocusedWindow = Windows[i];
                        DebugManager.CurrentWindow = CurrentlyFocusedWindow;

                        if (InputManager.LeftMousePressed)
                        {
                            Windows.RemoveAt(i);
                            Windows.Add(CurrentlyFocusedWindow);
                        }
                    }
                    i--;
                }
                if (!AssignedFocus) DebugManager.CurrentWindow = null;
            }

            if (IsDragging)
            {
                CurrentlyFocusedWindow.MoveWindowPosition(InputManager.MouseDeltaX, InputManager.MouseDeltaY);

                if (InputManager.LeftMouseReleased)
                {
                    //do snap here
                    IsDragging = false;
                }
            }

            if (!IsDragging && InputManager.LeftMousePressed && CurrentlyFocusedWindow != null && CurrentlyFocusedWindow.IsMouseOverTab())
            {
                IsDragging = true;
            }
        }

        public void AddNewWindow(Button Sender)
        {
            WindowView Win = new WindowView(350, 300);
            //Win.AddView(new TextProgrammingView());
            Windows.Add(Win);
        }

        public override void Draw()
        {
            GlobalMeshRenderer.Draw(ScreenBackground, Port);
            for (int i = 0; i < Windows.Count; i++)
            {
                Windows[i].Draw();
            }

            //draw snap pointss?

            GlobalMeshRenderer.Draw(ToolbarBackground, Port);
            GlobalMeshRenderer.Draw(RenderElements, Port);
        }

        public override void ViewResize(int NewWidth, int NewHeight)
        {
            ToolbarBackground = Mesh.CreateRectangle(Vector2.Zero, GlobalGraphicsData.Device.PresentationParameters.BackBufferWidth, GlobalGraphicsData.ToolbarHeight, GlobalGraphicsData.AccentColor);
            ScreenBackground = Mesh.CreateRectangle(Vector2.Zero, GlobalGraphicsData.Device.PresentationParameters.BackBufferWidth, GlobalGraphicsData.Device.PresentationParameters.BackBufferHeight, GlobalGraphicsData.BackgroundColor);

            Port.Width = NewWidth;
            Port.Height = NewHeight;
        }
    }
}
