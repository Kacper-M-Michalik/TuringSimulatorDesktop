using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringCore;

namespace TuringTesting
{
    static class ClientReceiveFunctions
    {
        public delegate void PacketFunctionPointer(Packet Data);
        public static Dictionary<int, PacketFunctionPointer> PacketToFunction = new Dictionary<int, PacketFunctionPointer>()
        {
            {(int)ServerSendPackets.ErrorNotification, ReceiveErrorNotification},
            {(int)ServerSendPackets.LogData, ReceiveLogData},
            {(int)ServerSendPackets.SentOrUpdatedFile, ReceivedFileFromServer},
            {(int)ServerSendPackets.SentFolderData, ReceivedFolderDataFromServer},
        };


        public static void ReceiveErrorNotification(Packet Data)
        {
            CustomLogging.Log("ERROR NOTIF FROM SERVER: " + Data.ReadString());
        }

        public static void ReceiveLogData(Packet Data)
        {
            CustomLogging.Log("LOG DATA FROM SERVER: " + Data.ReadString());
        }

        public static void ReceivedFileFromServer(Packet Data)
        {
            CustomLogging.Log("CLIENT: Recieved File");
        }

        public static void ReceivedFolderDataFromServer(Packet Data)
        {
            CustomLogging.Log("CLIENT: Recieved Folder Data");

            int FolderID = Data.ReadInt();
            string FolderName = Data.ReadString();
            int Folders = Data.ReadInt();
            for (int i = 0; i < Folders; i++)
            {
                CustomLogging.Log("FOLDER");
                CustomLogging.Log(Data.ReadString());
                CustomLogging.Log(Data.ReadInt().ToString());
            }
            int Files = Data.ReadInt();
            for (int i = 0; i < Files; i++)
            {
                CustomLogging.Log("FILE");
                CustomLogging.Log(Data.ReadString());
                CustomLogging.Log(Data.ReadInt().ToString());
            }
        }

    }
}
