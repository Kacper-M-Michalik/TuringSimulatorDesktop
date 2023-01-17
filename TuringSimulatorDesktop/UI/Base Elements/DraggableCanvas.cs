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

        public bool IsActive { get; set; } = true;

        public bool IsMarkedForDeletion { get; set; }

        public ActionGroup Group { get; private set; }
        Viewport Port;

        public bool DragBounded = false;
        public Vector2 ViewOffsetBoundsMin;
        public Vector2 ViewOffsetBoundsMax;
        public Vector2 ViewOffset;
        public bool Draggable;
        public float DragFactor = 0.2f;
        public bool DrawBounded = true;

        public List<IVisualElement> Elements;

        public DraggableCanvas()
        {
            Group = InputManager.CreateActionGroup();
            Group.PollableObjects.Add(this);
            Elements = new List<IVisualElement>();
            Port = new Viewport();

            Bounds = new Point(0, 0);
            Position = Vector2.Zero;
        }

        public bool IsMouseOver()
        {
            return (IsActive && InputManager.MouseData.X >= Position.X && InputManager.MouseData.X <= Position.X + bounds.X && InputManager.MouseData.Y >= Position.Y && InputManager.MouseData.Y <= Position.Y + bounds.Y);
        }

        public void PollInput(bool IsInActionGroupFrame)
        {
            if (IsInActionGroupFrame && Draggable && IsMouseOver() && InputManager.LeftMousePressed)
            {
                ViewOffset.X += (float)InputManager.MouseDeltaX * DragFactor;
                ViewOffset.Y += (float)InputManager.MouseDeltaY * DragFactor;

                if (DragBounded)
                {
                    ViewOffset.X = Math.Clamp(ViewOffset.X, ViewOffsetBoundsMin.X, ViewOffsetBoundsMax.X);
                    ViewOffset.Y = Math.Clamp(ViewOffset.Y, ViewOffsetBoundsMin.Y, ViewOffsetBoundsMax.Y);
                }

                UpdateLayout();
            }
        }

        public void UpdateLayout()
        {

        }

        public void MoveLayout()
        {
            Group.X = UIUtils.ConvertFloatToInt(position.X);
            Group.Y = UIUtils.ConvertFloatToInt(position.Y);
            Port.X = UIUtils.ConvertFloatToInt(Position.X);
            Port.Y = UIUtils.ConvertFloatToInt(Position.Y);
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
