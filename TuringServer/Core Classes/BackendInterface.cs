using TuringServer.Logging;
using TuringServer.Data;
using TuringServer.ServerSide;

namespace TuringServer
{
    //Provides an API for front ends to use this library
    public static class BackendInterface
    {
        //Reference to currently loaded project by server
        public static ProjectData LoadedProject { get { return Server.LoadedProject; } }

        //Start Server
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

        //CLose Server
        public static void CloseProject()
        {
            Server.CloseServer();
        }
    }
}
