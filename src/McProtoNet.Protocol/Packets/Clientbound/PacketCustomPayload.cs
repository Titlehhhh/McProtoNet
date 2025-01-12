using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets
{
    public abstract class CustomPayloadPacket : IServerPacket
    {
        public string Channel { get; set; }
        public byte[] Data { get; set; }

        internal sealed class V340_769 : CustomPayloadPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Channel = reader.ReadString();
                Data = reader.ReadRestBuffer();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static ServerPacket PacketId => ServerPacket.CustomPayload;

        public ServerPacket GetPacketId() => PacketId;
    }
}