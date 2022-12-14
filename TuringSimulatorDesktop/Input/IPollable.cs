using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI
{
    public interface IPollable
    {
        public bool IsMarkedForDeletion { get; set; }
        public void PollInput(bool IsInActionGroupFrame);
    }
}
