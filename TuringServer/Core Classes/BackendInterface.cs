using System;
using System.Collections.Generic;
using TuringCore;
using TuringServer.Logging;
using System.Threading;

namespace TuringServer
{
    public static class BackendInterface
    {
        public static ProjectData LoadedProject { get { return Server.LoadedProject; } }

        public static bool StartProjectServer(int SetMaxClients, int SetPort)
        {
            if (NetworkingUtils.PortInUse(SetPort))
            {
                CustomLogging.Log("SERVER: Can't start, port already in use!");
                return false;
            }

            Thread ServerThread = new Thread(() => Server.StartServer(SetMaxClients, SetPort));
            ServerThread.Start();

            return true;
        }

        public static void CloseProject()
        {
            Server.CloseServer();
        }
    }
}
