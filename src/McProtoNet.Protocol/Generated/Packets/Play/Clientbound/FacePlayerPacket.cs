using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.face_player", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("FeetEyes", "int")]
[PacketField("X", "double")]
[PacketField("Y", "double")]
[PacketField("Z", "double")]
[PacketField("Entity", "FacePlayerEntityTarget?")]
public sealed partial record FacePlayerPacket(int FeetEyes, double X, double Y, double Z, FacePlayerEntityTarget? Entity) : IPacket<FacePlayerPacket>, IPacket
{
    public static FacePlayerPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<FacePlayerPacket>(protocolVersion);
        var feetEyes = reader.ReadVarInt();
        var x = reader.ReadDouble();
        var y = reader.ReadDouble();
        var z = reader.ReadDouble();
        FacePlayerEntityTarget? entity = null;
        if (reader.ReadBoolean())
            entity = reader.ReadType<FacePlayerEntityTarget>(protocolVersion);
        return new FacePlayerPacket(feetEyes, x, y, z, entity);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<FacePlayerPacket>(protocolVersion);
        writer.WriteVarInt(FeetEyes);
        writer.WriteDouble(X);
        writer.WriteDouble(Y);
        writer.WriteDouble(Z);
        writer.WriteBoolean(Entity is not null);
        if (Entity is { } entityValue)
            writer.WriteType<FacePlayerEntityTarget>(entityValue, protocolVersion);
    }

    public static PacketIdentity Identity => new("play.toClient.face_player", "FacePlayer", PacketPhase.Play, PacketDirection.Clientbound, 45);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x34;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x33;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x37;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x35;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x38;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x37;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x3B;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x3D;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x3F;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x41;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x40;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x45;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x47;
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
