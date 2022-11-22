using System;
using System.Threading;
using System.Text.Json;
using System.Collections.Generic;
using System.Text;
using TuringCore;
using TuringServer;

namespace TuringTesting
{
    class Program
    {

        static void Main(string[] args)
        {
            CustomLogging.Log("UI: APP RUNNING ON THREAD " + Thread.CurrentThread.ManagedThreadId.ToString());

            /*
            ProjectFile PF = new ProjectFile
            {
                Folders = new List<string>() { "Tapes\\" },
                Files = new List<string>() { "Tapes\\newtape.tape", "Tapes\\TestTape.tape" }
            };

            JsonSerializerOptions Options = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true };
            string jsonString = JsonSerializer.Serialize<ProjectFile>(PF, Options);
            System.IO.File.WriteAllBytes("E:\\Professional Programming\\MAIN\\TestLocation\\TestProject.tproj", Encoding.ASCII.GetBytes(jsonString));
            */

            bool Continue = true;
            while (Continue)
            {
                Client.ProcessPackets();                
                string Option = Console.ReadLine(); 
                string Directory = "E:\\Professional Programming\\MAIN\\TestLocation";

                switch (Option.ToUpper())
                {
                    case ("START"):
                        BackendInterface.StartProjectServer(2, 28104);
                        ClientInstance.ConnectToLocalServer(28104);
                        break;
                    case ("SERVER"):
                        BackendInterface.StartProjectServer(2, 28104);
                        break;
                    case ("CPROJ"):
                        Directory = Console.ReadLine();
                        FileManager.CreateProject("TestProj", Directory, TuringProjectType.NonClassical);
                        break;
                    case ("SAVE"):
                        FileManager.SaveProject();
                        break;
                    case ("CONNECT"):
                        ClientInstance.ConnectToLocalServer(28104);
                        break;
                    case ("DISCONNECT"):
                        ClientInstance.Disconnect();
                        break;
                    case ("SERVERID"):
                        CustomLogging.Log("SERVER THREAD: " + Server.ServerThread.ManagedThreadId.ToString());
                        break;
                    case ("REQFOLDER"):
                        int ReqID = Convert.ToInt32(Console.ReadLine());
                        ClientSendFunctionsWrapper.SendTCPData(ClientSendFunctions.RequestFolderData(ReqID));
                        break;
                    case ("CREATE"):
                        int BaseFolder = Convert.ToInt32(Console.ReadLine());
                        string CreateName = Console.ReadLine();
                        ClientSendFunctionsWrapper.SendTCPData(ClientSendFunctions.CreateFile(BaseFolder, CreateName));
                        break;
                    case ("REQUEST"):
                        int RequestID = Convert.ToInt32(Console.ReadLine());
                        ClientSendFunctionsWrapper.SendTCPData(ClientSendFunctions.RequestFile(RequestID, true));
                        break;
                    case ("RENAME"):
                        int RenameID = Convert.ToInt32(Console.ReadLine());
                        string NewName = Console.ReadLine();
                        ClientSendFunctionsWrapper.SendTCPData(ClientSendFunctions.RenameFile(RenameID, NewName));
                        break;
                    case ("MOVE"):
                        int MoveID = Convert.ToInt32(Console.ReadLine());
                        int MoveFolderID = Convert.ToInt32(Console.ReadLine());
                        ClientSendFunctionsWrapper.SendTCPData(ClientSendFunctions.MoveFile(MoveID, MoveFolderID));
                        break;
                    case ("EDIT"):
                        int EditID = Convert.ToInt32(Console.ReadLine());
                        int Version = Convert.ToInt32(Console.ReadLine());
                        string NewContents = Console.ReadLine();
                        ClientSendFunctionsWrapper.SendTCPData(ClientSendFunctions.UpdateFile(EditID, Version, NewContents));
                        break;
                    case ("DELETE"):
                        int DeleteID = Convert.ToInt32(Console.ReadLine());
                        ClientSendFunctionsWrapper.SendTCPData(ClientSendFunctions.DeleteFile(DeleteID));
                        break;
                    case ("UNSUB"):
                        int UnsubID = Convert.ToInt32(Console.ReadLine());
                        ClientSendFunctionsWrapper.SendTCPData(ClientSendFunctions.UnsubscribeFromFileUpdates(UnsubID));
                        break;
                    case ("CFOLDER"):
                        int NBaseFolder = Convert.ToInt32(Console.ReadLine());
                        string Name = Console.ReadLine();
                        ClientSendFunctionsWrapper.SendTCPData(ClientSendFunctions.CreateFolder(NBaseFolder, Name));
                        break;
                    case ("RFOLDER"):
                        int RFolder = Convert.ToInt32(Console.ReadLine());
                        string Rename = Console.ReadLine();
                        ClientSendFunctionsWrapper.SendTCPData(ClientSendFunctions.RenameFolder(RFolder, Rename));
                        break;
                    case ("MFOLDER"):
                        int MFolder = Convert.ToInt32(Console.ReadLine()); 
                        int MTFolder = Convert.ToInt32(Console.ReadLine());
                        ClientSendFunctionsWrapper.SendTCPData(ClientSendFunctions.MoveFolder(MFolder, MTFolder));
                        break;
                    case ("DFOLDER"):
                        int DFolder = Convert.ToInt32(Console.ReadLine());
                        ClientSendFunctionsWrapper.SendTCPData(ClientSendFunctions.DeleteFolder(DFolder));
                        break;
                    case ("KILL CLIENT"):
                        Server.Clients[0].DisconnectClientFromServer();
                        break;
                    case ("BREAKPOINT"):
                        break;
                    case ("STOP"):
                        BackendInterface.CloseProject();
                        break;
                    default:
                        break;
                }
                
            }
        }

    }
}
