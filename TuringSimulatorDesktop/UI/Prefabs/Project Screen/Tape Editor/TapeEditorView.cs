using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.UI.Prefabs
{
    public class TapeEditorView : IView
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

        string title = "Empty Tape Editor View";
        public string Title => title;
        public int OpenFileID => CurrentlyOpenedFileID;

        int CurrentlyOpenedFileID;

        DraggableCanvas Canvas;
        TapeVisualItem Tape;

        public bool IsMarkedForDeletion
        {
            get => false;
            set
            {
            }
        }

        public TapeEditorView(int FileToDisplay)
        {
            Canvas = new DraggableCanvas();
            Icon Icon1 = new Icon(Color.Aquamarine);
            Icon1.Bounds = new Point(10, 10);
            Icon1.Position = new Vector2(20, 30);
            Icon Icon2 = new Icon(Color.YellowGreen);
            Icon2.Bounds = new Point(100, 10);
            Icon2.Position = new Vector2(0, 50);
            Icon Icon3 = new Icon(Color.Red);
            Icon3.Bounds = new Point(5, 35);
            Icon3.Position = new Vector2(-10, -20);

            Tape = new TapeVisualItem();

            InputBox TestBox = new InputBox(Canvas.Group);
            TestBox.Bounds = new Point(100, 100);
            TestBox.Position = Vector2.Zero;

            Canvas.Elements.Add(Icon1);
            Canvas.Elements.Add(Icon2);
            Canvas.Elements.Add(Icon3);
            Canvas.Elements.Add(Tape);
            Canvas.Elements.Add(TestBox);


            IsActive = false;


            CurrentlyOpenedFileID = FileToDisplay;
            //call switch here
        }

        void MoveLayout()
        {
            Canvas.Position = position;

            Tape.Position = Position + new Vector2(100, 100);

        }

        void ResizeLayout()
        {
            Canvas.Bounds = bounds;
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                Tape.CameraMin = (Matrix.CreateTranslation(Position.X, 0, 0) * Canvas.InverseMatrix).Translation.X;
                Tape.CameraMax = (Matrix.CreateTranslation(Position.X + Bounds.X, 0, 0) * Canvas.InverseMatrix).Translation.X;
                Tape.UpdateLayout();

                Canvas.Draw();
            }
        }

        public void Close()
        {

        }

    }
}
