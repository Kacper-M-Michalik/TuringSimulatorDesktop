using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TuringCore;
using TuringServer;
using System.Text.Json;
using System.Text.Json.Serialization;
using TuringCore.Networking;
using TuringSimulatorDesktop.Debugging;
using TuringSimulatorDesktop.UI;

namespace TuringSimulatorDesktop.Networking
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

        //TCP Helper Class
        class TCPInterface
        {
            public TcpClient ConnectionSocket;
            private NetworkStream DataStream;
            private byte[] ReceiveDataBuffer;
            private Packet PacketCurrentlyBeingRebuilt;

            //Timeout based connection system
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
                        CustomLogging.Log("CLIENT: Connection timed out.");
                        TCPInternalDisconnect();
                        UIEventManager.ClientFailedConnecting = true;
                    }                                    

                }
                catch (Exception E)
                {
                    CustomLogging.Log("CLIENT: Connection attempt failure! " + E.ToString());
                    UIEventManager.ClientFailedConnecting = true;
                    TCPInternalDisconnect();
                }
            }

            //If successfully connected, this is called
            public void OnConnectCallBack(IAsyncResult Result)            
            {
                try
                {
                    //Check connection is successful, if TCP socket is shut down while waiting for callback, this function gets called despite no connection according to Microsoft Docs
                    if (ConnectionSocket == null) return;

                    CustomLogging.Log("CLIENT: Connect callback called.");

                    ConnectionSocket.EndConnect(Result);
                    DataStream = ConnectionSocket.GetStream();
                    PacketCurrentlyBeingRebuilt = new Packet();

                    UIEventManager.ClientSuccessConnecting = true;

                    //Start reading incoming data from server
                    DataStream.BeginRead(ReceiveDataBuffer, 0, DataBufferSize, OnReceiveDataFromServer, null);
                }
                catch (Exception E)
                {
                    CustomLogging.Log(E.ToString());
                    TCPInternalDisconnect();
                }
            }

            //Processes and sends packet to server
            public void SendDataToServer(Packet Data)
            {
                try
                {
                    CustomLogging.Log("CLIENT: Client is writing data to server");
                    Data.InsertPacketLength();
                    DataStream.BeginWrite(Data.SaveTemporaryBufferToPernamentReadBuffer(), 0, Data.Length(), null, null);
                }
                catch (Exception E)
                {
                    CustomLogging.Log("CLIENT: Error Sending Data To Server! " + E.ToString());
                }
            }

            //Packet reconstruction algorithm as detailed in Front End Networking
            private void OnReceiveDataFromServer(IAsyncResult Result)
            {
                try
                {
                    //Check connection si still valid, when a TCP connection is terminated and we were waiting to read data, this function gets called according to Microsoft Docs
                    if (ConnectionSocket == null || !ConnectionSocket.Connected)
                    {
                        CustomLogging.Log("CLIENT: Server disconnected me!");
                        TCPInternalDisconnect();

                        return;
                    }

                    int IncomingDataLength = DataStream.EndRead(Result);

                    //0 length data also means disconnect
                    if (IncomingDataLength == 0)
                    {
                        CustomLogging.Log("CLIENT: Server disconnected me!");
                        TCPInternalDisconnect();

                        return;
                    }

                    //ReceiveBuffer is always 4096 or whatever , we need to only pack data that is useful into our rebuilt packet -> as such we only copy IncomingDataLength of recievebuffer.
                    byte[] UsefuldataBuffer = new byte[IncomingDataLength];
                    Array.Copy(ReceiveDataBuffer, UsefuldataBuffer, IncomingDataLength);

                    PacketCurrentlyBeingRebuilt.Write(UsefuldataBuffer, false);
                    PacketCurrentlyBeingRebuilt.SaveTemporaryBufferToPernamentReadBuffer();

                    if (PacketCurrentlyBeingRebuilt.UnreadLength() >= 4)
                    {
                        int PacketLength = PacketCurrentlyBeingRebuilt.ReadInt(false);

                        while (PacketCurrentlyBeingRebuilt.UnreadLength() >= PacketLength && PacketCurrentlyBeingRebuilt.UnreadLength() >= 4)
                        {
                            //Packet successfully reconstructed, add to queue to process on main UI thread
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
                    CustomLogging.Log(E.ToString());
                }
            }
            
            //Closes and cleans up TCP connection/socket
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

        //Process Responses
        public static void ProcessPackets()
        {     
            if (PacketProcessingQueue.Count > 0)
            {
                //Copy reconstructed packets
                lock (PacketProcessingQueue)
                {
                    PacketsBeingProcessed = new Queue<Packet>(PacketProcessingQueue);
                    PacketProcessingQueue.Clear();
                }

                int Length = PacketsBeingProcessed.Count;
                for (int i = 0; i < Length; i++)
                {
                    Packet Data = PacketsBeingProcessed.Dequeue();

                    if (Data.Length() >= 8)
                    {
                        //Get rid of packet size
                        Data.ReadInt();

                        int PacketType = Data.ReadInt();

                        //Execute function
                        if (ClientReceiveFunctions.PacketToFunction.ContainsKey(PacketType))
                            ClientReceiveFunctions.PacketToFunction[PacketType](Data);
                    }
                }

            }            
        }

        //Starts the join server thread
        public static void ConnectToServer(IPAddress IP, int Port)
        {
            if (IsConnecting || IsConnected) return;

            IsConnecting = true;
            Thread ConnectThread = new Thread(() => TCP.Connect(IP, Port, 1000));
            ConnectThread.Start();
        }

        //Writes data to server
        public static void SendTCPData(Packet Data)
        {
            TCP.SendDataToServer(Data);
        }

        //Disconnects the TCP connection
        public static void Disconnect()
        {
            CustomLogging.Log("CLIENT: DISCONNECTING FROM SERVER!");
            TCP.TCPInternalDisconnect();
        }
    }
}
