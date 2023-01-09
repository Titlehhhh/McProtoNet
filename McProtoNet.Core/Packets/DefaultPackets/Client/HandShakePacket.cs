﻿using McProtoNet.Core.IO;
using McProtoNet.Core.Protocol;

namespace McProtoNet.Core.Packets.DefaultPackets.Client
{
    public sealed class HandShakePacket : IMinecraftPacket
    {
        public HandShakeIntent Intent { get; private set; }
        public int ProtocolVersion { get; private set; }
        public string Host { get; private set; }
        public ushort Port { get; private set; }

        public HandShakePacket(HandShakeIntent handShakeIntent, int protocolVersion, string host, ushort port)
        {
            Intent = handShakeIntent;
            ProtocolVersion = protocolVersion;
            Host = host;
            Port = port;
        }

        public HandShakePacket()
        {

        }

        public void Read(IMinecraftPrimitiveReader stream)
        {
            ProtocolVersion = stream.ReadVarInt();
            Host = stream.ReadString();
            Port = stream.ReadUnsignedShort();
            Intent = (HandShakeIntent)stream.ReadVarInt();
        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteVarInt(ProtocolVersion);
            stream.WriteString(Host);
            stream.WriteUnsignedShort(Port);
            stream.WriteVarInt(Intent);
        }
    }
}