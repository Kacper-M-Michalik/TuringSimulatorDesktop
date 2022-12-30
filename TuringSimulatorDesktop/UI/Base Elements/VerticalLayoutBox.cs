using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.UI
{
    public class VerticalLayoutBox : IVisualElement
    {
        public int Width, Height;
        public bool IsActive = true;

        List<IVisualElement> Elements;
        public float Spacing;
        public bool UniformAreas;
        public bool UniformAreaAutoSize;
        public float UniformAreaSize;

        public bool DrawBounded = true;

        Vector2 position;
        public Vector2 Position { get => position; set => position = value; }
        public Vector2 GetBounds { get => new Vector2(Width, Height); }

        public VerticalLayoutBox(int SetWidth, int SetHeight)
        {
            Elements = new List<IVisualElement>();
        }

        public void UpdateLayout()
        {
            Vector2 PlacementPosition = Position;

            if (!UniformAreas)
            {
                for (int i = 0; i < Elements.Count; i++)
                {
                    Elements[i].Position = PlacementPosition;
                    PlacementPosition = new Vector2(PlacementPosition.X, PlacementPosition.Y + Elements[i].GetBounds.Y + Spacing);
                }
            }
            else
            {
                if (UniformAreaAutoSize)
                {
                    float GreatestBound = 0;
                    for (int i = 0; i < Elements.Count; i++)
                    {
                        if (Elements[i].GetBounds.Y > GreatestBound)
                        {
                            GreatestBound = Elements[i].GetBounds.Y;
                        }
                    }

                    for (int i = 0; i < Elements.Count; i++)
                    {
                        Elements[i].Position = PlacementPosition;
                        PlacementPosition = new Vector2(PlacementPosition.X, PlacementPosition.Y + GreatestBound + Spacing);
                    }
                }
                else
                {
                    for (int i = 0; i < Elements.Count; i++)
                    {
                        Elements[i].Position = PlacementPosition;
                        PlacementPosition = new Vector2(PlacementPosition.X, PlacementPosition.Y + UniformAreaSize + Spacing);
                    }
                }
            }
        }

        public void Draw(Viewport BoundPort = default)
        {
            if (IsActive)
            {
                if (DrawBounded)
                {
                    Viewport Port = new Viewport(UIUtils.ConvertFloatToInt(Position.X), UIUtils.ConvertFloatToInt(Position.Y), Width, Height);
                    if (!UIUtils.IsDefaultViewport(BoundPort))
                    {
                        Port = UIUtils.CalculateOverlapPort(Port, BoundPort);
                    }

                    for (int i = 0; i < Elements.Count; i++)
                    {
                        Elements[i].Draw(Port);
                    }
                }
                else
                {
                    if (UIUtils.IsDefaultViewport(BoundPort))
                    {
                        for (int i = 0; i < Elements.Count; i++)
                        {
                            Elements[i].Draw();
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
        }
    }
}
