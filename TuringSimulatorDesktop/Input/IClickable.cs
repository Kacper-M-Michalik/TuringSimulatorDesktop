using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI
{
    public interface IClickable
    {
        public bool IsMarkedForDeletion { get; set; }
        public void Clicked();
        public void ClickedAway();
        public bool IsMouseOver();
    }
}
