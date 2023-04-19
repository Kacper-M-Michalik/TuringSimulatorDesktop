using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.Input
{
    public interface IDragListener : IClickable
    {
        public void RecieveDragData();
    }
}
