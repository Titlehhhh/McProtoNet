using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, 754)]
[Packet("play.toClient.world_border", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Action", "WorldBorderAction")]
public sealed partial record WorldBorderPacket(WorldBorderAction Action) : IPacket<WorldBorderPacket>, IPacket
{
    public static WorldBorderPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<WorldBorderPacket>(protocolVersion);
        var _action = reader.ReadVarInt();
        var action = WorldBorderAction.Read(ref reader, protocolVersion, (int)_action);
        return new WorldBorderPacket(action);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<WorldBorderPacket>(protocolVersion);
        writer.WriteVarInt(Action.Discriminator(protocolVersion));
        Action.Write(writer, protocolVersion);
    }

    public static PacketIdentity Identity => new("play.toClient.world_border", "WorldBorder", PacketPhase.Play, PacketDirection.Clientbound, 128);

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
