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
    public class FileCreationMenu : IVisualElement
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
        Icon Divider1;
        Icon Divider2;
        ColorButton CreateFolderButton;
        ColorButton CreateTransitionFileButton;
        ColorButton CreateSlateFileButton;
        ColorButton CreateTapeFileButton;
        ColorButton CreateAlphabetFileButton;
        Label CreateFolderLabel;
        Label CreateTransitionFileLabel;
        Label CreateSlateFileLabel;
        Label CreateTapeFileLabel;
        Label CreateAlphabetFileLabel;

        public FileCreationMenu(FileBrowserView browser)
        {
            Group = InputManager.CreateActionGroup();

            Background = new Icon(GlobalInterfaceData.Scheme.InteractableAccent);

            Divider1 = new Icon(GlobalInterfaceData.Scheme.NonInteractableAccent);
            Divider2 = new Icon(GlobalInterfaceData.Scheme.NonInteractableAccent);

            CreateFolderButton = new ColorButton(Group);
            CreateFolderButton.BaseColor = GlobalInterfaceData.Scheme.InteractableAccent;
            CreateFolderButton.HighlightColor = GlobalInterfaceData.Scheme.DarkInteractableAccent;
            CreateFolderButton.HighlightOnMouseOver = true;
            CreateFolderButton.OnClickedEvent += browser.CreateFolder;

            CreateTransitionFileButton = new ColorButton(Group);
            CreateTransitionFileButton.BaseColor = GlobalInterfaceData.Scheme.InteractableAccent;
            CreateTransitionFileButton.HighlightColor = GlobalInterfaceData.Scheme.DarkInteractableAccent;
            CreateTransitionFileButton.HighlightOnMouseOver = true;
            CreateTransitionFileButton.OnClickedEvent += browser.CreateTransitionFile;

            CreateSlateFileButton = new ColorButton(Group);
            CreateSlateFileButton.BaseColor = GlobalInterfaceData.Scheme.InteractableAccent;
            CreateSlateFileButton.HighlightColor = GlobalInterfaceData.Scheme.DarkInteractableAccent;
            CreateSlateFileButton.HighlightOnMouseOver = true;
            CreateSlateFileButton.OnClickedEvent += browser.CreateSlateFile;

            CreateTapeFileButton = new ColorButton(Group);
            CreateTapeFileButton.BaseColor = GlobalInterfaceData.Scheme.InteractableAccent;
            CreateTapeFileButton.HighlightColor = GlobalInterfaceData.Scheme.DarkInteractableAccent;
            CreateTapeFileButton.HighlightOnMouseOver = true;
            CreateTapeFileButton.OnClickedEvent += browser.CreateTapeFile;

            CreateAlphabetFileButton = new ColorButton(Group);
            CreateAlphabetFileButton.BaseColor = GlobalInterfaceData.Scheme.InteractableAccent;
            CreateAlphabetFileButton.HighlightColor = GlobalInterfaceData.Scheme.DarkInteractableAccent;
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

            bounds = GlobalInterfaceData.Scale(new Point(225, 108));
            ResizeLayout();
            Position = Vector2.Zero;
        }

        void MoveLayout()
        {
            Group.X = UIUtils.ConvertFloatToInt(position.X);
            Group.Y = UIUtils.ConvertFloatToInt(position.Y);

            Background.Position = Position;

            Divider1.Position = Position + GlobalInterfaceData.Scale(new Vector2(0, 53));
            Divider2.Position = Position + GlobalInterfaceData.Scale(new Vector2(0, 78));

            CreateFolderButton.Position = Position + GlobalInterfaceData.Scale(new Vector2(0, 5));
            CreateTransitionFileButton.Position = Position + GlobalInterfaceData.Scale(new Vector2(0, 5));
            CreateSlateFileButton.Position = Position + GlobalInterfaceData.Scale(new Vector2(0, 29));
            CreateTapeFileButton.Position = Position + GlobalInterfaceData.Scale(new Vector2(0, 54));
            CreateAlphabetFileButton.Position = Position + GlobalInterfaceData.Scale(new Vector2(0, 79));

            CreateTransitionFileLabel.Position = Position + GlobalInterfaceData.Scale(new Vector2(30, 17));
            CreateSlateFileLabel.Position = Position + GlobalInterfaceData.Scale(new Vector2(30, 41));
            CreateTapeFileLabel.Position = Position + GlobalInterfaceData.Scale(new Vector2(30, 66));
            CreateAlphabetFileLabel.Position = Position + GlobalInterfaceData.Scale(new Vector2(30, 91));
        }

        void ResizeLayout()
        {
            Group.Width = bounds.X;
            Group.Height = bounds.Y;

            Background.Bounds = bounds;

            Divider1.Bounds = GlobalInterfaceData.Scale(new Point(225, 1));
            Divider2.Bounds = GlobalInterfaceData.Scale(new Point(225, 1));

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

                Divider1.Draw();
                Divider2.Draw();

                CreateFolderButton.Draw();
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
