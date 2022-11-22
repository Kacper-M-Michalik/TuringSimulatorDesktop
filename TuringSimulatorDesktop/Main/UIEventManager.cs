using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringCore;
using TuringServer;
using TuringSimulatorDesktop.UI;

namespace TuringSimulatorDesktop
{
    public delegate void SubscriberDataCallback(Packet Data);

    public static class UIEventManager
    {
        public static Dictionary<int, List<SubscriberDataCallback>> FileUpdateSubscribers;

    }
}
