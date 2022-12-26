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
    public class ProjectScreenView : View, IPollable
    {
        ActionGroup Group;

        UIMesh ScreenBackground;
        UIMesh ToolbarBackground;

        //Button ExitProjectButton;
        //Button WindowAddDropDown;
        TextLabel ProjectTitle;

        WindowGroupData BaseGroup;
        WindowGroupConnection Connections;

        List<WindowView> Windows;
        WindowView CurrentlyFocusedWindow;

        bool IsDragging;

        // DEBUG
        WindowView DebugWindow;

        public ProjectScreenView()
        {
            Group = InputManager.CreateActionGroup();
            ViewResize(GlobalInterfaceData.Device.PresentationParameters.BackBufferWidth, GlobalInterfaceData.Device.PresentationParameters.BackBufferHeight);

            ToolbarBackground = UIMesh.CreateRectangle(Vector2.Zero, GlobalInterfaceData.Device.PresentationParameters.BackBufferWidth, GlobalInterfaceData.ToolbarHeight, GlobalInterfaceData.AccentColor);
            ScreenBackground = UIMesh.CreateRectangle(Vector2.Zero, GlobalInterfaceData.Device.PresentationParameters.BackBufferWidth, GlobalInterfaceData.Device.PresentationParameters.BackBufferHeight, GlobalInterfaceData.BackgroundColor);

            ProjectTitle = new TextLabel(Vector2.One, 8, "LOREM ipsum wubbalubbadub dub");

            Windows = new List<WindowView>();

            CreateWindow();
            CreateWindow();

            /*
            WindowView Win1 = new WindowView(200, 200, this, BaseGroup);
            Win1.ViewPositionSet(300, 0);
            Windows.Add(Win1);
            DebugWindow = Win1;

            WindowView Win2 = new WindowView(100, 400, this, BaseGroup);
            Win2.ViewPositionSet(0, 25);
            Windows.Add(Win2);
            */

            Group.PollableObjects.Add(this);
        }

        public void PollInput(bool IsInActionFrameGroup)
        {
            for (int i = Windows.Count - 1; i > -1; i--)
            {
                if (Windows[i].IsMarkedForDeletion) Windows.RemoveAt(i);
            }

            int j = Windows.Count - 1;
            bool AssignedFocus = false;
            if (!IsDragging)
            {
                while (!AssignedFocus && j > -1)
                {
                    if (Windows[j].IsMouseOverWindow())
                    {
                        AssignedFocus = true;
                        CurrentlyFocusedWindow = Windows[j];
                        DebugManager.CurrentWindow = CurrentlyFocusedWindow;

                        if (InputManager.LeftMousePressed)
                        {
                            Windows.RemoveAt(j);
                            Windows.Add(CurrentlyFocusedWindow);
                        }
                    }
                    j--;
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
            GlobalUIRenderer.Draw(ScreenBackground);
            GlobalUIRenderer.Draw(ToolbarBackground);
            //GlobalMeshRenderer.Draw(ProjectTitle, Port);
            for (int i = 0; i < Windows.Count; i++)
            {
                Windows[i].Draw();
            }
        }

        public void CreateWindow()
        {
            WindowGroupData NewBaseGroup = new WindowGroupData();
                
            WindowGroupData NewWindowGroup = new WindowGroupData();
            WindowView NewWindow = new WindowView(100, 100, this);
            NewWindowGroup.ChildWindow = NewWindow;

            if (BaseGroup != null) NewBaseGroup.SubGroups.Add(BaseGroup);
            NewBaseGroup.SubGroups.Add(NewWindowGroup);

            BaseGroup = NewBaseGroup;

            Windows.Add(NewWindow);
        }

        public override void ViewResize(int NewWidth, int NewHeight)
        {
            Group.Width = NewWidth;
            Group.Height = NewHeight;

            ToolbarBackground = UIMesh.CreateRectangle(Vector2.Zero, NewWidth, GlobalInterfaceData.ToolbarHeight, GlobalInterfaceData.AccentColor);
            ScreenBackground = UIMesh.CreateRectangle(Vector2.Zero, NewWidth, NewHeight, GlobalInterfaceData.BackgroundColor); 
        }

        public override void ViewPositionSet(int X, int Y)
        {
        }
    }
}
