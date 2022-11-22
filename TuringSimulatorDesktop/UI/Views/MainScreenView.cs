using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TuringSimulatorDesktop.UI;
using TuringBackend;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using TuringSimulatorDesktop.Input;
using System.Threading;

namespace TuringSimulatorDesktop.UI
{
    public class MainScreenView : View
    {
        MeshRenderer Renderer;

        public Button NewProjectButton;
        public Button LoadProjectButton;
        public Button JoinProjectButton;

        public OpenFileDialog Dialog;

        public MainScreenView(int Width, int Height)
        {
            Renderer = new MeshRenderer(GlobalGraphicsData.Device, Width, Height);

            NewProjectButton = new Button(new Vector2(100f,100f), Mesh.CreateRectangle(Vector2.Zero, 100f, 30f, Color.White), ElementCreateType.Persistent);

            LoadProjectButton = new Button(new Vector2(100f, 150f), Mesh.CreateRectangle(Vector2.Zero, 100f, 30f, Color.Red), ElementCreateType.Persistent);
            LoadProjectButton.ClickEvent += SelectProjectLocation;

            JoinProjectButton = new Button(new Vector2(100f, 200f), Mesh.CreateRectangle(Vector2.Zero, 100f, 30f, Color.Blue), ElementCreateType.Persistent);

            Renderer.AddMesh(NewProjectButton.MeshData);
            Renderer.AddMesh(LoadProjectButton.MeshData);
            Renderer.AddMesh(JoinProjectButton.MeshData);
        }

        ~MainScreenView()
        {
            Renderer.Dispose();
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
            //client.sendloadproject

            InputManager.RemoveAllListenersOnQueue();    
            GlobalGraphicsData.BaseWindow.CurrentView = new ProjectScreenView();

        }

        public override void Draw()
        {
            Renderer.Draw();
        }

        public override void ViewResize(int NewWidth, int NewHeight)
        {
            Renderer.RecalculateProjection(0, 0, NewWidth, NewHeight);
        }
    }
}
