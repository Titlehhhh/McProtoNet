﻿using ProtoLib.API;
using ProtoLib.API.IO;
using ProtoLib.API.Networking;

namespace ProtoLib.API
{
    public sealed class HandShakePacket : IPacket
    {
        public HandShakeIntent Intent { get; set; }
        public int ProtocolVersion { get; set; }
        public ushort Port { get; set; }
        public string Host { get; set; }
        public void Write(IMinecraftStreamWriter stream)
        {
            stream.WriteVarInt(ProtocolVersion);
            stream.WriteString(Host);
            stream.WriteUnsignedShort(Port);
            stream.WriteVarInt((int)Intent);
        }

        public void Read(IMinecraftStreamReader stream)
        {

        }

        public HandShakePacket(HandShakeIntent intent, int protocolVersion, ushort port, string host)
        {
            Intent = intent;
            ProtocolVersion = protocolVersion;
            Port = port;
            Host = host;
        }
    }
}