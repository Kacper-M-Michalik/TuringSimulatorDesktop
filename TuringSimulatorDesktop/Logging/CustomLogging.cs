using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop
{
    public static class CustomLogging
    {
        static string LogFilePath = "";
        static FileStream? LogStream;
        static CustomLogging()
        {
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "Turing Machine - Desktop");
            LogFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "Turing Machine - Desktop" + Path.DirectorySeparatorChar + "Log--" + DateTime.Now.ToString("yyyy-MM-dd--HH-mm") + ".txt";

            try
            {
                //if (File.Exists(LogFilePath)) File.WriteAllText(LogFilePath, "");
                LogStream = File.Create(LogFilePath);
            }
            catch { }
        }
        public static void Log(string Message)
        {
            Debug.WriteLine(Message);
            byte[] Data = Encoding.ASCII.GetBytes(Message + "\n");
            LogStream?.Write(Data, 0, Data.Length);
            LogStream?.Flush();
        }

        public static void Write(string Message)
        {
            Debug.Write(Message);
        }
    }
}
