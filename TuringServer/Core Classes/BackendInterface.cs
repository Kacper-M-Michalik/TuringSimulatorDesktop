﻿using System;
using System.Collections.Generic;
using TuringCore;
using TuringServer.Logging;
using System.Threading;

namespace TuringServer
{
    public static class BackendInterface
    {
        public static ProjectData LoadedProject { get { return Server.LoadedProject; } }

        public static void StartProjectServer(int SetMaxClients, int SetPort)
        {
            Thread ServerThread = new Thread(() => Server.StartServer(SetMaxClients, SetPort));
            ServerThread.Start();
        }

        public static void CloseProject()
        {
            Server.CloseServer();
        }
    }
}