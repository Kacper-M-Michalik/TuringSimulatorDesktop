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

namespace TuringSimulatorDesktop.UI
{
    public class MainScreenView : View
    {
        ActionGroup Group;

        Label Title;
        //Icon TuringIcon;
        Button NewProjectButton;
        Button LoadProjectButton;
        Button JoinProjectButton;
        InputBox TestInput;

        UIMesh Background;

        public MainScreenView()
        {
            Group = InputManager.CreateActionGroup();
            ViewResize(GlobalInterfaceData.Device.PresentationParameters.BackBufferWidth, GlobalInterfaceData.Device.PresentationParameters.BackBufferHeight);
            
            Title = new Label(100, 200, new Vector2(50f, 50f));
            Title.Text = "Turing Simulator - Dekstop";
            Title.FontSize = 48;
            Title.AutoSizeMesh = true;
            Title.UpdateTexture();
            //TuringIcon = new Icon():

            TestInput = new InputBox(100, 100, new Vector2(300, 0), Group);
            TestInput.OutputLabel.FontSize = 20;
            TestInput.OutputLabel.AutoSizeMesh = false;

            NewProjectButton = new Button(100, 30, new Vector2(100f,100f), Group);
            LoadProjectButton = new Button(100, 30, new Vector2(100f, 150f), Group);
            LoadProjectButton.OnClickedEvent += SelectProjectLocation;
            JoinProjectButton = new Button(100, 30, new Vector2(100f, 200f), Group);
        }

        public void SelectProjectLocation(Button Sender)
        {
            BackendInterface.StartProjectServer(1, 28104);
            //conenct client here

            InputManager.DeleteAllActionGroups();    
            GlobalInterfaceData.BaseWindow.CurrentView = new ProjectScreenView();
        }

        public override void Draw()
        {
            GlobalUIRenderer.Draw(Background);
            NewProjectButton.Draw();
            LoadProjectButton.Draw();
            JoinProjectButton.Draw();
            Title.Draw();
            TestInput.Draw();

            //GlobalMeshRenderer.Draw(Background, Port);
            //GlobalMeshRenderer.Draw(NewProjectButton, Port);
            //GlobalMeshRenderer.Draw(LoadProjectButton, Port);
            //GlobalMeshRenderer.Draw(JoinProjectButton, Port);
            //GlobalMeshRenderer.Draw(TuringIcon, Port);
            //GlobalMeshRenderer.Draw(Title, Port);
        }

        public override void ViewResize(int NewWidth, int NewHeight)
        {
            Group.Width = NewWidth;
            Group.Height = NewHeight;

            Background = UIMesh.CreateRectangle(Vector2.Zero, NewWidth, NewHeight, Color.CornflowerBlue);
        }

        public override void ViewPositionSet(int X, int Y)
        {
        }
    }
}
