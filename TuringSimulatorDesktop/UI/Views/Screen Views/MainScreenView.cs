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

namespace TuringSimulatorDesktop.UI
{
    public class MainScreenView : View
    {        
        int Width, Height;
        ActionGroup Group;

        Icon Header;
        Icon Background;

        ButtonIcon CloseButton;

        Label Title;
        
        ButtonIcon NewProjectButton;
        ButtonIcon LoadProjectButton;
        ButtonIcon JoinProjectButton;
        ButtonIcon HostProjectButton;

        RecentFilesViewer Viewer;
        //DropDownMenu Menu;

        public MainScreenView()
        {          
            Group = InputManager.CreateActionGroup();
            ViewResize(GlobalRenderingData.Device.PresentationParameters.BackBufferWidth, GlobalRenderingData.Device.PresentationParameters.BackBufferHeight);

            //Background = new Icon(Width, Height, Vector2.Zero, GlobalInterfaceData.BackgroundColor);// UIMesh.CreateRectangle(Vector2.Zero, Width, Height, GlobalInterfaceData.BackgroundColor);
            //Header = new Icon(Width, GlobalInterfaceData.WindowTitleBarHeight, Vector2.Zero, GlobalInterfaceData.HeaderColor);//UIMesh.CreateRectangle(Vector2.Zero, Width, GlobalInterfaceData.WindowTitleBarHeight, GlobalInterfaceData.HeaderColor);

            CloseButton = new ButtonIcon(45, GlobalRenderingData.WindowTitleBarHeight, UILookupKey.Debug1, new Vector2(735, 0), Group);
            CloseButton.OnClickedEvent += Close;
            CloseButton.HighlightOnMouseOver = true;

            Title = new Label(new Vector2(0, 0), GlobalRenderingData.MediumRegularFont);
            Title.FontSize = 20;
            Title.Text = "T";

            Viewer = new RecentFilesViewer(500, 700, new Vector2(10, 42));
            if (GlobalProjectAndUserData.UserData != null)
            {
                Viewer.DisplayRecentFiles();
            }

            NewProjectButton = new ButtonIcon(250, 70, UILookupKey.NewProjectButton, UILookupKey.NewProjectButtonHightlight, new Vector2(520, 42), Group);
            NewProjectButton.HighlightOnMouseOver = true;

            LoadProjectButton = new ButtonIcon(250, 70, UILookupKey.LoadProjectButton, UILookupKey.LoadProjectButtonHightlight, new Vector2(520, 122), Group);
            LoadProjectButton.HighlightOnMouseOver = true;
            LoadProjectButton.OnClickedEvent += SelectProjectLocation;

            HostProjectButton = new ButtonIcon(250, 70, UILookupKey.HostProjectButton, new Vector2(520, 202), Group);
            HostProjectButton.HighlightOnMouseOver = true;

            JoinProjectButton = new ButtonIcon(250, 70, UILookupKey.JoinProjectButton, new Vector2(520, 282), Group);
            JoinProjectButton.HighlightOnMouseOver = true;

           // Menu = new DropDownMenu(60, 20, new Vector2(45, 6), Group);
        }

        public void SelectProjectLocation(Button Sender)
        {
            BackendInterface.StartProjectServer(2, 28104);
            Packet LoadPacket = ClientSendPacketFunctions.LoadProject("E:\\Professional Programming\\MAIN\\TestLocation");
            LoadPacket.InsertPacketLength();
            LoadPacket.SaveTemporaryBufferToPernamentReadBuffer();
            Server.AddPacketToProcessOnServerThread(0, LoadPacket);
            
            UIEventManager.ClientSuccessConnectingDelegate += ConnectedToServer;
            Client.ConnectToServer(System.Net.IPAddress.Parse("127.0.0.1"), 28104);
        }

        void ConnectedToServer(object sender, EventArgs e)
        {
            FileInfoWrapper OpenedProject = new FileInfoWrapper("TestProject", "E:\\Professional Programming\\MAIN\\TestLocation", DateTime.Now);
            GlobalProjectAndUserData.UserData.RecentlyAccessedFiles.Add(OpenedProject);
            GlobalProjectAndUserData.SaveUserData();

            UIEventManager.ClientSuccessConnecting = false;
            UIEventManager.ClientSuccessConnectingDelegate = null;

            InputManager.DeleteAllActionGroups();
            GlobalRenderingData.MainWindow.LeaveMainMenu();
            GlobalRenderingData.MainWindow.CurrentView = new ProjectScreenView();
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
