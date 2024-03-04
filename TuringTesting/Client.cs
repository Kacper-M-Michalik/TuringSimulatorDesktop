using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TuringCore.Networking;

namespace NetworkTesting
{
    public static class Client
    {
        public static bool IsConnected
        {
            get
            {
                return TCP.ConnectionSocket != null ? TCP.ConnectionSocket.Connected : false;
            }
        }
        public static bool IsConnecting;

        static TCPInterface TCP = new TCPInterface();
        static int DataBufferSize = 4096;

        class TCPInterface
        {
            public TcpClient ConnectionSocket;
            private NetworkStream DataStream;
            private byte[] ReceiveDataBuffer;
            private Packet PacketCurrentlyBeingRebuilt;

            public void Connect(IPAddress TargetIP, int Port, int Timeout)
            {
                try
                {
                    ConnectionSocket = new TcpClient();
                    ConnectionSocket.ReceiveBufferSize = DataBufferSize;
                    ConnectionSocket.SendBufferSize = DataBufferSize;

                    ReceiveDataBuffer = new byte[DataBufferSize];

                    ConnectionSocket.BeginConnect(TargetIP, Port, OnConnectCallBack, ConnectionSocket);
                    Thread.Sleep(Timeout);

                    IsConnecting = false;

                    if (!IsConnected)
                    {
                        Console.WriteLine("CLIENT: Connection timed out.");
                        TCPInternalDisconnect();
                    }

                }
                catch (Exception E)
                {
                    Console.WriteLine("CLIENT: Connection attempt failure! " + E.ToString());
                    TCPInternalDisconnect();
                }
            }

            public void OnConnectCallBack(IAsyncResult Result)
            {
                try
                {
                    //whats point of this?
                    if (ConnectionSocket == null) return;

                    Console.WriteLine("CLIENT: Connect callback called.");

                    ConnectionSocket.EndConnect(Result);
                    DataStream = ConnectionSocket.GetStream();
                    PacketCurrentlyBeingRebuilt = new Packet();

                    DataStream.BeginRead(ReceiveDataBuffer, 0, DataBufferSize, OnReceiveDataFromServer, null);
                }
                catch (Exception E)
                {
                    Console.WriteLine(E.ToString());
                    TCPInternalDisconnect();
                }
            }

            public void SendDataToServer(Packet Data)
            {
                try
                {
                    Console.WriteLine("CLIENT: Client is writing data to server");
                    Data.InsertPacketLength();
                    DataStream.BeginWrite(Data.SaveTemporaryBufferToPernamentReadBuffer(), 0, Data.Length(), null, null);
                }
                catch (Exception E)
                {
                    Console.WriteLine("CLIENT: Error Sending Data To Server! " + E.ToString());
                }
            }

            private void OnReceiveDataFromServer(IAsyncResult Result)
            {
                try
                {
                    if (ConnectionSocket == null || !ConnectionSocket.Connected)
                    {
                        Console.WriteLine("CLIENT: Server disconnected me!");
                        TCPInternalDisconnect();

                        return;
                    }

                    int IncomingDataLength = DataStream.EndRead(Result);

                    byte[] UsefuldataBuffer = new byte[IncomingDataLength];
                    Array.Copy(ReceiveDataBuffer, UsefuldataBuffer, IncomingDataLength);

                    PacketCurrentlyBeingRebuilt.Write(UsefuldataBuffer, false);
                    PacketCurrentlyBeingRebuilt.SaveTemporaryBufferToPernamentReadBuffer();

                    if (PacketCurrentlyBeingRebuilt.UnreadLength() >= 4)
                    {
                        int PacketLength = PacketCurrentlyBeingRebuilt.ReadInt(false);

                        while (PacketCurrentlyBeingRebuilt.UnreadLength() >= PacketLength && PacketCurrentlyBeingRebuilt.UnreadLength() >= 4)
                        {
                            Packet ProcessedPacket = new Packet(PacketCurrentlyBeingRebuilt.ReadBytes(PacketLength));
                            AddPacketToProcessOnMainThread(ProcessedPacket);

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

                    if (ConnectionSocket != null) DataStream.BeginRead(ReceiveDataBuffer, 0, DataBufferSize, OnReceiveDataFromServer, null);
                }
                catch (Exception E)
                {
                    Console.WriteLine(E.ToString());
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

        static Queue<Packet> PacketProcessingQueue = new Queue<Packet>();
        static Queue<Packet> PacketsBeingProcessed;

        static void AddPacketToProcessOnMainThread(Packet PacketToAdd)
        {
            lock (PacketProcessingQueue)
            {
                PacketProcessingQueue.Enqueue(PacketToAdd);
            }
        }

        public static void ProcessPackets()
        {
            if (PacketProcessingQueue.Count > 0)
            {
                lock (PacketProcessingQueue)
                {
                    PacketsBeingProcessed = new Queue<Packet>(PacketProcessingQueue);
                    PacketProcessingQueue.Clear();
                }

                int Length = PacketsBeingProcessed.Count;
                for (int i = 0; i < Length; i++)
                {
                    Packet Data = PacketsBeingProcessed.Dequeue();

                    //Get rid of packet size
                    Data.ReadInt();
                    //Get Type
                    int PacketType = Data.ReadInt();
                    //Execute function
                    if (ClientReceiveFunctions.PacketToFunction.ContainsKey(PacketType))
                        ClientReceiveFunctions.PacketToFunction[PacketType](Data);
                }

            }
        }

        public static void ConnectToServer(IPAddress IP, int Port)
        {
            if (IsConnecting || IsConnected) return;

            IsConnecting = true;
            Thread ConnectThread = new Thread(() => TCP.Connect(IP, Port, 1000));
            ConnectThread.Start();
        }

        public static void SendTCPData(Packet Data)
        {
            TCP.SendDataToServer(Data);
        }

        public static void Disconnect()
        {
            Console.WriteLine("CLIENT: DISCONNECTING FROM SERVER!");
            TCP.TCPInternalDisconnect();
        }
    }
}
