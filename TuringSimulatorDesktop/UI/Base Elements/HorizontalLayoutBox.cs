﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI
{
    public enum HorizontalCentering
    {
        Top, 
        Middle, 
        Bottom
    }

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
                Port.X = UIUtils.ConvertFloatToInt(Position.X);
                Port.Y = UIUtils.ConvertFloatToInt(Position.Y);
                UpdateLayout();
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

        public HorizontalCentering Centering = HorizontalCentering.Top;
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
                ViewOffset.X = Math.Clamp(ViewOffset.X, Math.Clamp(-LayoutEndBound.X + bounds.X - Spacing, float.MinValue, ViewOffsetBoundsMin.X), ViewOffsetBoundsMin.X);
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
            //add removal from group later
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
                Pollable.IsMarkedForDeletion = true;
            }

            Elements.Clear();
            UpdateLayout();
        }

        public void UpdateLayout()
        {
            //Calculate start placement position
            Vector2 PlacementPosition = Position + ViewOffset;

            if (!UniformAreas)
            {
                //Each element placed with equal width away from eachother, no matter their width 
                for (int i = 0; i < Elements.Count; i++)
                {
                    if (Elements[i].IsActive)
                    {                        
                        float Y = PlacementPosition.Y;
                        if (Centering == HorizontalCentering.Middle)
                        {
                            if (Elements[i] is Label)
                            {
                                Y += bounds.Y / 2f;
                            }
                            else
                            {
                                Y += bounds.Y / 2f - Elements[i].Bounds.Y / 2f;
                            }
                        }
                        else if (Centering == HorizontalCentering.Bottom)
                        {
                            Y += bounds.Y - Elements[i].Bounds.Y;
                        }

                        Elements[i].Position = new Vector2(PlacementPosition.X, Y);
                        PlacementPosition = new Vector2(PlacementPosition.X + Elements[i].Bounds.X + Spacing, PlacementPosition.Y);
                    }
                }
            }
            else
            {
                //Widest element found, each element is given the space of this largest width plus padding, as such no matter size elements will be centered in set positions
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

                //Same as above but with manual width for each element area
                for (int i = 0; i < Elements.Count; i++)
                {
                    if (Elements[i].IsActive)
                    {
                        float Y = PlacementPosition.Y;
                        if (Centering == HorizontalCentering.Middle)
                        {
                            if (Elements[i] is Label)
                            {
                                Y += bounds.Y / 2f;
                            }
                            else
                            {
                                Y += bounds.Y / 2f - Elements[i].Bounds.Y / 2f;
                            }
                        }
                        else if (Centering == HorizontalCentering.Bottom)
                        {
                            Y += bounds.Y - Elements[i].Bounds.Y;
                        }

                        Elements[i].Position = new Vector2(PlacementPosition.X, Y);
                        PlacementPosition = new Vector2(PlacementPosition.X + UniformAreaSize + Spacing, Y);
                    }
                }

            }

            LayoutEndBound = PlacementPosition - ViewOffset - Position - ViewOffsetBoundsMin - new Vector2(Spacing, 0);
        }


        //Draw all elements
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
