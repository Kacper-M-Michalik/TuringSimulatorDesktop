using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.UI
{
    public abstract class View
    {
        public abstract void Draw();
        public abstract void ViewResize(int NewWidth, int NewHeight);
    }
}
