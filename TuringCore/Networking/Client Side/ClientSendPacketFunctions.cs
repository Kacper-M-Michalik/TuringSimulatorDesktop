using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringCore;

namespace TuringCore
{
    public static class ClientSendPacketFunctions
    {
        public static Packet LoadProject(string Location)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.LoadProject);
            Data.Write(Location);

            return Data;
        }

        public static Packet RequestProjectData()
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.RequestProjectData);

            return Data;
        }

        public static Packet RequestLogReceiverStatus()
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.RequestLogReceieverStatus);

            return Data;
        }

        public static Packet RequestFile(Guid FileID, bool RecieveUpdates)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.RequestFileByGUID);
            Data.Write(FileID);
            Data.Write(RecieveUpdates);

            return Data;
        }

        public static Packet RequestFileByGUID(Guid FileID, bool RecieveUpdates)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.RequestFileByGUID);
            Data.Write(FileID);
            Data.Write(RecieveUpdates);

            return Data;
        }

        public static Packet RequestFolderData(int FolderID, bool RecieveUpdates)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.RequestFolderData);
            Data.Write(FolderID);
            Data.Write(RecieveUpdates);

            return Data;
        }

        public static Packet CreateFile(int Folder, string NewName, CoreFileType FileType)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.CreateFile);
            Data.Write(Folder);
            Data.Write(NewName);
            Data.Write((int)FileType);

            return Data;
        }

        public static Packet UpdateFile(Guid FileID, int Version, string NewContents)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.UpdateFile);
            Data.Write(FileID);
            Data.Write(Version);
            Data.Write(Encoding.ASCII.GetBytes(NewContents));

            return Data;
        }

        public static Packet UpdateFile(int Version, byte[] NewContents)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.UpdateFile);
            Data.Write(Version);
            Data.Write(NewContents);

            return Data;
        }

        public static Packet RenameFile(Guid FileID, string NewFileName)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.RenameFile);
            Data.Write(FileID);
            Data.Write(NewFileName);

            return Data;
        }

        public static Packet MoveFile(Guid FileID, int NewFolderID)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.MoveFile);
            Data.Write(FileID);
            Data.Write(NewFolderID);

            return Data;
        }

        public static Packet DeleteFile(Guid FileID)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.DeleteFile);
            Data.Write(FileID);

            return Data;
        }

        public static Packet UnsubscribeFromFileUpdates(Guid FileID)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.UnsubscribeFromUpdatesForFile);
            Data.Write(FileID);

            return Data;
        }

        public static Packet UnsubscribeFromFolderUpdates(int FileID)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.UnsubscribeFromUpdatesForFolder);
            Data.Write(FileID);

            return Data;
        }


        public static Packet CreateFolder(int BaseFolder, string NewName)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.CreateFolder);
            Data.Write(BaseFolder);
            Data.Write(NewName);

            return Data;
        }

        public static Packet RenameFolder(int Folder, string NewName)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.RenameFolder);
            Data.Write(Folder);
            Data.Write(NewName);

            return Data;
        }

        public static Packet MoveFolder(int Folder, int TargetFolder)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.MoveFolder);
            Data.Write(Folder);
            Data.Write(TargetFolder);

            return Data;
        }

        public static Packet DeleteFolder(int Folder)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.DeleteFolder);
            Data.Write(Folder);

            return Data;
        }
    }
}
