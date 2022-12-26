using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop
{
    public interface IPollable
    {
        public void PollInput(bool IsInActionGroupFrame);
    }
}
