using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.position", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("X", "double")]
[PacketField("Y", "double")]
[PacketField("Z", "double")]
[PacketField("Yaw", "float")]
[PacketField("Pitch", "float")]
[PacketField("Flags", "PositionUpdateRelatives")]
[PacketField("TeleportId", "int")]
[PacketField("DismountVehicle", "bool", Group = "V755_761", From = 755, To = 761)]
[PacketField("Dx", "double", Group = "V768_Last", From = 768)]
[PacketField("Dy", "double", Group = "V768_Last", From = 768)]
[PacketField("Dz", "double", Group = "V768_Last", From = 768)]
public sealed partial record PlayerPositionPacket(double X, double Y, double Z, float Yaw, float Pitch, PositionUpdateRelatives Flags, int TeleportId, PlayerPositionPacket.V755_761Layer? V755_761 = null, PlayerPositionPacket.V768_LastLayer? V768_Last = null) : IPacket<PlayerPositionPacket>, IPacket
{
    public readonly record struct V755_761Layer(bool DismountVehicle);
    public readonly record struct V768_LastLayer(double Dx, double Dy, double Dz);
    public static PlayerPositionPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PlayerPositionPacket>(protocolVersion);
        if (protocolVersion <= 754)
        {
            var x = reader.ReadDouble();
            var y = reader.ReadDouble();
            var z = reader.ReadDouble();
            var yaw = reader.ReadFloat();
            var pitch = reader.ReadFloat();
            var flags = reader.ReadType<PositionUpdateRelatives>(protocolVersion);
            var teleportId = reader.ReadVarInt();
            return new PlayerPositionPacket(x, y, z, yaw, pitch, flags, teleportId);
        }

        if (protocolVersion >= 755 && protocolVersion <= 761)
        {
            var x = reader.ReadDouble();
            var y = reader.ReadDouble();
            var z = reader.ReadDouble();
            var yaw = reader.ReadFloat();
            var pitch = reader.ReadFloat();
            var flags = reader.ReadType<PositionUpdateRelatives>(protocolVersion);
            var teleportId = reader.ReadVarInt();
            var dismountVehicle = reader.ReadBoolean();
            return new PlayerPositionPacket(x, y, z, yaw, pitch, flags, teleportId, V755_761: new V755_761Layer(dismountVehicle));
        }

        if (protocolVersion >= 762 && protocolVersion <= 765)
        {
            var x = reader.ReadDouble();
            var y = reader.ReadDouble();
            var z = reader.ReadDouble();
            var yaw = reader.ReadFloat();
            var pitch = reader.ReadFloat();
            var flags = reader.ReadType<PositionUpdateRelatives>(protocolVersion);
            var teleportId = reader.ReadVarInt();
            return new PlayerPositionPacket(x, y, z, yaw, pitch, flags, teleportId);
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            var x = reader.ReadDouble();
            var y = reader.ReadDouble();
            var z = reader.ReadDouble();
            var yaw = reader.ReadFloat();
            var pitch = reader.ReadFloat();
            var flags = reader.ReadType<PositionUpdateRelatives>(protocolVersion);
            var teleportId = reader.ReadVarInt();
            return new PlayerPositionPacket(x, y, z, yaw, pitch, flags, teleportId);
        }

        if (protocolVersion >= 768)
        {
            var teleportId = reader.ReadVarInt();
            var x = reader.ReadDouble();
            var y = reader.ReadDouble();
            var z = reader.ReadDouble();
            var dx = reader.ReadDouble();
            var dy = reader.ReadDouble();
            var dz = reader.ReadDouble();
            var yaw = reader.ReadFloat();
            var pitch = reader.ReadFloat();
            var flags = reader.ReadType<PositionUpdateRelatives>(protocolVersion);
            return new PlayerPositionPacket(x, y, z, yaw, pitch, flags, teleportId, V768_Last: new V768_LastLayer(dx, dy, dz));
        }

        throw new System.NotSupportedException($"PlayerPositionPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PlayerPositionPacket>(protocolVersion);
        if (protocolVersion <= 754)
        {
            writer.WriteDouble(X);
            writer.WriteDouble(Y);
            writer.WriteDouble(Z);
            writer.WriteFloat(Yaw);
            writer.WriteFloat(Pitch);
            writer.WriteType<PositionUpdateRelatives>(Flags, protocolVersion);
            writer.WriteVarInt(TeleportId);
            return;
        }

        if (protocolVersion >= 755 && protocolVersion <= 761)
        {
            var layer = V755_761 ?? throw new WrongLayerException("PlayerPositionPacket", protocolVersion, "V755_761");
            bool DismountVehicle = layer.DismountVehicle;
            writer.WriteDouble(X);
            writer.WriteDouble(Y);
            writer.WriteDouble(Z);
            writer.WriteFloat(Yaw);
            writer.WriteFloat(Pitch);
            writer.WriteType<PositionUpdateRelatives>(Flags, protocolVersion);
            writer.WriteVarInt(TeleportId);
            writer.WriteBoolean(DismountVehicle);
            return;
        }

        if (protocolVersion >= 762 && protocolVersion <= 765)
        {
            writer.WriteDouble(X);
            writer.WriteDouble(Y);
            writer.WriteDouble(Z);
            writer.WriteFloat(Yaw);
            writer.WriteFloat(Pitch);
            writer.WriteType<PositionUpdateRelatives>(Flags, protocolVersion);
            writer.WriteVarInt(TeleportId);
            return;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            writer.WriteDouble(X);
            writer.WriteDouble(Y);
            writer.WriteDouble(Z);
            writer.WriteFloat(Yaw);
            writer.WriteFloat(Pitch);
            writer.WriteType<PositionUpdateRelatives>(Flags, protocolVersion);
            writer.WriteVarInt(TeleportId);
            return;
        }

        if (protocolVersion >= 768)
        {
            var layer = V768_Last ?? throw new WrongLayerException("PlayerPositionPacket", protocolVersion, "V768_Last");
            double Dx = layer.Dx;
            double Dy = layer.Dy;
            double Dz = layer.Dz;
            writer.WriteVarInt(TeleportId);
            writer.WriteDouble(X);
            writer.WriteDouble(Y);
            writer.WriteDouble(Z);
            writer.WriteDouble(Dx);
            writer.WriteDouble(Dy);
            writer.WriteDouble(Dz);
            writer.WriteFloat(Yaw);
            writer.WriteFloat(Pitch);
            writer.WriteType<PositionUpdateRelatives>(Flags, protocolVersion);
            return;
        }

        throw new System.NotSupportedException($"PlayerPositionPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.position", "PlayerPosition", PacketPhase.Play, PacketDirection.Clientbound, 65);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x35;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x34;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x38;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x36;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x39;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x38;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x3C;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x3E;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x40;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x42;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x41;
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
