using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using TuringCore;

namespace TuringSimulatorDesktop
{
    public static class GlobalProjectAndUserData
    {
        static readonly JsonSerializerOptions Options = new JsonSerializerOptions() { WriteIndented = false };

        public static string UserDataPath;
        public static LocalUserData UserData;
        public static ConnectedProjectData ProjectData;

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

        public static void SaveUserData()
        {
            //string SaveJson = JsonSerializer.Serialize(UserData, Options);

            try
            {
                File.WriteAllBytes(UserDataPath, JsonSerializer.SerializeToUtf8Bytes(UserData, Options));
            }
            catch (Exception E)
            {
                CustomLogging.Log("UI Error: Failed to write LocalUserData File - " + E.ToString());
                //add something here to notify user and stop program
            }
        }

        public static void LoadUserData(string Path)
        {
            //string SaveFileJson = File.ReadAllText(Path, Encoding.Unicode);

            try
            {
                UserData = JsonSerializer.Deserialize<LocalUserData>(File.ReadAllBytes(Path));
                UserDataPath = Path;
            }
            catch (Exception E)
            {
                CustomLogging.Log("UI Error: Failed to deserialize LocalUserData File" + E.ToString());
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
