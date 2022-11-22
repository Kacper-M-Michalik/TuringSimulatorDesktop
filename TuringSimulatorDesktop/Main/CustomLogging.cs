using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop
{
    public static class CustomLogging
    {
        public static void Log(string Message)
        {
            Debug.WriteLine(Message);
        }

        public static void Write(string Message)
        {
            Debug.Write(Message);
        }
    }
}
