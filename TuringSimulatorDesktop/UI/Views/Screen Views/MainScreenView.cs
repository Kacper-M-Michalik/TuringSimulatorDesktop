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
using System.Net;

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

        IClosable OpenMenu;

        public MainScreenView()
        {          
            Group = InputManager.CreateActionGroup();


            CloseButton = new TextureButton(45, 32, new Vector2(705, 0), Group);
            CloseButton.OnClickedEvent += Close;
            CloseButton.HighlightOnMouseOver = true;
            CloseButton.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.CloseApplicationIcon];
            CloseButton.HighlightTexture = GlobalInterfaceData.TextureLookup[UILookupKey.CloseApplicationHighlightIcon];



            NewProjectButton = new ElementCollection(); 

            ColorButton CreateButton = new ColorButton(Group);
            CreateButton.Bounds = new Point(250, 60);
            
            CreateButton.BaseColor = GlobalInterfaceData.Scheme.InteractableAccent;
            CreateButton.HighlightColor = GlobalInterfaceData.Scheme.DarkInteractableAccent;
            CreateButton.HighlightOnMouseOver = true;
            CreateButton.OnClickedEvent += CreateNewProject;

            Icon CreateIcon = new Icon();
            CreateIcon.Bounds = new Point();
            CreateIcon.Position = new Vector2();

            Label CreateLabel = new Label();
            CreateLabel.DrawCentered = true;
            CreateLabel.AutoSizeMesh = false;
            CreateLabel.Bounds = new Point(250, 60);
            CreateLabel.FontSize = 20;
            CreateLabel.Text = "Create Project";

            NewProjectButton.AddElement(CreateButton);
            NewProjectButton.AddElement(CreateIcon);
            NewProjectButton.AddElement(CreateLabel);

            NewProjectButton.Offsets[2] = new Vector2(0, CreateButton.Bounds.Y * 0.5f);
            NewProjectButton.Position = new Vector2(14, 50);


            LoadProjectButton = new ElementCollection();

            ColorButton LoadButton = new ColorButton(Group);
            LoadButton.Bounds = new Point(250, 60);
            LoadButton.BaseColor = GlobalInterfaceData.Scheme.InteractableAccent;
            LoadButton.HighlightColor = GlobalInterfaceData.Scheme.DarkInteractableAccent;
            LoadButton.HighlightOnMouseOver = true;
            LoadButton.OnClickedEvent += LoadProject;

            Icon LoadIcon = new Icon();
            LoadIcon.Bounds = new Point();
            LoadIcon.Position = new Vector2();

            Label LoadLabel = new Label();
            LoadLabel.DrawCentered = true;
            LoadLabel.AutoSizeMesh = false;
            LoadLabel.Bounds = new Point(250, 60);
            LoadLabel.FontSize = 20;
            LoadLabel.Text = "Load Project";

            LoadProjectButton.AddElement(LoadButton);
            LoadProjectButton.AddElement(LoadIcon);
            LoadProjectButton.AddElement(LoadLabel);

            LoadProjectButton.Offsets[2] = new Vector2(0, LoadButton.Bounds.Y * 0.5f);
            LoadProjectButton.Position = new Vector2(14, 126);



            JoinProjectButton = new ElementCollection();

            ColorButton JoinButton = new ColorButton(Group);
            JoinButton.Bounds = new Point(250, 60);
            JoinButton.BaseColor = GlobalInterfaceData.Scheme.InteractableAccent;
            JoinButton.HighlightColor = GlobalInterfaceData.Scheme.DarkInteractableAccent;
            JoinButton.HighlightOnMouseOver = true;
            JoinButton.OnClickedEvent += JoinProject;

            Icon JoinIcon = new Icon();
            JoinIcon.DrawTexture = GlobalInterfaceData.TextureLookup[UILookupKey.ConnectIcon];
            JoinIcon.Bounds = new Point(12, 28);
            JoinIcon.Position = new Vector2(25, 16);

            Label JoinLabel = new Label();
            JoinLabel.DrawCentered = true;
            JoinLabel.AutoSizeMesh = false;
            JoinLabel.Bounds = new Point(250, 60);
            JoinLabel.FontSize = 20;
            JoinLabel.Text = "Join Project";

            JoinProjectButton.AddElement(JoinButton);
            JoinProjectButton.AddElement(JoinIcon);
            JoinProjectButton.AddElement(JoinLabel);

            JoinProjectButton.Offsets[2] = new Vector2(0, JoinButton.Bounds.Y * 0.5f);
            JoinProjectButton.Position = new Vector2(14, 202);


            Title = new Label(GlobalInterfaceData.MediumRegularFont);
            Title.FontSize = 20;
            Title.FontColor = GlobalInterfaceData.Scheme.FontColorBright;
            Title.Text = "T";
            Title.Position = new Vector2(16, 16);

            RecentFilesMenu Viewer = new RecentFilesMenu(this);
            Viewer.Bounds = new Point(450, GlobalInterfaceData.MainMenuHeight - 36 - 32);
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
            OpenMenu.Close();

            CreateProjectMenu Menu = new CreateProjectMenu(this);
            Menu.Bounds = new Point(450, GlobalInterfaceData.MainMenuHeight - 36 - 32);
            Menu.Position = new Vector2(282, 50);

            OpenMenu = Menu;
        }

        public void JoinProject(Button Sender)
        {
            OpenMenu.Close();

            JoinProjectMenu Menu = new JoinProjectMenu(this);
            Menu.Bounds = new Point(450, GlobalInterfaceData.MainMenuHeight - 36 - 32);
            Menu.Position = new Vector2(282, 50);

            OpenMenu = Menu;
        }

        public void LoadProject(Button Sender)
        {
            OpenMenu.Close();

            LoadProjectMenu Menu = new LoadProjectMenu(this);
            Menu.Bounds = new Point(450, GlobalInterfaceData.MainMenuHeight - 36 - 32);
            Menu.Position = new Vector2(282, 50);

            OpenMenu = Menu;
        }

        string Location;

        public void SelectedProject(string location, int MaxClientCount)
        {
            BackendInterface.StartProjectServer(MaxClientCount, 28104);
            Location = location;
            UIEventManager.ClientSuccessConnectingDelegate = ConnectedToLocalServer;
            UIEventManager.ClientFailedConnectingDelegate = ResetConnection;
            Client.ConnectToServer(IPAddress.Parse("127.0.0.1"), 28104);
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


        public void ConnectToOtherDevice(IPAddress IP)
        {
            UIEventManager.RecievedProjectDataFromServerDelegate = FullyConnectedToServer;
            UIEventManager.ClientFailedConnectingDelegate = ResetConnection;
            Client.ConnectToServer(IP, 28104);
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
            //temp?
            ProjectView.UpdatedProject(null, null);
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
