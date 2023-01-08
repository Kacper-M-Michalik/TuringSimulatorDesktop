using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TuringCore;
using TuringServer;

namespace TuringTesting
{
    public delegate void ReceivedDataCallback(Packet Data);

    public static class UIEventManager
    {
        public static bool ClientSuccessConnecting;
        public static EventHandler ClientSuccessConnectingDelegate;
        public static bool ClientFailedConnecting;
        public static EventHandler ClientFailedConnectingDelegate;
        public static bool RecievedErrorNotification;

        public static EventHandler UpdateFileBrowser;

        public static Dictionary<int, List<ReceivedDataCallback>> DataSubscribers = new Dictionary<int, List<ReceivedDataCallback>>();
    }
}
