using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TuringCore;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI.Prefabs
{
    public class FileBrowserView : IView
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
                bounds = value;
                ResizeLayout();
            }
        }

        bool isActive;
        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
                Group.IsActive = isActive;
                FileLayout.Group.IsActive = isActive;
            }
        }

        Icon Background;
        InputBox Searchbar;
        VerticalLayoutBox FileLayout;
        ActionGroup Group;

        public string Title => "File Browser";
        Window ownerWindow;
        public Window OwnerWindow
        {
            get => ownerWindow;
            set => ownerWindow = value;
        }

        string DefaultText = "Search Folder";
        int CurrentlyOpenedFolderID;
        List<FileDisplayItem> Files = new List<FileDisplayItem>();

        public FileBrowserView(int FolderToDisplay)
        {
            Group = InputManager.CreateActionGroup();

            Background = new Icon(GlobalRenderingData.BackgroundColor);
            Searchbar = new InputBox(0, 20, Group);
            Searchbar.Text = DefaultText;
            Searchbar.ClickEvent += ClearSearchbar;
            Searchbar.ClickAwayEvent += ResetSearchbar;
            Searchbar.EditEvent += FilterFiles;
            Searchbar.Modifiers.AllowsNewLine = false;

            FileLayout = new VerticalLayoutBox();
            FileLayout.Scrollable = true;
            FileLayout.Spacing = 10f;
            FileLayout.UniformAreas = true;
            FileLayout.UniformAreaSize = 60f;

            IsActive = false;

            SwitchOpenedFolder(FolderToDisplay);
        }
        
        public void FolderUpdated(Packet Data)
        {
            CustomLogging.Log("CLIENT: Window received Folder Data");

            FileLayout.Clear();
            Files.Clear();

            /*
            UIEventManager.Unsubscribe(CurrentlyOpenedFolderID, FilesUpdated);
            CurrentlyOpenedFolderID = Data.ReadInt();
            UIEventManager.Subscribe(CurrentlyOpenedFolderID, FilesUpdated);
            */
            if (Data.ReadInt() != CurrentlyOpenedFolderID)
            {
                CustomLogging.Log("Cllient: File Browser Window Fatal Error, recived unwated folder data!");
                return;
            }
            Data.ReadString();

            int FolderCount = Data.ReadInt();
            for (int i = 0; i < FolderCount; i++)
            {
                FileDisplayItem Item = new FileDisplayItem(new FileData(Data.ReadString(), Data.ReadInt(), FileType.Folder), this, FileLayout.Group);
                Files.Add(Item);                
            }
            int FileCount = Data.ReadInt();
            for (int i = 0; i < FileCount; i++)
            {
                FileDisplayItem Item = new FileDisplayItem(new FileData(Data.ReadString(), Data.ReadInt(), FileType.File), this, FileLayout.Group);
                Files.Add(Item);
            }

            FilterFiles(null);
        }

        public void SwitchOpenedFolder(int ID)
        {
            UIEventManager.Unsubscribe(CurrentlyOpenedFolderID, FolderUpdated);
            CurrentlyOpenedFolderID = ID;
            UIEventManager.Subscribe(CurrentlyOpenedFolderID, FolderUpdated);
            Client.SendTCPData(ClientSendPacketFunctions.RequestFolderData(ID));
        }

        void FilterFiles(InputBox Sender)
        {
            for (int i = 0; i < Files.Count; i++)
            {
                Files[i].IsActive = false;
            }
            FileLayout.Clear();
            if (Searchbar.Text == "" || Searchbar.Text == DefaultText)
            {
                for (int i = 0; i < Files.Count; i++)
                {
                    FileLayout.AddElement(Files[i]);
                    Files[i].IsActive = true;
                }
            }
            else
            {
                for (int i = 0; i < Files.Count; i++)
                {
                    if (Files[i].Data.Name.Contains(Searchbar.Text)) FileLayout.AddElement(Files[i]);
                }
            }
            FileLayout.UpdateLayout();
        }

        void ResetSearchbar(InputBox Sender)
        {
            if (Searchbar.Text == "") Searchbar.Text = DefaultText;
        }

        void ClearSearchbar(InputBox Sender)
        {
            Searchbar.Text = "";
        }

        void MoveLayout()
        {
            Group.X = UIUtils.ConvertFloatToInt(position.X);
            Group.Y = UIUtils.ConvertFloatToInt(position.Y);

            Background.Position = Position;
            Searchbar.Position = Position;
            FileLayout.Position = new Vector2(Position.X, Position.Y + 20);
        }

        void ResizeLayout()
        {
            Group.Width = bounds.X;
            Group.Height = bounds.Y;

            Background.Bounds = bounds;
            Searchbar.Bounds = new Point(bounds.X, 20);
            FileLayout.Bounds = new Point(bounds.X, bounds.Y - 20);
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                Background.Draw(BoundPort);
                Searchbar.Draw(BoundPort);
                FileLayout.Draw(BoundPort);
            }
        }
    }    
}
