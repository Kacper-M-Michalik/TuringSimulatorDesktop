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
    public class WindowView : View
    {
        public int X { get; private set; }
        public int Y { get; private set; }
                
        public int Width;
        public int Height;
        public int TabHeight = GlobalGraphicsData.TabHeight;

        public Viewport Port;
        MeshRenderer Renderer;

        public List<View> Views;
        public List<Button> ViewChangeButtons;

        public View CurrentView;

        Mesh BackgroundTab;
        Mesh BackgroundWindow;

        //DEBUG
        public int LastProjectionX;
        public int LastProjectionY;
        public int LastProjectionWidth;
        public int LastProjectionHeight;

        public WindowView(int SetWidth, int SetHeight)
        {
            Width = SetWidth;
            Height = SetHeight;

            Renderer = new MeshRenderer(GlobalGraphicsData.Device, GlobalGraphicsData.Device.PresentationParameters.BackBufferWidth, GlobalGraphicsData.Device.PresentationParameters.BackBufferHeight);

            BackgroundTab = Mesh.CreateRectangle(Vector2.Zero, Width, TabHeight, GlobalGraphicsData.AccentColor);
            BackgroundWindow = Mesh.CreateRectangle(new Vector2(0f, TabHeight), Width, Height, GlobalGraphicsData.BackgroundColor);
            Renderer.AddMesh(BackgroundWindow);
            Renderer.AddMesh(BackgroundTab);

            DebugManager.LastCreatedWindow = this;

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

        public override void Draw()
        {
            Viewport Original = GlobalGraphicsData.Device.Viewport;
            GlobalGraphicsData.Device.Viewport = Port;
            Renderer.Draw();
            GlobalGraphicsData.Device.Viewport = Original;
        }

        public void SetWindowPosition(int SetX, int SetY)
        {
            X = SetX;
            Y = SetY;

            Renderer.Effect.View = Matrix.CreateTranslation(X, Y, 0f);

            RecalculateRenderData();
        }

        public void MoveWindow(int SetX, int SetY)
        {
            X += SetX;
            Y += SetY;

            Renderer.Effect.View = Matrix.CreateTranslation(X, Y, 0f);

            RecalculateRenderData();
        }

        public override void ViewResize(int SetWindowWidth, int SetWindowHeight)
        {
            TabHeight = GlobalGraphicsData.TabHeight;

            Width = SetWindowHeight;
            Height = SetWindowHeight;

            Renderer.DeleteMesh(BackgroundTab);
            Renderer.DeleteMesh(BackgroundWindow);
            BackgroundTab = Mesh.CreateRectangle(Vector2.Zero, Width, TabHeight, GlobalGraphicsData.AccentColor);
            BackgroundWindow = Mesh.CreateRectangle(new Vector2(0f, TabHeight), Width, Height, GlobalGraphicsData.BackgroundColor);
            Renderer.AddMesh(BackgroundWindow);
            Renderer.AddMesh(BackgroundTab);

            RecalculateRenderData();
        }

        public void RecalculateRenderData()
        {
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
    }
}
