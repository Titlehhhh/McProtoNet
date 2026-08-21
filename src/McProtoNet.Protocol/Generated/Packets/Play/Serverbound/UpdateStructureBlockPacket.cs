using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.update_structure_block", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Location", "Position")]
[PacketField("Action", "int")]
[PacketField("Mode", "int")]
[PacketField("Name", "string")]
[PacketField("OffsetX", "int")]
[PacketField("OffsetY", "int")]
[PacketField("OffsetZ", "int")]
[PacketField("SizeX", "int")]
[PacketField("SizeY", "int")]
[PacketField("SizeZ", "int")]
[PacketField("Mirror", "int")]
[PacketField("Rotation", "int")]
[PacketField("Metadata", "string")]
[PacketField("Integrity", "float")]
[PacketField("Seed", "long")]
[PacketField("Flags", "int")]
public sealed partial record UpdateStructureBlockPacket(Position Location, int Action, int Mode, string Name, int OffsetX, int OffsetY, int OffsetZ, int SizeX, int SizeY, int SizeZ, int Mirror, int Rotation, string Metadata, float Integrity, long Seed, int Flags) : IPacket<UpdateStructureBlockPacket>, IPacket
{
    public static UpdateStructureBlockPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateStructureBlockPacket>(protocolVersion);
        if (protocolVersion <= 758)
        {
            var location = reader.ReadType<Position>(protocolVersion);
            var action = reader.ReadVarInt();
            var mode = reader.ReadVarInt();
            var name = reader.ReadString();
            var offsetX = reader.ReadSignedByte();
            var offsetY = reader.ReadSignedByte();
            var offsetZ = reader.ReadSignedByte();
            var sizeX = reader.ReadSignedByte();
            var sizeY = reader.ReadSignedByte();
            var sizeZ = reader.ReadSignedByte();
            var mirror = reader.ReadVarInt();
            var rotation = reader.ReadVarInt();
            var metadata = reader.ReadString();
            var integrity = reader.ReadFloat();
            var seed = reader.ReadVarLong();
            var flags = reader.ReadUnsignedByte();
            return new UpdateStructureBlockPacket(location, action, mode, name, offsetX, offsetY, offsetZ, sizeX, sizeY, sizeZ, mirror, rotation, metadata, integrity, seed, flags);
        }

        if (protocolVersion >= 759)
        {
            var location = reader.ReadType<Position>(protocolVersion);
            var action = reader.ReadVarInt();
            var mode = reader.ReadVarInt();
            var name = reader.ReadString();
            var offsetX = reader.ReadSignedByte();
            var offsetY = reader.ReadSignedByte();
            var offsetZ = reader.ReadSignedByte();
            var sizeX = reader.ReadSignedByte();
            var sizeY = reader.ReadSignedByte();
            var sizeZ = reader.ReadSignedByte();
            var mirror = reader.ReadVarInt();
            var rotation = reader.ReadVarInt();
            var metadata = reader.ReadString();
            var integrity = reader.ReadFloat();
            var seed = reader.ReadVarInt();
            var flags = reader.ReadUnsignedByte();
            return new UpdateStructureBlockPacket(location, action, mode, name, offsetX, offsetY, offsetZ, sizeX, sizeY, sizeZ, mirror, rotation, metadata, integrity, seed, flags);
        }

        throw new System.NotSupportedException($"UpdateStructureBlockPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateStructureBlockPacket>(protocolVersion);
        if (protocolVersion <= 758)
        {
            writer.WriteType<Position>(Location, protocolVersion);
            writer.WriteVarInt(Action);
            writer.WriteVarInt(Mode);
            writer.WriteString(Name);
            writer.WriteSignedByte((sbyte)OffsetX);
            writer.WriteSignedByte((sbyte)OffsetY);
            writer.WriteSignedByte((sbyte)OffsetZ);
            writer.WriteSignedByte((sbyte)SizeX);
            writer.WriteSignedByte((sbyte)SizeY);
            writer.WriteSignedByte((sbyte)SizeZ);
            writer.WriteVarInt(Mirror);
            writer.WriteVarInt(Rotation);
            writer.WriteString(Metadata);
            writer.WriteFloat(Integrity);
            writer.WriteVarLong(Seed);
            writer.WriteUnsignedByte((byte)Flags);
            return;
        }

        if (protocolVersion >= 759)
        {
            writer.WriteType<Position>(Location, protocolVersion);
            writer.WriteVarInt(Action);
            writer.WriteVarInt(Mode);
            writer.WriteString(Name);
            writer.WriteSignedByte((sbyte)OffsetX);
            writer.WriteSignedByte((sbyte)OffsetY);
            writer.WriteSignedByte((sbyte)OffsetZ);
            writer.WriteSignedByte((sbyte)SizeX);
            writer.WriteSignedByte((sbyte)SizeY);
            writer.WriteSignedByte((sbyte)SizeZ);
            writer.WriteVarInt(Mirror);
            writer.WriteVarInt(Rotation);
            writer.WriteString(Metadata);
            writer.WriteFloat(Integrity);
            writer.WriteVarInt((int)Seed);
            writer.WriteUnsignedByte((byte)Flags);
            return;
        }

        throw new System.NotSupportedException($"UpdateStructureBlockPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toServer.update_structure_block", "UpdateStructureBlock", PacketPhase.Play, PacketDirection.Serverbound, 61);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x29;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 758)
        {
            id = 0x2A;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x2C;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 763)
        {
            id = 0x2D;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x30;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x31;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x34;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 768)
        {
            id = 0x36;
            return true;
        }

        if (protocolVersion >= 769 && protocolVersion <= 770)
        {
            id = 0x38;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 774)
        {
            id = 0x39;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x3B;
            return true;
        }

        id = 0;
        return false;
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (TryGetPacketId(protocolVersion, out var id))
            return id;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
