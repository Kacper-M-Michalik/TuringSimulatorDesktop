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
        
        TextureButton NewProjectButton;
        TextureButton LoadProjectButton;
        TextureButton JoinProjectButton;
        TextureButton HostProjectButton;

        InputBox IPBox;
        RecentFilesViewer Viewer;
        //DropDownMenu Menu;

        public MainScreenView()
        {          
            Group = InputManager.CreateActionGroup();

            //Background = new Icon(Width, Height, Vector2.Zero, GlobalInterfaceData.BackgroundColor);// UIMesh.CreateRectangle(Vector2.Zero, Width, Height, GlobalInterfaceData.BackgroundColor);
            //Header = new Icon(Width, GlobalInterfaceData.WindowTitleBarHeight, Vector2.Zero, GlobalInterfaceData.HeaderColor);//UIMesh.CreateRectangle(Vector2.Zero, Width, GlobalInterfaceData.WindowTitleBarHeight, GlobalInterfaceData.HeaderColor);

            CloseButton = new TextureButton(45, 32, new Vector2(705, 0), Group);
            CloseButton.OnClickedEvent += Close;
            CloseButton.HighlightOnMouseOver = true;

            Title = new Label(new Vector2(0, 0), GlobalInterfaceData.MediumRegularFont);
            Title.FontSize = 20;
            Title.Text = "T";

            Viewer = new RecentFilesViewer(this);
            Viewer.Bounds = new Point(470, GlobalInterfaceData.MainMenuHeight - 52);
            Viewer.Position = new Vector2(10, 42);
            if (GlobalProjectAndUserData.UserData != null)
            {
                Viewer.DisplayRecentFiles();
            }

            NewProjectButton = new TextureButton(250, 70, new Vector2(490, 42), Group);
            //NewProjectButton.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.NewProjectButton];
            //NewProjectButton.HighlightTexture = GlobalInterfaceData.TextureLookup[UILookupKey.NewProjectButtonHightlight];
            NewProjectButton.HighlightOnMouseOver = true;

            LoadProjectButton = new TextureButton(250, 70, new Vector2(490, 122), Group);
            //NewProjectButton.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.LoadProjectButton];
            //NewProjectButton.HighlightTexture = GlobalInterfaceData.TextureLookup[UILookupKey.LoadProjectButtonHightlight];
            LoadProjectButton.HighlightOnMouseOver = true;
            LoadProjectButton.OnClickedEvent += SelectProjectLocation;

            HostProjectButton = new TextureButton(250, 70, new Vector2(490, 202), Group);
            //NewProjectButton.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.HostProjectButton];
            //NewProjectButton.HighlightTexture = GlobalInterfaceData.TextureLookup[UILookupKey.HostProjectButtonHightlight];
            HostProjectButton.HighlightOnMouseOver = true;

            JoinProjectButton = new TextureButton(250, 70, new Vector2(490, 282), Group);
            //NewProjectButton.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.JoinProjectButton];
            //NewProjectButton.HighlightTexture = GlobalInterfaceData.TextureLookup[UILookupKey.JoinProjectButtonHightlight];
            JoinProjectButton.HighlightOnMouseOver = true;
            JoinProjectButton.OnClickedEvent += ConnectToOtherDevice;


            IPBox = new InputBox(100, 20, Group);
            IPBox.Position = Vector2.Zero;

            ScreenResize();
           // Menu = new DropDownMenu(60, 20, new Vector2(45, 6), Group);
        }


        string Location;
        public void SelectProjectLocation(Button Sender)
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


        void ConnectToOtherDevice(Button Sender)
        {
            UIEventManager.RecievedProjectDataFromServerDelegate = FullyConnectedToServer;
            UIEventManager.ClientFailedConnectingDelegate = ResetConnection;
            Client.ConnectToServer(System.Net.IPAddress.Parse(IPBox.Text), 28104);
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

           // Menu.Draw();

            CloseButton.Draw();

            Viewer.Draw();

            NewProjectButton.Draw();
            LoadProjectButton.Draw();
            JoinProjectButton.Draw();
            HostProjectButton.Draw();

            IPBox.Draw();
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
