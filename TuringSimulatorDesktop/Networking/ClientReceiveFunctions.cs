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
        //We use response type ID as a lookup to the appropriate function pointer to execute -> Allows cleaner code
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

            //Create new project data object
            GlobalProjectAndUserData.ProjectData = new ConnectedProjectData(Message.ProjectName);
            //Fire received project data event
            UIEventManager.RecievedProjectDataFromServerDelegate?.Invoke(null, null);// = true;
        }

        public static void ReceiveErrorNotification(Packet Data)
        {
            ErrorNotificationMessage Message = JsonSerializer.Deserialize<ErrorNotificationMessage>(Data.ReadByteArray());

            CustomLogging.Log("CLIENT: Received Error Notification: " + Message.ErrorMessage);
        }

        public static void ReceiveLogData(Packet Data)
        {
            LogDataMessage Message = JsonSerializer.Deserialize<LogDataMessage>(Data.ReadByteArray());

            CustomLogging.Log("CLIENT: LOG DATA FROM SERVER: " + Message.LogMessage);
        }

        public static void ReceivedFileFromServer(Packet Data)
        {
            CustomLogging.Log("CLIENT: Received FILE Data");

            FileDataMessage Message = JsonSerializer.Deserialize<FileDataMessage>(Data.ReadByteArray(false));

            //Push payload to UI elements waiting for response
            UIEventManager.PushFileToListeners(Message.GUID, Message);
        }

        //deprecate?
        public static void ReceivedFileMetadataFromServer(Packet Data)
        {
            CustomLogging.Log("CLIENT: Received METADATA");
            
            //Push payload to UI elements waiting for response
            FileDataMessage Message = JsonSerializer.Deserialize<FileDataMessage>(Data.ReadByteArray(false));

            //Push payload to UI elements waiting for response
            UIEventManager.PushFileToListeners(Message.GUID, Message);
        }

        public static void ReceivedFolderDataFromServer(Packet Data)
        {
            CustomLogging.Log("CLIENT: Received FOLDER Data");

            FolderDataMessage Message = JsonSerializer.Deserialize<FolderDataMessage>(Data.ReadByteArray(false));

            //Push payload to UI elements waiting for response
            UIEventManager.PushFolderToListeners(Message.ID, Message);
        }

    }
}
