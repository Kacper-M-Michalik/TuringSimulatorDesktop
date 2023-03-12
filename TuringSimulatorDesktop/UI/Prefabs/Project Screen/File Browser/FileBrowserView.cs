using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
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
                HierarchyLayout.Group.IsActive = isActive;
                FileLayout.Group.IsActive = isActive;
                OpenMenu?.Close();
            }
        }

        Window ownerWindow;
        public Window OwnerWindow
        {
            get => ownerWindow;
            set => ownerWindow = value;
        }

        public string Title => "File Browser";
        public Guid OpenFileID => Guid.Empty;

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
        public HorizontalLayoutBox HierarchyLayout;
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
            Searchbar.Labeloffset = GlobalInterfaceData.Scale(new Vector2(FolderHierarchyItem.ReferencePadding/2f, 0));
            Searchbar.BackgroundColor = GlobalInterfaceData.Scheme.Background;
            Searchbar.OutputLabel.FontSize = GlobalInterfaceData.Scale(12);
            Searchbar.OutputLabel.FontColor = GlobalInterfaceData.Scheme.FontGrayedOutColor;
            Searchbar.Text = DefaultText;
            Searchbar.ClickEvent += ClearSearchbar;
            Searchbar.ClickAwayEvent += ResetSearchbar;
            Searchbar.EditEvent += FilterFiles;
            Searchbar.Modifiers.AllowsNewLine = false;

            Divider1 = new Icon(GlobalInterfaceData.Scheme.NonInteractableAccent);

            HierarchyLayout = new HorizontalLayoutBox();
            HierarchyLayout.Scrollable = false;
            HierarchyLayout.Centering = HorizontalCentering.Middle;
            HierarchyLayout.Spacing = GlobalInterfaceData.Scale(-1);
            //HierarchyLayout.Spacing = 5;
            //HierarchyLayout.ViewOffset = new Vector2(5, 0);

            Divider2 = new Icon(GlobalInterfaceData.Scheme.NonInteractableAccent);

            FileLayout = new VerticalLayoutBox();
            FileLayout.Scrollable = true;
            FileLayout.Spacing = GlobalInterfaceData.Scale(11f);
            FileLayout.ViewOffset = GlobalInterfaceData.Scale(new Vector2(0, 13f));
            FileLayout.ViewOffsetBoundsMin = GlobalInterfaceData.Scale(new Vector2(0, 13f));

            IsActive = false;

            SwitchOpenedFolder(0);
        }
        
        public void SwitchOpenedFolder(int ID)
        {
            UIEventManager.Unsubscribe(CurrentlyOpenedFolderID, FolderUpdated);
            Client.SendTCPData(ClientSendPacketFunctions.UnsubscribeFromFolderUpdates(CurrentlyOpenedFolderID));
            CurrentlyOpenedFolderID = ID;
            UIEventManager.Subscribe(CurrentlyOpenedFolderID, FolderUpdated);
            Client.SendTCPData(ClientSendPacketFunctions.RequestFolderData(ID,true));
        }

        public void FolderUpdated(object Data)
        {
            CustomLogging.Log("CLIENT: Window received Folder Data");

            FolderDataMessage Message = (FolderDataMessage)Data;

            if (Message.ID != CurrentlyOpenedFolderID)
            {
                CustomLogging.Log("Client: File Browser Window Fatal Error, recived unwated folder data!");
                return;
            }

            FileLayout.Clear();
            Files.Clear();

            HierarchyLayout.Clear();

            string FolderName = Message.Name;
            List<IVisualElement> Items = new List<IVisualElement>();

            int Max = Message.ParentFolders.Count;
            for (int i = 0; i < Max; i++)
            {
                Label TransitionLabel = new Label();
                TransitionLabel.FontSize = GlobalInterfaceData.Scale(12);
                TransitionLabel.Font = GlobalInterfaceData.StandardBoldFont;
                TransitionLabel.FontColor = GlobalInterfaceData.Scheme.FontGrayedOutColor;
                TransitionLabel.Text = " > ";
                TransitionLabel.DrawCentered = true;
                TransitionLabel.Bounds = TransitionLabel.Bounds + GlobalInterfaceData.Scale(new Point(FolderHierarchyItem.ReferencePadding, 0));
                Items.Add(TransitionLabel);
                Items.Add(new FolderHierarchyItem(new FileData(Message.ParentFolders[i].Name, Message.ParentFolders[i].ID), this, HierarchyLayout.Group));
            }

            for (int i = Items.Count - 1; i > -1; i--)
            {
                HierarchyLayout.AddElement(Items[i]);
            }

            Label CurrentFolderLabel = new Label();
            CurrentFolderLabel.FontSize = GlobalInterfaceData.Scale(12);
            CurrentFolderLabel.FontColor = GlobalInterfaceData.Scheme.FontGrayedOutColor;
            CurrentFolderLabel.Font = GlobalInterfaceData.StandardBoldFont;
            CurrentFolderLabel.Text = FolderName;
            CurrentFolderLabel.DrawCentered = true;
            CurrentFolderLabel.Bounds = CurrentFolderLabel.Bounds + GlobalInterfaceData.Scale(new Point(FolderHierarchyItem.ReferencePadding, 0));
            HierarchyLayout.AddElement(CurrentFolderLabel);


            HierarchyLayout.UpdateLayout();

            int FolderCount = Message.SubFolders.Count;
            for (int i = 0; i < FolderCount; i++)
            {
                FileDisplayItem Item = new FileDisplayItem(new FileData(Message.SubFolders[i].Name, Message.SubFolders[i].ID), this, FileLayout.Group);
                Item.Bounds = new Point(FileLayout.Bounds.X, 0);
                Files.Add(Item);
                FileLayout.AddElement(Item);
            }
            int FileCount = Message.Files.Count;
            for (int i = 0; i < FileCount; i++)
            {
                FileDisplayItem Item = new FileDisplayItem(new FileData(Message.Files[i].Name, Message.Files[i].GUID, Message.Files[i].FileType), this, FileLayout.Group);
                Item.Bounds = new Point(FileLayout.Bounds.X, 0);
                Files.Add(Item);
                FileLayout.AddElement(Item);
            }

            FilterFiles(null);
            
        }

        void FilterFiles(InputBox Sender)
        {
            for (int i = 0; i < Files.Count; i++)
            {
                Files[i].IsActive = false;
            }

            if (Searchbar.Text == "" || Searchbar.Text == DefaultText)
            {
                for (int i = 0; i < Files.Count; i++)
                {
                    Files[i].IsActive = true;
                }
            }
            else
            {
                for (int i = 0; i < Files.Count; i++)
                {
                    if (Files[i].Data.Name.Contains(Searchbar.Text, StringComparison.OrdinalIgnoreCase))
                    {
                        Files[i].IsActive = true;
                    }
                }
            }
            FileLayout.UpdateLayout();
        }

        void ResetSearchbar(InputBox Sender)
        {
            if (Searchbar.Text == "") Searchbar.Text = DefaultText;
            FilterFiles(null);
        }

        void ClearSearchbar(InputBox Sender)
        {
            Searchbar.Text = "";
            FilterFiles(null);
        }

        public void CreateFolder(Button Sender)
        {
            OpenMenu?.Close();
            Client.SendTCPData(ClientSendPacketFunctions.CreateFolder(CurrentlyOpenedFolderID, "Empty Folder"));
            //add auto select of fodler to rename here
        }
        public void CreateTransitionFile(Button Sender)
        {
            OpenMenu?.Close();
            Client.SendTCPData(ClientSendPacketFunctions.CreateFile(CurrentlyOpenedFolderID, "Empty Transition File", CoreFileType.TransitionFile));
        }
        public void CreateSlateFile(Button Sender)
        {
            OpenMenu?.Close();
            Client.SendTCPData(ClientSendPacketFunctions.CreateFile(CurrentlyOpenedFolderID, "Empty Slate File", CoreFileType.SlateFile));
        }
        public void CreateTapeFile(Button Sender)
        {
            OpenMenu?.Close();
            Client.SendTCPData(ClientSendPacketFunctions.CreateFile(CurrentlyOpenedFolderID, "Empty Tape Preset", CoreFileType.Tape));
        }
        public void CreateAlphabetFile(Button Sender)
        {
            OpenMenu?.Close();
            Client.SendTCPData(ClientSendPacketFunctions.CreateFile(CurrentlyOpenedFolderID, "Empty Alphabet", CoreFileType.Alphabet));
        }

        public void OpenFile(FileData Data)
        {
            IView ViewToAdd = null;
            switch (Data.Type)
            {
                case CoreFileType.Alphabet:
                    ViewToAdd = new AlphabetEditorView(Data.GUID);
                    break;
                case CoreFileType.Tape:
                    ViewToAdd = new TapeEditorView(Data.GUID);
                    break;
                case CoreFileType.TransitionFile:
                    ViewToAdd = new TextProgrammingView(Data.GUID);
                    OwnerWindow.OwnerScreen.SetActiveEditorWindow((IRunnable)ViewToAdd);
                    break;
                case CoreFileType.SlateFile:
                    ViewToAdd = new VisualProgrammingView(Data.GUID);
                    //OwnerWindow.OwnerScreen.SetActiveEditorWindow((IRunnable)ViewToAdd);
                    break;
                case CoreFileType.Other:
                    //display error
                    return;
            }

            if (ownerWindow.OwnerScreen.LastActiveWindow == null) 
            {
                ownerWindow.AddView(ViewToAdd);
                ownerWindow.OwnerScreen.LastActiveWindow = ownerWindow;
            }
            else
            {
                ownerWindow.OwnerScreen.LastActiveWindow.AddView(ViewToAdd);
            }
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
            Divider1.Position = Position + new Vector2(0, Searchbar.Bounds.Y);
            HierarchyLayout.Position = Divider1.Position + new Vector2(0, Divider1.Bounds.Y);
            Divider2.Position = HierarchyLayout.Position + new Vector2(0, HierarchyLayout.Bounds.Y);

            FileLayout.Position = Divider2.Position + new Vector2(0, Divider2.Bounds.Y);
        }

        void ResizeLayout()
        {
            Group.Width = bounds.X;
            Group.Height = bounds.Y;

            Background.Bounds = bounds;

            Divider1.Bounds = new Point(bounds.X, UIUtils.ConvertFloatToMinInt(GlobalInterfaceData.Scale(1), 1));
            Divider2.Bounds = new Point(bounds.X, UIUtils.ConvertFloatToMinInt(GlobalInterfaceData.Scale(1), 1));

            Searchbar.Bounds = new Point(bounds.X, UIUtils.ConvertFloatToInt(GlobalInterfaceData.Scale(28)));
            HierarchyLayout.Bounds = new Point(bounds.X, UIUtils.ConvertFloatToInt(GlobalInterfaceData.Scale(28)));
            FileLayout.Bounds = new Point(bounds.X, bounds.Y - Searchbar.Bounds.Y - HierarchyLayout.Bounds.Y - Divider1.Bounds.Y - Divider2.Bounds.Y);

            foreach (FileDisplayItem Item in Files)
            {
                Item.Bounds = new Point(FileLayout.Bounds.X, 0);
            }

        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                Background.Draw(BoundPort);
                Searchbar.Draw(BoundPort);
                Divider1.Draw(BoundPort);
                HierarchyLayout.Draw(BoundPort);
                Divider2.Draw(BoundPort);
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
