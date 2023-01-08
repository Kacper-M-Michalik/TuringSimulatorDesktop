using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI.Prefabs
{
    public class Window : IVisualElement
    {
        Vector2 position;
        public Vector2 Position
        {
            get => position;
            set
            {
                position = value;
                Background.Position = position;
                if (CurrentView != null) CurrentView.Position = new Vector2(position.X + 1, position.Y + 25);
                
                int X = UIUtils.ConvertFloatToInt(position.X);
                int Y = UIUtils.ConvertFloatToInt(position.Y);

                HeaderPort.X = X;
                HeaderPort.Y = Y; 
                MainPort.X = X + 1;
                MainPort.Y = Y + 25;

                HeaderGroup.X = X;
                HeaderGroup.Y = Y;
                MainGroup.X = X + 1;
                MainGroup.Y = Y + 25;
            }
        }

        Point bounds;
        public Point Bounds
        {
            get => bounds;
            set
            {
                bounds = value;
                Background.Bounds = bounds;
                if (CurrentView != null) CurrentView.Bounds = new Point(bounds.X - 2, bounds.Y - 26);

                HeaderPort.Width = bounds.X;
                HeaderPort.Height = 24;
                MainPort.Width = bounds.X;
                MainPort.Height = bounds.Y - 26;

                HeaderGroup.Width = bounds.X;
                HeaderGroup.Height = 24;
                MainGroup.Width = bounds.X;
                MainGroup.Height = bounds.Y - 26;
            }
        }

        public bool IsActive = true;

        public IVisualElement CurrentView;
        public bool IsMouseOverDraggableArea;
        public bool IsMarkedForDeletion;
        Viewport HeaderPort;
        Viewport MainPort;
        ActionGroup HeaderGroup;
        ActionGroup MainGroup;

        Icon Background;

        public Window(Vector2 position, Point bounds)
        {
            HeaderPort = new Viewport();
            MainPort = new Viewport();

            HeaderGroup = InputManager.CreateActionGroup();
            MainGroup = InputManager.CreateActionGroup();

            Background = new Icon(GlobalRenderingData.DarkAccentColor);
            Position = position;
            Bounds = bounds;

            DebugManager.LastCreatedWindow = this;
        }

        public bool IsMouseOverHeader()
        {
            return (IsActive && InputManager.MouseData.X >= Position.X && InputManager.MouseData.X <= Position.X + bounds.X && InputManager.MouseData.Y >= Position.Y && InputManager.MouseData.Y <= Position.Y + 24);
        }

        public bool IsMouseOverWindow()
        {
            return (IsActive && InputManager.MouseData.X >= Position.X && InputManager.MouseData.X <= Position.X + bounds.X && InputManager.MouseData.Y >= Position.Y && InputManager.MouseData.Y <= Position.Y + 24);
        }

        public void SetView(IVisualElement View)
        {
            CurrentView = View;
            CurrentView.Position = new Vector2(position.X + 1, position.Y + 25);
            CurrentView.Bounds = new Point(bounds.X - 2, bounds.Y - 26);
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                Background.Draw();
                if (CurrentView != null) CurrentView.Draw(MainPort);
            }
        }
    }
}
