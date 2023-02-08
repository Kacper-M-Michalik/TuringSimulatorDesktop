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
    public class DraggableCanvas : IVisualElement, IPollable
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
                for (int i = 0; i < Elements.Count; i++)
                {
                    Elements[i].IsActive = value;
                }
            }        
        }

        public bool IsMarkedForDeletion { get; set; }

        public ActionGroup Group { get; private set; }
        Viewport Port;

        public bool Draggable = true;
        public float DragFactor = 1f;
        public float ZoomFactor = 0.1f;
        public bool DrawBounded = true;

        Vector3 Offset;
        float Zoom = 1f;
        public Matrix OffsetMatrix;
        public Matrix ZoomMatrix;
        public Matrix InverseMatrix;

        public Matrix PositionMatrix;

        public List<ICanvasInteractable> Elements;

        public DraggableCanvas()
        {
            Group = InputManager.CreateActionGroup();
            Group.PollableObjects.Add(this);


            Elements = new List<ICanvasInteractable>();
            Port = new Viewport();

            InverseMatrix = Matrix.Identity;
            OffsetMatrix = Matrix.Identity;
            ZoomMatrix = Matrix.Identity;

            IsActive = true;
            Bounds = new Point(0, 0);
            Position = Vector2.Zero;
        }

        public bool IsMouseOver()
        {
            return (IsActive && InputManager.MouseData.X >= Position.X && InputManager.MouseData.X <= Position.X + bounds.X && InputManager.MouseData.Y >= Position.Y && InputManager.MouseData.Y <= Position.Y + bounds.Y);
        }

        public void PollInput(bool IsInActionGroupFrame)
        {
            if (IsInActionGroupFrame && Draggable && IsMouseOver())
            {
                if (InputManager.MouseData.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    Offset.X += (float)InputManager.MouseDeltaX * DragFactor;
                    Offset.Y += (float)InputManager.MouseDeltaY * DragFactor;
                    OffsetMatrix = Matrix.CreateTranslation(Offset);
                }

                Zoom += (InputManager.ScrollWheelDelta/120)*ZoomFactor;
                ZoomMatrix = Matrix.CreateScale(Zoom);

                ApplyMatrices();
            }
        }

        public void MoveLayout()
        {
            Group.X = UIUtils.ConvertFloatToInt(position.X);
            Group.Y = UIUtils.ConvertFloatToInt(position.Y);
            Port.X = UIUtils.ConvertFloatToInt(Position.X);
            Port.Y = UIUtils.ConvertFloatToInt(Position.Y);

            PositionMatrix = Matrix.CreateTranslation(Position.X, Position.Y, 0);

            ApplyMatrices();
        }

        public void ApplyMatrices()
        {
            Matrix ResultMatrix = PositionMatrix * OffsetMatrix * ZoomMatrix;
            InverseMatrix = Matrix.Invert(ResultMatrix);

            foreach (ICanvasInteractable InteractableItem in Elements)
            {
                InteractableItem.SetProjectionMatrix(ResultMatrix, InverseMatrix);
            }
        }

        public void ResizeLayout()
        {
            Group.Width = bounds.X;
            Group.Height = bounds.Y;
            Port.Width = bounds.X;
            Port.Height = bounds.Y;
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                if (DrawBounded)
                {
                    if (BoundPort != null)
                    {
                        Port = UIUtils.CalculateOverlapPort(Port, BoundPort.Value);
                    }
                    for (int i = 0; i < Elements.Count; i++)
                    {
                        Elements[i].Draw(Port);
                    }
                }
                else
                {
                    for (int i = 0; i < Elements.Count; i++)
                    {
                        Elements[i].Draw(BoundPort);
                    }
                }
            }
        }

        public void Close()
        {
            Group.IsMarkedForDeletion = true;
            IsMarkedForDeletion = true;
        }
    }
}
