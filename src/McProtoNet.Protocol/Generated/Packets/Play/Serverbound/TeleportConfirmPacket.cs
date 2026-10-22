using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.teleport_confirm", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("TeleportId", "int")]
[PacketField("X", "double", Group = "V777_Last", From = 777)]
[PacketField("Y", "double", Group = "V777_Last", From = 777)]
[PacketField("Z", "double", Group = "V777_Last", From = 777)]
[PacketField("YRot", "float", Group = "V777_Last", From = 777)]
[PacketField("XRot", "float", Group = "V777_Last", From = 777)]
public sealed partial record TeleportConfirmPacket(int TeleportId, TeleportConfirmPacket.V777_LastLayer? V777_Last = null) : IPacket<TeleportConfirmPacket>, IPacket
{
    public readonly record struct V777_LastLayer(double X, double Y, double Z, float YRot, float XRot);
    public static TeleportConfirmPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TeleportConfirmPacket>(protocolVersion);
        if (protocolVersion <= 776)
        {
            var teleportId = reader.ReadVarInt();
            return new TeleportConfirmPacket(teleportId);
        }

        if (protocolVersion >= 777)
        {
            var teleportId = reader.ReadVarInt();
            var x = reader.ReadDouble();
            var y = reader.ReadDouble();
            var z = reader.ReadDouble();
            var yRot = reader.ReadFloat();
            var xRot = reader.ReadFloat();
            return new TeleportConfirmPacket(teleportId, V777_Last: new V777_LastLayer(x, y, z, yRot, xRot));
        }

        throw new System.NotSupportedException($"TeleportConfirmPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TeleportConfirmPacket>(protocolVersion);
        if (protocolVersion <= 776)
        {
            writer.WriteVarInt(TeleportId);
            return;
        }

        if (protocolVersion >= 777)
        {
            var layer = V777_Last ?? throw new WrongLayerException("TeleportConfirmPacket", protocolVersion, "V777_Last");
            double X = layer.X;
            double Y = layer.Y;
            double Z = layer.Z;
            float YRot = layer.YRot;
            float XRot = layer.XRot;
            writer.WriteVarInt(TeleportId);
            writer.WriteDouble(X);
            writer.WriteDouble(Y);
            writer.WriteDouble(Z);
            writer.WriteFloat(YRot);
            writer.WriteFloat(XRot);
            return;
        }

        throw new System.NotSupportedException($"TeleportConfirmPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toServer.teleport_confirm", "TeleportConfirm", PacketPhase.Play, PacketDirection.Serverbound, 61);

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
