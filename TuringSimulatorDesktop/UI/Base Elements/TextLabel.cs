using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.UI
{
    public class TextLabel : UIElement
    {
        public string Text;

        public TextLabel(string SetText = "")
        {
            Text = SetText;
        }

        public override int GetBoundX => throw new NotImplementedException();

        public override int GetBoundY => throw new NotImplementedException();
    }
}
