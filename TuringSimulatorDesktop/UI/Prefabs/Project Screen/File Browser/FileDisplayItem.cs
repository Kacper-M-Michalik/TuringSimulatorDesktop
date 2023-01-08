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

        public bool IsActive = true;

        public FileData Data;
        FileBrowserView Browser;

        ButtonIcon Background;
        Label NameLabel;

        public FileDisplayItem(FileData data, FileBrowserView browser, ActionGroup group)
        {            
            Data = data;
            Browser = browser;

            Background = new ButtonIcon(group);
            Background.BaseTexture = GlobalRenderingData.TextureLookup[UILookupKey.FolderIcon];
            Background.OnClickedEvent += Clicked;
            NameLabel = new Label();

            Bounds = new Point(60, 60);

            NameLabel.FontSize = 14;
            NameLabel.Text = Data.Name + " " + Data.ID.ToString();
        }

        public void Clicked(Button Sender)
        {
            Browser.SwitchOpenedFolder(Data.ID);
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
