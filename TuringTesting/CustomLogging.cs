using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringTesting
{
    public static class CustomLogging
    {
        public static void Log(string Message)
        {
            Console.WriteLine(Message);
            Debug.WriteLine(Message);
        }

        public static void Write(string Message)
        {
            Console.Write(Message);
            Debug.Write(Message);
        }
    }
}
