using System;
using System.Collections.Generic;
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

        TextureButton CloseButton;
        TextureButton FullscreenIcon;
        TextureButton MinimiseIcon;

        HorizontalLayoutBox ToolbarLayout;

        TextureButton UndoButton;
        TextureButton RedoButton;

        TextureButton SaveButton;
        TextureButton SaveAllButton;

        RunProjectItem RunProjectButton;

        Label ProjectTitle;
        
        List<Window> Windows;


        Window CurrentlyFocusedWindow;        
        bool IsDragging;

        public Window LastActiveWindow;
        public IRunnable ActiveEditorView;


        public ProjectScreenView()
        {
            Group = InputManager.CreateActionGroup();
            Group.PollableObjects.Add(this);

            CloseButton = new TextureButton(45, 32, new Vector2(GlobalInterfaceData.Device.PresentationParameters.BackBufferWidth - 45, 0), Group);
            CloseButton.OnClickedEvent += Close;
            CloseButton.HighlightOnMouseOver = true;
            CloseButton.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.CloseApplicationIcon];
            CloseButton.HighlightTexture = GlobalInterfaceData.TextureLookup[UILookupKey.CloseApplicationHighlightIcon];

            FullscreenIcon = new TextureButton(45, 32, new Vector2(GlobalInterfaceData.Device.PresentationParameters.BackBufferWidth - 90, 0), Group);
            FullscreenIcon.OnClickedEvent += Maximise;
            FullscreenIcon.HighlightOnMouseOver = true;
            FullscreenIcon.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.FullscreenIcon];
            FullscreenIcon.HighlightTexture = GlobalInterfaceData.TextureLookup[UILookupKey.FullscreenHighlightIcon];

            MinimiseIcon = new TextureButton(45, 32, new Vector2(GlobalInterfaceData.Device.PresentationParameters.BackBufferWidth - 135, 0), Group);
            MinimiseIcon.OnClickedEvent += Minimise;
            MinimiseIcon.HighlightOnMouseOver = true;
            MinimiseIcon.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.MinimiseIcon];
            MinimiseIcon.HighlightTexture = GlobalInterfaceData.TextureLookup[UILookupKey.MinimiseHighlightIcon];

            Windows = new List<Window>();

            Window Temp = new Window(new Vector2(0, 70), new Point(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 410, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 70), this);
            Windows.Add(Temp);
            LastActiveWindow = Temp;
            Temp = new Window(new Vector2(Temp.Bounds.X + 10, 70), new Point(400, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 70), this);
            Temp.AddView(new FileBrowserView());
            Windows.Add(Temp);

            Background = new Icon(GlobalInterfaceData.Scheme.InteractableAccent);
            Subheader = new Icon(GlobalInterfaceData.Scheme.Background);
            Header = new Icon(GlobalInterfaceData.Scheme.Header);

            AppTitle = new Label();
            AppTitle.FontColor = Color.White;
            AppTitle.Font = GlobalInterfaceData.StandardBoldFont;
            AppTitle.FontSize = GlobalInterfaceData.Scale(20);
            AppTitle.Text = "T";
            AppTitle.DrawCentered = true;

            ToolbarLayout = new HorizontalLayoutBox();
            ToolbarLayout.Spacing = 5;

            UndoButton = new TextureButton(Group);
            UndoButton.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.UndoIcon];
            UndoButton.HighlightOnMouseOver = true;
            UndoButton.OnClickedEvent += Undo;

            RedoButton = new TextureButton(Group);
            RedoButton.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.RedoIcon];
            RedoButton.HighlightOnMouseOver = true;
            RedoButton.OnClickedEvent += Redo;

            SaveButton = new TextureButton(Group);
            SaveButton.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.Save];
            SaveButton.HighlightTexture = GlobalInterfaceData.TextureLookup[UILookupKey.SaveHighlight];
            SaveButton.HighlightOnMouseOver = true;
            SaveButton.OnClickedEvent += Save;

            SaveAllButton = new TextureButton(Group);
            SaveAllButton.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.SaveAll];
            SaveAllButton.HighlightTexture = GlobalInterfaceData.TextureLookup[UILookupKey.SaveAllHighlight];
            SaveAllButton.HighlightOnMouseOver = true;
            SaveAllButton.OnClickedEvent += SaveAll;

            RunProjectButton = new RunProjectItem(this, Group);

            ToolbarLayout.AddElement(UndoButton);
            ToolbarLayout.AddElement(RedoButton);
            ToolbarLayout.AddElement(SaveButton);
            ToolbarLayout.AddElement(SaveAllButton);
            ToolbarLayout.AddElement(RunProjectButton);

            ProjectTitle = new Label();
            ProjectTitle.AutoSizeMesh = false;
            ProjectTitle.DrawCentered = true;
            ProjectTitle.FontSize = GlobalInterfaceData.Scale(14);
            ProjectTitle.FontColor = GlobalInterfaceData.Scheme.FontColorBright;
            ProjectTitle.Font = GlobalInterfaceData.StandardRegularFont;

            ScreenResize();
        }

        public void UpdatedProject(object sender, EventArgs e)
        {
            ProjectTitle.Text = GlobalProjectAndUserData.ProjectData.ProjectName;
        }

        public void PollInput(bool IsInActionFrameGroup)
        {
            //Delete any windows that have been marked for deletion
            for (int i = Windows.Count - 1; i > -1; i--)
            {
                if (Windows[i].IsMarkedForDeletion) Windows.RemoveAt(i);
            }

            //Assigns the window the mouse is currently over as the main focus window
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

                        //Moves the last pressed window to the top of the window list, making it get drawn on top of all other windows
                        if (InputManager.LeftMousePressed)
                        {
                            Windows.RemoveAt(j);
                            Windows.Add(CurrentlyFocusedWindow);
                        }
                    }
                    j--;
                }
            }

            //If a window is currently being dragged, this updates the windows position and checks if dragging has finished
            if (IsDragging)
            {
                CurrentlyFocusedWindow.Position += new Vector2(InputManager.MouseDeltaX, InputManager.MouseDeltaY);

                if (InputManager.LeftMouseReleased)
                {
                    IsDragging = false;
                }
            }

            //If a windwo drag are is selected and no other windwo is being dragged currently, start dragging the newly selected window
            if (!IsDragging && InputManager.LeftMousePressed && CurrentlyFocusedWindow != null && CurrentlyFocusedWindow.IsMouseOverHeader())
            {
                IsDragging = true;
            }
        }

        //Create a new window
        public Window CreateWindow(int X, int Y, int Width, int Height)
        {
            Window NewWindow = new Window(new Vector2(X, Y), new Point(Width, Height), this);

            Windows.Add(NewWindow);

            return NewWindow;
        }

        public void DeleteWindow(Window DeleteWindow)
        {
            Windows.Remove(DeleteWindow);
            DeleteWindow.Close();
        }

        //Application window controls
        public void Minimise(Button Sender)
        {
            GlobalInterfaceData.MainWindow.MinimiseWindow();
        }
        public void Maximise(Button Sender)
        {
            GlobalInterfaceData.MainWindow.MaximiseWindow();
        }
        public void Close(Button Sender)
        {
            GlobalInterfaceData.MainWindow.Exit();
        }

        //When the user clicks the undo or redo buttons, th currently focused window is made to redo/undo the last action
        public void Undo(Button Sender)
        {
            IUndoRedoable Target = (IUndoRedoable)CurrentlyFocusedWindow.CurrentView;
            if (Target != null) Target.Undo();
        }

        public void Redo(Button Sender)
        {
            IUndoRedoable Target = (IUndoRedoable)CurrentlyFocusedWindow.CurrentView;
            if (Target != null) Target.Redo();
        }

        //Saves file opened in currently focused window
        public void Save(Button Sender)
        {
            if (LastActiveWindow.CurrentView is ISaveable) ((ISaveable)LastActiveWindow.CurrentView).Save();            
        }

        //Saves all files in all windows
        public void SaveAll(Button Sender)
        {
            for (int i = 0; i < Windows.Count; i++)
            {
                for (int j = 0; j < Windows[i].Headers.Count; j++)
                {
                    if (Windows[i].Headers[j].View is ISaveable) ((ISaveable)Windows[i].Headers[j].View).Save();
                }
            }
        }

        //Sets a windwo to be the program source window of the run program button
        public void SetActiveEditorWindow(IRunnable RunnableView)
        {
            //set button to clickable here
            RunProjectButton.UpdateTarget(RunnableView.Title);
            ActiveEditorView = RunnableView;
        }

        //When the run button is clicked, a new Turign Exdection window is created with the currently selected program source
        public void Run(Button Sender)
        {
            if (ActiveEditorView != null)
            {
                LastActiveWindow.AddView(new TuringMachineView(ActiveEditorView.OpenFileID));
            }
        }

        //Draw Application header, toolbar and background
        public override void Draw()
        {
            Background.Draw();
            Header.Draw();
            Subheader.Draw();

            ProjectTitle.Draw();
            AppTitle.Draw();

            CloseButton.Draw();
            FullscreenIcon.Draw();
            MinimiseIcon.Draw();

            UndoButton.Draw();
            RedoButton.Draw();

            SaveButton.Draw();
            SaveAllButton.Draw();

            RunProjectButton.Draw();
            
            for (int i = 0; i < Windows.Count; i++)
            {
                Windows[i].Draw();
            }
            
        }

        //Resizes and repositions UI on an applciation window resize 
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

            CloseButton.Position = new Vector2(GlobalInterfaceData.Device.PresentationParameters.BackBufferWidth - 45, 0);
            FullscreenIcon.Position = new Vector2(GlobalInterfaceData.Device.PresentationParameters.BackBufferWidth - 90, 0);
            MinimiseIcon.Position = new Vector2(GlobalInterfaceData.Device.PresentationParameters.BackBufferWidth - 135, 0);

            UndoButton.Bounds = GlobalInterfaceData.Scale(new Point(20, 20));
            RedoButton.Bounds = GlobalInterfaceData.Scale(new Point(20, 20));
            SaveButton.Bounds = GlobalInterfaceData.Scale(new Point(20, 20));
            SaveAllButton.Bounds = GlobalInterfaceData.Scale(new Point(20, 20));

            ToolbarLayout.Position = GlobalInterfaceData.Scale(new Vector2(16, 36));
            ToolbarLayout.UpdateLayout();

            RunProjectButton.ResizeLayout();

            AppTitle.Bounds = GlobalInterfaceData.Scale(new Point(45, 32));
            AppTitle.Position = new Vector2(0, AppTitle.Bounds.Y / 2f);

            ProjectTitle.Bounds = GlobalInterfaceData.Scale(new Point(Width, 32));
            ProjectTitle.Position = GlobalInterfaceData.Scale(new Vector2(0, 16));
        }
    }
}
