using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets
{
    public class UpdateStructureBlockPacket : IClientPacket
    {
        public Position Location { get; set; }
        public int Action { get; set; }
        public int Mode { get; set; }
        public string Name { get; set; }
        public sbyte OffsetX { get; set; }
        public sbyte OffsetY { get; set; }
        public sbyte OffsetZ { get; set; }
        public sbyte SizeX { get; set; }
        public sbyte SizeY { get; set; }
        public sbyte SizeZ { get; set; }
        public int Mirror { get; set; }
        public int Rotation { get; set; }
        public string Metadata { get; set; }
        public float Integrity { get; set; }
        public byte Flags { get; set; }

        public sealed class V393_758 : UpdateStructureBlockPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Location, Action, Mode, Name, OffsetX, OffsetY, OffsetZ, SizeX, SizeY, SizeZ, Mirror, Rotation, Metadata, Integrity, Seed, Flags);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, Position location, int action, int mode, string name, sbyte offsetX, sbyte offsetY, sbyte offsetZ, sbyte sizeX, sbyte sizeY, sbyte sizeZ, int mirror, int rotation, string metadata, float integrity, long seed, byte flags)
            {
                writer.WritePosition(location, protocolVersion);
                writer.WriteVarInt(action);
                writer.WriteVarInt(mode);
                writer.WriteString(name);
                writer.WriteSignedByte(offsetX);
                writer.WriteSignedByte(offsetY);
                writer.WriteSignedByte(offsetZ);
                writer.WriteSignedByte(sizeX);
                writer.WriteSignedByte(sizeY);
                writer.WriteSignedByte(sizeZ);
                writer.WriteVarInt(mirror);
                writer.WriteVarInt(rotation);
                writer.WriteString(metadata);
                writer.WriteFloat(integrity);
                writer.WriteVarLong(seed);
                writer.WriteUnsignedByte(flags);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 393 and <= 758;
            }

            public long Seed { get; set; }
        }

        public sealed class V759_769 : UpdateStructureBlockPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Location, Action, Mode, Name, OffsetX, OffsetY, OffsetZ, SizeX, SizeY, SizeZ, Mirror, Rotation, Metadata, Integrity, Seed, Flags);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, Position location, int action, int mode, string name, sbyte offsetX, sbyte offsetY, sbyte offsetZ, sbyte sizeX, sbyte sizeY, sbyte sizeZ, int mirror, int rotation, string metadata, float integrity, int seed, byte flags)
            {
                writer.WritePosition(location, protocolVersion);
                writer.WriteVarInt(action);
                writer.WriteVarInt(mode);
                writer.WriteString(name);
                writer.WriteSignedByte(offsetX);
                writer.WriteSignedByte(offsetY);
                writer.WriteSignedByte(offsetZ);
                writer.WriteSignedByte(sizeX);
                writer.WriteSignedByte(sizeY);
                writer.WriteSignedByte(sizeZ);
                writer.WriteVarInt(mirror);
                writer.WriteVarInt(rotation);
                writer.WriteString(metadata);
                writer.WriteFloat(integrity);
                writer.WriteVarInt(seed);
                writer.WriteUnsignedByte(flags);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 759 and <= 769;
            }

            public int Seed { get; set; }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V393_758.SupportedVersion(protocolVersion) || V759_769.SupportedVersion(protocolVersion);
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V393_758.SupportedVersion(protocolVersion))
                V393_758.SerializeInternal(ref writer, protocolVersion, Location, Action, Mode, Name, OffsetX, OffsetY, OffsetZ, SizeX, SizeY, SizeZ, Mirror, Rotation, Metadata, Integrity, default, Flags);
            else if (V759_769.SupportedVersion(protocolVersion))
                V759_769.SerializeInternal(ref writer, protocolVersion, Location, Action, Mode, Name, OffsetX, OffsetY, OffsetZ, SizeX, SizeY, SizeZ, Mirror, Rotation, Metadata, Integrity, default, Flags);
            else
                throw new ProtocolNotSupportException(nameof(ClientPacket.UpdateStructureBlock), protocolVersion);
        }

        public static ClientPacket PacketId => ClientPacket.UpdateStructureBlock;

        public ClientPacket GetPacketId() => PacketId;
    }
}