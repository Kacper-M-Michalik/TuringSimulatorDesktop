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

            CustomLogging.Log("SERVER: ErrorNotif COPY: " + ErrorString);

            Data.Write((int)ServerSendPackets.ErrorNotification);
            Data.Write(ErrorString);

            return Data;
        }

        public static Packet ProjectData()
        {
            Packet Data = new Packet();

            Data.Write((int)ServerSendPackets.SentProjectData);
            Data.Write(Server.LoadedProject.ProjectName);

            return Data;
        }

        public static Packet FolderData(int FolderID)
        {
            Packet Data = new Packet();

            Data.Write((int)ServerSendPackets.SentFolderData);
            Data.Write(FolderID);

            Data.Write(Server.LoadedProject.FolderDataLookup[FolderID].SubFolders.Count);
            foreach (DirectoryFolder Folder in Server.LoadedProject.FolderDataLookup[FolderID].SubFolders)
            {
                Data.Write(Folder.Name);
                Data.Write(Folder.ID);
            }

            Data.Write(Server.LoadedProject.FolderDataLookup[FolderID].SubFiles.Count);
            foreach (DirectoryFile File in Server.LoadedProject.FolderDataLookup[FolderID].SubFiles)
            {
                Data.Write(File.Name);
                Data.Write(File.ID);
            }

            return Data;
        }

        public static Packet FileData(int FileID)
        {
            Packet Data = new Packet();

            Data.Write((int)ServerSendPackets.SentOrUpdatedFile);
            Data.Write(Server.LoadedProject.FileDataLookup[FileID].Version);
            Data.Write(Server.LoadedProject.CacheDataLookup[FileID].FileData);

            return Data;
        }

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

        public static Packet FileDeleted(int FileID)
        {
            Packet Data = new Packet();

            Data.Write((int)ServerSendPackets.DeletedFile);
            Data.Write(FileID);

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
