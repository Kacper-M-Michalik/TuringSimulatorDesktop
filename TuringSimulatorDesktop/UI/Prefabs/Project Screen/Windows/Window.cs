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
                Header.Position = new Vector2(position.X + 1, position.Y + 1);
                ButtonLayout.Position = new Vector2(position.X + 1, position.Y + 1);
                if (CurrentView != null) CurrentView.Position = new Vector2(position.X + 1, position.Y + 25);
                
                int X = UIUtils.ConvertFloatToInt(position.X);
                int Y = UIUtils.ConvertFloatToInt(position.Y);

                MainPort.X = X + 1;
                MainPort.Y = Y + 25;

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
                Header.Bounds = new Point(bounds.X - 2, 24);
                ButtonLayout.Bounds = new Point(bounds.X - 2, 24);
                if (CurrentView != null) CurrentView.Bounds = new Point(bounds.X - 2, bounds.Y - 26);

                MainPort.Width = bounds.X;
                MainPort.Height = bounds.Y - 26;
            }
        }

        public bool IsActive = true;

        Icon Header;
        Icon Background;
        HorizontalLayoutBox ButtonLayout;


        IView CurrentView;
        public bool IsMouseOverDraggableArea;
        public bool IsMarkedForDeletion;
        Viewport MainPort;
        List<WindowHeaderItem> Headers = new List<WindowHeaderItem>();

        public Window(Vector2 position, Point bounds)
        {
            MainPort = new Viewport();

            Background = new Icon(GlobalRenderingData.DarkAccentColor);
            Header = new Icon(GlobalRenderingData.HeaderColor);
            ButtonLayout = new HorizontalLayoutBox();
            ButtonLayout.Spacing = 2;

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

        public void AddView(IView View)
        {
            WindowHeaderItem HB = new WindowHeaderItem(View, this, ButtonLayout.Group);
            View.OwnerWindow = this;
            Headers.Add(HB);
            ButtonLayout.AddElement(HB);
            ButtonLayout.UpdateLayout();
            SetView(HB);
        }

        public void SetView(WindowHeaderItem ViewButton)
        {
            if (CurrentView != null) CurrentView.IsActive = false;
            CurrentView = ViewButton.View;
            CurrentView.IsActive = true;
            CurrentView.Position = new Vector2(position.X + 1, position.Y + 25);
            CurrentView.Bounds = new Point(bounds.X - 2, bounds.Y - 26);
        }

        public void RemoveView(WindowHeaderItem ViewButton)
        {
            if (ViewButton.View == CurrentView)
            {
                int Index = Headers.IndexOf(ViewButton);
                if (Index < Headers.Count - 1)
                {
                    SetView(Headers[Index + 1]);
                }
                else if (Index > 0)
                {
                    SetView(Headers[Index - 1]);
                }
                else
                {
                    CurrentView = null;
                }
            }

            ViewButton.Close();
            Headers.Remove(ViewButton);
            ButtonLayout.RemoveElement(ViewButton);
            ButtonLayout.UpdateLayout();
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                Background.Draw();
                Header.Draw();
                ButtonLayout.Draw();

                if (CurrentView != null) CurrentView.Draw(MainPort);
            }
        }

        public void Close()
        {
            throw new NotImplementedException();
        }
    }
}
