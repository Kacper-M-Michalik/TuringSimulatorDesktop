using System;
using System.Threading;
using System.Text.Json;
using System.Collections.Generic;
using System.Text;
using TuringCore;
using TuringServer;
using System.Net;

namespace TuringTesting
{
    class Program
    {
        static string Directory;
        static void Main(string[] args)
        {
            CustomLogging.Log("UI: Console on thread: " + Thread.CurrentThread.ManagedThreadId.ToString());
            UIEventManager.ClientFailedConnectingDelegate += FailedConnection;

            Console.WriteLine("Change default Directory? (Type new one or leave empty if not)");
            Directory = Console.ReadLine();
            if (Directory == "") Directory = "E:\\Professional Programming\\MAIN\\TestLocation";

            bool Continue = true;
            while (Continue)
            {                
                if (Console.KeyAvailable)
                {
                    //ConsoleKeyInfo key = Console.ReadKey(true);
                    Console.WriteLine("Finish command to start logging!");
                    string Option = Console.ReadLine();

                    switch (Option.ToUpper())
                    {
                        case ("HELP"):
                            Console.WriteLine("QUICK - Starts server, loads project from last specified directory and connects to it\n" +
                                "SERVER - Starts server\n" +
                                "STOP SERVER - Stops server\n" +
                                "CL - Connectes to local server\n" +
                                "CONNECT - Takes IP and connects to that server\n" +
                                "DISCONNECT - Disconnects client from server\n" +
                                "KILLFIRSTCLIENT - Disconnects first client from server\n" +
                                "LOGSTATUS - Sets this console instance as the servers desingated status log reciever\n" +
                                "CPROJ - Takes directory and creates test project\n" +
                                "LPROJ - Takes directory and tells serevr to load project\n" +
                                "OTHERS ARE ALL SERVER FUNCS");
                            break;
                        case ("QUICK"):
                            BackendInterface.StartProjectServer(2, 28104);
                            Client.ConnectToServer(IPAddress.Parse("127.0.0.1"), 28104);
                            UIEventManager.ClientSuccessConnectingDelegate = ConnectedQuick;
                            break;
                        case ("SERVER"):
                            BackendInterface.StartProjectServer(2, 28104);
                            break;
                        case ("STOP SERVER"):
                            BackendInterface.CloseProject();
                            break;
                        case ("CL"):
                            Client.ConnectToServer(IPAddress.Parse("127.0.0.1"), 28104);
                            UIEventManager.ClientSuccessConnectingDelegate = Connected;
                            break;
                        case ("CONNECT"):
                            string IP = Console.ReadLine();
                            Client.ConnectToServer(IPAddress.Parse(IP), 28104);
                            UIEventManager.ClientSuccessConnectingDelegate = Connected;
                            break;
                        case ("DISCONNECT"):
                            Client.Disconnect();
                            break;
                        case ("KILLFIRSTCLIENT"):
                            Server.Clients[0].DisconnectClientFromServer();
                            break;
                        case ("LOGSTATUS"):
                            Client.SendTCPData(ClientSendPacketFunctions.RequestLogReceiverStatus());
                            break;
                        case ("CPROJ"):
                            Directory = Console.ReadLine();
                            FileManager.CreateProject("TestProj", Directory, TuringProjectType.NonClassical);
                            break;
                        case ("LPROJ"):
                            Directory = Console.ReadLine();
                            Client.SendTCPData(ClientSendPacketFunctions.LoadProject(Directory));
                            break;
                        case ("SAVE"):
                            //maybe unsafe
                            FileManager.SaveProject();
                            break;
                        case ("SERVERID"):
                            CustomLogging.Log("SERVER THREAD: " + Server.ServerThread.ManagedThreadId.ToString());
                            break;
                        case ("REQFOLDER"):
                            int ReqID = Convert.ToInt32(Console.ReadLine());
                            Client.SendTCPData(ClientSendPacketFunctions.RequestFolderData(ReqID, false));
                            break;
                        case ("CREATE"):
                            int BaseFolder = Convert.ToInt32(Console.ReadLine());
                            string CreateName = Console.ReadLine();
                            Client.SendTCPData(ClientSendPacketFunctions.CreateFile(BaseFolder, CreateName, CreateFileType.TransitionFile));
                            break;
                        case ("REQUEST"):
                            int RequestID = Convert.ToInt32(Console.ReadLine());
                            Client.SendTCPData(ClientSendPacketFunctions.RequestFile(RequestID, true));
                            break;
                        case ("RENAME"):
                            int RenameID = Convert.ToInt32(Console.ReadLine());
                            string NewName = Console.ReadLine();
                            Client.SendTCPData(ClientSendPacketFunctions.RenameFile(RenameID, NewName));
                            break;
                        case ("MOVE"):
                            int MoveID = Convert.ToInt32(Console.ReadLine());
                            int MoveFolderID = Convert.ToInt32(Console.ReadLine());
                            Client.SendTCPData(ClientSendPacketFunctions.MoveFile(MoveID, MoveFolderID));
                            break;
                        case ("EDIT"):
                            int EditID = Convert.ToInt32(Console.ReadLine());
                            int Version = Convert.ToInt32(Console.ReadLine());
                            string NewContents = Console.ReadLine();
                            Client.SendTCPData(ClientSendPacketFunctions.UpdateFile(EditID, Version, NewContents));
                            break;
                        case ("DELETE"):
                            int DeleteID = Convert.ToInt32(Console.ReadLine());
                            Client.SendTCPData(ClientSendPacketFunctions.DeleteFile(DeleteID));
                            break;
                        case ("UNSUB"):
                            int UnsubID = Convert.ToInt32(Console.ReadLine());
                            Client.SendTCPData(ClientSendPacketFunctions.UnsubscribeFromFileUpdates(UnsubID));
                            break;
                        case ("CFOLDER"):
                            int NBaseFolder = Convert.ToInt32(Console.ReadLine());
                            string Name = Console.ReadLine();
                            Client.SendTCPData(ClientSendPacketFunctions.CreateFolder(NBaseFolder, Name));
                            break;
                        case ("RFOLDER"):
                            int RFolder = Convert.ToInt32(Console.ReadLine());
                            string Rename = Console.ReadLine();
                            Client.SendTCPData(ClientSendPacketFunctions.RenameFolder(RFolder, Rename));
                            break;
                        case ("MFOLDER"):
                            int MFolder = Convert.ToInt32(Console.ReadLine());
                            int MTFolder = Convert.ToInt32(Console.ReadLine());
                            Client.SendTCPData(ClientSendPacketFunctions.MoveFolder(MFolder, MTFolder));
                            break;
                        case ("DFOLDER"):
                            int DFolder = Convert.ToInt32(Console.ReadLine());
                            Client.SendTCPData(ClientSendPacketFunctions.DeleteFolder(DFolder));
                            break;
                        case ("BREAKPOINT"):
                            break;
                        default:
                            break;
                    }
                }

                Client.ProcessPackets();
                
                if (UIEventManager.ClientSuccessConnecting) UIEventManager.ClientSuccessConnectingDelegate?.Invoke(null, new EventArgs());
                if (UIEventManager.ClientFailedConnecting) UIEventManager.ClientFailedConnectingDelegate?.Invoke(null, new EventArgs());
            }
        }

        static void ConnectedQuick(object Sender, EventArgs Args)
        {
            Console.WriteLine("Successfully connected to server!");
            Client.SendTCPData(ClientSendPacketFunctions.RequestLogReceiverStatus());
            Client.SendTCPData(ClientSendPacketFunctions.LoadProject(Directory));
            Client.SendTCPData(ClientSendPacketFunctions.RequestFolderData(0, true));
            UIEventManager.ClientSuccessConnecting = false;
            UIEventManager.ClientSuccessConnectingDelegate = null;
        }

        static void Connected(object Sender, EventArgs Args)
        {
            Console.WriteLine("Successfully connected to server!");
            Client.SendTCPData(ClientSendPacketFunctions.RequestLogReceiverStatus());
            UIEventManager.ClientSuccessConnecting = false;
            UIEventManager.ClientSuccessConnectingDelegate = null;
        }

        static void FailedConnection(object Sender, EventArgs Args)
        {
            Console.WriteLine("Failed connection!");
            UIEventManager.ClientFailedConnecting = false;
            UIEventManager.ClientSuccessConnectingDelegate = null;
        }
    }
}
