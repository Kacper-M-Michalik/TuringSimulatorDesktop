using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.UI
{
    public class DropdownButton : UIElement, IPoll, IClickable
    {
        public override int GetBoundX => throw new NotImplementedException();

        public override int GetBoundY => throw new NotImplementedException();

        public bool Clicked()
        {
            throw new NotImplementedException();
        }

        public void ClickedAway()
        {
            throw new NotImplementedException();
        }

        public bool IsMouseOver()
        {
            throw new NotImplementedException();
        }

        public void PollInput()
        {
            throw new NotImplementedException();
        }
    }
}
