using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringCore;

namespace TuringTesting
{
    public static class ClientSendFunctionsWrapper
    {
        public static void SendTCPData(Packet Data)
        {
            Client.TCP.SendDataToServer(Data);
            Data.Dispose();
        }
    }
}
