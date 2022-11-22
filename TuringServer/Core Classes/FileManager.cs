using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.Json;
using TuringCore.SaveFiles;
using TuringCore;
using TuringServer.Logging;

namespace TuringServer
{
    public static class FileManager
    {
        public static bool IsValidFileName(string FileName)
        {
            //Maybe rewrite using regex?
            //https://docs.microsoft.com/en-gb/windows/win32/fileio/naming-a-file?redirectedfrom=MSDN for seeing banned characters
            if (FileName.Contains("<") || FileName.Contains(">") || FileName.Contains(":") || FileName.Contains("\"") || FileName.Contains("/") || FileName.Contains("\\") || FileName.Contains("|") || FileName.Contains("?") || FileName.Contains("*"))
            {                
                return false;
            }

            return true;
        }
        
        public static string GetFileNameFromPath(string FilePath)
        {
            int Index = FilePath.LastIndexOf(Path.DirectorySeparatorChar) + 1;
            return FilePath.Substring(Index);
        } 

        //ID 0 reserved for BaseFolder
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

            string SaveFileJson = File.ReadAllText(CorrectPath);
            ProjectSaveFile SaveFile;

            try
            {
                SaveFile = JsonSerializer.Deserialize<ProjectSaveFile>(SaveFileJson);
            }
            catch (Exception E)
            {
                CustomConsole.Log("File Manager Error - Load Project - " + E.ToString());
                return null;
            }

            string ProjectBasePath = Directory.GetParent(CorrectPath).ToString() + Path.DirectorySeparatorChar;

            Dictionary<int, CacheFileData> NewCacheDataLookup = new Dictionary<int, CacheFileData>();
            Dictionary<int, DirectoryFile> NewFileDataLookup = new Dictionary<int, DirectoryFile>();
            DirectoryFolder BaseFolder = new DirectoryFolder(0, SaveFile.BaseFolder, null);
            Dictionary<int, DirectoryFolder> NewFolderDataLookup = new Dictionary<int, DirectoryFolder>() { { 0, BaseFolder } };

            Queue<(string, DirectoryFolder)> FolderQueue = new Queue<(string, DirectoryFolder)>();
            FolderQueue.Enqueue((ProjectBasePath + SaveFile.BaseFolder, null));
            
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
                    DirectoryFile NewFileData = new DirectoryFile(GetNewFileID(), GetFileNameFromPath(Files[i]), NewFolder);
                    NewFileDataLookup.Add(NewFileData.ID, NewFileData);
                    NewFolder.SubFiles.Add(NewFileData);
                }

                string[] Folders = Directory.GetDirectories(FolderInfo.Item1); 
                for (int i = 0; i < Folders.Length; i++)
                {
                    FolderQueue.Enqueue((Folders[i], NewFolder));
                }
            }

            return new ProjectData()
            {
                ProjectName = SaveFile.ProjectName,
                TuringTypeRule = SaveFile.TuringTypeRule,
                BaseDirectoryFolder = BaseFolder,

                BasePath = ProjectBasePath,
                ProjectFilePath = CorrectPath,

                CacheDataLookup = NewCacheDataLookup,
                FileDataLookup = NewFileDataLookup,
                FolderDataLookup = NewFolderDataLookup
            };            

        }

        public static bool LoadFileIntoCache(int FileID)
        {
            if (Server.LoadedProject.FileDataLookup.ContainsKey(FileID))
            {
                if (Server.LoadedProject.CacheDataLookup.ContainsKey(FileID))
                {
                    Server.LoadedProject.CacheDataLookup[FileID].ResetExpiryTimer();
                    return true;
                }
                else
                {
                    try
                    {
                        Server.LoadedProject.CacheDataLookup.Add(FileID, new CacheFileData(
                                File.ReadAllBytes(Server.LoadedProject.BasePath + Server.LoadedProject.FileDataLookup[FileID].GetLocalPath())
                            ));
                        return true;
                    }
                    catch (Exception E)
                    {
                        CustomConsole.Log("File Manager Error: LoadFileIntoCache - " + E.ToString());
                    }
                }       
            }

            return false;
        }
        
        public static bool DeleteFileByPath(string FilePath)
        {
            try
            {
                File.Delete(FilePath);
            }
            catch (Exception E)
            {
                CustomConsole.Log("File Manager Error: DeleteFile - " + E.ToString());
                return false;
            }

            return true;
        }

        public static bool CreateProject(string Name, string ProjectDirectory, TuringProjectType RuleType)
        {
            JsonSerializerOptions Options = new JsonSerializerOptions() { WriteIndented = true };
            string SaveJson = JsonSerializer.Serialize(new ProjectSaveFile(Name, Name + "Data", RuleType), Options);
            string ProjectPath = ProjectDirectory + Path.DirectorySeparatorChar + Name + ".tproj";
            try
            {
                File.WriteAllBytes(ProjectPath, Encoding.ASCII.GetBytes(SaveJson));
                Directory.CreateDirectory(ProjectDirectory + Path.DirectorySeparatorChar + Name + "Data");
            }
            catch (Exception E)
            {
                if (File.Exists(ProjectPath)) DeleteFileByPath(ProjectPath);
                CustomConsole.Log("File Manager Error: CreateProject - " + E.ToString());
                return false;
            }
            return true;
        }

        //is there a point of this?
        public static bool SaveProject()
        {
            JsonSerializerOptions Options = new JsonSerializerOptions() { WriteIndented = true };
            string SaveJson = JsonSerializer.Serialize(new ProjectSaveFile(Server.LoadedProject.ProjectName, Server.LoadedProject.BaseDirectoryFolder.Name, Server.LoadedProject.TuringTypeRule), Options);

            try
            {
                File.WriteAllBytes(Server.LoadedProject.ProjectFilePath, Encoding.ASCII.GetBytes(SaveJson));
            }            
            catch (Exception E)
            {
                CustomConsole.Log("File Manager Error: SaveProject - " + E.ToString());
                return false;
            }
            return true;
        }
    }
}
