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
    public class FileEditMenu : IVisualElement, IClosable
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
        public ColorButton RenameFileButton;
        public ColorButton DeleteFileButton;

        Icon Divider1;
        Icon Divider2;

        ColorButton CreateFolderButton;

        ColorButton CreateTransitionFileButton;
        ColorButton CreateSlateFileButton;
        ColorButton CreateTapeFileButton;
        ColorButton CreateAlphabetFileButton;

        Label RenameFileLabel;
        Label DeleteFileLabel;

        Label CreateFolderLabel;
        Label CreateTransitionFileLabel;
        Label CreateSlateFileLabel;
        Label CreateTapeFileLabel;
        Label CreateAlphabetFileLabel;

        FileBrowserView Browser;
        FileDisplayItem Item;

        //Constructor
        //Thsi context Menu requires the owner File Browser and the FileDisplaYitem it is going to target with options such as rename or delete
        public FileEditMenu(FileBrowserView browser, FileDisplayItem item)
        {
            Browser = browser;
            Item = item;
            Group = InputManager.CreateActionGroup();

            Background = new Icon(GlobalInterfaceData.Scheme.InteractableAccent);

            CreateFolderButton = new ColorButton(Group);
            CreateFolderButton.BaseColor = GlobalInterfaceData.Scheme.ContextMenuBackground;
            CreateFolderButton.HighlightColor = GlobalInterfaceData.Scheme.ContextMenuSelected;
            CreateFolderButton.HighlightOnMouseOver = true;
            CreateFolderButton.OnClickedEvent += browser.CreateFolder;

            RenameFileButton = new ColorButton(Group);
            RenameFileButton.BaseColor = GlobalInterfaceData.Scheme.ContextMenuBackground;
            RenameFileButton.HighlightColor = GlobalInterfaceData.Scheme.ContextMenuSelected;
            RenameFileButton.HighlightOnMouseOver = true;
            RenameFileButton.OnClickedEvent += Rename;

            DeleteFileButton = new ColorButton(Group);
            DeleteFileButton.BaseColor = GlobalInterfaceData.Scheme.ContextMenuBackground;
            DeleteFileButton.HighlightColor = GlobalInterfaceData.Scheme.ContextMenuSelected;
            DeleteFileButton.HighlightOnMouseOver = true;
            DeleteFileButton.OnClickedEvent += Delete;

            Divider1 = new Icon(GlobalInterfaceData.Scheme.NonInteractableAccent);
            Divider2 = new Icon(GlobalInterfaceData.Scheme.NonInteractableAccent);

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

            RenameFileLabel = new Label();
            RenameFileLabel.FontSize = 12;
            RenameFileLabel.FontColor = GlobalInterfaceData.Scheme.FontColor;
            RenameFileLabel.Text = "Rename";
            DeleteFileLabel = new Label();
            DeleteFileLabel.FontSize = 12;
            DeleteFileLabel.FontColor = GlobalInterfaceData.Scheme.FontColor;
            DeleteFileLabel.Text = "Delete";
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

            bounds = GlobalInterfaceData.Scale(new Point(225, 181));
            ResizeLayout();
            Position = Vector2.Zero;
        }

        void MoveLayout()
        {
            Group.X = UIUtils.ConvertFloatToInt(position.X);
            Group.Y = UIUtils.ConvertFloatToInt(position.Y);

            Background.Position = Position;

            RenameFileButton.Position = Position + GlobalInterfaceData.Scale(new Vector2(0, 5));
            DeleteFileButton.Position = Position + GlobalInterfaceData.Scale(new Vector2(0, 29));

            Divider1.Position = Position + GlobalInterfaceData.Scale(new Vector2(0, 54));

            CreateFolderButton.Position = Position + GlobalInterfaceData.Scale(new Vector2(0, 55));

            Divider2.Position = Position + GlobalInterfaceData.Scale(new Vector2(0, 79));

            CreateTransitionFileButton.Position = Position + GlobalInterfaceData.Scale(new Vector2(0, 80));
            CreateSlateFileButton.Position = Position + GlobalInterfaceData.Scale(new Vector2(0, 104));
            CreateTapeFileButton.Position = Position + GlobalInterfaceData.Scale(new Vector2(0, 128));
            CreateAlphabetFileButton.Position = Position + GlobalInterfaceData.Scale(new Vector2(0, 152));

            RenameFileLabel.Position = Position + new Vector2(30, 16);
            DeleteFileLabel.Position = Position + new Vector2(30, 40);

            CreateFolderLabel.Position = Position + new Vector2(30, 66);
            CreateTransitionFileLabel.Position = Position + GlobalInterfaceData.Scale(new Vector2(30, 91));
            CreateSlateFileLabel.Position = Position + GlobalInterfaceData.Scale(new Vector2(30, 115));
            CreateTapeFileLabel.Position = Position + GlobalInterfaceData.Scale(new Vector2(30, 139));
            CreateAlphabetFileLabel.Position = Position + GlobalInterfaceData.Scale(new Vector2(30, 163));
        }

        void ResizeLayout()
        {
            Group.Width = bounds.X;
            Group.Height = bounds.Y;

            Background.Bounds = bounds;

            Divider1.Bounds = GlobalInterfaceData.Scale(new Point(225, 1));
            Divider2.Bounds = GlobalInterfaceData.Scale(new Point(225, 1));

            RenameFileButton.Bounds = GlobalInterfaceData.Scale(new Point(225, 24));
            DeleteFileButton.Bounds = GlobalInterfaceData.Scale(new Point(225, 24));
            CreateFolderButton.Bounds = GlobalInterfaceData.Scale(new Point(225, 24));
            CreateTransitionFileButton.Bounds = GlobalInterfaceData.Scale(new Point(225, 24));
            CreateSlateFileButton.Bounds = GlobalInterfaceData.Scale(new Point(225, 24));
            CreateTapeFileButton.Bounds = GlobalInterfaceData.Scale(new Point(225, 24));
            CreateAlphabetFileButton.Bounds = GlobalInterfaceData.Scale(new Point(225, 24));

            float FontSize = GlobalInterfaceData.Scale(12);
            RenameFileLabel.FontSize = FontSize;
            DeleteFileLabel.FontSize = FontSize;
            CreateFolderLabel.FontSize = FontSize;
            CreateTransitionFileLabel.FontSize = FontSize;
            CreateSlateFileLabel.FontSize = FontSize;
            CreateTapeFileLabel.FontSize = FontSize;
            CreateAlphabetFileLabel.FontSize = FontSize;
        }

        //Tells the selected FileDisplayItem to start the rename process for the file/folder it represents
        public void Rename(Button Sender)
        {
            Item.RenameFile();
            Close();
        }

        //Tells the selected FileDisplayItem to delete the file it represents
        public void Delete(Button Sender)
        {
            Browser.Delete(Item.Data);
            Close();
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                Background.Draw();

                RenameFileButton.Draw();
                DeleteFileButton.Draw();

                Divider1.Draw();

                CreateFolderButton.Draw();

                Divider2.Draw();

                CreateTransitionFileButton.Draw();
                CreateSlateFileButton.Draw();
                CreateTapeFileButton.Draw();
                CreateAlphabetFileButton.Draw();

                RenameFileLabel.Draw();
                DeleteFileLabel.Draw();

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
