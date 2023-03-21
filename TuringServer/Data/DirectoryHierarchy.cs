using System;
using System.Collections.Generic;
using System.IO;
using TuringCore;

namespace TuringServer
{
    public class DirectoryFolder
    {
        public int ID;
        public HashSet<int> SubscriberIDs;

        public string Name;
        public DirectoryFolder ParentFolder;
        public string LocalPath;

        public List<DirectoryFolder> SubFolders;
        public List<DirectoryFile> SubFiles;

        public DirectoryFolder(int SetID, string SetName, DirectoryFolder SetParentFolder)
        {
            ID = SetID;
            SubscriberIDs = new HashSet<int>();

            Name = SetName;
            ParentFolder = SetParentFolder;
            UpdatePath();

            SubFolders = new List<DirectoryFolder>();
            SubFiles = new List<DirectoryFile>();
        }

        public void UpdatePath()
        {
            LocalPath = ParentFolder == null ? Name + Path.DirectorySeparatorChar : ParentFolder.LocalPath + Name + Path.DirectorySeparatorChar;
        }
    }

    public class DirectoryFile
    {
        public int ID;
        public Guid GUID;
        public int Version;
        public HashSet<int> SubscriberIDs;

        public string Name;
        public string Extension;
        public CoreFileType FileType;
        public DirectoryFolder ParentFolder;

        public DirectoryFile(int SetID, Guid SetGUID, string SetName, string SetExtension, DirectoryFolder SetParentFolder)
        {
            ID = SetID;
            GUID = SetGUID;
            Name = SetName;
            Extension = SetExtension;
            FileType = FileManager.ExtensionToFileType(Extension);

            Version = 1;
            ParentFolder = SetParentFolder;
            SubscriberIDs = new HashSet<int>();
        }
        public DirectoryFile(int SetID, Guid SetGUID, string SetName, CoreFileType SetType, DirectoryFolder SetParentFolder)
        {
            ID = SetID;
            GUID = SetGUID;
            Name = SetName;

            FileType = SetType;
            Extension = FileManager.FileTypeToExtension(FileType);

            Version = 1;
            ParentFolder = SetParentFolder;
            SubscriberIDs = new HashSet<int>();
        }

        public string GetLocalPath()
        {
            return ParentFolder.LocalPath + Name + Extension;
        }

        public string GetMetadataLocalPath()
        {
            return ParentFolder.LocalPath + Name + ".tmeta";
        }

        public ObjectMetadataFile ToMetaDataFile()
        {
            return new ObjectMetadataFile() { FileGUID = GUID, FileName = Name, FileType = FileType };
        }
    }

}
