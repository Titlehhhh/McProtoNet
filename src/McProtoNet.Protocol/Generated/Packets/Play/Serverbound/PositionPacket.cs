using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.position", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("X", "double")]
[PacketField("Y", "double")]
[PacketField("Z", "double")]
[PacketField("OnGround", "bool", Group = "VUntil767", To = 767)]
[PacketField("Flags", "MovementFlags", Group = "V768_Last", From = 768)]
public sealed partial record PositionPacket(double X, double Y, double Z, PositionPacket.VUntil767Layer? VUntil767 = null, PositionPacket.V768_LastLayer? V768_Last = null) : IPacket<PositionPacket>, IPacket
{
    public readonly record struct VUntil767Layer(bool OnGround);
    public readonly record struct V768_LastLayer(MovementFlags Flags);
    public static PositionPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PositionPacket>(protocolVersion);
        if (protocolVersion <= 767)
        {
            var x = reader.ReadDouble();
            var y = reader.ReadDouble();
            var z = reader.ReadDouble();
            var onGround = reader.ReadBoolean();
            return new PositionPacket(x, y, z, VUntil767: new VUntil767Layer(onGround));
        }

        if (protocolVersion >= 768)
        {
            var x = reader.ReadDouble();
            var y = reader.ReadDouble();
            var z = reader.ReadDouble();
            var flags = reader.ReadType<MovementFlags>(protocolVersion);
            return new PositionPacket(x, y, z, V768_Last: new V768_LastLayer(flags));
        }

        throw new System.NotSupportedException($"PositionPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PositionPacket>(protocolVersion);
        if (protocolVersion <= 767)
        {
            var layer = VUntil767 ?? throw new WrongLayerException("PositionPacket", protocolVersion, "VUntil767");
            bool OnGround = layer.OnGround;
            writer.WriteDouble(X);
            writer.WriteDouble(Y);
            writer.WriteDouble(Z);
            writer.WriteBoolean(OnGround);
            return;
        }

        if (protocolVersion >= 768)
        {
            var layer = V768_Last ?? throw new WrongLayerException("PositionPacket", protocolVersion, "V768_Last");
            MovementFlags Flags = layer.Flags;
            writer.WriteDouble(X);
            writer.WriteDouble(Y);
            writer.WriteDouble(Z);
            writer.WriteType<MovementFlags>(Flags, protocolVersion);
            return;
        }

        throw new System.NotSupportedException($"PositionPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toServer.position", "Position", PacketPhase.Play, PacketDirection.Serverbound, 40);

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
