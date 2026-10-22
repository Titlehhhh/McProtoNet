using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(777, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.add_transient_block", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Location", "Position")]
[PacketField("Type", "int")]
public sealed partial record AddTransientBlockPacket(Position Location, int Type) : IPacket<AddTransientBlockPacket>, IPacket
{
    public static AddTransientBlockPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<AddTransientBlockPacket>(protocolVersion);
        var location = reader.ReadType<Position>(protocolVersion);
        var type = reader.ReadVarInt();
        return new AddTransientBlockPacket(location, type);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<AddTransientBlockPacket>(protocolVersion);
        writer.WriteType<Position>(Location, protocolVersion);
        writer.WriteVarInt(Type);
    }

    public static PacketIdentity Identity => new("play.toClient.add_transient_block", "AddTransientBlock", PacketPhase.Play, PacketDirection.Clientbound, 4);

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
