using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets
{
    public abstract class RemoveResourcePackPacket : IServerPacket
    {
        public Guid? Uuid { get; set; }

        public sealed class V765_767 : RemoveResourcePackPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Uuid = reader.ReadOptional((ref MinecraftPrimitiveReader r_0) => r_0.ReadUUID());
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 765 and <= 767;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V765_767.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static ServerPacket PacketId => ServerPacket.RemoveResourcePack;

        public ServerPacket GetPacketId() => PacketId;
    }
}