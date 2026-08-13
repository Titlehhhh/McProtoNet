using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.vehicle_move", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("X", "double")]
[PacketField("Y", "double")]
[PacketField("Z", "double")]
[PacketField("Yaw", "float")]
[PacketField("Pitch", "float")]
[PacketField("OnGround", "bool", Group = "V769_Last", From = 769)]
public sealed partial record VehicleMovePacket(double X, double Y, double Z, float Yaw, float Pitch, VehicleMovePacket.V769_LastLayer? V769_Last = null) : IPacket<VehicleMovePacket>, IPacket
{
    public readonly record struct V769_LastLayer(bool OnGround);
    public static VehicleMovePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<VehicleMovePacket>(protocolVersion);
        if (protocolVersion <= 768)
        {
            var x = reader.ReadDouble();
            var y = reader.ReadDouble();
            var z = reader.ReadDouble();
            var yaw = reader.ReadFloat();
            var pitch = reader.ReadFloat();
            return new VehicleMovePacket(x, y, z, yaw, pitch);
        }

        if (protocolVersion >= 769)
        {
            var x = reader.ReadDouble();
            var y = reader.ReadDouble();
            var z = reader.ReadDouble();
            var yaw = reader.ReadFloat();
            var pitch = reader.ReadFloat();
            var onGround = reader.ReadBoolean();
            return new VehicleMovePacket(x, y, z, yaw, pitch, V769_Last: new V769_LastLayer(onGround));
        }

        throw new System.NotSupportedException($"VehicleMovePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<VehicleMovePacket>(protocolVersion);
        if (protocolVersion <= 768)
        {
            writer.WriteDouble(X);
            writer.WriteDouble(Y);
            writer.WriteDouble(Z);
            writer.WriteFloat(Yaw);
            writer.WriteFloat(Pitch);
            return;
        }

        if (protocolVersion >= 769)
        {
            var layer = V769_Last ?? throw new WrongLayerException("VehicleMovePacket", protocolVersion, "V769_Last");
            bool OnGround = layer.OnGround;
            writer.WriteDouble(X);
            writer.WriteDouble(Y);
            writer.WriteDouble(Z);
            writer.WriteFloat(Yaw);
            writer.WriteFloat(Pitch);
            writer.WriteBoolean(OnGround);
            return;
        }

        throw new System.NotSupportedException($"VehicleMovePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toServer.vehicle_move", "VehicleMove", PacketPhase.Play, PacketDirection.Serverbound, 60);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x16;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x16;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x15;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x17;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x18;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x17;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x18;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x1A;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x1B;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x1E;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 770)
        {
            id = 0x20;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 772)
        {
            id = 0x21;
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
