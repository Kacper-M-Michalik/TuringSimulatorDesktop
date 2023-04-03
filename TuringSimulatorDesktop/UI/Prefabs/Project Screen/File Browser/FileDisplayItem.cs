using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TuringCore;
using TuringCore.Networking;
using TuringSimulatorDesktop.Input;
using Microsoft.Xna.Framework.Input;

namespace TuringSimulatorDesktop.UI.Prefabs
{
    public class FileDisplayItem : IVisualElement, IClickable, IPollable, IDragListener
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
        InputBox RenameBox;

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

            RenameBox = new InputBox(group);
            RenameBox.Modifiers.AllowsNewLine = false;
            RenameBox.IsActive = false;
            RenameBox.ClickAwayEvent += EndRenameEvent;
            RenameBox.BackgroundColor = GlobalInterfaceData.Scheme.DarkInteractableAccent;

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
                Browser.OpenMenu = new FileEditMenu(Browser, this);
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

        public void ClickedAway()
        {
            Browser.OpenMenu?.Close();
            ClickedOnce = false;
            Background.DrawColor = GlobalInterfaceData.Scheme.Background;
        }

        public void RenameFile()
        {
            RenameBox.IsActive = true;
            InputManager.ManuallyClickElement(RenameBox);
            RenameBox.Text = "";
            //RenameBox.Text = Data.Name;
        }

        void EndRenameEvent(InputBox Sender)
        {
            EndRename();
        }

        public void EndRename()
        {
            RenameBox.IsActive = false;
            Browser.Rename(Data, RenameBox.Text);
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

            if (RenameBox.IsActive && Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                EndRename();
            }
        }

        public void RecieveDragData()
        {
            FileData ReceivedData = InputManager.DragData as FileData;
            if (ReceivedData != null && Data.IsFolder)
            {
                if (ReceivedData.IsFolder)
                {
                    Client.SendTCPData(ClientSendPacketFunctions.MoveFolder(ReceivedData.ID, Data.ID));
                }
                else
                {
                    Client.SendTCPData(ClientSendPacketFunctions.MoveFile(ReceivedData.GUID, Data.ID));
                }
            }
        }

        void MoveLayout()
        {
            Background.Position = Position;
            FileIcon.Position = Position + GlobalInterfaceData.Scale(new Vector2(18, bounds.Y/2f - FileIcon.Bounds.Y/2f));
            FileLabel.Position = Position + GlobalInterfaceData.Scale(new Vector2(68, bounds.Y/2f));
            RenameBox.Position = Position + new Vector2(68, 3);
        }

        void ResizeLayout()
        {
            Background.Bounds = bounds;
            FileIcon.Bounds = GlobalInterfaceData.Scale(new Point(30, 30));
            RenameBox.Bounds = new Point(bounds.X - 74, ReferenceHeight - 6);
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                Background.Draw(BoundPort);
                FileIcon.Draw(BoundPort);
                FileLabel.Draw(BoundPort);

                if (RenameBox.IsActive) RenameBox.Draw();
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
