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
        ProjectScreenView Parent;

        public int X { get { return Port.X; } }
        public int Y { get { return Port.Y; } }
        public int Width { get { return Port.Width; } }
        public int Height { get { return Port.Height; } }

        public Viewport Port;
        public View CurrentView;

        Mesh Border;
        Mesh Background;

        Button CloseButton;

        //DEBUG
        public int LastProjectionX;
        public int LastProjectionY;
        public int LastProjectionWidth;
        public int LastProjectionHeight;
        //

        public WindowView(int SetWidth, int SetHeight, ProjectScreenView SetParent)
        {
            Parent = SetParent;

            Port = new Viewport(0, 0, SetWidth, SetHeight);
            CloseButton = new Button(Vector2.Zero, Mesh.CreateRectangle(Vector2.Zero, 10, 10, GlobalGraphicsData.DebugColor), ElementCreateType.Persistent);
            CloseButton.ClickEvent += CloseWindow;

            UpdateElements();

            DebugManager.LastCreatedWindow = this;
        }

        public override void Draw()
        {
            GlobalMeshRenderer.Draw(Border, Port);
            GlobalMeshRenderer.Draw(Background, Port);
            GlobalMeshRenderer.Draw(CloseButton, Port);
            if (CurrentView != null) CurrentView.Draw();
        }

        public void CloseWindow(Button Sender)
        {
            Parent.WindowClosing(this);
        }

        public bool IsMouseOverWindow()
        {
            if (InputManager.MouseData.Y < (Y + Height) && InputManager.MouseData.Y > Y && InputManager.MouseData.X < (X + Width) && InputManager.MouseData.X > X) return true;
            return false;
        }

        public bool IsMouseOverTab()
        {
            if (InputManager.MouseData.Y < (Y + GlobalGraphicsData.WindowTabHeight) && InputManager.MouseData.Y > Y && InputManager.MouseData.X < (X + Width) && InputManager.MouseData.X > X) return true;
            return false;
        }

        public override void ViewResize(int SetWindowWidth, int SetWindowHeight)
        {            
            Port.Width = SetWindowWidth;
            Port.Height = SetWindowHeight;

            UpdateElements();

            LastProjectionX = X;
            LastProjectionY = Y;
            LastProjectionWidth = Width;
            LastProjectionHeight = Height;

            CurrentView?.ViewResize(Width, Height);
        }

        public override void ViewPositionSet(int X, int Y)
        {
            Port.X = X;
            Port.Y = Y;
            UpdateElements();
        }

        public void ViewPositionMove(int X, int Y)
        {
            Port.X += X;
            Port.Y += Y;
            UpdateElements();
        }
    
        public void UpdateElements()
        {
            Border = Mesh.CreateRectangle(new Vector2(X, Y), Width, Height, GlobalGraphicsData.DebugColor);
            Background = Mesh.CreateRectangle(new Vector2(X + 1, Y + 1), Width - 2, Height - 2, GlobalGraphicsData.BackgroundColor);
            CloseButton.Position = new Vector2(X + Width - 15, Y + 5);
            CurrentView?.ViewPositionSet(X, Y);
        }
    }

}
