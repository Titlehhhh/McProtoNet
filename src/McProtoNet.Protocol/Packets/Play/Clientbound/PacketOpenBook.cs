using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class OpenBookPacket : IServerPacket
    {
        public int Hand { get; set; }

        public sealed class V477_769 : OpenBookPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Hand = reader.ReadVarInt();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 477 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V477_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static PacketIdentifier PacketId => ServerPlayPacket.OpenBook;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}