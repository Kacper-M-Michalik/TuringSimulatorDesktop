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
        //bool indicates if PassThrough Allowed -> True == Click is allowed to pass through, False == Click is terminated
        //Passthrough enabled object are not eligble for Click away
        public bool Clicked();
        public void ClickedAway();
        public bool IsMouseOver();
    }
}
