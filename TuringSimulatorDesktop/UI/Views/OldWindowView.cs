using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI
{
    /*
    public class OldWindowView : View
    {
        public int X { get; private set; }
        public int Y { get; private set; }
                
        public int Width;
        public int Height;
        public int TabHeight = GlobalGraphicsData.WindowTabHeight;

        public Viewport Port;
        MeshRenderer Renderer;

        List<View> Views;
        List<Button> ViewChangeButtons;

        public View CurrentView;

        Mesh BackgroundTab;
        Mesh BackgroundWindow;

        Vector2 NextButtonPlacementPosition;

        //DEBUG
        public int LastProjectionX;
        public int LastProjectionY;
        public int LastProjectionWidth;
        public int LastProjectionHeight;
        //

        public OldWindowView(int SetWidth, int SetHeight)
        {
            Width = SetWidth;
            Height = SetHeight;

            Renderer = new MeshRenderer(GlobalGraphicsData.Device, GlobalGraphicsData.Device.PresentationParameters.BackBufferWidth, GlobalGraphicsData.Device.PresentationParameters.BackBufferHeight);

            BackgroundTab = Mesh.CreateRectangle(Vector2.Zero, Width, TabHeight, GlobalGraphicsData.AccentColor);
            BackgroundWindow = Mesh.CreateRectangle(new Vector2(0f, TabHeight), Width, Height, GlobalGraphicsData.BackgroundColor);
            Renderer.AddMesh(BackgroundWindow);
            Renderer.AddMesh(BackgroundTab);

            //DebugManager.LastCreatedWindow = this;

            Port = new Viewport();
            RecalculateRenderData();
        }

        public bool IsMouseOverWindow()
        {
            if (InputManager.MouseData.Y < (Y + Height) && InputManager.MouseData.Y > Y && InputManager.MouseData.X < (X + Width) && InputManager.MouseData.X > X) return true;
            return false;
        }

        public bool IsMouseOverTab()
        {
            if (InputManager.MouseData.Y < (Y + TabHeight) && InputManager.MouseData.Y > Y && InputManager.MouseData.X < (X + Width) && InputManager.MouseData.X > X) return true;
            return false;
        }

        public void SetWindowPosition(int SetX, int SetY)
        {
            X = SetX;
            Y = SetY;

            RecalculateRenderData();
        }

        public void MoveWindowPosition(int SetX, int SetY)
        {
            X += SetX;
            Y += SetY;

            RecalculateRenderData();
        }

        void RecalculateRenderData()
        {
            Renderer.Effect.View = Matrix.CreateTranslation(X, Y, 0f);

            Port.X = X;
            Port.Y = Y;
            Port.Width = Width;
            Port.Height = Height;

            LastProjectionX = X;
            LastProjectionY = Y;
            LastProjectionWidth = Width;
            LastProjectionHeight = Height;
            Renderer.RecalculateProjection(X, Y, Width, Height);
        }

        public override void Draw()
        {
            Viewport Original = GlobalGraphicsData.Device.Viewport;
            GlobalGraphicsData.Device.Viewport = Port;
            Renderer.Draw();
            if (CurrentView != null) CurrentView.Draw();
            GlobalGraphicsData.Device.Viewport = Original;
        }

        public override void ViewResize(int SetWindowWidth, int SetWindowHeight)
        {
            TabHeight = GlobalGraphicsData.WindowTabHeight;

            Width = SetWindowHeight;
            Height = SetWindowHeight;

            Renderer.DeleteMesh(BackgroundTab);
            Renderer.DeleteMesh(BackgroundWindow);
            BackgroundTab = Mesh.CreateRectangle(Vector2.Zero, Width, TabHeight, GlobalGraphicsData.AccentColor);
            BackgroundWindow = Mesh.CreateRectangle(new Vector2(0f, TabHeight), Width, Height, GlobalGraphicsData.BackgroundColor);
            Renderer.AddMesh(BackgroundWindow);
            Renderer.AddMesh(BackgroundTab);

            RecalculateRenderData();

            if (CurrentView != null) CurrentView.ViewResize(Width, Height);
        }

        public void AddView(View NewView)
        {
            Views.Add(NewView);
            Mesh M = Mesh.CreateRectangle(Vector2.Zero, 40f, 20f, GlobalGraphicsData.BackgroundColor);
            Renderer.AddMesh(M);
            Button NewButton = new Button(NextButtonPlacementPosition, M);
            NewButton.ClickEvent += (Button Sender) => { LoadView(Views.Count - 1); };
            ViewChangeButtons.Add(NewButton);
        }

        //problem -> if view coutn changes
        public void LoadView(int ID)
        {
            CurrentView = Views[ID];
            CurrentView.ViewResize(Width, Height);
        }

        public override void ViewPositionSet(int X, int Y)
        {
            throw new NotImplementedException();
        }
    }
    */
}
