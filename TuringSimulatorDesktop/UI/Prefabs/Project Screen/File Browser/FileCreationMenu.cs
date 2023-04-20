using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI.Prefabs
{
    public class FileCreationMenu : IVisualElement, IClosable
    {
        Vector2 position;
        public Vector2 Position
        {
            get => position;
            set
            {
                position = value;
                MoveLayout();
            }
        }

        Point bounds;
        public Point Bounds
        {
            get => bounds;
            set
            {
            }
        }

        bool isActive = true;
        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
                Group.IsActive = isActive;
            }
        }

        ActionGroup Group;

        Icon Background;
        ColorButton CreateFolderButton;
        Icon Divider1;
        ColorButton CreateTransitionFileButton;
        ColorButton CreateSlateFileButton;
        ColorButton CreateTapeFileButton;
        ColorButton CreateAlphabetFileButton;
        Label CreateFolderLabel;
        Label CreateTransitionFileLabel;
        Label CreateSlateFileLabel;
        Label CreateTapeFileLabel;
        Label CreateAlphabetFileLabel;

        //Constructor
        //Requires owner file browser view be passed for this context menu
        public FileCreationMenu(FileBrowserView browser)
        {
            Group = InputManager.CreateActionGroup();

            Background = new Icon(GlobalInterfaceData.Scheme.InteractableAccent);

            CreateFolderButton = new ColorButton(Group);
            CreateFolderButton.BaseColor = GlobalInterfaceData.Scheme.ContextMenuBackground;
            CreateFolderButton.HighlightColor = GlobalInterfaceData.Scheme.ContextMenuSelected;
            CreateFolderButton.HighlightOnMouseOver = true;
            CreateFolderButton.OnClickedEvent += browser.CreateFolder;

            Divider1 = new Icon(GlobalInterfaceData.Scheme.NonInteractableAccent);

            CreateTransitionFileButton = new ColorButton(Group);
            CreateTransitionFileButton.BaseColor = GlobalInterfaceData.Scheme.ContextMenuBackground;
            CreateTransitionFileButton.HighlightColor = GlobalInterfaceData.Scheme.ContextMenuSelected;
            CreateTransitionFileButton.HighlightOnMouseOver = true;
            CreateTransitionFileButton.OnClickedEvent += browser.CreateTransitionFile;

            CreateSlateFileButton = new ColorButton(Group);
            CreateSlateFileButton.BaseColor = GlobalInterfaceData.Scheme.ContextMenuBackground;
            CreateSlateFileButton.HighlightColor = GlobalInterfaceData.Scheme.ContextMenuSelected;
            CreateSlateFileButton.HighlightOnMouseOver = true;
            CreateSlateFileButton.OnClickedEvent += browser.CreateSlateFile;

            CreateTapeFileButton = new ColorButton(Group);
            CreateTapeFileButton.BaseColor = GlobalInterfaceData.Scheme.ContextMenuBackground;
            CreateTapeFileButton.HighlightColor = GlobalInterfaceData.Scheme.ContextMenuSelected;
            CreateTapeFileButton.HighlightOnMouseOver = true;
            CreateTapeFileButton.OnClickedEvent += browser.CreateTapeFile;

            CreateAlphabetFileButton = new ColorButton(Group);
            CreateAlphabetFileButton.BaseColor = GlobalInterfaceData.Scheme.ContextMenuBackground;
            CreateAlphabetFileButton.HighlightColor = GlobalInterfaceData.Scheme.ContextMenuSelected;
            CreateAlphabetFileButton.HighlightOnMouseOver = true;
            CreateAlphabetFileButton.OnClickedEvent += browser.CreateAlphabetFile;

            CreateFolderLabel = new Label();
            CreateFolderLabel.FontSize = 12;
            CreateFolderLabel.FontColor = GlobalInterfaceData.Scheme.FontColor;
            CreateFolderLabel.Text = "Create Folder";
            CreateTransitionFileLabel = new Label();
            CreateTransitionFileLabel.FontSize = 12;
            CreateTransitionFileLabel.FontColor = GlobalInterfaceData.Scheme.FontColor;
            CreateTransitionFileLabel.Text = "Create Transition File";
            CreateSlateFileLabel = new Label();
            CreateSlateFileLabel.FontSize = 12;
            CreateSlateFileLabel.FontColor = GlobalInterfaceData.Scheme.FontColor;
            CreateSlateFileLabel.Text = "Create Slate File";
            CreateTapeFileLabel = new Label();
            CreateTapeFileLabel.FontSize = 12;
            CreateTapeFileLabel.FontColor = GlobalInterfaceData.Scheme.FontColor;
            CreateTapeFileLabel.Text = "Create Tape Preset";
            CreateAlphabetFileLabel = new Label();
            CreateAlphabetFileLabel.FontSize = 12;
            CreateAlphabetFileLabel.FontColor = GlobalInterfaceData.Scheme.FontColor;
            CreateAlphabetFileLabel.Text = "Create Alphabet";

            bounds = GlobalInterfaceData.Scale(new Point(225, 132));
            ResizeLayout();
            Position = Vector2.Zero;
        }

        void MoveLayout()
        {
            Group.X = UIUtils.ConvertFloatToInt(position.X);
            Group.Y = UIUtils.ConvertFloatToInt(position.Y);

            Background.Position = Position;

            CreateFolderButton.Position = Position + GlobalInterfaceData.Scale(new Vector2(0, 5));

            Divider1.Position = Position + GlobalInterfaceData.Scale(new Vector2(0, 29));

            CreateTransitionFileButton.Position = Position + GlobalInterfaceData.Scale(new Vector2(0, 30));
            CreateSlateFileButton.Position = Position + GlobalInterfaceData.Scale(new Vector2(0, 54));
            CreateTapeFileButton.Position = Position + GlobalInterfaceData.Scale(new Vector2(0, 78));
            CreateAlphabetFileButton.Position = Position + GlobalInterfaceData.Scale(new Vector2(0, 102));

            CreateFolderLabel.Position = Position + new Vector2(30, 16);
            CreateTransitionFileLabel.Position = Position + GlobalInterfaceData.Scale(new Vector2(30, 41));
            CreateSlateFileLabel.Position = Position + GlobalInterfaceData.Scale(new Vector2(30, 65));
            CreateTapeFileLabel.Position = Position + GlobalInterfaceData.Scale(new Vector2(30, 89));
            CreateAlphabetFileLabel.Position = Position + GlobalInterfaceData.Scale(new Vector2(30, 113));
        }

        void ResizeLayout()
        {
            Group.Width = bounds.X;
            Group.Height = bounds.Y;

            Background.Bounds = bounds;

            Divider1.Bounds = GlobalInterfaceData.Scale(new Point(225, 1));

            CreateFolderButton.Bounds = GlobalInterfaceData.Scale(new Point(225, 24));
            CreateTransitionFileButton.Bounds = GlobalInterfaceData.Scale(new Point(225, 24));
            CreateSlateFileButton.Bounds = GlobalInterfaceData.Scale(new Point(225, 24));
            CreateTapeFileButton.Bounds = GlobalInterfaceData.Scale(new Point(225, 24));
            CreateAlphabetFileButton.Bounds = GlobalInterfaceData.Scale(new Point(225, 24));

            float FontSize = GlobalInterfaceData.Scale(12);
            CreateFolderLabel.FontSize = FontSize;
            CreateTransitionFileLabel.FontSize = FontSize;
            CreateSlateFileLabel.FontSize = FontSize;
            CreateTapeFileLabel.FontSize = FontSize;
            CreateAlphabetFileLabel.FontSize = FontSize;
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                Background.Draw();

                CreateFolderButton.Draw();

                Divider1.Draw();

                CreateTransitionFileButton.Draw();
                CreateSlateFileButton.Draw();
                CreateTapeFileButton.Draw();
                CreateAlphabetFileButton.Draw();

                CreateFolderLabel.Draw();
                CreateTransitionFileLabel.Draw();
                CreateSlateFileLabel.Draw();
                CreateTapeFileLabel.Draw();
                CreateAlphabetFileLabel.Draw();
            }
        }

        public void Close()
        {
            IsActive = false;
            Group.IsMarkedForDeletion = true;
        }
    }
}
