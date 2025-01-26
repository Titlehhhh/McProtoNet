using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class CraftProgressBarPacket : IServerPacket
    {
        public short Property { get; set; }
        public short Value { get; set; }

        public sealed class V340_767 : CraftProgressBarPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                WindowId = reader.ReadUnsignedByte();
                Property = reader.ReadSignedShort();
                Value = reader.ReadSignedShort();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 767;
            }

            public byte WindowId { get; set; }
        }

        public sealed class V768_769 : CraftProgressBarPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                WindowId = reader.ReadVarInt();
                Property = reader.ReadSignedShort();
                Value = reader.ReadSignedShort();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 768 and <= 769;
            }

            public int WindowId { get; set; }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_767.SupportedVersion(protocolVersion) || V768_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static PacketIdentifier PacketId => ServerPlayPacket.CraftProgressBar;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}