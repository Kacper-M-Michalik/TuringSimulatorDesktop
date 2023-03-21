using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TuringCore;
using TuringSimulatorDesktop.Input;
using Microsoft.Xna.Framework.Input;

namespace TuringSimulatorDesktop.UI.Prefabs
{
    public class FileDisplayItem : IVisualElement, IClickable, IPollable
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
                bounds = new Point(value.X, UIUtils.ConvertFloatToMinInt(GlobalInterfaceData.Scale(ReferenceHeight), 1f));
                ResizeLayout();
            }
        }

        public bool IsActive { get; set; } = true;

        public bool IsMarkedForDeletion { get; set; }

        Icon Background;
        Icon FileIcon;
        Label FileLabel;

        public const int ReferenceHeight = 32;

        public FileData Data;
        FileBrowserView Browser;
        bool ClickedOnce;

        public FileDisplayItem(FileData data, FileBrowserView browser, ActionGroup group)
        {            
            Data = data;
            Browser = browser;
            group.ClickableObjects.Add(this);
            group.PollableObjects.Add(this);

            Background = new Icon(GlobalInterfaceData.Scheme.Background);

            FileIcon = new Icon();

            if (data.IsFolder)
            {
                FileIcon.DrawTexture = GlobalInterfaceData.TextureLookup[UILookupKey.FolderIcon];
            }
            else
            {
                switch (Data.Type)
                {
                    case CoreFileType.Alphabet:
                        FileIcon.DrawTexture = GlobalInterfaceData.TextureLookup[UILookupKey.AlphabetIcon];
                        break;
                    case CoreFileType.Tape:
                        FileIcon.DrawTexture = GlobalInterfaceData.TextureLookup[UILookupKey.TapeIcon];
                        break;
                    case CoreFileType.TransitionFile:
                        FileIcon.DrawTexture = GlobalInterfaceData.TextureLookup[UILookupKey.TransitionTableIcon];
                        break;
                    case CoreFileType.SlateFile:
                        FileIcon.DrawTexture = GlobalInterfaceData.TextureLookup[UILookupKey.SlateFileTCIcon];
                        break;
                    case CoreFileType.Other:
                        FileIcon.DrawTexture = GlobalInterfaceData.TextureLookup[UILookupKey.DebugTexture];
                        break;
                }
            }

            FileLabel = new Label();
            FileLabel.FontColor = GlobalInterfaceData.Scheme.FontColor;
            FileLabel.Font = GlobalInterfaceData.StandardRegularFont;
            FileLabel.FontSize = GlobalInterfaceData.Scale(12);
            FileLabel.Text = data.Name;
        }

        public void Clicked()
        {
            Background.DrawColor = GlobalInterfaceData.Scheme.InteractableAccent;

            if (InputManager.RightMousePressed)
            {
                Browser.OpenMenu?.Close();
                FileEditMenu Menu = new FileEditMenu(Browser, this);
                Menu.DeleteFileButton.OnClickedEvent += Delete;
                Browser.OpenMenu = Menu;
                Browser.OpenMenu.Position = new Vector2(InputManager.MouseData.X, InputManager.MouseData.Y);
                return;
            }

            if (ClickedOnce && InputManager.LeftMousePressed)
            {               
                ClickedOnce = false;

                if (Data.IsFolder)
                {
                    Browser.SwitchOpenedFolder(Data.ID);
                }
                else
                {
                    Browser.OpenFile(Data);
                }

                return;
            }
                          
            ClickedOnce = true;              
        }

        public void Delete(Button Sender)
        {
            if (Data.IsFolder)
            {
                Client.SendTCPData(ClientSendPacketFunctions.DeleteFolder(Data.ID));
            }
            else
            {
                Client.SendTCPData(ClientSendPacketFunctions.DeleteFile(Data.GUID));
            }
        }

        public void ClickedAway()
        {
            Browser.OpenMenu?.Close();
            ClickedOnce = false;
            Background.DrawColor = GlobalInterfaceData.Scheme.Background;
        }

        public bool IsMouseOver()
        {
            return (IsActive && InputManager.MouseData.X >= Position.X && InputManager.MouseData.X <= Position.X + bounds.X && InputManager.MouseData.Y >= Position.Y && InputManager.MouseData.Y <= Position.Y + bounds.Y);
        }

        public void PollInput(bool IsInActionGroupFrame)
        {
            if (ClickedOnce && InputManager.MouseData.LeftButton == ButtonState.Pressed && !IsMouseOver())
            {
                InputManager.StartDragging(Data);
                ClickedOnce = false;
            }
        }

        void MoveLayout()
        {
            Background.Position = Position;
            FileIcon.Position = Position + GlobalInterfaceData.Scale(new Vector2(18, bounds.Y/2f - FileIcon.Bounds.Y/2f));
            FileLabel.Position = Position + GlobalInterfaceData.Scale(new Vector2(68, bounds.Y/2f));
        }

        void ResizeLayout()
        {
            Background.Bounds = bounds;
            FileIcon.Bounds = GlobalInterfaceData.Scale(new Point(30, 30));
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                Background.Draw(BoundPort);
                FileIcon.Draw(BoundPort);
                FileLabel.Draw(BoundPort);
            }
        }
    }

    public class FileData
    {
        public string Name;
        public int ID;
        public Guid GUID;
        public CoreFileType Type;
        public bool IsFolder;

        public FileData(string SetName, Guid SetGUID, CoreFileType SetType)
        {
            Name = SetName;
            GUID = SetGUID;
            Type = SetType;
            IsFolder = false;
        }

        public FileData(string SetName, int SetID)
        {
            Name = SetName;
            ID = SetID;
            Type = CoreFileType.Other;
            IsFolder = true;
        }
    }
}
