﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace TuringBackend.Logging
{
    public static class CustomConsole
    {
        public delegate void LogMethod(string Message);
        public static LogMethod LogPointer;
        public static LogMethod WritePointer;

        static string LogFilePath;
        static FileStream LogStream;

        static CustomConsole()
        {
            #if DEBUG
                LogPointer = delegate (string Message) { Debug.WriteLine(Message); };
                WritePointer = delegate (string Message) { Debug.Write(Message); };
            #endif

            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "Turing Machine - Desktop");
            LogFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "Turing Machine - Desktop" + Path.DirectorySeparatorChar + "Log--" + DateTime.Now.ToString("yyyy-MM-dd--HH-mm") + ".txt";

            try
            {
                LogStream = File.Create(LogFilePath);
            }
            catch (Exception E)
            {
                LogPointer(E.ToString());
            }
        }

        public static void Log(string Message)
        {
            if (LogPointer != null) LogPointer(Message);
            byte[] Data = Encoding.ASCII.GetBytes(Message + "\n");
            LogStream?.Write(Data, 0, Data.Length);
            LogStream?.Flush();
        }

        public static void Write(string Message)
        {
            if (WritePointer != null) WritePointer(Message);
            byte[] Data = Encoding.ASCII.GetBytes(Message);
            LogStream?.Write(Data, 0, Data.Length);
            LogStream?.Flush();
        }

    }
}
