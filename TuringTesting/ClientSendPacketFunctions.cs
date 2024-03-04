using System;
using System.Text;
using TuringCore.Networking;
using System.Text.Json;
using TuringCore;
using TuringCore.Files;

namespace NetworkTesting
{

    public static class ClientSendPacketFunctions
    {
        public static Packet InvalidShortPacket()
        {
            Packet Data = new Packet();
            return Data;
        }

        public static Packet InvalidRequestPacket()
        {
            Packet Data = new Packet();
            Data.Write(23454325);
            return Data;
        }

        public static Packet UpdateFile(string FileID, int Version, byte[] NewContents)
        {
            Packet Data = new Packet();

            Data.Write((int)ClientSendPackets.UpdateFile);
            Data.Write(Guid.Parse(FileID));
            Data.Write(Version);
            Data.Write(NewContents);

            return Data;
        }
    }
}
