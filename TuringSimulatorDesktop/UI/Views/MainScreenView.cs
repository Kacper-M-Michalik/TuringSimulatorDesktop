using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.UI;
using TuringCore;
using TuringServer;
using TuringSimulatorDesktop.Input;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Threading;
using FontStashSharp;
using FontStashSharp.RichText;
using System.Runtime.InteropServices;

namespace TuringSimulatorDesktop.UI
{

    public class MainScreenView : View
    {        
        int Width, Height;
        ActionGroup Group;

        UIMesh Header;
        UIMesh Background;

        Button MinimiseButton;
        Button WindowButton;
        Button CloseButton;

        UIMesh Icon;
        Label Title;
        
        Button NewProjectButton;
        Button LoadProjectButton;
        Button JoinProjectButton;
        Button HostProjectButton;

        //DropDownMenu Menu;

        public MainScreenView()
        {          
            Group = InputManager.CreateActionGroup();
            ViewResize(GlobalInterfaceData.Device.PresentationParameters.BackBufferWidth, GlobalInterfaceData.Device.PresentationParameters.BackBufferHeight);

            Background = UIMesh.CreateRectangle(Vector2.Zero, Width, Height, GlobalInterfaceData.BackgroundColor);
            Header = UIMesh.CreateRectangle(Vector2.Zero, Width, GlobalInterfaceData.WindowTitleBarHeight, GlobalInterfaceData.HeaderColor);

            MinimiseButton = new Button(45, GlobalInterfaceData.WindowTitleBarHeight, UILookupKey.Debug1, new Vector2(1785, 0), Group);
            MinimiseButton.OnClickedEvent += Minimise;
            MinimiseButton.HighlightOnMouseOver = true;

            WindowButton = new Button(45, GlobalInterfaceData.WindowTitleBarHeight, UILookupKey.Debug1, new Vector2(1830, 0), Group);
            WindowButton.OnClickedEvent += Window;
            WindowButton.HighlightOnMouseOver = true;

            CloseButton = new Button(45, GlobalInterfaceData.WindowTitleBarHeight, UILookupKey.Debug1, new Vector2(1875, 0), Group);
            CloseButton.OnClickedEvent += Close;
            CloseButton.HighlightOnMouseOver = true;

            Title = new Label(new Vector2(1920/2, 0), GlobalInterfaceData.StandardRegularFont);
            Title.FontSize = 14;
            Title.Text = "TURING SIMULATOR DESKTOP";




            NewProjectButton = new Button(250, 70, UILookupKey.NewProjectButton, UILookupKey.NewProjectButtonHightlight, new Vector2(520, 42), Group);
            NewProjectButton.HighlightOnMouseOver = true;

            LoadProjectButton = new Button(250, 70, UILookupKey.LoadProjectButton, UILookupKey.LoadProjectButtonHightlight, new Vector2(520, 122), Group);
            LoadProjectButton.HighlightOnMouseOver = true;
            LoadProjectButton.OnClickedEvent += SelectProjectLocation;

            HostProjectButton = new Button(250, 70, UILookupKey.HostProjectButton, new Vector2(520, 202), Group);
            HostProjectButton.HighlightOnMouseOver = true;

            JoinProjectButton = new Button(250, 70, UILookupKey.JoinProjectButton, new Vector2(520, 282), Group);
            JoinProjectButton.HighlightOnMouseOver = true;

           // Menu = new DropDownMenu(60, 20, new Vector2(45, 6), Group);
        }

        public void SelectProjectLocation(Button Sender)
        {
            GlobalProjectAndUserData.UserData.RecentlyAccessedFiles.Add(new FileInfoWrapper("testfile", "dsadsaads/asddasasd", DateTime.Now));
            GlobalProjectAndUserData.SaveUserData();
            //BackendInterface.StartProjectServer(1, 28104);
            //conenct client here

            //InputManager.DeleteAllActionGroups();    
            //GlobalInterfaceData.MainWindow.CurrentView = new ProjectScreenView();
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
            GlobalUIRenderer.Draw(Background);
            GlobalUIRenderer.Draw(Header);
                     
            Title.Position = new Vector2(MathF.Round(1920 /2 + 3f*MathF.Cos((float)GlobalInterfaceData.Time.TotalGameTime.TotalSeconds)), MathF.Round(3f *MathF.Cos((float)GlobalInterfaceData.Time.TotalGameTime.TotalSeconds)));
            Title.Draw();

           // Menu.Draw();

            /*
            RichText.Text = "TURING SIMULATOR DESKTOP"; 
            Batch.Begin();
            Batch.DrawString(RichText.Font, "TURING SIMULATOR DESKTOP", new Vector2(MathF.Round(1920 / 2 + 3f * MathF.Cos((float)GlobalInterfaceData.Time.TotalGameTime.TotalSeconds)), MathF.Round(20 + 3f * MathF.Cos((float)GlobalInterfaceData.Time.TotalGameTime.TotalSeconds))), GlobalInterfaceData.FontColor);
            Batch.End();

            Batch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap);
            RichText.Draw(Batch, new Vector2(MathF.Round(1920 / 2 + 3f * MathF.Cos((float)GlobalInterfaceData.Time.TotalGameTime.TotalSeconds)), MathF.Round(80 + 3f * MathF.Cos((float)GlobalInterfaceData.Time.TotalGameTime.TotalSeconds))), GlobalInterfaceData.FontColor);
            Batch.End(); 
            
            Batch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearClamp);
            RichText.Draw(Batch, new Vector2(MathF.Round(1920 / 2 + 3f * MathF.Cos((float)GlobalInterfaceData.Time.TotalGameTime.TotalSeconds)), MathF.Round(100 + 3f * MathF.Cos((float)GlobalInterfaceData.Time.TotalGameTime.TotalSeconds))), GlobalInterfaceData.FontColor);
            Batch.End();

            Batch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            RichText.Draw(Batch, new Vector2(MathF.Round( 1920 / 2 + 3f * MathF.Cos((float)GlobalInterfaceData.Time.TotalGameTime.TotalSeconds)), MathF.Round(40+ 3f * MathF.Cos((float)GlobalInterfaceData.Time.TotalGameTime.TotalSeconds))), GlobalInterfaceData.FontColor);
            Batch.End();

            Batch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);
            RichText.Draw(Batch, new Vector2(MathF.Round(1920 / 2 + 3f * MathF.Cos((float)GlobalInterfaceData.Time.TotalGameTime.TotalSeconds)), MathF.Round(60 + 3f * MathF.Cos((float)GlobalInterfaceData.Time.TotalGameTime.TotalSeconds))), GlobalInterfaceData.FontColor);
            Batch.End();
            */

            MinimiseButton.Draw();
            WindowButton.Draw();
            CloseButton.Draw();

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

            Background?.UpdateMesh(UIMesh.CreateRectangle(Vector2.Zero, Width, Height));
            Header?.UpdateMesh(UIMesh.CreateRectangle(Vector2.Zero, Width, GlobalInterfaceData.WindowTitleBarHeight));
        }

        public override void ViewPositionSet(int X, int Y)
        {
        }
    }
}
