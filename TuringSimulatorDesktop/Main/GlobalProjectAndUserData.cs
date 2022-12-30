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
        public static string UserDataPath;
        public static LocalUserData UserData;    

        public static void SaveUserData()
        {
            JsonSerializerOptions Options = new JsonSerializerOptions() { WriteIndented = false };
            string SaveJson = JsonSerializer.Serialize(UserData, Options);

            try
            {
                File.WriteAllBytes(UserDataPath, Encoding.Unicode.GetBytes(SaveJson));
            }
            catch (Exception E)
            {
                CustomLogging.Log("UI Error: Failed to write LocalUserData File - " + E.ToString());
                //add something here to notify user and stop program
            }
        }

        public static void LoadUserData(string Path)
        {
            string SaveFileJson = File.ReadAllText(Path, Encoding.Unicode);

            try
            {
                UserData = JsonSerializer.Deserialize<LocalUserData>(SaveFileJson);
                UserDataPath = Path;
            }
            catch (Exception E)
            {
                CustomLogging.Log("UI Error: Failed to deserialize LocalUserData File" + E.ToString());
            }

        }
    }
}
