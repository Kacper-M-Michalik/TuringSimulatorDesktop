using System;
using System.Text;

namespace TuringCore.Networking
{
    //This class simplifies the process of creating a request packet for clients by having them have to simply call a function with the appropriate parameters, and the request packet is generated automatically.
    public static class ClientSendPacketFunctions
    {
        //Generates Load Project Request Packet
        public static Packet LoadProject(string Location)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.LoadProject);
            Data.Write(Location);

            return Data;
        }

        //Generates Request Packet that asks for data about the currently loaded project
        public static Packet RequestProjectData()
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.RequestProjectData);

            return Data;
        }

        //Generates Request Packet for this client to become the debug client that receives all copies of debugs logs made by the server
        public static Packet RequestLogReceiverStatus()
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.RequestLogReceieverStatus);

            return Data;
        }

        //Generates Request Packet that asks for a file
        public static Packet RequestFile(Guid FileID, bool RecieveUpdates)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.RequestFile);
            Data.Write(FileID);
            Data.Write(RecieveUpdates);

            return Data;
        }

        //Generates Request Packet that asks for a files metadata
        public static Packet RequestFileMetadata(Guid FileID)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.RequestFileMetadata);
            Data.Write(FileID);

            return Data;
        }

        //Generates Request Packet that asks for a folder and its related data
        public static Packet RequestFolderData(int FolderID, bool RecieveUpdates)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.RequestFolderData);
            Data.Write(FolderID);
            Data.Write(RecieveUpdates);

            return Data;
        }

        //Generates Request Packet that asks to create a new file
        public static Packet CreateFile(int Folder, string NewName, CoreFileType FileType)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.CreateFile);
            Data.Write(Folder);
            Data.Write(NewName);
            Data.Write((int)FileType);

            return Data;
        }

        //Generates Request Packet that asks edit an existing files contents
        public static Packet UpdateFile(Guid FileID, int Version, string NewContents)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.UpdateFile);
            Data.Write(FileID);
            Data.Write(Version);
            Data.Write(Encoding.ASCII.GetBytes(NewContents));

            return Data;
        }

        //Generates Request Packet that asks edit an existing files contents
        public static Packet UpdateFile(Guid FileID, int Version, byte[] NewContents)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.UpdateFile);
            Data.Write(FileID);
            Data.Write(Version);
            Data.Write(NewContents);

            return Data;
        }

        /*
        public static Packet UpdateFile(int Version, byte[] NewContents)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.UpdateFile);
            Data.Write(Version);
            Data.Write(NewContents);

            return Data;
        }
        */

        //Generates Request Packet that asks rename an existing file
        public static Packet RenameFile(Guid FileID, string NewFileName)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.RenameFile);
            Data.Write(FileID);
            Data.Write(NewFileName);

            return Data;
        }

        //Generates Request Packet that asks file to be moved to a new folder
        public static Packet MoveFile(Guid FileID, int NewFolderID)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.MoveFile);
            Data.Write(FileID);
            Data.Write(NewFolderID);

            return Data;
        }

        //Generates Request Packet that asks for a file to be deleted
        public static Packet DeleteFile(Guid FileID)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.DeleteFile);
            Data.Write(FileID);

            return Data;
        }

        //Generates Request Packet that asks the server to stop sending updates about this file to the client
        public static Packet UnsubscribeFromFileUpdates(Guid FileID)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.UnsubscribeFromUpdatesForFile);
            Data.Write(FileID);

            return Data;
        }

        //Generates Request Packet that asks the server to stop sending updates about this folder to the client
        public static Packet UnsubscribeFromFolderUpdates(int FileID)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.UnsubscribeFromUpdatesForFolder);
            Data.Write(FileID);

            return Data;
        }


        //Generates Request Packet that asks to create a new folder
        public static Packet CreateFolder(int BaseFolder, string NewName)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.CreateFolder);
            Data.Write(BaseFolder);
            Data.Write(NewName);

            return Data;
        }

        //Generates Request Packet that asks rename an existing folder
        public static Packet RenameFolder(int Folder, string NewName)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.RenameFolder);
            Data.Write(Folder);
            Data.Write(NewName);

            return Data;
        }

        //Generates Request Packet that asks folder to be moved to a new folder
        public static Packet MoveFolder(int Folder, int TargetFolder)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.MoveFolder);
            Data.Write(Folder);
            Data.Write(TargetFolder);

            return Data;
        }

        //Generates Request Packet that asks for a folder to be deleted
        public static Packet DeleteFolder(int Folder)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.DeleteFolder);
            Data.Write(Folder);

            return Data;
        }
    }
}
