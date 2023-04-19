using System;
using System.Collections.Generic;
using System.IO;
using TuringServer.Logging;
using TuringCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using TuringCore.Files;
using TuringCore.Networking;
using TuringServer.Data;

namespace TuringServer.ServerSide
{
    static class ServerReceiveFunctions
    {
        //SInce every request has a request type ID, we can use this ID to create a function lookup dictionary, where we get returned a function pointer to the appropriate function to call to process this request, allowing us to skip a large case/if block of code, making it cleaner, and easy to modify, as new functions to process requests only have to be added to this dictionary to be used  to the server
        public delegate void PacketFunctionPointer(int SenderClientID, Packet Data);
        //Here we map the request ID's to the appropriate functions
        public static Dictionary<int, PacketFunctionPointer> PacketToFunction = new Dictionary<int, PacketFunctionPointer>()
        {
            {(int)ClientSendPackets.LoadProject, UserRequestedLoadProject},

            {(int)ClientSendPackets.RequestLogReceieverStatus, UserRequestedLogReceieverStatus},
            {(int)ClientSendPackets.RequestProjectData, UserRequestedProjectData},
            {(int)ClientSendPackets.RequestFolderData, UserRequestedFolderData},
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

            //Check packet validity
            try
            {
                Location = Data.ReadString();
            }
            catch
            {
                //Log error if an invalid packet was received
                CustomLogging.Log("ServerReceive Error: Invalid request Load Project packet received from client: " + SenderClientID.ToString());
                return;
            }

            ProjectData NewProjectData = FileManager.LoadProjectFile(Location);

            //Send back the client an error response
            if (NewProjectData == null)
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to load project - Project doesn't exist."));
                return;
            }

            Server.LoadedProject = NewProjectData;

            //Notify all clients that a new project has been loaded
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
         * bool Subscribe To Folder Changes
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

            //If the client subscribed to updates, we add them to the subscriber list for the folder
            if (SubscribeToFolderChanges) Server.LoadedProject.FolderDataLookup[FolderID].SubscriberIDs.Add(SenderClientID);

            //Send back a response to the client
            Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.FolderData(FolderID));
        }

        /* -PACKET LAYOUT-
         * GUID File ID
         * bool Subscribe To Updates (Whether or not client wants to receive new version of file when it is updated)
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
                CustomLogging.Log("ServerReceive Error: Invalid request file packet received from client: " + SenderClientID.ToString());
                return;
            }

            if (!Server.LoadedProject.GuidFileLookup.ContainsKey(FileGUID))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to retrieve file - File doesn't exist."));
                return;
            }

            int FileID = Server.LoadedProject.GuidFileLookup[FileGUID];

            //Check if the file requested exists in cache, if not it is automatically loaded in, if there is an error while loading it in we send back the client an Error Notification response
            if (!FileManager.LoadFileIntoCache(FileID))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to retrieve file - Server failed to load it."));
                return;
            }

            //If the client subscribed to updates, we add them to the subscriber list for the file
            if (SubscribeToUpdates) Server.LoadedProject.FileDataLookup[FileID].SubscriberIDs.Add(SenderClientID);

            //The cache was accessed recently so we we reset its timer
            Server.LoadedProject.CacheDataLookup[FileID].ResetExpiryTimer();

            Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.FileData(FileGUID));
        }

        /* -PACKET LAYOUT-
         * GUID File ID
         */
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
                CustomLogging.Log("ServerReceive Error: Invalid request file packet received from client: " + SenderClientID.ToString());
                return;
            }

            if (!Server.LoadedProject.GuidFileLookup.ContainsKey(FileGUID))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to retrieve file metadata - File doesn't exist."));
                return;
            }

            Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.FileMetadata(FileGUID));
        }

        /* -PACKET LAYOUT-
         * int Folder ID
         * string File Name
         * int FileType (This int is the equivalent encoding of an enum)
         */ 
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

            //Ensure clients cant create files with untrusted extensions
            if (FileType == CoreFileType.Other)
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to create file - Cannot create file with unknown extension."));
                return;
            }

            //Check validity of new name -> Cant contain certain characters or be too long
            if (!FileManager.IsValidFileName(FileName))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to create file - File name uses invalid characters."));
                return;
            }
            //Check the parent folder exists
            if (!Server.LoadedProject.FolderDataLookup.ContainsKey(FolderID))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to create file - Folder doesn't exist."));
                return;
            }

            DirectoryFolder ParentFolder = Server.LoadedProject.FolderDataLookup[FolderID];
            string NewFileLocation = Server.LoadedProject.BasePath + ParentFolder.LocalPath + FileName + FileManager.FileTypeToExtension(FileType);

            //If there is a file with an identical name already existing, here we loop and add a copy version to the name (as in "NewFile" may become "NewFile (1)") until we hit the loop limit or until a valid name is found
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

                //If we couldn't find a valid name within a certain limit, we send back the client an ErrorNoitification
                if (Failed)
                {
                    Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to create file - File already exists/too many default name files currently exist."));
                    return;
                }
            }

            //Generate new GUID for the file
            Guid FileGUID = Guid.NewGuid();

            //Create Metadata + Object Files
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
                    case CoreFileType.CustomGraphFile:
                        DataFileStream.Write(JsonSerializer.SerializeToUtf8Bytes(new VisualProgrammingFile()));
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

            //Add file to folder hierarchy and lookups
            Server.LoadedProject.GuidFileLookup.Add(FileGUID, NewID);
            DirectoryFile NewFileData = new DirectoryFile(NewID, FileGUID, FinalFileName, FileType, Server.LoadedProject.FolderDataLookup[FolderID]);
            Server.LoadedProject.FileDataLookup.Add(NewID, NewFileData);
            ParentFolder.SubFiles.Add(NewFileData);
            //Load file int cache as usually a new file will be accessed soon
            FileManager.LoadFileIntoCache(NewID);

            //Notify all clients who are listening to updates for the parent folder that the folder has been updated
            Packet SendPacket = ServerSendPacketFunctions.FolderData(FolderID);
            foreach (int SubscriberID in Server.LoadedProject.FolderDataLookup[FolderID].SubscriberIDs)
            {
                Server.SendTCPData(SubscriberID, SendPacket);
            }
        }

        //Identical to above except we add data as well to the new file
        /* -PACKET LAYOUT-
         * int Folder ID
         * string File Name
         * int FileType (This int is the equivalent encoding of an enum)
         * byte[] Data (this is the initial file data)
         */
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
                    //Check if the initial data is a valid JSON Object -> prevent malicious data being sent, such as an executable which the attacker may try to alter the extension of and then run later
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
                        case CoreFileType.CustomGraphFile:
                            VisualProgrammingFile S = JsonSerializer.Deserialize<VisualProgrammingFile>(InitialData);
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

        /* -PACKET LAYOUT-
         * GUID File ID
         * int File Version
         * byte[] New Data
         */
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
                //Check if the initial data is a valid JSON Object
                switch (FileData.FileType)
                {
                    case CoreFileType.Alphabet:
                        Alphabet A = JsonSerializer.Deserialize<Alphabet>(NewData);
                        break;
                    case CoreFileType.Tape:
                        TapeTemplate T = JsonSerializer.Deserialize<TapeTemplate>(NewData);
                        break;
                    case CoreFileType.TransitionFile:
                        TransitionFile Ta = JsonSerializer.Deserialize<TransitionFile>(NewData);
                        break;
                    case CoreFileType.CustomGraphFile:
                        VisualProgrammingFile S = JsonSerializer.Deserialize<VisualProgrammingFile>(NewData);
                        break;
                }                

                File.WriteAllBytes(Server.LoadedProject.BasePath + FileData.GetLocalPath(), NewData);
            }
            catch (Exception E)
            {
                CustomLogging.Log("ServerRecieve Error: UserUpdatedFile - " + E.ToString());
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to update file - Invalid update object sent or faield to write data."));
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

            //Increment file version on successful update
            FileData.Version++;

            Server.LoadedProject.CacheDataLookup[FileID].ResetExpiryTimer();

            //Notify users who are subscribed to this file of an update
            Packet SendPacket = ServerSendPacketFunctions.FileData(FileGUID);
            foreach (int Client in FileData.SubscriberIDs)
            {
                Server.SendTCPData(Client, SendPacket);

                //ServerSendFunctions.SendFileUpdate(Client, FileID);
            }            
        }

        /* -PACKET LAYOUT-
         * GUID File ID
         * string New File Name
         */
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

            //Make sure file with such name doesn't exist already
            if (File.Exists(NewFileLocation))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to rename file - File with this name already exists."));
                return;
            }

            try
            {
                File.WriteAllBytes(NewMetafileLocation, JsonSerializer.SerializeToUtf8Bytes(new ObjectMetadataFile() { FileName = NewFileName, FileGUID = FileGUID, FileType = FileData.FileType }));
                File.WriteAllBytes(NewFileLocation, Server.LoadedProject.CacheDataLookup[FileID].FileData);               
            }
            catch (Exception E)
            {
                CustomLogging.Log("ServerRecieveError: UserRenameFile - " + E.ToString());
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to rename file - Server failed to save the renamed/moved file."));
                return;
            }

            //Delete files with old name
            if (!FileManager.DeleteFileByPath(Server.LoadedProject.BasePath + FileData.GetLocalPath()) || !FileManager.DeleteFileByPath(Server.LoadedProject.BasePath + FileData.GetMetadataLocalPath())) 
            {
                //If deleting the old fiels fails, we notify the client
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to rename file - Server failed to clean old file."));
                //FileManager.DeleteFileByPath(NewFileLocation);
                //FileManager.DeleteFileByPath(NewMetafileLocation);
                return;
            }
            
            FileData.Name = NewFileName;

            //Notify about folder update
            Packet SendPacket = ServerSendPacketFunctions.FolderData(FileData.ParentFolder.ID);
            foreach (int SubscriberID in Server.LoadedProject.FolderDataLookup[FileData.ParentFolder.ID].SubscriberIDs)
            {
                Server.SendTCPData(SubscriberID, SendPacket);
            }

            //Notify file subscribers about rename
            //CHANGE TO METADATA LATER
            SendPacket = ServerSendPacketFunctions.FileData(FileData.GUID);
            foreach (int SubscriberID in Server.LoadedProject.FileDataLookup[FileData.ID].SubscriberIDs)
            {
                Server.SendTCPData(SubscriberID, SendPacket);
            }
        }

        /* -PACKET LAYOUT-
         * GUID File ID
         * int New Folder ID
         */
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
            string NewFileLocation = Server.LoadedProject.BasePath + FolderData.LocalPath + FileData.Name + FileData.Extension;
            string NewMetadataLocation = Server.LoadedProject.BasePath + FolderData.LocalPath + FileData.Name + ".tmeta";

            string OldFileLocation = Server.LoadedProject.BasePath + FileData.GetLocalPath();
            string OldMetadataFileLocation = Server.LoadedProject.BasePath + FileData.GetMetadataLocalPath();

            //Check such file doesn't already exist in the enw folder
            if (File.Exists(NewFileLocation) || File.Exists(NewMetadataLocation))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to move file - File/metadata file with this name already exists."));
                return;
            }

            try
            {
                File.WriteAllBytes(NewFileLocation, Server.LoadedProject.CacheDataLookup[FileID].FileData);
                File.WriteAllBytes(NewMetadataLocation, JsonSerializer.SerializeToUtf8Bytes(FileData.ToMetaDataFile()));
            }
            catch (Exception E)
            {
                CustomLogging.Log("ServerRecieveError: UserMoveFile - " + E.ToString());
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to move file - Server failed to save the renamed/moved file."));
                return;
            }

            if (!FileManager.DeleteFileByPath(OldFileLocation) || !FileManager.DeleteFileByPath(OldMetadataFileLocation))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to move file - Server failed to clean old file. Please manually delete old file."));
               // FileManager.DeleteFileByPath(NewFileLocation);
               // FileManager.DeleteFileByPath(NewMetadataLocation);
                return;
            }

            int PreviousFolderID = FileData.ParentFolder.ID;

            FileData.ParentFolder.SubFiles.Remove(FileData);
            FileData.ParentFolder = FolderData;
            FileData.ParentFolder.SubFiles.Add(FileData);

            Packet SendPacket = ServerSendPacketFunctions.FolderData(PreviousFolderID);
            foreach (int SubscriberID in Server.LoadedProject.FolderDataLookup[PreviousFolderID].SubscriberIDs)
            {
                Server.SendTCPData(SubscriberID, SendPacket);
            }

            SendPacket = ServerSendPacketFunctions.FolderData(NewFolderID);
            foreach (int SubscriberID in Server.LoadedProject.FolderDataLookup[NewFolderID].SubscriberIDs)
            {
                Server.SendTCPData(SubscriberID, SendPacket);
            }

            /*
            SendPacket = ServerSendPacketFunctions.FolderData(FileData.ID);
            foreach (int SubscriberID in Server.LoadedProject.FolderDataLookup[FileData.ID].SubscriberIDs)
            {
                Server.SendTCPData(SubscriberID, SendPacket);
            }
            */

            //Server.SendTCPToAllClients(ServerSendPacketFunctions.FileMoved(FileID));
        }

        /* -PACKET LAYOUT-
         * GUID File ID
         */
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

            //Delete object + Metadata Files
            FileManager.DeleteFileByPath(Server.LoadedProject.BasePath + FileData.GetLocalPath());
            FileManager.DeleteFileByPath(Server.LoadedProject.BasePath + FileData.GetMetadataLocalPath());

            //Clean up folder hierarchy and lookups
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


        /* -PACKET LAYOUT-
         * GUID File ID
         */
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
                CustomLogging.Log("ServerReceive Error: Invalid unsubscribe file packet received from client: " + SenderClientID.ToString());
                return;
            }

            //Ensure file exists
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

            CustomLogging.Log("SERVER INSTRUCTION: User "+SenderClientID.ToString()+" no longer receiving updates to file "+ FileID.ToString()+".");

            //Remove sender from subscriber list
            Server.LoadedProject.FileDataLookup[FileID].SubscriberIDs.Remove(SenderClientID);
        }

        /* -PACKET LAYOUT-
         * int Folder ID
         */
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
                CustomLogging.Log("ServerReceive Error: Invalid unsubscribe folder packet received from client: " + SenderClientID.ToString());
                return;
            }

            if (!Server.LoadedProject.FolderDataLookup.ContainsKey(FolderID))
            {
                Server.SendTCPData(SenderClientID, ServerSendPacketFunctions.ErrorNotification("Failed to unsubscribe from folder - Folder doesn't exist."));
                return;
            }

            CustomLogging.Log("SERVER INSTRUCTION: User " + SenderClientID.ToString() + " no longer recieiving updates to folder " + FolderID.ToString() + ".");

            //Remove sender from subscriber list
            Server.LoadedProject.FolderDataLookup[FolderID].SubscriberIDs.Remove(SenderClientID);
        }

        /* -PACKET LAYOUT-
         * int ParentFolderID
         * string New Folder Name
         */
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
            
            //Create folder on disk
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

            //Update folder hierarchy
            int NewID = FileManager.GetNewFileID();
            DirectoryFolder NewFolderData = new DirectoryFolder(NewID, FinalFolderName, ParentFolderData);
            ParentFolderData.SubFolders.Add(NewFolderData);
            Server.LoadedProject.FolderDataLookup.Add(NewID, NewFolderData);

            //Notify about folder update
            Packet SendPacket = ServerSendPacketFunctions.FolderData(ParentFolderData.ID);
            foreach (int SubscriberID in Server.LoadedProject.FolderDataLookup[ParentFolderData.ID].SubscriberIDs)
            {
                Server.SendTCPData(SubscriberID, SendPacket);
            }

            //Server.SendTCPToAllClients(ServerSendPacketFunctions.FolderCreated(NewID));
        }

        /* -PACKET LAYOUT-
         * int Folder ID
         * string New Folder Name
         */
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

            //Make sure such a folder doesn't already exist
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

            //We have to update every subfolders path, as their paths on disk have changed
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

        /* -PACKET LAYOUT-
         * int Folder ID
         * int TargetFolderID
         */
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

            //Update all subfolders and folder hierarchy
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

            //Notify previous folder and new folder subscribers about respective fodler updates
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

        /* -PACKET LAYOUT-
         * int Folder ID
         */
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

            //Delete folder
            try
            {
                Directory.Delete(Server.LoadedProject.BasePath + FolderData.LocalPath, true);
            }
            catch (Exception E)
            {
                CustomLogging.Log("ServerRecieve Error: UserDeletedFolder - " + E.ToString());
            }

            //We have to go and update our folder hierarchy and remove/unload any files that no longer exist
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
