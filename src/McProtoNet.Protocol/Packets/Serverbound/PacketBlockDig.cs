using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets
{
    public class BlockDigPacket : IClientPacket
    {
        public int Status { get; set; }
        public Position Location { get; set; }
        public sbyte Face { get; set; }

        internal sealed class V340_758 : BlockDigPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Status, Location, Face);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int status, Position location, sbyte face)
            {
                writer.WriteVarInt(status);
                writer.WritePosition(location, protocolVersion);
                writer.WriteSignedByte(face);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 758;
            }
        }

        internal sealed class V759_769 : BlockDigPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Status, Location, Face, Sequence);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int status, Position location, sbyte face, int sequence)
            {
                writer.WriteVarInt(status);
                writer.WritePosition(location, protocolVersion);
                writer.WriteSignedByte(face);
                writer.WriteVarInt(sequence);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 759 and <= 769;
            }

            public int Sequence { get; set; }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_758.SupportedVersion(protocolVersion) || V759_769.SupportedVersion(protocolVersion);
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_758.SupportedVersion(protocolVersion))
                V340_758.SerializeInternal(ref writer, protocolVersion, Status, Location, Face);
            else if (V759_769.SupportedVersion(protocolVersion))
                V759_769.SerializeInternal(ref writer, protocolVersion, Status, Location, Face, default);
            else
                throw new ProtocolNotSupportException(nameof(ClientPacket.BlockDig), protocolVersion);
        }

        public static ClientPacket PacketId => ClientPacket.BlockDig;

        public ClientPacket GetPacketId() => PacketId;
    }
}