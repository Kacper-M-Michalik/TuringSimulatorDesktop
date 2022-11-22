using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringCore;
using TuringServer;

namespace TuringSimulatorDesktop
{
    static class ClientReceiveFunctions
    {
        public delegate void PacketFunctionPointer(Packet Data);
        public static Dictionary<int, PacketFunctionPointer> PacketToFunction = new Dictionary<int, PacketFunctionPointer>()
        {
            {(int)ServerSendPackets.ErrorNotification, ReceiveErrorNotification},
            {(int)ServerSendPackets.SentOrUpdatedFile, ReceivedFileFromServer},
            {(int)ServerSendPackets.SentFolderData, ReceivedFolderDataFromServer},
        };


        public static void ReceiveErrorNotification(Packet Data)
        {

        }

        public static void ReceivedFileFromServer(Packet Data)
        {

        }

        public static void ReceivedFolderDataFromServer(Packet Data)
        {
            
        }

    }
}
