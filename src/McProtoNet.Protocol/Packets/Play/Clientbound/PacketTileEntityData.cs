using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class TileEntityDataPacket : IServerPacket
    {
        public Position Location { get; set; }
        public NbtTag? NbtData { get; set; }

        public sealed class V340_756 : TileEntityDataPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Location = reader.ReadPosition(protocolVersion);
                Action = reader.ReadUnsignedByte();
                NbtData = reader.ReadOptionalNbtTag(true);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 756;
            }

            public byte Action { get; set; }
        }

        public sealed class V757_763 : TileEntityDataPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Location = reader.ReadPosition(protocolVersion);
                Action = reader.ReadVarInt();
                NbtData = reader.ReadOptionalNbtTag(true);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 757 and <= 763;
            }

            public int Action { get; set; }
        }

        public sealed class V764_769 : TileEntityDataPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Location = reader.ReadPosition(protocolVersion);
                Action = reader.ReadVarInt();
                NbtData = reader.ReadOptionalNbtTag(false);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 764 and <= 769;
            }

            public int Action { get; set; }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_756.SupportedVersion(protocolVersion) || V757_763.SupportedVersion(protocolVersion) ||
                   V764_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static PacketIdentifier PacketId => ServerPlayPacket.TileEntityData;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}