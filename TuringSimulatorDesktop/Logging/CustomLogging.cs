using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.Debugging
{
    public static class CustomLogging
    {
        static string LogFilePath = "";
        static FileStream? LogStream;
        static CustomLogging()
        {
            //Create log file in ApplicationData area
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
            //Only full logs (not writes) get written to the log file 
            Debug.WriteLine(Message);
            byte[] Data = Encoding.ASCII.GetBytes(Message + "\n");
            LogStream?.Write(Data, 0, Data.Length);
            LogStream?.Flush();
        }

        public static void Write(string Message)
        {
            Debug.Write(Message);
            byte[] Data = Encoding.ASCII.GetBytes(Message);
            LogStream?.Write(Data, 0, Data.Length);
            LogStream?.Flush();
        }
    }
}
