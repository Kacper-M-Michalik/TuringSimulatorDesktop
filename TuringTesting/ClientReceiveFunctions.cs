using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringCore.Networking;

namespace NetworkTesting
{
    static class ClientReceiveFunctions
    {
        public delegate void PacketFunctionPointer(Packet Data);
        public static Dictionary<int, PacketFunctionPointer> PacketToFunction = new Dictionary<int, PacketFunctionPointer>()
        {

        };

        public static void ReceiveResponse(Packet Data)
        {
            Console.WriteLine();
            Console.WriteLine("CLIENT: RESPONSE FROM SERVER: " + Data.ReadString());
        }
    }
}
