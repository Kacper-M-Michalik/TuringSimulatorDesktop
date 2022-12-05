using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        TextLabel ProjectTitle;

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

            ProjectTitle = new TextLabel("TEST ASDASSADSDAASD");

            Windows = new List<WindowView>();

            WindowView Win1 = new WindowView(200, 200, this);
            Win1.ViewPositionSet(300, 0);
            Windows.Add(Win1);
            DebugWindow = Win1;

            WindowView Win2 = new WindowView(100, 400, this);
            Win2.ViewPositionSet(0, 25);
            Windows.Add(Win2);

            InputManager.RegisterPollableObjectOnQueuePersistent(this);
        }

        public void PollInput()
        {
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
                CurrentlyFocusedWindow.ViewPositionMove(InputManager.MouseDeltaX, InputManager.MouseDeltaY);

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

        public override void Draw()
        {
            GlobalMeshRenderer.Draw(ScreenBackground, Port);
            GlobalMeshRenderer.Draw(ToolbarBackground, Port);
            GlobalMeshRenderer.Draw(RenderElements, Port);
            GlobalMeshRenderer.Draw(ProjectTitle, Port);
            for (int i = 0; i < Windows.Count; i++)
            {
                Windows[i].Draw();
            }
        }

        public void WindowClosing(WindowView Child)
        {
            Windows.Remove(Child);
        }

        public override void ViewResize(int NewWidth, int NewHeight)
        {
            ToolbarBackground = Mesh.CreateRectangle(Vector2.Zero, GlobalGraphicsData.Device.PresentationParameters.BackBufferWidth, GlobalGraphicsData.ToolbarHeight, GlobalGraphicsData.AccentColor);
            ScreenBackground = Mesh.CreateRectangle(Vector2.Zero, GlobalGraphicsData.Device.PresentationParameters.BackBufferWidth, GlobalGraphicsData.Device.PresentationParameters.BackBufferHeight, GlobalGraphicsData.BackgroundColor);

            Port.Width = NewWidth;
            Port.Height = NewHeight;
        }

        public override void ViewPositionSet(int X, int Y)
        {
        }
    }
}
