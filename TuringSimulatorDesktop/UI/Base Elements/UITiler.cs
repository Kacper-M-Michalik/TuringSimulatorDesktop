using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.UI
{
    public class TilerElement
    {
        public List<UIElement> Elements;
        public TilePriority PriorityDirection = TilePriority.Vertical;
        public bool CanTileTwoDimensions;

        public void Tile()
        {
            Elements = new List<UIElement>();
        }
        

    }

    public enum TilePriority { Vertical, Horizontal }
}
