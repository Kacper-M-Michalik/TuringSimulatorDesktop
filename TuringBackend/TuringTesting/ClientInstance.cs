using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using TuringCore;
using TuringServer;

namespace TuringTesting
{
    public static class ClientInstance
    { 
        public static void ConnectToServer(string Address, int Port)
        {
            if (IPAddress.TryParse(Address, out IPAddress IP))
            {
                Client.ConnectToServer(IP, Port);
            }
            else
            {
                CustomLogging.Log("Invalid IP Address");
            }
        }

        public static void ConnectToLocalServer(int Port)
        {
            ConnectToServer("127.0.0.1", Port);
        }

        public static void Disconnect()
        {
            Client.Disconnect();
        }
    }
}
