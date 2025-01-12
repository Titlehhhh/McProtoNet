using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets
{
    public abstract class DestroyEntityPacket : IServerPacket
    {
        public int EntityId { get; set; }

        internal sealed class V755 : DestroyEntityPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion == 755;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V755.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static ServerPacket PacketId => ServerPacket.DestroyEntity;

        public ServerPacket GetPacketId() => PacketId;
    }
}