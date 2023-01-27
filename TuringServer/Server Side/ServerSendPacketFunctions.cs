using System;
using System.Collections.Generic;
using TuringCore;
using TuringServer.Logging;

namespace TuringServer
{
    static class ServerSendPacketFunctions
    {
        #region Main
        public static Packet LogData(string LogData)
        {
            Packet Data = new Packet();

            Data.Write((int)ServerSendPackets.LogData);
            Data.Write(LogData);

            return Data;
        }

        public static Packet ErrorNotification(string ErrorString)
        {
            Packet Data = new Packet();

            CustomLogging.Log("SERVER: Error Notif Copy - " + ErrorString);

            Data.Write((int)ServerSendPackets.ErrorNotification);
            Data.Write(ErrorString);

            return Data;
        }

        public static Packet ProjectData()
        {
            Packet Data = new Packet();

            Data.Write((int)ServerSendPackets.SentProjectData);
            Data.Write(Server.LoadedProject.ProjectName);
            //add lookup for alphabets here

            return Data;
        }

        public static Packet FileData(Guid FileGUID)
        {
            Packet Data = new Packet();

            Data.Write((int)ServerSendPackets.SentOrUpdatedFile);

            int FileID = Server.LoadedProject.GuidFileLookup[FileGUID];

            Data.Write(FileGUID);
            Data.Write((int)Server.LoadedProject.FileDataLookup[FileID].FileType);
            Data.Write(Server.LoadedProject.FileDataLookup[FileID].Name);
            Data.Write(Server.LoadedProject.FileDataLookup[FileID].Version);
            Data.Write(Server.LoadedProject.CacheDataLookup[FileID].FileData);

            return Data;
        }

        public static Packet FileMetadata(Guid FileGUID)
        {
            Packet Data = new Packet();

            Data.Write((int)ServerSendPackets.SentFileMetadata);

            int FileID = Server.LoadedProject.GuidFileLookup[FileGUID];

            Data.Write(FileGUID);
            Data.Write((int)Server.LoadedProject.FileDataLookup[FileID].FileType);
            Data.Write(Server.LoadedProject.FileDataLookup[FileID].Name);
            Data.Write(Server.LoadedProject.FileDataLookup[FileID].Version);

            return Data;
        }

        public static Packet FolderData(int FolderID)
        {
            Packet Data = new Packet();

            Data.Write((int)ServerSendPackets.SentFolderData);

            DirectoryFolder SendFolder = Server.LoadedProject.FolderDataLookup[FolderID];

            Data.Write(FolderID);
            Data.Write(SendFolder.Name);

            DirectoryFolder CurrentFolder = SendFolder;
            List<DirectoryFolder> ParentFolders = new List<DirectoryFolder>();
            int NumberOfParentFolders = 0;
            while (CurrentFolder.ParentFolder != null)
            {
                ParentFolders.Add(CurrentFolder.ParentFolder);
                CurrentFolder = CurrentFolder.ParentFolder;
                NumberOfParentFolders++;
            }

            Data.Write(NumberOfParentFolders);
            
            for (int i = 0; i < ParentFolders.Count; i++)
            {
                Data.Write(ParentFolders[i].Name);
                Data.Write(ParentFolders[i].ID);
            }

            Data.Write(Server.LoadedProject.FolderDataLookup[FolderID].SubFolders.Count);
            foreach (DirectoryFolder Folder in SendFolder.SubFolders)
            {
                Data.Write(Folder.Name);
                Data.Write(Folder.ID);
            }

            Data.Write(Server.LoadedProject.FolderDataLookup[FolderID].SubFiles.Count);
            foreach (DirectoryFile File in SendFolder.SubFiles)
            {
                Data.Write(File.Name);
                Data.Write(File.GUID);
                Data.Write((int)File.FileType);
            }

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

            //SendTCPToAllClients(Data);
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

            //SendTCPToAllClients(Data);
            return Data;
        }
        #endregion
    }
}
