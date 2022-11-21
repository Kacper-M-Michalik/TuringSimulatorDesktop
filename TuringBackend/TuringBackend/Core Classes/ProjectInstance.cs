using System;
using System.Collections.Generic;
using TuringBackend.Networking;
using TuringBackend.Logging;

namespace TuringBackend
{
    public static class ProjectInstance
    {
        public static Project LoadedProject;

        public static bool StartProjectServer(string Location, int SetMaxClients, int SetPort)
        {
            //add safety check if already started later
            //this will do for now
            if (LoadedProject != null) throw new Exception("Exisitng project already loaded.");

            LoadedProject = FileManager.LoadProjectFile(Location);

            if (LoadedProject != null)
            {
                CustomConsole.Log("Loader Successful");
                Server.StartServer(SetMaxClients, SetPort);
                return true;
            }
            else
            {
                CustomConsole.Log("Loader Unsuccessful");
            }

            return false;
        }

        public static void CloseProject()
        {
            Server.CloseServer();
        }
    }
}
