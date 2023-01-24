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
    static class ClientReceiveFunctions
    {
        public delegate void PacketFunctionPointer(Packet Data);
        public static Dictionary<int, PacketFunctionPointer> PacketToFunction = new Dictionary<int, PacketFunctionPointer>()
        {
            {(int)ServerSendPackets.SentProjectData, ReceivedProjectData},
            {(int)ServerSendPackets.ErrorNotification, ReceiveErrorNotification},
            {(int)ServerSendPackets.SentOrUpdatedFile, ReceivedFileFromServer},
            {(int)ServerSendPackets.SentFolderData, ReceivedFolderDataFromServer},
            {(int)ServerSendPackets.LogData, ReceiveLogData},
        };

        public static void ReceivedProjectData(Packet Data)
        {
            GlobalProjectAndUserData.ProjectData = new ConnectedProjectData(Data.ReadString());
            UIEventManager.RecievedProjectDataFromServerDelegate?.Invoke(null, null);// = true;
        }

        public static void ReceiveErrorNotification(Packet Data)
        {
            CustomLogging.Log("CLIENT: Received Error Notif: " + Data.ReadString());
        }

        public static void ReceiveLogData(Packet Data)
        {
            CustomLogging.Log("CLIENT: LOG DATA FROM SERVER: " + Data.ReadString());
        }

        public static void ReceivedFileFromServer(Packet Data)
        {
            CustomLogging.Log("CLIENT: Recieved FILE Data");

           // UIEventManager.PushToListeners(Data.ReadInt(false), Data);
            UIEventManager.PushToListeners(Data.ReadInt(false), Data);
        }

        public static void ReceivedFolderDataFromServer(Packet Data)
        {
            CustomLogging.Log("CLIENT: Recieved FOLDER Data");

            //UIEventManager.PushToListeners(Data.ReadInt(false), Data);
            UIEventManager.PushToListeners(Data.ReadInt(false), Data);
        }

    }
}
