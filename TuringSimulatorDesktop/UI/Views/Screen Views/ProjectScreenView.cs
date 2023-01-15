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
        ActionGroup Group;
        public bool IsMarkedForDeletion { get; set; }

        Icon Header;
        Icon Subheader;
        Icon Background;
            
        Label AppTitle;
        //screen drop down
        //view drop dwon
        //help dropdown

        HorizontalLayoutBox ToolbarLayout;

        TextureButton UndoButton;
        TextureButton RedoButton;

        TextureButton SaveButton;
        TextureButton SaveAllButton;

        TextureButton RunStateTableSourceButton;
        Label SelectedStateTableSourceLabel;

        Label ProjectTitle;


        List<Window> Windows;
        Window CurrentlyFocusedWindow;
        
        bool IsDragging;


        public Window LastActiveEditorWindow;

        public ProjectScreenView()
        {
            Group = InputManager.CreateActionGroup();
            Group.PollableObjects.Add(this);

            Windows = new List<Window>();

            Window Temp = new Window(new Vector2(0, 60), new Point(1200, 900), this);
            Windows.Add(Temp);
            LastActiveEditorWindow = Temp;
            Temp = new Window(new Vector2(1210, 60), new Point(400, 900), this);
            Temp.AddView(new FileBrowserView());
            Windows.Add(Temp);

            Background = new Icon(GlobalInterfaceData.Scheme.InteractableAccent);
            Subheader = new Icon(GlobalInterfaceData.Scheme.Background);
            Header = new Icon(GlobalInterfaceData.Scheme.Header);

            AppTitle = new Label();
            AppTitle.FontColor = GlobalInterfaceData.Scheme.FontColor;
            AppTitle.Font = GlobalInterfaceData.StandardBoldFont;
            AppTitle.FontSize = GlobalInterfaceData.Scale(20);
            AppTitle.Text = "T";
            AppTitle.DrawCentered = true;

            ToolbarLayout = new HorizontalLayoutBox();
            ToolbarLayout.Spacing = 0;

            UndoButton = new TextureButton(Group);
            //UndoButton.HighlightTexture
            UndoButton.HighlightOnMouseOver = true;
            UndoButton.OnClickedEvent += Undo;

            RedoButton = new TextureButton(Group);
            RedoButton.HighlightOnMouseOver = true;
            RedoButton.OnClickedEvent += Redo;

            SaveButton = new TextureButton(Group);
            SaveButton.HighlightOnMouseOver = true;
            SaveButton.OnClickedEvent += Save;

            SaveAllButton = new TextureButton(Group);
            SaveAllButton.HighlightOnMouseOver = true;
            SaveAllButton.OnClickedEvent += SaveAll;

            ToolbarLayout.AddElement(UndoButton);
            ToolbarLayout.AddElement(RedoButton);
            ToolbarLayout.AddElement(SaveButton);
            ToolbarLayout.AddElement(SaveAllButton);

            RunStateTableSourceButton = new TextureButton(Group);
            RunStateTableSourceButton.HighlightOnMouseOver = true;
            RunStateTableSourceButton.OnClickedEvent += Run;

            SelectedStateTableSourceLabel = new Label();
            SelectedStateTableSourceLabel.Text = "example source name";

            ProjectTitle = new Label();
            ProjectTitle.AutoSizeMesh = false;
            ProjectTitle.DrawCentered = true;
            ProjectTitle.FontSize = GlobalInterfaceData.Scale(14);
            ProjectTitle.FontColor = GlobalInterfaceData.Scheme.FontColor;
            ProjectTitle.Font = GlobalInterfaceData.StandardRegularFont;

            ScreenResize();
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
            //WindowGroupData NewBaseGroup = new WindowGroupData();
                
            //WindowGroupData NewWindowGroup = new WindowGroupData();
            Window NewWindow = new Window(new Vector2(100, 100), new Point(300, 400), this);
            //NewWindowGroup.ChildWindow = NewWindow;

            //if (BaseGroup != null) NewBaseGroup.SubGroups.Add(BaseGroup);
            //NewBaseGroup.SubGroups.Add(NewWindowGroup);

           // BaseGroup = NewBaseGroup;

            Windows.Add(NewWindow);
        }

        public void Undo(Button Sender)
        {

        }

        public void Redo(Button Sender)
        {

        }

        public void Save(Button Sender)
        {

        }

        public void SaveAll(Button Sender)
        {

        }

        public void Run(Button Sender)
        {

        }

        public override void Draw()
        {
            Background.Draw();
            Header.Draw();
            Subheader.Draw();

            ProjectTitle.Draw();
            AppTitle.Draw();

            UndoButton.Draw();
            RedoButton.Draw();

            SaveButton.Draw();
            SaveAllButton.Draw();

            RunStateTableSourceButton.Draw();
            SelectedStateTableSourceLabel.Draw();
            
            for (int i = 0; i < Windows.Count; i++)
            {
                Windows[i].Draw();
            }
            
        }

        public override void ScreenResize()
        {
            int Width = GlobalInterfaceData.Device.PresentationParameters.BackBufferWidth;
            int Height = GlobalInterfaceData.Device.PresentationParameters.BackBufferHeight;

            Group.Width = Width;
            Group.Height = Height;

            Background.Bounds = new Point(10000, 10000);
            Background.Position = Vector2.Zero;
            Subheader.Bounds = new Point(Width, UIUtils.ConvertFloatToMinInt(GlobalInterfaceData.Scale(28), 1));
            Subheader.Position = new Vector2(0, GlobalInterfaceData.Scale(32));
            Header.Bounds = new Point(Width, UIUtils.ConvertFloatToMinInt(GlobalInterfaceData.Scale(32), 1));
            Header.Position = Vector2.Zero;

            UndoButton.Bounds = GlobalInterfaceData.Scale(new Point(20, 20));
            RedoButton.Bounds = GlobalInterfaceData.Scale(new Point(20, 20));
            SaveButton.Bounds = GlobalInterfaceData.Scale(new Point(20, 20));
            SaveAllButton.Bounds = GlobalInterfaceData.Scale(new Point(20, 20));

            ToolbarLayout.Position = GlobalInterfaceData.Scale(new Vector2(16, 36));
            ToolbarLayout.UpdateLayout();

            RunStateTableSourceButton.Bounds = new Point(1,1);

            AppTitle.Bounds = GlobalInterfaceData.Scale(new Point(45, 32));
            AppTitle.Position = new Vector2(0,AppTitle.Bounds.Y / 2f);

            ProjectTitle.Bounds = GlobalInterfaceData.Scale(new Point(Width, 32));
            ProjectTitle.Position = GlobalInterfaceData.Scale(new Vector2(0, 16));
        }
    }
}
