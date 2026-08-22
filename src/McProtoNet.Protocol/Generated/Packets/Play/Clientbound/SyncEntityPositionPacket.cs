using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.sync_entity_position", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("X", "double")]
[PacketField("Y", "double")]
[PacketField("Z", "double")]
[PacketField("Dx", "double")]
[PacketField("Dy", "double")]
[PacketField("Dz", "double")]
[PacketField("Yaw", "float")]
[PacketField("Pitch", "float")]
[PacketField("OnGround", "bool")]
public sealed partial record SyncEntityPositionPacket(int EntityId, double X, double Y, double Z, double Dx, double Dy, double Dz, float Yaw, float Pitch, bool OnGround) : IPacket<SyncEntityPositionPacket>, IPacket
{
    public static SyncEntityPositionPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SyncEntityPositionPacket>(protocolVersion);
        var entityId = reader.ReadVarInt();
        var x = reader.ReadDouble();
        var y = reader.ReadDouble();
        var z = reader.ReadDouble();
        var dx = reader.ReadDouble();
        var dy = reader.ReadDouble();
        var dz = reader.ReadDouble();
        var yaw = reader.ReadFloat();
        var pitch = reader.ReadFloat();
        var onGround = reader.ReadBoolean();
        return new SyncEntityPositionPacket(entityId, x, y, z, dx, dy, dz, yaw, pitch, onGround);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SyncEntityPositionPacket>(protocolVersion);
        writer.WriteVarInt(EntityId);
        writer.WriteDouble(X);
        writer.WriteDouble(Y);
        writer.WriteDouble(Z);
        writer.WriteDouble(Dx);
        writer.WriteDouble(Dy);
        writer.WriteDouble(Dz);
        writer.WriteFloat(Yaw);
        writer.WriteFloat(Pitch);
        writer.WriteBoolean(OnGround);
    }

    public static PacketIdentity Identity => new("play.toClient.sync_entity_position", "SyncEntityPosition", PacketPhase.Play, PacketDirection.Clientbound, 100);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x20;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x1F;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 776)
        {
            id = 0x23;
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
