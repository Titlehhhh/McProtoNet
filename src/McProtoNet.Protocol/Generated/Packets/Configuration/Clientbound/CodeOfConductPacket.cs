using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;
[ProtocolSupport(773, MinecraftVersion.LatestProtocol)]
[Packet("configuration.toClient.code_of_conduct", PacketPhase.Configuration, PacketDirection.Clientbound)]
[PacketField("Contents", "string")]
public sealed partial record CodeOfConductPacket(string Contents) : IPacket<CodeOfConductPacket>, IPacket
{
    public static CodeOfConductPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CodeOfConductPacket>(protocolVersion);
        var contents = reader.ReadString();
        return new CodeOfConductPacket(contents);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CodeOfConductPacket>(protocolVersion);
        writer.WriteString(Contents);
    }

    public static PacketIdentity Identity => new("configuration.toClient.code_of_conduct", "CodeOfConduct", PacketPhase.Configuration, PacketDirection.Clientbound, 2);

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
