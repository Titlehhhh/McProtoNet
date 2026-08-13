using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.block_dig", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Status", "int")]
[PacketField("Location", "Position")]
[PacketField("Face", "int")]
[PacketField("Sequence", "int", Group = "V759_Last", From = 759)]
public sealed partial record BlockDigPacket(int Status, Position Location, int Face, BlockDigPacket.V759_LastLayer? V759_Last = null) : IPacket<BlockDigPacket>, IPacket
{
    public readonly record struct V759_LastLayer(int Sequence);
    public static BlockDigPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<BlockDigPacket>(protocolVersion);
        if (protocolVersion <= 758)
        {
            var status = reader.ReadVarInt();
            var location = reader.ReadType<Position>(protocolVersion);
            var face = reader.ReadSignedByte();
            return new BlockDigPacket(status, location, face);
        }

        if (protocolVersion >= 759)
        {
            var status = reader.ReadVarInt();
            var location = reader.ReadType<Position>(protocolVersion);
            var face = reader.ReadSignedByte();
            var sequence = reader.ReadVarInt();
            return new BlockDigPacket(status, location, face, V759_Last: new V759_LastLayer(sequence));
        }

        throw new System.NotSupportedException($"BlockDigPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<BlockDigPacket>(protocolVersion);
        if (protocolVersion <= 758)
        {
            writer.WriteVarInt(Status);
            writer.WriteType<Position>(Location, protocolVersion);
            writer.WriteSignedByte((sbyte)Face);
            return;
        }

        if (protocolVersion >= 759)
        {
            var layer = V759_Last ?? throw new WrongLayerException("BlockDigPacket", protocolVersion, "V759_Last");
            int Sequence = layer.Sequence;
            writer.WriteVarInt(Status);
            writer.WriteType<Position>(Location, protocolVersion);
            writer.WriteSignedByte((sbyte)Face);
            writer.WriteVarInt(Sequence);
            return;
        }

        throw new System.NotSupportedException($"BlockDigPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toServer.block_dig", "BlockDig", PacketPhase.Play, PacketDirection.Serverbound, 2);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x1B;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x1B;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x1A;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x1C;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x1D;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x1C;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x1D;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x20;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x21;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x24;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 768)
        {
            id = 0x26;
            return true;
        }

        if (protocolVersion >= 769 && protocolVersion <= 770)
        {
            id = 0x27;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 772)
        {
            id = 0x28;
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
