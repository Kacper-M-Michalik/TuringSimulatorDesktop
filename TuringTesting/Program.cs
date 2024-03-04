using System;
using System.Threading;
using System.Text.Json;
using System.Collections.Generic;
using System.Text;
using System.Net;
using NetworkTesting;
using System.IO;
using TuringCore.Files;

namespace TuringTesting
{
    class Program
    {
        static void Main(string[] args)
        {
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
                            /*Console.WriteLine("QUICK - Starts server, loads project from last specified directory and connects to it\n" +
                                "SERVER - Starts server\n" +
                                "STOP SERVER - Stops server\n" +
                                "LOCAL - Starts + connects to local server\n" +
                                "CONNECT - Takes IP and connects to that server\n" +
                                "DISCONNECT - Disconnects client from server\n" +
                                "KILLFIRSTCLIENT - Disconnects first client from server\n" +
                                "MESSAGE - Sends the server a text message\n");
                            */
                            break;
                        case ("JOIN"):
                            Client.ConnectToServer(IPAddress.Parse("127.0.0.1"), 28104);
                            break;
                        case ("SHORT PACKET"):
                            Client.SendTCPData(ClientSendPacketFunctions.InvalidShortPacket());
                            break;
                        case ("INVALID REQUEST PACKET"):
                            Client.SendTCPData(ClientSendPacketFunctions.InvalidRequestPacket());
                            break;
                        case ("UPDATE"):
                            Client.SendTCPData(ClientSendPacketFunctions.UpdateFile(Console.ReadLine(), Convert.ToInt32(Console.ReadLine()), JsonSerializer.SerializeToUtf8Bytes(new Alphabet())));
                            break;
                        case ("DISCONNECT"):
                            Client.Disconnect();
                            break;
                        default:
                            break;
                    }
                }

                Client.ProcessPackets();                
            }
        }
    }
}
