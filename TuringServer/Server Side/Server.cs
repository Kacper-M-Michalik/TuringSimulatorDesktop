using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TuringCore;
using TuringCore.Networking;
using TuringServer.Data;
using TuringServer.Logging;

namespace TuringServer.ServerSide
{
    public static class Server
    {
        //possible bug -> when user joins as they are added onto clients on new thread, having simualtenous SendTCPDataToAllClients may have problems
        //may have to add packet "User joined server" -> other thread creates, adds onto queue, server send this new client the project data, aka just folder structure for now

        public static bool IsServerOn;

        public static int MaxClients { get; private set; }
        public static int Port { get; private set; }
        public static long CacheExpiryTime { get; private set; } = TimeSpan.FromMinutes(5).Ticks;

        public static Thread ServerThread { get; private set; }
        static TcpListener ServerTcpListener;
        public static Dictionary<int, ServerClientSlot> Clients { get; private set; }

        static Queue<Packet> PacketProcessingQueue = new Queue<Packet>();
        static Queue<Packet> PacketsBeingProcessed;
        static bool MarkForClosing;
        static long LastTick;

        public static ProjectData LoadedProject;

        //Sets up server
        public static void StartServer(int SetMaxPlayers, int SetPort)
        {
            MaxClients = SetMaxPlayers;
            Port = SetPort;

            CustomLogging.Log("THREAD NOTIF SERVER: SERVER INITIATED ON THREAD " + Thread.CurrentThread.ManagedThreadId.ToString());

            IsServerOn = true;
            MarkForClosing = false;
            LastTick = DateTime.UtcNow.Ticks;

            //Create slot for every possible client
            Clients = new Dictionary<int, ServerClientSlot>();
            for (int i = 0; i < MaxClients; i++)
            {
                Clients.Add(i, new ServerClientSlot(i));
            }

            //Start listening for any incoming TCP connections
            ServerTcpListener = new TcpListener(IPAddress.Any, Port);
            ServerTcpListener.Start();

            CustomLogging.Log("SERVER: Server Started on port: " + Port.ToString());

            //When a TCP connection is found, call this function
            ServerTcpListener.BeginAcceptTcpClient(new AsyncCallback(NewTCPClientConnectedCallback), null);
            
            RunServer();

                /*
                 * 
            if (LoadedProject != null) throw new Exception("Already loaded project.");

            LoadedProject = FileManager.LoadProjectFile(Location);

            if (LoadedProject != null)
            {
                CustomConsole.Log("Loader Successful");
                //raise event here?
                RunServer();
            }
            else
            {
                CustomConsole.Log("Loader Unsuccessful");
                //raise event here?
            }
                 * 
            
            /*
            ServerThread = new Thread(RunServer);
            ServerThread.Start();
            */
        }

        #region Helper Functions

        //Sends a packet to specific client
        public static void SendTCPData(int ClientID, Packet Data)
        {
            Data.InsertPacketLength();
            Clients[ClientID].TCP.SendDataToClient(Data.SaveTemporaryBufferToPernamentReadBuffer());
            //Data.Dispose();
        }
        /*
        public static void SendTCPData(int ClientID, byte[] Data)
        {
            Clients[ClientID].TCP.SendDataToClient(Data);
        }
        */

        //Sends a packet to all connected clients
        public static void SendTCPToAllClients(Packet Data)
        {
            Data.InsertPacketLength();
            byte[] FinalData = Data.SaveTemporaryBufferToPernamentReadBuffer();
            for (int i = 0; i < MaxClients; i++)
            {
                if (Clients[i].TCP.ConnectionSocket != null)
                {
                    Clients[i].TCP.SendDataToClient(FinalData);
                }
            }
            //Data.Dispose();
        }
        #endregion

        //Server main loop as described by the Backend Design Section
        static void RunServer()
        {    
            //Continue while not told to close
            while (!MarkForClosing)
            {
                if (PacketProcessingQueue.Count > 0)
                {
                    //Copy queued packets
                    lock (PacketProcessingQueue)
                    {
                        PacketsBeingProcessed = new Queue<Packet>(PacketProcessingQueue);
                        PacketProcessingQueue.Clear();
                    }

                    int Length = PacketsBeingProcessed.Count;
                    for (int i = 0; i < Length; i++)
                    {
                        Packet Data = PacketsBeingProcessed.Dequeue();

                        //Check packet has valid header
                        if (Data.Length() >= 8)
                        {
                            //Packet Length has been replaced with sender ID
                            int SenderID = Data.ReadInt();
                            //Get Packet Type
                            int PacketType = Data.ReadInt();

                            //Check if Packet Type Valid + Process Packet
                            if (Enum.IsDefined(typeof(ClientSendPackets), PacketType))
                            {
                                ServerReceiveFunctions.PacketToFunction[PacketType](SenderID, Data);
                            }
                            else
                            {
                                CustomLogging.Log("SERVER: Invalid Packet recieved");
                            }
                            //Data.Dispose();
                        }
                    }
                }

                long CurrentTime = DateTime.UtcNow.Ticks;

                //Here we add the time passed to each cached file, when the time alive is larger than the maximum time alive, the cached file is unloaded
                if (LoadedProject != null)
                {
                    foreach (KeyValuePair<int, CacheFileData> CachedFile in LoadedProject.CacheDataLookup)
                    {
                        CachedFile.Value.ExpiryTimer += CurrentTime - LastTick;

                        if (CachedFile.Value.ExpiryTimer > CacheExpiryTime)
                        {
                            LoadedProject.CacheDataLookup.Remove(CachedFile.Key);
                        }
                    }
                }
                LastTick = CurrentTime;              
            }

            //Once the server has finished processing packets and was told turn off, we shut it down
            ShutDown();
        }

        public static void CloseServer()
        {
            MarkForClosing = true;
        }

        //Allows another thread to add a packet to the processing queue for the main server thread
        public static void AddPacketToProcessOnServerThread(int SenderID, Packet PacketToAdd)
        {
            //Locking the queue means that the main server loop has to wait until the queue is unlocked to use it, guaranteeing that the server thread doesn't copy the processing queue while more packets are being added to it
            lock (PacketProcessingQueue)
            {
                PacketToAdd.InsertPacketSenderIDUnsafe(SenderID);
                PacketProcessingQueue.Enqueue(PacketToAdd);
            }
        }

        //This gets called when a new TCP connection is incoming
        static void NewTCPClientConnectedCallback(IAsyncResult Result)
        {
            if (!IsServerOn) return;

            CustomLogging.Log("SERVER: Server dealing with new connection on thread " + Thread.CurrentThread.ManagedThreadId.ToString());

            //Accept the connection
            TcpClient NewClient = ServerTcpListener.EndAcceptTcpClient(Result);

            //Listen for any more incoming connections
            ServerTcpListener.BeginAcceptTcpClient(new AsyncCallback(NewTCPClientConnectedCallback), null);

            //Find next available slot for client to connect to, if none found, we reach the NewClient.Close() function at the end, closing the connection
            for (int i = 0; i < MaxClients; i++)
            {
                if (Clients[i].TCP.ConnectionSocket == null)
                {
                    Clients[i].TCP.ConnectClientToServer(NewClient);
                    if (LoadedProject != null) 
                    {
                        //When a new client join successfully, we send a fake request to the server, making it automatically send the user data about the currently loaded project
                        Packet MockPacket = ClientSendPacketFunctions.RequestProjectData();
                        MockPacket.InsertPacketLength();
                        MockPacket.SaveTemporaryBufferToPernamentReadBuffer();
                        AddPacketToProcessOnServerThread(i, MockPacket);
                    }
                    return;
                }
            }

            CustomLogging.Log("SERVER: " + NewClient.Client.RemoteEndPoint.ToString() + " failed to connect: Server Full!");

            NewClient.Close();
        }

        //Shuts down all TCP connections and cleans up server
        static void ShutDown()
        {
            IsServerOn = false;
            ServerTcpListener.Stop();
            ServerTcpListener = null;
            Clients = null;
            PacketProcessingQueue.Clear();
            PacketsBeingProcessed = null;
            LoadedProject = null;
            
            CustomLogging.LogClientID = -1;
            CustomLogging.Log("SERVER: Server Closed");
        }
    }

    public class ServerClientSlot
    {
        private int ClientId;
        public TCPInterface TCP;

        public static int DefaultDataBufferSize = 4096;

        //Constructor
        public ServerClientSlot(int SetClientID)
        {
            ClientId = SetClientID;
            TCP = new TCPInterface(ClientId);
        }

        public class TCPInterface
        {
            public TcpClient ConnectionSocket;
            NetworkStream DataStream;
            readonly int ID;

            byte[] ReceiveDataBuffer;
            int DataBufferSize = DefaultDataBufferSize;

            Packet PacketCurrentlyBeingRebuilt;

            public TCPInterface(int SetID)
            {
                ID = SetID;
            }

            //Setups up socket information and data stream once a connection is completed
            public void ConnectClientToServer(TcpClient SetConnectionSocket)
            {
                ConnectionSocket = SetConnectionSocket;
                ConnectionSocket.ReceiveBufferSize = DataBufferSize;
                ConnectionSocket.SendBufferSize = DataBufferSize;

                DataStream = ConnectionSocket.GetStream();
                ReceiveDataBuffer = new byte[DataBufferSize];
                PacketCurrentlyBeingRebuilt = new Packet();

                DataStream.BeginRead(ReceiveDataBuffer, 0, DataBufferSize, OnReceiveDataFromClient, null);

                CustomLogging.Log("SERVER: Client at " + ConnectionSocket.Client.RemoteEndPoint.ToString() + " has been connected to server!");
            }

            //Writes data to the data stream, sending it to the client
            public void SendDataToClient(byte[] Data)
            {
                if (ConnectionSocket == null) return;

                try
                {
                    DataStream.BeginWrite(Data, 0, Data.Length, null, null);
                }
                catch (Exception E)
                {
                    CustomLogging.Log("SERVER: Error Sending Data To Client " + ID.ToString() + ":  " + E.ToString());
                }
            }

            // Packet rebuilding algorithm as detailed in the Backend Networking Section
            private void OnReceiveDataFromClient(IAsyncResult Result)
            {
               // try
              //  {
                    //if (ConnectionSocket == null) return;

                CustomLogging.Log("SERVER: Server dealing with incoming data on thread " + Thread.CurrentThread.ManagedThreadId.ToString()+ ". From client: " + ID.ToString());

                //warning -> if there are packets that are intended for this client after disconnect, shit will hit the fan, possible bug with customconsole, as changing logclientid on seperate thread than variable is used on

                //Check if the socket is still active (this function may get called on a disconnect according to Microsoft Docs)
                if (!ConnectionSocket.Connected)
                {
                    TCPInternalDisconnect();
                    CustomLogging.Log("SERVER: Client " + ID.ToString() + " has disconnected!");

                    if (CustomLogging.LogClientID == ID) CustomLogging.LogClientID = -1;

                    return;
                }            

                //Read incoming data
                int IncomingDataLength = DataStream.EndRead(Result);

                //Copy into temp buffer
                byte[] UsefuldataBuffer = new byte[IncomingDataLength];
                Array.Copy(ReceiveDataBuffer, UsefuldataBuffer, IncomingDataLength);

                //Copy into Packet
                PacketCurrentlyBeingRebuilt.Write(UsefuldataBuffer, false);
                PacketCurrentlyBeingRebuilt.SaveTemporaryBufferToPernamentReadBuffer();

                //Check if a full packet has been received, if so, add to packet processing queue
                if (PacketCurrentlyBeingRebuilt.UnreadLength() >= 4)
                {
                    int PacketLength = PacketCurrentlyBeingRebuilt.ReadInt(false);

                    while (PacketCurrentlyBeingRebuilt.UnreadLength() >= PacketLength && PacketCurrentlyBeingRebuilt.UnreadLength() >= 4)
                    {
                        Packet ProcessedPacket = new Packet(PacketCurrentlyBeingRebuilt.ReadBytes(PacketLength));
                        Server.AddPacketToProcessOnServerThread(ID, ProcessedPacket);

                        if (PacketCurrentlyBeingRebuilt.UnreadLength() >= 4)
                        {
                            PacketLength = PacketCurrentlyBeingRebuilt.ReadInt(false);
                        }

                    }

                    if (PacketCurrentlyBeingRebuilt.UnreadLength() == 0)
                    {
                        PacketCurrentlyBeingRebuilt.Reset();
                    }

                }

                //Start reading the incoming data again
                if (ConnectionSocket != null) DataStream.BeginRead(ReceiveDataBuffer, 0, DataBufferSize, OnReceiveDataFromClient, null);
              //   }
             //   catch (Exception E)
             //   {
             //       CustomLogging.Log(E.ToString());
             //   }
            }

            //Disconnect and clean up the TCP socket and data stream
            public void TCPInternalDisconnect()
            {
                ConnectionSocket?.Close();
                ConnectionSocket = null;
                DataStream = null;
                ReceiveDataBuffer = null;
                PacketCurrentlyBeingRebuilt = null;
            }

        }

        //Shuts downb TCP conenction with this client
        public void DisconnectClientFromServer()
        {
            CustomLogging.Log("SERVER: " + TCP.ConnectionSocket.Client.RemoteEndPoint.ToString() + " aka. Client N:" + ClientId.ToString() + " has been disconnected from the server!");

            TCP.TCPInternalDisconnect();
        }
    }
}
