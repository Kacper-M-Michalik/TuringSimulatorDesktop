using System;
using System.Collections.Generic;
using TuringSimulatorDesktop.UI;
using TuringCore;
using TuringServer;
using TuringSimulatorDesktop.Input;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using TuringSimulatorDesktop.UI.Prefabs;
using System.Windows.Forms;

namespace TuringSimulatorDesktop.UI
{
    public class MainScreenView : ScreenView
    {        
        int Width, Height;
        ActionGroup Group;

        Icon Header;
        Icon Background;

        TextureButton CloseButton;

        Label Title;
        
        ElementCollection NewProjectButton;
        ElementCollection LoadProjectButton;
        ElementCollection JoinProjectButton;
        ElementCollection HostProjectButton;

        IVisualElement OpenMenu;

        public MainScreenView()
        {          
            Group = InputManager.CreateActionGroup();


            CloseButton = new TextureButton(45, 32, new Vector2(705, 0), Group);
            CloseButton.OnClickedEvent += Close;
            CloseButton.HighlightOnMouseOver = true;


            NewProjectButton = new ElementCollection(); 

            ColorButton CreateButton = new ColorButton(Group);
            CreateButton.Bounds = new Point(250, 90);
            CreateButton.BaseColor = GlobalInterfaceData.Scheme.InteractableAccent;
            CreateButton.HighlightColor = GlobalInterfaceData.Scheme.DarkInteractableAccent;
            CreateButton.HighlightOnMouseOver = true;
            CreateButton.OnClickedEvent += CreateNewProject;

            Icon CreateIcon = new Icon();
            CreateIcon.Bounds = new Point();
            CreateIcon.Position = new Vector2();

            Label CreateLabel = new Label();
            CreateLabel.FontSize = 24;
            CreateLabel.Text = "Create Project";

            NewProjectButton.AddElement(CreateButton);
            NewProjectButton.AddElement(CreateIcon);
            NewProjectButton.AddElement(CreateLabel);

            NewProjectButton.Offsets[2] = new Vector2(0, CreateButton.Bounds.Y * 0.5f);
            NewProjectButton.Position = new Vector2(14, 50);


            LoadProjectButton = new ElementCollection();

            ColorButton LoadButton = new ColorButton(Group);
            LoadButton.Bounds = new Point(250, 90);
            LoadButton.BaseColor = GlobalInterfaceData.Scheme.InteractableAccent;
            LoadButton.HighlightColor = GlobalInterfaceData.Scheme.DarkInteractableAccent;
            LoadButton.HighlightOnMouseOver = true;
            LoadButton.OnClickedEvent += LoadProject;

            Icon LoadIcon = new Icon();
            LoadIcon.Bounds = new Point();
            LoadIcon.Position = new Vector2();

            Label LoadLabel = new Label();
            LoadLabel.FontSize = 24;
            LoadLabel.Text = "Load Project";

            LoadProjectButton.AddElement(LoadButton);
            LoadProjectButton.AddElement(LoadIcon);
            LoadProjectButton.AddElement(LoadLabel);

            LoadProjectButton.Offsets[2] = new Vector2(0, LoadButton.Bounds.Y * 0.5f);
            LoadProjectButton.Position = new Vector2(14, 158);


            HostProjectButton = new ElementCollection();

            ColorButton HostButton = new ColorButton(Group);
            HostButton.Bounds = new Point(250, 90);
            HostButton.BaseColor = GlobalInterfaceData.Scheme.InteractableAccent;
            HostButton.HighlightColor = GlobalInterfaceData.Scheme.DarkInteractableAccent;
            HostButton.HighlightOnMouseOver = true;
            HostButton.OnClickedEvent += HostProject;

            Icon HostIcon = new Icon();
            HostIcon.Bounds = new Point();
            HostIcon.Position = new Vector2();

            Label HostLabel = new Label();
            HostLabel.FontSize = 24;
            HostLabel.Text = "Host Project";

            HostProjectButton.AddElement(HostButton);
            HostProjectButton.AddElement(HostIcon);
            HostProjectButton.AddElement(HostLabel);

            HostProjectButton.Offsets[2] = new Vector2(0, HostButton.Bounds.Y * 0.5f);
            HostProjectButton.Position = new Vector2(14, 266);


            JoinProjectButton = new ElementCollection();

            ColorButton JoinButton = new ColorButton(Group);
            JoinButton.Bounds = new Point(250, 90);
            JoinButton.BaseColor = GlobalInterfaceData.Scheme.InteractableAccent;
            JoinButton.HighlightColor = GlobalInterfaceData.Scheme.DarkInteractableAccent;
            JoinButton.HighlightOnMouseOver = true;
            JoinButton.OnClickedEvent += HostProject;

            Icon JoinIcon = new Icon();
            JoinIcon.Bounds = new Point();
            JoinIcon.Position = new Vector2();

            Label JoinLabel = new Label();
            JoinLabel.FontSize = 24;
            JoinLabel.Text = "Join Project";

            JoinProjectButton.AddElement(JoinButton);
            JoinProjectButton.AddElement(JoinIcon);
            JoinProjectButton.AddElement(JoinLabel);

            JoinProjectButton.Offsets[2] = new Vector2(0, JoinButton.Bounds.Y * 0.5f);
            JoinProjectButton.Position = new Vector2(14, 374);


            Title = new Label(GlobalInterfaceData.MediumRegularFont);
            Title.FontSize = 20;
            Title.FontColor = GlobalInterfaceData.Scheme.FontColorBright;
            Title.Text = "T";
            Title.Position = new Vector2(0, Title.Bounds.Y * 0.5f);

            RecentFilesMenu Viewer = new RecentFilesMenu(this);
            Viewer.Bounds = new Point(454, GlobalInterfaceData.MainMenuHeight - 36 - 32);
            Viewer.Position = new Vector2(282, 50);

            if (GlobalProjectAndUserData.UserData != null)
            {
                Viewer.DisplayRecentFiles();
            }

            OpenMenu = Viewer;

            ScreenResize();
        }


        public void CreateNewProject(Button Sender)
        {

        }

        string Location;
        public void LoadProject(Button Sender)
        {
            OpenFileDialog Dialog = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
                Title = "Browse tproj Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "tproj",
                Filter = "tproj files (*.tproj)|*.tproj",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                SelectedProject(Dialog.FileName);
            }
        }

        public void SelectedProject(string location)
        {
            //2 TEMP
            BackendInterface.StartProjectServer(2, 28104);
            Location = location;
            UIEventManager.ClientSuccessConnectingDelegate = ConnectedToLocalServer;
            UIEventManager.ClientFailedConnectingDelegate = ResetConnection;
            Client.ConnectToServer(System.Net.IPAddress.Parse("127.0.0.1"), 28104);
        }
        void ConnectedToLocalServer(object sender, EventArgs e)
        {
            UIEventManager.ClientSuccessConnecting = false;
            UIEventManager.ClientSuccessConnectingDelegate = null;
            //add some sort of timeout to this
            UIEventManager.RecievedProjectDataFromServerDelegate = ServerSentProjectData;
            Client.SendTCPData(ClientSendPacketFunctions.LoadProject(Location));
        }
        void ServerSentProjectData(object sender, EventArgs e)
        {   
            GlobalProjectAndUserData.UpdateRecentlyOpenedFile(Location);
            FullyConnectedToServer(this, null);
        }


        void ConnectToOtherDevice(string IP)
        {
            UIEventManager.RecievedProjectDataFromServerDelegate = FullyConnectedToServer;
            UIEventManager.ClientFailedConnectingDelegate = ResetConnection;
            Client.ConnectToServer(System.Net.IPAddress.Parse(IP), 28104);
        }

        void ResetConnection(object sender, EventArgs e)
        {
            UIEventManager.ClientSuccessConnecting = false;
            UIEventManager.ClientSuccessConnectingDelegate = null;
            UIEventManager.ClientFailedConnecting = false;
            UIEventManager.ClientFailedConnectingDelegate = null;
        }

        void FullyConnectedToServer(object sender, EventArgs e)
        {
            ResetConnection(null, null);

            InputManager.DeleteAllActionGroups();
            GlobalInterfaceData.MainWindow.LeaveMainMenu();
            ProjectScreenView ProjectView = new ProjectScreenView();
            UIEventManager.RecievedProjectDataFromServerDelegate = ProjectView.UpdatedProject;
            GlobalInterfaceData.MainWindow.CurrentView = ProjectView;
        }

        public void HostProject(Button Sender)
        {

        }

        public void JoinProject(Button Sender)
        {

        }

        public void Minimise(Button Sender)
        {
            GlobalInterfaceData.MainWindow.MinimiseWindow();
        }
        public void Window(Button Sender)
        {
            GlobalInterfaceData.MainWindow.MaximiseWindow();
        }
        public void Close(Button Sender)
        {
            GlobalInterfaceData.MainWindow.Exit();
        }

        public override void Draw()
        {
            Background.Draw();
            Header.Draw();

            Title.Draw();

            CloseButton.Draw();

            NewProjectButton.Draw();
            LoadProjectButton.Draw();
            JoinProjectButton.Draw();
            HostProjectButton.Draw();

            OpenMenu.Draw();
        }

        public override void ScreenResize()
        {
            int Width = GlobalInterfaceData.Device.PresentationParameters.BackBufferWidth;
            int Height = GlobalInterfaceData.Device.PresentationParameters.BackBufferHeight;

            Group.Width = Width;
            Group.Height = Height;

            Background = new Icon(Width, Height, Vector2.Zero, GlobalInterfaceData.Scheme.Background);
            Header = new Icon(Width, 32, Vector2.Zero, GlobalInterfaceData.Scheme.Header);
        }

    }
}
