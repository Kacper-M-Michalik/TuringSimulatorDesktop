using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TuringSimulatorDesktop.UI.Prefabs;
using TuringCore;

namespace TuringSimulatorDesktop.UI
{
    public class ProjectScreenView : ScreenView, IPollable
    {
        int Width, Height;
        ActionGroup Group;
        public bool IsMarkedForDeletion { get; set; }

        Icon Header;
        Icon Background;
        Label AppTitle;

        Label ProjectTitle;

       // FileBrowserView Browser;

        //Button ExitProjectButton;
        //Button WindowAddDropDown;
        //TextLabel ProjectTitle;

        WindowGroupData BaseGroup;
        WindowGroupConnection Connections;

        List<Window> Windows;
        Window CurrentlyFocusedWindow;

        bool IsDragging;


        public ProjectScreenView()
        {
            Group = InputManager.CreateActionGroup();
            ViewResize(GlobalInterfaceData.Device.PresentationParameters.BackBufferWidth, GlobalInterfaceData.Device.PresentationParameters.BackBufferHeight);

            Windows = new List<Window>();

            Window Temp = new Window(new Vector2(100, 100), new Point(800, 600));
            Temp.AddView(new FileBrowserView());
            Temp.AddView(new FileBrowserView());
            //Temp.AddView();
            Windows.Add(Temp);

            AppTitle = new Label();
            AppTitle.Font = GlobalInterfaceData.StandardBoldFont;
            AppTitle.FontSize = 20;
            AppTitle.Text = "T";
            AppTitle.Position = Vector2.Zero;

            ProjectTitle = new Label();
            ProjectTitle.FontSize = 14;
            ProjectTitle.Position = new Vector2(1920 / 2, 0);

            Group.PollableObjects.Add(this);
        }

        public void UpdatedProject(object sender, EventArgs e)
        {
            ProjectTitle.Text = GlobalProjectAndUserData.ProjectData.ProjectName;
            //do clearing etc here
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
                CurrentlyFocusedWindow.Position += new Vector2(InputManager.MouseDeltaX, InputManager.MouseDeltaY);

                if (InputManager.LeftMouseReleased)
                {
                    //do snap here
                    IsDragging = false;
                }
            }

            if (!IsDragging && InputManager.LeftMousePressed && CurrentlyFocusedWindow != null && CurrentlyFocusedWindow.IsMouseOverHeader())
            {
                IsDragging = true;
            }
        }

        public void CreateWindow()
        {
            WindowGroupData NewBaseGroup = new WindowGroupData();
                
            WindowGroupData NewWindowGroup = new WindowGroupData();
            Window NewWindow = new Window(new Vector2(100, 100), new Point(300, 400));
            NewWindowGroup.ChildWindow = NewWindow;

            if (BaseGroup != null) NewBaseGroup.SubGroups.Add(BaseGroup);
            NewBaseGroup.SubGroups.Add(NewWindowGroup);

            BaseGroup = NewBaseGroup;

            Windows.Add(NewWindow);
        }
        public override void Draw()
        {
            Background.Draw();
            Header.Draw();
            ProjectTitle.Draw();
            AppTitle.DrawFrame = !AppTitle.DrawFrame;
            AppTitle.Draw();

            for (int i = 0; i < Windows.Count; i++)
            {
                Windows[i].Draw();
            }
        }

        public override void ViewResize(int NewWidth, int NewHeight)
        {
            Width = NewWidth;
            Height = NewHeight;
            Group.Width = NewWidth;
            Group.Height = NewHeight;

            Background = new Icon(Width, Height, Vector2.Zero, GlobalInterfaceData.Scheme.Background);
            Header = new Icon(Width, 32, Vector2.Zero, GlobalInterfaceData.Scheme.Header);
        }

        public override void ViewPositionSet(int X, int Y)
        {
        }
    }
}
