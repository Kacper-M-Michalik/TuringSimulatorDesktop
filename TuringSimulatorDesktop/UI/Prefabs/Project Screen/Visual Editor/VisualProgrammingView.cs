using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.UI.Prefabs
{
    public class VisualProgrammingView : IView
    {
        Vector2 position;
        public Vector2 Position
        {
            get => position;
            set
            {
                position = value;
                MoveLayout();
            }
        }

        Point bounds;
        public Point Bounds
        {
            get => bounds;
            set
            {
                bounds = value;
                ResizeLayout();
            }
        }

        bool isActive;
        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
            }
        }

        Window ownerWindow;
        public Window OwnerWindow
        {
            get => ownerWindow;
            set => ownerWindow = value;
        }

        string title = "Empty Visual Programming View";
        public string Title => title;
        public int OpenFileID => CurrentlyOpenedFileID;

        int CurrentlyOpenedFileID;

        public bool IsMarkedForDeletion
        {
            get => false;
            set
            {
            }
        }

        public VisualProgrammingView(int FileToDisplay)
        {
            IsActive = false;

            CurrentlyOpenedFileID = FileToDisplay;

        }

        void MoveLayout()
        {
            

        }

        void ResizeLayout()
        {
            


        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {

            }
        }

        public void Close()
        {

        }

    }
}
