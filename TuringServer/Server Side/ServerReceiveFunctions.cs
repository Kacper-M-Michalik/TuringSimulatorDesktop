﻿using System;
using System.Collections.Generic;
using System.IO;
using TuringServer.Logging;
using TuringCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TuringServer
{
    static class ServerReceiveFunctions
    {
        //Add cehcks to see if project is loaded later
        //may have to add Disposes at ends of functions?


        public delegate void PacketFunctionPointer(int SenderClientID, Packet Data);
        public static Dictionary<int, PacketFunctionPointer> PacketToFunction = new Dictionary<int, PacketFunctionPointer>()
        {
            {(int)ClientSendPackets.LoadProject, UserRequestedLoadProject},

            {(int)ClientSendPackets.RequestLogReceieverStatus, UserRequestedLogReceieverStatus},
            {(int)ClientSendPackets.RequestProjectData, UserRequestedProjectData},
            {(int)ClientSendPackets.RequestFolderData, UserRequestedFolderData},
           // {(int)ClientSendPackets.RequestFileByID, UserRequestedFileByID},
            {(int)ClientSendPackets.RequestFile, UserRequestedFile},
            {(int)ClientSendPackets.RequestFileMetadata, UserRequestedFileMetadata},
            {(int)ClientSendPackets.UnsubscribeFromUpdatesForFile, UserUnsubscribedFromFileUpdates},
            {(int)ClientSendPackets.UnsubscribeFromUpdatesForFolder, UserUnsubscribedFromFolderUpdates},

            {(int)ClientSendPackets.UpdateFile, UserUpdatedFile},
            {(int)ClientSendPackets.CreateFile, UserCreatedNewFile},
            {(int)ClientSendPackets.RenameFile, UserRenamedFile},
            {(int)ClientSendPackets.MoveFile, UserMovedFile},
            {(int)ClientSendPackets.DeleteFile, UserDeletedFile},
            {(int)ClientSendPackets.CreateFolder, UserCreatedFolder},
            {(int)ClientSendPackets.RenameFolder, UserRenamedFolder},
            {(int)ClientSendPackets.MoveFolder, UserMovedFolder},
            {(int)ClientSendPackets.DeleteFolder, UserDeletedFolder}
        };

        #region Main
        /* -PACKET LAYOUT-
         * string Location
         */
        public static void UserRequestedLoadProject(int SenderClientID, Packet Data)
        {
            string Location;

            try
            {
                Location = Data.ReadString();
            }
            catch
            {
                CustomLogging.Log("ServerReceive Error: Invalid request Load Project packet recieved from client: " + SenderClientID.ToString());
                return;
            }

            ProjectData NewProjectData = FileManager.LoadProjectFile(Location);
            if (NewProjectData == null)
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to load project - Project doesn't exist."));
                return;
            }

            //save old proj now?

            Server.LoadedProject = NewProjectData;

            Server.SendTCPToAllClients(ServerSendPacketFunctions.ProjectData());
        }

        /* -PACKET LAYOUT-
         */
        public static void UserRequestedLogReceieverStatus(int SenderClientID, Packet Data)
        {
            CustomLogging.LogClientID = SenderClientID;
        }

        /* -PACKET LAYOUT-
         */
        public static void UserRequestedProjectData(int SenderClientID, Packet Data)
        {
            if (Server.LoadedProject == null)
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("No project currently loaded!"));
                return;
            }
            Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ProjectData());
        }

        /* -PACKET LAYOUT-
         * int Folder ID
         */
        public static void UserRequestedFolderData(int SenderClientID, Packet Data)
        {
            int FolderID;
            bool SubscribeToFolderChanges;
            
            try
            {
                FolderID = Data.ReadInt();
                SubscribeToFolderChanges = Data.ReadBool();
            }
            catch
            {
                CustomLogging.Log("ServerReceive Error: Invalid request folder packet recieved from client: " + SenderClientID.ToString());
                return;
            }

            if (!Server.LoadedProject.FolderDataLookup.ContainsKey(FolderID))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to request folder - Folder doesn't exist."));
                return;
            }

            if (SubscribeToFolderChanges) Server.LoadedProject.FolderDataLookup[FolderID].SubscriberIDs.Add(SenderClientID);

            Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.FolderData(FolderID));
        }

        /* -PACKET LAYOUT-
         * int File ID
         * bool Subscribe To Updates (Whether or not client wants to recieve new version of file when it is updated)
         */
        /*
        public static void UserRequestedFileByID(int SenderClientID, Packet Data)
        {
            CustomLogging.Log("SERVER INSTRUCTION: User requested file.");

            int FileID;
            bool SubscribeToUpdates;

            try
            {
                FileID = Data.ReadInt();
                SubscribeToUpdates = Data.ReadBool();
            }
            catch
            {
                CustomLogging.Log("ServerReceive Error: Invalid request file packet recieved from client: " + SenderClientID.ToString());
                return;
            }
           
            if (!FileManager.LoadFileIntoCache(FileID))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to retreive file - Server failed to load it."));
                return;
            }             
            
            if (SubscribeToUpdates) Server.LoadedProject.FileDataLookup[FileID].SubscriberIDs.Add(SenderClientID);

            Server.LoadedProject.CacheDataLookup[FileID].ResetExpiryTimer();
            Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.FileData(FileID));
        }
        */

        public static void UserRequestedFile(int SenderClientID, Packet Data)
        {
            CustomLogging.Log("SERVER INSTRUCTION: User requested GUID file.");

            Guid FileGUID;
            bool SubscribeToUpdates;

            try
            {
                FileGUID = Data.ReadGuid();
                SubscribeToUpdates = Data.ReadBool();
            }
            catch
            {
                CustomLogging.Log("ServerReceive Error: Invalid request file packet recieved from client: " + SenderClientID.ToString());
                return;
            }

            if (!Server.LoadedProject.GuidFileLookup.ContainsKey(FileGUID))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to retreive file - File doesn't exist."));
                return;
            }

            int FileID = Server.LoadedProject.GuidFileLookup[FileGUID];

            if (!FileManager.LoadFileIntoCache(FileID))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to retreive file - Server failed to load it."));
                return;
            }

            if (SubscribeToUpdates) Server.LoadedProject.FileDataLookup[FileID].SubscriberIDs.Add(SenderClientID);

            Server.LoadedProject.CacheDataLookup[FileID].ResetExpiryTimer();
            Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.FileData(FileGUID));
        }

        public static void UserRequestedFileMetadata(int SenderClientID, Packet Data)
        {
            CustomLogging.Log("SERVER INSTRUCTION: User requested GUID Metadata file.");

            Guid FileGUID;

            try
            {
                FileGUID = Data.ReadGuid();
            }
            catch
            {
                CustomLogging.Log("ServerReceive Error: Invalid request file packet recieved from client: " + SenderClientID.ToString());
                return;
            }

            if (!Server.LoadedProject.GuidFileLookup.ContainsKey(FileGUID))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to retreive file metadata - File doesn't exist."));
                return;
            }

            Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.FileMetadata(FileGUID));
        }

        public static void UserCreatedNewFile(int SenderClientID, Packet Data)
        {
            CustomLogging.Log("SERVER INSTRUCTION: User created file.");

            int FolderID;
            string FileName;
            CoreFileType FileType;

            try
            {
                FolderID = Data.ReadInt();
                FileName = Data.ReadString();
                FileType = (CoreFileType)Data.ReadInt();
            }
            catch
            {
                CustomLogging.Log("ServerReceive Error: Invalid create file packet recieved from client: " + SenderClientID.ToString());
                return;
            }

            if (FileType == CoreFileType.Other)
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to create file - Cannot create file with unknown extension."));
                return;
            }
            if (!FileManager.IsValidFileName(FileName))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to create file - File name uses invalid characters."));
                return;
            }
            if (!Server.LoadedProject.FolderDataLookup.ContainsKey(FolderID))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to create file - Folder doesnt exist."));
                return;
            }

            DirectoryFolder ParentFolder = Server.LoadedProject.FolderDataLookup[FolderID];
            string NewFileLocation = Server.LoadedProject.BasePath + ParentFolder.LocalPath + FileName + FileManager.FileTypeToExtension(FileType);

            string FinalFileName = FileName;
            if (File.Exists(NewFileLocation))
            {
                bool Failed = true;
                int CopyVersion = 1;
                while (CopyVersion < int.MaxValue-1 && Failed)
                {
                    FinalFileName = FileName + " (" + CopyVersion.ToString() + ")";
                    NewFileLocation = Server.LoadedProject.BasePath + ParentFolder.LocalPath + FinalFileName + FileManager.FileTypeToExtension(FileType);
                    Failed = File.Exists(NewFileLocation);
                    CopyVersion++;
                }

                if (Failed)
                {
                    Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to create file - File already exists/too many default name files currently exist."));
                    return;
                }
            }

            Guid FileGUID = Guid.NewGuid();

            try
            {
                FileStream MetadataFilestream = File.Create(Server.LoadedProject.BasePath + ParentFolder.LocalPath + FinalFileName + ".tmeta");
                MetadataFilestream.Write(JsonSerializer.SerializeToUtf8Bytes(new ObjectMetadataFile() { FileGUID = FileGUID, FileName = FinalFileName, FileType = FileType }));
                MetadataFilestream.Close(); 


                FileStream DataFileStream = File.Create(NewFileLocation);
                switch (FileType)
                {
                    case CoreFileType.Alphabet:
                        DataFileStream.Write(JsonSerializer.SerializeToUtf8Bytes(new Alphabet()));
                        break;
                    case CoreFileType.Tape:
                        DataFileStream.Write(JsonSerializer.SerializeToUtf8Bytes(new TapeTemplate()));
                        break;
                    case CoreFileType.TransitionFile:
                        DataFileStream.Write(JsonSerializer.SerializeToUtf8Bytes(new TransitionFile()));
                        break;
                    case CoreFileType.SlateFile:
                        DataFileStream.Write(JsonSerializer.SerializeToUtf8Bytes(new SlateFile()));
                        break;
                }
                DataFileStream.Close();
            }
            catch (Exception E)
            {
                CustomLogging.Log("ServerRecieve Error: UserCreatedNewFile - " + E.ToString());
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to create file - Server failed to create it."));
                return;
            }

            int NewID = FileManager.GetNewFileID();

            Server.LoadedProject.GuidFileLookup.Add(FileGUID, NewID);
            DirectoryFile NewFileData = new DirectoryFile(NewID, FileGUID, FinalFileName, FileType, Server.LoadedProject.FolderDataLookup[FolderID]);
            Server.LoadedProject.FileDataLookup.Add(NewID, NewFileData);
            ParentFolder.SubFiles.Add(NewFileData);
            FileManager.LoadFileIntoCache(NewID);

            Packet SendPacket = ServerSendPacketFunctions.FolderData(FolderID);
            foreach (int SubscriberID in Server.LoadedProject.FolderDataLookup[FolderID].SubscriberIDs)
            {
                Server.SendTCPData(SubscriberID, SendPacket);
            }
        }

        public static void UserCreatedNewFileWithData(int SenderClientID, Packet Data)
        {
            CustomLogging.Log("SERVER INSTRUCTION: User created file with inital data.");

            int FolderID;
            string FileName;
            CoreFileType FileType;
            byte[] InitialData;

            try
            {
                FolderID = Data.ReadInt();
                FileName = Data.ReadString();
                FileType = (CoreFileType)Data.ReadInt();
                InitialData = Data.ReadByteArray();
            }
            catch
            {
                CustomLogging.Log("ServerReceive Error: Invalid create file packet recieved from client: " + SenderClientID.ToString());
                return;
            }

            if (FileType == CoreFileType.Other)
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to create file - Cannot create file with unknown extension."));
                return;
            }
            if (!FileManager.IsValidFileName(FileName))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to create file - File name uses invalid characters."));
                return;
            }
            if (!Server.LoadedProject.FolderDataLookup.ContainsKey(FolderID))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to create file - Folder doesnt exist."));
                return;
            }

            DirectoryFolder ParentFolder = Server.LoadedProject.FolderDataLookup[FolderID];
            string NewFileLocation = Server.LoadedProject.BasePath + ParentFolder.LocalPath + FileName + FileManager.FileTypeToExtension(FileType);

            string FinalFileName = FileName;
            if (File.Exists(NewFileLocation))
            {
                bool Failed = true;
                int CopyVersion = 1;
                while (CopyVersion < int.MaxValue - 1 && Failed)
                {
                    FinalFileName = FileName + " (" + CopyVersion.ToString() + ")";
                    NewFileLocation = Server.LoadedProject.BasePath + ParentFolder.LocalPath + FinalFileName + FileManager.FileTypeToExtension(FileType);
                    Failed = File.Exists(NewFileLocation);
                    CopyVersion++;
                }

                if (Failed)
                {
                    Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to create file - File already exists/too many default name files currently exist."));
                    return;
                }
            }

            Guid FileGUID = Guid.NewGuid();

            try
            {
                FileStream MetadataFilestream = File.Create(Server.LoadedProject.BasePath + ParentFolder.LocalPath + FinalFileName + ".tmeta");
                MetadataFilestream.Write(JsonSerializer.SerializeToUtf8Bytes(new ObjectMetadataFile() { FileGUID = FileGUID, FileName = FinalFileName, FileType = FileType }));
                MetadataFilestream.Close();


                FileStream DataFileStream = File.Create(NewFileLocation);
                try
                {
                    switch (FileType)
                    {
                        case CoreFileType.Alphabet:
                            Alphabet A = JsonSerializer.Deserialize<Alphabet>(InitialData);
                            break;
                        case CoreFileType.Tape:
                            TapeTemplate T = JsonSerializer.Deserialize<TapeTemplate>(InitialData);
                            break;
                        case CoreFileType.TransitionFile:
                            TransitionFile Ta = JsonSerializer.Deserialize<TransitionFile>(InitialData);
                            break;
                        case CoreFileType.SlateFile:
                            SlateFile S = JsonSerializer.Deserialize<SlateFile>(InitialData);
                            break;
                    }
                    DataFileStream.Write(InitialData);
                }
                catch (Exception E)
                {
                    CustomLogging.Log("ServerRecieve Error: UserCreatedNewFile - " + E.ToString());
                    Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to create file - Invalid inital dat sent."));
                    return;
                }                    
               
                DataFileStream.Close();
            }
            catch (Exception E)
            {
                CustomLogging.Log("ServerRecieve Error: UserCreatedNewFile - " + E.ToString());
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to create file - Server failed to create it."));
                return;
            }

            int NewID = FileManager.GetNewFileID();

            Server.LoadedProject.GuidFileLookup.Add(FileGUID, NewID);
            DirectoryFile NewFileData = new DirectoryFile(NewID, FileGUID, FinalFileName, FileType, Server.LoadedProject.FolderDataLookup[FolderID]);
            Server.LoadedProject.FileDataLookup.Add(NewID, NewFileData);
            ParentFolder.SubFiles.Add(NewFileData);
            FileManager.LoadFileIntoCache(NewID);

            Packet SendPacket = ServerSendPacketFunctions.FolderData(FolderID);
            foreach (int SubscriberID in Server.LoadedProject.FolderDataLookup[FolderID].SubscriberIDs)
            {
                Server.SendTCPData(SubscriberID, SendPacket);
            }
        }

        public static void UserUpdatedFile(int SenderClientID, Packet Data)
        {
            CustomLogging.Log("SERVER INSTRUCTION: User updated file.");

            Guid FileGUID;
            int FileVersion;
            byte[] NewData;

            try
            {
                FileGUID = Data.ReadGuid();
                FileVersion = Data.ReadInt();
                NewData = Data.ReadByteArray();
            }
            catch
            {
                CustomLogging.Log("ServerReceive Error: Invalid update file packet recieved from client: " + SenderClientID.ToString());
                return;
            }

            //Possibly implement parsing bytes into actual object to see if it succeeds -> prevent users from sending corrupt files

            if (!Server.LoadedProject.GuidFileLookup.ContainsKey(FileGUID))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to update file - File doesn't exist."));
                return;
            }

            int FileID = Server.LoadedProject.GuidFileLookup[FileGUID];

            if (!Server.LoadedProject.FileDataLookup.ContainsKey(FileID))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to update file - File doesn't exist."));
                return;
            }

            DirectoryFile FileData = Server.LoadedProject.FileDataLookup[FileID];
            if (FileData.Version != FileVersion)
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to update file - You updated an older version of the file."));
                return;
            }

            try
            {
                File.WriteAllBytes(Server.LoadedProject.BasePath + FileData.GetLocalPath(), NewData);
            }
            catch (Exception E)
            {
                CustomLogging.Log("ServerRecieve Error: UserUpdatedFile - " + E.ToString());
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to update file - Server failed to write to file."));
                return;
            }

            if (!Server.LoadedProject.CacheDataLookup.ContainsKey(FileID))
            {
                if (!FileManager.LoadFileIntoCache(FileID))
                {
                    Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to update file - Server failed to load it."));
                    return;
                }
            }
            else
            {
                Server.LoadedProject.CacheDataLookup[FileID].FileData = NewData;
            }

            FileData.Version++;

            Server.LoadedProject.CacheDataLookup[FileID].ResetExpiryTimer();

            Packet SendPacket = ServerSendPacketFunctions.FileData(FileGUID);
            foreach (int Client in FileData.SubscriberIDs)
            {
                Server.SendTCPData(Client, SendPacket);

                //ServerSendFunctions.SendFileUpdate(Client, FileID);
            }            
        }

        public static void UserRenamedFile(int SenderClientID, Packet Data)
        {
            CustomLogging.Log("SERVER INSTRUCTION: User renamed file.");

            Guid FileGUID;
            string NewFileName;

            try
            {
                FileGUID = Data.ReadGuid();
                NewFileName = Data.ReadString();
            }
            catch
            {
                CustomLogging.Log("ServerReceive Error: Invalid rename file packet recieved from client: " + SenderClientID.ToString());
                return;
            }

            if (!Server.LoadedProject.GuidFileLookup.ContainsKey(FileGUID))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to rename file - File doesn't exist."));
                return;
            }

            int FileID = Server.LoadedProject.GuidFileLookup[FileGUID];

            if (!FileManager.IsValidFileName(NewFileName))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to rename file - File name uses invalid characters."));
                return;
            }

            if (!Server.LoadedProject.CacheDataLookup.ContainsKey(FileID))
            {
                if (!FileManager.LoadFileIntoCache(FileID))
                {
                    Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to rename file - Server failed to load it."));
                    return;
                }
            }

            DirectoryFile FileData = Server.LoadedProject.FileDataLookup[FileID];

            string NewFileLocation = Server.LoadedProject.BasePath + FileData.ParentFolder.LocalPath + Path.DirectorySeparatorChar + NewFileName + FileManager.FileTypeToExtension(FileData.FileType);
            string NewMetafileLocation = Server.LoadedProject.BasePath + FileData.ParentFolder.LocalPath + Path.DirectorySeparatorChar + NewFileName + ".tmeta";

            if (File.Exists(NewFileLocation))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to rename file - File with this name already exists."));
                return;
            }

            try
            {
                //Replace with async here later?
                File.WriteAllBytes(NewMetafileLocation, JsonSerializer.SerializeToUtf8Bytes(new ObjectMetadataFile() { FileName = NewFileName, FileGUID = FileGUID, FileType = FileData.FileType }));
                File.WriteAllBytes(NewFileLocation, Server.LoadedProject.CacheDataLookup[FileID].FileData);               
            }
            catch (Exception E)
            {
                CustomLogging.Log("ServerRecieveError: UserRenameFile - " + E.ToString());
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to rename file - Server failed to save the renamed/moved file."));
                return;
            }

            if (!FileManager.DeleteFileByPath(Server.LoadedProject.BasePath + FileData.GetLocalPath()) || !FileManager.DeleteFileByPath(Server.LoadedProject.BasePath + FileData.GetMetadataLocalPath())) 
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to rename file - Server failed to clean old file."));
                FileManager.DeleteFileByPath(NewFileLocation);
                FileManager.DeleteFileByPath(NewMetafileLocation);
                return;
            }
            
            FileData.Name = NewFileName;

            Packet SendPacket = ServerSendPacketFunctions.FolderData(FileData.ParentFolder.ID);
            foreach (int SubscriberID in Server.LoadedProject.FolderDataLookup[FileData.ParentFolder.ID].SubscriberIDs)
            {
                Server.SendTCPData(SubscriberID, SendPacket);
            }

            //REDO TO SEND ONYL RENAME INSTEAD OF WHOLE FILE
            SendPacket = ServerSendPacketFunctions.FileData(FileData.GUID);
            foreach (int SubscriberID in Server.LoadedProject.FileDataLookup[FileData.ID].SubscriberIDs)
            {
                Server.SendTCPData(SubscriberID, SendPacket);
            }

            //Server.SendTCPToAllClients(ServerSendPacketFunctions.FileRenamed(FileID));
        }

        //TODO THIS ONE
        public static void UserMovedFile(int SenderClientID, Packet Data)
        {
            CustomLogging.Log("SERVER INSTRUCTION: User moved file.");

            Guid FileGUID;
            int NewFolderID;

            try
            {
                FileGUID = Data.ReadGuid();
                NewFolderID = Data.ReadInt();
            }
            catch
            {
                CustomLogging.Log("ServerReceive Error: Invalid move file packet recieved from client: " + SenderClientID.ToString());
                return;
            }

            if (!Server.LoadedProject.GuidFileLookup.ContainsKey(FileGUID))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to move file - File doesn't exist."));
                return;
            }

            int FileID = Server.LoadedProject.GuidFileLookup[FileGUID];

            if (!Server.LoadedProject.FolderDataLookup.ContainsKey(NewFolderID))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to move file - Target folder doesn't exist."));
                return;
            }

            if (!Server.LoadedProject.CacheDataLookup.ContainsKey(FileID))
            {
                if (!FileManager.LoadFileIntoCache(FileID))
                {
                    Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to move file - Server failed to load it."));
                    return;
                }
            }

            DirectoryFile FileData = Server.LoadedProject.FileDataLookup[FileID];
            DirectoryFolder FolderData = Server.LoadedProject.FolderDataLookup[NewFolderID];
            string NewFileLocation = Server.LoadedProject.BasePath + FolderData.LocalPath + FileData.Name;

            if (File.Exists(NewFileLocation))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to move file - File with this name already exists."));
                return;
            }

            try
            {
                //Replace with async here later?
                File.WriteAllBytes(NewFileLocation, Server.LoadedProject.CacheDataLookup[FileID].FileData);
            }
            catch (Exception E)
            {
                CustomLogging.Log("ServerRecieveError: UserMoveFile - " + E.ToString());
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to move file - Server failed to save the renamed/moved file."));
                return;
            }

            if (!FileManager.DeleteFileByPath(Server.LoadedProject.BasePath + FileData.GetLocalPath()))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to move file - Server failed to clean old file."));
                FileManager.DeleteFileByPath(NewFileLocation);
                return;
            }

            int PreviousFolderID = FileData.ParentFolder.ID;

            FileData.ParentFolder.SubFiles.Remove(FileData);
            FileData.ParentFolder = FolderData;

            Packet SendPacket = ServerSendPacketFunctions.FolderData(PreviousFolderID);
            foreach (int SubscriberID in Server.LoadedProject.FolderDataLookup[PreviousFolderID].SubscriberIDs)
            {
                Server.SendTCPData(SubscriberID, SendPacket);
            }

            SendPacket = ServerSendPacketFunctions.FolderData(FileData.ID);
            foreach (int SubscriberID in Server.LoadedProject.FolderDataLookup[FileData.ID].SubscriberIDs)
            {
                Server.SendTCPData(SubscriberID, SendPacket);
            }

            //Server.SendTCPToAllClients(ServerSendPacketFunctions.FileMoved(FileID));
        }

        public static void UserDeletedFile(int SenderClientID, Packet Data)
        {
            CustomLogging.Log("SERVER INSTRUCTION: User deleted file.");

            Guid FileGUID;

            try
            {
                FileGUID = Data.ReadGuid();
            }
            catch
            {
                CustomLogging.Log("ServerReceive Error: Invalid delete file packet recieved from client: " + SenderClientID.ToString());
                return;
            }

            if (!Server.LoadedProject.GuidFileLookup.ContainsKey(FileGUID))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to delete file - File doesn't exist."));
                return;
            }

            int FileID = Server.LoadedProject.GuidFileLookup[FileGUID];

            DirectoryFile FileData = Server.LoadedProject.FileDataLookup[FileID];

            FileManager.DeleteFileByPath(Server.LoadedProject.BasePath + FileData.GetLocalPath());
            FileManager.DeleteFileByPath(Server.LoadedProject.BasePath + FileData.GetMetadataLocalPath());

            FileData.ParentFolder.SubFiles.Remove(FileData);

            Server.LoadedProject.CacheDataLookup.Remove(FileID);
            Server.LoadedProject.FileDataLookup.Remove(FileID);

            Packet SendPacket = ServerSendPacketFunctions.FolderData(FileData.ParentFolder.ID);
            foreach (int SubscriberID in Server.LoadedProject.FolderDataLookup[FileData.ParentFolder.ID].SubscriberIDs)
            {
                Server.SendTCPData(SubscriberID, SendPacket);
            }

            SendPacket = ServerSendPacketFunctions.FileDeleted(FileData.GUID);
            foreach (int SubscriberID in FileData.SubscriberIDs)
            {
                Server.SendTCPData(SubscriberID, SendPacket);
            }

            //Server.SendTCPToAllClients(ServerSendPacketFunctions.FileDeleted(FileID));
        }




        public static void UserUnsubscribedFromFileUpdates(int SenderClientID, Packet Data)
        {
            CustomLogging.Log("SERVER INSTRUCTION: User unsubbed from file.");

            Guid FileGUID;

            try
            {
                FileGUID = Data.ReadGuid();
            }
            catch
            {
                CustomLogging.Log("ServerReceive Error: Invalid unsubscribe file packet recieved from client: " + SenderClientID.ToString());
                return;
            }

            if (!Server.LoadedProject.GuidFileLookup.ContainsKey(FileGUID))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to unsubscribe from file - File doesn't exist."));
                return;
            }

            int FileID = Server.LoadedProject.GuidFileLookup[FileGUID];

            if (!Server.LoadedProject.GuidFileLookup.ContainsKey(FileGUID))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to unsubscribe from file - File doesn't exist."));
                return;
            }

            CustomLogging.Log("SERVER INSTRUCTION: User "+SenderClientID.ToString()+" no longer recieiving updates to file "+ FileID.ToString()+".");

            Server.LoadedProject.FileDataLookup[FileID].SubscriberIDs.Remove(SenderClientID);
        }

        public static void UserUnsubscribedFromFolderUpdates(int SenderClientID, Packet Data)
        {
            CustomLogging.Log("SERVER INSTRUCTION: User unsubbed from folder.");

            int FolderID;

            try
            {
                FolderID = Data.ReadInt();
            }
            catch
            {
                CustomLogging.Log("ServerReceive Error: Invalid unsubscribe folder packet recieved from client: " + SenderClientID.ToString());
                return;
            }

            if (!Server.LoadedProject.FolderDataLookup.ContainsKey(FolderID))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to unsubscribe from folder - Folder doesn't exist."));
                return;
            }

            CustomLogging.Log("SERVER INSTRUCTION: User " + SenderClientID.ToString() + " no longer recieiving updates to folder " + FolderID.ToString() + ".");

            Server.LoadedProject.FolderDataLookup[FolderID].SubscriberIDs.Remove(SenderClientID);
        }



        public static void UserCreatedFolder(int SenderClientID, Packet Data)
        {
            CustomLogging.Log("SERVER INSTRUCTION: User created folder.");

            int ParentFolderID;
            string NewFolderName;

            try
            {
                ParentFolderID = Data.ReadInt();
                NewFolderName = Data.ReadString();
            }
            catch
            {
                CustomLogging.Log("ServerReceive Error: Invalid create folder packet recieved from client: " + SenderClientID.ToString());
                return;
            }

            if (!FileManager.IsValidFileName(NewFolderName))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to create folder - New folder name invalid."));
                return;
            }

            if (!Server.LoadedProject.FolderDataLookup.ContainsKey(ParentFolderID))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to create folder - Root folder doesn't exist."));
                return;
            }

            DirectoryFolder ParentFolderData = Server.LoadedProject.FolderDataLookup[ParentFolderID];

            string NewFolderDirectory = Server.LoadedProject.BasePath + ParentFolderData.LocalPath + NewFolderName + Path.DirectorySeparatorChar;

            string FinalFolderName = NewFolderName;

            string BaseDirectoryName = Server.LoadedProject.BasePath + ParentFolderData.LocalPath;
            if (Directory.Exists(NewFolderDirectory))
            {
                bool Failed = true;
                int CopyVersion = 1;
                while (CopyVersion < int.MaxValue - 1 && Failed)
                {
                    FinalFolderName = NewFolderName + " (" + CopyVersion.ToString() + ")";
                    NewFolderDirectory = BaseDirectoryName +  FinalFolderName + Path.DirectorySeparatorChar;
                    Failed = Directory.Exists(NewFolderDirectory);
                    CopyVersion++;
                }

                if (Failed)
                {
                    Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to create folder - Folder already exists/too many default name folders currently exist."));
                    return;
                }
            }
            
            try
            {
                Directory.CreateDirectory(NewFolderDirectory);
            }
            catch (Exception E)
            {
                CustomLogging.Log("ServerRecieve Error: UserCreatedFolder - " + E.ToString());
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to create folder - Server failed to create the folder locally."));
                return;
            }

            int NewID = FileManager.GetNewFileID();
            DirectoryFolder NewFolderData = new DirectoryFolder(NewID, FinalFolderName, ParentFolderData);
            ParentFolderData.SubFolders.Add(NewFolderData);
            Server.LoadedProject.FolderDataLookup.Add(NewID, NewFolderData);

            Packet SendPacket = ServerSendPacketFunctions.FolderData(ParentFolderData.ID);
            foreach (int SubscriberID in Server.LoadedProject.FolderDataLookup[ParentFolderData.ID].SubscriberIDs)
            {
                Server.SendTCPData(SubscriberID, SendPacket);
            }

            //Server.SendTCPToAllClients(ServerSendPacketFunctions.FolderCreated(NewID));
        }

        public static void UserRenamedFolder(int SenderClientID, Packet Data)
        {
            CustomLogging.Log("SERVER INSTRUCTION: User renamed folder.");

            int FolderID;
            string NewFolderName;

            try
            {
                FolderID = Data.ReadInt();
                NewFolderName = Data.ReadString();
            }
            catch
            {
                CustomLogging.Log("ServerReceive Error: Invalid rename folder packet recieved from client: " + SenderClientID.ToString());
                return;
            }

            if (FolderID == 0)
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to rename folder - Cannot rename base project folder."));
                return;
            }

            if (!FileManager.IsValidFileName(NewFolderName))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to rename folder - New folder name invalid."));
                return;
            }

            if (!Server.LoadedProject.FolderDataLookup.ContainsKey(FolderID))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to rename folder - Folder doesn't exist."));
                return;
            }

            DirectoryFolder BaseFolder = Server.LoadedProject.FolderDataLookup[FolderID];
            string NewDirectory = Server.LoadedProject.BasePath + BaseFolder.ParentFolder.LocalPath + NewFolderName + Path.DirectorySeparatorChar;

            if (Directory.Exists(NewDirectory))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to rename folder - Folder with this name already exists."));
                return;
            }

            try
            {
                Directory.Move(Server.LoadedProject.BasePath + BaseFolder.LocalPath, NewDirectory);
            }
            catch (Exception E)
            {
                CustomLogging.Log("ServerRecieve Error: UserRenamedFolder - " + E.ToString());
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to rename folder - Server failed to rename the folder locally."));
                return;
            }

            BaseFolder.Name = NewFolderName;

            Queue<DirectoryFolder> FoldersToReworkPath = new Queue<DirectoryFolder>();
            FoldersToReworkPath.Enqueue(BaseFolder);

            while (FoldersToReworkPath.Count != 0)
            {
                DirectoryFolder RenameFolderData = FoldersToReworkPath.Dequeue();
                RenameFolderData.UpdatePath();

                for (int i = 0; i < RenameFolderData.SubFolders.Count; i++)
                {
                    FoldersToReworkPath.Enqueue(RenameFolderData.SubFolders[i]);
                }
            }

            Packet SendPacket = ServerSendPacketFunctions.FolderData(BaseFolder.ParentFolder.ID);
            foreach (int SubscriberID in Server.LoadedProject.FolderDataLookup[BaseFolder.ParentFolder.ID].SubscriberIDs)
            {
                Server.SendTCPData(SubscriberID, SendPacket);
            }

            //Server.SendTCPToAllClients(ServerSendPacketFunctions.FolderRenamed(FolderID));
        }

        public static void UserMovedFolder(int SenderClientID, Packet Data)
        {
            CustomLogging.Log("SERVER INSTRUCTION: User moved folder.");

            int FolderID;
            int TargetFolderID;

            try
            {
                FolderID = Data.ReadInt();
                TargetFolderID = Data.ReadInt();
            }
            catch
            {
                CustomLogging.Log("ServerReceive Error: Invalid move folder packet recieved from client: " + SenderClientID.ToString());
                return;
            }

            if (FolderID == 0)
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to move folder - Cannot move base project folder."));
                return;
            }

            if (!Server.LoadedProject.FolderDataLookup.ContainsKey(FolderID))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to move folder - Folder doesn't exist."));
                return;
            }

            if (!Server.LoadedProject.FolderDataLookup.ContainsKey(TargetFolderID))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to move folder - Target folder doesn't exist."));
                return;
            }

            DirectoryFolder FolderData = Server.LoadedProject.FolderDataLookup[FolderID];
            DirectoryFolder TargetFolderData = Server.LoadedProject.FolderDataLookup[TargetFolderID];

            string NewDirectory = Server.LoadedProject.BasePath + TargetFolderData.LocalPath + FolderData.Name + Path.DirectorySeparatorChar;

            if (Directory.Exists(NewDirectory))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to move folder - Folder with this name already exists."));
                return;
            }

            try
            {
                Directory.Move(Server.LoadedProject.BasePath + FolderData.LocalPath, NewDirectory);
            }
            catch (Exception E)
            {
                CustomLogging.Log("ServerRecieve Error: UserMovedFolder - " + E.ToString());
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to move folder - Server failed to move the folder locally."));
                return;
            }

            DirectoryFolder PreviousParentFolder = FolderData.ParentFolder;
            FolderData.ParentFolder.SubFolders.Remove(FolderData);
            FolderData.ParentFolder = TargetFolderData;
            TargetFolderData.SubFolders.Add(FolderData);

            Queue<DirectoryFolder> FoldersToReworkPath = new Queue<DirectoryFolder>();
            FoldersToReworkPath.Enqueue(FolderData);

            while (FoldersToReworkPath.Count != 0)
            {
                DirectoryFolder RenameFolderData = FoldersToReworkPath.Dequeue();
                RenameFolderData.UpdatePath();

                for (int i = 0; i < RenameFolderData.SubFolders.Count; i++)
                {
                    FoldersToReworkPath.Enqueue(RenameFolderData.SubFolders[i]);
                }
            }

            Packet SendPacket = ServerSendPacketFunctions.FolderData(TargetFolderID);
            foreach (int SubscriberID in Server.LoadedProject.FolderDataLookup[TargetFolderID].SubscriberIDs)
            {
                Server.SendTCPData(SubscriberID, SendPacket);
            }

            SendPacket = ServerSendPacketFunctions.FolderData(PreviousParentFolder.ID);
            foreach (int SubscriberID in Server.LoadedProject.FolderDataLookup[PreviousParentFolder.ID].SubscriberIDs)
            {
                Server.SendTCPData(SubscriberID, SendPacket);
            }

            //Server.SendTCPToAllClients(ServerSendPacketFunctions.FolderMoved(FolderID));
        }

        public static void UserDeletedFolder(int SenderClientID, Packet Data)
        {
            CustomLogging.Log("SERVER INSTRUCTION: User deleted folder.");

            int FolderID;

            try
            {
                FolderID = Data.ReadInt();
            }
            catch
            {
                CustomLogging.Log("ServerReceive Error: Invalid delete folder packet recieved from client: " + SenderClientID.ToString());
                return;
            }

            if (FolderID == 0)
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to delete folder - Cannot delete base project folder."));
                return;
            }

            if (!Server.LoadedProject.FolderDataLookup.ContainsKey(FolderID))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to delete folder - Folder doesn't exist."));
                return;
            }

            DirectoryFolder FolderData = Server.LoadedProject.FolderDataLookup[FolderID];

            try
            {
                Directory.Delete(Server.LoadedProject.BasePath + FolderData.LocalPath, true);
            }
            catch (Exception E)
            {
                CustomLogging.Log("ServerRecieve Error: UserDeletedFolder - " + E.ToString());
            }

            FolderData.ParentFolder.SubFolders.Remove(FolderData);

            Queue<DirectoryFolder> FoldersToClean = new Queue<DirectoryFolder>();
            FoldersToClean.Enqueue(FolderData);

            while (FoldersToClean.Count != 0)
            {
                DirectoryFolder SearchFolderData = FoldersToClean.Dequeue();
                Server.LoadedProject.FolderDataLookup.Remove(SearchFolderData.ID);

                for (int i = 0; i < SearchFolderData.SubFiles.Count; i++)
                {
                    Server.LoadedProject.FileDataLookup.Remove(SearchFolderData.SubFiles[i].ID);
                    Server.LoadedProject.CacheDataLookup.Remove(SearchFolderData.SubFiles[i].ID);
                }

                for (int i = 0; i < SearchFolderData.SubFolders.Count; i++)
                {
                    FoldersToClean.Enqueue(SearchFolderData.SubFolders[i]);
                }
            }

            Packet SendPacket = ServerSendPacketFunctions.FolderData(FolderData.ParentFolder.ID);
            foreach (int SubscriberID in Server.LoadedProject.FolderDataLookup[FolderData.ParentFolder.ID].SubscriberIDs)
            {
                Server.SendTCPData(SubscriberID, SendPacket);
            }

            //Server.SendTCPToAllClients(ServerSendPacketFunctions.FolderDeleted(FolderID));
        }

        #endregion

    }

}
