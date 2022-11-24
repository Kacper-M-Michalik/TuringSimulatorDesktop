using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TuringSimulatorDesktop.UI;
using TuringCore;
using TuringServer;
using TuringSimulatorDesktop.Input;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Threading;

namespace TuringSimulatorDesktop.UI
{
    public class MainScreenView : View
    {
        List<IRenderable> RenderElements;
        Viewport Port;

        public Button NewProjectButton;
        public Button LoadProjectButton;
        public Button JoinProjectButton;

        public OpenFileDialog Dialog;

        public MainScreenView(int Width, int Height)
        {
            Port = new Viewport(0, 0, Width, Height);
            RenderElements = new List<IRenderable>();

            NewProjectButton = new Button(new Vector2(100f,100f), Mesh.CreateRectangle(Vector2.Zero, 100f, 30f, Color.White), ElementCreateType.Persistent);
            LoadProjectButton = new Button(new Vector2(100f, 150f), Mesh.CreateRectangle(Vector2.Zero, 100f, 30f, Color.Red), ElementCreateType.Persistent);
            LoadProjectButton.ClickEvent += SelectProjectLocation;

            JoinProjectButton = new Button(new Vector2(100f, 200f), Mesh.CreateRectangle(Vector2.Zero, 100f, 30f, Color.Blue), ElementCreateType.Persistent);

            RenderElements.Add(NewProjectButton);
            RenderElements.Add(LoadProjectButton);
            RenderElements.Add(JoinProjectButton);
        
        }

        public void SelectProjectLocation(Button Sender)
        {
            /*
            Dialog = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
                Title = "Browse Turing Project Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "tproj",
                Filter = "tproj files (*.tproj)|*.tproj",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };
            Dialog.ShowDialog();
                /*
            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                ProjectInstance.StartProjectServer(Dialog.FileName, 1, 28104);
                //conenct client here

                GlobalGraphicsData.BaseWindow.CurrentView = new ProjectScreenView();
            }
                */
            // "E:\\Professional Programming\\MAIN\\TestLocation"

            BackendInterface.StartProjectServer(1, 28104);
            //conenct client here

            InputManager.RemoveAllListenersOnQueue();    
            GlobalGraphicsData.BaseWindow.CurrentView = new ProjectScreenView();

        }

        public override void Draw()
        {
            GlobalMeshRenderer.Draw(RenderElements, Port);
        }

        public override void ViewResize(int NewWidth, int NewHeight)
        {
            Port.Width = NewWidth;
            Port.Height = NewHeight;
        }
    }
}
