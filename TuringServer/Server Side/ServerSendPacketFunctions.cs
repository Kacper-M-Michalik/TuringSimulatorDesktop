using System;
using System.Collections.Generic;
using TuringCore;
using TuringServer.Logging;
using System.Text.Json.Serialization;
using System.Text.Json;
using TuringCore.Networking;
using TuringServer.Data;

namespace TuringServer.ServerSide
{
    static class ServerSendPacketFunctions
    {
        #region Main
        //Generates a specific packet
        public static Packet LogData(string LogData)
        {
            Packet Data = new Packet();

            //Generating our contents/payload
            LogDataMessage Payload = new LogDataMessage();
            Payload.RequestType = (int)ServerSendPackets.LogData;
            Payload.LogMessage = LogData;

            //Writing our Header information
            Data.Write((int)ServerSendPackets.LogData);
            //WRiting our Payload
            Data.Write(JsonSerializer.SerializeToUtf8Bytes(Payload));

            return Data;
        }

        public static Packet ErrorNotification(string ErrorString)
        {
            Packet Data = new Packet();

            CustomLogging.Log("SERVER: Error Notification Copy - " + ErrorString);

            ErrorNotificationMessage Payload = new ErrorNotificationMessage();
            Payload.RequestType = (int)ServerSendPackets.ErrorNotification;
            Payload.ErrorMessage = ErrorString;

            Data.Write((int)ServerSendPackets.ErrorNotification);
            Data.Write(JsonSerializer.SerializeToUtf8Bytes(Payload));

            return Data;
        }

        public static Packet ProjectData()
        {
            Packet Data = new Packet();

            ProjectDataMessage Payload = new ProjectDataMessage();
            Payload.RequestType = (int)ServerSendPackets.SentProjectData;
            Payload.ProjectName = Server.LoadedProject.ProjectName;
            Payload.ProjectType = Server.LoadedProject.TuringTypeRule;

            Data.Write((int)ServerSendPackets.SentProjectData);
            Data.Write(JsonSerializer.SerializeToUtf8Bytes(Payload));

            return Data;
        }

        public static Packet FileData(Guid FileGUID)
        {
            Packet Data = new Packet();
            int FileID = Server.LoadedProject.GuidFileLookup[FileGUID];

            FileDataMessage Payload = new FileDataMessage();            
            Payload.RequestType = (int)ServerSendPackets.SentOrUpdatedFile;
            Payload.GUID = FileGUID;
            Payload.FileType = Server.LoadedProject.FileDataLookup[FileID].FileType;
            Payload.Name = Server.LoadedProject.FileDataLookup[FileID].Name;
            Payload.Version = Server.LoadedProject.FileDataLookup[FileID].Version;
            Payload.Data = Server.LoadedProject.CacheDataLookup[FileID].FileData;

            Data.Write((int)ServerSendPackets.SentOrUpdatedFile);
            Data.Write(JsonSerializer.SerializeToUtf8Bytes(Payload));

            return Data;
        }

        public static Packet FileMetadata(Guid FileGUID)
        {
            Packet Data = new Packet();
            int FileID = Server.LoadedProject.GuidFileLookup[FileGUID];

            FileDataMessage Payload = new FileDataMessage();
            Payload.RequestType = (int)ServerSendPackets.SentFileMetadata;
            Payload.GUID = FileGUID;
            Payload.FileType = Server.LoadedProject.FileDataLookup[FileID].FileType;
            Payload.Name = Server.LoadedProject.FileDataLookup[FileID].Name;
            Payload.Version = Server.LoadedProject.FileDataLookup[FileID].Version;

            Data.Write((int)ServerSendPackets.SentFileMetadata);
            Data.Write(JsonSerializer.SerializeToUtf8Bytes(Payload));

            return Data;
        }

        public static Packet FolderData(int FolderID)
        {
            Packet Data = new Packet();

            DirectoryFolder SendFolder = Server.LoadedProject.FolderDataLookup[FolderID];

            FolderDataMessage Payload = new FolderDataMessage();
            Payload.RequestType = (int)ServerSendPackets.SentFolderData;
            Payload.ID = FolderID;
            Payload.Name = SendFolder.Name;

            //Add a list of parent folders
            DirectoryFolder CurrentFolder = SendFolder;
            List<FolderDataMessage> ParentFolders = new List<FolderDataMessage>();
            while (CurrentFolder.ParentFolder != null)
            {
                ParentFolders.Add(new FolderDataMessage() { ID = CurrentFolder.ParentFolder.ID, Name = CurrentFolder.ParentFolder.Name });
                CurrentFolder = CurrentFolder.ParentFolder;
            }

            Payload.ParentFolders = ParentFolders;

            //Add our list of subfolders
            List<FolderDataMessage> SubFolders = new List<FolderDataMessage>();
            foreach (DirectoryFolder Folder in SendFolder.SubFolders)
            {
                SubFolders.Add(new FolderDataMessage() { ID = Folder.ID, Name = Folder.Name });
            }

            Payload.SubFolders = SubFolders;

            //Add our list of subfiles
            List<FileDataMessage> Files = new List<FileDataMessage>();
            foreach (DirectoryFile File in SendFolder.SubFiles)
            {
                Files.Add(new FileDataMessage() { GUID = File.GUID, FileType = File.FileType, Name = File.Name, Version = File.Version});
            }

            Payload.Files = Files;

            //Header
            Data.Write((int)ServerSendPackets.SentFolderData);
            //Payload
            Data.Write(JsonSerializer.SerializeToUtf8Bytes(Payload));

            return Data;
        }

        /*
        public static Packet FileRenamed(int FileID)
        {
            Packet Data = new Packet();

            Data.Write((int)ServerSendPackets.RenamedFile);
            ///

            return Data;
            //SendTCPToAllClients(Data);
        }

        public static Packet FileMoved(int FileID)
        {
            Packet Data = new Packet();

            Data.Write((int)ServerSendPackets.MovedFile);
            ///

            return Data;
            //SendTCPToAllClients(Data);
        }
        */

        public static Packet FileDeleted(Guid FileGUID)
        {
            Packet Data = new Packet();

            Data.Write((int)ServerSendPackets.DeletedFile);
            Data.Write(FileGUID);

            return Data;
        }

        /*
        public static Packet FileUnsubscribed(int FileID)
        {
            Packet Data = new Packet();

            Data.Write((int)ServerSendPackets.);
            Data.Write(FileID);

            //SendTCPToAllClients(Data);
            return Data;
        }
        */

        /*
        public static Packet FolderCreated(int FolderID)
        {
            Packet Data = new Packet();

            Data.Write((int)ServerSendPackets.CreatedFolder);
            Data.Write(FolderID);

            //SendTCPToAllClients(Data);
            return Data;
        }

        public static Packet FolderRenamed(int FolderID)
        {
            Packet Data = new Packet();

            Data.Write((int)ServerSendPackets.RenamedFolder);
            Data.Write(FolderID);

            //SendTCPToAllClients(Data);
            return Data;
        }

        public static Packet FolderMoved(int FolderID)
        {
            Packet Data = new Packet();

            Data.Write((int)ServerSendPackets.MovedFolder);
            Data.Write(FolderID);

            //SendTCPToAllClients(Data);
            return Data;
        }
        */

        public static Packet FolderDeleted(int FolderID)
        {
            Packet Data = new Packet();

            Data.Write((int)ServerSendPackets.DeletedFolder);
            Data.Write(FolderID);

            return Data;
        }
        #endregion
    }
}
