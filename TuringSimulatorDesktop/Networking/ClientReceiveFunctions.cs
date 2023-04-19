using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringCore;
using TuringServer;
using TuringSimulatorDesktop.UI;
using System.Text.Json;
using TuringCore.Networking;
using TuringSimulatorDesktop.Debugging;

namespace TuringSimulatorDesktop.Networking
{
    static class ClientReceiveFunctions
    {
        public delegate void PacketFunctionPointer(Packet Data);
        public static Dictionary<int, PacketFunctionPointer> PacketToFunction = new Dictionary<int, PacketFunctionPointer>()
        {
            {(int)ServerSendPackets.SentProjectData, ReceivedProjectData},
            {(int)ServerSendPackets.ErrorNotification, ReceiveErrorNotification},
            {(int)ServerSendPackets.SentOrUpdatedFile, ReceivedFileFromServer},
            {(int)ServerSendPackets.SentFileMetadata, ReceivedFileMetadataFromServer},
            {(int)ServerSendPackets.SentFolderData, ReceivedFolderDataFromServer},
            {(int)ServerSendPackets.LogData, ReceiveLogData},
        };

        public static void ReceivedProjectData(Packet Data)
        {
            ProjectDataMessage Message = JsonSerializer.Deserialize<ProjectDataMessage>(Data.ReadByteArray());

            GlobalProjectAndUserData.ProjectData = new ConnectedProjectData(Message.ProjectName);
            UIEventManager.RecievedProjectDataFromServerDelegate?.Invoke(null, null);// = true;
        }

        public static void ReceiveErrorNotification(Packet Data)
        {
            ErrorNotificationMessage Message = JsonSerializer.Deserialize<ErrorNotificationMessage>(Data.ReadByteArray());

            CustomLogging.Log("CLIENT: Received Error Notif: " + Message.ErrorMessage);
        }

        public static void ReceiveLogData(Packet Data)
        {
            LogDataMessage Message = JsonSerializer.Deserialize<LogDataMessage>(Data.ReadByteArray());

            CustomLogging.Log("CLIENT: LOG DATA FROM SERVER: " + Message.LogMessage);
        }

        public static void ReceivedFileFromServer(Packet Data)
        {
            CustomLogging.Log("CLIENT: Recieved FILE Data");

            FileDataMessage Message = JsonSerializer.Deserialize<FileDataMessage>(Data.ReadByteArray(false));

            //Guid ID = Data.ReadGuid(false);
            //Data.ReadPointerPosition -= 4;

            UIEventManager.PushFileToListeners(Message.GUID, Message);
        }

        //deprecate?
        public static void ReceivedFileMetadataFromServer(Packet Data)
        {
            CustomLogging.Log("CLIENT: Recieved METADATA");

            FileDataMessage Message = JsonSerializer.Deserialize<FileDataMessage>(Data.ReadByteArray(false));
           // Guid ID = Data.ReadGuid(false);
           // Data.ReadPointerPosition -= 4;

            // UIEventManager.PushToListeners(Data.ReadInt(false), Data);
            UIEventManager.PushFileToListeners(Message.GUID, Message);
        }

        public static void ReceivedFolderDataFromServer(Packet Data)
        {
            CustomLogging.Log("CLIENT: Recieved FOLDER Data");

            FolderDataMessage Message = JsonSerializer.Deserialize<FolderDataMessage>(Data.ReadByteArray(false));

            UIEventManager.PushFolderToListeners(Message.ID, Message);
        }

    }
}
