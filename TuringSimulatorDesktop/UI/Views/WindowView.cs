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
        public int X { get { return Port.X; } }
        public int Y { get { return Port.Y; } }
        public int Width { get { return Port.Width; } }
        public int Height { get { return Port.Height; } }

        public Viewport Port;
        public int TabHeight;

        List<View> Views;
        List<Button> ViewChangeButtons;
        public View CurrentView;

        Mesh BackgroundTab;
        Mesh Background;

        List<IRenderable> RenderElements;

        Vector2 NextButtonPlacementPosition;

        //DEBUG
        public int LastProjectionX;
        public int LastProjectionY;
        public int LastProjectionWidth;
        public int LastProjectionHeight;
        //

        public WindowView(int SetWidth, int SetHeight)
        {
            Port = new Viewport(0, 0, SetWidth, SetHeight);
            RenderElements = new List<IRenderable>();

            TabHeight = GlobalGraphicsData.WindowTabHeight;

            BackgroundTab = Mesh.CreateRectangle(Vector2.Zero, Width, TabHeight, GlobalGraphicsData.AccentColor);
            Background = Mesh.CreateRectangle(Vector2.Zero, Width, Height, GlobalGraphicsData.BackgroundColor);

            Views = new List<View>();
            ViewChangeButtons = new List<Button>();

            Button TestButton = new Button(Vector2.Zero, Mesh.CreateRectangle(Vector2.Zero, 100f, 40f, Color.Red));
            ViewChangeButtons.Add(TestButton);
            RenderElements.Add(TestButton);

            DebugManager.LastCreatedWindow = this;
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
            Port.X = SetX;
            Port.Y = SetY;

            ViewResize(Width, Height);
        }

        public void MoveWindowPosition(int MoveX, int MoveY)
        {
            Port.X += MoveX;
            Port.Y += MoveY;

            ViewResize(Width, Height);
        }

        public override void Draw()
        {
            GlobalMeshRenderer.Draw(Background, Port);
            if (CurrentView != null) CurrentView.Draw();
            GlobalMeshRenderer.Draw(BackgroundTab, Port);
            GlobalMeshRenderer.Draw(RenderElements, Port);
        }

        //will have to manually move all ui elements
        public override void ViewResize(int SetWindowWidth, int SetWindowHeight)
        {
            TabHeight = GlobalGraphicsData.WindowTabHeight;

            Port.Width = SetWindowWidth;
            Port.Height = SetWindowHeight;

            BackgroundTab = Mesh.CreateRectangle(new Vector2(X, Y), Width, TabHeight, GlobalGraphicsData.AccentColor);
            Background = Mesh.CreateRectangle(new Vector2(X, Y + TabHeight), Width, Height, GlobalGraphicsData.BackgroundColor);

            LastProjectionX = X;
            LastProjectionY = Y;
            LastProjectionWidth = Width;
            LastProjectionHeight = Height;

            if (CurrentView != null) CurrentView.ViewResize(Width, Height);
        }

        public void AddView(View NewView)
        {
            Views.Add(NewView);
            Button NewButton = new Button(NextButtonPlacementPosition, Mesh.CreateRectangle(Vector2.Zero, 40f, 20f, GlobalGraphicsData.BackgroundColor));
            NewButton.ClickEvent += (Button Sender) => { LoadView(Views.Count - 1); };
            ViewChangeButtons.Add(NewButton);
            RenderElements.Add(NewButton);
        }

        //problem -> if view coutn changes
        public void LoadView(int ID)
        {
            CurrentView = Views[ID];
            CurrentView.ViewResize(Width, Height);
        }
    }
}
