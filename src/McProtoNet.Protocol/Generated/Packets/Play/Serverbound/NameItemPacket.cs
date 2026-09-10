using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.name_item", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Name", "string")]
public sealed partial record NameItemPacket(string Name) : IPacket<NameItemPacket>, IPacket
{
    public static NameItemPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<NameItemPacket>(protocolVersion);
        var name = reader.ReadString();
        return new NameItemPacket(name);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<NameItemPacket>(protocolVersion);
        writer.WriteString(Name);
    }

    public static PacketIdentity Identity => new("play.toServer.name_item", "NameItem", PacketPhase.Play, PacketDirection.Serverbound, 32);

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
