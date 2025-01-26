using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class UpdateViewPositionPacket : IServerPacket
    {
        public int ChunkX { get; set; }
        public int ChunkZ { get; set; }

        public sealed class V477_769 : UpdateViewPositionPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                ChunkX = reader.ReadVarInt();
                ChunkZ = reader.ReadVarInt();
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
        public static PacketIdentifier PacketId => ServerPlayPacket.UpdateViewPosition;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}