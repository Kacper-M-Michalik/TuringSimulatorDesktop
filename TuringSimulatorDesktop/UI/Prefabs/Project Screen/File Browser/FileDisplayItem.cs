using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI.Prefabs
{
    public class FileDisplayItem : IVisualElement
    {
        Vector2 position;
        public Vector2 Position
        {
            get => position;
            set
            {
                position = value;
                Background.Position = position;
                NameLabel.Position = position;
            }
        }

        Point bounds;
        public Point Bounds
        {
            get => bounds;
            set
            {
                bounds = value;
                Background.Bounds = bounds;
                NameLabel.Bounds = bounds;
            }
        }

        bool isActive;
        public bool IsActive 
        { 
            get => isActive;
            set
            {
                isActive = value;
                Background.IsActive = isActive;
            }
        }

        Button Background;
        Label NameLabel;

        public FileData Data;
        FileBrowserView Browser;

        public FileDisplayItem(FileData data, FileBrowserView browser, ActionGroup group)
        {            
            Data = data;
            Browser = browser;

            Background = new Button(group);
            if (Data.Type == FileType.Folder)
            {
                Background.BaseTexture = GlobalRenderingData.TextureLookup[UILookupKey.FolderIcon];
            }
            else
            {
                Background.BaseTexture = GlobalRenderingData.TextureLookup[UILookupKey.TransitionTableIcon];
            }
            Background.OnClickedEvent += Clicked;
            NameLabel = new Label();

            Bounds = new Point(60, 60);

            NameLabel.FontSize = 14;
            NameLabel.Text = Data.Name + " " + Data.ID.ToString();
        }

        public void Clicked(Button Sender)
        {
            if (Data.Type == FileType.Folder)
            {
                Browser.SwitchOpenedFolder(Data.ID);
            }
            else
            {
                Browser.OwnerWindow.AddView(new TextProgrammingView(Data.ID));
            }
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                Background.Draw(BoundPort);
                NameLabel.Draw(BoundPort);
            }
        }
    }

    public enum FileType { File, Folder };

    public class FileData
    {
        public string Name;
        public int ID;
        public FileType Type;

        public FileData(string SetName, int SetID, FileType SetType)
        {
            Name = SetName;
            ID = SetID;
            Type = SetType;
        }
    }
}
