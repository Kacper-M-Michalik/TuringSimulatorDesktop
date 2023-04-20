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
                Header.Position = position;
                ButtonLayout.Position = position;
                if (CurrentView != null) CurrentView.Position = new Vector2(position.X, position.Y + 26);
                
                int X = UIUtils.ConvertFloatToInt(position.X);
                int Y = UIUtils.ConvertFloatToInt(position.Y);

                MainPort.X = X;
                MainPort.Y = Y + 26;

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
                Header.Bounds = new Point(bounds.X, 26);
                ButtonLayout.Bounds = new Point(bounds.X , 26);
                if (CurrentView != null) CurrentView.Bounds = new Point(bounds.X, bounds.Y - 26);

                MainPort.Width = bounds.X;
                MainPort.Height = bounds.Y - 26;
            }
        }

        public bool IsActive { get; set; } = true;

        Icon Header;
        Icon HeaderStrip;
        Icon Background;
        HorizontalLayoutBox ButtonLayout;


        public IView CurrentView;
        WindowHeaderItem CurrentHeader;
        public bool IsMouseOverDraggableArea;
        public bool IsMarkedForDeletion;
        Viewport MainPort;
        public List<WindowHeaderItem> Headers = new List<WindowHeaderItem>();

        public ProjectScreenView OwnerScreen;

        public Window(Vector2 position, Point bounds, ProjectScreenView ownerScreen)
        {
            MainPort = new Viewport();
            OwnerScreen = ownerScreen;

            Background = new Icon(GlobalInterfaceData.Scheme.Background);
            Header = new Icon(GlobalInterfaceData.Scheme.Header);
            ButtonLayout = new HorizontalLayoutBox();
            ButtonLayout.Spacing = 0;
            ButtonLayout.ViewOffset = new Vector2(0, 0);
            ButtonLayout.Centering = HorizontalCentering.Bottom;

            Position = position;
            Bounds = bounds;
        }

        //The header is the top area you can drag a window by, and contains headings for what views are open in that window
        public bool IsMouseOverHeader()
        {
            return (IsActive && InputManager.MouseData.X >= Position.X && InputManager.MouseData.X <= Position.X + bounds.X && InputManager.MouseData.Y >= Position.Y && InputManager.MouseData.Y <= Position.Y + 24);
        }

        public bool IsMouseOverWindow()
        {
            return (IsActive && InputManager.MouseData.X >= Position.X && InputManager.MouseData.X <= Position.X + bounds.X && InputManager.MouseData.Y >= Position.Y && InputManager.MouseData.Y <= Position.Y + 24);
        }

        //Adds a view to the window
        public void AddView(IView View)
        {
            //Ensure view representing the same object doesnt already exist
            foreach (WindowHeaderItem Header in Headers)
            {
                if (View.OpenFileID != Guid.Empty && Header.View.OpenFileID == View.OpenFileID)
                {
                    //If it does we simply switch to that view
                    SetView(Header);
                    return;
                }
            }

            //Otherwise we generate a new header for our new view and add those to the list of headers + views
            WindowHeaderItem HB = new WindowHeaderItem(View, this, ButtonLayout.Group);
            View.OwnerWindow = this;
            Headers.Add(HB);
            ButtonLayout.AddElement(HB);
            ButtonLayout.UpdateLayout();
            SetView(HB);
        }

        //Set the view to that of a specific header
        public void SetView(WindowHeaderItem ViewButton)
        {
            //Deactivate previous view and header
            if (CurrentHeader != null) CurrentHeader.Deselect();
            CurrentHeader = ViewButton;
            CurrentHeader.Select();

            //Activate new view and header
            if (CurrentView != null) CurrentView.IsActive = false;
            CurrentView = ViewButton.View;
            CurrentView.IsActive = true;
            CurrentView.Bounds = new Point(bounds.X, bounds.Y - 26);
            CurrentView.Position = new Vector2(position.X, position.Y + 26);
        }

        //Deletes a view and header from the list
        public void RemoveView(WindowHeaderItem ViewButton)
        {
            //If the current view was closed, we must figure out which view to display next
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
                    //If no more views are left, close the window
                    CurrentView = null;
                    OwnerScreen.DeleteWindow(this);
                }
            }

            //Update UI
            ViewButton.Close();
            Headers.Remove(ViewButton);
            ButtonLayout.RemoveElement(ViewButton);
            ButtonLayout.UpdateLayout();
        }

        public void UpdateHeader()
        {
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

        //Shut down this view correctly
        public void Close()
        {            
            foreach (WindowHeaderItem CurrentHeader in Headers)
            {
                CurrentHeader.Close();
            }
            Headers.Clear();
            ButtonLayout.Clear();
            ButtonLayout.Close();
            IsActive = false;            
        }
    }
}
