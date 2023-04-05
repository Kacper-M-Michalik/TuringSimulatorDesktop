using System;
using System.Collections.Generic;
using System.Text;

namespace TuringCore.Networking
{
    //Request types the client can send
    public enum ClientSendPackets
    {
        LoadProject,
        RequestLogReceieverStatus,

        RequestProjectData,
        RequestFolderData,
        //RequestFileByID,
        RequestFileMetadata,
        RequestFile,
        UnsubscribeFromUpdatesForFile,
        UnsubscribeFromUpdatesForFolder,

        CreateFile,
        UpdateFile,
        RenameFile,
        MoveFile,
        DeleteFile,

        CreateFolder,
        RenameFolder,
        MoveFolder,
        DeleteFolder
    }

    //Response types the server can send
    public enum ServerSendPackets
    {
        LogData,
        ErrorNotification,

        SentProjectData,
        SentFolderData,
        SentOrUpdatedFile,
        SentFileMetadata,
        
        DeletedFile,

        CreatedFolder,
        RenamedFolder,
        MovedFolder,
        DeletedFolder
    }

    public class Packet //: IDisposable
    {
        byte[] ReadBuffer;
        public List<byte> TemporaryWriteBuffer;
        public int ReadPointerPosition;

        public Packet()
        {
            TemporaryWriteBuffer = new List<byte>(0);
            ReadPointerPosition = 0;
        }

        //Create Packet with existing binary data
        public Packet(byte[] Data)
        {
            TemporaryWriteBuffer = new List<byte>(0);
            ReadPointerPosition = 0;
            ReadBuffer = Data;
        }


        #region Writes

        //Each write function writes to the temporary write buffer, this is as to allow dynamically increasign the size of the packet as mroe data is writetn to it

        //AddLength controls whether the length of the byte[] is inserted before the byte[], it might not always be required if the size is known prehand and saves us memory
        public void Write(byte[] Data, bool AddLength = true)
        {
            if (AddLength) Write(Data.Length);
            TemporaryWriteBuffer.AddRange(Data);
        }

        public void Write(float Data)
        {
            TemporaryWriteBuffer.AddRange(BitConverter.GetBytes(Data));
        }

        public void Write(int Data)
        {
            TemporaryWriteBuffer.AddRange(BitConverter.GetBytes(Data));
        }

        public void Write(Guid Data)
        {
            TemporaryWriteBuffer.AddRange(Data.ToByteArray());
        }

        public void Write(short Data)
        {
            TemporaryWriteBuffer.AddRange(BitConverter.GetBytes(Data));
        }

        public void Write(bool Data)
        {
            TemporaryWriteBuffer.AddRange(BitConverter.GetBytes(Data));
        }

        public void Write(string Data)
        {
            Write(Data.Length);
            TemporaryWriteBuffer.AddRange(Encoding.ASCII.GetBytes(Data));
        }

        #endregion

        #region Reads

        //We read from the final ReadBuffer

        //MovePointer indicates whether we want the pointer to have moved the amount of data we have read after the read is complete
        public byte[] ReadBytes(int Length, bool MovePointer = true)
        {
            if (ReadPointerPosition + Length > ReadBuffer.Length) throw new Exception("ReadBytes Length out of bounds!");
            byte[] Result = new byte[Length];
            Array.Copy(ReadBuffer, ReadPointerPosition, Result, 0, Length);
            if (MovePointer) ReadPointerPosition += Length;
            return Result;
        }

        public byte[] ReadByteArray(bool MovePointer = true)
        {
            int Length = ReadInt();
            if (ReadPointerPosition + Length > ReadBuffer.Length) throw new Exception("ReadBytes Length out of bounds!");
            byte[] Result = new byte[Length];
            Array.Copy(ReadBuffer, ReadPointerPosition, Result, 0, Length);
            if (MovePointer) ReadPointerPosition += Length; 
            else ReadPointerPosition -= 4;
            return Result;
        }

        public int ReadInt(bool MovePointer = true)
        {
            if (ReadPointerPosition + 4 > ReadBuffer.Length) throw new Exception("ReadInt Length out of bounds!");
            int Result = BitConverter.ToInt32(ReadBuffer, ReadPointerPosition);
            if (MovePointer) ReadPointerPosition += 4;
            return Result;
        }

        public Guid ReadGuid(bool MovePointer = true)
        {
            if (ReadPointerPosition + 16 > ReadBuffer.Length) throw new Exception("ReadGuid Length out of bounds!");
            return new Guid(ReadBytes(16, MovePointer));
        }

        public short ReadShort(bool MovePointer = true)
        {
            if (ReadPointerPosition + 2 > ReadBuffer.Length) throw new Exception("ReadShort Length out of bounds!");
            short Result = BitConverter.ToInt16(ReadBuffer, ReadPointerPosition);
            if (MovePointer) ReadPointerPosition += 2;
            return Result;
        }

        public bool ReadBool(bool MovePointer = true)
        {
            if (ReadPointerPosition + 1 > ReadBuffer.Length) throw new Exception("ReadBool Length out of bounds!");
            bool Result = BitConverter.ToBoolean(ReadBuffer, ReadPointerPosition);
            if (MovePointer) ReadPointerPosition += 1;
            return Result;
        }

        public string ReadString(bool MovePointer = true)
        {
            int Length = ReadInt();
            if (ReadPointerPosition + Length > ReadBuffer.Length) throw new Exception("ReadString Length out of bounds!");
            string Result = Encoding.ASCII.GetString(ReadBuffer, ReadPointerPosition, Length);
            if (MovePointer) ReadPointerPosition += Length;
            else ReadPointerPosition -= 4;
            return Result;
        }

        #endregion

        #region Helper Functions

        public int Length()
        {
            return ReadBuffer.Length;
        }

        //Unread number of bytes of the packet
        public int UnreadLength()
        {
            return ReadBuffer.Length - ReadPointerPosition;
        }

        //Inserts the size of the packet in bytes in the front of the TemporaryWriteBuffer
        public void InsertPacketLength()
        {
            TemporaryWriteBuffer.InsertRange(0, BitConverter.GetBytes(TemporaryWriteBuffer.Count + 4));
        }

        //This is used by the server to overwrite the length of the packet (which will be no longer necessary when the packet is fully received) with the ID of the client who sent the packet to the server, which is important for further processing, this method of simply overwriting the beginning of the packet means we don't need to pass around multiple parameter in the server functions other than this packet and makes it quick to insert the sender ID
        public void InsertPacketSenderIDUnsafe(int SenderID)
        {
            byte[] IDToBytes = BitConverter.GetBytes(SenderID);
            ReadBuffer[0] = IDToBytes[0];
            ReadBuffer[1] = IDToBytes[1];
            ReadBuffer[2] = IDToBytes[2];
            ReadBuffer[3] = IDToBytes[3];
        }

        //Saves the write buffer to the final read buffer (which is used for sending/reading packet)
        public byte[] SaveTemporaryBufferToPernamentReadBuffer()
        {
            ReadBuffer = TemporaryWriteBuffer.ToArray();
            return ReadBuffer;
        }

        //Clear the packet to be empty
        public void Reset()
        {
            ReadBuffer = null;
            TemporaryWriteBuffer = new List<byte>();
            ReadPointerPosition = 0;
        }
        #endregion

        #region Disposing

        /*
        private bool Disposed = false;

        //Disposes of the packet, clearing its data
        protected virtual void Dispose(bool Disposing)
        {
            if (!Disposed)
            {
                if (Disposing)
                {
                    ReadBuffer = null;
                    TemporaryWriteBuffer = null;
                    ReadPointerPosition = 0;
                }

                Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
        */
    }

}
# endregion