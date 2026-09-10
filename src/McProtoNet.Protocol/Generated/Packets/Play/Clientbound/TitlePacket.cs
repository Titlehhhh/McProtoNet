using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, 754)]
[Packet("play.toClient.title", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Action", "TitleAction")]
public sealed partial record TitlePacket(TitleAction Action) : IPacket<TitlePacket>, IPacket
{
    public static TitlePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TitlePacket>(protocolVersion);
        var _action = reader.ReadVarInt();
        var action = TitleAction.Read(ref reader, protocolVersion, (int)_action);
        return new TitlePacket(action);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TitlePacket>(protocolVersion);
        writer.WriteVarInt(Action.Discriminator(protocolVersion));
        Action.Write(writer, protocolVersion);
    }

    public static PacketIdentity Identity => new("play.toClient.title", "Title", PacketPhase.Play, PacketDirection.Clientbound, 115);

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
