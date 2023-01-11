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
    public class HorizontalLayoutBox : IVisualElement, IPollable
    {
        Vector2 position;
        public Vector2 Position
        {
            get => position;
            set
            {
                position = value;
                Group.X = UIUtils.ConvertFloatToInt(position.X);
                Group.Y = UIUtils.ConvertFloatToInt(position.Y);
                UpdateLayout();
                Port.X = UIUtils.ConvertFloatToInt(Position.X);
                Port.Y = UIUtils.ConvertFloatToInt(Position.Y);
            }
        }

        Point bounds;
        public Point Bounds
        {
            get => bounds;
            set
            {
                bounds = value;
                Group.Width = bounds.X;
                Group.Height = bounds.Y;
                Port.Width = bounds.X;
                Port.Height = bounds.Y;
            }
        }

        public bool IsActive = true;

        Viewport Port;

        Vector2 LayoutEndBound;
        public float Spacing;
        public bool UniformAreas;
        public bool UniformAreaAutoSize;
        public float UniformAreaSize;
        public Vector2 ViewOffsetBoundsMin;
        public Vector2 ViewOffset;
        public bool Scrollable;
        public float ScrollFactor = 0.2f;
        public bool DrawBounded = true;

        public ActionGroup Group { get; private set; }
        public bool IsMarkedForDeletion { get; set; }
        List<IVisualElement> Elements;

        public HorizontalLayoutBox()
        {
            Group = InputManager.CreateActionGroup();
            Group.PollableObjects.Add(this);
            Elements = new List<IVisualElement>();
            Port = new Viewport();

            Bounds = new Point(0, 0);
            Position = Vector2.Zero;
        }
        public HorizontalLayoutBox(int width, int height)
        {
            Group = InputManager.CreateActionGroup();
            Group.PollableObjects.Add(this);
            Elements = new List<IVisualElement>();
            Port = new Viewport();

            Bounds = new Point(width, height);
            Position = Vector2.Zero;
        }
        public HorizontalLayoutBox(int width, int height, Vector2 position)
        {
            Group = InputManager.CreateActionGroup();
            Group.PollableObjects.Add(this);
            Elements = new List<IVisualElement>();
            Port = new Viewport();

            Bounds = new Point(width, height);
            Position = position;
        }

        public bool IsMouseOver()
        {
            return (IsActive && InputManager.MouseData.X >= Position.X && InputManager.MouseData.X <= Position.X + bounds.X && InputManager.MouseData.Y >= Position.Y && InputManager.MouseData.Y <= Position.Y + bounds.Y);
        }

        public void PollInput(bool IsInActionGroupFrame)
        {
            if (IsInActionGroupFrame && Scrollable && IsMouseOver() && InputManager.ScrollWheelDelta != 0)
            {
                ViewOffset.X += (float)InputManager.ScrollWheelDelta * ScrollFactor;
                ViewOffset.X = Math.Clamp(ViewOffset.X, Math.Clamp(-LayoutEndBound.X + bounds.X, float.MinValue, ViewOffsetBoundsMin.X), ViewOffsetBoundsMin.X);
                UpdateLayout();
            }
        }

        public void AddElement(IVisualElement Element)
        {
            //change element group here REMEBER
            Elements.Add(Element);
        }

        public void RemoveElement(IVisualElement Element)
        {
            Elements.Remove(Element);
            UpdateLayout();
        }

        public void Clear()
        {
            Elements.Clear();
            UpdateLayout();
        }

        public void UpdateLayout()
        {
            Vector2 PlacementPosition = Position + ViewOffset;

            if (!UniformAreas)
            {
                for (int i = 0; i < Elements.Count; i++)
                {
                    Elements[i].Position = PlacementPosition;
                    PlacementPosition = new Vector2(PlacementPosition.X + Elements[i].Bounds.X + Spacing, PlacementPosition.Y);
                }
            }
            else
            {
                if (UniformAreaAutoSize)
                {
                    float UniformAreaSize = 0;
                    for (int i = 0; i < Elements.Count; i++)
                    {
                        if (Elements[i].Bounds.X > UniformAreaSize)
                        {
                            UniformAreaSize = Elements[i].Bounds.X;
                        }
                    }
                }

                for (int i = 0; i < Elements.Count; i++)
                {
                    Elements[i].Position = PlacementPosition;
                    PlacementPosition = new Vector2(PlacementPosition.X + UniformAreaSize + Spacing, PlacementPosition.Y);
                }

            }

            LayoutEndBound = PlacementPosition - ViewOffset - Position - ViewOffsetBoundsMin - new Vector2(Spacing, 0);
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
