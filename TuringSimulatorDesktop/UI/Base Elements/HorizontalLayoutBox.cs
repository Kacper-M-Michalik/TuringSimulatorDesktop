using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.UI
{
    public class HorizontalLayoutBox : IVisualElement
    {
        public int Width, Height;

        List<IVisualElement> Elements;
        public float Spacing;
        public bool UniformAreas;
        public bool UniformAreaAutoSize;
        public float UniformAreaSize;

        Vector2 position;
        public Vector2 Position { get => position; set => position = value; }
        public Vector2 GetBounds { get => new Vector2(Width, Height); }

        public HorizontalLayoutBox(int SetWidth, int SetHeight)
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
                    PlacementPosition = new Vector2(PlacementPosition.X + Elements[i].GetBounds.X + Spacing, PlacementPosition.Y);
                }
            }
            else
            {
                if (UniformAreaAutoSize)
                {
                    float GreatestBound = 0;
                    for (int i = 0; i < Elements.Count; i++)
                    {                        
                        if (Elements[i].GetBounds.X > GreatestBound)
                        {
                            GreatestBound = Elements[i].GetBounds.X;
                        }
                    }

                    for (int i = 0; i < Elements.Count; i++)
                    {
                        Elements[i].Position = PlacementPosition;
                        PlacementPosition = new Vector2(PlacementPosition.X + GreatestBound, PlacementPosition.Y);
                    }
                }
                else
                {
                    for (int i = 0; i < Elements.Count; i++)
                    {
                        Elements[i].Position = PlacementPosition;
                        PlacementPosition = new Vector2(PlacementPosition.X + UniformAreaSize, PlacementPosition.Y);
                    }
                }
            }
        }

        public void Draw(Viewport BoundPort)
        {

        }
    }
}
