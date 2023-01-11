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

        Button CloseButton;

        Label Title;
        
        Button NewProjectButton;
        Button LoadProjectButton;
        Button JoinProjectButton;
        Button HostProjectButton;

        InputBox IPBox;
        RecentFilesViewer Viewer;
        //DropDownMenu Menu;

        public MainScreenView()
        {          
            Group = InputManager.CreateActionGroup();
            ViewResize(GlobalRenderingData.Device.PresentationParameters.BackBufferWidth, GlobalRenderingData.Device.PresentationParameters.BackBufferHeight);

            //Background = new Icon(Width, Height, Vector2.Zero, GlobalInterfaceData.BackgroundColor);// UIMesh.CreateRectangle(Vector2.Zero, Width, Height, GlobalInterfaceData.BackgroundColor);
            //Header = new Icon(Width, GlobalInterfaceData.WindowTitleBarHeight, Vector2.Zero, GlobalInterfaceData.HeaderColor);//UIMesh.CreateRectangle(Vector2.Zero, Width, GlobalInterfaceData.WindowTitleBarHeight, GlobalInterfaceData.HeaderColor);

            CloseButton = new Button(45, GlobalRenderingData.WindowTitleBarHeight, UILookupKey.Debug1, new Vector2(705, 0), Group);
            CloseButton.OnClickedEvent += Close;
            CloseButton.HighlightOnMouseOver = true;

            Title = new Label(new Vector2(0, 0), GlobalRenderingData.MediumRegularFont);
            Title.FontSize = 20;
            Title.Text = "T";

            Viewer = new RecentFilesViewer(this);
            Viewer.Bounds = new Point(470, GlobalRenderingData.MainMenuHeight - 52);
            Viewer.Position = new Vector2(10, 42);
            if (GlobalProjectAndUserData.UserData != null)
            {
                Viewer.DisplayRecentFiles();
            }

            NewProjectButton = new Button(250, 70, UILookupKey.NewProjectButton, UILookupKey.NewProjectButtonHightlight, new Vector2(490, 42), Group);
            NewProjectButton.HighlightOnMouseOver = true;

            LoadProjectButton = new Button(250, 70, UILookupKey.LoadProjectButton, UILookupKey.LoadProjectButtonHightlight, new Vector2(490, 122), Group);
            LoadProjectButton.HighlightOnMouseOver = true;
            LoadProjectButton.OnClickedEvent += SelectProjectLocation;

            HostProjectButton = new Button(250, 70, UILookupKey.HostProjectButton, UILookupKey.HostProjectButtonHightlight, new Vector2(490, 202), Group);
            HostProjectButton.HighlightOnMouseOver = true;

            JoinProjectButton = new Button(250, 70, UILookupKey.JoinProjectButton, UILookupKey.JoinProjectButtonHightlight, new Vector2(490, 282), Group);
            JoinProjectButton.HighlightOnMouseOver = true;
            JoinProjectButton.OnClickedEvent += ConnectToOtherDevice;


            IPBox = new InputBox(100, 20, Group);
            IPBox.Position = Vector2.Zero;
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
            //UIEventManager.ClientFailedConnecting//
            Client.ConnectToServer(System.Net.IPAddress.Parse("127.0.0.1"), 28104);
        }
        void ConnectedToLocalServer(object sender, EventArgs e)
        {
            UIEventManager.ClientSuccessConnecting = false;
            UIEventManager.ClientSuccessConnectingDelegate = null;
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
            //UIEventManager.ClientFailedConnecting//
            Client.ConnectToServer(System.Net.IPAddress.Parse(IPBox.Text), 28104);
        }


        void FullyConnectedToServer(object sender, EventArgs e)
        {
            UIEventManager.ClientSuccessConnecting = false;
            UIEventManager.ClientSuccessConnectingDelegate = null;

            InputManager.DeleteAllActionGroups();
            GlobalRenderingData.MainWindow.LeaveMainMenu();
            ProjectScreenView ProjectView = new ProjectScreenView();
            UIEventManager.RecievedProjectDataFromServerDelegate = ProjectView.UpdatedProject;
            GlobalRenderingData.MainWindow.CurrentView = ProjectView;
        }

        public void Minimise(Button Sender)
        {
            GlobalRenderingData.MainWindow.MinimiseWindow();
        }
        public void Window(Button Sender)
        {
            GlobalRenderingData.MainWindow.MaximiseWindow();
        }
        public void Close(Button Sender)
        {
            GlobalRenderingData.MainWindow.Exit();
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

        public override void ViewResize(int NewWidth, int NewHeight)
        {
            Width = NewWidth;
            Height = NewHeight;
            Group.Width = NewWidth;
            Group.Height = NewHeight;

            Background = new Icon(Width, Height, Vector2.Zero, GlobalRenderingData.BackgroundColor);
            Header = new Icon(Width, GlobalRenderingData.WindowTitleBarHeight, Vector2.Zero, GlobalRenderingData.HeaderColor);
        }

        public override void ViewPositionSet(int X, int Y)
        {

        }
    }
}
