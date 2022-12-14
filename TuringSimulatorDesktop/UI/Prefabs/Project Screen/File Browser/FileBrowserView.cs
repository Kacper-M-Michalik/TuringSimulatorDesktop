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
    public class FileBrowserView : IView, IClickable
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

        Window ownerWindow;
        public Window OwnerWindow
        {
            get => ownerWindow;
            set => ownerWindow = value;
        }

        public string Title => "File Browser";

        public bool IsMarkedForDeletion
        {
            get => false;
            set
            {

            }
        }

        ActionGroup Group;        

        Icon Background;
        Icon Divider1;
        Icon Divider2;
        InputBox Searchbar;
        Label HierarchyLabel;
        VerticalLayoutBox FileLayout;

        int CurrentlyOpenedFolderID;
        List<FileDisplayItem> Files = new List<FileDisplayItem>();
        FileCreationMenu OpenMenu;

        string DefaultText = "Search:";

        public FileBrowserView()
        {
            Group = InputManager.CreateActionGroup();
            Group.ClickableObjects.Add(this);

            Background = new Icon(GlobalInterfaceData.Scheme.Background);
            Searchbar = new InputBox(Group);
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

            SwitchOpenedFolder(0);
        }
        
        public void FolderUpdated(Packet Data)
        {
            CustomLogging.Log("CLIENT: Window received Folder Data");

            FileLayout.Clear();
            Files.Clear();

            if (Data.ReadInt() != CurrentlyOpenedFolderID)
            {
                CustomLogging.Log("Client: File Browser Window Fatal Error, recived unwated folder data!");
                return;
            }
            
            //To create hierarchy view
            string FolderName = Data.ReadString();

            int Max = Data.ReadInt();
            for (int i = 0; i < Max; i++)
            {
                Data.ReadString();
                Data.ReadInt();
            }

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
            Client.SendTCPData(ClientSendPacketFunctions.UnsubscribeFromFolderUpdates(CurrentlyOpenedFolderID));
            CurrentlyOpenedFolderID = ID;
            UIEventManager.Subscribe(CurrentlyOpenedFolderID, FolderUpdated);
            Client.SendTCPData(ClientSendPacketFunctions.RequestFolderData(ID,true));
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
                    if (Files[i].Data.Name.Contains(Searchbar.Text))
                    {
                        FileLayout.AddElement(Files[i]);
                        Files[i].IsActive = true;
                    }
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

        public void CreateTransitionFile(Button Sender)
        {
            OpenMenu?.Close();
            Client.SendTCPData(ClientSendPacketFunctions.CreateFile(CurrentlyOpenedFolderID, "Empty Transition File", CreateFileType.TransitionFile));
        }
        public void CreateSlateFile(Button Sender)
        {
            OpenMenu?.Close();
            Client.SendTCPData(ClientSendPacketFunctions.CreateFile(CurrentlyOpenedFolderID, "Empty Slate File", CreateFileType.SlateFile));
        }
        public void CreateTapeFile(Button Sender)
        {
            OpenMenu?.Close();
            Client.SendTCPData(ClientSendPacketFunctions.CreateFile(CurrentlyOpenedFolderID, "Empty Tape Preset", CreateFileType.Tape));
        }
        public void CreateAlphabetFile(Button Sender)
        {
            OpenMenu?.Close();
            Client.SendTCPData(ClientSendPacketFunctions.CreateFile(CurrentlyOpenedFolderID, "Empty Alphabet", CreateFileType.Alphabet));
        }

        public void Clicked()
        {
            if (InputManager.RightMousePressed)
            {
                OpenMenu?.Close();
                OpenMenu = new FileCreationMenu(this);
                OpenMenu.Position = new Vector2(InputManager.MouseData.X, InputManager.MouseData.Y);
            }
        }

        public void ClickedAway()
        {
            //remove the create window
        }

        public bool IsMouseOver()
        {
            return (IsActive && InputManager.MouseData.X >= Position.X && InputManager.MouseData.X <= Position.X + bounds.X && InputManager.MouseData.Y >= Position.Y && InputManager.MouseData.Y <= Position.Y + bounds.Y);
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

                OpenMenu?.Draw();
            }
        }

        public void Close()
        {
            OpenMenu?.Close();
            FileLayout.Close();
            Group.IsMarkedForDeletion = true;
        }

    }    
}
