using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TuringCore;
using TuringCore.Networking;
using TuringSimulatorDesktop.Input;
using TuringSimulatorDesktop.Networking;

namespace TuringSimulatorDesktop.UI.Prefabs
{
    public class FolderHierarchyItem : IVisualElement, IClickable, IPollable, IDragListener
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
                SelectFolderButton.IsActive = value;
            }
        }

        public bool IsMarkedForDeletion { get; set; }

        ActionGroup Group;

        Icon SelectFolderButton;
        Label FolderLabel;

        FileBrowserView Browser;
        FileData Data;
        public const int ReferencePadding = 16;

        //Constructor
        //Requires data on the folder this object is representing, the owner File Browser and the action group of the File Browser
        public FolderHierarchyItem(FileData data,  FileBrowserView browser, ActionGroup group)
        {
            Group = group;
            Browser = browser;
            Data = data;

            Group.ClickableObjects.Add(this);
            Group.PollableObjects.Add(this);

            SelectFolderButton = new Icon(GlobalInterfaceData.Scheme.Background);

            FolderLabel = new Label();
            FolderLabel.Font = GlobalInterfaceData.StandardBoldFont;
            FolderLabel.FontColor = GlobalInterfaceData.Scheme.FontGrayedOutColor;
            FolderLabel.FontSize = GlobalInterfaceData.Scale(12);
            FolderLabel.Text = Data.Name;

            bounds = GlobalInterfaceData.Scale(new Point(FolderLabel.Bounds.X + ReferencePadding, browser.HierarchyLayout.Bounds.Y));
            ResizeLayout();
            Position = new Vector2(0, 0);
        }

        //Equivalent to that of FileDisplayItems that represent folders having a file/folder dragged on them
        //Sends request to have a file/folder moved to be the subfile/subfolder of this folder if the file/folder is dragged on this object
        public void RecieveDragData()
        {
            FileData ReceivedData = InputManager.DragData as FileData;
            if (ReceivedData != null)
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

        public void Clicked()
        {
            Browser.SwitchOpenedFolder(Data.ID);
        }

        public void ClickedAway()
        {

        }

        //Polls whether to draw a highlighted version of itself if user hovers over it with mause
        public void PollInput(bool IsInActionGroupFrame)
        {
            if (IsInActionGroupFrame && IsMouseOver())
            {
                SelectFolderButton.DrawColor = GlobalInterfaceData.Scheme.DarkInteractableAccent;
            }
            else
            {
                SelectFolderButton.DrawColor = GlobalInterfaceData.Scheme.Background;
            }
        }

        public bool IsMouseOver()
        {
            return (IsActive && InputManager.MouseData.X >= Position.X && InputManager.MouseData.X <= Position.X + bounds.X && InputManager.MouseData.Y >= Position.Y && InputManager.MouseData.Y <= Position.Y + bounds.Y);
        }

        void MoveLayout()
        {
            SelectFolderButton.Position = Position;
            FolderLabel.Position = Position + new Vector2(GlobalInterfaceData.Scale(ReferencePadding / 2f), bounds.Y / 2f);
        }

        void ResizeLayout()
        {
            SelectFolderButton.Bounds = bounds;

            float FontSize = GlobalInterfaceData.Scale(12);
            FolderLabel.FontSize = FontSize;
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                SelectFolderButton.Draw(BoundPort);
                FolderLabel.Draw(BoundPort);
            }
        }

        public void Close()
        {
            IsActive = false;
            Group.IsDirtyClickable = true;
            Group.IsDirtyPollable = true;
            IsMarkedForDeletion = true;
        }
    }
}
