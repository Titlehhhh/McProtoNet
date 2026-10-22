using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.sync_entity_position", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("Yaw", "float")]
[PacketField("Pitch", "float")]
[PacketField("OnGround", "bool")]
[PacketField("X", "double", Group = "V768_776", From = 768, To = 776)]
[PacketField("Y", "double", Group = "V768_776", From = 768, To = 776)]
[PacketField("Z", "double", Group = "V768_776", From = 768, To = 776)]
[PacketField("Dx", "double", Group = "V768_776", From = 768, To = 776)]
[PacketField("Dy", "double", Group = "V768_776", From = 768, To = 776)]
[PacketField("Dz", "double", Group = "V768_776", From = 768, To = 776)]
[PacketField("Position", "SyncEntityPositionKind", Group = "V777_Last", From = 777)]
public sealed partial record SyncEntityPositionPacket(int EntityId, float Yaw, float Pitch, bool OnGround, SyncEntityPositionPacket.V768_776Layer? V768_776 = null, SyncEntityPositionPacket.V777_LastLayer? V777_Last = null) : IPacket<SyncEntityPositionPacket>, IPacket
{
    public readonly record struct V768_776Layer(double X, double Y, double Z, double Dx, double Dy, double Dz);
    public readonly record struct V777_LastLayer(SyncEntityPositionKind Position);
    public static SyncEntityPositionPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SyncEntityPositionPacket>(protocolVersion);
        if (protocolVersion >= 768 && protocolVersion <= 776)
        {
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
            return new SyncEntityPositionPacket(entityId, yaw, pitch, onGround, V768_776: new V768_776Layer(x, y, z, dx, dy, dz));
        }

        if (protocolVersion >= 777)
        {
            var entityId = reader.ReadVarInt();
            var _positionType = reader.ReadVarInt();
            var position = SyncEntityPositionKind.Read(ref reader, protocolVersion, (int)_positionType);
            var yaw = reader.ReadFloat();
            var pitch = reader.ReadFloat();
            var onGround = reader.ReadBoolean();
            return new SyncEntityPositionPacket(entityId, yaw, pitch, onGround, V777_Last: new V777_LastLayer(position));
        }

        throw new System.NotSupportedException($"SyncEntityPositionPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SyncEntityPositionPacket>(protocolVersion);
        if (protocolVersion >= 768 && protocolVersion <= 776)
        {
            var layer = V768_776 ?? throw new WrongLayerException("SyncEntityPositionPacket", protocolVersion, "V768_776");
            double X = layer.X;
            double Y = layer.Y;
            double Z = layer.Z;
            double Dx = layer.Dx;
            double Dy = layer.Dy;
            double Dz = layer.Dz;
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
            return;
        }

        if (protocolVersion >= 777)
        {
            var layer = V777_Last ?? throw new WrongLayerException("SyncEntityPositionPacket", protocolVersion, "V777_Last");
            SyncEntityPositionKind Position = layer.Position;
            writer.WriteVarInt(EntityId);
            writer.WriteVarInt(Position.Discriminator(protocolVersion));
            Position.Write(writer, protocolVersion);
            writer.WriteFloat(Yaw);
            writer.WriteFloat(Pitch);
            writer.WriteBoolean(OnGround);
            return;
        }

        throw new System.NotSupportedException($"SyncEntityPositionPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.sync_entity_position", "SyncEntityPosition", PacketPhase.Play, PacketDirection.Clientbound, 114);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        return PacketRegistry.TryGetId(Identity, protocolVersion, out id);
    }

    public static int GetPacketId(int protocolVersion)
    {
        return PacketRegistry.GetId(Identity, protocolVersion);
    }
}
