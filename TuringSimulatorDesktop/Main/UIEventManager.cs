using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringBackend;
using TuringSimulatorDesktop.UI;

namespace TuringSimulatorDesktop.Main
{
    public delegate void SubscriberDataCallback(Packet Data);

    public static class UIEventManager
    {
        public static Dictionary<int, List<SubscriberDataCallback>> FileUpdateSubscribers;

    }
}
