﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using TuringCore;
using TuringSimulatorDesktop.Debugging;
using TuringSimulatorDesktop.Files;

namespace TuringSimulatorDesktop
{
    public static class GlobalProjectAndUserData
    {
        public static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions() { WriteIndented = true };

        public static string UserDataPath;
        public static LocalUserData UserData;
        public static ConnectedProjectData ProjectData;

        //Add newly opened file to list
        public static void UpdateRecentlyOpenedFile(string FileDirectory)
        {
            for (int i = 0; i < UserData.RecentlyAccessedFiles.Count; i++)
            {
                if (UserData.RecentlyAccessedFiles[i].FullPath == FileDirectory)
                {
                    UserData.RecentlyAccessedFiles[i].LastAccessed = DateTime.Now;
                    SaveUserData();
                    return;
                }
            }

            UserData.RecentlyAccessedFiles.Add(new FileInfoWrapper(ProjectData.ProjectName, FileDirectory, DateTime.Now));
            SaveUserData();
        }

        //Save local user data
        public static void SaveUserData()
        {
            try
            {
                File.WriteAllBytes(UserDataPath, JsonSerializer.SerializeToUtf8Bytes(UserData, JsonOptions));
            }
            catch (Exception E)
            {
                CustomLogging.Log("UI Error: Failed to write LocalUserData File - " + E.ToString());
                //add something here to notify user and stop program
            }
        }

        //Load local user data
        public static bool LoadUserData(string Path)
        {
            try
            {
                UserData = JsonSerializer.Deserialize<LocalUserData>(File.ReadAllBytes(Path));
                UserDataPath = Path;
                return true;
            }
            catch (Exception E)
            {
                CustomLogging.Log("UI Error: Failed to deserialize LocalUserData File" + E.ToString());
                return false;
            }

        }
    }

    public class ConnectedProjectData
    {
        public string ProjectName;

        public ConnectedProjectData(string projectName)
        {
            ProjectName = projectName;
        }
    }
}
