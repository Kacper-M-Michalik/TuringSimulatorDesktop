using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI
{
    public class VerticalLayoutBox : IVisualElement, IPollable
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

        public bool IsActive { get; set; } = true;

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

        public VerticalLayoutBox()
        {
            Group = InputManager.CreateActionGroup();
            Group.PollableObjects.Add(this);
            Elements = new List<IVisualElement>();
            Port = new Viewport();

            Bounds = new Point(0, 0);
            Position = Vector2.Zero;
        }
        public VerticalLayoutBox(int width, int height)
        {
            Group = InputManager.CreateActionGroup();
            Group.PollableObjects.Add(this);
            Elements = new List<IVisualElement>();
            Port = new Viewport();

            Bounds = new Point(width, height);
            Position = Vector2.Zero;
        }
        public VerticalLayoutBox(int width, int height, Vector2 position)
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
                ViewOffset.Y += (float)InputManager.ScrollWheelDelta * ScrollFactor;
                ViewOffset.Y = Math.Clamp(ViewOffset.Y, Math.Clamp(-LayoutEndBound.Y + bounds.Y, float.MinValue, ViewOffsetBoundsMin.Y), ViewOffsetBoundsMin.Y);
                UpdateLayout();
            }
        }

        public void AddElement(IVisualElement Element)
        {
            Elements.Add(Element);
        }

        public void RemoveElement(IVisualElement Element)
        {
            Elements.Remove(Element);
            UpdateLayout();
        }

        public void Clear()
        {
            Group.IsDirtyClickable = true;
            foreach (IClickable Clickable in Group.ClickableObjects)
            {
                Clickable.IsMarkedForDeletion = true;
            }
            Group.IsDirtyPollable = true;
            foreach (IPollable Pollable in Group.PollableObjects)
            {
                if (Pollable != this) Pollable.IsMarkedForDeletion = true;
            }

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
                    if (Elements[i].IsActive)
                    {
                        Elements[i].Position = PlacementPosition;
                        PlacementPosition = new Vector2(PlacementPosition.X, PlacementPosition.Y + Elements[i].Bounds.Y + Spacing);
                    }
                }
            }
            else
            {
                if (UniformAreaAutoSize)
                {
                    float UniformAreaSize = 0;
                    for (int i = 0; i < Elements.Count; i++)
                    {
                        if (Elements[i].Bounds.Y > UniformAreaSize)
                        {
                            UniformAreaSize = Elements[i].Bounds.Y;
                        }
                    }
                }
               
                for (int i = 0; i < Elements.Count; i++)
                {
                    if (Elements[i].IsActive)
                    {
                        Elements[i].Position = PlacementPosition;
                        PlacementPosition = new Vector2(PlacementPosition.X, PlacementPosition.Y + UniformAreaSize + Spacing);
                    }
                }
                
            }

            LayoutEndBound = PlacementPosition - ViewOffset - Position - ViewOffsetBoundsMin - new Vector2(0, Spacing);
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
