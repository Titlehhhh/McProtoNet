using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class PlayerRemovePacket : IServerPacket
    {
        public Guid[] Players { get; set; }

        public sealed class V761_769 : PlayerRemovePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Players = reader.ReadArray(LengthFormat.VarInt, (ref MinecraftPrimitiveReader r_0) => r_0.ReadUUID());
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 761 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V761_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static PacketIdentifier PacketId => ServerPlayPacket.PlayerRemove;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}