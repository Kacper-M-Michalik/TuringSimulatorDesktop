using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.UI
{
    public interface ITileable
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int GetBoundX { get; }
        public int GetBoundY { get; }
    }
}
