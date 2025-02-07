using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("BlockDig", PacketState.Play, PacketDirection.Serverbound)]
    public partial class BlockDigPacket : IClientPacket
    {
        public int Status { get; set; }
        public Position Location { get; set; }
        public sbyte Face { get; set; }

        [PacketSubInfo(340, 758)]
        public sealed partial class V340_758 : BlockDigPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Status, Location, Face);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int status,
                Position location, sbyte face)
            {
                writer.WriteVarInt(status);
                writer.WritePosition(location, protocolVersion);
                writer.WriteSignedByte(face);
            }
        }

        [PacketSubInfo(759, 769)]
        public sealed partial class V759_769 : BlockDigPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Status, Location, Face, Sequence);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int status,
                Position location, sbyte face, int sequence)
            {
                writer.WriteVarInt(status);
                writer.WritePosition(location, protocolVersion);
                writer.WriteSignedByte(face);
                writer.WriteVarInt(sequence);
            }

            public int Sequence { get; set; }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_758.IsSupportedVersionStatic(protocolVersion))
                V340_758.SerializeInternal(ref writer, protocolVersion, Status, Location, Face);
            else if (V759_769.IsSupportedVersionStatic(protocolVersion))
                V759_769.SerializeInternal(ref writer, protocolVersion, Status, Location, Face, 0);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.BlockDig), protocolVersion);
        }
    }
}