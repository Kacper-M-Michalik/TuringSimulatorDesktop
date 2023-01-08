using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.UI
{
    public interface IVisualElement
    {
        public void Draw(Viewport? BoundPort = null);
        public Vector2 Position { get; set; }
        public Point Bounds { get; set; }
    }
}
