using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.UI;

namespace TuringSimulatorDesktop.UI
{
    //mayeb change to view or ui element?
    public class TilerElement
    {
        public int BaseX, BaseY;
        public List<ITileable> Elements;
        public TilePriority PriorityDirection = TilePriority.Vertical;

        public int Spacing;
        public bool UniformAreas;

        public TilerElement()
        {
            Elements = new List<ITileable>();
        }

        public void Tile()
        {
            int NextX = BaseX;
            int NextY = BaseY;
            if (PriorityDirection == TilePriority.Vertical)
            {
                if (!UniformAreas)
                {
                    for (int i = 0; i < Elements.Count; i++)
                    {
                        Elements[i].X = NextX;
                        Elements[i].Y = NextY;
                        NextY += Elements[i].GetBoundY + Spacing;                        
                    }
                }
                else
                {
                    int GreatestBound = 0;
                    for (int i = 0; i < Elements.Count; i++)
                    {
                        int BoundY = Elements[i].GetBoundY;
                        if (BoundY > GreatestBound)
                        {
                            GreatestBound = BoundY;
                        }
                    }

                    for (int i = 0; i < Elements.Count; i++)
                    {
                        Elements[i].X = NextX;
                        Elements[i].Y = NextY;
                        NextY += GreatestBound + Spacing;
                    }

                }
            }
            else 
            {
                if (!UniformAreas)
                {
                    for (int i = 0; i < Elements.Count; i++)
                    {
                        Elements[i].X = NextX;
                        Elements[i].Y = NextY;
                        NextX += Elements[i].GetBoundX + Spacing;
                    }
                }
                else
                {
                    int GreatestBound = 0;
                    for (int i = 0; i < Elements.Count; i++)
                    {
                        int BoundX = Elements[i].GetBoundX;
                        if (BoundX > GreatestBound)
                        {
                            GreatestBound = BoundX;
                        }
                    }

                    for (int i = 0; i < Elements.Count; i++)
                    {
                        Elements[i].X = NextX;
                        Elements[i].Y = NextY;
                        NextX += GreatestBound + Spacing;
                    }

                }
            }
        }

    }

    public enum TilePriority { Vertical, Horizontal }
}
