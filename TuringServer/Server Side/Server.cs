﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TuringCore;
using TuringServer.Logging;

namespace TuringServer
{
    public static class Server
    {
        //possible bug -> when user joins as they are added onto clients on new thread, having simualtenous SendTCPDataToAllClients may have problems
        //may have to add packet "User joined server" -> other thread creates, adds onto queue, server send this new client the project data, aka jsut folder structure for now

        public static bool IsServerOn;

        public static int MaxClients { get; private set; }
        public static int Port { get; private set; }
        public static long CacheExpiryTime { get; private set; } = TimeSpan.FromMinutes(5).Ticks;

        public static Thread ServerThread { get; private set; }
        static TcpListener ServerTcpListener;
        public static Dictionary<int, ServerClientSlot> Clients { get; private set; }

        static Queue<Packet> PacketProcessingQueue;
        static Queue<Packet> PacketsBeingProcessed;
        static bool MarkForClosing;
        static long LastTick;

        public static ProjectData LoadedProject;

        public static void StartServer(int SetMaxPlayers, int SetPort)
        {
            MaxClients = SetMaxPlayers;
            Port = SetPort;

            CustomConsole.Log("THREAD NOTIF SERVER: SERVER INIT ON THREAD " + Thread.CurrentThread.ManagedThreadId.ToString());

            IsServerOn = true;
            MarkForClosing = false;
            LastTick = DateTime.UtcNow.Ticks;

            Clients = new Dictionary<int, ServerClientSlot>();
            for (int i = 0; i < MaxClients; i++)
            {
                Clients.Add(i, new ServerClientSlot(i));
            }

            ServerTcpListener = new TcpListener(IPAddress.Any, Port);
            ServerTcpListener.Start();

            CustomConsole.Log("SERVER: Server Started on port: " + Port.ToString());

            ServerTcpListener.BeginAcceptTcpClient(new AsyncCallback(NewTCPClientConnectedCallback), null);

            PacketProcessingQueue = new Queue<Packet>();
            
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

        static void RunServer()
        {    
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

                        //Packet Length has been replaced with sender ID
                        int SenderID = Data.ReadInt();
                        //Get Type
                        int PacketType = Data.ReadInt();
                        //Execute function

                        if (Enum.IsDefined(typeof(ClientSendPackets), PacketType))
                        {
                            ServerReceiveFunctions.PacketToFunction[PacketType](SenderID, Data);
                        }
                        else
                        {
                            CustomConsole.Log("SERVER: Invalid Packet recieved");
                        }
                        Data.Dispose();
                    }
                }

                long CurrentTime = DateTime.UtcNow.Ticks;

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

            ShutDown();
        }

        public static void CloseServer()
        {
            MarkForClosing = true;
        }

        public static void AddPacketToProcessOnServerThread(int SenderID, Packet PacketToAdd)
        {
            lock (PacketProcessingQueue)
            {
                PacketToAdd.InsertPacketSenderIDUnsafe(SenderID);
                PacketProcessingQueue.Enqueue(PacketToAdd);
            }
        }

        static void NewTCPClientConnectedCallback(IAsyncResult Result)
        {
            if (!IsServerOn) return;

            CustomConsole.Log("SERVER: Server dealing with new connection on thread " + Thread.CurrentThread.ManagedThreadId.ToString());

            TcpClient NewClient = ServerTcpListener.EndAcceptTcpClient(Result);

            ServerTcpListener.BeginAcceptTcpClient(new AsyncCallback(NewTCPClientConnectedCallback), null);

            for (int i = 0; i < MaxClients; i++)
            {
                if (Clients[i].TCP.ConnectionSocket == null)
                {
                    Clients[i].TCP.ConnectClientToServer(NewClient);
                    return;
                }
            }

            CustomConsole.Log("SERVER: " + NewClient.Client.RemoteEndPoint.ToString() + " failed to connect: Server Full!");

            NewClient.Close();
        }

        static void ShutDown()
        {
            IsServerOn = false;
            ServerTcpListener.Stop();
            ServerTcpListener = null;
            Clients = null;
            PacketProcessingQueue = null;
            PacketsBeingProcessed = null;
            LoadedProject = null;

            CustomConsole.Log("SERVER: Server Closed");
        }
    }

    public class ServerClientSlot
    {
        private int ClientId;
        public TCPInterface TCP;

        public static int DefaultDataBufferSize = 4096;

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

            public void ConnectClientToServer(TcpClient SetConnectionSocket)
            {
                ConnectionSocket = SetConnectionSocket;
                ConnectionSocket.ReceiveBufferSize = DataBufferSize;
                ConnectionSocket.SendBufferSize = DataBufferSize;

                DataStream = ConnectionSocket.GetStream();
                ReceiveDataBuffer = new byte[DataBufferSize];
                PacketCurrentlyBeingRebuilt = new Packet();

                DataStream.BeginRead(ReceiveDataBuffer, 0, DataBufferSize, OnReceiveDataFromClient, null);

                CustomConsole.Log("SERVER: Client at " + ConnectionSocket.Client.RemoteEndPoint.ToString() + " has been connected to server!");
            }

            public void SendDataToClient(Packet Data)
            {
                if (ConnectionSocket == null) return;

                try
                {
                    Data.InsertPacketLength();
                    DataStream.BeginWrite(Data.SaveTemporaryBufferToPernamentReadBuffer(), 0, Data.Length(), null, null);
                }
                catch (Exception E)
                {
                    CustomConsole.Log("SERVER: Error Sending Data To Client " + ID.ToString() + ":  " + E.ToString());
                }
            }

            private void OnReceiveDataFromClient(IAsyncResult Result)
            {
                try
                {
                    if (ConnectionSocket == null) return;

                    CustomConsole.Log("THREAD NOTIF SERVER: Server dealing with incoming data on thread " + Thread.CurrentThread.ManagedThreadId.ToString());
                    int IncomingDataLength = DataStream.EndRead(Result);

                    //warning -> if ther are packets that are intended for this client after disconnect, shit will hit the fan, possible bug with customconsole, as changing logclientid on seperate thread than variable is used on
                    if (IncomingDataLength == 0)
                    {
                        TCPInternalDisconnect();
                        CustomConsole.Log("SERVER: Client " + ID.ToString() + " has disconnected!");

                        if (CustomConsole.LogClientID == ID) CustomConsole.LogClientID = -1;

                        return;
                    }

                    byte[] UsefuldataBuffer = new byte[IncomingDataLength];
                    Array.Copy(ReceiveDataBuffer, UsefuldataBuffer, IncomingDataLength);

                    CustomConsole.Log("SERVER: Server is receiving data from client " + ID.ToString() + "!");

                    PacketCurrentlyBeingRebuilt.Write(UsefuldataBuffer, false);
                    PacketCurrentlyBeingRebuilt.SaveTemporaryBufferToPernamentReadBuffer();

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

                    if (ConnectionSocket != null) DataStream.BeginRead(ReceiveDataBuffer, 0, DataBufferSize, OnReceiveDataFromClient, null);
                }
                catch (Exception E)
                {
                    CustomConsole.Log(E.ToString());
                }
            }

            public void TCPInternalDisconnect()
            {
                ConnectionSocket?.Close();
                ConnectionSocket = null;
                DataStream = null;
                ReceiveDataBuffer = null;
                PacketCurrentlyBeingRebuilt = null;
            }

        }

        public void DisconnectClientFromServer()
        {
            CustomConsole.Log("SERVER: " + TCP.ConnectionSocket.Client.RemoteEndPoint.ToString() + " aka. Client N:" + ClientId.ToString() + " has been disconnected from the server!");

            TCP.TCPInternalDisconnect();
        }
    }
}
