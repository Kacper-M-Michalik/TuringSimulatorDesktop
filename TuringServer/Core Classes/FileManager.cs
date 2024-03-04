using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.Json;
using TuringCore.Files;
using TuringCore;
using TuringServer.Logging;
using TuringServer.Data;
using TuringServer.ServerSide;

namespace TuringServer
{
    public static class FileManager
    {
        static readonly JsonSerializerOptions Options = new JsonSerializerOptions() { WriteIndented = true };
        
        public static bool IsValidFileName(string FileName)
        {
            for (int i = 0; i < FileName.Length; i++)
            {
                if (!char.IsAscii(FileName[i])) return false;
            }

            //https://docs.microsoft.com/en-gb/windows/win32/fileio/naming-a-file?redirectedfrom=MSDN for seeing banned characters
            if (FileName.Contains("<") || FileName.Contains(">") || FileName.Contains(":") || FileName.Contains("\"") || FileName.Contains("/") || FileName.Contains("\\") || FileName.Contains("|") || FileName.Contains("?") || FileName.Contains("*") || FileName.Length > 255)
            {                
                return false;
            }

            return true;
        }
        
        public static string GetFileNameFromPath(string FilePath)
        {
            int Index = FilePath.LastIndexOf(Path.DirectorySeparatorChar) + 1;

            int HasType = FilePath.LastIndexOf('.');
            if (HasType != -1) return FilePath.Substring(Index, FilePath.LastIndexOf('.') - Index);
            else return FilePath.Substring(Index);
        }

        public static string GetFileExtensionFromPath(string FilePath)
        {
            return FilePath.Substring(FilePath.LastIndexOf('.'));
        }        

        public static CoreFileType ExtensionToFileType(string Extension)
        {
            switch (Extension)
            {
                case ".alph":
                    return CoreFileType.Alphabet;
                case ".tape":
                    return CoreFileType.Tape;
                case ".trt":
                    return CoreFileType.TransitionFile;
                case ".cgr":
                    return CoreFileType.CustomGraphFile;
                default:
                    return CoreFileType.Other;
            }
        }

        public static string FileTypeToExtension(CoreFileType FileType)
        {
            switch (FileType)
            {
                case CoreFileType.Alphabet:
                    return ".alph";
                case CoreFileType.Tape:
                    return ".tape";
                case CoreFileType.TransitionFile:
                    return ".trt";
                case CoreFileType.CustomGraphFile:
                    return ".cgr";
                default:
                    return "";
            }
        }

        //ID 0 reserved for BaseFolder
        //These ID's are different than GUIS used for files, they are reallocated each boot up of a project, and used to assignID's to folders, they are also given to files for backwards compatibility, as during prototyping files used int ID's
        static int NextID = 1;
        public static int GetNewFileID()
        {
            return NextID++;
        }

        public static ProjectData LoadProjectFile(string FilePath)
        {
            string CorrectPath = "";
            if (FilePath.Substring(FilePath.Length-6) == ".tproj")
            {
                CorrectPath = FilePath;
            }
            else
            {
                //Search for tproj file
                string[] AllFiles = Directory.GetFiles(FilePath);
                for (int i = 0; i < AllFiles.Length; i++)
                {
                    if (AllFiles[i].Substring(AllFiles[i].Length - 6) == ".tproj")
                    {
                        CorrectPath = AllFiles[i];
                        i = AllFiles.Length;
                    }
                }
            }

            if (CorrectPath == "") return null;

            ProjectSaveFile SaveFile;

            //Try to read Project Data File
            try
            {
                SaveFile = JsonSerializer.Deserialize<ProjectSaveFile>(File.ReadAllBytes(CorrectPath));
            }
            catch (Exception E)
            {
                CustomLogging.Log("File Manager Error - Load Project - " + E.ToString());
                return null;
            }

            //Get the parent path of the entire project
            string ProjectBasePath = Directory.GetParent(CorrectPath).ToString() + Path.DirectorySeparatorChar;

            //Generate our lookups
            Dictionary<int, CacheFileData> NewCacheDataLookup = new Dictionary<int, CacheFileData>();
            Dictionary<int, DirectoryFile> NewFileDataLookup = new Dictionary<int, DirectoryFile>();
            DirectoryFolder BaseFolder = new DirectoryFolder(0, SaveFile.BaseFolder, null);
            Dictionary<int, DirectoryFolder> NewFolderDataLookup = new Dictionary<int, DirectoryFolder>() { { 0, BaseFolder } };
            Dictionary<Guid, int> NewGuidFileLookup = new Dictionary<Guid, int>();

            Queue<(string, DirectoryFolder)> FolderQueue = new Queue<(string, DirectoryFolder)>();
            FolderQueue.Enqueue((ProjectBasePath + SaveFile.BaseFolder, null));
            
            //Generate file/folder hierarchy from files/folders on disk
            while (FolderQueue.Count != 0)
            {
                (string, DirectoryFolder) FolderInfo = FolderQueue.Dequeue();

                DirectoryFolder NewFolder;
                if (FolderInfo.Item2 == null)
                {
                    NewFolder = BaseFolder;
                }
                else
                {
                    NewFolder = new DirectoryFolder(GetNewFileID(), GetFileNameFromPath(FolderInfo.Item1), FolderInfo.Item2);
                    NewFolderDataLookup.Add(NewFolder.ID, NewFolder);
                    FolderInfo.Item2.SubFolders.Add(NewFolder);
                }                

                string[] Files = Directory.GetFiles(FolderInfo.Item1);
                for (int i = 0; i < Files.Length; i++)
                {
                    ObjectMetadataFile MetadataFile = null;

                    //Check if a file is a metadata file
                    if (Files[i].EndsWith("tmeta"))
                    {
                        bool Failed;

                        byte[] Data = File.ReadAllBytes(Files[i]);
                        try
                        {
                            MetadataFile = JsonSerializer.Deserialize<ObjectMetadataFile>(Data);

                            //Check if equivalent object file exists, if it doesn't there is an error, and so we fail to load it in
                            Failed = !File.Exists(FolderInfo.Item1 + Path.DirectorySeparatorChar + MetadataFile.FileName + FileManager.FileTypeToExtension(MetadataFile.FileType));
                                      
                            //Verify JSON objects are valid
                            if (!Failed)
                            {
                                switch (MetadataFile.FileType)
                                {                                                            
                                    case CoreFileType.Alphabet:
                                        JsonSerializer.Deserialize<Alphabet>(File.ReadAllBytes(FolderInfo.Item1 + Path.DirectorySeparatorChar + MetadataFile.FileName + FileManager.FileTypeToExtension(MetadataFile.FileType)));
                                        break;
                                    case CoreFileType.Tape:
                                        JsonSerializer.Deserialize<TapeTemplate>(File.ReadAllBytes(FolderInfo.Item1 + Path.DirectorySeparatorChar + MetadataFile.FileName + FileManager.FileTypeToExtension(MetadataFile.FileType)));
                                        break;
                                    case CoreFileType.TransitionFile:
                                        JsonSerializer.Deserialize<TransitionFile>(File.ReadAllBytes(FolderInfo.Item1 + Path.DirectorySeparatorChar + MetadataFile.FileName + FileManager.FileTypeToExtension(MetadataFile.FileType)));
                                        break;
                                    case CoreFileType.CustomGraphFile:
                                        JsonSerializer.Deserialize<VisualProgrammingFile>(File.ReadAllBytes(FolderInfo.Item1 + Path.DirectorySeparatorChar + MetadataFile.FileName + FileManager.FileTypeToExtension(MetadataFile.FileType)));
                                        break;
                                    default:
                                        Failed = true;
                                        break;
                                }
                            }
                        }
                        catch
                        {
                            Failed = true;
                        }

                        if (!Failed)
                        {
                            int ID = GetNewFileID();

                            NewGuidFileLookup.Add(MetadataFile.FileGUID, ID);
                            DirectoryFile NewFileData = new DirectoryFile(ID, MetadataFile.FileGUID, MetadataFile.FileName, MetadataFile.FileType, NewFolder);
                            NewFileDataLookup.Add(NewFileData.ID, NewFileData);
                            NewFolder.SubFiles.Add(NewFileData);
                        }
                        else
                        {
                            CustomLogging.Log("Failed to load in file: " + Files[i]);
                        }
                    }
                }

                string[] Folders = Directory.GetDirectories(FolderInfo.Item1); 
                for (int i = 0; i < Folders.Length; i++)
                {
                    FolderQueue.Enqueue((Folders[i], NewFolder));
                }               
                
            }

            //Return ProjectData Object
            return new ProjectData()
            {
                ProjectName = SaveFile.ProjectName,
                TuringTypeRule = SaveFile.TuringTypeRule,
                BaseDirectoryFolder = BaseFolder,

                BasePath = ProjectBasePath,
                ProjectFilePath = CorrectPath,

                CacheDataLookup = NewCacheDataLookup,
                FileDataLookup = NewFileDataLookup,
                FolderDataLookup = NewFolderDataLookup,
                GuidFileLookup = NewGuidFileLookup                
            };            
            
        }

        public static bool LoadFileIntoCache(int FileID)
        {
            //Check file exists
            if (Server.LoadedProject.FileDataLookup.ContainsKey(FileID))
            {
                //Check if file isn't already cached
                if (Server.LoadedProject.CacheDataLookup.ContainsKey(FileID))
                {
                    Server.LoadedProject.CacheDataLookup[FileID].ResetExpiryTimer();
                    return true;
                }
                else
                {
                    //Load file and add to cache
                    try
                    {
                        Server.LoadedProject.CacheDataLookup.Add(FileID, new CacheFileData(
                                File.ReadAllBytes(Server.LoadedProject.BasePath + Server.LoadedProject.FileDataLookup[FileID].GetLocalPath())
                            ));
                        return true;
                    }
                    catch (Exception E)
                    {
                        CustomLogging.Log("File Manager Error: LoadFileIntoCache - " + E.ToString());
                    }
                }       
            }

            return false;
        }
        
        //Wrapper functions that handles errors when delteign a file
        public static bool DeleteFileByPath(string FilePath)
        {
            try
            {
                File.Delete(FilePath);
            }
            catch (Exception E)
            {
                CustomLogging.Log("File Manager Error: DeleteFile - " + E.ToString());
                return false;
            }

            return true;
        }

        //Create appropriate folder structure and generates Project Data File in specified location
        public static CreateProjectReturnData CreateProject(string Name, string ProjectDirectory, TuringProjectType RuleType)
        {
            string BaseFolder = ProjectDirectory + Path.DirectorySeparatorChar + Name;
            string ProjectPath = BaseFolder + Path.DirectorySeparatorChar + Name + ".tproj";
            try
            {
                Directory.CreateDirectory(BaseFolder);
                File.WriteAllBytes(ProjectPath, JsonSerializer.SerializeToUtf8Bytes(new ProjectSaveFile(Name, Name + "Data", RuleType), Options));
                Directory.CreateDirectory(BaseFolder + Path.DirectorySeparatorChar + Name + "Data");
            }
            catch (Exception E)
            {
                if (File.Exists(ProjectPath)) DeleteFileByPath(ProjectPath);
                CustomLogging.Log("File Manager Error: CreateProject - " + E.ToString());
                return new CreateProjectReturnData(false, "");
            }
            return new CreateProjectReturnData(true, ProjectPath);
        }

        //Saves changes made to a Project Data File
        public static bool SaveProject()
        {
            try
            {
                File.WriteAllBytes(Server.LoadedProject.ProjectFilePath, JsonSerializer.SerializeToUtf8Bytes(new ProjectSaveFile(Server.LoadedProject.ProjectName, Server.LoadedProject.BaseDirectoryFolder.Name, Server.LoadedProject.TuringTypeRule), Options));
            }            
            catch (Exception E)
            {
                CustomLogging.Log("File Manager Error: SaveProject - " + E.ToString());
                return false;
            }
            return true;
        }
    }
}
